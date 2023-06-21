#region

using System.Numerics;
using System.Runtime.CompilerServices;
using Engine2D.Cameras;
using Engine2D.Testing;
using ImGuiNET;

#endregion

namespace Engine2D.UI.Viewports;

internal abstract class ViewportWindow
{
    protected TestFrameBuffer? _frameBuffer;

    protected Camera Camera;
    protected Canvas mCanvas = new();

    protected Vector2 Origin;
    protected Vector2 Sz;

    internal Vector2 WindowSize { get; private set; }
    internal Vector2 WindowPos { get; set; }
    internal Vector2 gameViewportSize => WindowSize;
    internal Vector2 gameViewportPos => WindowPos;

    internal virtual void Begin(string title, Camera? cameraToRender, TestFrameBuffer? buffer)
    {
        Camera = cameraToRender;
        _frameBuffer = buffer;

        ImGui.Begin(title);
        BeforeImageRender();
        RenderImage();
        AfterImageRender();
        End();
    }

    internal abstract void BeforeImageRender();

    internal virtual void RenderImage()
    {
        ImGui.SetCursorPos(ImGui.GetCursorPos());

        WindowSize = new Vector2((int)GetVPSize().X, (int)GetVPSize().Y);
        WindowPos = GetCenteredPositionForViewport(WindowSize);

        ImGui.SetCursorPos(new Vector2(WindowPos.X, WindowPos.Y));

        ImGui.Image(_frameBuffer.TextureID, new Vector2(WindowSize.X, WindowSize.Y),
            UISETTINGS.ImageUV0, UISETTINGS.ImageUV1);

        SetCanvasData(new TileExtent());

        Origin = ImGui.GetItemRectMin();
        Sz = ImGui.GetItemRectSize();
    }

    private void SetCanvasData(TileExtent extent)
    {
        mCanvas.top_left = ImGui.GetItemRectMin();
        mCanvas.bottom_right = mCanvas.top_left + ImGui.GetItemRectSize();
        mCanvas.size = mCanvas.bottom_right - mCanvas.top_left;

        mCanvas.origin = mCanvas.top_left + new Vector2(0, 0);

        mCanvas.tile_size = new Vector2(1, 1);
        // mCanvas.grid_size = new Vector2(32,32);
        mCanvas.ratio = mCanvas.grid_size / mCanvas.tile_size;

        var tiles_in_viewport = mCanvas.size / mCanvas.grid_size;
        mCanvas.tiles_in_viewport_x = (int)tiles_in_viewport.X + 1;
        mCanvas.tiles_in_viewport_y = (int)tiles_in_viewport.Y + 1;

        mCanvas.row_count = extent.rows;
        mCanvas.col_count = extent.cols;
        mCanvas.content_size = new Vector2(mCanvas.col_count, mCanvas.row_count) * mCanvas.grid_size;
    }

    internal abstract void AfterImageRender();

    internal virtual void End()
    {
        ImGui.End();
    }


    internal abstract Vector2 GetVPSize();

    internal OpenTK.Mathematics.Vector2 GetCenteredPositionForViewport(OpenTK.Mathematics.Vector2 aspectSize)
    {
        var s = GetCenteredPositionForViewport(new Vector2(aspectSize.X, aspectSize.Y));
        return new OpenTK.Mathematics.Vector2(s.X, s.Y);
    }

    internal Vector2 GetCenteredPositionForViewport(Vector2 aspectSize)
    {
        var windowSize = ImGui.GetContentRegionAvail();

        var vpX = windowSize.X / 2 - aspectSize.X / 2;
        var vpY = windowSize.Y / 2 - aspectSize.Y / 2;

        return new Vector2(vpX + ImGui.GetCursorPos().X, vpY + ImGui.GetCursorPos().Y);
    }
}

internal class Canvas
{
    ///< Top-left point of the canvas.
    internal Vector2 bottom_right;

    ///< Total amount of columns.
    internal Region bounds;

    ///< Total amount of rows.
    internal float col_count;

    ///< Graphical tile size divided by logical tile size.
    internal Vector2 content_size;

    ///< Origin screen position.
    internal Vector2 grid_size = new(32, 32);

    ///< Total size of the canvas.
    internal Vector2 origin;

    ///< Logical tile size.
    internal Vector2 ratio;

    ///< Amount of visible tiles in the viewport, y-axis.
    internal float row_count;

    ///< Bottom-right point of the canvas.
    internal Vector2 size;

    ///< Graphical tile size.
    internal Vector2 tile_size;

    ///< Graphical size of the map or tileset.
    internal int tiles_in_viewport_x;

    ///< Amount of visible tiles in the viewport, x-axis.
    internal int tiles_in_viewport_y;

    internal Vector2 top_left;
}

internal struct Region
{
    internal TilePos begin;

    /// The top-left position.
    internal TilePos end; /// The bottom-right position.
}

internal class TilePos
{
    internal int col;
    private int mCol;
    private int mRow;
    internal int row;
}

internal struct TileExtent
{
    internal int rows = 1000;
    internal int cols = 1000;

    internal TileExtent(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
    }
}