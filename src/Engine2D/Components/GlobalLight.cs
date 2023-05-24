using Engine2D.Flags;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using KDBEngine.Core;
using Newtonsoft.Json;


namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
public class GlobalLight : Component
{
    public float Intensity = 1;
<<<<<<< HEAD

    public override void Init(GameObject parent)
=======
    
    public override void Init(Gameobject parent)
>>>>>>> parent of efcdaf4... AUTO REFACTORIO
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