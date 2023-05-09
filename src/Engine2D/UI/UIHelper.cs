using GlmNet;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.UI
{
    internal static class UIHelper
    {
        private static float defaultResetValue = 0.0f;
        private static float defaultColumnWidth = 256;
        private static float defaultDragSpeed = 0.1f;
        private static float divideMultiplier = 2;

        public static void DrawVec2Control(String label, ref Vector2 values)
        {
            DrawVec2Control(label, ref values, 0.0f, defaultColumnWidth);
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

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0f));

            float lineHeight = ImGui.GetFontSize() + ImGui.GetStyle().FramePadding.Y;
            Vector2 buttonSize = new Vector2(lineHeight +3, lineHeight +3);
            float widthEach = (ImGui.CalcItemWidth() - buttonSize.X * 2.0f) / 2.0f;

            ImGui.PushItemWidth(widthEach);
            ImGui.PushStyleColor(ImGuiCol.Button,        new Vector4(0.7f, 0.2f, 0.2f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.8f, 0.3f, 0.3f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive,  new Vector4(0.7f, 0.2f, 0.2f, 1.0f));

            if (ImGui.Button("X", new Vector2(buttonSize.X, buttonSize.Y)))
            {
                values.X = resetValue;
            }

            ImGui.SameLine();
            
            ImGui.DragFloat("##x", ref values.X, defaultDragSpeed);
            ImGui.SameLine();

            ImGui.PushItemWidth(widthEach);
            ImGui.PushStyleColor(ImGuiCol.Button,        new Vector4(0.2f, 0.7f, 0.2f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.8f, 0.3f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive,  new Vector4(0.2f, 0.7f, 0.2f, 1.0f));
            
            ImGui.Dummy(new Vector2(.5f, 0f));
            ImGui.SameLine();
            if (ImGui.Button("Y", new Vector2(buttonSize.X, buttonSize.Y)))
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

        public static float DragFloat(String label, ref float value)
        {
            ImGui.PushID(label);

            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, defaultColumnWidth/divideMultiplier);
            ImGui.Text(label);
            ImGui.NextColumn();

            ImGui.DragFloat("##dragFloat", ref value, 0.1f);

            ImGui.Columns(1);
            ImGui.PopID();
            return value;
        }

        public static bool ColorPicker4(String label, ref Vector4 color)
        {
            bool changed = false;
            ImGui.PushID(label);

            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, defaultColumnWidth / divideMultiplier);
            ImGui.Text(label);
            ImGui.NextColumn();

            if(ImGui.ColorEdit4("##colorPicker", ref color))
            {
                changed = true;
            }

            ImGui.Columns(1);
            ImGui.PopID();
            return changed;
        }

        public static bool ImageButton(String label, IntPtr textureID)
        {
            bool clicked = false;
            ImGui.PushID(label);

            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, defaultColumnWidth / divideMultiplier);
            ImGui.Text(label);
            ImGui.NextColumn();

            //if (ImGui.ColorEdit4("##colorPicker", ref color))
            //{
            //    changed = true;
            //}

            if(ImGui.ImageButton("##button", textureID, new Vector2(32, 32)))
            {
                clicked = true;
            }

            ImGui.Columns(1);
            ImGui.PopID();
            return clicked;
        }

    }
}
