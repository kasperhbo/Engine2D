﻿#region

using System.Numerics;
using Engine2D.Managers;
using Engine2D.Rendering;
using Newtonsoft.Json;

#endregion

namespace Engine2D.Components.Sprites;

public class Sprite
{
    public Sprite(string fullSavePath, Vector2[] texCoords, int spriteWidth, int spriteHeight, int index)
    {
        FullSavePath = fullSavePath;
        TextureCoords = texCoords;
        Width = spriteWidth;
        Height = spriteHeight;
        Index = index;
    }
    //
    // public Sprite(Vector4 color, Vector2[] textureCoords)
    // {
    //     this.Color = color;
    //     this.TextureCoords = textureCoords;
    // }

    [JsonProperty] public Vector4 Color { get; set; } = new Vector4(1, 1, 1, 1);

    [JsonProperty]public string FullSavePath { get; set; }
    [JsonProperty]public int Index { get; set; }
    
    [JsonProperty]internal int Width { get; set; }
    [JsonProperty]internal int Height { get; set; }
    [JsonProperty]internal Vector2[] TextureCoords { get; set; } = new Vector2[4];
    [JsonIgnore]internal Texture? Texture => ResourceManager.GetItem<SpriteSheet>(FullSavePath)?.Texture;
}