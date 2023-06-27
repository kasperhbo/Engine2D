#region

using Engine2D.Core;

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