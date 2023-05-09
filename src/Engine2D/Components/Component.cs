using Engine2D.GameObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Components
{
    [JsonConverter(typeof(ComponentSerializer))]
    internal abstract class Component
    {
        internal Gameobject Parent;
        public string Type = "Component";

        internal abstract void SetType();

        internal virtual void Init(Gameobject parent)
        {
            Console.WriteLine("Initialize");
            SetType();
            this.Parent = parent;
        }

        internal virtual void Start()
        { 
        }

        internal virtual void EditorUpdate(double dt)
        { 
        }

        internal virtual void GameUpdate(double dt)
        {
        }

        internal virtual void Destroy()
        {
        }

        internal abstract void ImGuiFields();
        
    }
}
