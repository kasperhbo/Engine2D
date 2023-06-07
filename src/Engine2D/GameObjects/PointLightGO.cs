using Engine2D.Components;
using Engine2D.Core;

namespace Engine2D.GameObjects;

public class PointLightGO : Gameobject
{
    public PointLightGO(string name) : base(name)
    {
        var currentScene = Engine.Get().CurrentScene;
        PointLightComponent pl = new PointLightComponent();
        pl.Parent = this;
        components.Add(pl);

        if (currentScene != null)
        {
            Name = "PointLight: " + currentScene.GameObjects.Count + 1;
            
            // currentScene.AddGameObjectToScene(this);
        }

    }
}