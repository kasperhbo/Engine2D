using System.Numerics;
using Engine2D.Core;
using Engine2D.Managers;
using Engine2D.SavingLoading;
using Engine2D.UI;
using ImGuiNET;
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

    [JsonIgnore]public Texture? Texture { get; init; } = null;
    public TextureData TextureData { get; private set; }
    public string FullSavePath { get; set; }

    public Sprite(TextureData textureData)
    {
        TextureData = textureData;
        Texture = ResourceManager.GetTexture(textureData);
        AssetName = this.FullSavePath;
    }

    public void Init()
    {
    }

    public override void OnGui()
    {
        if (ImGui.Button("Save"))
        {
            SaveLoad.OverWriteSprite(this);
            SpriterendererManager.UpdateSpriteRenderers(this.FullSavePath);
        };
        
        OpenTKUIHelper.DrawComponentWindow(
            "Sprite Settings",
            "Sprite Settings",
            OnGuiItems,
            ImGui.GetContentRegionAvail().Y
        );
    }

    private void OnGuiItems()
    {
        for (int i = 0; i < TextureCoords.Length; i++)
        {
            Vector2 temp = TextureCoords[i];
            OpenTKUIHelper.DrawProperty("Coord " + i, ref temp);
            TextureCoords[i] = temp;
        }
    }
}