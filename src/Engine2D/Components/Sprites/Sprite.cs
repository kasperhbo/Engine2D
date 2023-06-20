using System.Numerics;
using Engine2D.Core;
using Engine2D.Managers;
using Engine2D.Rendering;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.Components.Sprites;

public class Sprite : AssetBrowserAsset
{
    public string Type = "Sprite";
    
    // 0.5f,   0.5f, 0.0f,    1.0f, 1.0f,   // top right
    // 0.5f,  -0.5f, 0.0f,    1.0f, 0.0f,   // bottom right
    // -0.5f, -0.5f, 0.0f,    0.0f, 0.0f,   // bottom left
    // -0.5f,  0.5f, 0.0f,    0.0f, 1.0f    // top left 
    
    [JsonIgnore]public Vector2[] TextureCoords =
    {
        new Vector2(1, 1),
        new Vector2(1, 0),
        new Vector2(0, 0),
        new Vector2(0, 1f),
    };

    [JsonIgnore]public Texture? Texture { get; init; }
    
    public string? TexturePath = "";
    private bool _unsaved;
    public string? FullSavePath { get; set; }
    public int Width { get; set; }
    public int Height { get; set;}
    
    public Sprite(string? texturePath)
    {
        TexturePath = texturePath;

        if (texturePath == "")
        {
            Texture = new Texture(Utils.GetBaseEngineDir() + "\\Images\\Icons\\not-found-icon.png","",
                false, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
        }
        else
        {
            Texture = ResourceManager.LoadTextureFromJson(texturePath);
        }

        AssetName = FullSavePath;
    }

  
    
    [JsonConstructor]
    public Sprite(string type,  string? texturePath, string? assetName ,string? fullPath)
    {
        Type = type;
        TexturePath = texturePath;
        AssetName = assetName;
        FullSavePath = fullPath;
        
        if (TexturePath != "")
        {
            Texture = ResourceManager.LoadTextureFromJson(TexturePath);
        }
        else
        {
            Texture = IconManager.GetIcon("not-found-icon");
        }
        
        AssetName = FullSavePath;
    }


    public override void OnGui()
    {
        if(_unsaved) ImGui.Begin("Sprite editor", ImGuiWindowFlags.UnsavedDocument);
        else ImGui.Begin("Sprite editor");
        
        if (ImGui.Button("Save"))
        {
            Save();
        };
        
        var w = Width;
        var h = Height;

        if (h > 128)
        {
            int dif = Texture.Height - 128;
            w = w - dif;
            h = h - dif;
        }
        
        ImGui.Image(Texture.TexID, new Vector2(w, h), TextureCoords[0], TextureCoords[2]);
        
        OpenTkuiHelper.DrawComponentWindow(
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

            if (OpenTkuiHelper.DrawProperty("Coord " + i, ref temp))
            {
                _unsaved = true;
            }
            
            TextureCoords[i] = temp;
        }
    }

    public void Save()
    {
        string? path = TexturePath.Remove(TexturePath.Length - 4);
        path += ".sprite";
        ResourceManager.SaveSprite(path, this, null, true);
        _unsaved = false;
    }
}