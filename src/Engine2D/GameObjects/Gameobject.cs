using Engine2D.Core.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.GameObjects
{
    public class Gameobject
    {
        private List<IGameScript> scripts = new List<IGameScript>();    

        private bool _initialized = false;
        
        public void Init()
        {
            if(_initialized) return;

            
        }

        public void Start()
        {
            if(_initialized) return;

            _initialized = true;
        }

        public void OnRender()
        {

        }


        public void EditorUpdate(double dt)
        {

        }



        public void GameUpdate(double dt)
        {      
        }

        public void OnEndGameLoop()
        {

        }

        public void OnDestroy()
        {
            //remove from renderer
            //stop all components/gamescripts
        }

        public void OnClose()
        {

        }

    }
}
