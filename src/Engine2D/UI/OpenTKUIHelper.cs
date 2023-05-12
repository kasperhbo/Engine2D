using Box2DSharp.Common;
using Engine2D.Components;
using Engine2D.GameObjects;
using GlmNet;
using ImGuiNET;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using System.Drawing;
using System.Linq.Expressions;
using System.Windows.Markup;

namespace Engine2D.UI
{
    internal static class OpenTKUIHelper
    { 
        private const float defaultColumnWidth = 52;

        public static void DrawVec2Control(String label, ref Vector2 values)
        {
            DrawVec2Control(label, ref values, 0.0f, defaultColumnWidth);
        }
        public static void DrawVec2Control(String label, ref System.Numerics.Vector2 values)
        {
            Vector2 temp = new(values.X, values.Y);
            DrawVec2Control(label, ref temp, 0.0f, defaultColumnWidth);
            values = new(temp.X, temp.Y);
        }
        public static void DrawVec2Control(String label, ref Vector2 values, float resetValue)
        {
            DrawVec2Control(label, ref values, resetValue, defaultColumnWidth);
        }


        public static void DrawVec2Control(String label,ref Vector2 values, float resetValue, float columnWidth, float dragSpeed = 0.1f)
        {
        }




        public static bool DragFloat(String label, ref float value, float columnWidth = defaultColumnWidth, float dragSpeed = 0.1f)
        {
            bool changed = false;
            return changed;
            //bool changed = false;
            //ImGui.PushID(label);


            //ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new System.Numerics.Vector2(3, 3));
            
            //float lineHeight = ImGui.GetFontSize() + ImGui.GetStyle().FramePadding.Y;
            //System.Numerics.Vector2 buttonSize = new System.Numerics.Vector2(lineHeight + 3, lineHeight + 3);
            //float widthEach = (ImGui.CalcItemWidth());

            //ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(0, 0f));
            //ImGui.PushStyleColor(ImGuiCol.Button, RED);
            ////if (ImGui.Button("PX", buttonSize)) { value = 0; }

            ////ImGui.SameLine();
            //ImGui.SetNextItemWidth(widthEach);
            //if (ImGui.DragFloat("##posx", ref value, 0.1f, 0, 0, "%.1f")) changed = true; ;
            //ImGui.PopStyleColor(3);
            //ImGui.Dummy(new System.Numerics.Vector2(5, 5));
            //ImGui.PopStyleVar();
            //ImGui.PopID();

            //return changed;
        }

        public static void DrawComponentWindow(string id, string title, Action tablesToDraw,float size = 100)
        {
            ImGui.PushID(id);
            if (ImGui.CollapsingHeader(title, ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.PushStyleColor(ImGuiCol.ChildBg, new System.Numerics.Vector4(.19f, .19f, .19f, 1)); //For visibility
                ImGui.BeginChild("##transform_c", new System.Numerics.Vector2(0, size), false, 0); // Leave ~100
                ImGui.PopStyleColor();
                //Position Table
                if (ImGui.BeginTable("##transform_t", 2, ImGuiTableFlags.Resizable))
                {
                    tablesToDraw.Invoke();

                    ImGui.EndTable();
                }
                ImGui.EndChild();
            }
            ImGui.PopID();
        }


        public static bool DrawProperty(string name, ref System.Numerics.Vector2 property)
        {
            bool changed = false;
            ImGui.TableNextColumn();
            ImGui.Text(name);
            ImGui.TableNextColumn();
            if (ImGui.DragFloat2("##" + name, ref property)) changed = true;
            return changed;
        }

        public static bool DrawProperty(string name, ref SpriteColor property)
        {
            bool changed = false;
            ImGui.TableNextColumn();
            ImGui.Text(name);
            ImGui.TableNextColumn();
            if (ImGui.ColorEdit4("##" + name, ref property.Color)) changed = true;
            return changed;
        }

        public static bool DrawProperty(string name, ref bool property)
        {
            bool changed = false;
            ImGui.TableNextColumn();
            ImGui.Text(name);
            ImGui.TableNextColumn();
            if (ImGui.Checkbox("##" + name, ref property)) changed = true;
            return changed;
        }

        public static bool DrawProperty(string name, ref int property)
        {
            bool changed = false;
            ImGui.TableNextColumn();
            ImGui.Text(name);
            ImGui.TableNextColumn();
            if (ImGui.DragInt("##" + name, ref property)) changed = true;
            return changed;
        }

        public static bool DrawProperty(string name, ref System.Numerics.Vector3 property)
        {
            bool changed = false;
            ImGui.TableNextColumn();
            ImGui.Text(name);
            ImGui.TableNextColumn();
            if (ImGui.DragFloat3("##" + name, ref property)) changed = true;
            return changed;
        }

        public static bool DrawProperty(string name, ref System.Numerics.Vector4 property)
        {
            bool changed = false;
            ImGui.TableNextColumn();
            ImGui.Text(name);
            ImGui.TableNextColumn();
            if (ImGui.DragFloat4("##" + name, ref property)) changed = true;
            return changed;
        }

        public static bool DrawProperty(string name, ref float property)
        {
            bool changed = false;
            ImGui.TableNextColumn();
            ImGui.Text(name);
            ImGui.TableNextColumn();
            if (ImGui.DragFloat("##" + name, ref property)) changed = true;
            return changed;
        }

    }
}
