#region

using System.Numerics;
using System.Runtime.InteropServices;
using Engine2D.Components.Sprites;
using Engine2D.Components.Sprites.SpriteAnimations;
using Engine2D.Core;
using Engine2D.Core.Inputs;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

#endregion

namespace Engine2D.UI.Browsers;

internal enum ESupportedFileTypes
{
    unkown,
    folder,
    png,
    kdbscene,
    sprite,
    spritesheet,
    tex,
    txt,
    cs,
    prefab,
    animation,
    tmx
}

internal class AssetBrowserPanel : UIElement
{
    internal static List<AssetBrowserPanel> AssetBrowserPanels = new();

    private static readonly string BaseAssetDir = ProjectSettings.FullProjectPath + "\\Assets";

    private readonly Vector2 _sizeBetweenItems = new(15);

    private readonly TopBarButton backButton = new("Back");

    internal int _currentSelectedEntryIndex = -1;

    private List<DirectoryInfo> _directoriesInDirectory = new();
    private List<AssetBrowserEntry> _entries = new();
    private Texture _fileIcon = null!;
    private List<FileInfo> _filesInDirectory = new();

    private Texture _folderIcon = null!;
    private Texture _texIcon = null!;
    internal Vector2 ImageAdjust = new(0, 11);
    internal Vector2 ImageSize = new(90);


    internal Vector2 Padding = new(-1, -2);

    internal AssetBrowserPanel(string title) : base(title)
    {
        Flags = ImGuiWindowFlags.None | ImGuiWindowFlags.NoScrollbar;
        AssetBrowserPanels.Add(this);
        Init();
    }

    internal DirectoryInfo CurrentDirectory { get; private set; } = new(BaseAssetDir);

    internal static void Refresh()
    {
        foreach (var panel in AssetBrowserPanels) panel.SwitchDirectory(panel.CurrentDirectory);
    }

    internal override void RenderTopBar()
    {
        if (CurrentDirectory.FullName != ProjectSettings.FullProjectPath + "\\Assets")
        {
            if (Gui.TopBarButton(2, new Vector2(40, 20), backButton)) SwitchDirectory(CurrentDirectory.Parent);
            ImGui.SameLine();
        }

        var defPath = ProjectSettings.FullProjectPath + 1;
        ImGui.Text(CurrentDirectory.FullName.Remove(0, defPath.Length));
        ImGui.SameLine();

        base.RenderTopBar();
    }

    internal override void Render()
    {
        DrawUI();
    }

    internal override void FileDrop(FileDropEventArgs obj)
    {
        if (!IsHovering) return;

        var imageFiles = new List<FileInfo>();

        foreach (var file in obj.FileNames)
        {
            var fInfo = new FileInfo(file);
            if (Enum.TryParse(fInfo.Extension.Remove(0, 1), out ESupportedFileTypes ext))
                if (ext == ESupportedFileTypes.png)
                    imageFiles.Add(fInfo);
        }

        for (var i = 0; i < imageFiles.Count; i++)
        {
            var extensionLength = imageFiles[i].Extension.Length;
            var saveName = imageFiles[i].Name.Remove(imageFiles[i].Name.Length - extensionLength, extensionLength);
            saveName += ".tex"; 
            saveName = saveName.Insert(0, CurrentDirectory.FullName + "\\");
            saveName = saveName.Replace(ProjectSettings.FullProjectPath, "");
            
            
            var texture = new Texture(imageFiles[i].FullName, saveName, true,
                TextureMinFilter.Linear, TextureMagFilter.Linear);

    
            
            texture.Save();
            Log.Succes(string.Format("Succesfully made texture from {0}, and save it to {1}",
                imageFiles[i].FullName, CurrentDirectory + "\\" + imageFiles[i].Name));
        }

        //refresh ui
        SwitchDirectory(CurrentDirectory);
    }

    private void Init()
    {
        LoadIcons();
        SwitchDirectory(CurrentDirectory);
    }

    private void LoadIcons()
    {
        _folderIcon = IconManager.GetIcon("folder-icon");
        _fileIcon = IconManager.GetIcon("file-icon");
        _texIcon = IconManager.GetIcon("texture-icon");
    }


