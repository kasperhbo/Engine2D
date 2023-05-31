using Engine2D.Components;
using Engine2D.Core;

namespace Engine2D.GameObjects;

public class GlobalLightGO : Gameobject
{
    public GlobalLightGO() : base()
    {
        var currentScene = Engine.Get().CurrentScene;
        GlobalLight gl = new GlobalLight();
        gl.Parent = this;
        components.Add(gl);

        if (currentScene != null)
        {
            Name = "GlobalLight: " + currentScene.GameObjects.Count + 1;
            
            currentScene.AddGameObjectToScene(this);
        }

    }
}