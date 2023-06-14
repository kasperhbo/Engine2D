using System.Data;
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
    public bool IsEntryHovered;
    
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
        // ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing   , new Vector2(8f,8f) );// spacing(ImGuiStyleVar_ItemSpacing, ImVec2(8.0f, 8.0f));
        // ImGui.PushStyleVar(ImGuiStyleVar.FramePadding  , new Vector2(4f,4f) );// padding(ImGuiStyleVar_FramePadding, ImVec2(4.0f, 4.0f));
        // ImGui.PushStyleVar(ImGuiStyleVar.CellPadding   , new Vector2(10f,2f));// cellPadding(ImGuiStyleVar_CellPadding, ImVec2(10.0f, 2.0f));
        //

        
        ImGuiTableFlags tableFlags =   ImGuiTableFlags.Resizable
                                     | ImGuiTableFlags.SizingFixedFit
                                     | ImGuiTableFlags.BordersInnerV;
        
        ImGui.PushID("ContentBrowserTable");
        
        if (ImGui.BeginTable("table", 2, tableFlags, new Vector2(0.0f, 0.0f)))
        {
            ImGui.TableSetupColumn("Outliner", 0, 300.0f);
            ImGui.TableSetupColumn("Directory Structure", ImGuiTableColumnFlags.WidthStretch);

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);

            ImGui.BeginChild("##folders_common");
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.3f, 0.3f, 0.35f));

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0.0f, 0.0f));

                GetAllDirectories(new DirectoryInfo(ProjectSettings.FullProjectPath + "\\Assets"));
                
                ImGui.PopStyleVar();
            }
            ImGui.EndChild();
            
            ImGui.TableSetColumnIndex(1);

            const float topBarHeight = 26.0f;
            const float bottomBarHeight = 32.0f;
            
            ImGui.BeginChild("ccbrowser");
            {
                if (ImGui.BeginPopupContextWindow("windowpoup"))
                {
                    CurrentSelectedEntry = null;
                    
                    ImGui.MenuItem("Item1");
                    ImGui.MenuItem("Item2");
                    ImGui.EndPopup();
                }

                if (IsHovering)
                {
                    if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                    {
                        CurrentSelectedEntry = null;
                    }
                }

                IsEntryHovered = false;
                if (_isSwitching) return;
                int columnCount = (int)(ImGui.GetContentRegionAvail().X / (ImageSize.X + _sizeBetweenItems.X));
                
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0,10));
                
                ImGui.Columns((columnCount < 1) ? 1 : columnCount, "", false);
                {
                    foreach (var entry in _entries)
                    {
                        entry.Draw();
                        ImGui.NextColumn();
                    }
                }
                ImGui.Columns(1);
                ImGui.PopStyleVar();
            }
            ImGui.EndChild();
            ImGui.PopStyleColor(2);
            ImGui.EndTable();
        }
        ImGui.PopID();
        

    }

    private void GetAllDirectories(DirectoryInfo directory)
    {
        var dirs = directory.GetDirectories();
        ImGuiTreeNodeFlags flags = 
            (dirs.Length > 0 ? ImGuiTreeNodeFlags.None : ImGuiTreeNodeFlags.Leaf|
                                                         ImGuiTreeNodeFlags.OpenOnArrow |
                                                         ImGuiTreeNodeFlags.SpanFullWidth | 
                                                         (directory.FullName == _currentDirectory.FullName ? ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.None)
                                   );
        Console.WriteLine(directory.FullName + " / "  + _currentDirectory.FullName);
        bool open = ImGui.TreeNodeEx(directory.Name, flags);

        if (open)
        {
            foreach (var dir in dirs)
            {
                GetAllDirectories(dir);
            }
        }
        
        if (open) ImGui.TreePop();
    }

    private void RenderTopBar(float topBarHeight)
    {
        
    }

    private void CreateEntries()
    {
        foreach (var dir in _directoriesInDirectory)
        {
            AssetBrowserEntry entry = new AssetBrowserEntry(dir.Name, dir.FullName, dir.Parent.FullName, ESupportedFileTypes.folder,
                _folderTexture, this);
            _entries.Add(entry);
        }
        
        // ImGui.BeginGroup();
     
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
        

        // ImGui.EndGroup();
        
    }
    
}

public class AssetBrowserEntry
{
    public string Label { get; private set; }
    private string _fullPath;
    private string _parentPath;

    private ESupportedFileTypes _fileType;
    private Texture _texture;
    private AssetBrowser _assetBrowser;

    public bool IsSelected = false;
    
    public AssetBrowserEntry(string label, string fullPath, string parentPath, ESupportedFileTypes fileType, Texture texture, AssetBrowser assetBrowser)
    {
        Label = label;
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
            Label,
            _texture.TexID,
            _assetBrowser.ImageSize,
            new(1), new Vector2(0),
            _assetBrowser.Padding, _assetBrowser.ImageAdjust,
            new Vector4(1),
            out clicked, out doubleClicked, out rightClicked, IsSelected);

        if (ImGui.BeginPopupContextItem(Label + "popup")) // <-- This is using IsItemHovered()
        {
            _assetBrowser.CurrentSelectedEntry = this;
            if (ImGui.MenuItem(Label + " label")) {  }
            ImGui.EndPopup();
        }
        
        if (ImGui.IsItemHovered())
        {
            _assetBrowser.IsEntryHovered = true;
        }

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
