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
    public abstract class Component
    {
        [JsonIgnore]public Gameobject Parent;
        public string Type => GetItemType();
        [JsonIgnore] private bool _initialized = false;

        
        public virtual string GetItemType()
        {
            return "Component";
        }

        public virtual void Init(Gameobject parent)
        {
            if(_initialized) return;
            _initialized = true;
            this.Parent = parent;
        }

        public virtual void Start()
        { 
        }

        public virtual void EditorUpdate(double dt)
        { 
        }

        public virtual void GameUpdate(double dt)
        {
        }

        public virtual void Destroy()
        {
        }

        public virtual void ImGuiFields()
        {

        }
        
    }
}
