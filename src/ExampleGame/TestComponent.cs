using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ExampleGame
{
    public class TestComponent : Component
    {
        public float test = 0;

        public override void Init(Gameobject parent)
        {
            base.Init(parent);
            Console.WriteLine("init");

            Utils.GetFilePath("TestComponent.cs");
        }

        public override void ImGuiFields()
        {
            base.ImGuiFields();

            ImGui.DragFloat("float: ", ref test);
            ImGui.DragFloat("float: ", ref test);
            ImGui.DragFloat("float: ", ref test);
            ImGui.DragFloat("float: ", ref test);
            ImGui.DragFloat("float: ", ref test);
            ImGui.DragFloat("float: ", ref test);

        }

        public override string GetItemType()
        {            
            return "TestComponent";
        }

        public override Vector2 WindowSize()
        {
            return new(-1,-1);
        }
    }
}
