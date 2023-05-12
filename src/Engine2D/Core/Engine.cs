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
using Newtonsoft.Json.Bson;
using Engine2D.Core;
using Engine2D.Testing;
using OpenTK.Compute.OpenCL;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Engine2D.SavingLoading;
using Box2DSharp.Common;
using Engine2D.Logging;
using System.Diagnostics;
using System.Text;

namespace KDBEngine.Core { 
    // Be warned, there is a LOT of stuff here. It might seem complicated, but just take it slow and you'll be fine.
    // OpenGL's initial hurdle is quite large, but once you get past that, things will start making more sense.
    public class Engine : GameWindow
    {
        private static int small = 1;
		private float width = 1920/small;
		private float height = 1080/small;

        private TestFrameBuffer testFB;
        private TestViewportWindow viewportWindow;

        private static Engine _instance = null;
        
        private GameViewport gameViewport = new GameViewport();

        internal Scene? _currentScene = null;
        internal TestCamera testCamera;
        internal ImGuiController ImGuiController;

        private Dictionary<string, UIElemenet> _guiWindows = new Dictionary<string, UIElemenet>();
        
        //TODO: MAKE THIS STATIC AND SAVABLE
        private EngineSettingsWindow engineSettingsWindow = new EngineSettingsWindow();

        internal Asset? CurrentSelectedAsset = null;

        private TestContentBrowser cb;
        
        public static Engine Get()
        {   
            if (_instance  == null)
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

            Log.Message("Message", showFile: true, showLine: true, showFunction: true);
            Log.Succes("Succes");
            Log.Warning("Warning");
            Log.Error("Error");

            SaveLoad.LoadEngineSettings();

            ImGuiController = new ImGuiController(Size.X, Size.Y);

            if (Settings.s_IsEngine)
            {
                CreateUIWindows();
            }

            SaveLoad.LoadScene(ProjectSettings.s_FullProjectPath + "\\kasper1.kdbscene");
            TestInput.Init();
            
            testFB = new((int)width, (int)height);
            testCamera = new();
            viewportWindow = new();
            cb = new();



            if (!Settings.s_IsEngine)
            {
                _currentScene.IsPlaying = true;
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            //Input.Update(KeyboardState, MouseState);
            TestInput.mousePosCallback(MouseState, KeyboardState);
            _currentScene?.EditorUpdate(args.Time);
            TestInput.endFrame();
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            if (Settings.s_IsEngine)
            {
                if(TestViewportWindow.IsMouseInsideViewport() && TestInput.MousePressed(MouseButton.Left))
                {
                    foreach (Gameobject go in _currentScene?.Gameobjects)
                    {
                        if (go.AABB(TestInput.getWorld()))
                        {
                            Engine.Get().CurrentSelectedAsset = go;
                            break;
                        }
                    }
                }

                testFB.Bind();
                GameRenderer.Render();
                testFB.UnBind();

                ImGuiController.Update(this, e.Time);
                ImGui.BeginMainMenuBar();
                if (ImGui.BeginMenu("Menu"))
                {
                    if (ImGui.MenuItem("Save Scene"))
                    {
                        SaveLoad.SaveScene(_currentScene);
                    }
                    if (ImGui.MenuItem("Load Scene"))
                    {

                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Help"))
                {
                    if (ImGui.MenuItem("Website"))
                    {
                        Utils.TryOpenUrl("https://github.com/kasperhbo/Engine2D");
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Settings"))
                {
                    if(ImGui.MenuItem("Engine Settings")){
                        engineSettingsWindow.SetVisibility(true);
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
                ImGui.DockSpaceOverViewport();
                ImGui.ShowDemoWindow();

                //gameViewport.OnGui(_frameBuffer.TextureID, () => { });
                viewportWindow.OnGui();
                cb.OnGui();
                
                testCamera.CameraSettingsGUI();

                foreach (UIElemenet window in _guiWindows.Values)
                {
                    window.Render();
                }

                ImGuiController.Render();

                ImGuiController.CheckGLError("End of frame");
                SwapBuffers();
                return;
            }
            _currentScene?.GameUpdate(e.Time);
            _currentScene?.Render(dt: e.Time);

            SwapBuffers();
        }
        
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            _currentScene?.OnResized(e);

            testCamera.adjustProjection();
            this.testFB = new TestFrameBuffer(ClientSize.X, ClientSize.Y);
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

        protected override void OnUnload()
        {
            _currentScene.IsPlaying = false;
            //_currentScene.OnClose();
            base.OnUnload();
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
            SaveLoad.SaveScene(_currentScene);
        }

        internal static void CreateNewProject(string newProjectLocation, string newProjectName)
        {
            ProjectSettings.s_ProjectName = newProjectName;
            ProjectSettings.s_ProjectLocation = newProjectLocation;
            ProjectSettings.s_FullProjectPath = ProjectSettings.s_ProjectLocation + ProjectSettings.s_ProjectName;

            SaveLoad.LoadScene(ProjectSettings.s_FullProjectPath + "\\defaultscenes\\example.kdbscene");
        }
               
        private void CreateUIWindows()
        {
            AssetBrowser assetBrowser = new AssetBrowser();
            _guiWindows.Add(assetBrowser.Title, assetBrowser);

            Inspector inspector = new Inspector();
            _guiWindows.Add(inspector.Title, inspector);

            SceneHierachy hierarch = new SceneHierachy(inspector);
            _guiWindows.Add(hierarch.Title, hierarch);

            
            _guiWindows.Add(engineSettingsWindow.Title, engineSettingsWindow);
        }


        internal TestFrameBuffer getFramebuffer()
        {
            return testFB;
        }

        internal float getWidth()
        {
            return ClientSize.X;
        }

        internal float getHeight()
        {
            return ClientSize.Y;
        }

        internal float getTargetAspectRatio()
        {
            return (float)getWidth() / getHeight();
        }
    }

}

public static class EngineSettings
{ 
    public static float GlobalScale = 1;
    public static float DefaultFontSize = 18;
}

public static class WindowSettings
{
    public static string s_Title = "Kasper Engine";
    public static Vector2i s_Size = new Vector2i(1920, 1080);
    
    public static float s_UpdateFrequency = 60;
    public static float s_RenderFrequency = 60;
}

public static class Settings
{
    public static bool s_IsEngine = true;
}

public static class ProjectSettings
{
    public static string s_ProjectName = "ExampleGame";
    public static string s_ProjectLocation = "D:\\dev\\EngineDev\\Engine2D\\src\\";
    public static string s_FullProjectPath = s_ProjectLocation + s_ProjectName;
}
