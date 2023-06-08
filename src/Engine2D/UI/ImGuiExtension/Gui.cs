using System.Numerics;
using Engine2D.Logging;
using ImGuiNET;
using static ImGuiNET.ImGui;

namespace Engine2D.UI.ImGuiExtension;

public static class Gui
{
    public static bool FileIcon(string filename, IntPtr icon, Action? onClick = null, Action? onDoubleClick = null, Action? onRightClick = null, Action? afterRender = null)
    {
        ImageButtonExTextDown(filename, GetID(filename), icon, 
            new(120), 
            new(0, 1), new(1, 0),
            new(0), 
            new(0.0f), new(1.0f), 
            out bool isClicked, out bool isDoubleClicked, out bool isRightClicked, afterRender);
        
        if(isClicked)onClick?.Invoke();
        if(isDoubleClicked)onDoubleClick?.Invoke();
        if(isRightClicked)onRightClick?.Invoke();
        
        
        return isClicked;
    }

    public static bool ImageButtonExTextDown(
        string label,
        uint id,
        IntPtr texture_id,
        Vector2 size,
        Vector2 uv0, Vector2 uv1,
        Vector2 padding, 
        Vector4 bg_col, Vector4 tint_col,
        Action afterRender)
    {
        return ImageButtonExTextDown(
             label,
             id,
             texture_id,
             size,
             uv0,  uv1,
             padding,  
             bg_col, tint_col,
            out bool click,
            out bool doubleclick,
            out bool rightclick, 
             afterRender
        );
    }
    
    public static bool ImageButtonExTextDown(
        string label,
        uint id,
        IntPtr texture_id, 
        Vector2 size, 
        Vector2 uv0, Vector2 uv1,
        Vector2 padding, Vector4 bg_col, Vector4 tint_col,
        out bool isClicked, out bool isDoubleClicked, out bool isRightClicked,
        Action? afterRender)
    {
        
        isClicked = false;
        isDoubleClicked = false;
        isRightClicked = false;
            
        var window = GetCurrentWindow();
        if (window.SkipItems)
            return false;

        Vector2 textSize = CalcTextSize(label, 0, true);
            
        var start = GetCursorScreenPos();
            
        Vector2 totalSizeWithoutPadding = new(size.X, size.Y > textSize.Y ? size.Y : textSize.Y);
            
        ImRect bb = new ImRect
        {
            Min = GetCursorScreenPos(), 
            Max = GetCursorScreenPos() + totalSizeWithoutPadding + padding * 2
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
            Min =GetCursorScreenPos() + reajustMIN,
            Max =GetCursorScreenPos() + reajustMAX
        };

        start.X += (size.X - textSize.X) * .5f;
        start.Y += (size.Y - textSize.Y) - 5f;
            
        bool hovered = false;
        bool held = false;
        
        isClicked = ButtonBehavior(bb, id, ref hovered, ref held);
            
        // Render
        var col = GetColorU32((held && hovered) ? 
            ImGuiCol.ButtonActive : hovered ? ImGuiCol.ButtonHovered : ImGuiCol.Button);

        if (hovered && IsMouseClicked(ImGuiMouseButton.Right)) isRightClicked = true;
        if (hovered && IsMouseDoubleClicked(ImGuiMouseButton.Left)) isDoubleClicked = true;
        
        RenderNavHighlight(bb, id);

        RenderFrame(
            bb.Min, bb.Max,
            col, true,10);

        if (bg_col.W > 0.0f)
            GetWindowDrawList().AddRectFilled(image_bb.Min + new Vector2(20,20), image_bb.Max + new Vector2(20,20),
                GetColorU32(bg_col));

        GetWindowDrawList().AddImage(texture_id,
            (image_bb.Min + padding) + new Vector2(5), (image_bb.Max - padding) - new Vector2(5),
            uv0, uv1, GetColorU32(tint_col));
            
            
        RenderText(start, label);
        
        if (ImGui.InvisibleButton(label, new Vector2(GetWidth(image_bb), GetHeight(image_bb)))) ;
        
        return isClicked;
    }
    

    #region ImRect Extensions
    
    private static float GetWidth(ImRect imageBb) { return (imageBb.Max.X - imageBb.Min.X); }
    private static float GetHeight(ImRect imageBb) { return imageBb.Max.Y - imageBb.Min.Y; }
    static Vector2  GetSize(ImRect rect)   { return new Vector2(rect.Max.X - rect.Min.X, rect.Max.Y - rect.Min.Y); }
    
    #endregion
}