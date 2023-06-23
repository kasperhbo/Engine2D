#region

using System.Numerics;
using Engine2D.GameObjects;
using Engine2D.UI.Browsers;
using Engine2D.Utilities;
using ImGuiNET;
using static ImGuiNET.ImGui;

#endregion

namespace Engine2D.UI.ImGuiExtension;

internal static class Gui
{
    internal static void DrawTable(string label, Action tableComponents)
    {
        if (CollapsingHeader(label, ImGuiTreeNodeFlags.DefaultOpen))
        {
            BeginTable("##table", 2, ImGuiTableFlags.None | ImGuiTableFlags.Resizable);
            tableComponents.Invoke();
            EndTable();
            Separator();
        }
    }
    
    internal static void DrawProperty(string label, ref float value)
    {
        PushID(label);
        TableNextRow();
        TableSetColumnIndex(0);
        Text(label);
        TableSetColumnIndex(1);
        KDragFloat("##"+label,ref value);
        PopID();
    }

    internal static bool DrawProperty(string label, ref int value, int min, int max)
    {
        PushID(label);
        TableNextRow();
        TableSetColumnIndex(0);
        Text(label);
        TableSetColumnIndex(1);
        bool dragged = KDragInt("##"+label,ref value, min, max);
        PopID();
        return dragged;
    }

    internal static void DrawProperty(string label, ref int value)
    {
        PushID(label);
        TableNextRow();
        TableSetColumnIndex(0);
        Text(label);
        TableSetColumnIndex(1);
        KDragInt("##"+label,ref value);
        PopID();
    }
    
    internal static void DrawProperty(string label, ref string value)
    {
        PushID(label);
        TableNextRow();
        TableSetColumnIndex(0);
        Text(label);
        TableSetColumnIndex(1);
        KStringInput("##"+label,ref value);
        PopID();
    }
    
    internal static void DrawProperty(string label, ref bool value)
    {
        PushID(label);
        TableNextRow();
        TableSetColumnIndex(0);
        Text(label);
        TableSetColumnIndex(1);
        KBoolInput("##"+label,ref value);
        PopID();
    }
    

    internal static void DrawProperty(string label, ref OpenTK.Mathematics.Vector2 value)
    {
        Vector2 v = new Vector2(value.X, value.Y);
        DrawProperty(label, ref v);
        value.X = v.X;
        value.Y = v.Y;
    }

    
    internal static void DrawProperty(string label, ref Vector2 value)
    {
        PushID(label);
        TableNextRow();
        TableSetColumnIndex(0);
        Text(label);
        TableSetColumnIndex(1);
        Vector2Prop(ref value);
        PopID();
    }

    internal static void DrawProperty(string label, ref OpenTK.Mathematics.Vector3 value)
    {
        Vector3 v = new Vector3(value.X, value.Y, value.Z);
        DrawProperty(label, ref v);
        value.X = v.X;
        value.Y = v.Y;
        value.Z = v.Z;
    }

    internal static void DrawProperty(string label, ref Vector3 value)
    {
        PushID(label);
        TableNextRow();
        TableSetColumnIndex(0);
        Text(label);
        TableSetColumnIndex(1);
        Vector3Prop(ref value);
        PopID();
    }
    
    internal static void DrawProperty(string label, ref OpenTK.Mathematics.Vector4 value)
    {
        Vector4 v = new Vector4(value.X, value.Y, value.Z, value.W);
        DrawProperty(label, ref v);
        value.X = v.X;
        value.Y = v.Y;
        value.Z = v.Z;
        value.W = v.W;
    }

    internal static void DrawProperty(string label, ref Vector4 value)
    {
        PushID(label);
        TableNextRow();
        TableSetColumnIndex(0);
        Text(label);
        TableSetColumnIndex(1);
        Vector4Prop(ref value);
        PopID();
    }
    
