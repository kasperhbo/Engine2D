using Engine2D.Components;
using Engine2D.Testing;
using KDBEngine.Core;
using OpenTK.Mathematics;

namespace Engine2D.GameObjects;

public class CameraGO : Gameobject
{
    public CameraGO() : base()
    {
        var currentScene = Engine.Get()._currentScene;
        TestCamera camera = new TestCamera(new(0,0), new(1280,720));
        camera.Parent = this;
        components.Add(camera);

        if (currentScene != null)
        {
            Name = "Camera: " + currentScene.Gameobjects.Count + 1;

            currentScene.AddGameObjectToScene(this);
        }

    }
}