using System.Runtime.CompilerServices;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.UI;
using Engine2D.UI.Browsers;
using Engine2D.UI.ImGuiExtension;
using Engine2D.UI.Viewports;
using ImGuiNET;
using OpenTK.Windowing.Common;

namespace Dear_ImGui_Sample;

public static class UIRenderer
{
    private static List<UiElemenet> _windows = new List<UiElemenet>();
    private static ImGuiController _controller;
    private static Engine _engine;
    
    private static EditorViewport? _editorViewport = null;

    public static EditorViewport? CurrentEditorViewport
    {
        get
        {
            if (_editorViewport == null)
            {
                Log.Error("There is currently no editor vp, please open one firts");
                return null;
            }
                
            return _editorViewport;
        } 
    }
        
    private static GameViewport _gameViewport;
 
    public static void Init(Engine engine, bool createDefaultWindows)
    {
        _engine = engine;

        _engine.UpdateFrame += Update;
        _engine.RenderFrame += Render;
        _engine.MouseWheel += OnMouseWheel;
        _engine.TextInput += OnTextInput;
        _engine.Resize += OnResize;
        
        _windows = new List<UiElemenet>();
        _controller = new ImGuiController(Engine.Get().Size.X, Engine.Get().Size.Y);

        if (createDefaultWindows) CreateDefaultWindows();
        LoadFont(Utils.GetBaseEngineDir() + "\\UI\\Fonts\\Roboto\\Roboto-Black.ttf", 22);
    }

    private static void LoadFont(string fontPath, int fontSize)
    {
        ImFontPtr font = ImGui.GetIO().Fonts.AddFontFromFileTTF(fontPath, fontSize);
 
        ImGui.GetIO().FontGlobalScale = 1f; // Adjust the font scale if needed
        ImGui.PushFont(font);
    }
    
    private static void Update(FrameEventArgs args)
    {
        //_controller.Update(Engine.Get(), args.Time);
    }
    
    private static void Render(FrameEventArgs args)
    {
        _controller.Update(_engine, args.Time);
        
        DrawMainMenuBar();
        DrawToolbar();
        SetupDockspace();
        
        ImGui.ShowDemoWindow();
        
        _controller.Render();
    }

    private static void OnMouseWheel(MouseWheelEventArgs args)
    {
        _controller.MouseWheel(args);
    }

    private static void OnTextInput(TextInputEventArgs args)
    {
        _controller.PressChar(args);
    }

    private static void OnResize(ResizeEventArgs args)
    {
        _controller.WindowResized(args);
    }
    static int currentItem = 0;
    private static void RenderUIWindows()
    {
        ImGui.Begin("Styles");

        if (ImGui.Combo("label", ref currentItem, "Default (dark)\0" +
                                                  "Default always RED buttons (dark)\0" +
                                                  "Default gray buttons (dark)\0" +
                                                  "Colors on black (dark)\0" +
                                                  "Solarized (dark)\0" +
                                                  "SetupStyle\0" +
                                                  "Cyan/White on Grey (dark)\0" +
                                                  "Cyan/Yellow on Gray/Black (dark)\0" +
                                                  "Red on Gray/Black (dark)\0" +
                                                  "Cyan/Yellow on White (light)\0" +
                                                  "Grey scale on White (light)\0" +
                                                  "ImGui Classic\0" +
                                                  "ImGui Dark\0" +
                                                  "ImGui Light\0"+
                                                  "chatgpt\0"+
                                                  "chatgptlight\0"
            ))
        {
            ImGuiStyleManager.selectTheme(currentItem);
            Console.WriteLine(currentItem);
            ImGui.EndCombo();
        }
        
        ImGui.End();
        
        foreach (var window in _windows)
        {
            window.Render();
        }
            
        _engine.CurrentScene?.OnGui();
            
        _editorViewport.Begin("Editor", _engine.CurrentScene?.EditorCamera,
            _engine.CurrentScene?.Renderer?.EditorGameBuffer);
            
        _gameViewport.Begin("Game", _engine.CurrentScene?.CurrentMainGameCamera,
            _engine.CurrentScene?.Renderer.GameBuffer);
    }
    
    private static void AddGuiWindow(UiElemenet window)
    {
        _windows.Add(window);
        _engine.FileDrop += window.OnFileDrop;
    }

    private static void RemoveGuiWindow(UiElemenet window)
    {
        _engine.FileDrop -= window.OnFileDrop;
        _windows.Remove(window);
    }
    
    private static void CreateDefaultWindows()
    {
        _editorViewport = new EditorViewport();
        _gameViewport = new GameViewport();
        
        var assetBrowser2 = new AssetBrowser();
        AddGuiWindow(assetBrowser2);
            
        var inspector = new Inspector();
        AddGuiWindow(inspector);
            
        var hierarch = new SceneHierachy();
        AddGuiWindow(hierarch);

        var uiSettingsPanel = new UISettingsPanel();
        AddGuiWindow(uiSettingsPanel);
    }
    
    private static float _titleBarHeight = 45;
    
    private static void SetupDockspace()
    {
        ImGuiWindowFlags host_window_flags = 0;
        host_window_flags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDocking;
        host_window_flags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
        
        ImGui.SetNextWindowPos(new System.Numerics.Vector2(0,55));
        ImGui.SetNextWindowSize(
            new System.Numerics.Vector2(_engine.Size.X, _engine.Size.Y - 55));
        
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

    private static void DrawMainMenuBar()
    {
        ImGui.SetNextWindowSize(new System.Numerics.Vector2(_engine.Size.X, 55));
        ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, 1));
        ImGui.Begin("titlebar", ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoNav );
        ImGui.BeginMenuBar(); 
        ImGui.MenuItem("File");
        ImGui.MenuItem("Windows");
        ImGui.MenuItem("Help");
        ImGui.EndMenuBar();
    }
    
    private static void DrawToolbar()
    {
        ImGui.Button("Button");
        ImGui.SameLine();
        ImGui.Button("Button2");
        ImGui.End();
    }
}