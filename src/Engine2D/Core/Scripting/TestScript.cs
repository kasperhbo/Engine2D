using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Core.Scripting
{
    internal class TestScript : IGameScript
    {
        public void Init()
        {
            Console.WriteLine("Init Test Script");
        }

        public void Start()
        {
            Console.WriteLine("Start Test Script");
        }

        public void Update(double dt)
        {
            Console.WriteLine("Update Test Script");
        }
    }
}
