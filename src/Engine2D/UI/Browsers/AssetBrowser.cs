using System.Data;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.UI.Browsers;

public struct DragDropPayload
{
    public string PayloadData;
    
}

public struct DragDropPayloadReference
{
    public int Index;
}


public class AssetBrowser : UiElemenet
{
    private enum ESupportedFileTypes
    {
        png      ,
        kdbscene ,
        sprite   ,
        texture  ,
        txt
    }
    
    public AssetBrowser() : base()
    {
        Init();
    }
    
    protected override string GetWindowTitle()
    {
        Log.Message("Creating a new Asset Browser");
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

    private static string _baseAssetDir = ProjectSettings.FullProjectPath + "\\Assets";
    private DirectoryInfo _currentDirectory = new DirectoryInfo(_baseAssetDir);
    
    private IntPtr _dirIcon;
    private IntPtr _fileIcon;

    private Texture dirtexture;
    private Texture fileTexture;

    private List<DirectoryInfo> _directoriesInDirectory = new List<DirectoryInfo>();
    private List<FileInfo> _filesInDirectory = new List<FileInfo>();
    
    private GCHandle? _currentlyDraggedHandle;
    private bool _currentlyDragging;

    private string _draggingValueFullPath;
    private string _draggingValueSelfName;

    private Vector4 _overwriteColorOnDragDrop = new(1,0,0,1);
    private bool _onDragDropTarget = false;
    
    private void Init()
    {
        LoadIcons();
        SwitchDirectory(_currentDirectory);
    }
    
    private void LoadIcons()
    {
        dirtexture  = new Texture(Utils.GetBaseEngineDir() + "\\Images\\Icons\\directoryicon.png" , false, 
        TextureMinFilter.Linear, TextureMagFilter.Linear);
        
        fileTexture = new Texture(Utils.GetBaseEngineDir() + "\\Images\\Icons\\fileicon.png", false, 
        TextureMinFilter.Linear, TextureMagFilter.Linear);
    }

    private bool isSwitching = false;
    private int _dragDropPayloadCounter = 0;
    private Dictionary<int, DragDropPayload> _dragDropPayloads = new Dictionary<int, DragDropPayload>();
    private bool _initiatedDragDrop = false;

    private void SwitchDirectory(DirectoryInfo newDirectory)
    {
        isSwitching = true;
        _currentDirectory = newDirectory;
        
        _directoriesInDirectory = new List<DirectoryInfo>();
        _filesInDirectory = new List<FileInfo>(); 
        
        GetDirectoriesInCurrent();
        GetFilesInCurrent();
        isSwitching = false;
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
        _onDragDropTarget = false;
        
        ImGui.BeginChild("item view", new(0, -ImGui.GetFrameHeightWithSpacing()));
        {
            if (isSwitching) return;
            int columnCount = (int)(ImGui.GetContentRegionAvail().X / (90 + 20));
            ImGui.Columns((columnCount < 1) ? 1 : columnCount, "", false);
            {       
                if(!isSwitching)
                    DrawFolders();
                
                if(!isSwitching)
                    DrawFiles();
            }
            ImGui.Columns(1);
        }
    }

    private unsafe void DrawFolders()
    {
        for (int i = 0; i < _directoriesInDirectory.Count; i++)
        {
            var dir = _directoriesInDirectory[i];
            bool clicked = false;
            bool doubleClicked = false;
            bool rightClicked = false;
            
            var size = ImGui.GetFrameHeight() * 5.0f * 1;
            
            ImGui.PushID(dir.FullName);
            
            DrawEntry(
                dirtexture.TexID, dir.Name,
                new(256, 256), new(size),
                new(1), new(0),
                -1,
                new(0f), new(1f),
                out clicked, out doubleClicked, out rightClicked
            );
            
            HandleDragDrop(dir: dir);
            
            if (ImGui.BeginDragDropTarget())
            {
                var payload = ImGui.AcceptDragDropPayload("asset_browser_drop");
                if (payload.NativePtr != null)
                {
                    DragDropPayloadReference* h = (DragDropPayloadReference*)payload.Data;
                    var pload = _dragDropPayloads[h->Index];
                    _dragDropPayloads.Remove(h->Index);
                    Console.WriteLine(pload.PayloadData + "onto " + dir.Name);
                }
            
                ImGui.EndDragDropTarget();
            }
            
            // Our buttons are both drag sources and drag targets here!
            // if (ImGui.BeginDragDropSource(ImGuiDragDropFlags.None))
            // {
            //     // Set payload to carry the index of our item (could be anything)
            //     _currentlyDraggedHandle ??= GCHandle.Alloc(dir.FullName);
            //     ImGui.SetDragDropPayload("asset_browser_drop", GCHandle.ToIntPtr(_currentlyDraggedHandle.Value),(uint)sizeof(IntPtr));
            //
            //     // Display preview (could be anything, e.g. when dragging an image we could decide to display
            //     // the filename and a small preview of the image, etc.)
            //     ImGui.Text(dir.Name);
            //     ImGui.EndDragDropSource();
            // }
            //
            // if (ImGui.BeginDragDropTarget())
            // {
            //     var payload = ImGui.AcceptDragDropPayload("asset_browser_drop");
            //     if (payload.IsValidPayload())
            //     {
            //         var data = (string)GCHandle.FromIntPtr(payload.Data).Target!;
            //         Console.WriteLine(data + " onto " + dir.Name);
            //     }
            // }
            
            // HandleDragDropFolder(dir.Name);
            ImGui.PopID();
            ImGui.NextColumn();
        }

    }

    private unsafe void DrawFiles()
    {
        foreach (var file in _filesInDirectory)
        {
            string fileExtension = file.Extension;
            fileExtension = fileExtension.Remove(0, 1);
            if(Enum.TryParse(fileExtension, out ESupportedFileTypes ext))
            {
                Action actionOnClick = () => { };
                Action actionOnDoubleClick = () => { };
                Action actionOnRightClick = () => { };
                Action actionAfterRender = () => { };
                
                bool clicked = false;
                bool doubleClicked = false;
                bool rightClicked = false;

                switch (ext)
                {
                    case ESupportedFileTypes.txt:
                        actionOnClick = () => { Log.Succes("Succesfully actionOnClick on " + file.Name); };
                        actionOnDoubleClick = () => { Log.Succes("Succesfully actionOnDoubleClick on " + file.Name); };
                        actionOnRightClick = () => { Log.Succes("Succesfully actionOnRightClick on " + file.Name); };
                        break;
                }
                // ImGui.PushID(file.FullName);
                
                var size = ImGui.GetFrameHeight() * 5.0f * 1;
                
                ImGui.PushID(file.FullName);
                
                DrawEntry(
                    dirtexture.TexID, file.Name,
                    new(256, 256), new(size),
                    new(1), new(0),
                    -1,
                    new(0f), new(1f),
                    out clicked, out doubleClicked, out rightClicked
                );

                HandleDragDrop(file: file);
                
                ImGui.PopID();
            }
            
            ImGui.NextColumn();
        }
    }

    private unsafe void HandleDragDrop(FileInfo? file = null, DirectoryInfo? dir = null)
    {
        string name = "";
        string fullName = "";
        
        if (file != null)
        {
            name = file.Name;
            fullName = file.FullName;
        }
        else if (dir != null)
        {
            name = dir.Name;
            fullName = dir.FullName;
        }
        else
        {
            return;
        }
        
        if (ImGui.BeginDragDropSource())
        {
            ImGui.Text(name);
            DragDropPayload p = new DragDropPayload()
            {
                PayloadData =  fullName
            };

            _dragDropPayloads.Add(_dragDropPayloadCounter, p);
            DragDropPayloadReference r = new DragDropPayloadReference();
            r.Index = _dragDropPayloadCounter;
            _dragDropPayloadCounter++;
                
            GCHandle handle = GCHandle.Alloc(r, GCHandleType.Pinned);
            ImGui.SetDragDropPayload("asset_browser_drop", handle.AddrOfPinnedObject(), (uint)sizeof(DragDropPayloadReference));
            ImGui.EndDragDropSource();
            handle.Free();
                
            _initiatedDragDrop = true;
        }
    }

    private void MoveFile(string fullName, string destFullname)
    {
        // File.Move(fullName, destFullname);
        // SwitchDirectory(_currentDirectory);
    }

    private bool DrawEntry(
        IntPtr texId, string name, 
        Vector2 texture_size, Vector2 imageSize,
        Vector2 uv0, Vector2 uv1, int frame_padding,
        Vector4 bg_col, Vector4 tint_col,
        out bool isClicked, out bool isDoubleClicked, out bool isRightClicked,
        Action? afterRender = null,
        float r = -1f,
        float g = -1f,
        float b = -1f,
        float a = -1f
    )
    {
        return Gui.ImageButtonExTextDown(name, (uint)texId, texId, imageSize, uv0, uv1, new(-1f),
            bg_col, tint_col,
            out isClicked, out isDoubleClicked, out isRightClicked,
            afterRender, r,g,b,a);

        // return Gui.FileIcon(
        //     texId, name, texture_size, imageSize, uv0, uv1, frame_padding,
        //     bg_col, tint_col,
        //     out isClicked, out isDoubleClicked, out isRightClicked,
        //     afterRender);
        //return Gui.FileIcon(name, icon, onClick, onDoubleClick, onRightClick, afterRender);
    }

    // void test(){
    //     if (ImGui.BeginDragDropSource())
    //     {
    //         ImGui.Text(e.PrettyName);
    //         // Kinda meme
    //         DragDropPayload p = new DragDropPayload();
    //         p.Entity = e;
    //         _dragDropPayloads.Add(_dragDropPayloadCounter, p);
    //         DragDropPayloadReference r = new DragDropPayloadReference();
    //         r.Index = _dragDropPayloadCounter;
    //         _dragDropPayloadCounter++;
    //         
    //         GCHandle handle = GCHandle.Alloc(r, GCHandleType.Pinned);
    //         ImGui.SetDragDropPayload("entity", handle.AddrOfPinnedObject(), (uint)sizeof(DragDropPayloadReference));
    //         ImGui.EndDragDropSource();
    //         handle.Free();
    //         
    //         _initiatedDragDrop = true;
    //     }
    //
    //     if (hierarchial && ImGui.BeginDragDropTarget())
    //     {
    //         var payload = ImGui.AcceptDragDropPayload("entity");
    //         if (payload.NativePtr != null)
    //         {
    //             DragDropPayloadReference* h = (DragDropPayloadReference*)payload.Data;
    //             var pload = _dragDropPayloads[h->Index];
    //             _dragDropPayloads.Remove(h->Index);
    //             _dragDropSources.Add(pload.Entity);
    //             _dragDropDestObjects.Add(e);
    //             _dragDropDests.Add(e.Children.Count);
    //         }
    //
    //         ImGui.EndDragDropTarget();
    //     }
    // }
}
