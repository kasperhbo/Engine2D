using Engine2D.Core;
using Engine2D.Rendering;
using Engine2D.Scenes;
using ImGuiNET;
using KDBEngine.Core;
using KDBEngine.UI;
using Newtonsoft.Json;
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

            IntPtr dirTexture =   (IntPtr)ResourceManager.GetTexture(texDataDir).TexID;
            IntPtr fileTexture =  (IntPtr)ResourceManager.GetTexture(texDataFile).TexID;
            IntPtr sceneTexture = (IntPtr)ResourceManager.GetTexture(texDataScene).TexID;

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


                    for (int i = 0; i < 100; i++)
                    {
                        //OpenTKUIHelper.IconWithText(i.ToString(), i, dirTexture, new OpenTK.Mathematics.Vector2(56,56));
                        //ImGui.PushID(i);
                        //ImGui.ImageButton(i.ToString(), dirTexture, new System.Numerics.Vector2(64, 64));
                        //ImGui.AlignTextToFramePadding();
                        //ImGui.Text("test");
                        //ImGui.PopID();
                        //ImGui.NextColumn();
                    }
                }
            };
        }



       
    }
}

