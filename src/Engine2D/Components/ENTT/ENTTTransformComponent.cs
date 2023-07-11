using System.Numerics;
using Engine2D.UI.ImGuiExtension;
using EnTTSharp.Entities;

namespace Engine2D.Components.ENTT;

public struct ENTTTransformComponent : IENTTComponent
{
    public Entity Parent;

    Matrix4x4 Transform = Matrix4x4.Identity;
    
    public Vector2 Position = Vector2.Zero;
    public Vector2 Rotation = Vector2.Zero;
    public Vector2 Scale = Vector2.One;
    
    
    public ENTTTransformComponent(Entity parent)
    {
        this.Parent = parent;
    }
}