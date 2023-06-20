using System.Numerics;
using System.Runtime.CompilerServices;
using Engine2D.Cameras;
using Engine2D.Testing;
using ImGuiNET;

namespace Engine2D.UI.Viewports;

public abstract class ViewportWindow
{
    protected Canvas mCanvas = new();
    
    protected Vector2 Origin;
    protected Vector2 Sz;

    public Vector2 WindowSize { get; private set; }
    public Vector2 WindowPos { get; set; }

    protected Camera Camera;
    protected TestFrameBuffer? _frameBuffer;
    public Vector2 gameViewportSize => WindowSize;
    public Vector2 gameViewportPos => WindowPos;

    public virtual void Begin(string title, Camera? cameraToRender, TestFrameBuffer? buffer)
    {
        Camera = cameraToRender;
        _frameBuffer = buffer;
        
        ImGui.Begin(title);
        BeforeImageRender();
        RenderImage();
        AfterImageRender();
        End();
    }

    public abstract void BeforeImageRender();
    
    public virtual void RenderImage()
    {
        ImGui.SetCursorPos(ImGui.GetCursorPos());
        
        WindowSize = new Vector2((int)GetVPSize().X, (int)GetVPSize().Y);
        WindowPos = GetCenteredPositionForViewport(WindowSize);
        
        ImGui.SetCursorPos(new(WindowPos.X, WindowPos.Y));
        
        ImGui.Image(_frameBuffer.TextureID, new(WindowSize.X, WindowSize.Y),
            UISETTINGS.ImageUV0, UISETTINGS.ImageUV1);
    
        SetCanvasData(new TileExtent());

        Origin = ImGui.GetItemRectMin();
        Sz = ImGui.GetItemRectSize();
    }

    private void SetCanvasData(TileExtent extent)
    {
        mCanvas.top_left =     ImGui.GetItemRectMin();
        mCanvas.bottom_right = mCanvas.top_left + (ImGui.GetItemRectSize());
        mCanvas.size =         mCanvas.bottom_right - mCanvas.top_left;

        mCanvas.origin = mCanvas.top_left + (new Vector2(0, 0));

        mCanvas.tile_size = new Vector2(1,1);
        // mCanvas.grid_size = new Vector2(32,32);
        mCanvas.ratio     = mCanvas.grid_size / mCanvas.tile_size;

        var tiles_in_viewport = mCanvas.size / mCanvas.grid_size;
        mCanvas.tiles_in_viewport_x = (int)(tiles_in_viewport.X) + 1;
        mCanvas.tiles_in_viewport_y = (int)(tiles_in_viewport.Y) + 1;

        mCanvas.row_count    = extent.rows;
        mCanvas.col_count    = extent.cols;
        mCanvas.content_size = new Vector2(mCanvas.col_count, mCanvas.row_count) * mCanvas.grid_size;
    }

    public abstract void AfterImageRender();

    public virtual void End()
    {
        ImGui.End();
    }
    
    
    public abstract Vector2 GetVPSize();

    public OpenTK.Mathematics.Vector2 GetCenteredPositionForViewport(OpenTK.Mathematics.Vector2 aspectSize)
    {
        Vector2 s = GetCenteredPositionForViewport(new Vector2(aspectSize.X, aspectSize.Y));
        return new(s.X, s.Y);
    }

    public Vector2 GetCenteredPositionForViewport(Vector2 aspectSize)
    {
        Vector2 windowSize = ImGui.GetContentRegionAvail();

        float vpX = ((windowSize.X / 2) - (aspectSize.X / 2));
        float vpY = ((windowSize.Y / 2) - (aspectSize.Y / 2));

        return new(vpX + ImGui.GetCursorPos().X, vpY + ImGui.GetCursorPos().Y);
    }    
}

public class Canvas
{
    public Vector2 top_left                          ;///< Top-left point of the canvas.
    public Vector2 bottom_right                      ;///< Bottom-right point of the canvas.
    public Vector2 size                              ;///< Total size of the canvas.
    public Vector2 origin                            ;///< Origin screen position.
    public Vector2 grid_size  = new(32,32)       ;///< Graphical tile size.
    public Vector2 tile_size                         ;///< Logical tile size.
    public Vector2 ratio                             ;///< Graphical tile size divided by logical tile size.
    public Vector2 content_size                      ;///< Graphical size of the map or tileset.
    public int tiles_in_viewport_x                   ;///< Amount of visible tiles in the viewport, x-axis.
    public int tiles_in_viewport_y                   ;///< Amount of visible tiles in the viewport, y-axis.
    public float row_count                           ;///< Total amount of rows.
    public float col_count                           ;///< Total amount of columns.

    public Region bounds;
}

public struct Region
{
    public TilePos begin;  /// The top-left position.
    public TilePos end;    /// The bottom-right position.
}

public class TilePos
{
    public int row;
    public int col;
    private int mRow;
    private int mCol;
}



public struct TileExtent {
    public int rows=1000;
    public int cols=1000;

    public TileExtent(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(TileExtent left, TileExtent right)
    {
        return
            (left.rows == right.rows &&
             left.cols == right.cols);

    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(TileExtent left, TileExtent right)
    {
        return
            (left.rows != right.rows ||
             left.cols != right.cols);

    }
}