    internal static void Vector2Prop(ref Vector2 value)
    {
        PushItemWidth(100);
        KDragFloat("X", ref value.X, new Vector4(1,0,0,1),0.1f);
        SameLine();
        KDragFloat("Y", ref value.Y, new Vector4(0,1,0,1),0.1f);
        PopItemWidth();
    }
    
    internal static void Vector3Prop(ref Vector3 value)
    {
        PushItemWidth(100);
        KDragFloat("X", ref value.X, new Vector4(1,0,0,1),0.1f);
        SameLine();
        KDragFloat("Y", ref value.Y, new Vector4(0,1,0,1),0.1f);
        SameLine();
        KDragFloat("Z", ref value.Z, new Vector4(0,0,1,1),0.1f);
        PopItemWidth();
    }
    
    internal static void Vector4Prop(ref Vector4 value)
    {
        PushItemWidth(100);
        KDragFloat("X", ref value.X, new Vector4(1,0,0,1),0.1f);
        SameLine();
        KDragFloat("Y", ref value.Y, new Vector4(0,1,0,1),0.1f);
        SameLine();
        KDragFloat("Z", ref value.Z, new Vector4(0,0,1,1),0.1f);
        SameLine();
        KDragFloat("W", ref value.W, new Vector4(1,0,1,1),0.1f);
        PopItemWidth();
    }
    
    
    internal static void KStringInput(string label, ref string value)
    {
        InputText(("##" + label), ref value, 256);
    }
    
    internal static void KBoolInput(string label, ref bool value)
    {
        Checkbox(("##" + label), ref value);
    }
    
    internal static void KDragInt(string label, ref int intRef, Vector4 buttonColor = new(), float dragSpeed = 0.1f)
    {
        PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
        if(buttonColor != Vector4.Zero)
            PushStyleColor(ImGuiCol.Button, buttonColor);
        
        if (Button(label)) intRef = 0;
        SameLine();
        DragInt(("##" + label), ref intRef, dragSpeed);
        PopStyleVar();
        
        if(buttonColor != Vector4.Zero)
            PopStyleColor();
    }
    
    internal static bool KDragInt(string label, ref int intRef, int min, int max, Vector4 buttonColor = new(), float dragSpeed = 0.1f)
    {
        bool dragged = false;
        PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
        if(buttonColor != Vector4.Zero)
            PushStyleColor(ImGuiCol.Button, buttonColor);
        
        if (Button(label)) intRef = 0;
        SameLine();
        if (DragInt(("##" + label), ref intRef, dragSpeed, min, max))
        {
            dragged = true;
        }
        PopStyleVar();
        
        if(buttonColor != Vector4.Zero)
            PopStyleColor();

        return dragged;
    }
    
    
    
