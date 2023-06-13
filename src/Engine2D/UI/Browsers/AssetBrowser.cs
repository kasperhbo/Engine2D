using System.Numerics;
using System.Runtime.InteropServices;
using Engine2D.Core;
using Engine2D.Rendering;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.UI.Browsers;

public enum ESupportedFileTypes
{
    folder, 
    png,
    kdbscene ,
    sprite   ,
    texture  ,
    txt
}

public class AssetBrowser : UiElemenet
{
    private static readonly string BaseAssetDir = ProjectSettings.FullProjectPath + "\\Assets";
    
    private DirectoryInfo _currentDirectory = new DirectoryInfo(BaseAssetDir);
    
    private Texture _folderTexture = null!;
    private Texture _fileTexture = null!;

    private List<DirectoryInfo> _directoriesInDirectory = new List<DirectoryInfo>();
    private List<FileInfo> _filesInDirectory = new List<FileInfo>();
    private List<AssetBrowserEntry> _entries = new List<AssetBrowserEntry>();


    public Vector2 Padding = new(-1,-2);
    public Vector2 ImageSize = new(90);
    public Vector2 ImageAdjust = new(0,11);
    
    private Vector2 _sizeBetweenItems = new Vector2(15);

    private AssetBrowserEntry? _currentSelectedEntry = null;

    public AssetBrowser() : base()
    {
        // ImageSize = new(ImGui.GetFrameHeight() * 5.0f * 1);
        Init();
        Init();
    }
    
    protected override string GetWindowTitle()
    {
        return "New Asset Browser";
    }

    protected override ImGuiWindowFlags GetWindowFlags()
    {
        return ImGuiWindowFlags.None | ImGuiWindowFlags.MenuBar;
    }

    protected override Action GetMenuBarContent()
    {
        return () =>
        {
            //todo: remove this, this is for debugging the ui
            ImGui.SetNextItemWidth(100);
            ImGui.DragFloat2("Padding", ref Padding);
            ImGui.SameLine();
            ImGui.SetNextItemWidth(100);
            ImGui.DragFloat2("ImageSize", ref ImageSize);
            ImGui.SameLine();
            ImGui.SetNextItemWidth(100);
            ImGui.DragFloat2("ImageAdjust", ref ImageAdjust);
            ImGui.SameLine();
            ImGui.SetNextItemWidth(100);
            ImGui.DragFloat2("SizeBetween", ref _sizeBetweenItems);
            
            ImGui.BeginMenuBar();

            if (_currentDirectory.FullName != (ProjectSettings.FullProjectPath + "\\Assets"))
            {
                if (ImGui.MenuItem("Back")) SwitchDirectory(new DirectoryInfo(_currentDirectory.Parent.FullName));
            }


            
            int projectLength = (ProjectSettings.FullProjectPath).Length;
            string titleBar = _currentDirectory.FullName.Remove(0, projectLength);
            ImGui.Text(titleBar);
            
            ImGui.EndMenuBar();
        };
    }

    protected override Action GetWindowContent()
    {
        return DrawUI;
    }
    
    public AssetBrowserEntry? CurrentSelectedEntry
    {
        set
        {
            if (_currentSelectedEntry != null)
                _currentSelectedEntry.IsSelected = false;

            if (value != null)
                value.IsSelected = true;
            
            _currentSelectedEntry = value;
        }
   }


    private void Init()
    {
        LoadIcons();
        SwitchDirectory(_currentDirectory);
    }

    private void LoadIcons()
    {
        _folderTexture  = new Texture(Utils.GetBaseEngineDir() + "\\Images\\Icons\\folder-icon.png" , false, 
        TextureMinFilter.Linear, TextureMagFilter.Linear);
        
        _fileTexture = new Texture(Utils.GetBaseEngineDir() + "\\Images\\Icons\\file-icon.png", false, 
        TextureMinFilter.Linear, TextureMagFilter.Linear);
    }

    private bool _isSwitching = false;
    private int _dragDropPayloadCounter = 0;
    private bool _initiatedDragDrop = false;


    public void SwitchDirectory(DirectoryInfo newDirectory)
    {
        _isSwitching = true;
        _currentDirectory = newDirectory;
        
        _directoriesInDirectory = new List<DirectoryInfo>();
        _filesInDirectory = new List<FileInfo>();
        _entries = new List<AssetBrowserEntry>();
        
        GetDirectoriesInCurrent();
        GetFilesInCurrent();
        
        CreateEntries();
        
        _isSwitching = false;
    }
    

    private void GetDirectoriesInCurrent()
    {
        _directoriesInDirectory = _currentDirectory.GetDirectories().ToList();
    }

    private void GetFilesInCurrent()
    {
        _filesInDirectory = _currentDirectory.GetFiles().ToList();
    }

    private void DrawUI()
    {
        ImGui.BeginChild("Content Browser", new(0, -ImGui.GetFrameHeightWithSpacing()));
        {
            if (_isSwitching) return;
            int columnCount = (int)(ImGui.GetContentRegionAvail().X / (ImageSize.X + _sizeBetweenItems.X));
            ImGui.Columns((columnCount < 1) ? 1 : columnCount, "", false);
            {
                foreach (var entry in _entries)
                {
                    entry.Draw();
                    ImGui.NextColumn();
                }
            
            }
            ImGui.Columns(1);
        }
    }

    private void CreateEntries()
    {
        foreach (var dir in _directoriesInDirectory)
        {
            AssetBrowserEntry entry = new AssetBrowserEntry(dir.Name, dir.FullName, dir.Parent.FullName, ESupportedFileTypes.folder,
                _folderTexture, this);
            _entries.Add(entry);
        }
        
        foreach (var file in _filesInDirectory)
        {
            string fileExtension = file.Extension;
            fileExtension = fileExtension.Remove(0, 1);
            if(Enum.TryParse(fileExtension, out ESupportedFileTypes ext))
            {
                
            }

            AssetBrowserEntry entry = new AssetBrowserEntry(file.Name, file.FullName, file.Directory.FullName, ESupportedFileTypes.txt,
                _fileTexture, this);
            _entries.Add(entry);
        }
    }
    
}

public class AssetBrowserEntry
{
    private string _label;
    private string _fullPath;
    private string _parentPath;

    private ESupportedFileTypes _fileType;
    
    private Texture _texture;

    private AssetBrowser _assetBrowser;

    public bool IsSelected = false;
    
    public AssetBrowserEntry(string label, string fullPath, string parentPath, ESupportedFileTypes fileType, Texture texture, AssetBrowser assetBrowser)
    {
        _label = label;
        _fullPath = fullPath;
        _parentPath = parentPath;
        _fileType = fileType;
        
        _texture = texture;
        _assetBrowser = assetBrowser;
    }

    
    public void Draw()
    {
        ImGui.PushID(_fullPath);
        bool clicked = false;
        bool doubleClicked = false;
        bool rightClicked = false;
        
        Gui.ImageButtonExTextDown(
            _label,
            _texture.TexID,
            _assetBrowser.ImageSize,
            new(1), new Vector2(0),
            _assetBrowser.Padding, _assetBrowser.ImageAdjust,
            new Vector4(1),
            out clicked, out doubleClicked, out rightClicked, IsSelected);

        if (ImGui.IsItemClicked())
            _assetBrowser.CurrentSelectedEntry = this;

        if (ImGui.IsItemClicked() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
        {
            if (_fileType == ESupportedFileTypes.folder)
                _assetBrowser.SwitchDirectory(new DirectoryInfo(this._fullPath));
        }
        
        ImGui.PopID();
    }
}
