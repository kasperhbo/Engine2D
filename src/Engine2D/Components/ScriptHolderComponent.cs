﻿using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Rendering;
using ImGuiNET;
using KDBEngine.Core;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Engine2D.Components
{
    [JsonConverter(typeof(ComponentSerializer))]
    public class ScriptHolderComponent : Component
    {

        //public string refFile = "";
        public Component component = null;

        public override void Init(Gameobject parent)
        {
            base.Init(parent);            
        }

        public override void ImGuiFields()
        {
            //ImGui.ImageButton("", IntPtr.Zero, new System.Numerics.Vector2(56, 56));
            if (component != null)
            {
                ImGui.Button(component?.Type, new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, 29));
            }
            else
            {
                ImGui.Button("script", new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, 29));
            }
            if (ImGui.BeginDragDropTarget())
            {
                ImGuiPayloadPtr payload = ImGui.AcceptDragDropPayload("SCRIPT_DROP");
                if (payload.IsValidPayload())
                {
                    string filename = (string)GCHandle.FromIntPtr(payload.Data).Target;
                    
                    //Component instance = (Comp;onent)Activator.CreateInstance(refComponent);

                    //refFile = filename;

                    Component instance = (Component)Activator.CreateInstance(ComponentRegistry.Get(filename));
                    component = instance;   
                }

                ImGui.EndDragDropTarget();
            }

            component?.ImGuiFields();

        }


        public override string GetItemType()
        {
            return "ScriptHolderComponent";
        }
    }
}
