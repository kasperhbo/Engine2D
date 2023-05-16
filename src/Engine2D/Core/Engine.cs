using Engine2D.Core;
using Engine2D.GameObjects;
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
        private static readonly int small = 1;

        private static Engine _instance;

        internal Scene? _currentScene;

        private readonly Dictionary<string, UIElemenet> _guiWindows = new();

        private TestContentBrowser cb;

        internal Asset? CurrentSelectedAsset;

        //TODO: MAKE THIS STATIC AND SAVABLE
        private readonly EngineSettingsWindow engineSettingsWindow = new();

        private GameViewport gameViewport = new();
        private readonly float height = 1080 / small;
        internal ImGuiController ImGuiController;
        internal TestCamera testCamera;

        private TestFrameBuffer testFB;
        private TestViewportWindow viewportWindow;
        private readonly float width = 1920 / small;

        public Vector2 LightLocation = new(0, 0);
        public SpriteColor LightColor = new(0, 0, 0,0);
        public float intensity = 0;
        public Vector2 LightLocation2 = new(0, 0);
        public float intensity2 = 0;
        public SpriteColor LightColor2 = new(0, 0, 0,0);
        
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

        protected override void OnLoad()
        {
            base.OnLoad();

            Log.Message("Message", showFile: true, showLine: true, showFunction: true);
            Log.Succes("Succes");
            Log.Warning("Warning");
            Log.Error("Error");

            SaveLoad.LoadEngineSettings();

            ImGuiController = new ImGuiController(Size.X, Size.Y);

            if (Settings.s_IsEngine) CreateUIWindows();

            SaveLoad.LoadScene(ProjectSettings.s_FullProjectPath + "\\kasper1.kdbscene");
            TestInput.Init();

            testFB = new TestFrameBuffer((int)width, (int)height);
            testCamera = new TestCamera();
            viewportWindow = new TestViewportWindow();
            cb = new TestContentBrowser();


            if (!Settings.s_IsEngine) _currentScene.IsPlaying = true;
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
                if (TestViewportWindow.IsMouseInsideViewport() && TestInput.MousePressed(MouseButton.Left))
                    foreach (var go in _currentScene?.Gameobjects)
                        if (go.AABB(TestInput.getWorld()))
                        {
                            Get().CurrentSelectedAsset = go;
                            break;
                        }

                if (CurrentSelectedAsset != null)
                {
                }

                testFB.Bind();
                GameRenderer.Render();
                testFB.UnBind();

                ImGuiController.Update(this, e.Time);
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
                ImGui.DockSpaceOverViewport();
                ImGui.ShowDemoWindow();

                //gameViewport.OnGui(_frameBuffer.TextureID, () => { });
                viewportWindow.OnGui();
                cb.OnGui();
                ImGui.Begin("test");
                //ImageTextIcon img = new ImageTextIcon();

                //if (OpenTKUIHelper.DrawIconWithText("LABEL", dirTexture))
                //    Console.WriteLine("double");
                LightLocation = OpenTKUIHelper.DrawProperty("light1", LightLocation);
                OpenTKUIHelper.DrawProperty("light1Intensity", ref intensity);
                OpenTKUIHelper.DrawProperty("light1Color", ref LightColor);
                LightLocation2 = OpenTKUIHelper.DrawProperty("light2", LightLocation2);
                OpenTKUIHelper.DrawProperty("light2Intensity", ref intensity2);
                OpenTKUIHelper.DrawProperty("light2Color", ref LightColor2);
                
                ImGui.End();

                testCamera.CameraSettingsGUI();

                foreach (var window in _guiWindows.Values) window.Render();

                _currentScene.OnGui();

                ImGuiController.Render();

                ImGuiController.CheckGLError("End of frame");
                SwapBuffers();
                return;
            }

            _currentScene?.GameUpdate(e.Time);
            _currentScene?.Render(e.Time);

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            _currentScene?.OnResized(e);

            testCamera.adjustProjection();
            testFB = new TestFrameBuffer(ClientSize.X, ClientSize.Y);
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
            Title = WindowSettings.s_Title + " | " + sceneName;
            Scene newScene = new();
            newScene.Init(this, sceneName, Size.X, Size.Y);
            _currentScene = newScene;
        }

        internal void NewScene(string sceneName)
        {
            GameRenderer.Flush();
            Title = WindowSettings.s_Title + " | " + sceneName;
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
            var assetBrowser = new AssetBrowser();
            _guiWindows.Add(assetBrowser.Title, assetBrowser);

            var inspector = new Inspector();
            _guiWindows.Add(inspector.Title, inspector);

            var hierarch = new SceneHierachy();
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
            return getWidth() / getHeight();
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