using Engine2D.Core;
using Engine2D.Rendering;
using ImGuiNET;
using KDBEngine.Core;
using KDBEngine.UI;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.UI
{
    public struct DragDropFile
    {
        public string FileName;
    }

    internal class AssetBrowser : UIElemenet
    {
        public static string CurrentDraggingFileName = "";

        private string _currentDirectory = ProjectSettings.s_FullProjectPath;
        
        
        internal unsafe AssetBrowser()
        {
            TextureData texDataDir = new TextureData(Utils.GetBaseEngineDir() + "\\icons\\directoryicon.png", false, TextureMinFilter.Linear, TextureMagFilter.Linear);
            TextureData texDataFile = new TextureData(Utils.GetBaseEngineDir() + "\\icons\\fileicon.png", false, TextureMinFilter.Linear, TextureMagFilter.Linear);
            TextureData texDataScene = new TextureData(Utils.GetBaseEngineDir() + "\\icons\\mapIcon.png", false, TextureMinFilter.Linear, TextureMagFilter.Linear);

            int dirTexture = ResourceManager.GetTexture(texDataDir).TexID;
            int fileTexture = ResourceManager.GetTexture(texDataFile).TexID;
            int sceneTexture = ResourceManager.GetTexture(texDataScene).TexID;

            this._flags = ImGuiWindowFlags.None;
            this.Title = "Asset Browser";
            this._windowContents = () =>
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(_currentDirectory);
                
                {
                    if (_currentDirectory != ProjectSettings.s_FullProjectPath)
                    {
                        if (ImGui.Button("Back"))
                        {
                            _currentDirectory = directoryInfo!.Parent!.FullName;
                        }
                    }

                    float padding = 16;
                    float thumbnailSize = 128.0f/2;
                    float cellSize = thumbnailSize + padding;
                    float panelWidth = ImGui.GetContentRegionAvail().X;
                    int columnCount = (int)(panelWidth / cellSize);
                    if (columnCount < 1)
                        columnCount = 1;
                    ImGui.Columns(columnCount, "0", false);

                    foreach (var dir in directoryInfo.GetDirectories())
                    {
                        ImGui.PushID(dir.Name);
                        if (ImGui.ImageButton(dir.Name, (IntPtr)dirTexture,
                                new System.Numerics.Vector2(128 / 2, 128 / 2)))
                        {
                            _currentDirectory += "\\" + dir.Name;
                        }

                        ImGui.Text(dir.Name);
                        ImGui.NextColumn();
                        ImGui.PopID();
                    }

                    foreach (var file in directoryInfo.GetFiles())
                    {
                        ImGui.PushID(file.Name);
                        int tex = fileTexture;
                        if (file.Extension == ".kdbscene")
                        {
                            tex = sceneTexture;
                        }
                        
                        ImGui.ImageButton(file.Name, (IntPtr)tex, new System.Numerics.Vector2(128/2, 128/2));

                        if (file.Extension == ".kdbscene")
                        {
                            bool open = false;
                            if (ImGui.BeginPopupContextItem())
                            {
                                if (ImGui.MenuItem("Delete"))
                                {
                                    open = true;
                                }

                                ImGui.EndPopup();


                            }

                            if (open)
                            {
                                ImGui.OpenPopup("DELETEDLG");
                            }

                            if (ImGui.BeginPopupModal("DELETEDLG"))
                            {
                                if (ImGui.Button("ok"))
                                {
                                    File.Delete(file.FullName);
                                    ImGui.CloseCurrentPopup();
                                }
                                if (ImGui.Button("no"))
                                {
                                    ImGui.CloseCurrentPopup();
                                }
                                ImGui.EndPopup();
                            }




                            //TODO: MAKE THIS WITH POINTERS ETC
                            if (ImGui.BeginDragDropSource())
                            {
                                CurrentDraggingFileName = file.FullName;

                                ImGui.SetDragDropPayload("SCENE_DRAG_DROP", IntPtr.Zero, 0);
                                ImGui.Text(CurrentDraggingFileName);
                                ImGui.EndDragDropSource();
                            }
                            
                        }


                        ImGui.Text(file.Name);
                        ImGui.NextColumn();
                        ImGui.PopID();
                    }
                }
            };
        }

        private void DeleteDialog(string file)
        {
            
        }


    }

    //public static UIElemenet Create()
    //{
    //    new UIElemenet("Asset Browser", ImGuiNET.ImGuiWindowFlags.None, () =>
    //    {
    //       
    //    });     
    //}
}

