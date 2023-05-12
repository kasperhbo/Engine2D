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
        public float y = 0;
        public float s = 0;
        public float f = 0;
        public float g = 0;
        public float u = 0;
        public float t = 0;
        public float e = 0;
        public float w = 0;
        public float Q = 0;
        public float k = 0;
        public float Z = 0;
        public float x = 0;

        public override void Init(Gameobject parent)
        {
            base.Init(parent);
            Console.WriteLine("init");

            Utils.GetFilePath("TestComponent.cs");
        }


        public override string GetItemType()
        {            
            return "TestComponent";
        }

    }
}
