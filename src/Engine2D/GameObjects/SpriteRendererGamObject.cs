using Engine2D.Core;

namespace Engine2D.GameObjects;

public class SpriteRendererGamObject : GameObject
{
    private TextureData _textureData = null;
    
    public SpriteRendererGamObject(string name) : base(name)
    {
        
    }
    
    public SpriteRendererGamObject(string name, TextureData textureData) : base(name)
    {
        _textureData = textureData;
    }
    
    public override void Init()
    {
        SpriteRenderer spr = new SpriteRenderer();
        if (_textureData != null)
        {
            spr.textureData = _textureData;
        }
        this.components.Add(spr);
        
        base.Init();
    }
}