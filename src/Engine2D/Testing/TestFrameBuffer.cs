using System.Security.Cryptography;
using Engine2D.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Engine2D.Testing;

internal class TestFrameBuffer
{
    private int fboId;
    public Texture Texture { get; private set; }

    public TestFrameBuffer(Vector2i size)
    {
        Init(size.X, size.Y);
    }    
    
    public TestFrameBuffer(int width, int height)
    {
        Init(width, height);
    }

<<<<<<< HEAD
    public Texture? Texture { get; private set; }

    public int GetTextureID => Texture.TexID;

=======
>>>>>>> parent of efcdaf4... AUTO REFACTORIO
    private void Init(int width, int height)
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
        //GL.GetNamedFramebufferAttachmentParameter(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, );
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

    public void BindToSlot(int unit)
    {
        GL.ActiveTexture(TextureUnit.Texture0 + unit);
        GL.BindTexture(TextureTarget.Texture2D, GetTextureID);
    }
}