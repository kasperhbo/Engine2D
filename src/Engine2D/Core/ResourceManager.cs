using Engine2D.Components;
using Engine2D.Rendering;
using KDBEngine.Shaders;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.Core;

public class ShaderData
{
    public string Identifier;
    private string _vertexPath;
    private string _fragPath;

    public Shader Shader;

    public ShaderData(string identifier, string vertexPath, string fragPath)
    {
        Identifier = identifier;
        _vertexPath = vertexPath;
        _fragPath = fragPath;
    }

    public ShaderData(string identifier, string vertexPath, string fragPath, Shader shader)
    {
        Identifier = identifier;
        _vertexPath = vertexPath;
        _fragPath = fragPath;
        Shader = shader;
    }

    public void CreateShader()
    {
        Shader = new Shader(_vertexPath, _fragPath);
    }
}

[JsonConverter(typeof(ComponentSerializer))]
public class TextureData
{
    public string ItemType = "TextureData";
    public string Identifier;
    public string _filePath;
    public bool _flipped;
    public TextureMinFilter _textureMinFilter;
    public TextureMagFilter _textureMagFilter;

    [JsonIgnore]public Texture Texture;
    
    public TextureData(string identifier, string filePath, bool flipped,TextureMinFilter textureMinFilter,TextureMagFilter textureMagFilter)
    {
        Identifier = identifier;
        _filePath = filePath;
        _flipped = flipped;
        _textureMagFilter = textureMagFilter;
        _textureMinFilter = textureMinFilter;
    }

    public void CreateTexture()
    {
        Texture = new Texture(
            _filePath,
            _flipped,
            _textureMinFilter,
            _textureMagFilter
        );
    }
}

public static class ResourceManager
{
    private static Dictionary<string, ShaderData> s_shaders = new Dictionary<string, ShaderData>();
    private static Dictionary<string, TextureData> s_textures = new Dictionary<string, TextureData>();


    public static Texture GetTexture(TextureData textureData)
    {
        TextureData data;
        if (s_textures.TryGetValue(textureData.Identifier, out data ))
        {
            return data.Texture;
        }
        
        textureData.CreateTexture();
        s_textures.Add(textureData.Identifier, textureData);
        
        return textureData.Texture;
    }  
    
    public static Texture GetTexture(string identifier)
    {
        TextureData data;
        if (s_textures.TryGetValue(identifier, out data ))
        {
            return data.Texture;
        }

        throw new Exception("ONLY CALL THIS WHEN YOU ARE SURE YOU CREATED THE TEXTURE BEFORE");
    }  
    
    public static Shader GetShader(ShaderData shaderData)
    {
        ShaderData data;
        if (s_shaders.TryGetValue(shaderData.Identifier, out data ))
        {
            return data.Shader;
        }
        
        shaderData.CreateShader();
        s_shaders.Add(shaderData.Identifier, shaderData);
        
        return shaderData.Shader;
    }  

}
