using Engine2D.Flags;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Newtonsoft.Json;

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
public class PointLight : Component
{
    [ShowUI(show = false)] private SpriteColor _lastColor = new();
    public SpriteColor Color = new();
    public float Intensity = 1;

    [JsonIgnore] public Transform LastTransform { get; } = new();

    public override void Init(Gameobject parent, Renderer renderer)
    {
        base.Init(parent, renderer);
        renderer.AddPointLight(this);
    }

    public override void Start()
    {
    }

    public override void EditorUpdate(double dt)
    {
        //Console.WriteLine(this.texture?.TexID);
        if (!LastTransform.Equals(Parent.transform)) Parent.transform.Copy(LastTransform);

        if (!_lastColor.Color.Equals(Color.Color)) _lastColor = new SpriteColor(Color.Color);
    }

    public override void GameUpdate(double dt)
    {
        if (!LastTransform.Equals(Parent.transform)) Parent.transform.Copy(LastTransform);

        if (!_lastColor.Color.Equals(Color.Color)) _lastColor = new SpriteColor(Color.Color);
    }

    public override string GetItemType()
    {
        return "PointLight";
    }
}