using Engine2D.Components;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Testing;
using Engine2D.UI;
using ImGuiNET;
using KDBEngine.Core;
using Newtonsoft.Json;
using OpenTK.Mathematics;
using System.Globalization;
using System.Runtime.InteropServices;
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
                
                bool open = false;

                ImGui.Separator();
                string title = component.Type;
                if (component.Type == "ScriptHolderComponent")
                {
                    ScriptHolderComponent scriptHolder = (ScriptHolderComponent)component;
                    if(scriptHolder.component != null)
                    {
                        title = scriptHolder.component.Type;
                    }
                }

                open = ImGui.TreeNodeEx(title, treeNodeFlags, title);

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
                {

                    if (ImGui.BeginDragDropTarget())
                    {
                        ImGuiPayloadPtr payload = ImGui.AcceptDragDropPayload("Script_Drop");
                        if (payload.IsValidPayload())
                        {
                            string component = (string)GCHandle.FromIntPtr(payload.Data).Target;
                            Log.Message("Dropped: " + component);
                            
                            //Window.Get().ChangeScene(new LevelEditorScene(), filename);
                        }

                        ImGui.EndDragDropTarget();
                    }

                    ImGui.OpenPopup("AddComponent");
                }

              
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
