using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

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
    private ImageTextIcon? currentSelected = null;
    
    private IntPtr dirTexture;
    private IntPtr fileTexture;
    private IntPtr sceneTexture;

    private List<string> _spritesIndDirectory = new List<string>();
    private List<string> _texturesInDirector = new List<string>();
    
    internal AssetBrowser()
    {
        LoadIcons();
        SwitchDirectory(ProjectSettings.FullProjectPath + "\\Assets");
    }

    public static float ThumbnailSize { get; } = 128;
    public static bool DisplayAssetType { get; } = true;

    public void Reload()
    {
        SwitchDirectory(_currentDirectory.FullName);
    }
    
    public override void OnFileDrop(FileDropEventArgs args)
    {
        base.OnFileDrop(args);
        foreach (var file in FileUtils.GetFileInfos(args.FileNames))
        {
            switch (file.Extension)
            {
                case ".png":
                    Texture texture = new Texture(file.FullName, false, TextureMinFilter.Linear,
                        TextureMagFilter.Linear);
                    SaveLoad.SaveTexture(
                        texture,
                        _currentDirectory
                    );
                    break;
                default:
                    Log.Error(string.Format("{0} is not an supported file format", file.Extension));
                    break;

            }
        }

        Reload();
    }
    private void LoadIcons()
    {
        dirTexture = (IntPtr)(new Texture(Utils.GetBaseEngineDir() + "\\Images\\icons\\directoryIcon.png",
            true,
            TextureMinFilter.Linear, TextureMagFilter.Linear).TexID);
        
        fileTexture = (IntPtr)(new Texture(Utils.GetBaseEngineDir() + "\\Images\\icons\\fileicon.png", true,
            TextureMinFilter.Linear, TextureMagFilter.Linear).TexID);
        
        sceneTexture = (IntPtr)(new Texture(Utils.GetBaseEngineDir() + "\\Images\\icons\\mapIcon.png", true,
            TextureMinFilter.Linear, TextureMagFilter.Linear).TexID);
        
    }

    private void SwitchDirectory(string newDir)
    {
        _spritesIndDirectory = new List<string>();
        _texturesInDirector =  new List<string>();
        
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
                
                _spritesIndDirectory.Add(file.FullName);
            }

            if (file.Extension == ".texture")
            {
                icon = new ImageTextIcon(file.Name, sceneTexture, sceneTexture, sceneTexture, file.FullName,
                    FileType.Texture);
                
                _texturesInDirector.Add(file.FullName);
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
            if (_currentDirectory.FullName != (ProjectSettings.FullProjectPath + "\\Assets"))
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

            ImTool.Widgets.IconButton("icon", "text", new Vector4(255, 0, 255, 255));
            
            foreach (var folder in _currentFolders) folder.IsSelected = false;
            foreach (var file in _currentFiles) file.IsSelected = false;

            foreach (var folder in _currentFolders)
            {
                if (folder == currentSelected) folder.IsSelected = true;

                folder.Draw(out var doublec, out var single, out var rightClick, out var deleted);
                if (doublec) SwitchDirectory(folder.Path);
                if (single) currentSelected = folder;
                
                if(deleted)
                {
                    Reload();
                    return;
                }

                ImGui.NextColumn();
            }

            foreach (var file in _currentFiles)
            {
                if (file == currentSelected) file.IsSelected = true;

                file.Draw(out var doublec, out var single, out var rightClick, out var deleted);
                if(deleted)
                {
                    Reload();
                    return;
                };
                
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
                    if (currentSelected == null)
                    {
                        SelectItem(file);
                    }

                    else if (currentSelected.Path != file.Path)
                    {
                        SelectItem(file);
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

    private void SelectItem(ImageTextIcon file)
    {
        currentSelected = file;
                        
        Log.Message("Selected " + file.Path);
                        
        if (file.FileType == FileType.Sprite)
        {
            Sprite? sprite = SaveLoad.LoadSpriteFromJson(file.Path);
            if (sprite == null)
            {
                return;
            }
            Engine.Get().CurrentSelectedAsset = sprite;
        }

        if (file.FileType == FileType.Texture)
        {
            {
                Texture? texture = SaveLoad.LoadTextureFromJson(file.Path);
                if (texture == null)
                {
                    return;
                }
                Engine.Get().CurrentSelectedAsset = texture;
            }
        }
    }

    private void CreateSprite()
    {
        Sprite sprite = new Sprite();
        SaveLoad.SaveSprite(sprite, _currentDirectory); 
    }
    
}


internal class ImageTextIcon
{
    private static System.Numerics.Vector4 _defaultCol = new(1f, 0f, .0f, 1);

    private readonly string _label;
    private readonly IntPtr _texture;
    private readonly IntPtr _textureHovered;
    private GCHandle? _currentlyDraggedHandle;
    private bool _currentlyDragging;
    private IntPtr _textureActive;

    public readonly FileType FileType;
    public readonly string Path;

    public bool IsSelected = false;

    public ImageTextIcon(string label, IntPtr texture, IntPtr textureHovered, IntPtr textureActive, string path,
        FileType fileType)
    {
        _label = label;
        _texture = texture;
        _textureHovered = textureHovered;
        _textureActive = textureActive;
        FileType = fileType;
        Path = path;
    }



    public unsafe void Draw(out bool doubleClick, out bool singleClick, out bool rightClick, out bool deleted)
    {
        doubleClick = false;
        singleClick = false;
        rightClick = false;
        deleted = false;


        //TODO: MOVE TO EDITOR SETTINGS
        var thumbnailSize = AssetBrowser.ThumbnailSize;
        var displayAssetType = AssetBrowser.DisplayAssetType;

        ImGui.PushID(Path);

        if (!IsSelected)
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new System.Numerics.Vector4(.19f, .19f, .19f, 1)); //For visibility
        else
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new System.Numerics.Vector4(1f, .19f, .19f, 1));

        ImGui.BeginChild("##transform_c", new Vector2(thumbnailSize + 32, thumbnailSize + 32), false,
            ImGuiWindowFlags.NoScrollbar); // Leave ~100
        ImGui.PopStyleColor();
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));

        float infoPanelHeight = 64;

        var topLeft = ImGui.GetCursorScreenPos();
        Vector2 bottomRight = new(topLeft.X - 14 + thumbnailSize, topLeft.Y + thumbnailSize + infoPanelHeight);
        
        // Fill background
        //----------------

        if (ImGui.IsItemHovered())
            ImGui.GetWindowDrawList().AddRectFilled(topLeft, bottomRight, (int)ImGuiCol.ButtonHovered, 6.0f);

        // Thumbnail
        //==========
        // TODO: replace with actual Asset Thumbnail interface

        if (ImGui.InvisibleButton("##thumbnailButton", new Vector2(thumbnailSize, thumbnailSize))) singleClick = true;
        OpenTKUIHelper.DrawButtonImage(
            _texture, _textureHovered, _texture,
            OpenTKUIHelper.RectExpanded(
                OpenTKUIHelper.GetItemRect(),
                -6,
                -6
            )
        );

        if (FileType == FileType.Scene)
        {
            if (ImGui.BeginDragDropSource())
            {
                _currentlyDragging = true;
                _currentlyDraggedHandle ??= GCHandle.Alloc(Path);

                ImGui.SetDragDropPayload("Scene_Drop", GCHandle.ToIntPtr(_currentlyDraggedHandle.Value),
                    (uint)sizeof(IntPtr));

                ImGui.EndDragDropSource();
            }
        }

        if (FileType == FileType.Sprite)
        {
            if (ImGui.BeginDragDropSource())
            {
                _currentlyDragging = true;
                _currentlyDraggedHandle ??= GCHandle.Alloc(Path);

                ImGui.SetDragDropPayload("Sprite_Drop", GCHandle.ToIntPtr(_currentlyDraggedHandle.Value),
                    (uint)sizeof(IntPtr));

                ImGui.EndDragDropSource();
            }
        }

        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
            if (ImGui.IsItemHovered())
                doubleClick = true;
        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            if (ImGui.IsItemHovered())
                singleClick = true;


        ImGui.Text(_label);
        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
            if (ImGui.IsItemHovered())
                doubleClick = true;
        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            if (ImGui.IsItemHovered())
                singleClick = true;

        if (ImGui.BeginPopupContextWindow("test"))
        {
            if (ImGui.MenuItem("Show In Explorer"))
            {
                var proc = Process.Start("explorer.exe", "/select, " + Path);
            }

            if (ImGui.MenuItem("Delete"))
            {
                //TODO: MAKE SURE WINDOW
                Log.Error("Create make sure window!!!!");
                deleted = true;
                File.Delete(Path);
            }

            ImGui.EndPopup();
        }


        // End of the Item Group
        //======================
        ImGui.PopStyleVar(); // ItemSpacing

        ImGui.EndChild();
    }
}


