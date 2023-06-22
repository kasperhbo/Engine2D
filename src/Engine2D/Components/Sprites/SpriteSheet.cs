#region

using System.Numerics;
using System.Runtime.InteropServices;
using Engine2D.Core;
using Engine2D.GameObjects;
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
    [JsonIgnore] private GCHandle? _currentlyDraggedHandle;
    [JsonIgnore] public Texture Texture => ResourceManager.GetItem<Texture>(_texturePath);
    [JsonIgnore] private bool _unsaved;
    
    [JsonProperty] internal List<Sprite> Sprites = new();


    [JsonProperty] private string? _texturePath = "";
    [JsonProperty] public string? SavePath = "";
    [JsonProperty] private int _numSprites;
    [JsonProperty] private int _spacing;
    [JsonProperty] private int _spriteHeight;
    [JsonProperty] private int _spriteWidth;

    internal SpriteSheet(string? texturePath, string? savePath)
    {
        Init(texturePath, savePath, -1, -1, 1, 0);
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
        
        _spacing = spacing;
        SavePath = savePath;
        _numSprites = numSprites;

        Sprites = new();
        _texturePath = texturePath;

        if (spriteHeight == -1 && spriteWidth == -1)
        {
            spriteHeight = Texture.Height;
            spriteWidth = Texture.Width;
        }
        
        this._spriteHeight = spriteHeight;
        this._spriteWidth = spriteWidth;
        
        if (!File.Exists(_texturePath))
        {
            Log.Error(_texturePath + " Texture doesnt exsist");
            return;
        }
        
        if (Texture == null) Log.Error("No texture");


        int currentX = 0;
        int currentY = Texture.Height - spriteHeight;
        
        for (var i = 0; i < numSprites; i++)
        {
            float topY = (currentY + spriteHeight) / (float)Texture.Height;
            float rightX = (currentX + spriteWidth) / (float)Texture.Width;
            float leftX = currentX / (float)Texture.Width;
            float bottomY = currentY / (float)Texture.Height;
            
            Vector2[] texCoords =
            {
                new Vector2(rightX, topY),
                new Vector2(rightX, bottomY),
                new Vector2(leftX, bottomY),
                new Vector2(leftX, topY)
            };
             
            var sprite = new Sprite(savePath, texCoords, spriteWidth, spriteHeight, i);

            Sprites.Add(sprite);

            currentX += spriteWidth + spacing;
            if (currentX >= Texture.Width) {
                currentX = 0;
                currentY -= spriteHeight + spacing;
            }
        }
    }

    internal unsafe override void OnGui()
    {
        if (_unsaved)
            ImGui.Begin("Sprite sheet inspector", ImGuiWindowFlags.UnsavedDocument);
        else
            ImGui.Begin("Sprite sheet inspector");

        var columnCount = (int)(ImGui.GetContentRegionAvail().X / (90 + 15));

        if (ImGui.Button("Save")) Save();

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 10));
        ImGui.Text(SavePath);

        if (ImGui.InputInt("sprite width", ref _spriteWidth) ||
            ImGui.InputInt("sprite height", ref _spriteHeight) ||
            ImGui.InputInt("num of sprites", ref _numSprites) ||
            ImGui.InputInt("padding", ref _spacing))
        {
            Init(_texturePath, SavePath, _spriteWidth, _spriteHeight, _numSprites, _spacing, true);
            Engine.Get().CurrentSelectedSpriteSheetAssetBrowserAsset = this;            
            _unsaved = true;
        }

        ;

        ImGui.Columns(columnCount < 1 ? 1 : columnCount, "", false);
        {
            if (Texture == null)
            {
                if (_texturePath == "")
                {
                    Log.Error("Texture path not set");
                    return;
                }
            }

            for (var i = 0; i < Sprites.Count; i++)
            {
                _currentlyDraggedHandle ??= GCHandle.Alloc(Sprites[i]);
                var entry = Sprites[i];
                ImGui.PushID(entry.TextureCoords.ToString());
                
                bool clicked;
                bool doubleClicked;
                bool rightClicked;
                var coord = entry.TextureCoords;

                var _isSelected = false;
                
                
                Gui.ImageButtonExTextDown(
                    i.ToString(),
                    ESupportedFileTypes.sprite,
                    Texture.TexID,
                    new Vector2(90),
                    coord[3], coord[1],
                    new Vector2(-1, -2), new Vector2(0, 11),
                    new Vector4(1),
                    out clicked, out doubleClicked, out rightClicked, _isSelected, false);

                if (ImGui.BeginDragDropSource())
                {
                    byte[] spriteData = SpriteRenderer.SerializeSprite(entry);
                
                    // Set payload data
                    GCHandle spriteHandle = GCHandle.Alloc(spriteData, GCHandleType.Pinned);
                    IntPtr spritePtr = spriteHandle.AddrOfPinnedObject();
                    ImGui.SetDragDropPayload("spritesheet_drop", spritePtr, (uint)spriteData.Length);
                
                    ImGui.EndDragDropSource();
                
                    spriteHandle.Free();
                }

                ImGui.PopID();
                

                ImGui.NextColumn();
            }
        }
        ImGui.Columns(1);
        ImGui.PopStyleVar();


        ImGui.End();
    }

    internal void Save()
    {
        if (SavePath == "")
        {
            var path = _texturePath.Remove(_texturePath.Length - 4);
            path += ".spritesheet";
            SavePath = path;
        }

        Init(_texturePath, SavePath, _spriteWidth, _spriteHeight, _numSprites, _spacing);
        AssetName = SavePath;
        ResourceManager.SaveSpriteSheet(SavePath, this, null, true);
        
        _unsaved = false;
    }
}