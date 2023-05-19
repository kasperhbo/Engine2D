using KDBEngine.Core;
using OpenTK.Graphics.OpenGL4;
using TextureTarget = OpenTK.Graphics.OpenGL4.TextureTarget;


namespace Engine2D.Rendering.Buffers;

public class Framebuffer
{
    private int _height;
    private int _id = -1;
    private int _width;
    
    private FramebufferSpec _spec;
    
    /** The color attachment textures' specifications */
    private List<FramebufferTextureSpec> colorAttachmentSpecs;
    /** The depth attachment texture's specification. Initialized to an Invalid default */
    private FramebufferTextureSpec depthAttachmentSpec = new FramebufferTextureSpec();

    /** Color attachment textures to which the framebuffer renders to */
    private List<int> colorAttachmentTextures;
    /** Depth attachment texture to which the framebuffer renders to */
    private int depthAttachmentTexture;

    /** Static list maintaining all framebuffers so as to delete them all in the end */
    private static List<Framebuffer> fbos = new List<Framebuffer>();
    
    public Framebuffer(int width, int height, FramebufferSpec spec) {
        _width = width;
        _height = height;
        _spec = spec;

        colorAttachmentSpecs = new();
        colorAttachmentTextures = new();

        // Sort the texture formats.
        foreach (FramebufferTextureSpec format in spec.attachments) {
            if (!format.format.isDepth) {
                colorAttachmentSpecs.Add(format);
            } else {
                depthAttachmentSpec = format;
            }
        }
        // Generate the framebuffer
        Invalidate();

        fbos.Add(this);
    }
    
    private Framebuffer(int id) {
        _id = id;
    }
    
    public static Framebuffer CreateDefault() {
        return new Framebuffer(0);
    }
    
    public int FetchColorAttachment(int i) {
        if (colorAttachmentTextures.Count() >= i)
            return colorAttachmentTextures[i];
        return -1;
    }
    
    public int FetchDepthAttachment() {
        return depthAttachmentTexture;
    }

    public void BlitColorBuffersToScreen() {
        GL.BlitNamedFramebuffer(_id, 0,
            0, 0, _width, _height,
            0, 0, Engine.Get().Size.X, Engine.Get().ClientSize.Y,
            ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
    }
    
    public void BlitEntireFboToScreen()
    {
	    GL.BlitNamedFramebuffer(_id, 0,
            0, 0, _width, _height,
            0, 0, Engine.Get().Size.X, Engine.Get().Size.Y,
            ClearBufferMask.ColorBufferBit |ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit, BlitFramebufferFilter.Linear);
    }
    
    /**
     * Deletes the framebuffer if it was already created.
     * Then it creates a brand new framebuffer and adds new texture attachments to it based on the spec
     */
    private void Invalidate() {
		if (_id != -1)
			Delete();

		GL.CreateFramebuffers(1, out _id);
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, _id);

		// If there are any color attachments requested, create them all
		if (colorAttachmentSpecs.Count() > 0) {
			colorAttachmentTextures.Clear();

			for (int i = 0; i < colorAttachmentSpecs.Count(); i++) {
				FramebufferTextureSpec format = colorAttachmentSpecs[i];
				int texture = CreateColorTexture(this._width, this._height, format.format.internalFormat,
					(int)format.format.format, (int)PixelType.UnsignedByte);
				colorAttachmentTextures.Add(texture);

				// Set the Texture's resizing and wrap parameters as per the specification
				GL.TextureParameter((int)TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
				 		(int)format.minificationFilter.glType);
				 		
				GL.TextureParameter((int)TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
				 		(int)format.magnificationFilter.glType);
				 		
				GL.TextureParameter((int)TextureTarget.Texture2D, TextureParameterName.TextureWrapR,
				 		(int)format.rFilter.GLType);
				 		
				GL.TextureParameter((int)TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
				 		(int)format.sFilter.GLType);
				 		
				GL.TextureParameter((int)TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
				 		(int)format.tFilter.GLType);
				 		

				GL.FramebufferTexture2D(
					FramebufferTarget.Framebuffer, 
					FramebufferAttachment.ColorAttachment0 + i, 
					TextureTarget.Texture2D, 
					_id, 
					0);
			}
		}

		// If the depth attachment spec is not the default
		if (depthAttachmentSpec.format.isDepth) {
			// Generate the depth texture
			depthAttachmentTexture = CreateDepthTexture(this._width, this._height, depthAttachmentSpec.format.internalFormat);

			// Set the Texture's resizing and wrap parameters as per the specification
			GL.TextureParameter((int)TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)depthAttachmentSpec.minificationFilter.glType);
			GL.TextureParameter((int)TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)depthAttachmentSpec.magnificationFilter.glType);
			GL.TextureParameter((int)TextureTarget.Texture2D, TextureParameterName.TextureWrapR,     (int)depthAttachmentSpec.rFilter.GLType);
			GL.TextureParameter((int)TextureTarget.Texture2D, TextureParameterName.TextureWrapS,     (int)depthAttachmentSpec.sFilter.GLType);
			GL.TextureParameter((int)TextureTarget.Texture2D, TextureParameterName.TextureWrapT,     (int)depthAttachmentSpec.tFilter.GLType);
			
			GL.FramebufferTexture2D(
				FramebufferTarget.Framebuffer, (FramebufferAttachment)depthAttachmentSpec.format.format, 
				TextureTarget.Texture2D, _id, 0);
		}

		// Check if the framebuffer is complete
		if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete) {
			Logging.Log.Error("Incomplete Framebuffer :(");
		}

		// Unbind this fbo
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
    
    public void Resize(int width, int height) {
	    this._width = width;
	    this._height = height;
	    Invalidate();
    }
    
    private static int CreateColorTexture(int width, int height, int internalFormat, int format, int type) {
	    GL.CreateTextures(TextureTarget.Texture2D, 1, out int texture);
	    GL.BindTexture(TextureTarget.Texture2D, texture);

	    GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)internalFormat, width, height,
		    0, (PixelFormat)format, (PixelType)type, IntPtr.Zero);

	    return texture;
    }
    
    private static int CreateDepthTexture(int width, int height, int internalFormat) {
	    GL.CreateTextures(TextureTarget.Texture2D, 1, out int texture);
	    GL.BindTexture(TextureTarget.Texture2D, texture);

	    GL.TexStorage2D(TextureTarget2d.Texture2D,1, (SizedInternalFormat)internalFormat,width,height);
	    
	    return texture;
    }
    
    public void Bind() {
	    GL.BindFramebuffer(FramebufferTarget.Framebuffer, this._id);
    }
    
    public static void Unbind() {
	    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
    
    public void Delete()
    {
	    colorAttachmentTextures.ForEach(i => GL.DeleteTexture(i));
	   	GL.DeleteFramebuffer(this._id);
    }
    
    public static void Clean() {
	    fbos.ForEach(framebuffer => framebuffer.Delete());
    }

}