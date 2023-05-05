using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using Engine2D.Scenes;
using System.Runtime.CompilerServices;
using Engine2D.GameObjects;
using Newtonsoft.Json;
using Engine2D.Rendering;

namespace KDBEngine.Core { 
    // Be warned, there is a LOT of stuff here. It might seem complicated, but just take it slow and you'll be fine.
    // OpenGL's initial hurdle is quite large, but once you get past that, things will start making more sense.
    public class Engine : GameWindow
    {
        private static Engine _instance = null;

        internal Scene? _currentScene = null;        

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
            SwitchScene("ExampleScene");
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (Settings.s_IsEngine) { _currentScene?.EditorUpdate(args.Time); }            
            
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            _currentScene?.Render(isEditor: Settings.s_IsEngine);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            _currentScene?.OnResized(e);
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            _currentScene?.OnTextInput(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _currentScene?.OnMouseWheel(e);
        }

        internal void SwitchScene(string sceneName)
        {
            GameRenderer.Flush();

            this.Title = WindowSettings.s_Title + " | " + sceneName;
            Scene newScene = new();
            newScene.Init(this, sceneName, Size.X, Size.Y);
            _currentScene = newScene;           
        }

        internal static  void LoadScene(string sceneToLoad)
        {
            Engine.Get().SwitchScene(sceneToLoad);

            string path = sceneToLoad + ".json";
            if (File.Exists(path))
            {
                Console.WriteLine("Load");
                List<Gameobject?> objs = JsonConvert.DeserializeObject<List<Gameobject>>(File.ReadAllText(path))!;

                foreach (var t in objs!)
                {
                    Console.WriteLine("add");
                    Engine.Get()._currentScene.AddGameObjectToScene(t);
                }
            }
        }

        internal static void SaveScene(Scene scene)
        {
            var gameObjectArray = scene.Gameobjects.ToArray();
            string sceneData = JsonConvert.SerializeObject(gameObjectArray, Formatting.Indented);

            string path = scene.SceneName + ".json";
            if (File.Exists(path))
            {
                File.WriteAllText(path, sceneData);
            }
            else
            {
                using (FileStream fs = File.Create(path))
                {
                    fs.Close();
                }

                File.WriteAllText(path, sceneData);
            }
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

public static class Settings
{
    public static bool s_IsEngine = true;
}

public static class RenderSettings
{
    public static Vector2 s_DefaultRenderResolution = new(3840, 2160);
}