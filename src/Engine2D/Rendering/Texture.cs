using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Engine2D.Rendering;

public class Texture
{
    public readonly int TexID;

    public Texture(string filepath, bool flipped, TextureMinFilter minFilter, TextureMagFilter magFilter)
    {
        Filepath = filepath;
        // Generate handle
        TexID = GL.GenTexture();

        // Bind the handle
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, TexID);

        // For this example, we're going to use .NET's built-in System.Drawing library to load textures.

        // OpenGL has it's texture origin in the lower left corner instead of the top left corner,
        // so we tell StbImageSharp to flip the image when loading.
        var flipVal = 0;
        if (flipped) flipVal = 1;
        StbImage.stbi_set_flip_vertically_on_load(flipVal);

        // Here we open a stream to the file and pass it to StbImageSharp to load.
        using (Stream stream = File.OpenRead(filepath))
        {
            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            Width = image.Width;
            Height = image.Height;
            // Now that our pixels are prepared, it's time to generate a texture. We do this with GL.TexImage2D.
            // Arguments:
            //   The type of texture we're generating. There are various different types of textures, but the only one we need right now is Texture2D.
            //   Level of detail. We can use this to start from a smaller mipmap (if we want), but we don't need to do that, so leave it at 0.
            //   Target format of the pixels. This is the format OpenGL will store our image with.
            //   Width of the image
            //   Height of the image.
            //   Border of the image. This must always be 0; it's a legacy parameter that Khronos never got rid of.
            //   The format of the pixels, explained above. Since we loaded the pixels as ARGB earlier, we need to use BGRA.
            //   Data type of the pixels.
            //   And finally, the actual pixels.
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        }

        // Now that our texture is loaded, we can set a few settings to affect how the image appears on rendering.

        // First, we set the min and mag filter. These are used for when the texture is scaled down and up, respectively.
        // Here, we use Linear for both. This means that OpenGL will try to blend pixels, meaning that textures scaled too far will look blurred.
        // You could also use (amongst other options) Nearest, which just grabs the nearest pixel, which makes the texture look pixelated if scaled too far.
        // NOTE: The default settings for both of these are LinearMipmap. If you leave these as default but don't generate mipmaps,
        // your image will fail to render at all (usually resulting in pure black instead).
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)minFilter);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
            (int)magFilter);

        // Now, set the wrapping mode. S is for the X axis, and T is for the Y axis.
        // We set this to Repeat so that textures will repeat when wrapped. Not demonstrated here since the texture coordinates exactly match
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        // Next, generate mipmaps.
        // Mipmaps are smaller copies of the texture, scaled down. Each mipmap level is half the size of the previous one
        // Generated mipmaps go all the way down to just one pixel.
        // OpenGL will automatically switch between mipmaps when an object gets sufficiently far away.
        // This prevents moiré effects, as well as saving on texture bandwidth.
        // Here you can see and read about the morié effect https://en.wikipedia.org/wiki/Moir%C3%A9_pattern
        // Here is an example of mips in action https://en.wikipedia.org/wiki/File:Mipmap_Aliasing_Comparison.png
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }

    public Texture(int width, int height)
    {
        Height = height;
        Width = width;
        Filepath = "Generated";

        //    // Generate texture on GPU
        //    TexID = GetHas;
        //    GL.BindTexture(TextureTarget.Texture2D, TexID);

        //    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, 0);


        // Generate handle
        TexID = GL.GenTexture();

        var nullPtr = IntPtr.Zero;

        // Bind the handle            
        GL.BindTexture(TextureTarget.Texture2D, TexID);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
            (int)TextureMagFilter.Linear);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb,
            PixelType.UnsignedByte, nullPtr);
    }

    public object Height { get; }
    public object Filepath { get; }
    public object Width { get; }

    public void bind()
    {
        GL.BindTexture(TextureTarget.Texture2D, TexID);
    }

    public void unbind()
    {
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            Console.WriteLine("Obj is null");
            return false;
        }

        if (!(obj is Texture)) return false;

        var oTex = (Texture)obj;
        return oTex.Width.Equals(Width) && oTex.Height.Equals(Height) && oTex.TexID.Equals(TexID)
               && oTex.Filepath.Equals(Filepath);
    }

    public void Use(TextureUnit unit)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, TexID);
    }
}