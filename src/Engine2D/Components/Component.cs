using Engine2D.Flags;
using Engine2D.GameObjects;
using Newtonsoft.Json;
using System.Reflection;
using System.Numerics;
using System.ComponentModel;
using ImGuiNET;

namespace Engine2D.Components
{
    [JsonConverter(typeof(ComponentSerializer))]
    public abstract class Component
    {        
        [JsonIgnore]public Gameobject Parent;
        public string Type => GetItemType();
        [JsonIgnore] private bool _initialized = false;


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

        float sizeYGUI = 0;
        public virtual System.Numerics.Vector2 WindowSize()
        {
            
            return new Vector2(0, sizeYGUI);
        }

        


        public virtual void ImGuiFields()
        {
            sizeYGUI = 0;
            float y = ImGui.GetFontSize() + 10;
            FieldInfo[] fields = this.GetType().GetFields(
                                                          BindingFlags.DeclaredOnly
                                                        | BindingFlags.Public
                                                        | BindingFlags.NonPublic
                                                        | BindingFlags.Instance); foreach (var field in fields)
            {
                Type type = field.FieldType;
                var value = field.GetValue(this);
                string name = field.Name;


                if (type == typeof(int))
                {
                    int val = (int)value;
                    sizeYGUI += y;
                    if (ImGuiNET.ImGui.DragInt(name + ": ", ref val))
                    {
                        field.SetValue(this, val);
                    }
                }

                if (type == typeof(float))
                {
                    float val = (float)value;
                    sizeYGUI += y;

                    if (ImGuiNET.ImGui.DragFloat(name + ": ", ref val))
                    {
                        field.SetValue(this, val);
                    }
                }

                if (type == typeof(bool))
                {
                    bool val = (bool)value;
                    sizeYGUI += y;

                    if (ImGuiNET.ImGui.Checkbox(name + ": ", ref val))
                    {
                        field.SetValue(this, !val);
                    }
                }

                if (type == typeof(System.Numerics.Vector2))
                {
                    Vector2 val = (Vector2)value;
                    sizeYGUI += y;

                    if (ImGuiNET.ImGui.DragFloat2(name + ": ", ref val))
                    {
                        field.SetValue(this, val);
                    }
                }

                if (type == typeof(Vector3))
                {
                    Vector3 val = (Vector3)value;
                    sizeYGUI += y;

                    if (ImGuiNET.ImGui.DragFloat3(name + ": ", ref val))
                    {
                        field.SetValue(this, val);
                    }
                }

                if (type == typeof(Vector4))
                {
                    Vector4 val = (Vector4)value;
                    sizeYGUI += y;

                    if (ImGuiNET.ImGui.DragFloat4(name + ": ", ref val))
                    {
                        field.SetValue(this, val);
                    }
                }
            }
        }


    }
}
