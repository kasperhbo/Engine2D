using Engine2D.Core;
using Engine2D.Rendering;
using ImGuiNET;
using KDBEngine.Core;
using KDBEngine.UI;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.UI
{
    public struct DragDropFile
    {
        public string FileName;
    }

    internal class AssetBrowser : UIElemenet
    {
        public static string CurrentDraggingFileName = "";

        private string _currentDirectory = ProjectSettings.s_FullProjectPath;
        

        private string _errorMessage;
        private string _newSceneName;

        int _dirTexture = new Texture(Utils.GetBaseEngineDir() + "\\icons\\directoryicon.png", false, TextureMinFilter.Linear, TextureMagFilter.Linear).TexID;
        int _fileTexture = new Texture(Utils.GetBaseEngineDir() + "\\icons\\fileicon.png", false, TextureMinFilter.Linear, TextureMagFilter.Linear).TexID;
        int _sceneTexture = new Texture(Utils.GetBaseEngineDir() + "\\icons\\mapIcon.png", false, TextureMinFilter.Linear, TextureMagFilter.Linear).TexID;

        internal unsafe AssetBrowser()
        {            
            this._flags = ImGuiWindowFlags.None;
            this.Title = "Asset Browser";
            this._windowContents = () =>
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(_currentDirectory);
                bool createNewScene = false;

                if (ImGui.BeginPopupContextWindow("p"))
                {
                    if (ImGui.MenuItem("New Scene"))
                    {
                        createNewScene = true;
                    }
                    ImGui.EndPopup();
                }

                if (createNewScene)
                {
                    ImGui.OpenPopup("New Scene");
                }

                if (ImGui.BeginPopupModal("New Scene"))
                {


                    ImGui.Text("Name: ");
                    ImGui.SameLine();
                    ImGui.InputText("", ref _newSceneName, 256);
                    string newSceneFull = _currentDirectory + "\\" + _newSceneName + ".kdbscene";

                    if (ImGui.Button("OK"))
                    {
                        bool canCreate = true;
                        foreach (var f in Directory.GetFiles(_currentDirectory))
                        {
                            if (f == newSceneFull)
                            {
                                canCreate = false;
                                _errorMessage = "Already Scene With Same Name " + newSceneFull;
                                ImGui.OpenPopup("Error");
                            }
                        }

                        if (canCreate)
                        {
                            Engine.Get().NewScene(newSceneFull);
                            ImGui.CloseCurrentPopup();
                        }
                    }

                    if (ImGui.BeginPopupModal("Error"))
                    {
                        ImGui.Text(_errorMessage);
                        if (ImGui.Button("OK"))
                        {
                            ImGui.CloseCurrentPopup();
                        }

                        ImGui.EndPopup();
                    }
                    ImGui.EndPopup();
                }

                {
                    if (_currentDirectory != ProjectSettings.s_FullProjectPath)
                    {
                        if (ImGui.Button("Back"))
                        {
                            _currentDirectory = directoryInfo!.Parent!.FullName;
                        }
                    }

                    float padding = 16;
                    float thumbnailSize = 128.0f/2;
                    float cellSize = thumbnailSize + padding;
                    float panelWidth = ImGui.GetContentRegionAvail().X;
                    int columnCount = (int)(panelWidth / cellSize);
                    if (columnCount < 1)
                        columnCount = 1;
                    ImGui.Columns(columnCount, "0", false);

                    foreach (var dir in directoryInfo.GetDirectories())
                    {
                        ImGui.PushID(dir.Name);
                        if (ImGui.ImageButton(dir.Name, (IntPtr)_dirTexture, new System.Numerics.Vector2(128/2, 128/2)))
                        {
                            _currentDirectory += "\\" + dir.Name;
                        }
                        ImGui.Text(dir.Name);
                        ImGui.NextColumn();
                        ImGui.PopID();
                    }

                    foreach (var file in directoryInfo.GetFiles())
                    {
                        ImGui.PushID(file.Name);
                        int tex = _fileTexture;
                        if (file.Extension == ".kdbscene")
                        {
                            tex = _sceneTexture;
                        }
                       ImGui.ImageButton(file.Name, (IntPtr)tex, new System.Numerics.Vector2(128/2, 128/2));

                        if (file.Extension == ".kdbscene")
                        {
                            //TODO: MAKE THIS WITH POINTERS ETC
                            if (ImGui.BeginDragDropSource())
                            {
                                CurrentDraggingFileName = file.FullName;

                                ImGui.SetDragDropPayload("SCENE_DRAG_DROP", IntPtr.Zero, 0);
                                ImGui.Text(CurrentDraggingFileName);
                                ImGui.EndDragDropSource();
                            }
                            
                        }


                        ImGui.Text(file.Name);
                        ImGui.NextColumn();
                        ImGui.PopID();
                    }
                }
            };
        }

        //public static UIElemenet Create()
        //{
        //    new UIElemenet("Asset Browser", ImGuiNET.ImGuiWindowFlags.None, () =>
        //    {
        //       
        //    });     
        //}
    }
}
