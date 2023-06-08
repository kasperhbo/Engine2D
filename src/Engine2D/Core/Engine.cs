using System.Numerics;
using Engine2D.Logging;
using Engine2D.SavingLoading;
using Engine2D.Scenes;
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

        private void AddGuiWindow(UiElemenet window)
        {
            _guiWindows.Add(window.Title,window);
            FileDrop += window.OnFileDrop;
        }

        private void RemoveGuiWindow(UiElemenet window)
        {
            FileDrop -= window.OnFileDrop;
            _guiWindows.Remove(window.Title);
        }
        
        private readonly EngineSettingsWindow engineSettingsWindow = new();

        private ImGuiController _imGuiController;

        private EditorViewport? _editorViewport = null;

        public EditorViewport? CurrentEditorViewport
        {
            get
            {
                if (_editorViewport == null)
                {
                    Log.Error("There is currently no editor vp, please open one firts");
                    return null;
                }
                
                //TODO: REPLACE THIS FOR MULTIPLE VPS
                return _editorViewport;
            } 
        }
        
        private GameViewport _gameViewport;

        #region setup

        public Engine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(
            gameWindowSettings, nativeWindowSettings)
        {
            
        }

        public static unsafe Engine Get()
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
                LoadEditor();

            Get().WindowState = WindowSettings.FullScreen;

            AssignDefaultEvents();

            SwitchScene(ProjectSettings.FullProjectPath + "\\kasper1.kdbscene");

            if (Settings.s_IsEngine || Settings.s_RenderDebugWindowSeperate)
                LoadViewports();
            
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
            foreach (var window in _guiWindows.Values)
            {
                window.Render();
            }
            
            CurrentScene?.OnGui();
            
            _editorViewport.Begin("Editor", CurrentScene?.EditorCamera,
                CurrentScene?.Renderer?.EditorGameBuffer);
            
            _gameViewport.Begin("Game", CurrentScene.CurrentMainGameCamera,
                CurrentScene.Renderer.GameBuffer);
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
            DrawMainMenuBar();
            DrawToolbar();
            SetupDockspace();

            _imGuiController.Render();
        }

        private float _titleBarHeight = 45;
        
        private unsafe void SetupDockspace()
        {
            ImGuiWindowFlags host_window_flags = 0;
            host_window_flags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDocking;
            host_window_flags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
            
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0,55));
            ImGui.SetNextWindowSize(
                new System.Numerics.Vector2(ClientSize.X, ClientSize.Y - 55));
            
            string label = "";

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(0.0f, 0.0f));

            bool open = false;
            ImGui.Begin("DockspaceViewport", ref open, host_window_flags);
            ImGui.PopStyleVar(3);

            ImGuiDockNodeFlags flags = ImGuiDockNodeFlags.None;

            uint id = ImGui.GetID("DockSpace");
            ImGui.DockSpace(id, new System.Numerics.Vector2(0, 0), flags
            );
            ImGui.End();

            RenderUIWindows();
        }

        private void DrawMainMenuBar()
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(ClientSize.X, 55));
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, 1));
            ImGui.Begin("titlebar", ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoNav );

            ImGui.BeginMenuBar(); 
            ImGui.MenuItem("File");
            ImGui.MenuItem("Windows");
            ImGui.MenuItem("Help");
            ImGui.EndMenuBar();
        }
        
        private void DrawToolbar()
        {   
            //ImGui.Begin("Toolbar", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoNav );
            ImGui.Button("Button");
            ImGui.SameLine();
            ImGui.Button("Button2");
            ImGui.End();
        }


        private void CreateUIWindows()
        {
            var assetBrowser = new AssetBrowser();
            AddGuiWindow(assetBrowser);
            
            var assetBrowser2 = new UI.Browsers.AssetBrowser();
            AddGuiWindow(assetBrowser2);
            
            var inspector = new Inspector();
            AddGuiWindow(inspector);
            
            var hierarch = new SceneHierachy();
            AddGuiWindow(hierarch);

            var uiSettingsPanel = new UISettingsPanel();
            AddGuiWindow(uiSettingsPanel);
            
            AddGuiWindow(engineSettingsWindow);
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
     public static bool SaveOnClose       = true;
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
    public static bool s_RenderDebugWindowSeperate = true;
}

public static class ProjectSettings
{
    public static string ProjectName     {get;} = "ExampleGame";
    public static string ProjectLocation {get;} = "D:\\dev\\EngineDev\\Engine2D\\src\\";
    public static string FullProjectPath {get;} = ProjectLocation + ProjectName;
}