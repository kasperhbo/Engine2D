using Engine2D.Components;
using Engine2D.Core;
using Engine2D.Flags;
using Engine2D.GameObjects;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ExampleGame
{
    public class TestComponent : Component
    {
        [ShowUI(show = true)]public float y = 0;
        [ShowUI(show = true)]public float s = 0;
        [ShowUI(show = true)]public float f = 0;
        [ShowUI(show = true)]public float g = 0;
        [ShowUI(show = true)]public float u = 0;
        [ShowUI(show = false)]public float t = 0;
        [ShowUI(show = false)]public float e = 0;
        [ShowUI(show = false)]public float w = 0;
        [ShowUI(show = false)]public float Q = 0;
        [ShowUI(show = false)]public float k = 0;
        [ShowUI(show = false)]public float Z = 0;
        [ShowUI(show = false)] public float x = 0;

        // public override void Init(Gameobject parent)
        // {
        //     base.Init(parent);
        //     Console.WriteLine("init");
        //
        //     Utils.GetFilePath("TestComponent.cs");
        // }


        public override string GetItemType()
        {            
            return "TestComponent";
        }

    }
}
