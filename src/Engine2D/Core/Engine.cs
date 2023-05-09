using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using Engine2D.Scenes;
using System.Runtime.CompilerServices;
using Engine2D.GameObjects;
using Newtonsoft.Json;
using Engine2D.Rendering;
using System.Data;
using static System.Formats.Asn1.AsnWriter;
using KDBEngine.UI;
using Engine2D.UI;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ImGuiNET;

namespace KDBEngine.Core { 
    // Be warned, there is a LOT of stuff here. It might seem complicated, but just take it slow and you'll be fine.
    // OpenGL's initial hurdle is quite large, but once you get past that, things will start making more sense.
    public class Engine : GameWindow
    {
        private static Engine _instance = null;
        private GameViewport gameViewport = new GameViewport();

        internal Scene? _currentScene = null;        

        public float TargetAspectRatio => 16.0f / 9.0f;

        public ImGuiController ImGuiController { get; internal set; }
        private FrameBuffer _frameBuffer;
        private Dictionary<string, UIElemenet> _guiWindows = new Dictionary<string, UIElemenet>();

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
            ImGuiController = new ImGuiController(Size.X, Size.Y);
            
            if (Settings.s_IsEngine)
            {
                Console.WriteLine("engine");
                CreateUIWindows();
            }

            LoadScene(ProjectSettings.s_FullProjectPath + "\\DefaultScenes\\testscene.kdbscene");
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (Settings.s_IsEngine) { _currentScene?.EditorUpdate(args.Time); }            
            
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            if (Settings.s_IsEngine)
            {
                _frameBuffer.Bind();
                GameRenderer.Render();
                _frameBuffer.UnBind();

                ImGuiController.Update(this, e.Time);
                ImGui.BeginMainMenuBar();
                if (ImGui.BeginMenu("Menu"))
                {
                    if (ImGui.MenuItem("Save Scene"))
                    {
                        SaveScene(_currentScene);
                    }
                    if (ImGui.MenuItem("Load Scene"))
                    {

                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Help"))
                {
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();


                ImGui.DockSpaceOverViewport();
                ImGui.ShowDemoWindow();

                gameViewport.OnGui(_frameBuffer.TextureID, () => { });

                foreach (UIElemenet window in _guiWindows.Values)
                {
                    window.Render();
                }

                ImGuiController.Render();

                ImGuiController.CheckGLError("End of frame");
                SwapBuffers();
                return;
            } 

            _currentScene?.Render(dt: e.Time);
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            _frameBuffer = new FrameBuffer(ClientSize.X, ClientSize.Y);
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

        internal void NewScene(string sceneName)
        {
            GameRenderer.Flush();
            this.Title = WindowSettings.s_Title + " | " + sceneName;
            Scene newScene = new();
            newScene.Init(this, sceneName, Size.X, Size.Y);
            _currentScene = newScene;
            SaveScene(_currentScene);
        }

        internal static void CreateNewProject(string newProjectLocation, string newProjectName)
        {
            ProjectSettings.s_ProjectName = newProjectName;
            ProjectSettings.s_ProjectLocation = newProjectLocation;
            ProjectSettings.s_FullProjectPath = ProjectSettings.s_ProjectLocation + ProjectSettings.s_ProjectName;

            LoadScene(ProjectSettings.s_FullProjectPath + "\\defaultscenes\\example.kdbscene");
        }

        internal static  void LoadScene(string sceneToLoad)
        {         
            Engine.Get().SwitchScene(sceneToLoad);

            if (File.Exists(sceneToLoad))
            {               
                List<Gameobject?> objs = JsonConvert.DeserializeObject<List<Gameobject>>(File.ReadAllText(sceneToLoad))!;

                foreach (var t in objs!)
                {
                    Get()?._currentScene?.AddGameObjectToScene(t);
                }
            }
        }

        internal static void SaveScene(Scene scene)
        {
            var gameObjectArray = scene.Gameobjects.ToArray();
            Console.WriteLine("saving: ", scene.ScenePath);
            string sceneData = JsonConvert.SerializeObject(gameObjectArray, Formatting.Indented);
            
            if (File.Exists(scene.ScenePath))
            {
                File.WriteAllText(scene.ScenePath, sceneData);
            }
            else
            {
                using (FileStream fs = File.Create(scene.ScenePath))
                {
                    fs.Close();
                }

                File.WriteAllText(scene.ScenePath, sceneData);
            }
        }

        private void CreateUIWindows()
        {
            AssetBrowser assetBrowser = new AssetBrowser();
            _guiWindows.Add(assetBrowser.Title, assetBrowser);

            Inspector inspector = new Inspector();
            _guiWindows.Add(inspector.Title, inspector);

            SceneHierachy hierarch = new SceneHierachy(inspector);
            _guiWindows.Add(hierarch.Title, hierarch);

            _frameBuffer = new FrameBuffer(Size.X, Size.Y);
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
    public static Vector2 s_DefaultRenderResolution = new(1920,1080);
}

public static class ProjectSettings
{
    public static string s_ProjectName = "helloworld-01";
    public static string s_ProjectLocation = "C:\\Users\\Kasper\\Documents\\GAMEPROJECTS\\";
    public static string s_FullProjectPath = s_ProjectLocation + s_ProjectName;
}
