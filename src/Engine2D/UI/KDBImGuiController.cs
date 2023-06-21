#region

using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using Engine2D.Core;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;
using ImGuizmoNET;
using ImPlotNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ErrorCode = OpenTK.Graphics.OpenGL4.ErrorCode;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Vector2 = System.Numerics.Vector2;

#endregion

// using OpenTK.Mathematics;

namespace Dear_ImGui_Sample;

/// <summary>
///     A modified version of Veldrid.ImGui's ImGuiRenderer.
///     Manages input for ImGui and handles rendering ImGui's DrawLists with Veldrid.
/// </summary>
internal static class KDBImGuiController
{
    private static bool _frameBegun;

    // Veldrid objects
    private static int _vertexArray;
    private static int _vertexBuffer;
    private static int _vertexBufferSize;
    private static int _indexBuffer;
    private static int _indexBufferSize;

    private static Texture _fontTexture;
    private static Shader _shader;

    private static int _windowWidth;
    private static int _windowHeight;

    private static readonly Vector2 _scaleFactor = Vector2.One;

    private static readonly List<char> PressedChars = new();

    /// <summary>
    ///     Constructs a new ImGuiController.
    /// </summary>
    internal static void Init(int width, int height)
    {
        _windowWidth = width;
        _windowHeight = height;

        var context = ImGui.CreateContext();

        ImGui.SetCurrentContext(context);
        ImGuizmo.SetImGuiContext(context);

        ImPlot.SetImGuiContext(context);
        ImPlot.CreateContext();

        var io = ImGui.GetIO();
        //io.Fonts.AddFontDefault();

        //Load default font
        ImGui.GetIO().Fonts.AddFontFromFileTTF(Utils.GetBaseEngineDir()
                                               + "\\UI\\Fonts\\Roboto\\Roboto-Light.ttf", 16);

        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

        CreateDeviceResources();
        SetKeyMappings();

        SetPerFrameImGuiData(1f / 60f);

        ImGuiStyleManager.SetGuiStyle();

        ImGui.NewFrame();
        ImGuizmo.BeginFrame();

        _frameBegun = true;
    }

    internal static void WindowResized(ResizeEventArgs obj)
    {
        _windowWidth = obj.Width;
        _windowHeight = obj.Height;
    }

    internal static void DestroyDeviceObjects()
    {
        Destroy();
    }

