#region

using Engine2D.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using TextureMagFilter = OpenTK.Graphics.OpenGL4.TextureMagFilter;
using TextureMinFilter = OpenTK.Graphics.OpenGL4.TextureMinFilter;

#endregion

namespace Engine2D.Testing;

internal class TestFrameBuffer
{
    private int fboId;
    private int rboId;
    internal Vector2i Size;

    internal TestFrameBuffer(Vector2i size)
    {
        Init(size.X, size.Y);
    }

    internal TestFrameBuffer(int width, int height)
    {
        Init(width, height);
    }

    internal int TextureID { get; private set; } = -1;

    private void Init(int width, int height)
    {
        Size = new Vector2i(width, height);
        // Generate framebuffer
        fboId = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboId);

        TextureID = Texture.GenTexture(width, height, TextureMagFilter.Linear, TextureMinFilter.Linear);

        GL.FramebufferTexture2D(
            FramebufferTarget.Framebuffer,
            FramebufferAttachment.ColorAttachment0,
            TextureTarget.Texture2D,
            TextureID,
            0
        );

        // Create renderbuffer store the depth info
        rboId = GL.GenRenderbuffer();
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboId);
        GL.RenderbufferStorage(
            RenderbufferTarget.Renderbuffer,
            RenderbufferStorage.DepthComponent32,
            width,
            height
        );
        GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
            FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rboId);

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    internal void Resize(int width, int height)
    {
        Size.X = width;
        Size.Y = height;
        // resize renderbuffer

        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboId);
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32,
            width, height);
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

        GL.BindTexture(TextureTarget.Texture2D, TextureID);
        GL.TexImage2D(TextureTarget.Texture2D, 0,
            PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

        // GL.GenerateMipmap(GenerateMipmapTarget.Texture2D); // do i need this?
    }

    internal void Bind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboId);
    }

    internal void UnBind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    internal void BindToSlot(int unit)
    {
        GL.ActiveTexture(TextureUnit.Texture0 + unit);
        GL.BindTexture(TextureTarget.Texture2D, TextureID);
    }

    internal TestFrameBuffer SetViewportSize(Vector2i viewportSize)
    {
        if (viewportSize.X == Size.X) return null;
        return new TestFrameBuffer(viewportSize);
    }
}