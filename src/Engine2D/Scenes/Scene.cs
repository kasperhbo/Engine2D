using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Engine2D.UI;
using ImGuiNET;
using KDBEngine.Core;
using KDBEngine.UI;
using NativeFileDialogExtendedSharp;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Reflection.Emit;

namespace Engine2D.Scenes
{
    internal class Scene
    {
        internal string ScenePath { get; private set; } = "NoScene";
        internal List<Gameobject> Gameobjects { get; private set; } = new List<Gameobject>();
        
        private Engine _window;

        private Gameobject? _currentSelectedObject;

        #region UI Fields
        Dictionary<string,UIElemenet> _windows = new Dictionary<string, UIElemenet>();

        private FrameBuffer _frameBuffer;
        private GameViewport _gameViewport = new GameViewport();
        private ImGuiController _controller;
        private UIElemenet? _debugWindow;
        #endregion

        internal Scene() 
        {       
        }

        internal virtual void Init(Engine engine, string scenePath, int width, int height)
        {
            _window = engine;
            ScenePath = scenePath;

            GameRenderer.Init();
            
            _controller = new ImGuiController(_window.ClientSize.X, _window.ClientSize.Y);
            _frameBuffer = new FrameBuffer(width,height);

            _debugWindow = new UIElemenet(
                "Debug Window", ImGuiWindowFlags.None, () => { } );

            UIElemenet sceneHierachy = new UIElemenet(
                "Hierachy"
                , ImGuiWindowFlags.None,
                () =>
                {
                    for (int i = 0; i < Gameobjects.Count; i++) {
                        ImGui.PushID(i);
                        ImGui.TreeNodeEx(
                            Gameobjects[i].Name,
                            ImGuiTreeNodeFlags.DefaultOpen |
                            ImGuiTreeNodeFlags.FramePadding |
                            ImGuiTreeNodeFlags.OpenOnArrow |
                            ImGuiTreeNodeFlags.SpanAvailWidth,
                            Gameobjects[i].Name
                            );

                        if (ImGui.IsItemClicked())
                        {
                            _currentSelectedObject = Gameobjects[i];
                            Console.WriteLine("Selected: " + _currentSelectedObject.Name);
                        }
                        ImGui.PopID();
                    }
                });
                       

            UIElemenet currentSelectedObjInspector = new UIElemenet(
                "Inspector",
                ImGuiWindowFlags.None,
                () =>
                    {
                        _currentSelectedObject?.ImGuiFields();
                    }
                );

                
            _windows.Add(_debugWindow.Title,_debugWindow);
            _windows.Add(sceneHierachy.Title, sceneHierachy);
            _windows.Add(currentSelectedObjInspector.Title, currentSelectedObjInspector);
        }

        internal virtual void EditorUpdate(double dt) 
        {                        
            foreach (Gameobject obj in Gameobjects) { obj.Update(dt); }               
        }

        internal virtual void Render(bool isEditor, double dt) {
            if (isEditor)
            {
                _frameBuffer.Bind();
                GameRenderer.Render();
                _frameBuffer.UnBind();

                _controller.Update(_window, dt);

                ImGui.DockSpaceOverViewport();
                
                _gameViewport.OnGui(_frameBuffer.TextureID);
                TEMP_CREATE_AND_SAVE_MENU();

                foreach (UIElemenet window in _windows.Values)
                {
                    window.Render();
                }

                _controller.Render();

                ImGuiController.CheckGLError("End of frame");
                _window.SwapBuffers();
                return;
            }

            GameRenderer.Render();
            _window.SwapBuffers();
        }

        internal virtual void OnClose() {

            GameRenderer.OnClose();
        }

        internal void AddGameObjectToScene(Gameobject go)
        {
            Gameobjects.Add(go);
            go.Init();
            go.Start();
        }

        private string _tempProjectPath = "";
        private string _tempProjectName = "";

        //TODO: ABSTRACT THIS
        private void TEMP_CREATE_AND_SAVE_MENU()
        {
            ImGui.Begin("test");
            if (ImGui.Button("save scene"))
            {
                Engine.SaveScene(this);
            }

            if (ImGui.Button("New Project"))
            {
                ImGui.OpenPopup("Create New Project");
                _tempProjectPath = ProjectSettings.s_ProjectLocation;
                _tempProjectName = ProjectSettings.s_ProjectName;
            }

            if (ImGui.BeginPopupModal("Create New Project"))
            {    
                
                ImGui.Text("Enter project details:");
                ImGui.InputText("Project File Path", ref _tempProjectPath, 256);
                ImGui.SameLine();

                if (ImGui.Button("..."))
                {
                    NfdDialogResult res = Nfd.PickFolder();
                    if(res.Path != null)
                        _tempProjectPath = res.Path;
                }

                ImGui.InputText("Project Name", ref _tempProjectName, 256);
                if (ImGui.Button("OK"))
                {
                    ProjectSettings.s_ProjectLocation = _tempProjectPath;
                    ProjectSettings.s_ProjectName = _tempProjectName;
                    ProjectSettings.s_FullProjectPath = ProjectSettings.s_ProjectLocation + "/" + ProjectSettings.s_ProjectName;

                    Console.WriteLine("Create new project" + ProjectSettings.s_ProjectLocation);

                    if (!Directory.Exists(ProjectSettings.s_FullProjectPath))
                    {
                        Directory.CreateDirectory(ProjectSettings.s_FullProjectPath);
                    }
                    else
                    {
                        throw new Exception("Project dir not empty");
                    }

                    Utils.CopyDirectory(Utils.GetBaseEngineDir()+"/ExampleProject/", ProjectSettings.s_FullProjectPath, true);

                    Engine.LoadScene(ProjectSettings.s_FullProjectPath+ "/DefaultScenes/example.kdbscene");

                    ImGui.CloseCurrentPopup();
                }
                
                ImGui.SameLine();
                
                if (ImGui.Button("Cancel"))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }


            ImGui.End();
        }

        #region inputs
        internal virtual void OnResized(ResizeEventArgs newSize)
        {
            GameRenderer.OnResize(newSize);
            _frameBuffer = new FrameBuffer(newSize.Size.X, newSize.Size.Y);
            _controller.WindowResized(newSize.Size.X, newSize.Size.Y);
        }

        internal virtual void OnMouseWheel(MouseWheelEventArgs mouseWheelEventArgs)
        {
            _controller.MouseScroll(mouseWheelEventArgs.Offset);
        }

        internal virtual void OnTextInput(TextInputEventArgs inputEventArgs)
        {
            _controller.PressChar((char)inputEventArgs.Unicode);
        }
        #endregion
    }
}
