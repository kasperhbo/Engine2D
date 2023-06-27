#region

using System.Numerics;
using Dear_ImGui_Sample;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.UI.Browsers;
using Engine2D.UI.Viewports;
using Engine2D.Utilities;
using ImGuiNET;
using OpenTK.Windowing.Common;

#endregion

namespace Engine2D.UI;

internal static class UiRenderer
{
    private static List<UIElement> _windows = new();
    private static Engine _engine = null!;
    private static EditorViewport? _editorViewport;
    private static List<UIElement> _windowsToRemoveEndOfFrame = new();

    internal static int _hierarchyWindowCount = 1;
    internal static int _inspectorWindowCount = 1;
    internal static int _assetBrowserWindowCount = 1;
    internal static int _styleSettingsWindowCount = 1;

    private static GameViewport? _gameViewport;

    public static void Flush()
    {
        List<UIElement> _windows = new();
        Engine _engine = null!;
        EditorViewport? _editorViewport;
        List<UIElement> _windowsToRemoveEndOfFrame = new(); 
         int _hierarchyWindowCount = 1;
         int _inspectorWindowCount = 1;
         int _assetBrowserWindowCount = 1;
         int _styleSettingsWindowCount = 1; 
        GameViewport? _gameViewport;
    }

    internal static EditorViewport? CurrentEditorViewport
    {
        get
        {
            if (_editorViewport == null)
            {
                Log.Error("There is currently no editor vp, please open one first");
                return null;
            }

            return _editorViewport;
        }
    }

    internal static void Init(Engine engine, bool createDefaultWindows)
    {
        _engine = engine;

        _engine.UpdateFrame += Update;
        _engine.RenderFrame += Render;
        _engine.MouseWheel += OnMouseWheel;
        _engine.TextInput += OnTextInput;
        _engine.Resize += OnResize;

        _windows = new List<UIElement>();

        KDBImGuiController.Init(Engine.Get().Size.X, Engine.Get().Size.Y);

        if (createDefaultWindows) CreateDefaultWindows();
    }
    

    private static void Update(FrameEventArgs args)
    {
    }
    
    
    private static void Render(FrameEventArgs args)
    {
        if (!Settings.s_IsEngine) return;
        KDBImGuiController.Update(_engine, args.Time);

        ImGui.Begin("Debug Helper");
        ImGui.BeginChild("Renderer", new(-1, 90), true);
        
        ImGui.Text($"FPS: {1 / args.Time:0.00}");
        ImGui.Text($"Frame Time: {args.Time * 1000:0.00}ms");
        ImGui.Text("Render batches: " + Engine.Get().CurrentScene.Renderer.RenderBatches.Count);
        
        ImGui.EndChild();
        if (ImGui.Button("Reload Assembly")) AssemblyUtils.Reload();

        ImGui.End();
        
        DrawMainMenuBar();
        
        SetupDockSpace();

        Engine.Get().CurrentScene.Renderer.OnGui();
        Engine.Get().CurrentSelectedSpriteSheetAssetBrowserAsset?.OnGui(); 
        Engine.Get().CurrentSelectedTextureAssetBrowserAsset?.OnGui();     
        Engine.Get().CurrentSelectedAnimationAssetBrowserAsset?.OnGui();   
        

        ImGui.ShowDemoWindow();

        KDBImGuiController.Render();
        
        ResourceManager.OnGUI();
        
        
    }


    private static void OnMouseWheel(MouseWheelEventArgs args)
    {
        KDBImGuiController.MouseWheel(args);
    }

    private static void OnTextInput(TextInputEventArgs args)
    {
        KDBImGuiController.PressChar(args);
    }

    private static void OnResize(ResizeEventArgs args)
    {
        KDBImGuiController.WindowResized(args);
    }

    private static void RenderUiWindows()
    {
        foreach (var window in _windows)
            // if (window.IsVisible)
            // {
        {
            window.BeginRender();
            window.Render();
            window.EndRender();
        }
            //}

        _engine.CurrentScene?.OnGui();

        _editorViewport?.Begin("Editor VP", _engine.CurrentScene?.EditorCamera,
            _engine.CurrentScene?.Renderer?.EditorGameBuffer);

        _gameViewport?.Begin("Game VP", _engine.CurrentScene?.CurrentMainGameCamera,
            _engine.CurrentScene?.Renderer?.GameBuffer);

        foreach (var window in _windowsToRemoveEndOfFrame) _windows.Remove(window);

        _windowsToRemoveEndOfFrame = new List<UIElement>();
    }

    internal static void AddGuiWindow(UIElement window)
    {
        Engine.Get().FileDrop += window.FileDrop;
        _windows.Add(window);
    }

    internal static void RemoveGuiWindow(UIElement window)
    {
        if (window.GetType() == typeof(SceneHierachyPanel)) _hierarchyWindowCount--;
        if (window.GetType() == typeof(InspectorPanel)) _inspectorWindowCount--;
        if (window.GetType() == typeof(AssetBrowserPanel))
        {
            AssetBrowserPanel.AssetBrowserPanels.Remove((AssetBrowserPanel)window);
            _assetBrowserWindowCount--;
        }

        ;
        if (window.GetType() == typeof(StyleSettingsPanel)) _styleSettingsWindowCount--;

        Engine.Get().FileDrop -= window.FileDrop;

        // _engine.FileDrop -= window.OnFileDrop;
        _windowsToRemoveEndOfFrame.Add(window);
    }

