
using System.Drawing;
using System.Numerics;
using Engine2D.Core;
using Engine2D.Logging;
using Engine2D.Rendering;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;

namespace Engine2D.UI.Browsers;

public class AssetBrowser : UiElemenet
{
    public AssetBrowser() : base()
    {
        Init();
    }
    
    protected override string SetWindowTitle()
    {
        Log.Message("Creating a new Asset Browser");
        return "New Asset Browser";
    }

    protected override ImGuiWindowFlags SetWindowFlags()
    {
        return ImGuiWindowFlags.None;
    }

    protected override Action SetWindowContent()
    {
        return DrawUI;
    }

    private static string _baseAssetDir = ProjectSettings.FullProjectPath + "\\Assets";
    private DirectoryInfo _currentDirectory = new DirectoryInfo(_baseAssetDir);
    
    private IntPtr _dirIcon;
    private IntPtr _fileIcon;

    private Texture dirtexture;
    private Texture fileTexture;

    private List<DirectoryInfo> _directoriesInDirectory = new List<DirectoryInfo>();
    private List<FileInfo> _filesInDirectory = new List<FileInfo>();

    private void Init()
    {
        LoadIcons();
        SwitchDirectory(_currentDirectory);
    }
    
    private void LoadIcons()
    {
        dirtexture  = new Texture(Utils.GetBaseEngineDir() + "\\Images\\Icons\\directoryicon.png" , false, 
        TextureMinFilter.Linear, TextureMagFilter.Linear);
        
        fileTexture = new Texture(Utils.GetBaseEngineDir() + "\\Images\\Icons\\fileicon.png", false, 
        TextureMinFilter.Linear, TextureMagFilter.Linear);
    }

    private void SwitchDirectory(DirectoryInfo newDirectory)
    {
        ClearLists();
        
        GetDirectoriesInCurrent();
        GetFilesInCurrent();
        
        _currentDirectory = newDirectory;
    }

    private void ClearLists()
    {
        _directoriesInDirectory = new();
        _filesInDirectory = new();
    }

    private void GetDirectoriesInCurrent()
    {
        _directoriesInDirectory = _currentDirectory.GetDirectories().ToList();
    }

    private void GetFilesInCurrent()
    {
        _filesInDirectory = _currentDirectory.GetFiles().ToList();
    }

    private void DrawUI()
    {
        ImGui.BeginChild("item view", new(0, -ImGui.GetFrameHeightWithSpacing()));
        {
            int columnCount = (int)(ImGui.GetContentRegionAvail().X / (90 + 20));
            ImGui.Columns((columnCount < 1) ? 1 : columnCount, "", false);
            {
                DrawFolders();
                DrawFiles();
            }
            ImGui.Columns(1);
        }
    }

    private void DrawFolders()
    {
        foreach (var dir in _directoriesInDirectory)
        {
            DrawEntry(dir.Name, dir.FullName, dirtexture.TexID);
            ImGui.NextColumn();
        }
    }

    private void DrawFiles()
    {
        foreach (var file in _filesInDirectory)
        {
            DrawEntry(file.Name, file.FullName, fileTexture.TexID);
            ImGui.NextColumn();
        }
    }

    private bool DrawEntry(string name, string fullName, IntPtr icon, Action? onClick = null, Action? onDoubleClick = null, Action? onRightClick = null)
    {
        ImGui.PushID(fullName);
        return UIHelper.FileIcon(name, icon, onClick, onDoubleClick, onRightClick);
    }
}

