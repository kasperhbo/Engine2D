using Box2DSharp.Common;
using GlmNet;
using ImGuiNET;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
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
            //ImGui.PushID(label);

                       
            //float lineHeight = ImGui.GetFontSize() + ImGui.GetStyle().FramePadding.Y;
            //System.Numerics.Vector2 buttonSize = new System.Numerics.Vector2(lineHeight + 3, lineHeight + 3);
            //float widthEach = (ImGui.CalcItemWidth() - buttonSize.X * 2.0f) / 2.0f;

            //if (ImGui.Button("PX", buttonSize)) { values.X = 0; }
            
            //ImGui.SameLine();
            //ImGui.DragFloat("##posx", ref values.X, 0.1f, 0, 0, "%.1f");            
            //ImGui.SameLine();
            
            //if (ImGui.Button("PY", buttonSize)) 
            //{ values.Y = 0; }
            //ImGui.SameLine();
            //ImGui.SetNextItemWidth(widthEach);
            //ImGui.DragFloat("##posx", ref values.Y, 0.1f, 0, 0, "%.1f");
            //ImGui.PopStyleVar();
            //ImGui.PopID();            
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

        public static void DrawTransformControl(Action tablesToDraw)
        {
            if (ImGui.CollapsingHeader("Transform", ImGuiTreeNodeFlags.DefaultOpen))
            {                
                ImGui.PushStyleColor(ImGuiCol.ChildBg, new System.Numerics.Vector4(.19f, .19f, .19f, 1)); //For visibility
                ImGui.BeginChild("##transform_c", new System.Numerics.Vector2(0, 90), false, 0); // Leave ~100
                ImGui.PopStyleColor();
                //Position Table
                if (ImGui.BeginTable("##transform_t", 2, ImGuiTableFlags.Resizable))
                {
                    tablesToDraw.Invoke();

                    ImGui.EndTable();
                }
                ImGui.EndChild();

            }
        }



    }
}
