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

    internal Sprite()
    {
        
    }
    
    internal Sprite(string? texturePath)
    {
        TexturePath = texturePath;
        SetSprite();
    }

    public void SetSprite()
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