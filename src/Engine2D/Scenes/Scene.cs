using Engine2D.Core.Scripting;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Engine2D.UI;
using ImGuiNET;
using KDBEngine.Core;
using KDBEngine.UI;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine2D.Scenes
{
    internal class Scene
    {
        public string Name { get; private set; } = "No Scene Scene";

        private GameRenderer _renderer;
        private ScriptRunner scriptRunner = new();

        private KDBEngine.Core.Window _window;

        private List<Gameobject> gameobjects = new List<Gameobject>();
        private Camera _camera;


        public FrameBuffer FrameBuffer;
        private GameViewport _gameViewport = new GameViewport();
        private ImGuiController _controller;


        internal Scene(KDBEngine.Core.Window window, string sceneName) 
        {
            this._window = window;
            Name = sceneName;
           
        }

        internal virtual void Init(){
            //_camera = new Camera(new Vector2(0,0));

            _renderer = new GameRenderer();
            _renderer.Init(_window.ClientSize);

            _controller = new ImGuiController(_window.ClientSize.X, _window.ClientSize.Y);
            FrameBuffer = new FrameBuffer(1920,1080);

            foreach (Gameobject gameobject in gameobjects)  gameobject.Init();

            var go = AddGameObjectToScene(new Gameobject());
            scriptRunner.LoadScript(new TestScript(), go);
        }

        internal virtual void EditorUpdate(double dt) {
            _renderer.Update(dt);
            _controller.Update(_window, dt);
        }

        bool _firstRun = true;

        private void StartGame()
        {
            foreach (Gameobject gameobject in gameobjects) gameobject.Init();
            scriptRunner.RaiseStartEvenet();
            _firstRun = false;
        }

        internal virtual void GameUpdate(double dt)
        {
            if(_firstRun) { StartGame(); }    
            _renderer.Update(dt);
            //Run Game Scripts
            scriptRunner.RaiseUpdateEvent(dt);
            //Run Physics
        }

        internal virtual void PreRender()
        {
        }



        internal virtual void Render(bool inEditor) {

            if(inEditor) { 
                FrameBuffer.Bind();
                _renderer.Render();
                FrameBuffer.UnBind();           
            

                ImGui.DockSpaceOverViewport();
                ImGui.ShowDemoWindow();

                _gameViewport.OnGui();
                
                _controller.Render();

                ImGuiController.CheckGLError("End of frame");
                _window.SwapBuffers();
                return;
            }

            _renderer.Render();
            _window.SwapBuffers();
        }

        internal virtual void PostRender()
        {
            
        }

        internal virtual void OnClose() {
            _renderer.OnClose();
        }

        internal virtual void OnResized(Vector2i newSize) { 
            _renderer.OnResize(newSize);
            FrameBuffer = new FrameBuffer(newSize.X, newSize.Y);
            _controller.WindowResized(newSize.X, newSize.Y);
        }

        internal virtual void OnMouseWheel(MouseWheelEventArgs mouseWheelEventArgs)
        {
        }

        internal virtual void OnTextInput(TextInputEventArgs inputEventArgs)
        {
        }

        internal Gameobject AddGameObjectToScene(Gameobject go)
        {
            gameobjects.Add(go);
            go.Init();
            return go;
        }
    }
}
