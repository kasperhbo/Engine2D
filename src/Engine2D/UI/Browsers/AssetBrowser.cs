using System.Data;
using System.Numerics;
using System.Runtime.InteropServices;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.UI.Browsers;

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
    
    protected override string GSetWindowTitle()
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

    private void ClearLists()
    {

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
        foreach (var dir in _directoriesInDirectory)
        {
            //SELECT FOLDER
            Action actionOnClick = () => { };
            //GOTO FOLDER
            Action actionOnDoubleClick = () =>
            {
                SwitchDirectory(new DirectoryInfo(dir.FullName));
            };
            //POPUP
            Action actionOnRightClick = () => { };
            
            Action actionAfterRender = () => { };


            ImGui.PushID(dir.FullName);
            DrawEntry(dir.Name, dir.FullName, dirtexture.TexID, actionOnClick, actionOnDoubleClick, actionOnRightClick, actionAfterRender);
            HandleDragDropFolder(dir.FullName);
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

                switch (ext)
                {
                    case ESupportedFileTypes.txt:
                        actionOnClick = () => { Log.Succes("Succesfully actionOnClick on " + file.Name); };
                        actionOnDoubleClick = () => { Log.Succes("Succesfully actionOnDoubleClick on " + file.Name); };
                        actionOnRightClick = () => { Log.Succes("Succesfully actionOnRightClick on " + file.Name); };
                        break;
                }
                ImGui.PushID(file.FullName);
                DrawEntry(file.Name, file.FullName, dirtexture.TexID, actionOnClick, actionOnDoubleClick, actionOnRightClick, actionAfterRender);
                HandleDragDropFile(file.FullName, fileExtension, file.Name);
                ImGui.PopID();
            }
            
            ImGui.NextColumn();
        }
    }
    
    private unsafe void HandleDragDropFolder(string thisFullname)
    {
        if (ImGui.BeginDragDropTarget())
        {
            bool isValid = false;
            {
                var payload = ImGui.AcceptDragDropPayload("asset_browser_drop");
                if (payload.IsValidPayload())
                {
                    // var filePath = (string)GCHandle.FromIntPtr(payload.Data).Target;
                    var filePath = _draggingValueFullPath;
                    Console.WriteLine("Dropped : " + filePath + " onto " + thisFullname);
                    isValid = true;
                }
            }


            foreach (ESupportedFileTypes type in (ESupportedFileTypes[])Enum.GetValues(typeof(ESupportedFileTypes)))
            {
                if (!isValid)
                {
                    var payload = ImGui.AcceptDragDropPayload("asset_browser_drop" + "_" + type);
                    if (payload.IsValidPayload())
                    {
                        MoveFile(_draggingValueFullPath, thisFullname + "\\" + _draggingValueSelfName);
                        isValid = true;
                    }
                }
            }

            ImGui.EndDragDropTarget();
        }
            
        if (ImGui.BeginDragDropSource())
        {
            _currentlyDragging = true;
            _currentlyDraggedHandle ??= GCHandle.Alloc(thisFullname);
            _draggingValueFullPath = thisFullname;
            
            ImGui.SetDragDropPayload("asset_browser_drop", GCHandle.ToIntPtr(_currentlyDraggedHandle.Value),
                (uint)sizeof(IntPtr));
                
            ImGui.Text(thisFullname);
                
            ImGui.EndDragDropSource();
        }
    }

    private unsafe void HandleDragDropFile(string thisFullname, string extensions, string thisSelfName)
    {
        if (!ImGui.BeginDragDropSource()) return;
        
        _currentlyDragging = true;
        _currentlyDraggedHandle ??= GCHandle.Alloc(thisFullname);
        _draggingValueFullPath = thisFullname;
        _draggingValueSelfName = thisSelfName;

        string type = "asset_browser_drop" + "_" + extensions; 
        
        ImGui.SetDragDropPayload(type, GCHandle.ToIntPtr(_currentlyDraggedHandle.Value),
            (uint)sizeof(IntPtr));
                
        ImGui.Text(extensions);
        ImGui.EndDragDropSource();
    }

    private void MoveFile(string fullName, string destFullname)
    {
        File.Move(fullName, destFullname);
        SwitchDirectory(_currentDirectory);
    }

    private bool DrawEntry(string name, string fullName, IntPtr icon, Action? onClick = null, Action? onDoubleClick = null, Action? onRightClick = null, Action? afterRender = null)
    {
        return Gui.FileIcon(name, icon, onClick, onDoubleClick, onRightClick, afterRender);
    }
}
