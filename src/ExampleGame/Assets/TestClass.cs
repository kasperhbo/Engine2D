﻿//FOLDER:  D:\dev\Engine2D\src\ExampleGame\Assets\TestClass.cs

using Engine2D.Components;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Rendering;
using Newtonsoft.Json;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ExampleGame.Assets;

public class TestClass : Component
{
    [JsonProperty]public int twospeed = 2;
    [JsonProperty]public int speed = 10;
    [JsonProperty]public string testName = "hello name";
    
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