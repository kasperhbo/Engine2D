using Engine2D.Rendering;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Testing
{
    internal class TestFrameBuffer
    {
        private int fboId = 0;
        private Texture texture;

        public TestFrameBuffer(int width, int height)
        {
            // Generate framebuffer
            fboId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboId);

            this.texture = new Texture(width, height);
            GL.FramebufferTexture2D(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D,
                this.texture.TexID,
                0
            );

            // Create renderbuffer store the depth info
            int rboId = GL.GenRenderbuffer();
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

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboId);
        }

        public void UnBind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public int GetTextureID => texture.TexID;
    }
}
