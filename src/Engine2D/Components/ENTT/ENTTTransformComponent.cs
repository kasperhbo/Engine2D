using System.Drawing.Drawing2D;
using System.Numerics;
using Engine2D.UI.ImGuiExtension;
using EnTTSharp.Entities;
using Newtonsoft.Json;

namespace Engine2D.Components.ENTT;

public struct ENTTTransformComponent : IENTTComponent
{
    internal Matrix4x4 Transform(ENTTSpriteRenderer? enttSpriteRenderer = null)
    {
       if(enttSpriteRenderer != null)
       {
           return Matrix4x4.CreateScale(Scale.X * enttSpriteRenderer.Value.Sprite.Texture.Width,
                      Scale.Y * enttSpriteRenderer.Value.Sprite.Texture.Height, 1) *
                  Matrix4x4.CreateFromQuaternion(Rotation) *
                  Matrix4x4.CreateTranslation(Position.X, Position.Y, 0);
       }
       else
       {
           return Matrix4x4.CreateScale(Scale.X,
                      Scale.Y, 1) *
                  Matrix4x4.CreateFromQuaternion(Rotation) *
                  Matrix4x4.CreateTranslation(Position.X, Position.Y, 0);
       }
    }
    
    
    [JsonProperty]public Vector2    Position = Vector2.Zero;
    [JsonProperty]public Quaternion Rotation = new Quaternion();
    [JsonProperty]public Vector2    Scale = Vector2.One;
    
    
    public ENTTTransformComponent()
    {
    }
}