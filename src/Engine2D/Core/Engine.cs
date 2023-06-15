using System.Numerics;
using Dear_ImGui_Sample;
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

namespace Engine2D.Core
{
    public class Engine : GameWindow
    {
        private static Engine? s_instance;
        
        public  Scene? CurrentScene { get; private set; }
        public AssetBrowserAsset? CurrentSelectedAssetBrowserAsset;
        public Asset? CurrentSelectedAsset;

        private readonly Dictionary<string, UIElement> _guiWindows = new();

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
            SaveLoad.LoadWindowSettings();
            SaveLoad.LoadEngineSettings();

            base.OnLoad();

            WindowState = WindowSettings.FullScreen;

            AssignDefaultEvents();

            SwitchScene(ProjectSettings.FullProjectPath + "\\kasper1.kdbscene");

            if (Settings.s_IsEngine)
                UiRenderer.Init(this, true);

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
            base.Unload += OnClose;
        }

        private void OnClose()
        {
            Log.Message("Closing engine");
            SaveLoad.SaveWindowSettings();
            SaveLoad.SaveEngineSettings();
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
    public static string ProjectLocation {get;} = @"D:\dev\Engine2D\src\";
    public static string FullProjectPath {get;} = ProjectLocation + ProjectName;
}