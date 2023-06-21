#region

using System.Numerics;
using Engine2D.UI.Browsers;
using ImGuiNET;
using static ImGuiNET.ImGui;

#endregion

namespace Engine2D.UI.ImGuiExtension;

internal static class Gui
{
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