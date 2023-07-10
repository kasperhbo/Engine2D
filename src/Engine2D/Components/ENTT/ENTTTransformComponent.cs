using System.Numerics;

namespace Engine2D.Components;

public struct ENTTTransformComponent
{
    Matrix4x4 Transform = Matrix4x4.Identity;
    
    Vector3 Position = Vector3.Zero;
    Vector3 Rotation = Vector3.Zero;
    Vector3 Scale = Vector3.One;
    
    
    public ENTTTransformComponent()
    {
    }
}