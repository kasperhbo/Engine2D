using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using Engine2D;
using Engine2D.Core;
using ImGuiNET;
using ImGuizmoNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ErrorCode = OpenTK.Graphics.OpenGL4.ErrorCode;
using Vector2 = System.Numerics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace KDBEngine.UI;

public class ImGuiController
{
    private static bool KHRDebugAvailable;

    private readonly Vector2 _scaleFactor = Vector2.One;

    private readonly List<char> PressedChars = new();

    //private Texture _fontTexture;
    private int _fontTexture;
    private bool _frameBegun;
    private int _indexBuffer;
    private int _indexBufferSize;

    private int _shader;
    private int _shaderFontTextureLocation;
    private int _shaderProjectionMatrixLocation;

    private int _vertexArray;
    private int _vertexBuffer;
    private int _vertexBufferSize;
    private int _windowHeight;

    private int _windowWidth;


    /// <summary>
    ///     Constructs a new ImGuiController.
    /// </summary>
    public ImGuiController(int width, int height)
    {
        _windowWidth = width;
        _windowHeight = height;

        var major = GL.GetInteger(GetPName.MajorVersion);
        var minor = GL.GetInteger(GetPName.MinorVersion);

        KHRDebugAvailable = (major == 4 && minor >= 3) || IsExtensionSupported("KHR_debug");

        var context = ImGui.CreateContext();
        ImGui.SetCurrentContext(context);
        var io = ImGui.GetIO();
        
        ImGuizmo.SetImGuiContext(context);
        
        //io.Fonts.AddFontDefault();
        //io.Fonts.AddFontFromFileTTF("C:/Windows/Fonts/LEELAWUI.TTF", 25f);

        //TODO: MAKE THIS CHANGEABLE IN ENGINE SETTINGS
        io.Fonts.AddFontFromFileTTF(Utils.GetBaseEngineDir() + "\\fonts\\opensans\\OpenSans-Regular.ttf",
            EngineSettings.DefaultFontSize);


        //io.Fonts.AddFontFromFileTTF("C:/Windows/Fonts/FRABK.TTF", 25f);

        //io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;


        //ImGui Set Style
        /*
        {

            style.Colors[(int)ImGuiCol.Text] = new System.Numerics.Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TextDisabled] = new System.Numerics.Vector4(0.50f, 0.50f, 0.50f, 1.00f);
            style.Colors[(int)ImGuiCol.WindowBg] = new System.Numerics.Vector4(0.13f, 0.14f, 0.15f, 1.00f);
            style.Colors[(int)ImGuiCol.ChildBg] = new System.Numerics.Vector4(0.13f, 0.14f, 0.15f, 1.00f);
            style.Colors[(int)ImGuiCol.PopupBg] = new System.Numerics.Vector4(0.13f, 0.14f, 0.15f, 1.00f);
            style.Colors[(int)ImGuiCol.Border] = new System.Numerics.Vector4(0.43f, 0.43f, 0.50f, 0.50f);
            style.Colors[(int)ImGuiCol.BorderShadow] = new System.Numerics.Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            style.Colors[(int)ImGuiCol.FrameBg] = new System.Numerics.Vector4(0.25f, 0.25f, 0.25f, 1.00f);
            style.Colors[(int)ImGuiCol.FrameBgHovered] = new System.Numerics.Vector4(0.38f, 0.38f, 0.38f, 1.00f);
            style.Colors[(int)ImGuiCol.FrameBgActive] = new System.Numerics.Vector4(0.67f, 0.67f, 0.67f, 0.39f);
            style.Colors[(int)ImGuiCol.TitleBg] = new System.Numerics.Vector4(0.08f, 0.08f, 0.09f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = new System.Numerics.Vector4(0.08f, 0.08f, 0.09f, 1.00f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new System.Numerics.Vector4(0.00f, 0.00f, 0.00f, 0.51f);
            style.Colors[(int)ImGuiCol.MenuBarBg] = new System.Numerics.Vector4(0.14f, 0.14f, 0.14f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = new System.Numerics.Vector4(0.02f, 0.02f, 0.02f, 0.53f);
            style.Colors[(int)ImGuiCol.ScrollbarGrab] = new System.Numerics.Vector4(0.31f, 0.31f, 0.31f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new System.Numerics.Vector4(0.41f, 0.41f, 0.41f, 1.00f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new System.Numerics.Vector4(0.51f, 0.51f, 0.51f, 1.00f);
            style.Colors[(int)ImGuiCol.CheckMark] = new System.Numerics.Vector4(0.11f, 0.64f, 0.92f, 1.00f);
            style.Colors[(int)ImGuiCol.SliderGrab] = new System.Numerics.Vector4(0.11f, 0.64f, 0.92f, 1.00f);
            style.Colors[(int)ImGuiCol.SliderGrabActive] = new System.Numerics.Vector4(0.08f, 0.50f, 0.72f, 1.00f);
            style.Colors[(int)ImGuiCol.Button] = new System.Numerics.Vector4(0.25f, 0.25f, 0.25f, 1.00f);
            style.Colors[(int)ImGuiCol.ButtonHovered] = new System.Numerics.Vector4(0.38f, 0.38f, 0.38f, 1.00f);
            style.Colors[(int)ImGuiCol.ButtonActive] = new System.Numerics.Vector4(0.67f, 0.67f, 0.67f, 0.39f);
            style.Colors[(int)ImGuiCol.Header] = new System.Numerics.Vector4(0.22f, 0.22f, 0.22f, 1.00f);
            style.Colors[(int)ImGuiCol.HeaderHovered] = new System.Numerics.Vector4(0.25f, 0.25f, 0.25f, 1.00f);
            style.Colors[(int)ImGuiCol.HeaderActive] = new System.Numerics.Vector4(0.67f, 0.67f, 0.67f, 0.39f);
            style.Colors[(int)ImGuiCol.Separator] = style.Colors[(int)ImGuiCol.Border];
            style.Colors[(int)ImGuiCol.SeparatorHovered] = new System.Numerics.Vector4(0.41f, 0.42f, 0.44f, 1.00f);
            style.Colors[(int)ImGuiCol.SeparatorActive] = new System.Numerics.Vector4(0.26f, 0.59f, 0.98f, 0.95f);
            style.Colors[(int)ImGuiCol.ResizeGrip] = new System.Numerics.Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            style.Colors[(int)ImGuiCol.ResizeGripHovered] = new System.Numerics.Vector4(0.29f, 0.30f, 0.31f, 0.67f);
            style.Colors[(int)ImGuiCol.ResizeGripActive] = new System.Numerics.Vector4(0.26f, 0.59f, 0.98f, 0.95f);
            style.Colors[(int)ImGuiCol.Tab] = new System.Numerics.Vector4(0.08f, 0.08f, 0.09f, 0.83f);
            style.Colors[(int)ImGuiCol.TabHovered] = new System.Numerics.Vector4(0.33f, 0.34f, 0.36f, 0.83f);
            style.Colors[(int)ImGuiCol.TabActive] = new System.Numerics.Vector4(0.23f, 0.23f, 0.24f, 1.00f);
            style.Colors[(int)ImGuiCol.TabUnfocused] = new System.Numerics.Vector4(0.08f, 0.08f, 0.09f, 1.00f);
            style.Colors[(int)ImGuiCol.TabUnfocusedActive] = new System.Numerics.Vector4(0.13f, 0.14f, 0.15f, 1.00f);
            style.Colors[(int)ImGuiCol.DockingPreview] = new System.Numerics.Vector4(0.26f, 0.59f, 0.98f, 0.70f);
            style.Colors[(int)ImGuiCol.DockingEmptyBg] = new System.Numerics.Vector4(0.20f, 0.20f, 0.20f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotLines] = new System.Numerics.Vector4(0.61f, 0.61f, 0.61f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotLinesHovered] = new System.Numerics.Vector4(1.00f, 0.43f, 0.35f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogram] = new System.Numerics.Vector4(0.90f, 0.70f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new System.Numerics.Vector4(1.00f, 0.60f, 0.00f, 1.00f);
            style.Colors[(int)ImGuiCol.TextSelectedBg] = new System.Numerics.Vector4(0.26f, 0.59f, 0.98f, 0.35f);
            style.Colors[(int)ImGuiCol.DragDropTarget] = new System.Numerics.Vector4(0.11f, 0.64f, 0.92f, 1.00f);
            style.Colors[(int)ImGuiCol.NavHighlight] = new System.Numerics.Vector4(0.26f, 0.59f, 0.98f, 1.00f);
            style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new System.Numerics.Vector4(1.00f, 1.00f, 1.00f, 0.70f);
            style.Colors[(int)ImGuiCol.NavWindowingDimBg] = new System.Numerics.Vector4(0.80f, 0.80f, 0.80f, 0.20f);
            style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new System.Numerics.Vector4(0.80f, 0.80f, 0.80f, 0.35f);
            //style.GrabRounding = style.FrameRounding = 2.3f;
        }
          */

        LoadStyle();
        //LoadStyle2();

        CreateDeviceResources();
        SetKeyMappings();

        SetPerFrameImGuiData(1f / 60f);

        ImGui.NewFrame();
        ImGuizmo.BeginFrame();
        _frameBegun = true;
    }

