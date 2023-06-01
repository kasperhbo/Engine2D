using System.Diagnostics;
using System.Runtime.InteropServices;
using Engine2D.Core;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using ImGuiNET;
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

internal class AssetBrowser : UiElemenet
{
    private DirectoryInfo _currentDirectory;
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

    private Dictionary<string, Sprite> _spritesIndDirectory = new Dictionary<string, Sprite>();


    internal AssetBrowser()
    {
        LoadIcons();
        SwitchDirectory(ProjectSettings.FullProjectPath);
    }

    public static float ThumbnailSize { get; } = 128;
    public static bool DisplayAssetType { get; } = true;

    private void LoadIcons()
    {
        texDataDir = new TextureData(Utils.GetBaseEngineDir() + "\\Images\\icons\\directoryIcon.png", false,
            TextureMinFilter.Linear, TextureMagFilter.Linear);
        texDataFile = new TextureData(Utils.GetBaseEngineDir() + "\\Images\\icons\\fileicon.png", false,
            TextureMinFilter.Linear, TextureMagFilter.Linear);
        texDataScene = new TextureData(Utils.GetBaseEngineDir() + "\\Images\\icons\\mapIcon.png", false,
            TextureMinFilter.Linear, TextureMagFilter.Linear);

        dirTexture = (IntPtr)ResourceManager.GetTexture(texDataDir).TexID;
        fileTexture = (IntPtr)ResourceManager.GetTexture(texDataFile).TexID;
        sceneTexture = (IntPtr)ResourceManager.GetTexture(texDataScene).TexID;
    }

    private void SwitchDirectory(string newDir)
    {
        _spritesIndDirectory = new Dictionary<string, Sprite>();
        
        _currentFolders = new List<ImageTextIcon>();
        _currentFiles = new List<ImageTextIcon>();
        _currentDirectory = new DirectoryInfo(newDir);

        GetDirectories();
        GetFiles();
    }

    private void GetDirectories()
    {
        var dirs = _currentDirectory.GetDirectories();
        foreach (var dir in dirs)
        {
            var icon = new ImageTextIcon(dir.Name, dirTexture, dirTexture, dirTexture, dir.FullName, FileType.Folder);
            _currentFolders.Add(icon);
        }
    }

    private void GetFiles()
    {
        var files = _currentDirectory.GetFiles();
        foreach (var file in files)
        {
            ImageTextIcon icon = null;
            if (file.Extension == ".cs" || file.Extension == ".csproj")
                icon = new ImageTextIcon(file.Name, fileTexture, fileTexture, fileTexture, file.FullName,
                    FileType.Script);
            if (file.Extension == ".sprite")
            {
                icon = new ImageTextIcon(file.Name, sceneTexture, sceneTexture, sceneTexture, file.FullName,
                    FileType.Sprite);
                
                _spritesIndDirectory.Add(file.FullName, SaveLoad.LoadSpriteFromJson(file.FullName));
            }
            
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
            if (_currentDirectory.FullName != ProjectSettings.FullProjectPath)
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
                
                if (single)
                {
                    currentSelected = file;

                    if (file.FileType == FileType.Sprite)
                    {
                        Sprite sprite = _spritesIndDirectory[file.Path];
                        sprite.FullSavePath = file.Path;
                        
                        Engine.Get().CurrentSelectedAsset = sprite;
                    }
                };

                ImGui.NextColumn();
            }

            ImGui.Columns(1);

            if (ImGui.BeginPopupContextWindow("test"))
            {
                if (ImGui.MenuItem("Create Sprite"))
                {
                    CreateSprite();
                }
                ImGui.EndPopup();
            }
        };
    }

    private void CreateSprite()
    {
        TextureData data = new TextureData()
        {
            flipped = false,
            magFilter = TextureMagFilter.Nearest,
            minFilter = TextureMinFilter.Nearest,
            texturePath = ProjectSettings.FullProjectPath + "\\Images\\testImage.png",
        };
        

        Sprite sprite = new Sprite(data);
        SaveLoad.SaveSprite(sprite, _currentDirectory); 
    }
}