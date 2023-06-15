using System.Numerics;

namespace Engine2D.Components.Sprites;

public class SpriteSheetSprite
{
    public Vector2[] TextureCoords =
    {
        new(0.0f, 0.0f),
        new(1.0f, 0.0f),
        new(1.0f, 1.0f),
        new(0.0f, 1.0f)
    };

    public string FullSavePath { get; set; }
    public int Width { get; set; }
    public int Height { get; set;}

    public string? TexturePath = "";


}