#region

using System.Numerics;
using Dear_ImGui_Sample;
using Engine2D.Core;
using Engine2D.Logging;
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
        GenerateSampleKeyframes();
    }
    
    private static void GenerateSampleKeyframes()
    {
        // Generate 10 sample keyframes
        for (int i = 0; i < 10; i++)
        {
            float time = i * (endTime - startTime) / 9f;  // Equally spaced along the timeline
            float value = MathF.Sin(time); // Sample value based on time (sine wave)

            keyframes.Add(new Keyframe(time, value));
        }
    }
    
    private static void HandleKeyframeDragAndDrop(float timelineWidth)
    {
        // Handle dragging keyframes
        // if (isDraggingKeyframe)
        // {
        //     // Update the position of the dragging keyframe based on the mouse cursor
        //     float draggingKeyframeTime = (ImGui.GetMousePos().X - ImGui.GetCursorScreenPos().X) / timelineWidth * (endTime - startTime) + startTime;
        //     keyframes[draggingKeyframeIndex].Time = draggingKeyframeTime;
        //
        //     // Check for mouse release to stop dragging
        //     if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
        //     {
        //         isDraggingKeyframe = false;
        //     }
        // }
        //
        // // Handle dropping keyframes
        // if (ImGui.BeginDragDropTarget())
        // {
        //     // Check if there is a payload being dropped
        //     if (ImGui.AcceptDragDropPayload(
        //             "KEYFRAME_PAYLOAD", ImGuiDragDropFlags.AcceptBeforeDelivery))
        //     {
        //         // Get the dropped payload data
        //         Keyframe droppedKeyframe = (Keyframe)ImGui.GetDragDropPayloadData();
        //
        //         // Add the dropped keyframe to the timeline
        //         keyframes.Add(new Keyframe(droppedKeyframe.Time, droppedKeyframe.Value));
        //     }
        //
        //     ImGui.EndDragDropTarget();
        // }
    }



    private static void Update(FrameEventArgs args)
    {
    }
    
    private static List<Keyframe> keyframes = new List<Keyframe>();
    private static float currentTime = 0f;
    private static float startTime = 0f;
    private static float endTime = 10f;
    private static bool isDraggingKeyframe = false;
    private static int draggingKeyframeIndex = -1;
    
    public struct Keyframe
    {
        public float Time;
        public float Value;

        public Keyframe(float time, float value)
        {
            Time = time;
            Value = value;
        }
    }
    
    private static void Render(FrameEventArgs args)
    {
        KDBImGuiController.Update(_engine, args.Time);

        ImGui.Begin("Debug Helper");

        if (ImGui.Button("Reload Assembly")) AssemblyUtils.Reload();

        ImGui.End();
        
        DrawMainMenuBar();
        DrawToolbar();
        SetupDockSpace();

        var currentSelectedAssetBrowserAssets = Engine.Get().CurrentSelectedAssetBrowserAsset;
        
        foreach (var asset in currentSelectedAssetBrowserAssets)
        {
            asset.OnGui();
        }

        ImGui.ShowDemoWindow();

        KDBImGuiController.Render();
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

        _editorViewport?.Begin("Launcher", _engine.CurrentScene?.EditorCamera,
            _engine.CurrentScene?.Renderer?.EditorGameBuffer);

        _gameViewport?.Begin("Game", _engine.CurrentScene?.CurrentMainGameCamera,
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

        ImGui.SetNextWindowPos(new Vector2(0, 55));
        ImGui.SetNextWindowSize(
            new Vector2(_engine.Size.X, _engine.Size.Y - 55));

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

    private static void DrawMainMenuBar()
    {
        ImGui.SetNextWindowSize(new Vector2(_engine.Size.X, 55));
        ImGui.SetNextWindowPos(new Vector2(0, 1));
        ImGui.Begin("titlebar",
            ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDecoration |
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoNav);
        ImGui.BeginMenuBar();


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
        _editorViewport = new EditorViewport();
        _gameViewport = new GameViewport();

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

    private static void DrawToolbar()
    {
        ImGui.Button("Button");
        ImGui.SameLine();
        ImGui.Button("Button2");
        ImGui.End();
    }

    internal static void ChangeFont(string path, float size)
    {
        Console.WriteLine("Changing font");
        ImGui.GetIO().Fonts.AddFontFromFileTTF(path, size);
    }
}