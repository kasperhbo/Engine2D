using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.GameObjects
{
    public class Gameobject
    {
        private bool _initialized = false;

        public void Init()
        {
            if(_initialized) return;

            _initialized = true;
        }

        public void OnClose()
        {         
        }

        public void Render()
        {
        }

        public void Update(double dt)
        {         
        }
    }
}
