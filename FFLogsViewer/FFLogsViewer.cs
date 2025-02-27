﻿using Dalamud.ContextMenu;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using FFLogsViewer.GUI.Config;
using FFLogsViewer.GUI.Main;
using FFLogsViewer.Manager;

namespace FFLogsViewer;

// ReSharper disable once UnusedType.Global
public sealed class FFLogsViewer : IDalamudPlugin
{
    public string Name => "FFLogsViewer";

    private readonly WindowSystem windowSystem;
    private readonly ContextMenu contextMenu;

    public FFLogsViewer(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();

        Service.Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Service.Configuration.Initialize();

        IPC.Initialize();

        Service.Commands = new Commands();
        Service.GameDataManager = new GameDataManager();
        Service.CharDataManager = new CharDataManager();
        Service.TeamManager = new TeamManager();
        Service.OpenWithManager = new OpenWithManager();
        Service.FFLogsClient = new FFLogsClient();

        Service.MainWindow = new MainWindow();
        Service.ConfigWindow = new ConfigWindow();
        this.windowSystem = new WindowSystem("FFLogsViewer");
        this.windowSystem.AddWindow(Service.ConfigWindow);
        this.windowSystem.AddWindow(Service.MainWindow);

        Service.ContextMenu = new DalamudContextMenu();
        this.contextMenu = new ContextMenu();

        Service.Interface.UiBuilder.OpenConfigUi += OpenConfigUi;
        Service.Interface.UiBuilder.Draw += this.windowSystem.Draw;
    }

    public void Dispose()
    {
        IPC.Dispose();
        Commands.Dispose();
        Service.ContextMenu.Dispose();
        this.contextMenu.Dispose();
        Service.GameDataManager.Dispose();
        Service.OpenWithManager.Dispose();

        Service.Interface.UiBuilder.OpenConfigUi -= OpenConfigUi;
        Service.Interface.UiBuilder.Draw -= this.windowSystem.Draw;
    }

    private static void OpenConfigUi()
    {
        Service.ConfigWindow.IsOpen = true;
    }
}
