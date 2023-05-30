using Engine2D.Components;
using KDBEngine.Core;

namespace Engine2D.GameObjects;

public class PointLightGO : Gameobject
{
    public PointLightGO() : base()
    {
        var currentScene = Engine.Get()._currentScene;
        PointLightComponent pl = new PointLightComponent();
        pl.Parent = this;
        components.Add(pl);

        if (currentScene != null)
        {
            Name = "PointLight: " + currentScene.Gameobjects.Count + 1;
            
            currentScene.AddGameObjectToScene(this);
        }

    }
}