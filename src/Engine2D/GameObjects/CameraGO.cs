#region

using Engine2D.Cameras;
using Engine2D.Core;

#endregion

namespace Engine2D.GameObjects;

internal class CameraGO : Gameobject
{
    internal CameraGO(string name) : base(name)
    {
        var currentScene = Engine.Get().CurrentScene;
        var camera = new Camera();
        camera.Parent = this;
        Components.Add(camera);

        if (currentScene != null) Name = "Camera: " + currentScene.GameObjects.Count + 1;
        // currentScene.AddGameObjectToScene(this);
    }
}