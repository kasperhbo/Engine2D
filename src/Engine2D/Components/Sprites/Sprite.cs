using System.Numerics;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.Rendering;
using Newtonsoft.Json;

namespace Engine2D.Components.Sprites;

internal class Sprite : AssetBrowserAsset
{
    [JsonProperty] internal string SavePath = "";
    [JsonProperty] internal string TexturePath = "";

    [JsonProperty] private int width = -1;
    [JsonProperty] private int height = -1;
    
    [JsonProperty]
    internal Vector2[] TextureCoords { get; set; } = new Vector2[4]
    {
        new Vector2(0f, 0f),
        new Vector2(1f, 0f),
        new Vector2(1f, 1f),
        new Vector2(0f, 1f)
    };

    [JsonIgnore]
    internal int Width
    {
        get
        {
            if (width != -1)
            {
                return width;
            }       
            else
            {
                if (Texture != null)
                {
                    return Texture.Width;
                }
                else
                {
                    Log.Error("Texture is null");
                    return 1;
                }
            }
        }
    }
    
    [JsonIgnore] internal int Height 
    {
        get
        {
            if (height != -1)
            {
                return height;
            }       
            else
            {
                if (Texture != null)
                {
                    return Texture.Height;
                }
                else
                {
                    Log.Error("Texture is null");
                    return 1;
                }
            }
        }
    } 
    
    [JsonIgnore] internal Texture? Texture { get; set; } = null;

    [JsonConstructor]
    internal Sprite(
        string savePath, 
        string texturePath,
        int width, int height,
        Vector2[] textureCoords
)
    {
        this.SavePath = savePath;
        this.TexturePath = texturePath;
        this.TextureCoords = textureCoords;
        if(width == 0 && height == 0)
        {
            this.width = -1;
            this.height = -1;
        }
        else
        {
            this.width = width;
            this.height = height;
        }
        
        SetSprite();
    }
    
    internal Sprite(string? texturePath)
    {
        TexturePath = texturePath;
        SetSprite();
    }

    private void SetSprite()
    {
        if(TexturePath == "" || TexturePath == null)
        {
            Log.Error("TexturePath is empty");
            return;
        }
        
        var item = ResourceManager.GetItem<Texture>(TexturePath);
        Texture = item;
    }
    
    

    internal override void OnGui()
    {
        
    }

    internal override void Refresh()
    {
        
    }
}