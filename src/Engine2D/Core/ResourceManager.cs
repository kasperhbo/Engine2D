using Engine2D.Components;
using Engine2D.Rendering;
using KDBEngine.Shaders;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp.Processing.Processors.Dithering;

namespace Engine2D.Core;

internal struct ShaderData
{
    internal string FragPath;
    internal string VertexPath;
}

[JsonConverter(typeof(ComponentSerializer))]
public class TextureData
{
    public bool flipped;
    public TextureMagFilter magFilter;
    public TextureMinFilter minFilter;
    public string texturePath;

    public string Type { get; }= "TextureData";

    public TextureData()
    {
    }

    public TextureData(string texturePath, bool flipped, TextureMinFilter minFilter, TextureMagFilter magFilter)
    {
        this.texturePath = texturePath;
        this.flipped = flipped;
        this.minFilter = minFilter;
        this.magFilter = magFilter;
    }

    public TextureData Copy(TextureData to, string filePath)
    {
        to.texturePath = filePath;
        to.flipped = flipped;
        to.minFilter = minFilter;
        to.magFilter = magFilter;
        return to;
    }
    
    public static TextureData CopyToNew(TextureData from,string filePath)
    {
        TextureData to = new TextureData();
        to.texturePath = filePath;
        to.flipped =   from.flipped;
        to.minFilter = from.minFilter;
        to.magFilter = from.magFilter;
        return to;
    }
}

internal static class ResourceManager
{
    private static readonly Dictionary<ShaderData, Shader> _shaders = new();
    private static readonly Dictionary<TextureData, Texture> _textures = new();

    internal static Shader GetShader(ShaderData shaderLocations)
    {
        Shader shader;
        if (!_shaders.TryGetValue(shaderLocations, out shader))
        {
            shader = new Shader(shaderLocations.VertexPath, shaderLocations.FragPath);
            _shaders.Add(shaderLocations, shader);
        }

        return shader;
    }

    internal static Texture GetTexture(TextureData textureData)
    {
        Texture tex;
        if (!_textures.TryGetValue(textureData, out tex))
        {
            tex = new Texture(textureData.texturePath, textureData.flipped, textureData.minFilter,
                textureData.magFilter);
            _textures.Add(textureData, tex);
        }

        return tex;
    }
}