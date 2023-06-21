#region

using System.Numerics;
using Engine2D.Core;
using Engine2D.Managers;
using Engine2D.Rendering;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;

#endregion

namespace Engine2D.Components.Sprites;

internal class Sprite : AssetBrowserAsset
{
    private bool _unsaved;
    private string Type = "Engine2D.Components.Sprites.Sprite";
    // 0.5f,   0.5f, 0.0f,    1.0f, 1.0f,   // top right
    // 0.5f,  -0.5f, 0.0f,    1.0f, 0.0f,   // bottom right
    // -0.5f, -0.5f, 0.0f,    0.0f, 0.0f,   // bottom left
    // -0.5f,  0.5f, 0.0f,    0.0f, 1.0f    // top left 

    [JsonIgnore] internal Vector2[] TextureCoords =
    {
        new(1, 1),
        new(1, 0),
        new(0, 0),
        new(0, 1f)
    };

    [JsonProperty]internal string? TexturePath = "";
    

    internal Sprite(string? texturePath)
    {
        TexturePath = texturePath;

        if (texturePath == "")
            Texture = new Texture(Utils.GetBaseEngineDir() + "\\Images\\Icons\\not-found-icon.png", "",
                false, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
        else
            Texture = ResourceManager.LoadTextureFromJson(texturePath);

        AssetName = FullSavePath;
    }


    [JsonConstructor]
    internal Sprite(string type, string? texturePath, string? assetName, string? fullPath)
    {
        TexturePath = texturePath;
        AssetName = assetName;
        FullSavePath = fullPath;

        if (TexturePath != "")
            Texture = ResourceManager.LoadTextureFromJson(TexturePath);
        else
            Texture = IconManager.GetIcon("not-found-icon");

        AssetName = FullSavePath;
    }

    [JsonIgnore] internal Texture? Texture { get; init; }
    internal string? FullSavePath { get; set; }
    internal int Width { get; set; }
    internal int Height { get; set; }


    internal override void OnGui()
    {
        if (_unsaved) ImGui.Begin("Sprite editor", ImGuiWindowFlags.UnsavedDocument);
        else ImGui.Begin("Sprite editor");

        if (ImGui.Button("Save")) Save();
        ;

        var w = Width;
        var h = Height;

        if (h > 128)
        {
            var dif = Texture.Height - 128;
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
        for (var i = 0; i < TextureCoords.Length; i++)
        {
            var temp = TextureCoords[i];

            if (OpenTkuiHelper.DrawProperty("Coord " + i, ref temp)) _unsaved = true;

            TextureCoords[i] = temp;
        }
    }

    internal void Save()
    {
        var path = TexturePath.Remove(TexturePath.Length - 4);
        path += ".sprite";
        ResourceManager.SaveSprite(path, this, null, true);
        _unsaved = false;
    }
}