    private static void KDragFloat(string label, ref float floatRef,  Vector4 buttonColor = new(), float dragSpeed = 0.1f)
    {
        
        PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
        if(buttonColor != Vector4.Zero)
            PushStyleColor(ImGuiCol.Button, buttonColor);
        
        if (Button(label)) floatRef = 0;
        SameLine();
        DragFloat(("##" + label), ref floatRef, dragSpeed);
        PopStyleVar();
        
        if(buttonColor != Vector4.Zero)
            PopStyleColor();
    }
    
    
    internal static bool ImageButtonExTextDown(
        string label,
        ESupportedFileTypes fileType,
        IntPtr texture_id,
        Vector2 size,
        Vector2 uv0, Vector2 uv1,
        Vector2 padding, Vector2 ImageAdjustLocation,
        Vector4 tint_col,
        out bool isClicked, out bool isDoubleClicked, out bool isRightClicked,
        bool isSelected, bool showType = true)
    {
        isClicked = false;
        isDoubleClicked = false;
        isRightClicked = false;

        var originalLabel = label;

        var window = GetCurrentWindow();
        if (window.SkipItems)
            return false;

        var textSize = CalcTextSize(label, 0, true);

        var isTextBig = textSize.X > size.X;
        if (isTextBig) label = label.Remove(7, label.Length - 7) + "...";

        textSize = CalcTextSize(label, 0, true);

        var start = GetCursorScreenPos() + padding;

        var bb = new ImRect
        {
            Min = GetCursorScreenPos(),
            Max = GetCursorScreenPos()
        };

        Vector2 reajustMIN = new(0, 0);
        var reajustMAX = size;

        if (bb.Max.Y - textSize.Y < start.Y + reajustMAX.Y)
        {
            reajustMIN.X += textSize.Y / 2;
            reajustMAX.X -= textSize.Y / 2;
            reajustMAX.Y -= textSize.Y;
        }

        ImRect image_bb = new()
        {
            Min = GetCursorScreenPos(), // + reajustMIN,
            Max = GetCursorScreenPos() + size // + reajustMAX
        };

        var hovered = false;
        var held = false;

        start = GetCursorScreenPos() + padding;
        start.Y += size.Y - textSize.Y - 2;
        start.X += (size.X - textSize.X) / 2;

        InvisibleButton(label,
            new Vector2(
                GetWidth(image_bb),
                GetHeight(image_bb)
            ));

        if (IsItemHovered())
            hovered = true;

        if (IsItemClicked())
            isClicked = true;

        // Render

        if (hovered && IsMouseClicked(ImGuiMouseButton.Right)) isRightClicked = true;
        if (hovered && IsMouseDoubleClicked(ImGuiMouseButton.Left)) isDoubleClicked = true;
        var col = GetColorU32(ImGuiCol.TextDisabled);

        col = GetColorU32(held && hovered ? ImGuiCol.ButtonActive : hovered ? ImGuiCol.ButtonHovered : ImGuiCol.Button);

        if (isSelected) col = GetColorU32(ImGuiCol.TextSelectedBg);

        RenderFrame(
            image_bb.Min, image_bb.Max,
            col, true, 8);

        padding = new Vector2(15);

        GetWindowDrawList().AddImage(texture_id,
            image_bb.Min - ImageAdjustLocation + padding, image_bb.Max - ImageAdjustLocation - padding,
            uv0, uv1, GetColorU32(tint_col));

        if (IsItemHovered()) SetTooltip(fileType + " " + originalLabel);

        PushStyleColor(ImGuiCol.Text, new Vector4(0, 0, 0, 1));
        image_bb.Min.Y = image_bb.Min.Y + 10;

        if (showType)
            RenderText(image_bb.Min - ImageAdjustLocation + padding, fileType.ToString());

        PopStyleColor();

        RenderText(start, label);

        return isClicked;
    }

    internal static bool TopBarButton(float buttonLocationX, Vector2 buttonSize, TopBarButton button)
    {
        var clicked = false;
        PushStyleColor(ImGuiCol.ButtonHovered, button._hoverColor);
        SetCursorPosX(buttonLocationX);
        if (Button(button._label, buttonSize)) clicked = true;
        PopStyleColor();

        return clicked;
    }


    #region ImRect Extensions

    private static float GetWidth(ImRect imageBb)
    {
        return imageBb.Max.X - imageBb.Min.X;
    }

    private static float GetHeight(ImRect imageBb)
    {
        return imageBb.Max.Y - imageBb.Min.Y;
    }

    private static Vector2 GetSize(ImRect rect)
    {
        return new Vector2(rect.Max.X - rect.Min.X, rect.Max.Y - rect.Min.Y);
    }

    #endregion
}

internal class TopBarButton
{
    internal Vector4 _hoverColor;
    internal string _label;

    internal TopBarButton(string label, Vector4 hoverColor)
    {
        _hoverColor = hoverColor;
        _label = label;
    }

    internal TopBarButton(string label)
    {
        var style = GetStyle();
        _hoverColor = style.Colors[(int)ImGuiCol.ButtonHovered];
        _label = label;
    }
}