using Engine2D.Rendering;
using OpenTK.Graphics.OpenGL;

namespace Engine2D.Testing;

internal class TestFrameBuffer
{
    private readonly int fboId;
    public Texture Texture { get; private set; }

    public TestFrameBuffer(int width, int height)
    {
        // Generate framebuffer
        fboId = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboId);

        Texture = new Texture(width, height);
        GL.FramebufferTexture2D(
            FramebufferTarget.Framebuffer,
            FramebufferAttachment.ColorAttachment0,
            TextureTarget.Texture2D,
            Texture.TexID,
            0
        );

        // Create renderbuffer store the depth info
        var rboId = GL.GenRenderbuffer();
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

    public int GetTextureID => Texture.TexID;

    public void Bind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboId);
    }

    public void UnBind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
}