#region

using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Newtonsoft.Json;
using OpenTK.Windowing.Common;

#endregion

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
internal class GlobalLight : Component
{
    [JsonProperty]internal float Intensity = 1;

    public override void Init()
    {
        var currentSceneRenderer = Engine.Get().CurrentScene.Renderer;
       // if (currentSceneRenderer != null) currentSceneRenderer.GlobalLight = this;
    }

    public override void Start()
    {
    }

    public override void Update(FrameEventArgs args)
    {
        
    }

    public override void StartPlay()
    {
        
    }

    public override string GetItemType()
    {
        return this.GetType().FullName;
    }

    public override object Clone()
    {
        throw new NotImplementedException();
        return base.Clone();
    }
}