    private void LoadStyle()
    {
        var style = ImGui.GetStyle();

        style.AntiAliasedFill = true;
        style.AntiAliasedLines = true;

        style.Colors[(int)ImGuiCol.Text] = new System.Numerics.Vector4(0.88f, 0.88f, 0.88f, 1.00f);
        style.Colors[(int)ImGuiCol.TextDisabled] = new System.Numerics.Vector4(0.50f, 0.50f, 0.50f, 1.00f);
        style.Colors[(int)ImGuiCol.WindowBg] = new System.Numerics.Vector4(0.196f, 0.196f, 0.196f, 1.00f);
        style.Colors[(int)ImGuiCol.ChildBg] = new System.Numerics.Vector4(0.196f, 0.196f, 0.196f, 1.00f);
        style.Colors[(int)ImGuiCol.PopupBg] = new System.Numerics.Vector4(0.13f, 0.13f, 0.13f, 1.00f);
        style.Colors[(int)ImGuiCol.Border] = new System.Numerics.Vector4(0.13f, 0.13f, 0.13f, 1.00f);
        style.Colors[(int)ImGuiCol.BorderShadow] = new System.Numerics.Vector4(0.36f, 0.36f, 0.36f, 0.21f);
        style.Colors[(int)ImGuiCol.FrameBg] = new System.Numerics.Vector4(0.15f, 0.15f, 0.15f, 1.00f);
        style.Colors[(int)ImGuiCol.FrameBgHovered] = new System.Numerics.Vector4(0.13f, 0.13f, 0.13f, 1.00f);
        style.Colors[(int)ImGuiCol.FrameBgActive] = new System.Numerics.Vector4(0.12f, 0.12f, 0.12f, 1.00f);
        style.Colors[(int)ImGuiCol.TitleBg] = new System.Numerics.Vector4(0.13f, 0.13f, 0.13f, 1.00f);
        style.Colors[(int)ImGuiCol.TitleBgActive] = new System.Numerics.Vector4(0.13f, 0.13f, 0.13f, 1.00f);
        style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new System.Numerics.Vector4(0.00f, 0.00f, 0.00f, 0.51f);
        style.Colors[(int)ImGuiCol.MenuBarBg] = new System.Numerics.Vector4(0.14f, 0.14f, 0.14f, 1.00f);
        style.Colors[(int)ImGuiCol.ScrollbarBg] = new System.Numerics.Vector4(0.16f, 0.16f, 0.16f, 1.00f);
        style.Colors[(int)ImGuiCol.ScrollbarGrab] = new System.Numerics.Vector4(0.28f, 0.28f, 0.28f, 1.00f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new System.Numerics.Vector4(0.28f, 0.28f, 0.28f, 1.00f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new System.Numerics.Vector4(0.24f, 0.24f, 0.24f, 1.00f);
        style.Colors[(int)ImGuiCol.CheckMark] = new System.Numerics.Vector4(0.50f, 0.50f, 0.50f, 1.00f);
        style.Colors[(int)ImGuiCol.SliderGrab] = new System.Numerics.Vector4(0.28f, 0.28f, 0.28f, 1.00f);
        style.Colors[(int)ImGuiCol.SliderGrabActive] = new System.Numerics.Vector4(0.28f, 0.28f, 0.28f, 1.00f);
        style.Colors[(int)ImGuiCol.Button] = new System.Numerics.Vector4(0.16f, 0.16f, 0.16f, 1.00f);
        style.Colors[(int)ImGuiCol.ButtonHovered] = new System.Numerics.Vector4(0.14f, 0.14f, 0.14f, 1.00f);
        style.Colors[(int)ImGuiCol.ButtonActive] = new System.Numerics.Vector4(0.12f, 0.12f, 0.12f, 1.00f);
        style.Colors[(int)ImGuiCol.Header] = new System.Numerics.Vector4(0.18f, 0.18f, 0.18f, 1.00f);
        style.Colors[(int)ImGuiCol.HeaderHovered] = new System.Numerics.Vector4(0.16f, 0.16f, 0.16f, 1.00f);
        style.Colors[(int)ImGuiCol.HeaderActive] = new System.Numerics.Vector4(0.13f, 0.13f, 0.13f, 1.00f);
        style.Colors[(int)ImGuiCol.Separator] = new System.Numerics.Vector4(0.15f, 0.15f, 0.15f, 1.00f);
        style.Colors[(int)ImGuiCol.SeparatorHovered] = new System.Numerics.Vector4(0.37f, 0.37f, 0.37f, 1.00f);
        style.Colors[(int)ImGuiCol.SeparatorActive] = new System.Numerics.Vector4(0.49f, 0.49f, 0.49f, 1.00f);
        style.Colors[(int)ImGuiCol.ResizeGrip] = new System.Numerics.Vector4(0.15f, 0.15f, 0.15f, 1.00f);
        style.Colors[(int)ImGuiCol.ResizeGripHovered] = new System.Numerics.Vector4(0.35f, 0.35f, 0.35f, 1.00f);
        style.Colors[(int)ImGuiCol.ResizeGripActive] = new System.Numerics.Vector4(0.49f, 0.49f, 0.49f, 1.00f);
        style.Colors[(int)ImGuiCol.Tab] = new System.Numerics.Vector4(0.16f, 0.16f, 0.16f, 1.00f);
        style.Colors[(int)ImGuiCol.TabHovered] = new System.Numerics.Vector4(0.18f, 0.18f, 0.18f, 1.00f);
        style.Colors[(int)ImGuiCol.TabActive] = new System.Numerics.Vector4(0.20f, 0.20f, 0.20f, 1.00f);
        style.Colors[(int)ImGuiCol.TabUnfocused] = new System.Numerics.Vector4(0.16f, 0.16f, 0.16f, 1.00f);
        style.Colors[(int)ImGuiCol.TabUnfocusedActive] = new System.Numerics.Vector4(0.20f, 0.20f, 0.20f, 1.00f);
        style.Colors[(int)ImGuiCol.DockingPreview] = new System.Numerics.Vector4(0.41f, 0.41f, 0.41f, 1.00f);
        style.Colors[(int)ImGuiCol.DockingEmptyBg] = new System.Numerics.Vector4(0.20f, 0.20f, 0.20f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotLines] = new System.Numerics.Vector4(0.66f, 0.66f, 0.66f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotLinesHovered] = new System.Numerics.Vector4(0.27f, 0.37f, 0.13f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotHistogram] = new System.Numerics.Vector4(0.34f, 0.47f, 0.17f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new System.Numerics.Vector4(0.41f, 0.56f, 0.20f, 0.99f);
        style.Colors[(int)ImGuiCol.TextSelectedBg] = new System.Numerics.Vector4(0.80f, 0.80f, 0.80f, 0.27f);
        style.Colors[(int)ImGuiCol.DragDropTarget] = new System.Numerics.Vector4(0.59f, 0.59f, 0.59f, 0.98f);
        style.Colors[(int)ImGuiCol.NavHighlight] = new System.Numerics.Vector4(0.83f, 0.83f, 0.83f, 1.00f);
        style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new System.Numerics.Vector4(0.83f, 0.83f, 0.83f, 1.00f);
        style.Colors[(int)ImGuiCol.NavWindowingDimBg] = new System.Numerics.Vector4(0.05f, 0.05f, 0.05f, 0.50f);
        style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new System.Numerics.Vector4(0.05f, 0.05f, 0.05f, 0.50f);
    }

    private void LoadStyle2()
    {
        var style = ImGui.GetStyle();
        style.Colors[(int)ImGuiCol.Text] = new System.Numerics.Vector4(1, 1, 1, 0.85f);
        style.Colors[(int)ImGuiCol.TextDisabled] = new System.Numerics.Vector4(Colors.TextDisabled.R,
            Colors.TextDisabled.G, Colors.TextDisabled.B, Colors.TextDisabled.A);
        style.Colors[(int)ImGuiCol.Button] =
            new System.Numerics.Vector4(Colors.Button.R, Colors.Button.G, Colors.Button.B, Colors.Button.A);
        style.Colors[(int)ImGuiCol.ButtonHovered] = new System.Numerics.Vector4(Colors.ButtonHover.R,
            Colors.ButtonHover.G, Colors.ButtonHover.B, Colors.ButtonHover.A);
        style.Colors[(int)ImGuiCol.Border] = new System.Numerics.Vector4(0, 0.00f, 0.00f, 0.97f);
        style.Colors[(int)ImGuiCol.BorderShadow] = new System.Numerics.Vector4(0.00f, 0.00f, 0.00f, 1.00f);
        style.Colors[(int)ImGuiCol.FrameBg] = new System.Numerics.Vector4(0.13f, 0.13f, 0.13f, 0.80f);
        style.Colors[(int)ImGuiCol.FrameBgHovered] = new System.Numerics.Vector4(0.38f, 0.38f, 0.38f, 0.40f);
        style.Colors[(int)ImGuiCol.FrameBgActive] = new System.Numerics.Vector4(0.00f, 0.55f, 0.8f, 1.00f);
        style.Colors[(int)ImGuiCol.ScrollbarBg] = new System.Numerics.Vector4(0.12f, 0.12f, 0.12f, 0.53f);
        style.Colors[(int)ImGuiCol.ScrollbarGrab] = new System.Numerics.Vector4(0.31f, 0.31f, 0.31f, 0.33f);
        style.Colors[(int)ImGuiCol.ResizeGrip] = new System.Numerics.Vector4(0.00f, 0.00f, 0.00f, 0.25f);
        style.Colors[(int)ImGuiCol.WindowBg] = new System.Numerics.Vector4(0.1f, 0.1f, 0.1f, 0.98f);
        style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new System.Numerics.Vector4(0.1f, 0.1f, 0.1f, 0.1f);
        style.Colors[(int)ImGuiCol.MenuBarBg] = new System.Numerics.Vector4(0.0f, 0.0f, 0.0f, 1.0f);
        style.Colors[(int)ImGuiCol.Separator] = new System.Numerics.Vector4(0.0f, 0.0f, 0.0f, 1.0f);

        //style.Colors[(int)ImGuiCol.SeparatorHovered] =      Color.FromString("#FF00B2FF");
        //style.Colors[(int)ImGuiCol.TabUnfocused] =          Color.FromString("#FF1C1C1C");
        //style.Colors[(int)ImGuiCol.TabActive] =             Color.FromString("#FF505050");
        //style.Colors[(int)ImGuiCol.Tab] =                   Color.FromString("#FF202020");
        //style.Colors[(int)ImGuiCol.TabUnfocused] =          Color.FromString("#FF151515");
        //style.Colors[(int)ImGuiCol.TabUnfocusedActive] =    Color.FromString("#FF202020");
        //style.Colors[(int)ImGuiCol.TitleBgActive] =         Color.FromString("#FF000000");
        //style.Colors[(int)ImGuiCol.TitleBg] =               Color.FromString("#FF000000");

        style.WindowPadding = Vector2.Zero;
        style.FramePadding = new Vector2(7, 4);
        style.ItemSpacing = new Vector2(1, 1);
        style.ItemInnerSpacing = new Vector2(3, 2);
        style.GrabMinSize = 10;
        style.FrameBorderSize = 0;
        style.WindowRounding = 0;
        style.ChildRounding = 0;
        style.ScrollbarRounding = 2;
        style.FrameRounding = 0f;
        style.DisplayWindowPadding = Vector2.Zero;
        style.DisplaySafeAreaPadding = Vector2.Zero;
        style.ChildBorderSize = 1;
        style.TabRounding = 2;
    }

    public void WindowResized(int width, int height)
    {
        _windowWidth = width;
        _windowHeight = height;
    }

    public void DestroyDeviceObjects()
    {
        Dispose();
    }

    public void CreateDeviceResources()
    {
        _vertexBufferSize = 10000;
        _indexBufferSize = 2000;

        var prevVAO = GL.GetInteger(GetPName.VertexArrayBinding);
        var prevArrayBuffer = GL.GetInteger(GetPName.ArrayBufferBinding);

        _vertexArray = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArray);
        LabelObject(ObjectLabelIdentifier.VertexArray, _vertexArray, "ImGui");

        _vertexBuffer = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
        LabelObject(ObjectLabelIdentifier.Buffer, _vertexBuffer, "VBO: ImGui");
        GL.BufferData(BufferTarget.ArrayBuffer, _vertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

        _indexBuffer = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
        LabelObject(ObjectLabelIdentifier.Buffer, _indexBuffer, "EBO: ImGui");
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

        RecreateFontDeviceTexture();

        var VertexSource = @"#version 330 core
            uniform mat4 projection_matrix;
            layout(location = 0) in vec2 in_position;
            layout(location = 1) in vec2 in_texCoord;
            layout(location = 2) in vec4 in_color;
            out vec4 color;
            out vec2 texCoord;
            void main()
            {
                gl_Position = projection_matrix * vec4(in_position, 0, 1);
                color = in_color;
                texCoord = in_texCoord;
            }";

        var FragmentSource = @"#version 330 core
            uniform sampler2D in_fontTexture;
            in vec4 color;
            in vec2 texCoord;
            out vec4 outputColor;
            void main()
            {
                outputColor = color * texture(in_fontTexture, texCoord);
            }";

        _shader = CreateProgram("ImGui", VertexSource, FragmentSource);
        _shaderProjectionMatrixLocation = GL.GetUniformLocation(_shader, "projection_matrix");
        _shaderFontTextureLocation = GL.GetUniformLocation(_shader, "in_fontTexture");

        var stride = Unsafe.SizeOf<ImDrawVert>();
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 8);
        GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, stride, 16);

        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);
        GL.EnableVertexAttribArray(2);

        GL.BindVertexArray(prevVAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, prevArrayBuffer);

        CheckGLError("End of ImGui setup");
    }


