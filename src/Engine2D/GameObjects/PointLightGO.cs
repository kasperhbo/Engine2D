#region

using Engine2D.Components;
using Engine2D.Core;

#endregion

namespace Engine2D.GameObjects;

internal class PointLightGO : Gameobject
{
    internal PointLightGO(string name) : base(name)
    {
        var currentScene = Engine.Get().CurrentScene;
        var pl = new PointLightComponent();
        pl.Parent = this;
        Components.Add(pl);

        if (currentScene != null) Name = "PointLight: " + currentScene.GameObjects.Count + 1;
        // currentScene.AddGameObjectToScene(this);
    }
}