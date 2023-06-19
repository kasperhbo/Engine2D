using Engine2D.GameObjects;

namespace Engine2D.Utilities;

public static class ArrayUtils
{
    public static List<SpriteRenderer> ShiftSpriteRenderers(List<SpriteRenderer> spriteRenderers, SpriteRenderer toRemove)
    {
        SpriteRenderer[] spriteRenderersAr = spriteRenderers.ToArray();
        int count = spriteRenderers.Count();
        
        for (var i = 0; i < count; i++)
            if (spriteRenderersAr[i] == toRemove)
            {
                for (var j = i; j < count - 1; j++)
                {
                    spriteRenderersAr[j] = spriteRenderersAr[j + 1];
                    spriteRenderersAr[j].IsDirty = true;
                }

                count--;
            }

        return spriteRenderersAr.ToList();
    }
}