    internal void SwitchDirectory(DirectoryInfo newDirectory)
    {
        CurrentDirectory = newDirectory;

        _directoriesInDirectory = new List<DirectoryInfo>();
        _filesInDirectory = new List<FileInfo>();
        _entries = new List<AssetBrowserEntry>();

        GetDirectoriesInCurrent();
        GetFilesInCurrent();

        CreateEntries();
    }


    private void GetDirectoriesInCurrent()
    {
        _directoriesInDirectory = CurrentDirectory.GetDirectories().ToList();
    }

    private void GetFilesInCurrent()
    {
        _filesInDirectory = CurrentDirectory.GetFiles().ToList();
    }

    private void DrawUI()
    {
        var columnCount = 0;
        var tableFlags = ImGuiTableFlags.Resizable
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

            ImGui.BeginChild("ccbrowser");
            {
                if (ImGui.BeginPopupContextWindow("windowpoup"))
                {
                    _currentSelectedEntryIndex = -1;

                    ImGui.MenuItem("Item1");
                    ImGui.MenuItem("Item2");
                    
                    if(ImGui.MenuItem("Create animation"))
                    {
                        var savePath = CurrentDirectory.FullName + "\\";
                        savePath += "NewAnimation";
                        savePath += ".animation";
                        
                        savePath = savePath.Replace(ProjectSettings.FullProjectPath, "");
                        
                        var animation = new Animation(savePath);
                        animation.Save();
                    }
                    
                    ImGui.EndPopup();
                }

                if (IsHovering)
                    if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                        _currentSelectedEntryIndex = -1;

                columnCount = (int)(ImGui.GetContentRegionAvail().X / (ImageSize.X + _sizeBetweenItems.X));

                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 10));

                ImGui.Columns(columnCount < 1 ? 1 : columnCount, "", false);
                {
                    for (var i = 0; i < _entries.Count; i++)
                    {
                        var entry = _entries[i];
                        entry.Draw(i);
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
        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGui.AcceptDragDropPayload("gameobject_drop_hierachy");
            if (payload.IsValidPayload())
            {
                Console.WriteLine("drop");
                var handle = GCHandle.FromIntPtr(new IntPtr(payload.Data));
                var draggedObject = (Gameobject?)handle.Target;
                
                if (draggedObject != null)
                {
                    var fileName = CurrentDirectory.FullName + "\\" + draggedObject.Name;
                    
                    SaveLoad.SaveGameobject(fileName, draggedObject);
                }
            }
            ImGui.EndDragDropTarget();
        }

        ImGui.PopID();

        if (IsFocussed) KeyEvents(columnCount);
    }

    private void KeyEvents(int columnCount)
    {
        if (Input.KeyPressed(Keys.Enter))
        {
            if (_currentSelectedEntryIndex == -1) return;
            if (_entries[_currentSelectedEntryIndex]._fileType == ESupportedFileTypes.folder)
            {
                var fullPath = _entries[_currentSelectedEntryIndex].FullPath;

                if (fullPath != null)
                    SwitchDirectory(new DirectoryInfo(fullPath));
            }
        }

        if (Input.KeyPressed(Keys.Right))
        {
            if (_currentSelectedEntryIndex == -1)
                _currentSelectedEntryIndex = 0;
            else if (_currentSelectedEntryIndex > _entries.Count - 1)
                _currentSelectedEntryIndex = 0;
            else
                _currentSelectedEntryIndex++;
        }

        if (Input.KeyPressed(Keys.Left))
        {
            if (_currentSelectedEntryIndex == -1)
                _currentSelectedEntryIndex = _entries.Count - 1;
            else if (_currentSelectedEntryIndex < 0)
                _currentSelectedEntryIndex = _entries.Count - 1;
            else
                _currentSelectedEntryIndex--;
        }

        if (Input.KeyPressed(Keys.Up))
        {
            if (_currentSelectedEntryIndex == -1)
                _currentSelectedEntryIndex = 0;
            else
                _currentSelectedEntryIndex -= columnCount;

            if (_currentSelectedEntryIndex == -1) _currentSelectedEntryIndex = 0;

            if (_currentSelectedEntryIndex < 0) _currentSelectedEntryIndex = _entries.Count - 1;
        }

        if (Input.KeyPressed(Keys.Down))
        {
            if (_currentSelectedEntryIndex == -1)
                _currentSelectedEntryIndex = 0;
            else
                _currentSelectedEntryIndex += columnCount;

            if (_currentSelectedEntryIndex == _entries.Count)
                _currentSelectedEntryIndex = _entries.Count - 1;

            if (_currentSelectedEntryIndex > _entries.Count - 1) _currentSelectedEntryIndex = 0;
        }
    }

