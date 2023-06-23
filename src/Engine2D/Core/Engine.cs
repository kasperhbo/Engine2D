﻿#region

using Engine2D.Components.SpriteAnimations;
using Engine2D.Components.Sprites;
using Engine2D.Components.Sprites.SpriteAnimations;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Engine2D.Scenes;
using Engine2D.UI;
using Engine2D.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

#endregion

namespace Engine2D.Core
{
    public class Engine : GameWindow
    {
        private static Engine? s_instance;

        private readonly Dictionary<string, UIElement> _guiWindows = new();

        private int _frameCounter;
        private double _previousTime;
        
        internal Asset? CurrentSelectedAsset;
        
        
        internal SpriteSheet? CurrentSelectedSpriteSheetAssetBrowserAsset { get; set; }= null;
        internal Texture? CurrentSelectedTextureAssetBrowserAsset     { get; set; } = null;
        internal Animation? CurrentSelectedAnimationAssetBrowserAsset   { get; set; } = null;
        

        internal Scene? CurrentScene { get; private set; }

        internal void SwitchScene(string newSceneName)
        {
            if (CurrentScene != null)
            {
                base.TextInput -= CurrentScene.OnTextInput;
                base.MouseWheel -= CurrentScene.OnMouseWheel;
                Unload -= CurrentScene.Close;
            }

            CurrentScene = new Scene();

            base.TextInput += CurrentScene.OnTextInput;
            base.MouseWheel += CurrentScene.OnMouseWheel;
            Unload += CurrentScene.Close;

            CurrentScene.Init(newSceneName);
            if (!Settings.s_IsEngine)
            {
                CurrentScene.IsPlaying = true;
            }

        }

        private void Update(FrameEventArgs args)
        {
            CurrentScene?.Update(args);
        }

        public static double DeltaTime = 0;
        
        private void Render(FrameEventArgs args)
        {
            
            // Calculate delta time
            double currentTime = TimeSinceStart;
            DeltaTime = currentTime - _previousTime;
            _previousTime = currentTime;
            
            SetTitle((float)DeltaTime);
            
            CurrentScene?.Render((float)DeltaTime);
            
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

        #region setup

        internal Engine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(
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

                s_instance.LoadProject();
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

            _previousTime = TimeSinceStart;
        }

        private void LoadProject()
        {
            AssemblyUtils.LoadAssembly(@"D:\dev\Engine2D\src\ExampleGame\bin\Debug\net7.0\ExampleGame.dll");
            AssemblyUtils.GetComponent("ExampleGame.Assets.TestClass");
        }

        private void AssignDefaultEvents()
        {
            //Updates
            UpdateFrame += Update;
            RenderFrame += Render;

            //Keyboards
            base.MouseWheel += MouseWheel;
            base.TextInput += TextInput;

            //other events
            Resize += OnResize;
            Unload += OnClose;
        }

        private void OnClose()
        {
            Log.Message("Closing engine");
            SaveLoad.SaveWindowSettings();
            SaveLoad.SaveEngineSettings();
        }
        
        private double TimeSinceStart
        {
            get { return (double)DateTime.Now.Ticks / TimeSpan.TicksPerSecond; }
        }

        

        #endregion
    }
}

internal static class EngineSettings
{
    internal static float DefaultFontSize = 18;
    internal static bool SaveOnClose = true;
}

internal static class WindowSettings
{
    internal static string Title { get; } = "Kasper Engine";
    internal static Vector2i Size { get; } = new(1920, 1080);
    internal static float UpdateFrequency { get; } = -1;
    internal static float RenderFrequency { get; } = -1;
    internal static WindowState FullScreen { get; } = WindowState.Maximized;
    internal static bool Decorated { get; } = true;
}

public static class Settings
{
    public static bool s_IsEngine = true;
    public static bool s_RenderDebugWindowSeperate = true;
}

internal static class ProjectSettings
{
    //TODO: Make this a json file
    //TODO: MAKE THIS ACCESSIBLE FROM A LAUNCHER
    internal static string ProjectName { get; } = "ExampleGame";
    internal static string ProjectLocation { get; } = @"D:\dev\Engine2D\src\";
    internal static string FullProjectPath { get; } = ProjectLocation + ProjectName;
}