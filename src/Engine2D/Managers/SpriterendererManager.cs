using Engine2D.GameObjects;
using Engine2D.Rendering;
using Engine2D.SavingLoading;

namespace Engine2D.Managers;

public static class SpriterendererManager
{
    private static Dictionary<string, List<SpriteRenderer>> s_sprites = new Dictionary<string, List<SpriteRenderer>>();

    public static void AddSpriteRenderer(string sprite, SpriteRenderer spriteRenderer)
    {
        
        if (s_sprites.ContainsKey(sprite))
        {
            List<SpriteRenderer> tempList = s_sprites[sprite];
            if (tempList.Contains(spriteRenderer)) return;
            
            tempList.Add(spriteRenderer);
            s_sprites[sprite] = tempList;
        }
        else
        {
            var list = new List<SpriteRenderer>();
            list.Add((spriteRenderer));
            s_sprites.Add(sprite, list);
        }
    }

    public static List<SpriteRenderer> GetSpriteRenderers(string sprite)
    {
        return s_sprites[sprite];
    }

    public static void UpdateSpriteRenderers(string sprite)
    {
        if (!s_sprites.ContainsKey(sprite))
        {
            s_sprites.Add(sprite, new List<SpriteRenderer>());
        }
        
        foreach (var spriteRenderer in GetSpriteRenderers(sprite))
        {
            spriteRenderer.SetSprite(SaveLoad.LoadSpriteFromJson(sprite));
        }
    }
}