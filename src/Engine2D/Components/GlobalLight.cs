using Engine2D.GameObjects;
using KDBEngine.Core;
using Newtonsoft.Json;

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
public class GlobalLight : Component
{
    public float Intensity = 1;

    public override void Init(Gameobject parent)
    {
        base.Init(parent);
        Engine.Get()._currentScene.GlobalLight = this;
    }

    public override void Start()
    {
    }

    public override string GetItemType()
    {
        return "GlobalLight";
    }
}