using Engine2D.Components;
using Engine2D.Core;

namespace Engine2D.GameObjects;

public class SpriteRendererGO : Gameobject
{
    public SpriteRendererGO() : base()
    {
        var currentScene = Engine.Get().CurrentScene;
        SpriteRenderer spr = new SpriteRenderer();
        spr.Parent = this;
        components.Add(spr);

        if (currentScene != null)
        {
            Name = "SpriteRenderer: " + currentScene.GameObjects.Count + 1;
            
            currentScene.AddGameObjectToScene(this);
        }

    }
}