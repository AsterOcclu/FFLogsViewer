﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Logging;
using FFLogsViewer.Manager;
using FFLogsViewer.Model;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json.Linq;

namespace FFLogsViewer;

public class CharData
{
    public Metric? LoadedMetric;
    public CharacterError? CharError;
    public string FirstName = string.Empty;
    public string LastName = string.Empty;
    public string WorldName = string.Empty;
    public string RegionName = string.Empty;
    public string LoadedFirstName = string.Empty;
    public string LoadedLastName = string.Empty;
    public string LoadedWorldName = string.Empty;
    public uint JobId; // only used in party view
    public volatile bool IsDataLoading;
    public volatile bool IsDataReady;

    public string Abbreviation
    {
        get
        {
            if (this.FirstName == string.Empty || this.LastName == string.Empty)
            {
                return "-";
            }

            return $"{this.FirstName[0]}. {this.LastName[0]}.";
        }
    }

    public List<Encounter> Encounters = new();

    public CharData(string? firstName = null, string? lastName = null, string? worldName = null, uint? jobId = null)
    {
        if (firstName != null)
        {
            this.FirstName = firstName;
        }

        if (lastName != null)
        {
            this.LastName = lastName;
        }

        if (worldName != null)
        {
            this.WorldName = worldName;
        }

        if (jobId != null)
        {
            this.JobId = (uint)jobId;
        }
    }

