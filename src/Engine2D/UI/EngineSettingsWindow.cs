using Engine2D.SavingLoading;
using ImGuiNET;
using KDBEngine.UI;

namespace Engine2D.UI;

internal class EngineSettingsWindow : UIElemenet
{
    private bool showRestartBar;

    internal EngineSettingsWindow()
    {
        _flags = ImGuiWindowFlags.None;
        Title = "Engine Settings";

        SetVisibility(false);

        _windowContents = () =>
        {
            if (ImGui.Button("Close"))
            {
                SaveLoad.SaveEngineSettings();
                SetVisibility(false);
            }

            ImGui.Columns(2);
            ImGui.Text("Global Scale");
            ImGui.NextColumn();
            ImGui.DragFloat("##globslscale", ref EngineSettings.GlobalScale);

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
        };
    }

    public override void SetVisibility(bool visibility)
    {
        base.SetVisibility(visibility);
        if (visibility) showRestartBar = false;
    }
}