    public void SetupDockspace()
    {
        var windowFlags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;

        var mainViewport = ImGui.GetMainViewport();

        ImGui.SetNextWindowPos(mainViewport.WorkPos);
        ImGui.SetNextWindowSize(mainViewport.WorkSize);

        ImGui.SetNextWindowViewport(mainViewport.ID);

        ImGui.SetNextWindowPos(new Vector2(0.0f, 0.0f));

        //TODO: REMOVE THIS HARDCODED SHITTY
        ImGui.SetNextWindowSize(new Vector2(1920, 1080));

        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);

        windowFlags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse |
                       ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove |
                       ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;

        ImGui.Begin("Dockspace Demo", windowFlags);
        ImGui.PopStyleVar(2);

        // Dockspace
        ImGui.DockSpace(ImGui.GetID("Dockspace"));

        ImGui.End();
    }

    /// <summary>
    ///     Recreates the device texture used to render text.
    /// </summary>
    public void RecreateFontDeviceTexture()
    {
        var io = ImGui.GetIO();
        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out var width, out var height, out var bytesPerPixel);

        var mips = (int)Math.Floor(Math.Log(Math.Max(width, height), 2));

        var prevActiveTexture = GL.GetInteger(GetPName.ActiveTexture);
        GL.ActiveTexture(TextureUnit.Texture0);
        var prevTexture2D = GL.GetInteger(GetPName.TextureBinding2D);

        _fontTexture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, _fontTexture);
        GL.TexStorage2D(TextureTarget2d.Texture2D, mips, SizedInternalFormat.Rgba8, width, height);
        LabelObject(ObjectLabelIdentifier.Texture, _fontTexture, "ImGui Text Atlas");

        GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte,
            pixels);

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, mips - 1);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

        // Restore state
        GL.BindTexture(TextureTarget.Texture2D, prevTexture2D);
        GL.ActiveTexture((TextureUnit)prevActiveTexture);

        io.Fonts.SetTexID((IntPtr)_fontTexture);

        io.Fonts.ClearTexData();
    }

    /// <summary>
    ///     Renders the ImGui draw list data.
    /// </summary>
    public void Render()
    {
        if (_frameBegun)
        {
            _frameBegun = false;
            ImGui.Render();
         
            RenderImDrawData(ImGui.GetDrawData());
        }
    }

    /// <summary>
    ///     Updates ImGui input and IO configuration state.
    /// </summary>
    public void Update(GameWindow wnd, double deltaSeconds)
    {
        //SetupDockspace();
        ImGuizmo.SetImGuiContext(ImGui.GetCurrentContext());
        
        if (_frameBegun) ImGui.Render();

        SetPerFrameImGuiData((float)deltaSeconds);
        UpdateImGuiInput(wnd);

        _frameBegun = true;
   

        ImGui.NewFrame();
        ImGuizmo.BeginFrame();
    }

    /// <summary>
    ///     Sets per-frame data based on the associated window.
    ///     This is called by Update(float).
    /// </summary>
    private void SetPerFrameImGuiData(float deltaSeconds)
    {
        var io = ImGui.GetIO();
        io.DisplaySize = new Vector2(
            _windowWidth / _scaleFactor.X,
            _windowHeight / _scaleFactor.Y);
        io.DisplayFramebufferScale = _scaleFactor;
        io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
    }

    private void UpdateImGuiInput(GameWindow wnd)
    {
        var io = ImGui.GetIO();

        var MouseState = wnd.MouseState;
        var KeyboardState = wnd.KeyboardState;

        io.MouseDown[0] = MouseState[MouseButton.Left];
        io.MouseDown[1] = MouseState[MouseButton.Right];
        io.MouseDown[2] = MouseState[MouseButton.Middle];

        var screenPoint = new Vector2i((int)MouseState.X, (int)MouseState.Y);
        var point = screenPoint; //wnd.PointToClient(screenPoint);
        io.MousePos = new Vector2(point.X, point.Y);

        foreach (Keys key in Enum.GetValues(typeof(Keys)))
        {
            if (key == Keys.Unknown) continue;
            io.KeysDown[(int)key] = KeyboardState.IsKeyDown(key);
        }

        foreach (var c in PressedChars) io.AddInputCharacter(c);
        PressedChars.Clear();

        io.KeyCtrl = KeyboardState.IsKeyDown(Keys.LeftControl) || KeyboardState.IsKeyDown(Keys.RightControl);
        io.KeyAlt = KeyboardState.IsKeyDown(Keys.LeftAlt) || KeyboardState.IsKeyDown(Keys.RightAlt);
        io.KeyShift = KeyboardState.IsKeyDown(Keys.LeftShift) || KeyboardState.IsKeyDown(Keys.RightShift);
        io.KeySuper = KeyboardState.IsKeyDown(Keys.LeftSuper) || KeyboardState.IsKeyDown(Keys.RightSuper);
    }

    internal void PressChar(char keyChar)
    {
        PressedChars.Add(keyChar);
    }

    internal void MouseScroll(OpenTK.Mathematics.Vector2 offset)
    {
        var io = ImGui.GetIO();

        io.MouseWheel = offset.Y;
        io.MouseWheelH = offset.X;
    }

    private static void SetKeyMappings()
    {
        var io = ImGui.GetIO();
        io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab;
        io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left;
        io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right;
        io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up;
        io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down;
        io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp;
        io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown;
        io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home;
        io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End;
        io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete;
        io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Backspace;
        io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter;
        io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape;
        io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A;
        io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C;
        io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V;
        io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X;
        io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y;
        io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z;
    }

    private void RenderImDrawData(ImDrawDataPtr draw_data)
    {
        if (draw_data.CmdListsCount == 0) return;

        // Get intial state.
        var prevVAO = GL.GetInteger(GetPName.VertexArrayBinding);
        var prevArrayBuffer = GL.GetInteger(GetPName.ArrayBufferBinding);
        var prevProgram = GL.GetInteger(GetPName.CurrentProgram);
        var prevBlendEnabled = GL.GetBoolean(GetPName.Blend);
        var prevScissorTestEnabled = GL.GetBoolean(GetPName.ScissorTest);
        var prevBlendEquationRgb = GL.GetInteger(GetPName.BlendEquationRgb);
        var prevBlendEquationAlpha = GL.GetInteger(GetPName.BlendEquationAlpha);
        var prevBlendFuncSrcRgb = GL.GetInteger(GetPName.BlendSrcRgb);
        var prevBlendFuncSrcAlpha = GL.GetInteger(GetPName.BlendSrcAlpha);
        var prevBlendFuncDstRgb = GL.GetInteger(GetPName.BlendDstRgb);
        var prevBlendFuncDstAlpha = GL.GetInteger(GetPName.BlendDstAlpha);
        var prevCullFaceEnabled = GL.GetBoolean(GetPName.CullFace);
        var prevDepthTestEnabled = GL.GetBoolean(GetPName.DepthTest);
        var prevActiveTexture = GL.GetInteger(GetPName.ActiveTexture);
        
        GL.ActiveTexture(TextureUnit.Texture0);
        
        var prevTexture2D = GL.GetInteger(GetPName.TextureBinding2D);
        
        Span<int> prevScissorBox = stackalloc int[4];
        
        unsafe
        {
            fixed (int* iptr = &prevScissorBox[0])
            {
                GL.GetInteger(GetPName.ScissorBox, iptr);
            }
        }

        // Bind the element buffer (thru the VAO) so that we can resize it.
        GL.BindVertexArray(_vertexArray);
        // Bind the vertex buffer so that we can resize it.
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
        for (var i = 0; i < draw_data.CmdListsCount; i++)
        {
            var cmd_list = draw_data.CmdListsRange[i];

            var vertexSize = cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();
            if (vertexSize > _vertexBufferSize)
            {
                var newSize = (int)Math.Max(_vertexBufferSize * 1.5f, vertexSize);

                GL.BufferData(BufferTarget.ArrayBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                _vertexBufferSize = newSize;

                Console.WriteLine($"Resized dear imgui vertex buffer to new size {_vertexBufferSize}");
            }

            var indexSize = cmd_list.IdxBuffer.Size * sizeof(ushort);
            if (indexSize > _indexBufferSize)
            {
                var newSize = (int)Math.Max(_indexBufferSize * 1.5f, indexSize);
                GL.BufferData(BufferTarget.ElementArrayBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                _indexBufferSize = newSize;

                Console.WriteLine($"Resized dear imgui index buffer to new size {_indexBufferSize}");
            }
        }

        // Setup orthographic projection matrix into our constant buffer
        var io = ImGui.GetIO();
        var mvp = Matrix4.CreateOrthographicOffCenter(
            0.0f,
            io.DisplaySize.X,
            io.DisplaySize.Y,
            0.0f,
            -1.0f,
            1.0f);

        GL.UseProgram(_shader);
        GL.UniformMatrix4(_shaderProjectionMatrixLocation, false, ref mvp);
        GL.Uniform1(_shaderFontTextureLocation, 0);
        CheckGLError("Projection");

        GL.BindVertexArray(_vertexArray);
        CheckGLError("VAO");

        draw_data.ScaleClipRects(io.DisplayFramebufferScale);

        GL.Enable(EnableCap.Blend);
        GL.Enable(EnableCap.ScissorTest);
        GL.BlendEquation(BlendEquationMode.FuncAdd);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Disable(EnableCap.CullFace);
        GL.Disable(EnableCap.DepthTest);

        // Render command lists
        for (var n = 0; n < draw_data.CmdListsCount; n++)
        {
            var cmd_list = draw_data.CmdListsRange[n];

            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero,
                cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmd_list.VtxBuffer.Data);
            CheckGLError($"Data Vert {n}");

            GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, cmd_list.IdxBuffer.Size * sizeof(ushort),
                cmd_list.IdxBuffer.Data);
            CheckGLError($"Data Idx {n}");

            for (var cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
            {
                var pcmd = cmd_list.CmdBuffer[cmd_i];
                if (pcmd.UserCallback != IntPtr.Zero) throw new NotImplementedException();

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, (int)pcmd.TextureId);
                CheckGLError("Texture");

                // We do _windowHeight - (int)clip.W instead of (int)clip.Y because gl has flipped Y when it comes to these coordinates
                var clip = pcmd.ClipRect;
                GL.Scissor((int)clip.X, _windowHeight - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));
                CheckGLError("Scissor");

                if ((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0)
                    GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)pcmd.ElemCount,
                        DrawElementsType.UnsignedShort, (IntPtr)(pcmd.IdxOffset * sizeof(ushort)),
                        unchecked((int)pcmd.VtxOffset));
                else
                    GL.DrawElements(BeginMode.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort,
                        (int)pcmd.IdxOffset * sizeof(ushort));
                CheckGLError("Draw");
            }
        }

        GL.Disable(EnableCap.Blend);
        GL.Disable(EnableCap.ScissorTest);

        // Reset state
        GL.BindTexture(TextureTarget.Texture2D, prevTexture2D);
        GL.ActiveTexture((TextureUnit)prevActiveTexture);
        GL.UseProgram(prevProgram);
        GL.BindVertexArray(prevVAO);
        GL.Scissor(prevScissorBox[0], prevScissorBox[1], prevScissorBox[2], prevScissorBox[3]);
        GL.BindBuffer(BufferTarget.ArrayBuffer, prevArrayBuffer);
        GL.BlendEquationSeparate((BlendEquationMode)prevBlendEquationRgb, (BlendEquationMode)prevBlendEquationAlpha);
        GL.BlendFuncSeparate(
            (BlendingFactorSrc)prevBlendFuncSrcRgb,
            (BlendingFactorDest)prevBlendFuncDstRgb,
            (BlendingFactorSrc)prevBlendFuncSrcAlpha,
            (BlendingFactorDest)prevBlendFuncDstAlpha);
        if (prevBlendEnabled) GL.Enable(EnableCap.Blend);
        else GL.Disable(EnableCap.Blend);
        if (prevDepthTestEnabled) GL.Enable(EnableCap.DepthTest);
        else GL.Disable(EnableCap.DepthTest);
        if (prevCullFaceEnabled) GL.Enable(EnableCap.CullFace);
        else GL.Disable(EnableCap.CullFace);
        if (prevScissorTestEnabled) GL.Enable(EnableCap.ScissorTest);
        else GL.Disable(EnableCap.ScissorTest);
    }

    /// <summary>
    ///     Frees all graphics resources used by the renderer.
    /// </summary>
    public void Dispose()
    {
        GL.DeleteVertexArray(_vertexArray);
        GL.DeleteBuffer(_vertexBuffer);
        GL.DeleteBuffer(_indexBuffer);

        GL.DeleteTexture(_fontTexture);
        GL.DeleteProgram(_shader);
    }

    public static void LabelObject(ObjectLabelIdentifier objLabelIdent, int glObject, string name)
    {
        if (KHRDebugAvailable)
            GL.ObjectLabel(objLabelIdent, glObject, name.Length, name);
    }

    private static bool IsExtensionSupported(string name)
    {
        var n = GL.GetInteger(GetPName.NumExtensions);
        for (var i = 0; i < n; i++)
        {
            var extension = GL.GetString(StringNameIndexed.Extensions, i);
            if (extension == name) return true;
        }

        return false;
    }

    public static int CreateProgram(string name, string vertexSource, string fragmentSoruce)
    {
        var program = GL.CreateProgram();
        LabelObject(ObjectLabelIdentifier.Program, program, $"Program: {name}");

        var vertex = CompileShader(name, ShaderType.VertexShader, vertexSource);
        var fragment = CompileShader(name, ShaderType.FragmentShader, fragmentSoruce);

        GL.AttachShader(program, vertex);
        GL.AttachShader(program, fragment);

        GL.LinkProgram(program);

        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var success);
        if (success == 0)
        {
            var info = GL.GetProgramInfoLog(program);
            Debug.WriteLine($"GL.LinkProgram had info log [{name}]:\n{info}");
        }

        GL.DetachShader(program, vertex);
        GL.DetachShader(program, fragment);

        GL.DeleteShader(vertex);
        GL.DeleteShader(fragment);

        return program;
    }

    private static int CompileShader(string name, ShaderType type, string source)
    {
        var shader = GL.CreateShader(type);
        LabelObject(ObjectLabelIdentifier.Shader, shader, $"Shader: {name}");

        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out var success);
        if (success == 0)
        {
            var info = GL.GetShaderInfoLog(shader);
            Debug.WriteLine($"GL.CompileShader for shader '{name}' [{type}] had info log:\n{info}");
        }

        return shader;
    }

    public static void CheckGLError(string title)
    {
        ErrorCode error;
        var i = 1;
        while ((error = GL.GetError()) != ErrorCode.NoError) Debug.Print($"{title} ({i++}): {error}");
    }

    public static class Colors
    {
        public static readonly Color ConnectedParameter = new(0.6f, 0.6f, 1f);
        public static readonly Color ValueLabel = new(1, 1, 1, 0.5f);
        public static readonly Color ValueLabelHover = new(1, 1, 1, 1.2f);
        public static readonly Color GraphLine = new(1, 1, 1, 0.3f);
        public static readonly Color GraphLineHover = new(1, 1, 1, 0.7f);
        public static readonly Color GraphAxis = new(0, 0, 0, 0.3f);

        public static readonly Color Button = Color.FromString("#CC282828");
        public static readonly Color ButtonHover = new(43, 65, 80);

        public static readonly Color ButtonActive = Color.FromString("#4592FF");
        public static readonly Color DarkGray = Color.FromString("#131313");

        public static readonly Color WidgetSlider = new(0.15f);
        public static readonly Color TextWidgetTitle = new(0.65f);
        public static readonly Color Text = new(0.9f);
        public static readonly Color TextMuted = new(0.5f);
        public static readonly Color TextDisabled = new(0.2f);
        public static readonly Color Warning = new(203, 19, 113);

        public static readonly Color WindowBackground = new(0.05f, 0.05f, 0.05f);
        public static readonly Color Background = new(0.1f, 0.1f, 0.1f, 0.98f);

        public static readonly Color GraphActiveLine = Color.Orange;
    }
}

