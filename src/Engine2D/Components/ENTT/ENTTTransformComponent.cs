using System.Numerics;
using Engine2D.UI.ImGuiExtension;
using EnTTSharp.Entities;
using Newtonsoft.Json;

namespace Engine2D.Components.ENTT;

public struct ENTTTransformComponent : IENTTComponent
{
    Matrix4x4 Transform = Matrix4x4.Identity;
    
    [JsonProperty]public Vector2 Position = Vector2.Zero;
    [JsonProperty]public Vector2 Rotation = Vector2.Zero;
    [JsonProperty]public Vector2 Scale = Vector2.One;
    
    
    public ENTTTransformComponent()
    {
    }
}