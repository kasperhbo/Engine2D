using System.Numerics;
using Engine2D.Core;
using Newtonsoft.Json;

namespace Engine2D.Rendering;

public class Sprite : Asset
{
    public string Type = "Sprite";
    
    // 0.5f,   0.5f, 0.0f,    1.0f, 1.0f,   // top right
    // 0.5f,  -0.5f, 0.0f,    1.0f, 0.0f,   // bottom right
    // -0.5f, -0.5f, 0.0f,    0.0f, 0.0f,   // bottom left
    // -0.5f,  0.5f, 0.0f,    0.0f, 1.0f    // top left 
    
    public Vector2[] TextureCoords =
    {
        new(1.0f, 1.0f),
        new(1.0f, 0.0f),
        new(0.0f, 0.0f),
        new(0.0f, 1.0f)
    };

    public Texture? Texture = null;
    
    public void Init()
    {
    }

    public override void OnGui()
    {
    }
}