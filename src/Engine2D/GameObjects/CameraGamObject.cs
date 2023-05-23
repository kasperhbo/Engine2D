using Engine2D.Core;
using Engine2D.Testing;

namespace Engine2D.GameObjects;

public class CameraGameObject : GameObject
{
    private TextureData _textureData = null;
    
    public CameraGameObject(string name) : base(name)
    {
        this.components.Add(new TestCamera(this.transform.position));
    }
    
    public override void Init()
    {
        
        base.Init();
    }
}