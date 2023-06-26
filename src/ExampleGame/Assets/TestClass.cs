//FOLDER:  D:\dev\Engine2D\src\ExampleGame\Assets\TestClass.cs

using Engine2D.Components;
using Engine2D.Components.TransformComponents;
using Newtonsoft.Json;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ExampleGame.Assets;

public class TestClass : Component
{
    [JsonProperty]public int twospeed = 2;
    [JsonProperty]public int speed = 10;
    [JsonProperty]public string testName = "hello name";

    public override void Update(FrameEventArgs args)
    {
        
    }

    public override void GameUpdate(double dt)
    {
        base.GameUpdate(dt);
        Parent.GetComponent<Transform>().Position.X += speed * (float)dt;
    }

    public override void EditorUpdate(double dt)
    {
        base.EditorUpdate(dt);
    }

    public override void StartPlay()
    {
        
    }

    public override string GetItemType()
    {
        return "ExampleGame.Assets.TestClass";
    }

    public override void ImGuiFields()
    {
        base.ImGuiFields();
    }
}