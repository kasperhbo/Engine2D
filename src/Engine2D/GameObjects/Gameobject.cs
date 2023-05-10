using Engine2D.Components;
using Engine2D.Testing;
using Engine2D.UI;
using ImGuiNET;
using OpenTK.Mathematics;
using System.Globalization;
using System.Xml.Serialization;

namespace Engine2D.GameObjects
{
    internal class Gameobject
    {
        
        public Transform transform = new();

        public string Name = "";
        public List<Component> components = new();
               

        internal Gameobject() { }

        internal Gameobject(string name, Transform transform)
        {
            this.Name = name;
            this.transform = transform;
            this.components = new();
        }


        internal Gameobject(string name, List<Component> components, Transform transform)
        {
            this.Name = name;            
            this.transform = transform;
            this.components = components;
        }


        internal void Init()
        {
            foreach (var component in components) { component.Init(this); }
        }

        internal void Start()
        {            
            foreach (var component in components) { component.Start(); }
        }

        internal void EditorUpdate(double dt)
        {
            foreach (var component in components) { component.EditorUpdate(dt); } 
        }

        internal void GameUpdate(double dt)
        {
            foreach (var component in components)
            {
                component.GameUpdate(dt);
            }
        }

        internal void Destroy()
        {
            foreach (var component in components) { component.Destroy(); }
        }

        internal void AddComponent(Component component)
        {
            components.Add(component);
            component.Init(this);
        }

        internal void ImGuiFields()
        {
            ImGui.Text("Name: ");
            ImGui.SameLine();
            ImGui.InputText("", ref Name, 256);

            if(ImGui.CollapsingHeader("Transform"))
            {
                //ImGui.DragFloat2("Position", ref transform.position);
                OpenTKUIHelper.DrawVec2Control("Position", ref transform.position);
                OpenTKUIHelper.DrawVec2Control("Size", ref transform.size);
                OpenTKUIHelper.DragFloat("Rotation", ref transform.rotation);
            }



            foreach (var component in components) { component.ImGuiFields(); }
        }

        public bool AABB(Vector2 point)
        {
            return (
               point.X >= this.transform.position.X - this.transform.size.X * .5
            && point.X <= this.transform.position.X + this.transform.size.X * .5
            && point.Y >= this.transform.position.Y - this.transform.size.Y * .5
            && point.Y <= this.transform.position.Y + this.transform.size.Y * .5);
        }
    }
}
