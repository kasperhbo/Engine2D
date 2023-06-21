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
    
    public override void EditorUpdate(double dt)
    {
        base.EditorUpdate(dt);
    }

    public override string GetItemType()
    {
        return "ExampleGame.Assets.TestClass";
    }

    public override void ImGuiFields()
    {
        base.ImGuiFields();
        Console.ForegroundColor = (System.ConsoleColor.Red);
        Console.WriteLine("New build");
        Console.WriteLine("Updating imgui fields");
    }
}