    private void GetAllDirectories(DirectoryInfo directory)
    {
        var dirs = directory.GetDirectories();

        var currentDir = CurrentDirectory.FullName == directory.FullName;
        var hasFolders = dirs.Length > 0;

        if (directory.FullName == ProjectSettings.FullProjectPath + "\\Assets")
        {
            foreach (var dir in dirs) GetAllDirectories(dir);

            return;
        }

        ImGuiTreeNodeFlags flags;

        flags = (currentDir ? ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.None) |
                (hasFolders ? ImGuiTreeNodeFlags.None : ImGuiTreeNodeFlags.Leaf) |
                ImGuiTreeNodeFlags.OpenOnArrow;

        var open = ImGui.TreeNodeEx(directory.Name, flags);

        ImGui.GetWindowDrawList().AddImage(_folderIcon.TexID, new Vector2(0, 0), new Vector2(25, 25));

        if (ImGui.IsItemClicked()) SwitchDirectory(directory);

        if (open)
            foreach (var dir in dirs)
                GetAllDirectories(dir);

        if (open) ImGui.TreePop();
    }

    private void CreateEntries()
    {
        foreach (var dir in _directoriesInDirectory)
        {
            var entry = new AssetBrowserEntry(dir.Name, dir.FullName, dir.Parent.FullName, ESupportedFileTypes.folder,
                _folderIcon, this);
            _entries.Add(entry);
        }

        foreach (var file in _filesInDirectory)
        {
            var fileExtension = file.Extension;
            fileExtension = fileExtension.Remove(0, 1);
            var ext = ESupportedFileTypes.unkown;
            Enum.TryParse(fileExtension, out ext);


            ref var tex = ref _fileIcon;
            if (ext == ESupportedFileTypes.tex)
                tex = ref _texIcon;

            var entry = new AssetBrowserEntry(file.Name, file.FullName, file.Directory.FullName, ext,
                tex, this);


            _entries.Add(entry);
        }
    }
}

internal class AssetBrowserEntry
{
    private readonly AssetBrowserPanel _assetBrowserPanel;

    internal readonly ESupportedFileTypes _fileType;
    private readonly Texture _texture;
    internal readonly string? FullPath;
    private GCHandle? _currentlyDraggedHandle;

    private bool _isSelected;
    private string _parentPath;

    internal AssetBrowserEntry(string label, string? fullPath, string parentPath, ESupportedFileTypes fileType,
        Texture texture, AssetBrowserPanel assetBrowserPanel)
    {
        Label = label;
        FullPath = fullPath;
        _parentPath = parentPath;
        _fileType = fileType;

        _texture = texture;
        _assetBrowserPanel = assetBrowserPanel;

        if (_fileType == ESupportedFileTypes.tex) _texture = ResourceManager.LoadTextureFromJson(FullPath);
    }

    internal string Label { get; }


