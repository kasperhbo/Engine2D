using Engine2D.Flags;
using Engine2D.GameObjects;
using Newtonsoft.Json;
using System.Reflection;
using System.Numerics;
using System.ComponentModel;
using ImGuiNET;
using Engine2D.UI;

namespace Engine2D.Components
{
    [JsonConverter(typeof(ComponentSerializer))]
    public abstract class Component
    {        
        [JsonIgnore]public Gameobject Parent;
        [JsonIgnore]private bool _initialized = false;
        public string Type => GetItemType();


        public abstract string GetItemType();
        

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

        protected float sizeYGUI = 0;
        public virtual System.Numerics.Vector2 WindowSize()
        {            
            return new Vector2(0, sizeYGUI);
        }




        public virtual float ImGuiFields()
        {
            sizeYGUI = 3;
            float y = ImGui.GetFontSize() + 14;
            FieldInfo[] fields = this.GetType().GetFields(
                                                          BindingFlags.DeclaredOnly
                                                        | BindingFlags.Public
                                                        | BindingFlags.NonPublic
                                                        | BindingFlags.Instance                                                        
                                                        ); foreach (var field in fields)
            {
                Type type = field.FieldType;
                var value = field.GetValue(this);
                string name = field.Name;

                var attrs = (ShowUIAttribute[])field.GetCustomAttributes
                (typeof(ShowUIAttribute), false);

                bool ignore = false;
                foreach (var attr in attrs) if (!attr.show) ignore = true;

                

                if (!ignore)
                {
                    if (type == typeof(SpriteColor))
                    {
                        SpriteColor val = (SpriteColor)value;

                        sizeYGUI += y;
                        
                        OpenTKUIHelper.DrawProperty(name, ref val);
                        
                        field.SetValue(this, val);
                    }

                    if (type == typeof(int))
                    {
                        int val = (int)value;
                        sizeYGUI += y;
                        OpenTKUIHelper.DrawProperty(name, ref val);
                        field.SetValue(this, val);
                    }

                    if (type == typeof(float))
                    {
                        float val = (float)value;
                        sizeYGUI += y;
                        OpenTKUIHelper.DrawProperty(name, ref val);
                        field.SetValue(this,val);
                    }

                    if (type == typeof(bool))
                    {
                        bool val = (bool)value;
                        sizeYGUI += y;
                        OpenTKUIHelper.DrawProperty(name, ref val);
                        field.SetValue(this, val);
                    }

                    if (type == typeof(System.Numerics.Vector2))
                    {
                        Vector2 val = (Vector2)value;
                        sizeYGUI += y;
                        OpenTKUIHelper.DrawProperty(name, ref val);
                        field.SetValue(this,val);
                    }

                    if (type == typeof(Vector3))
                    {
                        Vector3 val = (Vector3)value;
                        sizeYGUI += y;
                        OpenTKUIHelper.DrawProperty(name, ref val);
                        field.SetValue(this,val);
                    }

                    if (type == typeof(Vector4))
                    {
                        Vector4 val = (Vector4)value;
                        sizeYGUI += y;
                        OpenTKUIHelper.DrawProperty(name, ref val);
                        field.SetValue(this,val);
                    }
                }
            }
            return sizeYGUI;   
        }
    }
}
