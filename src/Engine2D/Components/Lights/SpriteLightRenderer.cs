using Engine2D.Core;
using Engine2D.Flags;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace Engine2D.Components.Lights;

[JsonConverter(typeof(ComponentSerializer))]
public class SpriteLightRenderer : Component
{
    [ShowUI(show = false)] private SpriteColor _lastColor = new();

    [ShowUI(show = false)] private readonly Transform _lastTransform = new();
    public SpriteColor Color = new();

    [ShowUI(show = false)] internal bool IsDirty = true;

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
        if(_addToRendererAsSprite)
            GameRenderer.AddSpriteLightRenderer(this);
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
        return "SpriteLightRenderer";
    }
}