using System.Numerics;
using Engine2D.Components.Sprites;
using Engine2D.Core;
using Engine2D.Managers;
using Engine2D.Rendering;
using Newtonsoft.Json;
using Octokit;

internal class SpriteSheet : AssetBrowserAsset
{
    [JsonProperty] internal List<Sprite> _sprites = new();
    [JsonProperty]private string _savePath = "";
    [JsonProperty]private string? _texturePath = "";
    
    [JsonProperty]private int _spriteWidth = 0;
    [JsonProperty]private int _spriteHeight = 0;
    [JsonProperty] private int _spacing = 0;
    [JsonProperty]private int _spriteCount = 0;
    
    [JsonIgnore]private Texture _texture = null;
    
    //     [JsonProperty] private string? _texturePath = "";
//     [JsonProperty] private string? _savePath = "";
//     [JsonProperty] private int _numSprites;
//     [JsonProperty] private int _spriteHeight;
//     [JsonProperty] private int _spriteWidth;
    
    
    internal SpriteSheet(
        string savePath,
        int spriteWidth, int spriteHeight,
        int spacing,int spriteCount,
        string texturePath = "\\assets\\spritesheet.tex"
        )
    {
        _savePath = savePath;
        _texturePath = texturePath;
        _spriteWidth = spriteWidth;
        _spriteHeight = spriteHeight;
        _spacing = spacing;
        _spriteCount = spriteCount;

        SetTexture();
        CreateSprites();
    }

    private void SetTexture()
    {
        if (_texturePath != "" && _texturePath != null)
        {
            _texture = ResourceManager.GetItem<Texture>(_texturePath);
        }
    }

    private void CreateSprites()
    {
        if (_texture == null) return;
        _sprites = new();

        if (_spriteHeight == -1)
        {
            _spriteHeight = _texture.Height;
        }
        
        if (_spriteWidth == -1)
        {
            _spriteWidth = _texture.Width;
        }
        
        int currentX = 0;
        int currentY = _texture.Height - _spriteHeight;

        for (int i = 0; i < _spriteCount; i++)
        {
            float rightX = (currentX + _spriteWidth) / (float)_texture.Width;
            float topY = (currentY + _spriteHeight) / (float)_texture.Height;
            
            float leftX = currentX / (float)_texture.Width;
            float bottomY = currentY / (float)_texture.Height;
            Vector2[] texCoords =
            {
                new Vector2(leftX, bottomY),
                new Vector2(rightX, bottomY),
                new Vector2(rightX, topY),
                new Vector2(leftX, topY)
            };
            
            currentX += _spriteWidth + _spacing;
            if (currentX >= _texture.Width)
            {
                currentX = 0;
                currentY -= _spriteHeight + _spacing;
            }
            
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_savePath);  
            var spriteSaveName = fileNameWithoutExtension + "\\" + i + ".sprite";

            var sprite = new Sprite(
                spriteSaveName, _texturePath, _spriteWidth, _spriteHeight, texCoords);
            _sprites.Add(sprite);
            //ResourceManager.SaveSprite(spriteSaveName, sprite);
        }
    }


    internal override void OnGui()
    {
        
    }

    internal override void Refresh()
    {
        
    }
}