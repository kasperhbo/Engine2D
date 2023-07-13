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

    [JsonProperty]
    internal Vector2[] TextureCoords { get; set; } = new Vector2[4]
    {
        new Vector2(0f, 0f),
        new Vector2(1f, 0f),
        new Vector2(1f, 1f),
        new Vector2(0f, 1f)
    };

    [JsonIgnore] internal Texture? Texture { get; set; } = null;

    [JsonConstructor]
    internal Sprite(string savePath, string texturePath, Vector2[] textureCoords)
    {
        SavePath = savePath;
        TexturePath = texturePath;
        TextureCoords = textureCoords;
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