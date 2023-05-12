using Engine2D.Core;
using ImGuiNET;
using KDBEngine.UI;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.UI;

public struct DragDropFile
{
    public string FileName;
}

internal class AssetBrowser : UIElemenet
{
    public static string CurrentDraggingFileName = "";

    private DirectoryInfo _currentDirectory;

    private List<ImageTextIcon> _currentFolders = new();
    private ImageTextIcon currentSelected;
    private IntPtr dirTexture;
    private IntPtr fileTexture;
    private IntPtr sceneTexture;

    private TextureData texDataDir;

    private TextureData texDataFile;

    private TextureData texDataScene;

    internal AssetBrowser()
    {
        LoadIcons();
        SwitchDirectory(ProjectSettings.s_FullProjectPath);

        _flags = ImGuiWindowFlags.MenuBar;
        Title = "Asset Browser";
        _windowContents = () =>
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
            var cellSize = ThumbnailSize + padding;
            var panelWidth = ImGui.GetContentRegionAvail().X;
            var columnCount = (int)(panelWidth / cellSize);
            if (columnCount < 1)
                columnCount = 1;

            ImGui.Columns(columnCount, "0", false);

            foreach (var folder in _currentFolders) folder.IsSelected = false;

            foreach (var folder in _currentFolders)
            {
                //if (folder.Draw())
                //{
                //    folder.IsSelected = true;
                //    //SwitchDirectory(folder.Path);
                //}
                if (folder == currentSelected) folder.IsSelected = true;
                //Console.WriteLine(folder.IsSelected);
                folder.Draw(out var doublec, out var single);
                if (doublec) SwitchDirectory(folder.Path);
                if (single) currentSelected = folder;


                ImGui.NextColumn();
            }


            ImGui.Columns(1);
        };
    }

    public static float ThumbnailSize { get; } = 128;
    public static bool DisplayAssetType { get; } = true;

    private void SwitchDirectory(string newDir)
    {
        _currentFolders = new List<ImageTextIcon>();

        _currentDirectory = new DirectoryInfo(newDir);

        GetDirectories();
    }

    private void GetDirectories()
    {
        var dirs = _currentDirectory.GetDirectories();
        foreach (var dir in dirs)
        {
            var icon = new ImageTextIcon(dir.Name, dirTexture, fileTexture, sceneTexture, dir.FullName);
            _currentFolders.Add(icon);
        }
    }

    private void LoadIcons()
    {
        texDataDir = new TextureData(Utils.GetBaseEngineDir() + "\\icons\\directoryIcon.png", false,
            TextureMinFilter.Linear, TextureMagFilter.Linear);
        texDataFile = new TextureData(Utils.GetBaseEngineDir() + "\\icons\\fileicon.png", false,
            TextureMinFilter.Linear, TextureMagFilter.Linear);
        texDataScene = new TextureData(Utils.GetBaseEngineDir() + "\\icons\\mapIcon.png", false,
            TextureMinFilter.Linear, TextureMagFilter.Linear);

        dirTexture = (IntPtr)ResourceManager.GetTexture(texDataDir).TexID;
        fileTexture = (IntPtr)ResourceManager.GetTexture(texDataFile).TexID;
        sceneTexture = (IntPtr)ResourceManager.GetTexture(texDataScene).TexID;
    }
}