    public void SetInfo(string firstName, string lastName, string worldName)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.WorldName = worldName;
    }

    public bool SetInfo(PlayerCharacter playerCharacter)
    {
        if (playerCharacter.HomeWorld.GameData?.Name == null)
        {
            this.CharError = CharacterError.GenericError;
            PluginLog.Error("SetInfo character world was null");
            return false;
        }

        this.FirstName = playerCharacter.Name.TextValue.Split(' ')[0];
        this.LastName = playerCharacter.Name.TextValue.Split(' ')[1];
        this.WorldName = playerCharacter.HomeWorld.GameData.Name.ToString();
        return true;
    }

    public bool IsInfoSet()
    {
        return this.FirstName != string.Empty && this.LastName != string.Empty && this.WorldName != string.Empty;
    }

    public void FetchTargetChar()
    {
        var target = Service.TargetManager.Target;
        if (target is PlayerCharacter targetCharacter && target.ObjectKind != ObjectKind.Companion)
        {
            if (this.SetInfo(targetCharacter))
            {
                this.FetchLogs();
            }
        }
        else
        {
            this.CharError = CharacterError.InvalidTarget;
        }
    }

    public void FetchLogs()
    {
        if (this.IsDataLoading)
        {
            return;
        }

        this.CharError = null;

        if (!this.IsInfoSet())
        {
            this.CharError = CharacterError.MissingInputs;
            return;
        }

        var regionName = CharDataManager.GetRegionName(this.WorldName);
        if (regionName == null)
        {
            this.CharError = CharacterError.InvalidWorld;
            return;
        }

        this.RegionName = regionName;

        this.IsDataLoading = true;
        this.ResetData();
        Task.Run(async () =>
        {
            var rawData = await Service.FFLogsClient.FetchLogs(this).ConfigureAwait(false);
            if (rawData == null)
            {
                this.IsDataLoading = false;
                Service.FFLogsClient.InvalidateCache(this);
                this.CharError = CharacterError.Unreachable;
                PluginLog.Error("rawData is null");
                return;
            }

            if (rawData.data?.characterData?.character == null)
            {
                this.IsDataLoading = false;

                if (rawData.error != null)
                {
                    if (rawData.error == "Unauthenticated.")
                    {
                        this.CharError = CharacterError.Unauthenticated;
                        Service.FFLogsClient.InvalidateCache(this);
                        PluginLog.Information($"Unauthenticated: {rawData}");
                        return;
                    }

                    if (rawData.status != null && rawData.status == 429)
                    {
                        this.CharError = CharacterError.OutOfPoints;
                        Service.FFLogsClient.InvalidateCache(this);
                        PluginLog.Information($"Ran out of points: {rawData}");
                        return;
                    }

                    this.CharError = CharacterError.GenericError;
                    Service.FFLogsClient.InvalidateCache(this);
                    PluginLog.Information($"Generic error: {rawData}");
                    return;
                }

                if (rawData.errors != null)
                {
                    this.CharError = CharacterError.MalformedQuery;
                    Service.FFLogsClient.InvalidateCache(this);
                    PluginLog.Information($"Malformed GraphQL query: {rawData}");
                    return;
                }

                this.CharError = CharacterError.CharacterNotFoundFFLogs;
                return;
            }

            var character = rawData.data.characterData.character;

            if (character.hidden == "true")
            {
                this.IsDataLoading = false;
                this.CharError = CharacterError.HiddenLogs;
                return;
            }

            this.Encounters = new List<Encounter>();

            var properties = character.Properties();
            foreach (var prop in properties)
            {
                if (prop.Name != "hidden")
                {
                    this.ParseZone(prop.Value);
                }
            }

            this.IsDataReady = true;
            this.LoadedFirstName = this.FirstName;
            this.LoadedLastName = this.LastName;
            this.LoadedWorldName = this.WorldName;
        }).ContinueWith(t =>
        {
            this.IsDataLoading = false;
            if (!t.IsFaulted) return;
            if (t.Exception == null) return;
            this.CharError = CharacterError.NetworkError;
            foreach (var e in t.Exception.Flatten().InnerExceptions)
            {
                PluginLog.Error(e, "Networking error");
            }
        });
    }

    public bool ParseTextForChar(string rawText)
    {
        var character = new CharData();
        var placeholder = CharDataManager.FindPlaceholder(rawText);
        if (placeholder != null)
        {
            rawText = placeholder;
        }

        rawText = rawText.Replace("'s party for", " ");
        rawText = rawText.Replace("You join", " ");
        rawText = Regex.Replace(rawText, "\\[.*?\\]", " ");
        rawText = Regex.Replace(rawText, "[^A-Za-z '-]", " ");
        rawText = string.Concat(rawText.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        rawText = Regex.Replace(rawText, @"\s+", " ");

        var words = rawText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        var index = -1;
        for (var i = 0; index == -1 && i < Service.CharDataManager.ValidWorlds.Length; i++) index = Array.IndexOf(words, Service.CharDataManager.ValidWorlds[i]);

        if (index - 2 >= 0)
        {
            character.FirstName = words[index - 2];
            character.LastName = words[index - 1];
            character.WorldName = words[index];
        }
        else if (words.Length >= 2)
        {
            if (Service.ClientState.LocalPlayer?.HomeWorld.GameData?.Name == null)
            {
                return false;
            }

            character.FirstName = words[0];
            character.LastName = words[1];
            character.WorldName = Service.ClientState.LocalPlayer.HomeWorld.GameData.Name.ToString();
        }
        else
        {
            return false;
        }

        if (!char.IsUpper(character.FirstName[0]) || !char.IsUpper(character.LastName[0]))
        {
            return false;
        }

        this.FirstName = character.FirstName;
        this.LastName = character.LastName;
        this.WorldName = character.WorldName;

        return true;
    }

    public void FetchClipboardCharacter()
    {
        string clipboardRawText;
        try
        {
            if (ImGui.GetClipboardText() == null)
            {
                this.CharError = CharacterError.ClipboardError;
                return;
            }

            clipboardRawText = ImGui.GetClipboardText();
        }
        catch
        {
            this.CharError = CharacterError.ClipboardError;
            return;
        }

        this.FetchCharacter(clipboardRawText);
    }

    public void FetchCharacter(string text)
    {
        Service.MainWindow.IsPartyView = false;

        if (!this.ParseTextForChar(text))
        {
            this.CharError = CharacterError.CharacterNotFound;
            return;
        }

        this.FetchLogs();
    }

    public void FetchCharacter(string fullName, ushort worldId)
    {
        var world = Service.DataManager.GetExcelSheet<World>()
                           ?.FirstOrDefault(x => x.RowId == worldId);

        if (world == null)
        {
            this.CharError = CharacterError.WorldNotFound;
            return;
        }

        var playerName = $"{fullName}@{world.Name}";
        this.FetchCharacter(playerName);
    }

    public void ResetData()
    {
        this.Encounters = new List<Encounter>();
        this.IsDataReady = false;
        this.LoadedMetric = null;
    }

    private void ParseZone(dynamic zone)
    {
        if (zone.rankings == null)
        {
            return;
        }

        // metric not valid for this zone
        if (zone.rankings.Count == 0)
        {
            this.Encounters.Add(new Encounter { ZoneId = zone.zone, IsValid = false });
        }

        foreach (var ranking in zone.rankings)
        {
            if (ranking.encounter == null)
            {
                continue;
            }

            var encounter = new Encounter
            {
                ZoneId = zone.zone,
                Id = ranking.encounter.id,
                Difficulty = zone.difficulty,
                Metric = zone.metric,
            };

            if (ranking.spec != null)
            {
                encounter.IsLockedIn = ranking.lockedIn;
                encounter.Best = ranking.rankPercent;
                encounter.Median = ranking.medianPercent;
                encounter.Kills = ranking.totalKills;
                encounter.Fastest = ranking.fastestKill;
                encounter.BestAmount = ranking.bestAmount;
                var jobName = Regex.Replace(ranking.spec.ToString(), "([a-z])([A-Z])", "$1 $2");
                encounter.Job = Service.GameDataManager.Jobs.FirstOrDefault(job => job.Name == jobName);
                var bestJobName = Regex.Replace(ranking.bestSpec.ToString(), "([a-z])([A-Z])", "$1 $2");
                encounter.BestJob = Service.GameDataManager.Jobs.FirstOrDefault(job => job.Name == bestJobName);
                var allStars = ranking.allStars;
                if (allStars != null)
                {
                    encounter.AllStarsPoints = allStars.points;

                    // both "-" if fresh log
                    if (allStars.rank.Type != JTokenType.String && allStars.rankPercent.Type != JTokenType.String)
                    {
                        encounter.AllStarsRank = allStars.rank;
                        encounter.AllStarsRankPercent = allStars.rankPercent;
                    }
                }
            }

            this.Encounters.Add(encounter);
        }
    }
}