public static class Colors
{
    public static readonly Color ConnectedParameter = new(0.6f, 0.6f, 1f);
    public static readonly Color ValueLabel = new(1, 1, 1, 0.5f);
    public static readonly Color ValueLabelHover = new(1, 1, 1, 1.2f);
    public static readonly Color GraphLine = new(1, 1, 1, 0.3f);
    public static readonly Color GraphLineHover = new(1, 1, 1, 0.7f);
    public static readonly Color GraphAxis = new(0, 0, 0, 0.3f);

    public static readonly Color Button = Color.FromString("#CC282828");
    public static readonly Color ButtonHover = new(43, 65, 80);

    public static readonly Color ButtonActive = Color.FromString("#4592FF");
    public static readonly Color DarkGray = Color.FromString("#131313");

    public static readonly Color WidgetSlider = new(0.15f);
    public static readonly Color TextWidgetTitle = new(0.65f);
    public static readonly Color Text = new(0.9f);
    public static readonly Color TextMuted = new(0.5f);
    public static readonly Color TextDisabled = new(0.2f);
    public static readonly Color Warning = new(203, 19, 113);

    public static readonly Color WindowBackground = new(0.05f, 0.05f, 0.05f);
    public static readonly Color Background = new(0.1f, 0.1f, 0.1f, 0.98f);

