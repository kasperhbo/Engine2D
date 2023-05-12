using System.Numerics;
using Engine2D.Core;
using Newtonsoft.Json;

namespace Engine2D.Rendering;

public class Sprite : Asset
{
    public bool IsDirty = false;
    [JsonIgnore] public Texture? Texture;

    public Vector2 TextureCoords = new(1, 1);

    public TextureData TextureData = new();

    public string Type = "Sprite";

    public void Init(Vector2 textureCoords, TextureData textureData)
    {
        TextureCoords = textureCoords;
        TextureData = textureData;
        Texture = ResourceManager.GetTexture(TextureData);
    }

    public override void OnGui()
    {
    }
}