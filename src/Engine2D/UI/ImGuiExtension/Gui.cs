using System.Numerics;
using Engine2D.Utilities;
using ImGuiNET;
using static ImGuiNET.ImGui;

namespace Engine2D.UI.ImGuiExtension;

public static class Gui
{
    public static bool ImageButtonExTextDown(string label,
        IntPtr texture_id,
        Vector2 size,
        Vector2 uv0, Vector2 uv1,
        Vector2 padding, Vector2 ImageAdjustLocation,
        Vector4 tint_col,
        out bool isClicked, out bool isDoubleClicked, out bool isRightClicked,
        bool isSelected,
        float rOverwrite = -1,
        float gOverwrite = -1,
        float bOverwrite = -1,
        float aOverwrite = -1)
    {
        isClicked = false;
        isDoubleClicked = false;
        isRightClicked = false;
            
        string originalLabel = label;
        
        var window = GetCurrentWindow();
        if (window.SkipItems)
            return false;

        Vector2 textSize = CalcTextSize(label, 0, true);

        bool isTextBig = textSize.X > size.X;
        if(isTextBig)
        {
            label = label.Remove(7, label.Length - 7) + "...";
        }
        
        textSize = CalcTextSize(label, 0, true);
        
        var start = GetCursorScreenPos() + padding;
        
        ImRect bb = new ImRect
        {
            Min = GetCursorScreenPos(), 
            Max = GetCursorScreenPos()
        };
            
        Vector2 reajustMIN = new(0, 0);
        Vector2 reajustMAX = size;
            
        if(bb.Max.Y - textSize.Y < start.Y + reajustMAX.Y)
        {
            reajustMIN.X += textSize.Y / 2;
            reajustMAX.X -= textSize.Y / 2;
            reajustMAX.Y -= textSize.Y;
        }
            
        ImRect image_bb = new()
        {
            Min = GetCursorScreenPos(),// + reajustMIN,
            Max = (GetCursorScreenPos()) + size// + reajustMAX
        };

        bool hovered = false;
        bool held = false;
        
        start = GetCursorScreenPos() + padding;
        start.Y += (size.Y - textSize.Y) - 2;
        start.X += (size.X - textSize.X) / 2;
        
        InvisibleButton(label, 
            new Vector2(
                GetWidth(image_bb), 
                GetHeight(image_bb)
            )) ;

        if (IsItemHovered())
            hovered = true;
        
        if (IsItemClicked())
            isClicked = true;

        // Render
        
        if (hovered && IsMouseClicked(ImGuiMouseButton.Right)) isRightClicked = true;
        if (hovered && IsMouseDoubleClicked(ImGuiMouseButton.Left)) isDoubleClicked = true;
        
        var col = GetColorU32((held && hovered) ? 
            ImGuiCol.ButtonActive : hovered ? ImGuiCol.ButtonHovered : ImGuiCol.Button);

        if (isSelected)
        {
            col = GetColorU32(ImGuiCol.TextSelectedBg);
        }
        
        if (rOverwrite != -1 && gOverwrite != -1 && bOverwrite != -1 && aOverwrite != -1)
        {
            col = GetColorU32(new Vector4(rOverwrite, gOverwrite, bOverwrite, aOverwrite));
        }
        
        RenderFrame(
            image_bb.Min, image_bb.Max,
            col, true, 8);
        
        padding = new(15);
        
        GetWindowDrawList().AddImage(texture_id,
            (image_bb.Min - ImageAdjustLocation  + padding), (image_bb.Max - ImageAdjustLocation- padding),
            uv0, uv1, GetColorU32(tint_col));
        
        if(IsItemHovered())
        {
            SetTooltip(originalLabel);
        }

        RenderText(start, label);
        
        return isClicked;
    }
    

    #region ImRect Extensions
    
    private static float GetWidth(ImRect imageBb) { return (imageBb.Max.X - imageBb.Min.X); }
    private static float GetHeight(ImRect imageBb) { return imageBb.Max.Y - imageBb.Min.Y; }
    static Vector2  GetSize(ImRect rect)   { return new Vector2(rect.Max.X - rect.Min.X, rect.Max.Y - rect.Min.Y); }
    
    #endregion
}