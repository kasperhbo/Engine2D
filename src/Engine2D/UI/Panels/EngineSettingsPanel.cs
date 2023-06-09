﻿#region

using ImGuiNET;

#endregion

namespace Engine2D.UI;

internal class EngineSettingsPanel : UIElement
{
    private UIElement _uiElementImplementation;
    private bool showRestartBar;

    internal EngineSettingsPanel(string title) : base(title)
    {
        Flags = ImGuiWindowFlags.None;
    }


    internal override void Render()
    {
        ImGui.Columns(2);


        ImGui.NextColumn();
        ImGui.Text("Fonst Scale");
        ImGui.NextColumn();
        if (ImGui.DragFloat("##fontscale", ref EngineSettings.DefaultFontSize)) showRestartBar = true;
        if (showRestartBar)
        {
            ImGui.Begin("##restartwindow", ImGuiWindowFlags.NoTitleBar
                                           | ImGuiWindowFlags.NoMove
                                           | ImGuiWindowFlags.NoDecoration
                                           | ImGuiWindowFlags.NoResize
                                           | ImGuiWindowFlags.AlwaysUseWindowPadding
                                           | ImGuiWindowFlags.NoScrollbar);
            ImGui.Columns(2);
            ImGui.Text("YOU NEED TO RESTART THE EDITOR BEFORE CHANGES APPLY");
            ImGui.NextColumn();
            if (ImGui.Button("RESTART"))
                //TODO: MAKE EDITOR 
                Console.WriteLine(AppDomain.CurrentDomain.FriendlyName);
            //Engine.Get().Close();
            ImGui.End();
        }
    }
}