using System.Numerics;
using Dear_ImGui_Sample;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Engine2D.Scenes;
using Engine2D.Testing;
using Engine2D.UI;
using Engine2D.UI.Viewports;
using ImGuiNET;
using KDBEngine.UI;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ImGuiController = Dear_ImGui_Sample.ImGuiController;

namespace KDBEngine.Core
{
    // Be warned, there is a LOT of stuff here. It might seem complicated, but just take it slow and you'll be fine.
    // OpenGL's initial hurdle is quite large, but once you get past that, things will start making more sense.
    public class Engine : GameWindow
    {
        private static Engine _instance;
        
        private readonly Dictionary<string, UIElemenet> _guiWindows = new();
        private readonly EngineSettingsWindow engineSettingsWindow = new();
        
        private ImGuiController _imGuiController;
        
        private EditorViewport _editorViewport;
        private ViewportWindow _gameViewport;
        
        internal Scene? _currentScene;
        public Asset? CurrentSelectedAsset;
        
#region setup

        public Engine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(
            gameWindowSettings, nativeWindowSettings)
        {
        }

        public unsafe static Engine Get()
        {
            if (_instance == null)
            {
                var gameWindowSettings             = GameWindowSettings.Default;
                gameWindowSettings.UpdateFrequency = WindowSettings.UpdateFrequency;
                gameWindowSettings.RenderFrequency = WindowSettings.RenderFrequency;

                var ntwSettings   = NativeWindowSettings.Default;
                ntwSettings.Title = WindowSettings.Title;
                ntwSettings.Size  = WindowSettings.Size;
                
                _instance = new Engine(gameWindowSettings, ntwSettings);
                
                GLFW.SetWindowAttrib(_instance.WindowPtr, WindowAttribute.Decorated, WindowSettings.Decorated);
            }

            return _instance;
        }
        

        protected override void OnLoad()
        {
            base.OnLoad();
            
            SaveLoad.LoadEngineSettings();
            _currentScene = new Scene();
            _currentScene.Init(ProjectSettings.FullProjectPath + "\\kasper1.kdbscene");
            
            if (Settings.s_IsEngine)
                LoadEditor();
            
            Get().WindowState = WindowSettings.FullScreen;
            
        }

        private void LoadEditor()
        {
            _imGuiController = new ImGuiController(ClientSize.X, ClientSize.Y);
            _editorViewport = new("Editor VP");
            _gameViewport = new("Game VP");
            CreateUIWindows();
        }

#endregion

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            TestInput.mousePosCallback(MouseState, KeyboardState, _currentScene?.EditorCamera);

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
                    _imGuiController.Update(this, (float)e.Time);
                    
                    ImGui.SetNextWindowPos(new(0, 0));
                    ImGui.SetNextWindowSize(new(ClientSize.X, ClientSize.Y));
                    
                    ImGui.Begin("MainWindow",  
                        ImGuiWindowFlags.NoDecoration 
                        | ImGuiWindowFlags.NoNavFocus 
                        | ImGuiWindowFlags.NoFocusOnAppearing 
                        | ImGuiWindowFlags.NoBringToFrontOnFocus 
                        | ImGuiWindowFlags.NoDocking
                        | ImGuiWindowFlags.NoScrollbar
                        | ImGuiWindowFlags.NoCollapse
                        | ImGuiWindowFlags.NoScrollWithMouse
                        | ImGuiWindowFlags.NoMove);

                    ImGui.SetCursorPos(new(0,0));
                    
                    ImGui.BeginTabBar("MainTab");
                    ImGui.SetCursorPos(new(0,0));
                    ImGui.DockSpace(99999, new(ClientSize.X, ClientSize.Y),  ImGuiDockNodeFlags.DockSpace);
                    ImGui.EndTabBar();

                    ImGui.ShowDemoWindow();
                    foreach (var window in _guiWindows.Values) window.Render();

                    _currentScene?.OnGui();
                    
                    TestCamera? cam = _currentScene?.EditorCamera;
                    TestFrameBuffer? frameBuffer = _currentScene?.Renderer.EditorGameBuffer;
                    
                    _editorViewport.OnGui(frameBuffer, cam);
                    
                    cam = _currentScene?.CurrentMainGameCamera;
                    frameBuffer = _currentScene?.Renderer.GameBuffer;
                    
                    _gameViewport.OnGui(frameBuffer, cam);

                    ImGui.End();

                    _imGuiController.Render();
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
                _imGuiController.WindowResized(ClientSize.X, ClientSize.Y);
            }
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            _currentScene?.OnTextInput(e);
            
            if (Settings.s_IsEngine)
            {
                //UPDATE UI
                _imGuiController.PressChar((char)e.Unicode);
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _currentScene?.OnMouseWheel(e);
            
            if(Settings.s_IsEngine)
            {
                //UPDATE UI
                _imGuiController.MouseScroll(e.Offset);
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
            TopMenu top = new TopMenu();
            
            var assetBrowser = new AssetBrowser();
            _guiWindows.Add(assetBrowser.Title, assetBrowser);

            var inspector = new Inspector();
            _guiWindows.Add(inspector.Title, inspector);

            var hierarch = new SceneHierachy();
            _guiWindows.Add(hierarch.Title, hierarch);

            _guiWindows.Add(engineSettingsWindow.Title, engineSettingsWindow);
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
    public static float GlobalScale      = 1;
    public static float DefaultFontSize  = 18;
    public static bool SaveOnClose      = false;
}

public static class WindowSettings
{
    public static string Title            { get; }= "Kasper Engine";
    public static Vector2i Size           { get; }= new(1920, 1080);
    public static float UpdateFrequency   { get; }= 60;
    public static float RenderFrequency   { get; }= 60;
    public static WindowState FullScreen  { get; }= WindowState.Maximized ;
    public static bool Decorated          { get; } = true;
}

public static class Settings
{
    public static bool s_IsEngine = true;
}

public static class ProjectSettings
{
    public static string ProjectName     {get;} = "ExampleGame";
    public static string ProjectLocation {get;} = "D:\\dev\\EngineDev\\Engine2D\\src\\";
    public static string FullProjectPath {get;} = ProjectLocation + ProjectName;
}