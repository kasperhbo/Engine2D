#region

using System.Numerics;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Managers;
using Engine2D.Rendering;
using Engine2D.UI.Browsers;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;
using Newtonsoft.Json;

#endregion

namespace Engine2D.Components.Sprites;

internal class SpriteSheet : AssetBrowserAsset
{
    [JsonProperty] private List<SpriteSheetSprite> _sprites = new();


    [JsonIgnore] private Texture _texture;
    [JsonProperty] private string? _texturePath = "";
    [JsonIgnore] private bool _unsaved;
    [JsonProperty] private int numSprites;
    [JsonProperty] private string? savePath = "";
    [JsonProperty] private int spacing;
    [JsonProperty] private int spriteHeight;
    [JsonProperty] private int spriteWidth;

    internal SpriteSheet(string? texturePath, int spriteWidth, int spriteHeight, int numSprites,
        int spacing, bool unsaved = false)
    {
        Init(texturePath, "", spriteWidth, spriteHeight, numSprites, spacing, unsaved);
    }

    [JsonConstructor]
    internal SpriteSheet(string? texturePath, string? savePath, int spriteWidth, int spriteHeight, int numSprites,
        int spacing)
    {
        Init(texturePath, savePath, spriteWidth, spriteHeight, numSprites,
            spacing);
    }

    private void Init(string? texturePath, string? savePath, int spriteWidth, int spriteHeight, int numSprites,
        int spacing, bool unsaved = false)
    {
        _unsaved = unsaved;
        this.spacing = spacing;
        this.spriteHeight = spriteHeight;
        this.spriteWidth = spriteWidth;
        this.numSprites = numSprites;
        this.savePath = savePath;

        _sprites = new List<SpriteSheetSprite>();
        _texturePath = texturePath;

        if (!File.Exists(_texturePath))
        {
            Log.Error(_texturePath + " Texture doesnt exsist");
            return;
        }

        _texture = ResourceManager.LoadTextureFromJson(texturePath);
        if (_texture == null) Log.Error("No texture");


        var currentX = 0;
        var currentY = _texture.Height - spriteHeight;

        for (var i = 0; i < numSprites; i++)
        {
            var topY = (currentY + spriteHeight) / (float)_texture.Height;
            var rightX = (currentX + spriteWidth) / (float)_texture.Width;
            var leftX = currentX / (float)_texture.Width;
            var bottomY = currentY / (float)_texture.Height;

            Vector2[] texCoords =
            {
                new(rightX, topY),
                new(rightX, bottomY),

                new(leftX, bottomY),
                new(leftX, topY)
            };

            var TextureCoords = texCoords;
            var Width = spriteWidth;
            var Height = spriteHeight;

            var sprite = new SpriteSheetSprite();

            sprite.TextureCoords = TextureCoords;
            sprite.Width = Width;
            sprite.Height = Height;

            _sprites.Add(sprite);

            currentX += spriteWidth + spacing;
            if (currentX >= _texture.Height)
            {
                currentX = 0;
                currentY -= spriteHeight + spacing;
            }
        }
    }

    internal override void OnGui()
    {
        if (_unsaved)
            ImGui.Begin("Sprite sheet inspector", ImGuiWindowFlags.UnsavedDocument);
        else
            ImGui.Begin("Sprite sheet inspector");

        var columnCount = (int)(ImGui.GetContentRegionAvail().X / (90 + 15));

        if (ImGui.Button("Save")) Save();

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 10));
        ImGui.Text(savePath);

        if (ImGui.InputInt("sprite width", ref spriteWidth) ||
            ImGui.InputInt("sprite height", ref spriteHeight) ||
            ImGui.InputInt("num of sprites", ref numSprites) ||
            ImGui.InputInt("padding", ref spacing))
        {
            Init(_texturePath, savePath, spriteWidth, spriteHeight, numSprites, spacing, true);

            Engine.Get().CurrentSelectedAssetBrowserAsset = this;
            _unsaved = true;
        }

        ;

        ImGui.Columns(columnCount < 1 ? 1 : columnCount, "", false);
        {
            if (_texture == null)
            {
                if (_texturePath == "")
                {
                    Log.Error("Texture path not set");
                    return;
                }

                _texture = ResourceManager.LoadTextureFromJson(_texturePath);
            }

            for (var i = 0; i < _sprites.Count; i++)
            {
                var entry = _sprites[i];
                bool clicked;
                bool doubleClicked;
                bool rightClicked;
                var coord = entry.TextureCoords;

                var _isSelected = false;
                Gui.ImageButtonExTextDown(
                    i.ToString(),
                    ESupportedFileTypes.sprite,
                    _texture.TexID,
                    new Vector2(90),
                    coord[3], coord[1],
                    new Vector2(-1, -2), new Vector2(0, 11),
                    new Vector4(1),
                    out clicked, out doubleClicked, out rightClicked, _isSelected, false);


                ImGui.NextColumn();
            }
        }
        ImGui.Columns(1);
        ImGui.PopStyleVar();


        ImGui.End();
    }

    internal void Save()
    {
        if (savePath == "")
        {
            var path = _texturePath.Remove(_texturePath.Length - 4);
            path += ".spritesheet";
            savePath = path;
        }

        Init(_texturePath, savePath, spriteWidth, spriteHeight, numSprites, spacing);
        AssetName = savePath;
        ResourceManager.SaveSpriteSheet(savePath, this, null, true);
        _unsaved = false;
    }
}