    public static readonly Color GraphActiveLine = Color.Orange;
}

public struct Color
{
    public Vector4 Rgba;

    public static readonly Color Transparent = new(1f, 1f, 1f, 0f);
    public static readonly Color TransparentBlack = new(0f, 0f, 0f, 0f);
    public static readonly Color White = new(1f, 1f, 1f);
    public static readonly Color Gray = new(0.6f, 0.6f, 0.6f);
    public static readonly Color DarkGray = new(0.3f, 0.3f, 0.3f);
    public static readonly Color Black = new(0, 0, 0, 1f);
    public static readonly Color Red = new(1f, 0.2f, 0.2f);
    public static readonly Color Green = new(0.2f, 0.9f, 0.2f);
    public static readonly Color Blue = new(0.4f, 0.5f, 1f);
    public static readonly Color Orange = new(1f, 0.46f, 0f);

    /// <summary>
    ///     Creates white transparent color
    /// </summary>
    public Color(float alpha)
    {
        Rgba = new Vector4(1, 1, 1, alpha);
    }

    public Color(float r, float g, float b, float a = 1.0f)
    {
        Rgba.X = r;
        Rgba.Y = g;
        Rgba.Z = b;
        Rgba.W = a;
    }

    public Color(int r, int g, int b, int a = 255)
    {
        var sc = 1.0f / 255.0f;
        Rgba.X = r * sc;
        Rgba.Y = g * sc;
        Rgba.Z = b * sc;
        Rgba.W = a * sc;
    }


