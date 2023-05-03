using Engine2D.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Core.Scripting
{
    internal class ScriptRunner
    {
        private Dictionary<Gameobject, List<IGameScript>> scripts = new Dictionary<Gameobject, List<IGameScript>>();

        public void LoadScript(IGameScript script, Gameobject gameObject)
        {
            if (!scripts.ContainsKey(gameObject))
            {
                scripts.Add(gameObject, new List<IGameScript>());
            }
            scripts[gameObject].Add(script);
        }

        public void RaiseStartEvenet()
        {
            
            foreach (var pair in scripts)
            {
                var gameObject = pair.Key;
                var gameObjectScripts = pair.Value;
                foreach (var script in gameObjectScripts)
                {
                    script.Init();
                    script.Start();
                }
            }
        }

        public void RaiseUpdateEvent(double dt)
        {
            foreach (var pair in scripts)
            {
                var gameObject = pair.Key;
                var gameObjectScripts = pair.Value;
                foreach (var script in gameObjectScripts)
                {
                    script.Update(dt);
                }
            }
        }
    }
}
