using Engine2D.Components;
using Engine2D.Core;
using Engine2D.Testing;
using Engine2D.UI;
using ImGuiNET;
using KDBEngine.Core;
using OpenTK.Mathematics;
using System.Globalization;
using System.Xml.Serialization;

namespace Engine2D.GameObjects
{
    internal class Gameobject : Asset
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

        public bool AABB(Vector2 point)
        {
            return (
               point.X >= this.transform.position.X - this.transform.size.X * .5
            && point.X <= this.transform.position.X + this.transform.size.X * .5
            && point.Y >= this.transform.position.Y - this.transform.size.Y * .5
            && point.Y <= this.transform.position.Y + this.transform.size.Y * .5);
        }

        internal override void OnGui()
        {
            //ImGui.Text("Name: ");
            //ImGui.SameLine();
            //ImGui.InputText("", ref Name, 256);
            //ImGui.SameLine();
            //float xSize = ImGui.GetContentRegionAvail().X;
            //ImGui.Dummy(new System.Numerics.Vector2(xSize-30, 0));
            //ImGui.SameLine();
            //if(ImGui.Button("+", new System.Numerics.Vector2(28, 28)))
            //{

            //}

            ImGui.InputText("##name", ref Name, 256);
            ImGui.SameLine();
            ImGui.Separator();

            if (ImGui.CollapsingHeader("Transform"))
            {
                //ImGui.DragFloat2("Position", ref transform.position);
                OpenTKUIHelper.DrawVec2Control("Position", ref transform.position);
                OpenTKUIHelper.DrawVec2Control("Size", ref transform.size);
                OpenTKUIHelper.DragFloat("Rotation", ref transform.rotation);
            }
            List<Component> componentsToRemove = new List<Component>();

            foreach (var component in components)
            {
                ImGuiTreeNodeFlags treeNodeFlags = ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.AllowItemOverlap | ImGuiTreeNodeFlags.FramePadding;

                System.Numerics.Vector2 contentRegionAvailable = ImGui.GetContentRegionAvail();

                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new System.Numerics.Vector2(4, 4));

                float lineHeight = EngineSettings.DefaultFontSize + 3 * 2.0f;

                ImGui.Separator();
                bool open = ImGui.TreeNodeEx(component.Type, treeNodeFlags, component.Type);
                ImGui.PopStyleVar();
                ImGui.SameLine(contentRegionAvailable.X - lineHeight * 0.5f);
                if (ImGui.Button("+", new System.Numerics.Vector2(lineHeight, lineHeight)))
                {
                    ImGui.OpenPopup("ComponentSettings");
                }

                if (ImGui.BeginPopup("ComponentSettings"))
                {
                    if (ImGui.MenuItem("Remove component"))
                        componentsToRemove.Add(component);

                    ImGui.EndPopup();
                }
                if (open)
                {
                    component.ImGuiFields();


                }
            }

            {
                ImGui.Dummy(new System.Numerics.Vector2(0, ImGui.GetContentRegionAvail().Y - 30));
                ImGui.Separator();

                if (ImGui.Button("Add component", new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, 26)))
                    ImGui.OpenPopup("AddComponent");

                //TODO: ADD COMPONENT TO GOP
                if (ImGui.BeginPopup("AddComponent"))
                {
                    //if (ImGui.MenuItem("Camera"))
                    //{                    
                    //    ImGui.CloseCurrentPopup();
                    //}
                    if (ImGui.MenuItem("RigidBody"))
                    {
                        RigidBody rb = new RigidBody(Box2DSharp.Dynamics.BodyType.DynamicBody);
                        Gameobject? go = (Gameobject)Engine.Get().CurrentSelectedAsset;
                        go?.AddComponent(rb);
                        ImGui.CloseCurrentPopup();
                    }
                    if (ImGui.MenuItem("Sprite Renderer"))
                    {
                        SpriteRenderer spr = new SpriteRenderer();
                        Gameobject? go = (Gameobject)Engine.Get().CurrentSelectedAsset;
                        go?.AddComponent(spr);
                        ImGui.CloseCurrentPopup();
                    }

                    ImGui.EndPopup();
                }
            }

            foreach (var component in componentsToRemove)
            {
                RemoveComponents(component);
            }
        }

        private void RemoveComponents(Component comp)
        {
            components.Remove(comp);
        }
    }
}
