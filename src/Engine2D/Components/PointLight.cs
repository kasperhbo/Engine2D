using System.Numerics;
using Engine2D.Core;
using Engine2D.Flags;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Newtonsoft.Json;

namespace Engine2D.Components;

public class LightColor
{
    public float R;
    public float G;
    public float B;

    public LightColor()
    {
        this.R = 1;
        this.G = 1;
        this.B = 1;
    }
    
    public LightColor(float r, float g, float b)
    {
        this.R = r;
        this.G = g;
        this.B = b;
    }
}

[JsonConverter(typeof(ComponentSerializer))]
public class PointLight : Component
{
<<<<<<< HEAD
    
    public LightColor Color = new();
    public float Intensity = 1;

    [JsonIgnore] public Transform LastTransform { get; } = new();

    public override void Init(GameObject parent)
=======
    [ShowUI(show = false)] private SpriteColor _lastColor = new();
    public SpriteColor Color = new();
    public float Intensity = 100;
    
    [JsonIgnore]public Transform LastTransform { get; private set; } = new();
    
    public override void Init(Gameobject parent)
>>>>>>> parent of efcdaf4... AUTO REFACTORIO
    {
        base.Init(parent);
        Renderer.AddPointLight(this);
    }

    public override void Start()
    {
    }

    public override void EditorUpdate(double dt)
    {
        //Console.WriteLine(this.texture?.TexID);
<<<<<<< HEAD
        if (!LastTransform.Equals(Parent.Transform)) Parent.Transform.Copy(LastTransform);
=======
        if (!LastTransform.Equals(Parent.transform))
        {
            Parent.transform.Copy(LastTransform);
        }

        if (!_lastColor.Color.Equals(Color.Color))
        {
            _lastColor = new SpriteColor(Color.Color);
        }
>>>>>>> parent of efcdaf4... AUTO REFACTORIO
    }

    public override void GameUpdate(double dt)
    {
<<<<<<< HEAD
        if (!LastTransform.Equals(Parent.Transform)) Parent.Transform.Copy(LastTransform);
=======
        if (!LastTransform.Equals(Parent.transform))
        {
            Parent.transform.Copy(LastTransform);
        }

        if (!_lastColor.Color.Equals(Color.Color))
        {
            _lastColor = new SpriteColor(Color.Color);
        }
>>>>>>> parent of efcdaf4... AUTO REFACTORIO
    }

    public override string GetItemType()
    {
        return "PointLight";
    }
}
