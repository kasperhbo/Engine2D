using Engine2D.Components;
using KDBEngine.Core;

namespace Engine2D.GameObjects;

public class GlobalLightGO : Gameobject
{
    public GlobalLightGO() : base()
    {
        var currentScene = Engine.Get()._currentScene;
        GlobalLight gl = new GlobalLight();
        gl.Parent = this;
        components.Add(gl);

        if (currentScene != null)
        {
            Name = "GlobalLight: " + currentScene.Gameobjects.Count + 1;
            
            currentScene.AddGameObjectToScene(this);
        }

    }
}