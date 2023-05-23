using System.Diagnostics;
using System.Runtime.InteropServices;
using Engine2D.Core;
using ImGuiNET;
using KDBEngine.UI;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.UI;

public enum FileType
{
    Folder,
    File,
    Script,
    Shader,
    Scene,
    Sprite,
    Texture,
    TextFile
}

internal class AssetBrowser : UIElemenet
{
    public DirectoryInfo CurrentDirectory { get; private set; } 
    
    private static GCHandle? _currentlyDraggedHandle;

    private List<ImageTextIcon> _currentFiles = new();

    private List<ImageTextIcon> _currentFolders = new();

    private bool _currentlyDragging;
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
    }

    public static float ThumbnailSize { get; } = 128;
    public static bool DisplayAssetType { get; } = true;

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

    private void SwitchDirectory(string newDir)
    {
        _currentFolders = new List<ImageTextIcon>();
        _currentFiles = new List<ImageTextIcon>();
        CurrentDirectory = new DirectoryInfo(newDir);

        GetDirectories();
        GetFiles();
    }

    private void GetDirectories()
    {
        var dirs = CurrentDirectory.GetDirectories();
        foreach (var dir in dirs)
        {
            var icon = new ImageTextIcon(dir.Name, dirTexture, dirTexture, dirTexture, dir.FullName, FileType.Folder);
            _currentFolders.Add(icon);
        }
    }

    private void GetFiles()
    {
        var files = CurrentDirectory.GetFiles();
        foreach (var file in files)
        {
            ImageTextIcon icon = null;
            if (file.Extension == ".cs" || file.Extension == ".csproj")
                icon = new ImageTextIcon(file.Name, fileTexture, fileTexture, fileTexture, file.FullName,
                    FileType.Script);
            if (file.Extension == ".kdbscene")
                icon = new ImageTextIcon(file.Name, sceneTexture, sceneTexture, sceneTexture, file.FullName,
                    FileType.Scene);

            if (icon != null)
                _currentFiles.Add(icon);
        }
    }


    protected override string SetWindowTitle()
    {
        return "Asset Browser";
    }

    protected override ImGuiWindowFlags SetWindowFlags()
    {
        return ImGuiWindowFlags.MenuBar;
    }

    protected override Action SetWindowContent()
    {
        return () =>
        {
            var currentlyDragging = false;
            if (CurrentDirectory.FullName != ProjectSettings.s_FullProjectPath)
            {
                ImGui.BeginMenuBar();
                if (ImGui.MenuItem("Back")) SwitchDirectory(CurrentDirectory.Parent.FullName);
                ImGui.Text(CurrentDirectory.FullName);
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
            foreach (var file in _currentFiles) file.IsSelected = false;

            foreach (var folder in _currentFolders)
            {
                if (folder == currentSelected) folder.IsSelected = true;

                folder.Draw(out var doublec, out var single, out var rightClick);
                if (doublec) SwitchDirectory(folder.Path);
                if (single) currentSelected = folder;


                ImGui.NextColumn();
            }

            foreach (var file in _currentFiles)
            {
                if (file == currentSelected) file.IsSelected = true;

                file.Draw(out var doublec, out var single, out var rightClick);

                if (doublec)
                    new Process
                    {
                        StartInfo = new ProcessStartInfo(file.Path)
                        {
                            UseShellExecute = true
                        }
                    }.Start();
                if (single) currentSelected = file;

                ImGui.NextColumn();
            }


            ImGui.Columns(1);
        };
    }

    internal bool CurrentDirContainsFile(string fileName)
    {
        foreach (var file in _currentFiles)
        {
            if (file.Label == fileName)
            {
                return true;
            }
        }

        return false;
    }
    
}