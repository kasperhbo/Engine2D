using System.Numerics;
using Engine2D.Core;
using Newtonsoft.Json;

namespace Engine2D.Rendering;

public class Sprite : Asset
{
    public string Type = "Sprite";
    
    public Vector2[] TextureCoords =
    {
        new(1, 1),
        new(1, 0),
        new(0, 0),
        new(0, 1)
    };

    public Texture? Texture = null;
    
    public void Init()
    {
    }

    public override void OnGui()
    {
    }
}