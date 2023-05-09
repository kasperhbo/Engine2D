﻿using Engine2D.Components;
using ImGuiNET;
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
                ImGui.DragFloat2("Position", ref transform.position);
                ImGui.DragFloat2("Size", ref transform.size);
                ImGui.DragFloat("Rotation", ref transform.rotation);
            }



            foreach (var component in components) { component.ImGuiFields(); }
        }
    }
}