    public Color(Vector4 color)
    {
        Rgba = color;
    }

    public override string ToString()
    {
        return $"[{Rgba.X:0.00}, {Rgba.Y:0.00}, {Rgba.Z:0.00}, {Rgba.W:0.00}]";
    }

    public static Color FromHSV(float h, float s, float v, float a = 1.0f)
    {
        ImGui.ColorConvertHSVtoRGB(h, s, v, out var r, out var g, out var b);
        return new Color(r, g, b, a);
    }

    public static Color FromString(string hex)
    {
        var systemColor = ColorTranslator.FromHtml(hex);
        return new Color(systemColor.R, systemColor.G, systemColor.B, systemColor.A);
    }

    public string ToHTML()
    {
        var drawingColor = System.Drawing.Color.FromArgb((int)(A * 255).Clamp(0, 255),
            (int)(R * 255).Clamp(0, 255),
            (int)(G * 255).Clamp(0, 255),
            (int)(B * 255).Clamp(0, 255));
        return ColorTranslator.ToHtml(drawingColor);
    }


    public static implicit operator Vector4(Color color)
    {
        return color.Rgba;
    }

    public static Color operator *(Color c, float f)
    {
        c.Rgba.W *= f;
        return c;
    }

    public static Color Mix(Color c1, Color c2, float t)
    {
        return new Color(
            c1.Rgba.X + (c2.Rgba.X - c1.Rgba.X) * t,
            c1.Rgba.Y + (c2.Rgba.Y - c1.Rgba.Y) * t,
            c1.Rgba.Z + (c2.Rgba.Z - c1.Rgba.Z) * t,
            c1.Rgba.W + (c2.Rgba.W - c1.Rgba.W) * t
        );
    }

