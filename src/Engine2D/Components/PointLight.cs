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
    
    public LightColor Color = new();
    public float Intensity = 1;

    [JsonIgnore] public Transform LastTransform { get; } = new();

    public override void Init(GameObject parent)
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
        if (!LastTransform.Equals(Parent.transform)) Parent.transform.Copy(LastTransform);
    }

    public override void GameUpdate(double dt)
    {
        if (!LastTransform.Equals(Parent.transform)) Parent.transform.Copy(LastTransform);
    }

    public override string GetItemType()
    {
        return "PointLight";
    }
}