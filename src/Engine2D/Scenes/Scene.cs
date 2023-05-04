using Engine2D.Core.Scripting;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Engine2D.UI;
using ImGuiNET;
using KDBEngine.Core;
using KDBEngine.UI;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Engine2D.Scenes
{
    public class Scene
    {
        public string Name { get; private set; } = "No Scene Scene";
        private KDBEngine.Core.Engine _window;

        private GameRenderer _renderer;
        
        private List<Gameobject> gameobjects = new List<Gameobject>();
        private List<Gameobject> gameObjectsDuplicate = new List<Gameobject>();

        Dictionary<string,UIElemenet> _windows = new Dictionary<string, UIElemenet>();
                
        public FrameBuffer FrameBuffer { get; private set; }
        private GameViewport _gameViewport = new GameViewport();
        private ImGuiController _controller;

        private UIElemenet? _debugWindow;


        public Scene(KDBEngine.Core.Engine window, string sceneName) 
        {
            this._window = window;
            Name = sceneName;
           
        }

        public virtual void Init(){
            //_camera = new Camera(new Vector2(0,0));

            _renderer = new GameRenderer();
            _renderer.Init(_window.ClientSize);

            _controller = new ImGuiController(_window.ClientSize.X, _window.ClientSize.Y);
            FrameBuffer = new FrameBuffer(1920,1080);
                        
            _debugWindow = new UIElemenet(
                "Debug Window", ImGuiWindowFlags.None, () => { } );

            _windows.Add(_debugWindow.Title,_debugWindow);
        }

        public virtual void EditorUpdate(double dt) {
            
            _renderer.Update(dt);
            _controller.Update(_window, dt);

            foreach (Gameobject gameobject in gameobjects) { gameobject.EditorUpdate(dt); }
        }


        #region gameloop
        bool _firstRunGameLoop = true;

        public void StartGameLoop()
        {
            _firstRunGameLoop = false;
            foreach (Gameobject gameobject in gameobjects) { gameobject.Init(); }
            foreach (Gameobject gameobject in gameobjects) { gameobject.Start(); }
        }

        public virtual void GameUpdate(double dt)
        {
            if(_firstRunGameLoop) {
                StartGameLoop();
            }
            
            foreach (Gameobject gameobject in gameobjects) { gameobject.GameUpdate(dt); }
        }

        public virtual void EndGameLoop()
        {
            foreach (Gameobject gameobject in gameobjects) { gameobject.OnEndGameLoop(); }
        }
        #endregion

        public virtual void Render(bool inEditor) {

            _renderer.DrawCalls = 0;

            if(inEditor) { 
                FrameBuffer.Bind();
                _renderer.Render();
                FrameBuffer.UnBind();           
            

                ImGui.DockSpaceOverViewport();
                ImGui.ShowDemoWindow();

                _gameViewport.OnGui();
                _debugWindow?.SetWindowContent(() => { ImGui.Text("DrawCalls: " + _renderer.DrawCalls); } );


                foreach (UIElemenet window in _windows.Values)
                {
                    window.Render();
                }

                _controller.Render();

                ImGuiController.CheckGLError("End of frame");
                _window.SwapBuffers();
                return;
            }

            _renderer.Render();
            
            foreach (Gameobject gameobject in gameobjects) { gameobject.OnRender(); }

            _window.SwapBuffers();
        }

        public virtual void OnClose() {
            foreach (Gameobject go in gameobjects)
            {
                go.OnClose();
            }
            
            _renderer.OnClose();
        }

        #region inputs
        public virtual void OnResized(Vector2i newSize) { 
            _renderer.OnResize(newSize);
            FrameBuffer = new FrameBuffer(newSize.X, newSize.Y);
            _controller.WindowResized(newSize.X, newSize.Y);
        }
                
        public virtual void OnMouseWheel(MouseWheelEventArgs mouseWheelEventArgs)
        {
            _controller.MouseScroll(mouseWheelEventArgs.Offset);
        }

        public virtual void OnTextInput(TextInputEventArgs inputEventArgs)
        {
            _controller.PressChar((char)inputEventArgs.Unicode);
        }
        #endregion

        public Gameobject AddGameObjectToScene(Gameobject go)
        {
            gameobjects.Add(go);
            
            go.Init();
            go.Start();

            return go;
        }
        public void RemoveGameObjectFromScene(Gameobject go)
        {
            go.OnDestroy();
            
            gameobjects.Remove(go);
        }

    }
}
