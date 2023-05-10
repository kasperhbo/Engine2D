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


        internal unsafe AssetBrowser()
        {
            TextureData texDataDir = new TextureData(Utils.GetBaseEngineDir() + "\\icons\\directoryicon.png", false, TextureMinFilter.Linear, TextureMagFilter.Linear);
            TextureData texDataFile = new TextureData(Utils.GetBaseEngineDir() + "\\icons\\fileicon.png", false, TextureMinFilter.Linear, TextureMagFilter.Linear);
            TextureData texDataScene = new TextureData(Utils.GetBaseEngineDir() + "\\icons\\mapIcon.png", false, TextureMinFilter.Linear, TextureMagFilter.Linear);

            int dirTexture = ResourceManager.GetTexture(texDataDir).TexID;
            int fileTexture = ResourceManager.GetTexture(texDataFile).TexID;
            int sceneTexture = ResourceManager.GetTexture(texDataScene).TexID;

            this._flags = ImGuiWindowFlags.MenuBar;
            this.Title = "Asset Browser";
            this._windowContents = () =>
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(_currentDirectory);
                {
                    if (_currentDirectory != ProjectSettings.s_FullProjectPath)
                    {
                        ImGui.BeginMenuBar();
                        if (ImGui.MenuItem("Back")) _currentDirectory = directoryInfo!.Parent!.FullName;
                        ImGui.EndMenuBar();
                    }

                    //Thanks @The Cherno
                    float padding = 16;
                    float thumbnailSize = 128.0f / 2;
                    float cellSize = thumbnailSize + padding;
                    float panelWidth = ImGui.GetContentRegionAvail().X;
                    int columnCount = (int)(panelWidth / cellSize);
                    if (columnCount < 1)
                        columnCount = 1;

                    ImGui.Columns(columnCount, "0", false);

                    foreach (var dir in directoryInfo.GetDirectories())
                    {
                        CreateAssetBrowserItem(dir.Name, dirTexture, actionOnButtonClick: () => { _currentDirectory += "\\" + dir.Name; });
                    }

                    foreach (var file in directoryInfo.GetFiles())
                    {
                        ImGui.PushID(file.Name);
                        int tex = fileTexture;

                        if (file.Extension == ".kdbscene")
                        {
                            tex = sceneTexture;
                        }
                        CreateAssetBrowserItem(file.Name, tex, OtherActions: () => OnSceneDragAndClick(file.FullName));


                    }
                }
            };

        }

        private void OnSceneDragAndClick(string file)
        {
            //TODO: MAKE THIS WITH POINTERS ETC
            if (ImGui.BeginDragDropSource())
            {
                CurrentDraggingFileName = file;

                ImGui.SetDragDropPayload("SCENE_DRAG_DROP", IntPtr.Zero, 0);
                ImGui.Text(CurrentDraggingFileName);
                ImGui.EndDragDropSource();
            }
        }


        private void CreateAssetBrowserItem(string name, int texture, Action? actionOnButtonClick = null, Action? OtherActions = null)
        {
            ImGui.PushID(name);
            if (ImGui.ImageButton(name, (IntPtr)texture, new System.Numerics.Vector2(128 / 2, 128 / 2)))
            {                
                   actionOnButtonClick?.Invoke();
            }

            
            OtherActions?.Invoke();

            ImGui.Text(name);
            ImGui.NextColumn();
            ImGui.PopID();
        }
    }
}

