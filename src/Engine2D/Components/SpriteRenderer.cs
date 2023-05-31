using System.Numerics;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.Flags;
using Engine2D.Rendering;
using Newtonsoft.Json;
using Engine2D.Components.TransformComponents;

namespace Engine2D.GameObjects;

public class KDBColor
{
    public float r;
    public float g;
    public float b;
    public float a;

    public float R
    {
        get { return r / 255; }
    }
    
    public float G
    {
        get { return g / 255; }
    }
    
    public float B
    {
        get { return b / 255; }
    }
    
    public float A
    {
        get { return a / 255; }
    }
    
    public KDBColor()
    {
        this.r = 255;
        this.g = 255;
        this.b = 255;
        this.a = 255;
    }
    
    public KDBColor(KDBColor other)
    {
        this.r = other.r;
        this.g = other.g;
        this.b = other.b;
        this.a = other.a;
    }

    public KDBColor(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
}

[JsonConverter(typeof(ComponentSerializer))]
public class SpriteRenderer : Component
{
    [ShowUI(show = false)] private Transform _lastTransform = new Transform();
    [ShowUI(show = false)] private KDBColor _lastColor = new();
    [JsonIgnore] [ShowUI(show = false)] private int _prevZIndex;

    [JsonIgnore] public bool AddToRendererAsSprite = true;
    public KDBColor Color = new();

    [ShowUI(show = false)] internal bool IsDirty = true;

    [JsonIgnore] internal Texture texture;

    internal Vector2[] TextureCoords =
    {
        new(1, 1),
        new(1, 0),
        new(0, 0),
        new(0, 1)
    };

    public TextureData? textureData;

    public int ZIndex = 0;

    public override void Init(Gameobject parent, Renderer? renderer)
    {
        base.Init(parent, renderer);
       
        if (textureData != null)
        {
            texture = ResourceManager.GetTexture(textureData);
        }
        renderer.AddSpriteRenderer(this);
    }

    public override void Start()
    {
    }

    public override void EditorUpdate(double dt)
    {
        //Console.WriteLine(this.texture?.TexID);
        if (_lastTransform.Equals(Parent.Transform)
            );
        {
            IsDirty = true;
            Parent.Transform.Copy(_lastTransform);
        }

        if (!_lastColor.Equals(Color))
        {
            IsDirty = true;
            _lastColor = new KDBColor(Color.R, Color.G, Color.B, Color.A);
        }

        if (!_prevZIndex.Equals(ZIndex))
        {
            _prevZIndex = ZIndex;
            throw new NotImplementedException();
            IsDirty = true;
        }
    }

    public override void GameUpdate(double dt)
    {
    }

    public override string GetItemType()
    {
        return "SpriteRenderer";
    }
}