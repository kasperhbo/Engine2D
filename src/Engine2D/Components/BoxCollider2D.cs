using System.Numerics;
using Newtonsoft.Json;

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
public class BoxCollider2D : Component
{
    public float Density = 1.0f;
    public float Friction = 0.5f;
    public Vector2 HalfSize = new(0.5f, 0.5f);
    public Vector2 Offset = new();
    public float Restitution = 0.0f;
    public float RestitutionThreshold = 0.5f;

    //public override void ImGuiFields()
    //{
    //}

    public override string GetItemType()
    {
        return "BoxCollider2D";
    }
}