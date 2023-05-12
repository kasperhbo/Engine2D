using Engine2D.Components;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Testing;
using Engine2D.UI;
using GlmNet;
using ImGuiNET;
using KDBEngine.Core;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Drawing;
using System.Globalization;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Engine2D.GameObjects
{
    public class Gameobject : Asset
    {
        
        public Transform transform = new();

        public string Name = "";
        public List<Component> components = new();
        public List<Component> LinkedComponents = new();


        public Gameobject() { }

        public Gameobject(string name, Transform transform)
        {
            this.Name = name;
            this.transform = transform;
            this.components = new();
            this.LinkedComponents = new();
        }



        public Gameobject(string name, List<Component> components, Transform transform)
        {
            this.Name = name;
            this.transform = transform;
            this.components = components;
            this.LinkedComponents = new();
        }


        public Gameobject(string name, List<Component> components, List<Component> linked, Transform transform)
        {
            this.Name = name;            
            this.transform = transform;
            this.components = components;
            this.LinkedComponents = linked;
        }


        public void Init()
        {
            foreach (var component in components) { component.Init(this); }
            foreach (var component in LinkedComponents) { component?.Init(this); }
        }

        public void Start()
        {            
            foreach (var component in components) { component.Start(); }
            foreach (var component in LinkedComponents) { component?.Start(); }
        }

        public void EditorUpdate(double dt)
        {
            foreach (var component in components) { component.EditorUpdate(dt); }
            foreach (var component in LinkedComponents) { component?.EditorUpdate(dt); }
        }

        public void GameUpdate(double dt)
        {
            foreach (var component in components){component.GameUpdate(dt);}
            foreach (var component in LinkedComponents) { component?.GameUpdate(dt); }
        }

        public void Destroy()
        {
            foreach (var component in components) { component.Destroy(); }
            foreach (var component in LinkedComponents) { component?.Destroy(); }
        }

        private List<Component> _componentsToAddEndOfFrame = new List<Component>();

        public void AddComponent(Component component)
        {
            _componentsToAddEndOfFrame.Add(component);
        }

        private void ActualAddComponent(Component component )
        {
            component.Init(this);
            components.Add(component);
        }

        public void AddLinkedComponent(Component component)
        {
            component.Init(this);
            LinkedComponents.Add(component);
        }

        public bool AABB(Vector2 point)
        {
            return (
               point.X >= this.transform.position.X - this.transform.size.X * .5
            && point.X <= this.transform.position.X + this.transform.size.X * .5
            && point.Y >= this.transform.position.Y - this.transform.size.Y * .5
            && point.Y <= this.transform.position.Y + this.transform.size.Y * .5);
        }

        public override void OnGui()
        {
           
            ImGui.InputText("##name", ref Name, 256);
            ImGui.SameLine();
            ImGui.Separator();

             List<Component> componentsToRemove = new List<Component>();

            OpenTKUIHelper.DrawTransformControl(() =>
            {
                ImGui.TableNextColumn();
                ImGui.Text("Position");
                ImGui.TableNextColumn();
                ImGui.DragFloat2("##postion", ref transform.position);

                ImGui.TableNextColumn();
                ImGui.Text("Rotation");
                ImGui.TableNextColumn();
                ImGui.DragFloat("##rotation", ref transform.rotation);

                ImGui.TableNextColumn();
                ImGui.Text("Scale");
                ImGui.TableNextColumn();
                ImGui.DragFloat2("##scale", ref transform.size);
            });

            for (int i = 0; i < components.Count; i++)
            {
                ImGui.PushID(i);
                if (ImGui.CollapsingHeader(components[i].GetItemType(), ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ImGui.PushStyleColor(ImGuiCol.ChildBg, new System.Numerics.Vector4(0.19f, .19f, .19f, 1)); //For visibility

                    ImGui.BeginChild("##", new System.Numerics.Vector2(0, components[i].WindowSize().Y), false, 0); ; // Leave ~100
                    ImGui.PopStyleColor(3);

                    //ImGui.BeginGroup();

                    components[i].ImGuiFields();
                    //ImGui.EndGroup();
                    //ImGui.GetWindowDrawList().AddRect(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), (int)ImGuiCol.ChildBg);
                    ImGui.GetForegroundDrawList().AddRect(
                        ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), 3);

                    ImGui.EndChild();
                }
                ImGui.PopID();
            }


            {
                ImGui.Dummy(new System.Numerics.Vector2(0, ImGui.GetContentRegionAvail().Y - 80));
                ImGui.Separator();

                if (ImGui.Button("Add component", new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, 26)))
                {

                    if (ImGui.BeginDragDropTarget())
                    {
                        ImGuiPayloadPtr payload = ImGui.AcceptDragDropPayload("Script_Drop");
                        if (payload.IsValidPayload())
                        {
                            string component = (string)GCHandle.FromIntPtr(payload.Data).Target;
                            Log.Message("Dropped: " + component);
                        }

                        ImGui.EndDragDropTarget();
                    }

                    ImGui.OpenPopup("AddComponent");
                }
                ImGui.GetMousePos();
                
                ImGui.Dummy(new System.Numerics.Vector2(0, ImGui.GetContentRegionAvail().Y));


                //TODO: ADD COMPONENT TO GOP
                if (ImGui.BeginPopup("AddComponent"))
                {
                    //if (ImGui.MenuItem("Camera"))
                    //{                    
                    //    ImGui.CloseCurrentPopup();
                    //}
                    if (ImGui.MenuItem("ScriptComponent"))
                    {
                        ScriptHolderComponent rb = new ScriptHolderComponent();
                        Gameobject? go = (Gameobject)Engine.Get().CurrentSelectedAsset;
                        go?.AddComponent(rb);
                        ImGui.CloseCurrentPopup();
                    }
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

            foreach (var component in _componentsToAddEndOfFrame)
            {
                ActualAddComponent(component);
            }
            _componentsToAddEndOfFrame.Clear();
        }
               

        private void RemoveComponents(Component comp)
        {
            components.Remove(comp);
        }

        private void RemoveLinkedComponents(Component comp)
        {
            LinkedComponents.Remove(comp);
        }

    }


}
