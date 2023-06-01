using Engine2D.Cameras;
using Engine2D.SavingLoading;
using Engine2D.Scenes;
using Engine2D.Testing;
using Engine2D.UI;
using Engine2D.UI.Viewports;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ImGuiController = Dear_ImGui_Sample.ImGuiController;

namespace Engine2D.Core
{
    public class Engine : GameWindow
    {
        private static Engine? s_instance;
        
        public  Scene? CurrentScene { get; private set; }
        public Asset? CurrentSelectedAsset;
        

        private readonly Dictionary<string, UiElemenet> _guiWindows = new();
        private readonly EngineSettingsWindow engineSettingsWindow = new();

        private ImGuiController _imGuiController;

        private EditorViewport _editorViewport;
        private GameViewport _gameViewport;

        #region setup

        public Engine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(
            gameWindowSettings, nativeWindowSettings)
        {
        }

        public unsafe static Engine? Get()
        {
            if (s_instance == null)
            {
                var gameWindowSettings = GameWindowSettings.Default;
                gameWindowSettings.UpdateFrequency = WindowSettings.UpdateFrequency;
                gameWindowSettings.RenderFrequency = WindowSettings.RenderFrequency;

                var ntwSettings = NativeWindowSettings.Default;
                ntwSettings.Title = WindowSettings.Title;
                ntwSettings.Size = WindowSettings.Size;

                s_instance = new Engine(gameWindowSettings, ntwSettings);

                s_instance.LoadEngine();

                GLFW.SetWindowAttrib(s_instance.WindowPtr, WindowAttribute.Decorated, WindowSettings.Decorated);
            }

            return s_instance;
        }


        private void LoadEngine()
        {
            base.OnLoad();

            SaveLoad.LoadEngineSettings();

            if (Settings.s_IsEngine)
            {
                LoadEditor();
            }

            Get().WindowState = WindowSettings.FullScreen;

            AssignDefaultEvents();

            SwitchScene(ProjectSettings.FullProjectPath + "\\kasper1.kdbscene");

            if (Settings.s_IsEngine)
            {
                LoadViewports();
            }
        }

        private void AssignDefaultEvents()
        {
            //Updates
            base.UpdateFrame += Update;
            base.RenderFrame += Render;

            //Keyboards
            base.MouseWheel += MouseWheel;
            base.TextInput  += TextInput;
            
            //other events
            base.Resize += OnResize;
            base.Unload += Close;
            
        }
        
        private void LoadEditor()
        {
            RenderFrame += RenderUI;
            
            _imGuiController = new ImGuiController(ClientSize.X, ClientSize.Y);
            
            base.MouseWheel += _imGuiController.MouseWheel;
            base.TextInput += _imGuiController.PressChar;
            base.Resize += _imGuiController.WindowResized;
            CreateUIWindows();
        }

        private void LoadViewports()
        {
            _editorViewport = new EditorViewport();
            _gameViewport = new GameViewport();
        }
        
        #endregion

        public void SwitchScene(string newSceneName)
        {
            if (CurrentScene != null)
            {
                base.TextInput  -= CurrentScene.OnTextInput;
                base.MouseWheel -= CurrentScene.OnMouseWheel;
                base.Unload     -= CurrentScene.Close;
                
                foreach (var eventA in CurrentScene.GetDefaultUpdateEvents())
                {
                    UpdateFrame -= eventA;
                }
            }

            CurrentScene = new Scene();
            
            base.TextInput  += CurrentScene.OnTextInput;
            base.MouseWheel += CurrentScene.OnMouseWheel;
            base.Unload     += CurrentScene.Close;

            foreach (var eventA in CurrentScene.GetDefaultUpdateEvents())
            {
                UpdateFrame += eventA;
            }
            
            CurrentScene.Init(newSceneName);
        }

        private void Update(FrameEventArgs args)
        {
        }

        private void Render(FrameEventArgs args)
        {
            SetTitle((float)args.Time);
            CurrentScene?.Render(args);
            SwapBuffers();
        }
        
        private void RenderUIWindows()
        {
            foreach (var window in _guiWindows.Values) window.Render();

            CurrentScene?.OnGui();

            Camera? cam = CurrentScene?.EditorCamera;
            
            _editorViewport.OnGui(cam, "Editor");

            // _gameViewport.OnGui(frameBuffer, cam);
        }

        private new void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

            CurrentScene?.OnResized(e);
        }

        private void TextInput(TextInputEventArgs e)
        {
        }

        private void MouseWheel(MouseWheelEventArgs e)
        {
        }


        private void RenderUI(FrameEventArgs e)
        {
            _imGuiController.Update(this, e.Time);

            SetupDockspace();

            _imGuiController.Render();
        }

        private void SetupDockspace()
        {
            float tabBarSize = 30;
            
            ImGui.DockSpaceOverViewport();
            RenderUIWindows();
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
                Title = string.Format("KDB ENGIN V{0} | Scene : {1} | FPS : {2}", 0.1, CurrentScene.ScenePath, fps);
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