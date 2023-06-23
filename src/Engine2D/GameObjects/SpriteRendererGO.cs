#region

using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.UI.ImGuiExtension;
using ImGuiNET;

#endregion

namespace Engine2D.GameObjects;

internal class SpriteRendererGo : Gameobject
{
    internal SpriteRendererGo(string name) : base(name)
    {
        var currentScene = Engine.Get().CurrentScene;
        var spr = new SpriteRenderer();
        spr.Parent = this;
        Components.Add(spr);

        if (currentScene != null) Name = "SpriteRenderer: " + currentScene.GameObjects.Count + 1;
    }
    
}