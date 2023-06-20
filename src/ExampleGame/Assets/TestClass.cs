//FOLDER:  D:\dev\Engine2D\src\ExampleGame\Assets\TestClass.cs

using Engine2D.Components;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Rendering;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ExampleGame.Assets;

public class TestClass : Component
{
    public int twospeed = 2;
    public int speed = 10;
    public string testName = "hello name";
    
    
    public override void Init(Gameobject parent)
    {
        Log.Succes("Loaded test class");
        base.Init(parent);
    }

    public override void Init(Gameobject parent, Renderer? renderer)
    {
        Log.Succes("Loaded test class");
        base.Init(parent, renderer);
    }


    public override void EditorUpdate(double dt)
    {
        // Console.ForegroundColor = (System.ConsoleColor.Blue);
        // Console.WriteLine(testName);
        base.EditorUpdate(dt);
    }

    public override string GetItemType()
    {
        return "ExampleGame.Assets.TestClass";
    }
}