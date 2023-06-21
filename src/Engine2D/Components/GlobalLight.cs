#region

using Engine2D.GameObjects;
using Engine2D.Rendering;
using Newtonsoft.Json;

#endregion

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
internal class GlobalLight : Component
{
    [JsonProperty]internal float Intensity = 1;

    internal override void Init(Gameobject parent, Renderer? renderer)
    {
        base.Init(parent, renderer);
        renderer.GlobalLight = this;
    }

    public override void Start()
    {
    }

    public override string GetItemType()
    {
        return this.GetType().FullName;
    }
}