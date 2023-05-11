using System.Numerics;
using System.Runtime.InteropServices;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.Rendering;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.UI
{
    public class TestContentBrowser
    {
        private Directory currentDirectory = new Directory("", ProjectSettings.s_FullProjectPath, "");


        TextureData texDataDir;
        TextureData texDataFile ;
        TextureData texDataScene;
        int dirTexture      ;
        int fileTexture     ;
        int sceneTexture;

        public TestContentBrowser()
        {
            texDataDir   = new TextureData(Utils.GetBaseEngineDir() + "\\icons\\directoryicon.png", false, TextureMinFilter.Linear, TextureMagFilter.Linear);
            texDataFile  = new TextureData(Utils.GetBaseEngineDir() + "\\icons\\fileicon.png", false, TextureMinFilter.Linear, TextureMagFilter.Linear);
            texDataScene = new TextureData(Utils.GetBaseEngineDir() + "\\icons\\mapIcon.png", false, TextureMinFilter.Linear, TextureMagFilter.Linear);
            dirTexture   = ResourceManager.GetTexture(texDataDir).TexID;
            fileTexture  = ResourceManager.GetTexture(texDataFile).TexID;
            sceneTexture = ResourceManager.GetTexture(texDataScene).TexID;

        }

        private static GCHandle? _currentlyDraggedHandle;
        List<ContentBrowserItemInfo> previous = new List<ContentBrowserItemInfo>();

        public unsafe void OnGui()
        {
            bool currentlyDragging = false;
            DirectoryInfo directoryInfo = new DirectoryInfo(currentDirectory.Self);

            var files = directoryInfo.GetFiles().ToList();
            var folders = directoryInfo.GetDirectories().ToList();

            List<ContentBrowserItemInfo> cbInfo = new List<ContentBrowserItemInfo>();

            foreach (var f in files)
            {
                ContentBrowserItemInfo cInfo =
                    new ContentBrowserItemInfo(FileType.File, new Directory(f.DirectoryName, f.Name, f.FullName));

                if (f.Extension == ".kdbscene")
                    cInfo.FileType = FileType.Scene;

                if (f.Extension == ".cs")
                    cInfo.FileType = FileType.Script;

                cbInfo.Add(cInfo);
            }

            foreach (var f in folders)
            {
                ContentBrowserItemInfo cInfo =
                    new ContentBrowserItemInfo(FileType.Folder, new Directory(f.Parent.Name, f.Name, f.FullName));

                cbInfo.Add(cInfo);
            }

            //Thanks the Cherno <3
            //Actual ImGui Code
            ImGui.Begin("Content Browser");

            //Back button
            if (directoryInfo != null && directoryInfo.Name != "Resources")
            {
                if (ImGui.Button("<- " + currentDirectory.Self))
                {
                    var tempSelf = currentDirectory.Self;
                    currentDirectory.Self = currentDirectory.Parent;
                    int pos = tempSelf.LastIndexOf("/");
                    currentDirectory.Parent = currentDirectory.Parent.Remove(pos);
                }
            }

            float padding = 16.0f;
            float thumbnailSize = 128.0f;
            float cellSize = thumbnailSize + padding;

            float panelWidth = ImGui.GetContentRegionAvail().X;
            int columnCount = (int)(panelWidth / cellSize);
            if (columnCount < 1)
                columnCount = 1;

            ImGuiNET.ImGui.Columns(columnCount, "0", false);

            for (int i = 0; i < cbInfo.Count; i++)
            {
                ContentBrowserItemInfo cInfo = cbInfo[i];
                int icon = cInfo.FileType == FileType.Folder ? dirTexture : fileTexture;

                ImGui.PushID(i);

                ImGui.ImageButton("##" + i,(IntPtr)icon, new Vector2(thumbnailSize, thumbnailSize));


                if (cInfo.FileType == FileType.File)
                {
                    if (ImGui.BeginDragDropSource())
                    {
                        currentlyDragging = true;
                        _currentlyDraggedHandle ??= GCHandle.Alloc(cInfo.DirectoryDate.Full);
                        ImGui.SetDragDropPayload("File_Drop", GCHandle.ToIntPtr(_currentlyDraggedHandle.Value),
                            (uint)sizeof(IntPtr));
                        ImGui.EndDragDropSource();
                    }
                }


                if (cInfo.FileType == FileType.Script)
                {
                    {
                        string data = File.ReadAllText(cInfo.DirectoryDate.Full);
                        if (data.Contains(" : Component"))
                        {
                            if (ImGui.BeginDragDropSource())
                            {
                                currentlyDragging = true;
                                _currentlyDraggedHandle ??= GCHandle.Alloc(cInfo.DirectoryDate.Full);
                                ImGui.SetDragDropPayload("SCRIPT_DROP", GCHandle.ToIntPtr(_currentlyDraggedHandle.Value),
                                    (uint)sizeof(IntPtr));
                                ImGui.EndDragDropSource();
                            }
                        }
                    }
                }
                
                if (cInfo.FileType == FileType.Scene)
                {
                    if (ImGui.BeginDragDropSource())
                    {
                        currentlyDragging = true;
                        _currentlyDraggedHandle ??= GCHandle.Alloc(cInfo.DirectoryDate.Self);

                        ImGui.SetDragDropPayload("Scene_Drop", GCHandle.ToIntPtr(_currentlyDraggedHandle.Value),
                            (uint)sizeof(IntPtr));

                        ImGui.EndDragDropSource();
                    }
                }

                if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                {
                    if (cInfo.FileType == FileType.Folder || cInfo.FileType == FileType.Scene)
                    {
                        var tempParent = currentDirectory.Self;
                        currentDirectory.Self = currentDirectory.Self + "/" + cInfo.DirectoryDate.Self;
                        currentDirectory.Parent = tempParent;
                    }
                }

                ImGui.TextWrapped(cInfo.DirectoryDate.Self);
                ImGui.NextColumn();
                ImGui.PopID();
            }

            ImGui.Columns(1);

            //Todo status bar;


            ImGui.End();

            previous = cbInfo;
            if (!currentlyDragging && _currentlyDraggedHandle != null)
            {
                _currentlyDraggedHandle.Value.Free();
                _currentlyDraggedHandle = null;
            }
        }

        private class ContentBrowserItemInfo
        {
            public FileType FileType { get; set; }
            public Directory DirectoryDate { get; }

            public ContentBrowserItemInfo(FileType fileType, Directory directoryDate)
            {
                this.FileType = fileType;
                this.DirectoryDate = directoryDate;
            }
        }

        struct Directory
        {
            public string Parent;
            public string Self;
            public string Full;

            public Directory(string parent, string self, string full)
            {
                Parent = parent;
                Self = self;
                Full     = full;

            }
        }

        public enum FileType
        {
            Folder,
            File,
            Script,
            Shader,
            Scene,
            Sprite,
            Texture,
            TextFile,
        }
    }
}





