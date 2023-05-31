using Engine2D.Components.TransformComponents;
using Engine2D.Flags;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Newtonsoft.Json;

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
public class PointLightComponent : Component
{
    [ShowUI(show = false)] private KDBColor _lastColor = new();
    public KDBColor Color = new();
    public float Intensity = 1;

    [JsonIgnore] public Transform LastTransform;

    public override void Init(Gameobject parent, Renderer? renderer)
    {
        LastTransform = new Transform();
        base.Init(parent, renderer);
        renderer.AddPointLight(this);
        LastTransform = parent.Transform;
    }

    public override void Start()
    {
    }

    public override void EditorUpdate(double dt)
    {
        //Console.WriteLine(this.texture?.TexID);
        if (!LastTransform.Equals(Parent.Transform))Parent.Transform.Copy(this.LastTransform);

        if (!_lastColor.Equals(Color)) _lastColor = new KDBColor(Color);
    }

    public override void GameUpdate(double dt)
    {
    }

    public override string GetItemType()
    {
        return "PointLight";
    }
}