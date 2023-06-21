#region

using System.Numerics;
using Newtonsoft.Json;

#endregion

namespace Engine2D.Components.Sprites;

internal class SpriteSheetSprite
{
    [JsonProperty]internal Vector2[] TextureCoords =
    {
        new(0.0f, 0.0f),
        new(1.0f, 0.0f),
        new(1.0f, 1.0f),
        new(0.0f, 1.0f)
    };

    [JsonProperty]internal string FullSavePath { get; set; }
    [JsonProperty]internal int Width { get; set; }
    [JsonProperty]internal int Height { get; set; }
}