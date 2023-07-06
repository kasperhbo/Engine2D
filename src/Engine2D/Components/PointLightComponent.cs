#region

using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.Flags;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

#endregion

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
internal class PointLightComponent : Component
{
    [JsonProperty]internal Vector4 Color = new();
    [JsonProperty]internal float Intensity = 1;
    
    [ShowUI(show = false)] private Vector4 _lastColor = new();
    [JsonIgnore] internal Transform? LastTransform;

    public override void Init()
    {
        //     LastTransform = new Transform();
        //     Engine.Get().CurrentScene.Renderer?.AddPointLight(this);
        // }
    }

    public override void Start()
    {
    }

    public override void Update(FrameEventArgs args)
    {
        if (!LastTransform.Equals(Parent.GetComponent<Transform>()))
                    Transform.Copy(Parent.GetComponent<Transform>(), LastTransform);

        if (!_lastColor.Equals(Color)) _lastColor = Color;
    }

    public override void EditorUpdate(double dt)
    {
        
    }


    public override void GameUpdate(double dt)
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