    private static void SetupDockSpace()
    {
        ImGuiWindowFlags hostWindowFlags = 0;
        hostWindowFlags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize |
                           ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDocking;
        hostWindowFlags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;

        ImGui.SetNextWindowPos(new Vector2(0, 32));
        ImGui.SetNextWindowSize(
            new Vector2(_engine.Size.X, _engine.Size.Y - 32));

        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));

        var open = false;
        ImGui.Begin("DockSpaceViewport", ref open, hostWindowFlags);
        ImGui.PopStyleVar(3);
        var id = ImGui.GetID("DockSpace");
        ImGui.DockSpace(id, new Vector2(0, 0));
        ImGui.End();

        RenderUiWindows();
    }

    private static bool _isHoveringToolbar = false;
    private static void DrawMainMenuBar()
    {
        _isHoveringToolbar = false;
        ImGui.SetNextWindowSize(new Vector2(_engine.Size.X, 20));
        ImGui.SetNextWindowPos(new Vector2(0, 0));
        
        ImGui.Begin("titlebar", ImGuiWindowFlags.NoBringToFrontOnFocus 
                                | ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoCollapse 
            | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.MenuBar |
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoNav);

        {
            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    Log.Error("Not Implemented");
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("View"))
                {
                    if (ImGui.MenuItem("Asset Browser")) CreateAssetBrowserWindow();

                    if (ImGui.BeginMenu("Settings"))
                    {
                        if (ImGui.MenuItem("Style Settings")) CreateStyleSettingsWindow();

                        ImGui.EndMenu();
                    }

                    if (ImGui.BeginMenu("View"))
                    {
                        if (ImGui.MenuItem("Hierarchy")) CreateHierachyWindow();

                        if (ImGui.MenuItem("Inspector")) CreateInspectWindow();

                        if (ImGui.BeginMenu("Layouts"))
                        {
                            if (ImGui.MenuItem("Default"))
                                ImGui.LoadIniSettingsFromDisk(Utils.GetBaseEngineDir() + "\\Layouts\\Default.ini");
                            if (ImGui.MenuItem("Layout01"))
                                ImGui.LoadIniSettingsFromDisk(Utils.GetBaseEngineDir() + "\\Layouts\\Layout01.ini");
                            ImGui.EndMenu();
                        }

                        ImGui.EndMenu();
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Help"))
                {
                    if (ImGui.MenuItem("Website")) Log.Error("Not Implemented");
                    if (ImGui.MenuItem("WIKI")) Log.Error("Not Implemented");
                    ImGui.EndMenu();
                }

                ImGui.EndMenuBar();
            }
        }

        {
            // ImGui.SetNextWindowPos(new Vector2(ImGui.GetContentRegionMax().X - 25 * 2, 1));
            // ImGui.BeginChild("Buttons", new(25 * 2, 26), true, ImGuiWindowFlags.NoScrollbar);
            // ImGui.SetCursorPosX(0.5f);
            // ImGui.SetCursorPosY(0.5f);
            // ImGui.Button("X", new Vector2(22));
            // ImGui.SameLine();
            // ImGui.SetCursorPosY(0.5f);
            // ImGui.Button("X", new Vector2(22));
            // ImGui.EndChild();
        }
        
        if (ImGui.IsWindowHovered())
        {
            _isHoveringToolbar = true;
        }
        
        ImGui.End();

    }

    

    private static void CreateStyleSettingsWindow()
    {
        if (_styleSettingsWindowCount >= 1) return;

        var styleSettings = new StyleSettingsPanel("Style Settings");
        _styleSettingsWindowCount++;
        AddGuiWindow(styleSettings);
    }

    private static void CreateHierachyWindow()
    {
        var hierarch = new SceneHierachyPanel("Hierarchy " + _hierarchyWindowCount);
        _hierarchyWindowCount++;
        AddGuiWindow(hierarch);
    }

    private static void CreateInspectWindow()
    {
        var Inspector = new InspectorPanel("Inspector " + _inspectorWindowCount);
        _inspectorWindowCount++;
        AddGuiWindow(Inspector);
    }

    private static void CreateAssetBrowserWindow()
    {
        var assetBrowserWindow = new AssetBrowserPanel("Asset Browser " + _assetBrowserWindowCount);
        _assetBrowserWindowCount++;
        AddGuiWindow(assetBrowserWindow);
    }

    private static void CreateDefaultWindows()
    {
        _editorViewport = new EditorViewport("editor viewport");
        _gameViewport = new GameViewport("game viewport");

        var hierCo = _hierarchyWindowCount;
        var inspCo = _inspectorWindowCount;
        var assCo = _assetBrowserWindowCount;

        _hierarchyWindowCount = 0;
        _inspectorWindowCount = 0;
        _assetBrowserWindowCount = 0;

        for (var i = 0; i < hierCo; i++) CreateHierachyWindow();
        for (var i = 0; i < inspCo; i++) CreateInspectWindow();
        for (var i = 0; i < assCo; i++) CreateAssetBrowserWindow();
    }


    internal static void ChangeFont(string path, float size)
    {
        Console.WriteLine("Changing font");
        ImGui.GetIO().Fonts.AddFontFromFileTTF(path, size);
    }
}