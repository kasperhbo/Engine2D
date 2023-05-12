using Engine2D.Core;
using Engine2D.Rendering;
using Engine2D.Scenes;
using Engine2D.Testing;
using ImGuiNET;
using KDBEngine.Core;
using KDBEngine.UI;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.AccessControl;

namespace Engine2D.UI
{
    public struct DragDropFile
    {
        
        public string FileName;
    }

    internal class AssetBrowser : UIElemenet
    {
        public static string CurrentDraggingFileName = "";

        public static float ThumbnailSize   {get; private set;}= 128;
        public static bool DisplayAssetType { get; private set; } = true;

        private DirectoryInfo _currentDirectory     = null;

        private List<ImageTextIcon> _currentFolders = new List<ImageTextIcon>();
        ImageTextIcon currentSelected = null;

        private void SwitchDirectory(string newDir)
        {
            _currentFolders = new();

            _currentDirectory = new DirectoryInfo(newDir);

            GetDirectories();
        }

        TextureData texDataDir; 
        IntPtr      dirTexture;
        
        TextureData texDataFile;
        IntPtr fileTexture;

        TextureData texDataScene;
        IntPtr sceneTexture;

        private void GetDirectories()
        {            
            var dirs = _currentDirectory.GetDirectories();
            foreach (var dir in dirs)
            {
                ImageTextIcon icon = new ImageTextIcon(dir.Name, dirTexture,fileTexture, sceneTexture, dir.FullName);
                _currentFolders.Add(icon);
            }
        }

        private void LoadIcons()
        {

            texDataDir = new TextureData(Utils.GetBaseEngineDir() + "\\icons\\directoryIcon.png", false, TextureMinFilter.Linear, TextureMagFilter.Linear);
            texDataFile = new TextureData(Utils.GetBaseEngineDir() + "\\icons\\fileicon.png", false, TextureMinFilter.Linear, TextureMagFilter.Linear);
            texDataScene = new TextureData(Utils.GetBaseEngineDir() + "\\icons\\mapIcon.png", false, TextureMinFilter.Linear, TextureMagFilter.Linear);

            dirTexture = (IntPtr)ResourceManager.GetTexture(texDataDir).TexID;
            fileTexture = (IntPtr)ResourceManager.GetTexture(texDataFile).TexID;
            sceneTexture = (IntPtr)ResourceManager.GetTexture(texDataScene).TexID;

        }

        internal unsafe AssetBrowser()
        {
            LoadIcons();
            SwitchDirectory(ProjectSettings.s_FullProjectPath);

            this._flags = ImGuiWindowFlags.MenuBar;
            this.Title = "Asset Browser";
            this._windowContents = () =>
            {     
                if (_currentDirectory.FullName != ProjectSettings.s_FullProjectPath)
                {
                    ImGui.BeginMenuBar();
                    if (ImGui.MenuItem("Back")) SwitchDirectory(_currentDirectory.Parent.FullName);
                    ImGui.Text(_currentDirectory.FullName);
                    ImGui.EndMenuBar();
                }

                //Thanks @The Cherno
                float padding = 16;
                float cellSize = ThumbnailSize + padding;
                float panelWidth = ImGui.GetContentRegionAvail().X;
                int columnCount = (int)(panelWidth / cellSize);
                if (columnCount < 1)
                    columnCount = 1;

                ImGuiNET.ImGui.Columns(columnCount, "0", false);

                foreach (var folder in _currentFolders)
                {
                    folder.IsSelected = false;
                }

                foreach (var folder in _currentFolders)
                {
                    //if (folder.Draw())
                    //{
                    //    folder.IsSelected = true;
                    //    //SwitchDirectory(folder.Path);
                    //}
                    if (folder == currentSelected) folder.IsSelected = true;
                    //Console.WriteLine(folder.IsSelected);
                    folder.Draw(out bool doublec, out bool single);
                    if (doublec) SwitchDirectory(folder.Path);
                    if (single) currentSelected = folder;

                    

                    ImGui.NextColumn();
                }


                ImGui.Columns(1);

            };
        }



       
    }
}

