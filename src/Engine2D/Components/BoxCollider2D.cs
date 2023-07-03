#region

using System.Numerics;
using Engine2D.Rendering;
using Newtonsoft.Json;
using OpenTK.Windowing.Common;
using Vortice.Mathematics;

#endregion

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
internal class BoxCollider2D : Component
{
    [JsonProperty]internal float   Density              = 1.0f;
    [JsonProperty]internal float   Friction             = 0.5f;
    [JsonProperty]internal Vector2 Offset               = new();
    [JsonProperty]internal float   Restitution          = 0.0f;
    [JsonProperty]internal float   RestitutionThreshold = 0.5f;

    //internal override void ImGuiFields()
    //{
    //}

    public override void StartPlay()
    {
        
    }

    public override string GetItemType()
    {
        return this.GetType().FullName;
    }

    public override void Init()
    {
        
    }

    public override void Update(FrameEventArgs args)
    {
        
    }

    public override void EditorUpdate(double dt)
    {
        base.EditorUpdate(dt);
        DebugDraw.AddBox2D(new(Parent.Transform.Position.X, Parent.Transform.Position.Y), 
            new OpenTK.Mathematics.Vector2(Parent.Transform.GetFullSize(true).X, Parent.Transform.GetFullSize(true).Y), 0);
    }

    public override BoxCollider2D Clone()
    {
        BoxCollider2D clone = new();
        clone.Density = Density;
        clone.Friction = Friction;
        clone.Offset = Offset;
        clone.Restitution = Restitution;
        clone.RestitutionThreshold = RestitutionThreshold;
        return clone;
    }
}