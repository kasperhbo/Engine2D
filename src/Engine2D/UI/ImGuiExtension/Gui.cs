using System.ComponentModel;
using System.Numerics;
using Engine2D.Logging;
using Engine2D.Utilities;
using ImGuiNET;
using static ImGuiNET.ImGui;

namespace Engine2D.UI.ImGuiExtension;

public static class Gui
{
    // public static bool FileIcon(        
    //     IntPtr texId, string name, 
    //     Vector2 texture_size, Vector2 imageSize,
    //     Vector2 uv0, Vector2 uv1, int frame_padding,
    //     Vector4 bg_col, Vector4 tint_col,
    //     out bool isClicked, out bool isDoubleClicked, out bool isRightClicked,
    //     Action? afterRender
    //     )
    // {
    //     return ImageButtonExTextDown(
    //         texId, name, 
    //         texture_size, imageSize, 
    //         uv0, uv1, frame_padding,
    //         bg_col, tint_col,
    //         out isClicked, out isDoubleClicked, out isRightClicked,
    //         afterRender);
    // }

    // public static bool ImageButtonExTextDown(
    //     string label,
    //     uint id,
    //     IntPtr texture_id,
    //     Vector2 size,
    //     Vector2 uv0, Vector2 uv1,
    //     Vector2 padding, 
    //     Vector4 bg_col, Vector4 tint_col,
    //     Action afterRender)
    // {
    //     return ImageButtonExTextDown(
    //          label,
    //          id,
    //          texture_id,
    //          size,
    //          uv0,  uv1,
    //          padding,  
    //          bg_col, tint_col,
    //         out bool click,
    //         out bool doubleclick,
    //         out bool rightclick, 
    //          afterRender
    //     );
    // }

    public static bool FileIcon(
        IntPtr texId, string name, 
        Vector2 texture_size, Vector2 imageSize,
        Vector2 uv0, Vector2 uv1, int frame_padding,
        Vector4 bg_col, Vector4 tint_col,
        out bool isClicked, out bool isDoubleClicked, out bool isRightClicked,
        Action? afterRender)
    {
        isClicked = false;
        isDoubleClicked = false;
        isRightClicked = false;

        Vector2 size = imageSize;
        
        if(size.X <= 0 && size.Y <= 0)
        {
            size.X = size.Y = GetTextLineHeightWithSpacing();
        }
        else
        {
            if(size.X <= 0)
                size.X = size.Y;
            else if(size.Y <= 0)
                size.Y = size.X;
            
            size *= GetCurrentWindow().FontWindowScale * GetIO().FontGlobalScale;
        }
        
        var style = GetStyle();

        Vector2 size_name = CalcTextSize(name, false);
        var name_sz = name.Length;
        
        char[] label_str = name.ToCharArray();

        if (size_name.X > imageSize.X)
        {
            for (var ds = name_sz; ds > 3; --ds)
            {
                size_name = CalcTextSize(name, true);
                if (size_name.X < imageSize.X)
                {
                    label_str[ds - 2] = '.';
                    label_str[ds - 1] = '.';
                    label_str[ds] = '.';
                    break;
                }
            }
        }

        string label = StringUtils.CharToString(label_str);

        Vector2 textSize = CalcTextSize(label, true);
        bool hasText = textSize.X > 0;
        
        float innerSpacing =
            hasText ? ((frame_padding >= 0) ? frame_padding : (style.ItemInnerSpacing.X)) : 0f;
        
        var padding =
            (frame_padding >= 0) ? new Vector2(frame_padding, frame_padding) : style.FramePadding;
     
        
        bool istextBig = textSize.X > imageSize.X;

        var totalSizeWithoutPadding = new Vector2(size.X, size.Y > textSize.Y ? size.Y : textSize.Y);

        ImRect bb = new ImRect
        {
            Min = GetCursorPos(),
            Max = GetCursorPos() + totalSizeWithoutPadding + padding * 2f
        };

        Vector2 start = new(0);

        start = GetCursorPos() + padding;

        if (size.Y < textSize.Y)
        {
            start.Y += (textSize.Y - size.Y) * .5f;
        }

        Vector2 reajustMIN = new(0);
        Vector2 reajustMAX = size;
        
        if(bb.Max.Y - textSize.Y < start.Y + reajustMAX.Y)
        {
            reajustMIN.X += textSize.Y / 2;
            reajustMAX.X -= textSize.Y / 2;
            reajustMAX.Y -= textSize.Y;
        }

        ImRect image_bb = new ImRect
        {
            Min = start + reajustMIN,
            Max = start + reajustMAX
        };

        start = GetCursorPos() + padding;
        start.Y += (size.Y - textSize.Y) + 2;
        
        if(!istextBig)
        {
            start.X += (size.X - textSize.X) * .5f;
        }
        
        ItemSize(bb);
        // if (!ItemAdd(bb, id))
        // {
        //     return false;
        // }
        //
        bool hovered = false;
        bool held = false;
        // isClicked = ButtonBehavior(bb, , ref hovered, ref held);

        var col = GetColorU32((hovered && held) ? ImGuiCol.ButtonActive
            : hovered ? ImGuiCol.ButtonHovered : ImGuiCol.Button);

        RenderFrame(bb.Min, bb.Max, col, true, 
            Math.Clamp(MathF.Min(padding.X, padding.Y), 0.0f, style.FrameRounding));
        
        GetWindowDrawList().AddRectFilled(image_bb.Min, image_bb.Max, GetColorU32(bg_col));
        float w = texture_size.X;
        float h = texture_size.Y;
        var imgSz = new Vector2(GetWidth(image_bb), GetHeight(image_bb));
        float max_size = MathF.Max(imgSz.X, imgSz.Y);
        
        float aspect = w / h;
        if(w > h)
        {
            float m = MathF.Min(max_size, w);

            imgSz.X = m;
            imgSz.Y = m / aspect;
        }
        else if(h > w)
        {
            float m = MathF.Min(max_size, h);

            imgSz.X = m * aspect;
            imgSz.Y = m;
        }

        if(imgSz.X > imgSz.Y)
            image_bb.Min.Y += (max_size - imgSz.Y) * 0.5f;
        if(imgSz.X < imgSz.Y)
            image_bb.Min.X += (max_size - imgSz.X) * 0.5f;

        image_bb.Max = image_bb.Min + imgSz;
        
        GetWindowDrawList().AddImage(texId, image_bb.Min, image_bb.Max, uv0, uv1, GetColorU32(tint_col));
        
        return isClicked;
    }