public static class UIHelper
{
    public static bool FileIcon(string filename, IntPtr icon, Action? onClick = null, Action? onDoubleClick = null, Action? onRightClick = null)
    {
        var cursorScreenPos = ImGui.GetCursorScreenPos();
        
        // IntPtr texId, string name, 
        // Vector2 texture_size, Vector2 imageSize,
        // Vector2 uv0, Vector2 uv1, int frame_padding,
        // Vector4 bg_col, Vector4 tint_col
        //
        // ImageButtonWithText(
        //     icon, filename,
        //     new Vector2(120,  120), new Vector2(120, 120),
        //     new Vector2(0, 0), new Vector2(1, 1), 5,
        //     new Vector4(255, 0, 0, 255), new Vector4(255, 255, 1, 255)
        // );
        
        ImageButtonExTextDown(filename, ImGui.GetID(filename), icon, new(120), new(0, 1),
            new(1, 0), new(0), new(0), new(1.0f));

        if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
        {
            onClick?.Invoke();
        }
        
        if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
        {
            onDoubleClick?.Invoke();
        }
        
        if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
        {
            onRightClick?.Invoke();
        }
        
        bool isClicked = ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left);
        
        // input label
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (20) / 2.0f);
        
        if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
        {
            onClick?.Invoke();
        }
        
        if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
        {
            onDoubleClick?.Invoke();
        }
        
        if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
        {
            onRightClick?.Invoke();
        }
        
        return isClicked;
    }

    public static bool ImageButtonExTextDown(
        string label,
        uint id,
        IntPtr texture_id, 
        Vector2 size, 
        Vector2 uv0, Vector2 uv1,
        Vector2 padding, Vector4 bg_col, Vector4 tint_col)
    {
        unsafe
        {
            var window = ImGui.GetCurrentWindow();
            if (window.SkipItems)
                return false;

            Vector2 textSize = ImGui.CalcTextSize(label, 0, true);
            
            var start = ImGui.GetCursorScreenPos();
            
            Vector2 totalSizeWithoutPadding = new(size.X, size.Y > textSize.Y ? size.Y : textSize.Y);
            
            ImRect bb = new ImRect()
            {
                Min = ImGui.GetCursorScreenPos(), 
                Max = ImGui.GetCursorScreenPos() + totalSizeWithoutPadding + padding * 2
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
                Min =ImGui.GetCursorScreenPos() + reajustMIN,
                Max =ImGui.GetCursorScreenPos() + reajustMAX
            };

            start.X += (size.X - textSize.X) * .5f;
            start.Y += (size.Y - textSize.Y) - 5f;
            
            bool hovered = false;
            bool held = false;
        
            bool pressed = ImGui.ButtonBehavior(bb, id, ref hovered, ref held);

            // Render
            var col = ImGui.GetColorU32((held && hovered) ? 
                ImGuiCol.ButtonActive : hovered ? ImGuiCol.ButtonHovered : ImGuiCol.Button);
        
            ImGui.RenderNavHighlight(bb, id);

            ImGui.RenderFrame(
                bb.Min, bb.Max,
                col, true,10);

            if (bg_col.W > 0.0f)
                ImGui.GetWindowDrawList().AddRectFilled(image_bb.Min + new Vector2(20,20), image_bb.Max + new Vector2(20,20),
                    ImGui.GetColorU32(bg_col));

            ImGui.GetWindowDrawList().AddImage(texture_id,
                (image_bb.Min + padding) + new Vector2(5), (image_bb.Max - padding) - new Vector2(5),
                uv0, uv1, ImGui.GetColorU32(tint_col));
            
            
            ImGui.RenderText(start, label);
            
            return pressed;
        }
    }
    
    /// <summary>
    /// CODE WHERE UPPER CODE IS TAKEN FROM!
    /// </summary>
    /// <param name="texId"></param>
    /// <param name="name"></param>
    /// <param name="texture_size"></param>
    /// <param name="imageSize"></param>
    /// <param name="uv0"></param>
    /// <param name="uv1"></param>
    /// <param name="frame_padding"></param>
    /// <param name="bg_col"></param>
    /// <param name="tint_col"></param>
    private static void ImageButtonWithText(        
        IntPtr texId, string name, 
        Vector2 texture_size, Vector2 imageSize,
        Vector2 uv0, Vector2 uv1, int frame_padding,
        Vector4 bg_col, Vector4 tint_col
        )
    {
        Vector2 size = imageSize;
        
        if (size.X <= 0 && size.Y <= 0)
        {
            size.X = size.Y = ImGui.GetTextLineHeight();
        }
        else
        {
            if (size.X <= 0)
                size.X = size.Y;
            else if(size.Y <= 0)
                size.Y = size.X;
            size *= ImGui.GetCurrentWindow().FontWindowScale * ImGui.GetIO().FontGlobalScale;
        }
        
        var style = ImGui.GetStyle();
        
        string label_str = name;
        
        Vector2 sizeName = ImGui.CalcTextSize(label_str, 0, true);
        char[] name_sz = label_str.ToCharArray();
        
        
        if (sizeName.X > imageSize.X)
        {
            for (var ds = name_sz.Length; ds > 3; --ds)
            {
                sizeName = ImGui.CalcTextSize(label_str, 0, true);
                if(sizeName.X < imageSize.X)
                {
                    name_sz[ds - 2] = '.';
                    name_sz[ds - 1] = '.';
                    name_sz[ds] = '.';
                    break;
                }
            }
        }
        
        string label = CharToString(name_sz);
        
        
        var id = ImGui.GetID(label);
        
        var textSize = ImGui.CalcTextSize(label, 0, true);
        var hasText = textSize.Length() > 0;
        
        var innerSpacing = hasText ? ((frame_padding >= 0) ? (float)frame_padding : (style.ItemInnerSpacing.X)) : 0f;
        var padding =
            (frame_padding >= 0) ? new Vector2((float)frame_padding, (float)frame_padding) : style.FramePadding;
        
        
        bool istextBig = textSize.X > imageSize.X;
        
        var totalSizeWithoutPadding = new Vector2(size.X, size.Y > textSize.Y ? size.Y : textSize.Y);
        
        ImRect bb = new(//     
        )
        {
            Min = ImGui.GetCursorScreenPos(),
            Max = ImGui.GetCursorScreenPos() + totalSizeWithoutPadding + padding * 2
        };
        
        var start = new Vector2(0, 0);
        
        start = ImGui.GetCursorScreenPos() + padding;
        
        if(size.Y < textSize.Y)
        {
            start.Y += (textSize.Y - size.Y) * .5f;
        }
        
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
            Min = start + reajustMIN,
            Max = start + reajustMAX
        };
        
        start = ImGui.GetCursorScreenPos();
        start.Y += (size.Y - textSize.Y) + 2;
        if(istextBig == false)
        {
            start.X += (size.X - textSize.X) * .5f;
        }
        start.Y += (size.Y - textSize.Y) + 2;
        
        bool hovered = false;
        bool held = false;
        
        bool pressed = ImGui.ButtonBehavior(bb, id, ref hovered, ref held);
        
        var col = ImGui.GetColorU32((hovered && held) ? ImGuiCol.ButtonActive
            : hovered ? ImGuiCol.ButtonHovered : ImGuiCol.Button);
        
        ImGui.RenderFrame(bb.Min, bb.Max, col, true,
            Math.Clamp((float)MathF.Min(padding.X, padding.Y), 0.0f, style.FrameRounding));
        
        if (bg_col.W > 0.0f)
        {
            //ImGui.GetWindowDrawList().AddRectFilled(image_bb.Min, image_bb.Min, ImGui.GetColorU32(bg_col));
        }
        
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
        ImGui.GetWindowDrawList().AddImage(texId, 
            image_bb.Min, image_bb.Max, uv0, uv1, ImGui.GetColorU32(tint_col));
        
        if(textSize.X > 0)
        {
            ImGui.RenderText(start, label);
        }
    }


    private static float GetWidth(ImRect imageBb)
    {
        return (imageBb.Max.X - imageBb.Min.X);
        
    }
    
    
    private static float GetHeight(ImRect imageBb)
    {
        return imageBb.Max.Y - imageBb.Min.Y;
    }

    private static string CharToString(char[] chars)
    {
        string text = "";

        for (int i = 0; i < chars.Length; i++)
        {
            text += chars[i];
        }
        
        return text;
    }
    static Vector2  GetSize(ImRect rect)   { return new Vector2(rect.Max.X - rect.Min.X, rect.Max.Y - rect.Min.Y); }

}