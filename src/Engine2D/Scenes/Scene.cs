using Engine2D.GameObjects;
using Engine2D.Rendering;
using Engine2D.UI;
using ImGuiNET;
using KDBEngine.Core;
using KDBEngine.UI;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine2D.Scenes
{
    internal class Scene
    {
        internal string SceneName { get; private set; } = "No Scene Scene";
        internal List<Gameobject> Gameobjects { get; private set; } = new List<Gameobject>();
        
        private Engine _window;

        #region UI Fields
        Dictionary<string,UIElemenet> _windows = new Dictionary<string, UIElemenet>();

        private FrameBuffer _frameBuffer;
        private GameViewport _gameViewport = new GameViewport();
        private ImGuiController _controller;
        private UIElemenet? _debugWindow;
        private UIElemenet? _gameObjectWindow;
        #endregion

        internal Scene() 
        {       
        }

        internal virtual void Init(Engine engine, string sceneName, int width, int height)
        {
            _window = engine;
            SceneName = sceneName;

            GameRenderer.Init();
            
            _controller = new ImGuiController(_window.ClientSize.X, _window.ClientSize.Y);
            _frameBuffer = new FrameBuffer(width,height);
                        
            Gameobject obj1 = new Gameobject();
            obj1.AddComponent(new SpriteRenderer());
            AddGameObjectToScene(obj1);

            _gameObjectWindow = new UIElemenet(
                "Game obj 1", ImGuiWindowFlags.None, () => {
                    ImGui.DragFloat2("pos", ref obj1.transform.position);
                    ImGui.DragFloat2("scale", ref obj1.transform.size);
                    ImGui.DragFloat4("color", ref obj1.GetComponent<SpriteRenderer>.color);
                    if(ImGui.Button("save scene"))
                    {
                        Engine.SaveScene(this);
                    }

                    if (ImGui.Button("Load scene"))
                    {
                        Engine.LoadScene("ExampleScene");
                    }
                });

            _debugWindow = new UIElemenet(
                "Debug Window", ImGuiWindowFlags.None, () => { } );


            _windows.Add(_debugWindow.Title,_debugWindow);
            _windows.Add(_gameObjectWindow.Title, _gameObjectWindow);
        }

        internal virtual void EditorUpdate(double dt) 
        {                        
            foreach (Gameobject obj in Gameobjects) { obj.Update(dt); }
            _controller.Update(_window, dt);         
        }

        internal virtual void Render(bool isEditor) {
            if (isEditor)
            {
                _frameBuffer.Bind();
                GameRenderer.Render();
                _frameBuffer.UnBind();


                ImGui.DockSpaceOverViewport();
                ImGui.ShowDemoWindow();

                _gameViewport.OnGui(_frameBuffer.TextureID);

                foreach (UIElemenet window in _windows.Values)
                {
                    window.Render();
                }

                _controller.Render();

                ImGuiController.CheckGLError("End of frame");
                _window.SwapBuffers();
                return;
            }
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
