using Engine2D.Core;
using Engine2D.SavingLoading;
using ImGuiNET;
using KDBEngine.Core;

namespace Engine2D.UI;

public static class MainMenuUI
{
    
    public static void OnGui()
    {
        bool openNewScenePopup = false;
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Menu"))
                {

                    if (ImGui.MenuItem("Save Scene")) 
                        SaveLoad.SaveScene(Engine.Get()._currentScene);
                    if (ImGui.MenuItem("New Scene"))
                    {
                        openNewScenePopup = true;
                    }

                    if (ImGui.MenuItem("Load Scene"))
                    {

                    }
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Help"))
                {
                    if (ImGui.MenuItem("Website")) Utils.TryOpenUrl("https://github.com/kasperhbo/Engine2D");
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Settings"))
                {
                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }

            OpenNewScenePopup(openNewScenePopup);
    }

    
    static string newSceneName = "";
    private static string errorText = "";
    private static void OpenNewScenePopup(bool openNewScene)
    {
        if(openNewScene)
        {
            newSceneName = "";
            errorText = "";
            ImGui.OpenPopup("NewScene");
        }
        
        if (ImGui.BeginPopupModal("NewScene"))
        {
            OpenTKUIHelper.BeginPopup();
            OpenTKUIHelper.DrawProperty("Scene Name: ", ref newSceneName);
            if (errorText != "")
            {
                ImGui.Text(errorText);
            }
            if (ImGui.Button("OK"))
            {
                if (newSceneName == "")
                {
                    errorText = "Scene name can't be empty!";
                }

                if (Engine.Get().AssetBrowser.CurrentDirContainsFile(newSceneName + ".kdbscene"))
                {
                    errorText = "Already file with same name in current directory!";
                }
                else
                {
                    Engine.Get().NewScene(newSceneName);
                    OpenTKUIHelper.EndPopup();
                    ImGui.CloseCurrentPopup();
                }
            }
            ImGui.SameLine();
            if (ImGui.Button("Cancel"))
            {
                OpenTKUIHelper.EndPopup();
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }
    }
    
}