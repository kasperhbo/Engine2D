using Engine2D.Rendering;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.Components.Sprites;

public class SpriteSheet
{
    private List<Sprite> _sprites = new List<Sprite>();

    //
    // public SpriteSheet(string texturePath, int numOfSpriteHor, int numOfSpritesVert)
    // {
    //     ProjectSettings.FullProjectPath + "\\Images\\" +
    //         "pixelcrawler\\weapons\\hands\\Hands.png";
    // }
    //
    // public Sprite GetSprite(int spriteIndex)
    // {
    //     return _sprites[spriteIndex];
    // }
    //
    // private Texture LoadTexture(string filePath)
    // {
    //     Texture Texture = new Texture(
    //         filePath, false, TextureMinFilter.Nearest, TextureMagFilter.Nearest
    //     );
    //     return Texture;
    // }
}