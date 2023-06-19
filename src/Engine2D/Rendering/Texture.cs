using System.Numerics;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.SavingLoading;
using Engine2D.UI;
using Engine2D.UI.Browsers;
using ImGuiNET;
using Newtonsoft.Json;
using Octokit;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Engine2D.Rendering;

public class Texture : AssetBrowserAsset
{
    public string Type = "Texture";

    [JsonIgnore] public int TexID;
    [JsonIgnore] public byte[] Data;
    [JsonIgnore] public bool Flipped;

    [JsonProperty]private string? _saveName = "";
    
    public string EncodedData;
    public int Height;
    public string Filepath;
    public int Width;
    public TextureMinFilter MinFilter;
    public TextureMagFilter MagFilter;

    public Texture(string filepath, string? saveName, bool flipped, TextureMinFilter minFilter, TextureMagFilter magFilter)
    {
        this._saveName = saveName;
        Filepath = filepath;
        MinFilter = minFilter;
        MagFilter = magFilter;
        Flipped = flipped;
        
        Gen();
        LoadFromImage();
        CreateOpenGL();
        EncodedData = Convert.ToBase64String(Data);
    }

    //
    [JsonConstructor]
    public Texture(
        string? savePath,
        string encodedData,
        int height, int width,
        TextureMinFilter MinFilter,
        TextureMagFilter MagFilter)
    {
        this._saveName = savePath;
        this.EncodedData = encodedData;
        this.Width = width;
        this.Height = height;
        this.MagFilter = MagFilter;
        this.MinFilter = MinFilter;

        Data = Convert.FromBase64String(EncodedData);

        Gen();
        CreateOpenGL();

        Log.Succes("Create Image From JSON");
    }

    private void Gen()
    {
        TexID = GL.GenTexture();
        // Bind the handle
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, TexID);
    }

    private void LoadFromImage()
    {
        var flipVal = 1;
        if (!Flipped) flipVal = 0;
        StbImage.stbi_set_flip_vertically_on_load(flipVal);

        try
        {
            using (Stream stream = File.OpenRead(Filepath))
            {
                var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                Data = image.Data;
                Width = image.Width;
                Height = image.Height;
            }
        }
        catch
        {
            Log.Error("No file" + Filepath);
            Filepath = Utils.GetBaseEngineDir() + "\\Images\\icons\\not-found-icon.jpg";
            using (Stream stream = File.OpenRead(Filepath))
            {
                var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                Data = image.Data;
                Width = image.Width;
                Height = image.Height;
            }
        }
    }

    private void CreateOpenGL()
    {
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8,
            Width, Height, 0,
            PixelFormat.Rgba, PixelType.UnsignedByte, Data);

        // Now that our texture is loaded, we can set a few settings to affect how the image appears on rendering.

        // First, we set the min and mag filter. These are used for when the texture is scaled down and up, respectively.
        // Here, we use Linear for both. This means that OpenGL will try to blend pixels, meaning that textures scaled too far will look blurred.
        // You could also use (amongst other options) Nearest, which just grabs the nearest pixel, which makes the texture look pixelated if scaled too far.
        // NOTE: The default settings for both of these are LinearMipmap. If you leave these as default but don't generate mipmaps,
        // your image will fail to render at all (usually resulting in pure black instead).
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)MinFilter);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
            (int)MagFilter);

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

    public static int GenTexture(int width, int height, TextureMagFilter magFilter, TextureMinFilter minFilter,
        TextureTarget textureTarget = TextureTarget.Texture2D)
    {
        int id = GL.GenTexture();

        var nullPtr = IntPtr.Zero;

        // Bind the handle            
        GL.BindTexture(textureTarget, id);
        GL.TexParameter(textureTarget, TextureParameterName.TextureMinFilter,
            (int)minFilter);
        GL.TexParameter(textureTarget, TextureParameterName.TextureMagFilter,
            (int)magFilter);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb,
            PixelType.UnsignedByte, nullPtr);

        return id;
    }

    public void bind()
    {
        GL.BindTexture(TextureTarget.Texture2D, TexID);
    }

    public void unbind()
    {
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            Log.Warning("Obj is null");
            return false;
        }

        if (!(obj is Texture))
        {
            Log.Warning("other object is not an texture");
            return false;
        }

        var oTex = (Texture)obj;
        return oTex.Width.Equals(Width) && oTex.Height.Equals(Height) && oTex.TexID.Equals(TexID)
               && oTex.Filepath.Equals(Filepath);
    }

    public override void OnGui()
    {
        ImGui.Begin("Texture inspect");
        ImGui.Text("Image Preview");
        ImGui.SameLine();
        var w = Width;
        var h = Height;

        if (h > 32)
        {
            int dif = Height - 32;
            w = w - dif;
            h = h - dif;
        }

        if (ImGui.Button("Save"))
        {
            Save();
        }

        ImGui.Image(this.TexID, new Vector2(w, h), UISETTINGS.ImageUV0, UISETTINGS.ImageUV1);

        int currentIndexMinFilter = (MinFilter == TextureMinFilter.Linear) ? 0 : 1;

        if (ImGui.Combo("Min Filter: ", ref currentIndexMinFilter,
                "TextureMinFilter.Linear\0" +
                "TextureMinFilter.Nearest\0"
            ))
        {
            SelectNewMinFilter(currentIndexMinFilter);
            ImGui.EndCombo();
        }
        
        int currentIndexMagFilter = (MagFilter == TextureMagFilter.Linear) ? 0 : 1;

        if (ImGui.Combo("Mag Filter: ", ref currentIndexMagFilter,
                "TextureMagFilter.Linear\0" +
                "TextureMagFilter.Nearest\0"
            ))
        {
            SelectNewMagFilter(currentIndexMagFilter);
            ImGui.EndCombo();
        }

        
        ImGui.End();
    }

    private void SelectNewMinFilter(int index)
    {
        if (index == 0)
        {
            MinFilter = TextureMinFilter.Linear;
        }
        else
        {
            MinFilter = TextureMinFilter.Nearest;
        }
    }

    private void SelectNewMagFilter(int index)
    {
        if (index == 0)
        {
            MagFilter = TextureMagFilter.Linear;
        }
        else
        {
            MagFilter = TextureMagFilter.Nearest;
        }
    }
    
    public static void Use(TextureUnit unit, int textureID)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, textureID);
    }
    
    public void Save()
    {
        if (_saveName == "")
        {
            Log.Error("Save name not set, not saving!");
            return;
        }
        
        ResourceManager.SaveTexture(_saveName, this, null, true);
    }
}