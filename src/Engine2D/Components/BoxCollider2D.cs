#region

using System.Numerics;
using Newtonsoft.Json;

#endregion

namespace Engine2D.Components;

[JsonConverter(typeof(ComponentSerializer))]
internal class BoxCollider2D : Component
{
    [JsonProperty]internal float Density = 1.0f;
    [JsonProperty]internal float Friction = 0.5f;
    [JsonProperty]internal Vector2 HalfSize = new(0.5f, 0.5f);
    [JsonProperty]internal Vector2 Offset = new();
    [JsonProperty]internal float Restitution = 0.0f;
    [JsonProperty]internal float RestitutionThreshold = 0.5f;

    //internal override void ImGuiFields()
    //{
    //}

    public override string GetItemType()
    {
        return this.GetType().FullName;
    }
}