    public static Color GetStyleColor(ImGuiCol color)
    {
        unsafe
        {
            var c = ImGui.GetStyleColorVec4(color);
            return new Color(c->X, c->Y, c->Z, c->W);
        }
    }

    /// <summary>
    ///     This is a variation of the normal HSV function in that it returns a desaturated "white" colors brightness above 0.5
    /// </summary>
    public static Color ColorFromHsl(float h, float s, float l, float a = 1)
    {
        float r, g, b, m, c, x;

        h /= 60;
        if (h < 0) h = 6 - -h % 6;
        h %= 6;

        s = Math.Max(0, Math.Min(1, s));
        l = Math.Max(0, Math.Min(1, l));

        c = (1 - Math.Abs(2 * l - 1)) * s;
        x = c * (1 - Math.Abs(h % 2 - 1));

        if (h < 1)
        {
            r = c;
            g = x;
            b = 0;
        }
        else if (h < 2)
        {
            r = x;
            g = c;
            b = 0;
        }
        else if (h < 3)
        {
            r = 0;
            g = c;
            b = x;
        }
        else if (h < 4)
        {
            r = 0;
            g = x;
            b = c;
        }
        else if (h < 5)
        {
            r = x;
            g = 0;
            b = c;
        }
        else
        {
            r = c;
            g = 0;
            b = x;
        }

        m = l - c / 2;

        return new Color(r + m, g + m, b + m, a);
    }

