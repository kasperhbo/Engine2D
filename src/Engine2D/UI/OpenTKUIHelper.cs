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
        private const float defaultColumnWidth = 256;
        private const float defaultDragSpeed = 0.1f;
        private const float divideMultiplier = 2;

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

        public static void DrawVec2Control(String label,ref Vector2 values, float resetValue, float columnWidth)
        {
            ImGui.PushID(label);

            ImGui.Columns(2);
            
            ImGui.SetColumnWidth(0, columnWidth/divideMultiplier);
            ImGui.SetColumnWidth(1, defaultColumnWidth * 3);
            ImGui.Text(label);
            
            ImGui.NextColumn();

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(0, 0f));

            float lineHeight = ImGui.GetFontSize() + ImGui.GetStyle().FramePadding.Y;
            Vector2 buttonSize = new Vector2(lineHeight +3, lineHeight +3);
            float widthEach = (ImGui.CalcItemWidth() - buttonSize.X * 2.0f) / 2.0f;

            ImGui.PushItemWidth(widthEach);
            ImGui.PushStyleColor(ImGuiCol.Button,        new System.Numerics.Vector4(0.7f, 0.2f, 0.2f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new System.Numerics.Vector4(0.8f, 0.3f, 0.3f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive,  new System.Numerics.Vector4(0.7f, 0.2f, 0.2f, 1.0f));

            if (ImGui.Button("X", new System.Numerics.Vector2(buttonSize.X, buttonSize.Y)))
            {
                values.X = resetValue;
            }

            ImGui.SameLine();
            
            ImGui.DragFloat("##x", ref values.X, defaultDragSpeed);
            ImGui.SameLine();

            ImGui.PushItemWidth(widthEach);
            ImGui.PushStyleColor(ImGuiCol.Button,        new System.Numerics.Vector4(0.2f, 0.7f, 0.2f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new System.Numerics.Vector4(0.3f, 0.8f, 0.3f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive,  new System.Numerics.Vector4(0.2f, 0.7f, 0.2f, 1.0f));
            
            ImGui.SameLine();
            if (ImGui.Button("Y", new System.Numerics.Vector2(buttonSize.X, buttonSize.Y)))
            {
                values.Y = resetValue;
            }
            ImGui.SameLine();            
            ImGui.DragFloat("##y", ref values.Y, defaultDragSpeed);
            ImGui.PopItemWidth();
            ImGui.SameLine();

            ImGui.NextColumn();

            ImGui.Columns(1);
            ImGui.PopStyleVar();
            ImGui.PopStyleColor(6);
            ImGui.Spacing();
            ImGui.PopID();
        }


        public static bool DragFloat(String label, ref float value, float columnWidth = defaultColumnWidth, float dragSpeed = defaultDragSpeed)
        {
            bool changed = false;
            ImGui.PushID(label);

            ImGui.Columns(2);

            ImGui.SetColumnWidth(0, columnWidth / divideMultiplier);
            ImGui.SetColumnWidth(1, columnWidth * 3);

            ImGui.Text(label);

            ImGui.NextColumn();

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(0, 0f));

            float lineHeight = ImGui.GetFontSize() + ImGui.GetStyle().FramePadding.Y;
            Vector2 buttonSize = new Vector2(lineHeight + 3, lineHeight + 3);
            float widthEach = (ImGui.CalcItemWidth() - buttonSize.X * 2.0f) / 2.0f;

            ImGui.PushItemWidth(widthEach);
            ImGui.PushStyleColor(ImGuiCol.Button, new System.Numerics.Vector4(0.7f, 0.2f, 0.2f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new System.Numerics.Vector4(0.8f, 0.3f, 0.3f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new System.Numerics.Vector4(0.7f, 0.2f, 0.2f, 1.0f));

            ImGui.Button("X", new System.Numerics.Vector2(buttonSize.X, buttonSize.Y));
            ImGui.SameLine();

            if (ImGui.DragFloat("##x", ref value, dragSpeed)) changed = true;
            
            ImGui.SameLine();

            ImGui.PushItemWidth(widthEach);
                       
            ImGui.PopItemWidth();
            ImGui.SameLine();

            ImGui.NextColumn();

            ImGui.Columns(1);
            ImGui.PopStyleVar();
            ImGui.PopStyleColor(6);
            ImGui.Spacing();
            ImGui.PopID();

            return changed;
        }

       
    }
}