    internal static void CreateDeviceResources()
    {
        Util.CreateVertexArray("ImGui", out _vertexArray);

        _vertexBufferSize = 10000;
        _indexBufferSize = 2000;

        Util.CreateVertexBuffer("ImGui", out _vertexBuffer);
        Util.CreateElementBuffer("ImGui", out _indexBuffer);
        GL.NamedBufferData(_vertexBuffer, _vertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
        GL.NamedBufferData(_indexBuffer, _indexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

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
        _shader = new Shader("ImGui", VertexSource, FragmentSource);

        GL.VertexArrayVertexBuffer(_vertexArray, 0, _vertexBuffer, IntPtr.Zero, Unsafe.SizeOf<ImDrawVert>());
        GL.VertexArrayElementBuffer(_vertexArray, _indexBuffer);

        GL.EnableVertexArrayAttrib(_vertexArray, 0);
        GL.VertexArrayAttribBinding(_vertexArray, 0, 0);
        GL.VertexArrayAttribFormat(_vertexArray, 0, 2, VertexAttribType.Float, false, 0);

        GL.EnableVertexArrayAttrib(_vertexArray, 1);
        GL.VertexArrayAttribBinding(_vertexArray, 1, 0);
        GL.VertexArrayAttribFormat(_vertexArray, 1, 2, VertexAttribType.Float, false, 8);

        GL.EnableVertexArrayAttrib(_vertexArray, 2);
        GL.VertexArrayAttribBinding(_vertexArray, 2, 0);
        GL.VertexArrayAttribFormat(_vertexArray, 2, 4, VertexAttribType.UnsignedByte, true, 16);

        Util.CheckGLError("End of ImGui setup");
    }

    /// <summary>
    ///     Recreates the device texture used to render text.
    /// </summary>
    internal static void RecreateFontDeviceTexture()
    {
        Console.WriteLine("Recreating Font Device Textures");

        var io = ImGui.GetIO();

        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out var width, out var height, out var bytesPerPixel);

        _fontTexture = new Texture("ImGui Text Atlas", width, height, pixels);
        _fontTexture.SetMagFilter(TextureMagFilter.Linear);
        _fontTexture.SetMinFilter(TextureMinFilter.Linear);

        io.Fonts.SetTexID(_fontTexture.GLTexture);

        io.Fonts.ClearTexData();

        // var io = ImGui.GetIO();
        //
        // ImFontAtlasPtr fontAtlas = io.Fonts;
        //
        // ImFontConfig fontConfig = new ImFontConfig(); // Natively allocated object, should be explicitly destroyed
        //
        // // Glyphs could be added per-font as well as per config used globally like here
        // fontConfig.GlyphRanges = (ushort*)fontAtlas.GetGlyphRangesDefault();
        //
        // // Fonts merge example
        // fontConfig.PixelSnapH = (1);
        // fontAtlas.AddFontDefault();
        // fontAtlas.Build();
        // io.Fonts.SetTexID(fontAtlas.TexID);
        // io.Fonts.ClearTexData();
        // fontAtlas.Destroy();
        // fontAtlas.AddFontFromFileTTF(Engine2D.Core.Utils.GetBaseEngineDir()
        // +"\\Fonts\\Roboto\\Roboto-Italic.ttf", 32);
        //
    }

    /// <summary>
    ///     Renders the ImGui draw list data.
    ///     This method requires a <see cref="Veldrid.GraphicsDevice" /> because it may create new DeviceBuffers if the size of
    ///     vertex
    ///     or index data has increased beyond the capacity of the existing buffers.
    ///     A <see cref="Veldrid.CommandList" /> is needed to submit drawing and resource update commands.
    /// </summary>
    internal static void Render()
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
    internal static void Update(GameWindow wnd, double deltaSeconds)
    {
        Update(wnd, (float)deltaSeconds);
    }

    internal static void Update(GameWindow wnd, float deltaSeconds)
    {
        if (_frameBegun) ImGui.Render();

        SetPerFrameImGuiData(deltaSeconds);
        UpdateImGuiInput(wnd);

        _frameBegun = true;
        ImGui.NewFrame();
        ImGuizmo.BeginFrame();
    }

    /// <summary>
    ///     Sets per-frame data based on the associated window.
    ///     This is called by Update(float).
    /// </summary>
    private static void SetPerFrameImGuiData(float deltaSeconds)
    {
        var io = ImGui.GetIO();
        io.DisplaySize = new Vector2(
            _windowWidth / _scaleFactor.X,
            _windowHeight / _scaleFactor.Y);
        io.DisplayFramebufferScale = _scaleFactor;
        io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
    }

    private static void UpdateImGuiInput(GameWindow wnd)
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

    internal static void PressChar(TextInputEventArgs obj)
    {
        PressedChars.Add((char)obj.Unicode);
    }

    internal static void MouseWheel(MouseWheelEventArgs obj)
    {
        var io = ImGui.GetIO();

        io.MouseWheel = obj.Offset.Y;
        io.MouseWheelH = obj.Offset.X;
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

    private static void RenderImDrawData(ImDrawDataPtr draw_data)
    {
        if (draw_data.CmdListsCount == 0) return;

        for (var i = 0; i < draw_data.CmdListsCount; i++)
        {
            var cmd_list = draw_data.CmdListsRange[i];

            var vertexSize = cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();
            if (vertexSize > _vertexBufferSize)
            {
                var newSize = (int)Math.Max(_vertexBufferSize * 1.5f, vertexSize);
                GL.NamedBufferData(_vertexBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                _vertexBufferSize = newSize;

                Console.WriteLine($"Resized dear imgui vertex buffer to new size {_vertexBufferSize}");
            }

            var indexSize = cmd_list.IdxBuffer.Size * sizeof(ushort);
            if (indexSize > _indexBufferSize)
            {
                var newSize = (int)Math.Max(_indexBufferSize * 1.5f, indexSize);
                GL.NamedBufferData(_indexBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
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

        _shader.UseShader();
        GL.UniformMatrix4(_shader.GetUniformLocation("projection_matrix"), false, ref mvp);
        GL.Uniform1(_shader.GetUniformLocation("in_fontTexture"), 0);
        Util.CheckGLError("Projection");

        GL.BindVertexArray(_vertexArray);
        Util.CheckGLError("VAO");

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

            GL.NamedBufferSubData(_vertexBuffer, IntPtr.Zero, cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(),
                cmd_list.VtxBuffer.Data);
            Util.CheckGLError($"Data Vert {n}");

            GL.NamedBufferSubData(_indexBuffer, IntPtr.Zero, cmd_list.IdxBuffer.Size * sizeof(ushort),
                cmd_list.IdxBuffer.Data);
            Util.CheckGLError($"Data Idx {n}");

            var vtx_offset = 0;
            var idx_offset = 0;

            for (var cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
            {
                var pcmd = cmd_list.CmdBuffer[cmd_i];
                if (pcmd.UserCallback != IntPtr.Zero) throw new NotImplementedException();

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, (int)pcmd.TextureId);
                Util.CheckGLError("Texture");

                // We do _windowHeight - (int)clip.W instead of (int)clip.Y because gl has flipped Y when it comes to these coordinates
                var clip = pcmd.ClipRect;
                GL.Scissor((int)clip.X, _windowHeight - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));
                Util.CheckGLError("Scissor");

                if ((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0)
                    GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)pcmd.ElemCount,
                        DrawElementsType.UnsignedShort, idx_offset * sizeof(ushort), vtx_offset);
                else
                    GL.DrawElements(BeginMode.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort,
                        (int)pcmd.IdxOffset * sizeof(ushort));
                Util.CheckGLError("Draw");

                idx_offset += (int)pcmd.ElemCount;
            }

            vtx_offset += cmd_list.VtxBuffer.Size;
        }

        GL.Disable(EnableCap.Blend);
        GL.Disable(EnableCap.ScissorTest);
    }

    /// <summary>
    ///     Frees all graphics resources used by the renderer.
    /// </summary>
    internal static void Destroy()
    {
        _fontTexture.Dispose();
        _shader.Dispose();
    }
}

internal static class Util
{
    [Pure]
    internal static float Clamp(float value, float min, float max)
    {
        return value < min ? min : value > max ? max : value;
    }

    [Conditional("DEBUG")]
    internal static void CheckGLError(string title)
    {
        var error = GL.GetError();
        if (error != ErrorCode.NoError) Debug.Print($"{title}: {error}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void LabelObject(ObjectLabelIdentifier objLabelIdent, int glObject, string name)
    {
        GL.ObjectLabel(objLabelIdent, glObject, name.Length, name);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CreateTexture(TextureTarget target, string Name, out int Texture)
    {
        GL.CreateTextures(target, 1, out Texture);
        LabelObject(ObjectLabelIdentifier.Texture, Texture, $"Texture: {Name}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CreateProgram(string Name, out int Program)
    {
        Program = GL.CreateProgram();
        LabelObject(ObjectLabelIdentifier.Program, Program, $"Program: {Name}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CreateShader(ShaderType type, string Name, out int Shader)
    {
        Shader = GL.CreateShader(type);
        LabelObject(ObjectLabelIdentifier.Shader, Shader, $"Shader: {type}: {Name}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CreateBuffer(string Name, out int Buffer)
    {
        GL.CreateBuffers(1, out Buffer);
        LabelObject(ObjectLabelIdentifier.Buffer, Buffer, $"Buffer: {Name}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CreateVertexBuffer(string Name, out int Buffer)
    {
        CreateBuffer($"VBO: {Name}", out Buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CreateElementBuffer(string Name, out int Buffer)
    {
        CreateBuffer($"EBO: {Name}", out Buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void CreateVertexArray(string Name, out int VAO)
    {
        GL.CreateVertexArrays(1, out VAO);
        LabelObject(ObjectLabelIdentifier.VertexArray, VAO, $"VAO: {Name}");
    }
}

internal enum TextureCoordinate
{
    S = TextureParameterName.TextureWrapS,
    T = TextureParameterName.TextureWrapT,
    R = TextureParameterName.TextureWrapR
}

internal class Texture : IDisposable
{
    internal const SizedInternalFormat Srgb8Alpha8 = (SizedInternalFormat)All.Srgb8Alpha8;
    internal const SizedInternalFormat RGB32F = (SizedInternalFormat)All.Rgb32f;

    internal const GetPName MAX_TEXTURE_MAX_ANISOTROPY = (GetPName)0x84FF;

    internal static readonly float MaxAniso;
    internal readonly int GLTexture;
    internal readonly SizedInternalFormat InternalFormat;
    internal readonly int MipmapLevels;

    internal readonly string Name;
    internal readonly int Width, Height;

    static Texture()
    {
        MaxAniso = GL.GetFloat(MAX_TEXTURE_MAX_ANISOTROPY);
    }

    internal Texture(string name, Bitmap image, bool generateMipmaps, bool srgb)
    {
        Name = name;

        Width = image.Width;
        Height = image.Height;
        InternalFormat = srgb ? Srgb8Alpha8 : SizedInternalFormat.Rgba8;

        if (generateMipmaps)
            // Calculate how many levels to generate for this texture
            MipmapLevels = (int)Math.Floor(Math.Log(Math.Max(Width, Height), 2));
        else
            // There is only one level
            MipmapLevels = 1;

        Util.CheckGLError("Clear");

        Util.CreateTexture(TextureTarget.Texture2D, Name, out GLTexture);
        GL.TextureStorage2D(GLTexture, MipmapLevels, InternalFormat, Width, Height);
        Util.CheckGLError("Storage2d");

        var data = image.LockBits(new Rectangle(0, 0, Width, Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        GL.TextureSubImage2D(GLTexture, 0, 0, 0, Width, Height, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
            PixelType.UnsignedByte, data.Scan0);
        Util.CheckGLError("SubImage");

        image.UnlockBits(data);

        if (generateMipmaps) GL.GenerateTextureMipmap(GLTexture);

        GL.TextureParameter(GLTexture, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        Util.CheckGLError("WrapS");
        GL.TextureParameter(GLTexture, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        Util.CheckGLError("WrapT");

        GL.TextureParameter(GLTexture, TextureParameterName.TextureMinFilter,
            (int)(generateMipmaps ? TextureMinFilter.Linear : TextureMinFilter.LinearMipmapLinear));
        GL.TextureParameter(GLTexture, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        Util.CheckGLError("Filtering");

        GL.TextureParameter(GLTexture, TextureParameterName.TextureMaxLevel, MipmapLevels - 1);

        // This is a bit weird to do here
        image.Dispose();
    }

    internal Texture(string name, int GLTex, int width, int height, int mipmaplevels, SizedInternalFormat internalFormat)
    {
        Name = name;
        GLTexture = GLTex;
        Width = width;
        Height = height;
        MipmapLevels = mipmaplevels;
        InternalFormat = internalFormat;
    }

    internal Texture(string name, int width, int height, IntPtr data, bool generateMipmaps = false, bool srgb = false)
    {
        Name = name;
        Width = width;
        Height = height;
        InternalFormat = srgb ? Srgb8Alpha8 : SizedInternalFormat.Rgba8;
        MipmapLevels = generateMipmaps == false ? 1 : (int)Math.Floor(Math.Log(Math.Max(Width, Height), 2));

        Util.CreateTexture(TextureTarget.Texture2D, Name, out GLTexture);
        GL.TextureStorage2D(GLTexture, MipmapLevels, InternalFormat, Width, Height);

        GL.TextureSubImage2D(GLTexture, 0, 0, 0, Width, Height, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
            PixelType.UnsignedByte, data);

        if (generateMipmaps) GL.GenerateTextureMipmap(GLTexture);

        SetWrap(TextureCoordinate.S, TextureWrapMode.Repeat);
        SetWrap(TextureCoordinate.T, TextureWrapMode.Repeat);

        GL.TextureParameter(GLTexture, TextureParameterName.TextureMaxLevel, MipmapLevels - 1);
    }

    public void Dispose()
    {
        GL.DeleteTexture(GLTexture);
    }

    internal void SetMinFilter(TextureMinFilter filter)
    {
        GL.TextureParameter(GLTexture, TextureParameterName.TextureMinFilter, (int)filter);
    }

    internal void SetMagFilter(TextureMagFilter filter)
    {
        GL.TextureParameter(GLTexture, TextureParameterName.TextureMagFilter, (int)filter);
    }

    internal void SetAnisotropy(float level)
    {
        const TextureParameterName TEXTURE_MAX_ANISOTROPY = (TextureParameterName)0x84FE;
        GL.TextureParameter(GLTexture, TEXTURE_MAX_ANISOTROPY, Util.Clamp(level, 1, MaxAniso));
    }

    internal void SetLod(int @base, int min, int max)
    {
        GL.TextureParameter(GLTexture, TextureParameterName.TextureLodBias, @base);
        GL.TextureParameter(GLTexture, TextureParameterName.TextureMinLod, min);
        GL.TextureParameter(GLTexture, TextureParameterName.TextureMaxLod, max);
    }

    internal void SetWrap(TextureCoordinate coord, TextureWrapMode mode)
    {
        GL.TextureParameter(GLTexture, (TextureParameterName)coord, (int)mode);
    }
}

internal struct UniformFieldInfo
{
    internal int Location;
    internal string Name;
    internal int Size;
    internal ActiveUniformType Type;
}

internal class Shader
{
    private readonly (ShaderType Type, string Path)[] Files;
    internal readonly string Name;
    private readonly Dictionary<string, int> UniformToLocation = new();
    private bool Initialized;

    internal Shader(string name, string vertexShader, string fragmentShader)
    {
        Name = name;
        Files = new[]
        {
            (ShaderType.VertexShader, vertexShader),
            (ShaderType.FragmentShader, fragmentShader)
        };
        Program = CreateProgram(name, Files);
    }

    internal int Program { get; }

    internal void UseShader()
    {
        GL.UseProgram(Program);
    }

    internal void Dispose()
    {
        if (Initialized)
        {
            GL.DeleteProgram(Program);
            Initialized = false;
        }
    }

    internal UniformFieldInfo[] GetUniforms()
    {
        GL.GetProgram(Program, GetProgramParameterName.ActiveUniforms, out var UnifromCount);

        var Uniforms = new UniformFieldInfo[UnifromCount];

        for (var i = 0; i < UnifromCount; i++)
        {
            var Name = GL.GetActiveUniform(Program, i, out var Size, out var Type);

            UniformFieldInfo FieldInfo;
            FieldInfo.Location = GetUniformLocation(Name);
            FieldInfo.Name = Name;
            FieldInfo.Size = Size;
            FieldInfo.Type = Type;

            Uniforms[i] = FieldInfo;
        }

        return Uniforms;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetUniformLocation(string uniform)
    {
        if (UniformToLocation.TryGetValue(uniform, out var location) == false)
        {
            location = GL.GetUniformLocation(Program, uniform);
            UniformToLocation.Add(uniform, location);

            if (location == -1) Debug.Print($"The uniform '{uniform}' does not exist in the shader '{Name}'!");
        }

        return location;
    }

    private int CreateProgram(string name, params (ShaderType Type, string source)[] shaderPaths)
    {
        Util.CreateProgram(name, out var Program);

        var Shaders = new int[shaderPaths.Length];
        for (var i = 0; i < shaderPaths.Length; i++)
            Shaders[i] = CompileShader(name, shaderPaths[i].Type, shaderPaths[i].source);

        foreach (var shader in Shaders)
            GL.AttachShader(Program, shader);

        GL.LinkProgram(Program);

        GL.GetProgram(Program, GetProgramParameterName.LinkStatus, out var Success);
        if (Success == 0)
        {
            var Info = GL.GetProgramInfoLog(Program);
            Debug.WriteLine($"GL.LinkProgram had info log [{name}]:\n{Info}");
        }

        foreach (var Shader in Shaders)
        {
            GL.DetachShader(Program, Shader);
            GL.DeleteShader(Shader);
        }

        Initialized = true;

        return Program;
    }

    private int CompileShader(string name, ShaderType type, string source)
    {
        Util.CreateShader(type, name, out var Shader);
        GL.ShaderSource(Shader, source);
        GL.CompileShader(Shader);

        GL.GetShader(Shader, ShaderParameter.CompileStatus, out var success);
        if (success == 0)
        {
            var Info = GL.GetShaderInfoLog(Shader);
            Debug.WriteLine($"GL.CompileShader for shader '{Name}' [{type}] had info log:\n{Info}");
        }

        return Shader;
    }
}