#region

using Engine2D.Components.TransformComponents;
using Engine2D.Flags;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Newtonsoft.Json;

#endregion

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
internal class PointLightComponent : Component
{
    [JsonProperty]internal KDBColor Color = new();
    [JsonProperty]internal float Intensity = 1;
    
    [ShowUI(show = false)] private KDBColor _lastColor = new();
    [JsonIgnore] internal Transform? LastTransform;

    internal override void Init(Gameobject parent, Renderer? renderer)
    {
        LastTransform = new Transform();
        base.Init(parent, renderer);
        renderer.AddPointLight(this);
        LastTransform = parent.GetComponent<Transform>();
    }

    public override void Start()
    {
    }

    public override void EditorUpdate(double dt)
    {
        if (!LastTransform.Equals(Parent.GetComponent<Transform>()))
            Transform.Copy(Parent.GetComponent<Transform>(), LastTransform);

        if (!_lastColor.Equals(Color)) _lastColor = new KDBColor(Color);
    }


    public override void GameUpdate(double dt)
    {
    }

    public override string GetItemType()
    {
        return this.GetType().FullName;
    }
}