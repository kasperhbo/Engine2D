using System.Numerics;
using Engine2D.Rendering;
using Engine2D.SavingLoading;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.Components.Sprites;

public class SpriteSheet
{
    private List<Sprite> _sprites = new List<Sprite>();
    private string _texturePath = "";
    [JsonIgnore]private Texture _texture;

    public SpriteSheet(string texturePath, int spriteWidth, int spriteHeight, int numSprites, int spacing, string Label, DirectoryInfo directoryInfo)
    {
        _sprites = new();
        _texturePath = texturePath;

        Texture texture = SaveLoad.LoadTextureFromJson(texturePath);
        _texture = texture;

        int currentX = 0;
        int currentY = texture.Height - spriteHeight;

        for (int i = 0; i < numSprites; i++)
        {
            float topY = (currentY + spriteHeight) / (float)texture.Height;
            float rightX = (currentX + spriteWidth) / (float)texture.Width;
            float leftX = currentX / (float)texture.Width;
            float bottomY = currentY / (float)texture.Height;

            Vector2[] texCoords =
            {
                new(leftX, bottomY),
                new(rightX, bottomY),
                
                new(rightX, topY),
                new(leftX, topY)
            };

            var TextureCoords = (texCoords);
            var Width = (spriteWidth);
            var Height = (spriteHeight);

            Sprite sprite = new Sprite(texturePath);

            sprite.TextureCoords = TextureCoords;
            sprite.Width = Width;
            sprite.Height = Height;

            this._sprites.Add(sprite);

            currentX += spriteWidth + spacing;
            if (currentX >= texture.Height)
            {
                currentX = 0;
                currentY -= spriteHeight + spacing;
            }
            
            string saveName = Label.Remove(Label.Length - 4, 4);
            saveName += i + ".sprite";
            SaveLoad.SaveSprite(sprite, saveName,directoryInfo);
        }
    }
}