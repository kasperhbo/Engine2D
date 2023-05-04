using Engine2D.Core.Scripting;
using Engine2D.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleGame
{
    public class TestGameScript : IGameScript
    {
        public int UID { get; set; }
        
        public Gameobject Gameobject { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Init()
        {
            Console.WriteLine("UID: " +  UID);
        }

        public void Start()
        {
            Console.WriteLine("start");
        }

        bool r = false;
        bool r2 = false;
        public void EngineUpdate(double dt)
        {
            if(!r2) { Console.WriteLine("engine up"); r2 = true; }
        }

        public void GameUpdate()
        {
            if (!r) { Console.WriteLine("game up"); r = true; }
        }

        public void Destroy()
        {
        }
    }
}
