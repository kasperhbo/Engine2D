using Engine2D.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleGame.Registers
{
    public class CustomComponentRegister
    {
        static string[] _typesStr =
        {
            "D:\\dev\\EngineDev\\Engine2D\\src\\ExampleGame\\TestComponent.cs",
            "D:\\dev\\EngineDev\\Engine2D\\src\\ExampleGame\\NewComponent.cs",
//LAST LINE 01
        };

        static Type[] _types =
        {
            typeof(TestComponent),
            typeof(NewComponent),
//LAST LINE 02
        };

        internal static void StartRegister()
        {
            for (int i = 0; i < _types.Length; i++)
            {
                ComponentRegistry.Register(_typesStr[i], _types[i]);
            }
        }
    }
}
