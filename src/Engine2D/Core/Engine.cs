using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using Engine2D.Scenes;
using System.Runtime.CompilerServices;
using Engine2D.GameObjects;
using Engine2D.Core.Scripting;

namespace KDBEngine.Core { 
    // Be warned, there is a LOT of stuff here. It might seem complicated, but just take it slow and you'll be fine.
    // OpenGL's initial hurdle is quite large, but once you get past that, things will start making more sense.
    public class Engine : GameWindow
    {
        private static Engine _instance = null;

        public Scene? _currentScene = null;

        private static bool _isEditor = true;
        private bool _gameIsRunning = false;

        public float TargetAspectRatio => 16.0f / 9.0f;

        public static Engine Get()
        {
            if(_instance  == null)
            {
                GameWindowSettings gameWindowSettings = GameWindowSettings.Default;
                gameWindowSettings.UpdateFrequency = WindowSettings.s_UpdateFrequency;
                gameWindowSettings.RenderFrequency = WindowSettings.s_RenderFrequency;

                NativeWindowSettings ntwSettings  = NativeWindowSettings.Default;
                ntwSettings.Title = WindowSettings.s_Title;
                ntwSettings.Size = WindowSettings.s_Size;
                
                Engine window = new Engine(gameWindowSettings, ntwSettings);
                
                _instance = window;
            }
                        
            return _instance;
        }

        public static Engine Get(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        {
            if (_instance == null)
            {
                Engine window = new Engine(gameWindowSettings, nativeWindowSettings);

                _instance = window;
            }

            return _instance;
        }


        public Engine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            SwitchScene(new Scene(_instance, "test scene"));
            
            _gameIsRunning = true;
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (_isEditor) { _currentScene?.EditorUpdate(args.Time); }
            if(_gameIsRunning ) { _currentScene?.GameUpdate(args.Time); }   
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            _currentScene?.Render(inEditor: _isEditor);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            _currentScene?.OnResized(ClientSize);
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            _currentScene?.OnTextInput(e);

            if ((char)e.Unicode == 'c')
                _gameIsRunning = !_gameIsRunning;
                //SwitchScene(new Scene(this, "test scene"));
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _currentScene?.OnMouseWheel(e);
        }

        public void SwitchScene(Scene newScene)
        {
            if (newScene == null) { throw new Exception("Scene is null"); }                       

            _currentScene = newScene;           

            newScene.Init();        
        }
        public void StartPlayingScene()
        {
            _gameIsRunning = true;
            _currentScene?.StartGameLoop();
        }
        public void StopGameSceneFromPlaying()
        {
            _currentScene?.EndGameLoop();
            _gameIsRunning = false;
        }
    }
}

public static class WindowSettings
{
    public static string s_Title = "Kasper Engine";
    public static Vector2i s_Size = new Vector2i(1920/2, 1080/2);
    
    public static float s_UpdateFrequency = 60;
    public static float s_RenderFrequency = 60;
}