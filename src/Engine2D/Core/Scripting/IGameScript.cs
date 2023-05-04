using Engine2D.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Core.Scripting
{
    public interface IGameScript
    {        
        public int UID { get; set; }
        public Gameobject Gameobject { get; set; }

        void Init();
        void Start();
        void EngineUpdate(double dt);
        void GameUpdate();
        void Destroy();

    }
}