    public Vector3 AsHsl
    {
        get
        {
            var r = Rgba.X;
            var g = Rgba.Y;
            var b = Rgba.Z;

            var tmp = r < g ? r : g;
            var min = tmp < b ? tmp : b;

            tmp = r > g ? r : g;
            var max = tmp > b ? tmp : b;

            var delta = max - min;
            var lum = (min + max) / 2.0f;
            float sat = 0;
            if (lum > 0.0f && lum < 1.0f) sat = delta / (lum < 0.5f ? 2.0f * lum : 2.0f - 2.0f * lum);

            var hue = 0.0f;
            if (delta > 0.0f)
            {
                if (max == r && max != g)
                    hue += (g - b) / delta;
                if (max == g && max != b)
                    hue += 2.0f + (b - r) / delta;
                if (max == b && max != r)
                    hue += 4.0f + (r - g) / delta;
                hue *= 60.0f;
            }

            return new Vector3(hue, sat, lum);
        }
    }

    public readonly Color Fade(float f)
    {
        return new Color(Rgba.X, Rgba.Y, Rgba.Z, Rgba.W * f);
    }

    public float R
    {
        get => Rgba.X;
        set => Rgba.X = value;
    }

    public float G
    {
        get => Rgba.Y;
        set => Rgba.Y = value;
    }

    public float B
    {
        get => Rgba.Z;
        set => Rgba.Z = value;
    }

    public float A
    {
        get => Rgba.W;
        set => Rgba.W = value;
    }

    /**
         * Normalized from [0..1]
         */
    public float Hue
    {
        get
        {
            ImGui.ColorConvertRGBtoHSV(R, G, B, out var h, out var s, out var v);
            return h;
        }
        set
        {
            ImGui.ColorConvertRGBtoHSV(R, G, B, out var h, out var s, out var v);
            h = value;
            ImGui.ColorConvertHSVtoRGB(h, s, v, out var r, out var g, out var b);
            R = r;
            G = g;
            B = b;
        }
    }


    public float Saturation
    {
        get
        {
            ImGui.ColorConvertRGBtoHSV(R, G, B, out var h, out var s, out var v);
            return s;
        }
        set
        {
            ImGui.ColorConvertRGBtoHSV(R, G, B, out var h, out var s, out var v);
            s = value;
            ImGui.ColorConvertHSVtoRGB(h, s, v, out var r, out var g, out var b);
            R = r;
            G = g;
            B = b;
        }
    }


    public float V
    {
        get
        {
            ImGui.ColorConvertRGBtoHSV(R, G, B, out var h, out var s, out var v);
            return v;
        }
        set
        {
            ImGui.ColorConvertRGBtoHSV(R, G, B, out var h, out var s, out var v);
            v = value;
            ImGui.ColorConvertHSVtoRGB(h, s, v, out var r, out var g, out var b);
            R = r;
            G = g;
            B = b;
        }
    }
}