using OpenTK.Graphics.OpenGL4;

namespace Engine2D.Rendering
{
    public class FrameBuffer
    {
      
        private int _fboID = 0;
        private Texture _texture;
        public int TextureID => _texture.TexID;

        internal FrameBuffer(int width, int height)
        {
            _fboID = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fboID);

            // Create the texture to render the data to, and attach it to our framebuffer
            _texture = new Texture(width, height);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                    TextureTarget.Texture2D,
                    this._texture.TexID, 0);

            // Create renderbuffer store the depth info
            int rboID = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboID);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                    RenderbufferTarget.Renderbuffer,
                    rboID);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        internal void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fboID);
        }

        internal void UnBind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }


    }
}
