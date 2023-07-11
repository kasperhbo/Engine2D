using System.Numerics;

namespace Engine2D.Components.Sprites;

public class Sprite
{
    public Vector2[] TextureCoords { get; set; } = new[]
    {
        new Vector2(0,1),
        new Vector2(0,1),
        new Vector2(0,1),
        new Vector2(0,1),
    };
}