    public static bool ImageButtonExTextDown(
        string label,
        uint id,
        IntPtr texture_id,
        Vector2 size,
        Vector2 uv0, Vector2 uv1,
        Vector2 padding, Vector4 bg_col, Vector4 tint_col,
        out bool isClicked, out bool isDoubleClicked, out bool isRightClicked,
        Action? afterRender,
        float rOverwrite = -1,
        float gOverwrite = -1,
        float bOverwrite = -1,
        float aOverwrite = -1
        )
    {
        isClicked = false;
        isDoubleClicked = false;
        isRightClicked = false;
            
        var window = GetCurrentWindow();
        if (window.SkipItems)
            return false;

        Vector2 textSize = CalcTextSize(label, 0, true);
            
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
            Max = GetCursorScreenPos() + size// + reajustMAX
        };

        bool hovered = false;
        bool held = false;
        
        start = GetCursorScreenPos() + padding;
        start.Y += (size.Y - textSize.Y) - 2;
        start.X += (size.X - textSize.X) / 2;
        
        ImGui.InvisibleButton(label, 
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

        if (rOverwrite != -1 && gOverwrite != -1 && bOverwrite != -1 && aOverwrite != -1)
        {
            col = GetColorU32(new Vector4(rOverwrite, gOverwrite, bOverwrite, aOverwrite));
        }
        
        RenderFrame(
            image_bb.Min, image_bb.Max,
            col, true, 8);

        // if(bg_col.W > 0.0f)
        //     GetWindowDrawList().AddRectFilled((image_bb.Min + padding) + new Vector2(5), (image_bb.Max - padding) - new Vector2(5), 
        //         GetColorU32(bg_col));
        padding = new(15);
        
        GetWindowDrawList().AddImage(texture_id,
            (image_bb.Min + padding), (image_bb.Max - padding),
            uv0, uv1, GetColorU32(tint_col));
            

        // if(istextBig == false)

        RenderText(start, label);
        
        //if 
        //
        return isClicked;
    }
    

    #region ImRect Extensions
    
    private static float GetWidth(ImRect imageBb) { return (imageBb.Max.X - imageBb.Min.X); }
    private static float GetHeight(ImRect imageBb) { return imageBb.Max.Y - imageBb.Min.Y; }
    static Vector2  GetSize(ImRect rect)   { return new Vector2(rect.Max.X - rect.Min.X, rect.Max.Y - rect.Min.Y); }
    
    #endregion
}