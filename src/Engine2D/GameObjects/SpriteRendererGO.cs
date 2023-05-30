using Engine2D.Components;
using KDBEngine.Core;

namespace Engine2D.GameObjects;

public class SpriteRendererGO : Gameobject
{
    public SpriteRendererGO() : base()
    {
        var currentScene = Engine.Get()._currentScene;
        SpriteRenderer spr = new SpriteRenderer();
        spr.Parent = this;
        components.Add(spr);

        if (currentScene != null)
        {
            Name = "SpriteRenderer: " + currentScene.Gameobjects.Count + 1;
            
            currentScene.AddGameObjectToScene(this);
        }

    }
}