    internal unsafe void Draw(int index)
    {
        ImGui.PushID(FullPath);
        var clicked = false;
        var doubleClicked = false;
        var rightClicked = false;

        if (index == _assetBrowserPanel._currentSelectedEntryIndex)
            _isSelected = true;
        else
            _isSelected = false;

        Gui.ImageButtonExTextDown(
            Label,
            _fileType,
            _texture.TexID,
            _assetBrowserPanel.ImageSize,
            new Vector2(0, 1), new Vector2(1, 0),
            _assetBrowserPanel.Padding, _assetBrowserPanel.ImageAdjust,
            new Vector4(1),
            out clicked, out doubleClicked, out rightClicked, _isSelected);
        
        var relativePath = FullPath?.Replace(ProjectSettings.FullProjectPath, "");
        
        if (ImGui.BeginDragDropSource())
        {
            
            _currentlyDraggedHandle ??= GCHandle.Alloc(relativePath);

            switch (_fileType)
            {
                case ESupportedFileTypes.sprite:
                    ImGui.SetDragDropPayload("sprite_drop", GCHandle.ToIntPtr(_currentlyDraggedHandle.Value),
                        (uint)sizeof(IntPtr));
                    break;
                case ESupportedFileTypes.spritesheet:
                    ImGui.SetDragDropPayload("spritesheet_drop", GCHandle.ToIntPtr(_currentlyDraggedHandle.Value),
                        (uint)sizeof(IntPtr));
                    break;
                case ESupportedFileTypes.animation:
                {
                    ImGui.SetDragDropPayload("animation_drop", GCHandle.ToIntPtr(_currentlyDraggedHandle.Value),
                        (uint)sizeof(IntPtr));
                    break;
                }
                case ESupportedFileTypes.cs:
                    ImGui.SetDragDropPayload("script_drop", GCHandle.ToIntPtr(_currentlyDraggedHandle.Value),
                        (uint)sizeof(IntPtr));
                    break;
                case ESupportedFileTypes.tmx:
                    ImGui.SetDragDropPayload("tilemap_drop", GCHandle.ToIntPtr(_currentlyDraggedHandle.Value),
                        (uint)sizeof(IntPtr));
                    break;
                case ESupportedFileTypes.prefab:
                {
                    ImGui.SetDragDropPayload("prefab_drop", GCHandle.ToIntPtr(_currentlyDraggedHandle.Value),
                        (uint)sizeof(IntPtr));
                    break;
                }
            }


            ImGui.EndDragDropSource();
        }

        if (ImGui.BeginPopupContextItem(Label + "popup")) // <-- This is using IsItemHovered()
        {
            _assetBrowserPanel._currentSelectedEntryIndex = index;

            if (_fileType == ESupportedFileTypes.tex)
            {
                
                if (ImGui.MenuItem("Create Sprite)"))
                {
                    var savePath = _assetBrowserPanel.CurrentDirectory.FullName + "\\";
                    savePath += Label.Remove(Label.Length - 4);
                    savePath += ".spritesheet";
                    savePath = savePath?.Replace(ProjectSettings.FullProjectPath, "");

                    var spriteSheet = new SpriteSheet(relativePath, savePath);
                    spriteSheet.Save();
                    AssetBrowserPanel.Refresh();
                }
                if (ImGui.MenuItem("Create Sprite(Sheet)"))
                {
                    var savePath = _assetBrowserPanel.CurrentDirectory.FullName + "\\";
                    savePath += Label.Remove(Label.Length - 4);
                    savePath += ".spritesheet";
                    savePath = savePath?.Replace(ProjectSettings.FullProjectPath, "");

                    var spriteSheet = new SpriteSheet(relativePath, savePath, 16, 16, 1, 0);
                    spriteSheet.Save();
                    AssetBrowserPanel.Refresh();
                }
            }

            if (ImGui.MenuItem(Label + " label"))
            {
            }

            ImGui.EndPopup();
        }

        //
        if (ImGui.IsItemClicked())
            _assetBrowserPanel._currentSelectedEntryIndex = index;

        //Loading items on clicked to show their inspector
        if (ImGui.IsItemClicked() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
        {
            if (_fileType == ESupportedFileTypes.tex)
                Engine.Get().CurrentSelectedTextureAssetBrowserAsset = _texture;
            
            if (_fileType == ESupportedFileTypes.spritesheet)
            {
                // SpriteSheet sprite = SaveLoad.LoadSpriteSheetFromJson(fullPath);
                var spriteSheet = ResourceManager.GetItem<SpriteSheet>(relativePath);
                Engine.Get().CurrentSelectedSpriteSheetAssetBrowserAsset = spriteSheet;
            }

            if (_fileType == ESupportedFileTypes.animation)
            {
                var animation = ResourceManager.GetItem<Animation>(relativePath);
                Engine.Get().CurrentSelectedAnimationAssetBrowserAsset = animation;
            }
            
        }

        ImGui.PopID();
    }
}