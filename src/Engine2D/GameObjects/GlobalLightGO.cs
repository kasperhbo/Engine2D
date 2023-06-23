#region

using Engine2D.Components;
using Engine2D.Core;

#endregion

namespace Engine2D.GameObjects;

internal class GlobalLightGO : Gameobject
{
    internal GlobalLightGO(string name) : base(name)
    {
        var currentScene = Engine.Get().CurrentScene;
        var gl = new GlobalLight();
        gl.Parent = this;
        Components.Add(gl);

        if (currentScene != null) Name = "GlobalLight: " + currentScene.GameObjects.Count + 1;
        // currentScene.AddGameObjectToScene(this);
    }
}