﻿using System;
using System.Collections.Generic;
using Dalamud.Configuration;
using FFLogsViewer.Model;
using Newtonsoft.Json;

namespace FFLogsViewer;

[Serializable]
public class Configuration : IPluginConfiguration
{
    [JsonIgnore]
    public const int CurrentConfigVersion = 1;
    public int Version { get; set; } = CurrentConfigVersion;
    public string? ClientId { get; set; } = string.Empty;
    public string? ClientSecret { get; set; } = string.Empty;
    public bool ContextMenu { get; set; } = true;
    public bool ContextMenuStreamer { get; set; }
    public bool OpenInBrowser { get; set; }
    public string? ContextMenuButtonName { get; set; } = "Search FF Logs";
    public bool HideInCombat { get; set; }
    public bool IsDefaultLayout { get; set; } = true;
    public bool IsHistoricalDefault { get; set; } = true;
    public bool IsEncounterLayout { get; set; } = true;
    public bool IsCachingEnabled { get; set; } = true;
    public int NbOfDecimalDigits { get; set; }
    public StatType? DefaultStatTypePartyView { get; set; }
    public LayoutEntry? DefaultEncounterPartyView { get; set; }
    public List<LayoutEntry> Layout { get; set; } = new();
    public List<Stat> Stats { get; set; } = new();
    public Metric Metric { get; set; } = new() { Name = "rDPS", InternalName = "rdps" };
    public Style Style { get; set; } = new();
    public OpenWith OpenWith { get; set; } = new();
    public bool IsUpdateDismissed2100 { get; set; } = false;

    public void Save()
    {
        Service.Interface.SavePluginConfig(this);
    }

    public void Initialize()
    {
        this.ClientId ??= string.Empty;
        this.ClientSecret ??= string.Empty;
        this.ContextMenuButtonName ??= string.Empty;

        if (this.IsDefaultLayout || this.Layout.Count == 0)
        {
            this.SetDefaultLayout();
            this.IsDefaultLayout = true;
        }

        if (this.Stats.Count == 0)
        {
            this.Stats.AddRange(GetDefaultStats());
        }

        this.Upgrade();
    }

    public void Upgrade()
    {
        // all stars stats
        if (this.Version == 0)
        {
            var defaultStats = GetDefaultStats();
            if (this.Stats.Count < defaultStats.Count)
            {
                for (var i = this.Stats.Count; i < defaultStats.Count; i++)
                {
                    this.Stats.Add(defaultStats[i]);
                }
            }

            this.Version++;
            this.Save();
        }
    }

    public void SetDefaultLayout()
    {
        this.Layout = GetDefaultLayout();
    }

    private static List<LayoutEntry> GetDefaultLayout()
    {
        return new List<LayoutEntry>
        {
            new() { Type = LayoutEntryType.Header, Alias = "Abyssos", Expansion = "-", Zone = "-", Encounter = "-", Difficulty = "-" },
            new() { Type = LayoutEntryType.Encounter, Expansion = "Endwalker", Zone = "Abyssos", ZoneId = 49, Encounter = "Proto-Carbuncle", EncounterId = 83, Difficulty = "Savage", DifficultyId = 101 },
            new() { Type = LayoutEntryType.Encounter, Expansion = "Endwalker", Zone = "Abyssos", ZoneId = 49, Encounter = "Hegemone", EncounterId = 84, Difficulty = "Savage", DifficultyId = 101 },
            new() { Type = LayoutEntryType.Encounter, Expansion = "Endwalker", Zone = "Abyssos", ZoneId = 49, Encounter = "Agdistis", EncounterId = 85, Difficulty = "Savage", DifficultyId = 101 },
            new() { Type = LayoutEntryType.Encounter, Expansion = "Endwalker", Zone = "Abyssos", ZoneId = 49, Encounter = "Hephaistos", EncounterId = 86, Difficulty = "Savage", DifficultyId = 101 },
            new() { Type = LayoutEntryType.Encounter, Expansion = "Endwalker", Zone = "Abyssos", ZoneId = 49, Encounter = "Hephaistos II", EncounterId = 87, Difficulty = "Savage", DifficultyId = 101 },
            new() { Type = LayoutEntryType.Header, Alias = "Ultimates (EW)", Expansion = "-", Zone = "-", Encounter = "-", Difficulty = "-" },
            new() { Type = LayoutEntryType.Encounter, Expansion = "Endwalker", Zone = "Ultimates (Legacy)", ZoneId = 43, Encounter = "The Unending Coil of Bahamut", EncounterId = 1060, Difficulty = "Normal", DifficultyId = 100, Alias = "UCoB" },
            new() { Type = LayoutEntryType.Encounter, Expansion = "Endwalker", Zone = "Ultimates (Legacy)", ZoneId = 43, Encounter = "The Weapon's Refrain", EncounterId = 1061, Difficulty = "Normal", DifficultyId = 100, Alias = "UwU" },
            new() { Type = LayoutEntryType.Encounter, Expansion = "Endwalker", Zone = "Ultimates (Legacy)", ZoneId = 43, Encounter = "The Epic of Alexander", EncounterId = 1062, Difficulty = "Normal", DifficultyId = 100, Alias = "TEA" },
            new() { Type = LayoutEntryType.Encounter, Expansion = "Endwalker", Zone = "Dragonsong's Reprise", ZoneId = 45, Encounter = "Dragonsong's Reprise", EncounterId = 1065, Difficulty = "Normal", DifficultyId = 100, Alias = "DSR" },
            new() { Type = LayoutEntryType.Encounter, Expansion = "Endwalker", Zone = "The Omega Protocol", ZoneId = 53, Encounter = "The Omega Protocol", EncounterId = 1068, Difficulty = "Normal", DifficultyId = 100, Alias = "TOP" },
            new() { Type = LayoutEntryType.Header, Alias = "Trials (Extreme)", Expansion = "-", Zone = "-", Encounter = "-", Difficulty = "-" },
            new() { Type = LayoutEntryType.Encounter, Expansion = "Endwalker", Zone = "Trials I (Extreme)", ZoneId = 42, Encounter = "Barbariccia", EncounterId = 1066, Difficulty = "Normal", DifficultyId = 100 },
            new() { Type = LayoutEntryType.Encounter, Expansion = "Endwalker", Zone = "Trials II (Extreme)", ZoneId = 50, Encounter = "Rubicante", EncounterId = 1067, Difficulty = "Normal", DifficultyId = 100 },
        };
    }

    private static List<Stat> GetDefaultStats()
    {
        return new List<Stat>
        {
            new() { Name = "Best", Type = StatType.Best, IsEnabled = true },
            new() { Alias = "Med.", Name = "Median", Type = StatType.Median, IsEnabled = true },
            new() { Name = "Kills", Type = StatType.Kills, IsEnabled = true },
            new() { Name = "Fastest", Type = StatType.Fastest, IsEnabled = false },
            new() { Alias = "/metric/", Name = "Best Metric", Type = StatType.BestAmount, IsEnabled = false },
            new() { Name = "Job", Type = StatType.Job, IsEnabled = true },
            new() { Name = "Best Job", Type = StatType.BestJob, IsEnabled = false },
            new() { Alias = "ASP", Name = "All Stars Points", Type = StatType.AllStarsPoints, IsEnabled = false },
            new() { Alias = "ASP R", Name = "All Stars Rank", Type = StatType.AllStarsRank, IsEnabled = false },
            new() { Alias = "ASP R%", Name = "All Stars Rank %", Type = StatType.AllStarsRankPercent, IsEnabled = false },
        };
    }
}
