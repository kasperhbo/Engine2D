using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Engine2D.Scenes;
using Engine2D.Testing;
using Engine2D.UI;
using ImGuiNET;
using KDBEngine.UI;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace KDBEngine.Core
{
    // Be warned, there is a LOT of stuff here. It might seem complicated, but just take it slow and you'll be fine.
    // OpenGL's initial hurdle is quite large, but once you get past that, things will start making more sense.
    public class Engine : GameWindow
    {
        private static Engine _instance;
        
        //UI
        private readonly Dictionary<string, UIElemenet> _guiWindows = new();
        private readonly EngineSettingsWindow engineSettingsWindow = new();
        internal ImGuiController ImGuiController;
        private TestViewportWindow viewportWindow;
        
        internal Scene? _currentScene;
        public Asset? CurrentSelectedAsset;
        
#region setup
        public Engine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(
            gameWindowSettings, nativeWindowSettings)
        {
        }

        public static Engine Get()
        {
            if (_instance == null)
            {
                var gameWindowSettings = GameWindowSettings.Default;
                gameWindowSettings.UpdateFrequency = WindowSettings.s_UpdateFrequency;
                gameWindowSettings.RenderFrequency = WindowSettings.s_RenderFrequency;

                var ntwSettings = NativeWindowSettings.Default;
                ntwSettings.Title = WindowSettings.s_Title;
                ntwSettings.Size = WindowSettings.s_Size;

                var window = new Engine(gameWindowSettings, ntwSettings);
                _instance = window;
            }

            return _instance;
        }

        public static Engine Get(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        {
            if (_instance == null)
            {
                var window = new Engine(gameWindowSettings, nativeWindowSettings);
                _instance = window;
            }

            return _instance;
        }
#endregion
        protected override void OnLoad()
        {
            base.OnLoad();
            
            SaveLoad.LoadEngineSettings();
            ImGuiController = new ImGuiController(Size.X, Size.Y);

            viewportWindow = new TestViewportWindow();
            
            _currentScene = new Scene();
            _currentScene.Init(ProjectSettings.s_FullProjectPath + "\\kasper1.kdbscene");
            
            if (Settings.s_IsEngine) CreateUIWindows();
        }


        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            //Input.Update(KeyboardState, MouseState);
            TestInput.mousePosCallback(MouseState, KeyboardState, _currentScene?.TestCamera);

            _currentScene?.EditorUpdate(args.Time);

            TestInput.endFrame();
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            //Set title to fps + title
            SetTitle((float)e.Time);
            
            _currentScene?.Render();
            
            //If engine render UI
            if (Settings.s_IsEngine) 
            {
                //ImGui
                {
                    ImGuiController.Update(this, e.Time);

                    #region Menu

                    ImGui.BeginMainMenuBar();
                    if (ImGui.BeginMenu("Menu"))
                    {
                        if (ImGui.MenuItem("Save Scene")) SaveLoad.SaveScene(_currentScene);
                        if (ImGui.MenuItem("Load Scene"))
                        {
                        }

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Help"))
                    {
                        if (ImGui.MenuItem("Website")) Utils.TryOpenUrl("https://github.com/kasperhbo/Engine2D");
                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("Settings"))
                    {
                        if (ImGui.MenuItem("Engine Settings")) engineSettingsWindow.SetVisibility(true);
                        ImGui.EndMenu();
                    }
                    
                    ImGui.EndMainMenuBar();
                    
                    #endregion
                    
                    ImGui.DockSpaceOverViewport();
                    ImGui.ShowDemoWindow();


                    foreach (var window in _guiWindows.Values) window.Render();

                    _currentScene?.OnGui(viewportWindow);
                    ImGuiController.Render();

                    ImGuiController.CheckGLError("End of frame");
                }
            }

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            
            _currentScene?.OnResized(e);
            
            if (Settings.s_IsEngine)
            {
                ImGuiController.WindowResized(ClientSize.X, ClientSize.Y);
            }
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            _currentScene?.OnTextInput(e);
            
            if (Settings.s_IsEngine)
            {
                //UPDATE UI
                ImGuiController.PressChar((char)e.Unicode);
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _currentScene?.OnMouseWheel(e);
            
            if(Settings.s_IsEngine)
            {
                //UPDATE UI
                ImGuiController.MouseScroll(e.Offset);
            }
        }

        protected override void OnUnload()
        {
            if (_currentScene != null)
            {
                _currentScene.IsPlaying = false;
                _currentScene.OnClose();
            }

            base.OnUnload();
        }

        private void CreateUIWindows()
        {
            var assetBrowser = new AssetBrowser();
            _guiWindows.Add(assetBrowser.Title, assetBrowser);

            var inspector = new Inspector();
            _guiWindows.Add(inspector.Title, inspector);

            var hierarch = new SceneHierachy();
            _guiWindows.Add(hierarch.Title, hierarch);

            _guiWindows.Add(engineSettingsWindow.Title, engineSettingsWindow);
        }

        internal float TargetAspectRatio()
        {
            var res = (float)ClientSize.X /(float)ClientSize.Y;
            return res;
        }
        
        private int _frameCounter;
        private void SetTitle(float time)
        {
            var fps = 1.0f / time;
            _frameCounter++;
            if (_frameCounter == 30)
            {
                Title = string.Format("KDB ENGIN V{0} | Scene : {1} | FPS : {2}", 0.1, _currentScene.ScenePath, fps);
                _frameCounter = 0;
            }
        }
    }
}

public static class EngineSettings
{
    public static float GlobalScale = 1;
    public static float DefaultFontSize = 18;
    public static bool SaveOnClose = false;
}

public static class WindowSettings
{
    public static string s_Title = "Kasper Engine";
    public static Vector2i s_Size = new(1920, 1080);

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