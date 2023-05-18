using System.Numerics;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.Flags;
using Engine2D.Rendering;
using Newtonsoft.Json;

namespace Engine2D.GameObjects;

public class SpriteColor
{
    public Vector4 Color;

    public SpriteColor()
    {
        Color = new Vector4();
    }

    public SpriteColor(Vector4 color)
    {
        Color = color;
    }

    public SpriteColor(float r, float g, float b, float a)
    {
        Color = new Vector4(r, g, b, a);
    }
}

[JsonConverter(typeof(ComponentSerializer))]
public class SpriteRenderer : Component
{
    [ShowUI(show = false)] private SpriteColor _lastColor = new();

    [ShowUI(show = false)] private readonly Transform _lastTransform = new();
    public SpriteColor Color = new();
    
    [ShowUI(show = false)] internal bool IsDirty = true;
    
    public int ZIndex = 0;
    [JsonIgnore][ShowUI(show = false)]int _prevZIndex = 0;
    
    [JsonIgnore] internal Texture texture;

    [JsonIgnore] protected bool _addToRendererAsSprite = true;
    
    internal Vector2[] TextureCoords =
    {
        new(1, 1),
        new(1, 0),
        new(0, 0),
        new(0, 1)
    };

    public TextureData? textureData;

    public override void Init(Gameobject parent)
    {
        base.Init(parent);
        if (textureData != null)
        {
            Console.WriteLine("has texture data, loading texture...." + textureData.texturePath);
            texture = ResourceManager.GetTexture(textureData);
        }
        if(_addToRendererAsSprite)throw new NotImplementedException();
            //TODO: ADD TO RENDERER
    }

    public override void Start()
    {
    }

    public override void EditorUpdate(double dt)
    {
        //Console.WriteLine(this.texture?.TexID);
        if (!_lastTransform.Equals(Parent.transform))
        {
            IsDirty = true;
            Parent.transform.Copy(_lastTransform);
        }

        if (!_lastColor.Color.Equals(Color.Color))
        {
            IsDirty = true;
            _lastColor = new SpriteColor(Color.Color);
        }

        if (!_prevZIndex.Equals(ZIndex))
        {
            _prevZIndex = ZIndex;
            //TODO: REMOVE FROM RENDERER
            // GameRenderer.RemoveSprite(this);
            // GameRenderer.AddSpriteRenderer(this);
            this.IsDirty = true;
        }
    }

    public override void GameUpdate(double dt)
    {
        if (!_lastTransform.Equals(Parent.transform))
        {
            IsDirty = true;
            Parent.transform.Copy(_lastTransform);
        }

        if (!_lastColor.Color.Equals(Color.Color))
        {
            IsDirty = true;
            _lastColor = new SpriteColor(Color.Color);
        }
    }

    public override string GetItemType()
    {
        return "SpriteRenderer";
    }
}