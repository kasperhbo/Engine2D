#region

using System.Numerics;
using Engine2D.Cameras;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.Core.Inputs;
using Engine2D.GameObjects;
using ImGuiNET;
using ImGuizmoNET;
using OpenTK.Windowing.GraphicsLibraryFramework;

#endregion

namespace Engine2D.UI.Viewports;

internal class EditorViewport : ViewportWindow
{
    private MODE _currentMode = MODE.WORLD;

    private OPERATION _currentOperation = OPERATION.TRANSLATE;
    internal bool IsWindowHovered;

    internal override void BeforeImageRender()
    {
        Camera.ProjectionSize = GetVPSize();
    }

    internal override void AfterImageRender()
    {
        IsWindowHovered = ImGui.IsItemHovered();

        Guizmo();
    }

    private void Lines()
    {
    }

    private void Guizmo()
    {
        try
        {
            var selectedGo = (Gameobject)Engine.Get().CurrentSelectedAsset;


            if (selectedGo != null)
            {
                if (Input.KeyPressed(Keys.Q))
                {
                    if (_currentMode == MODE.LOCAL) _currentMode = MODE.WORLD;
                    else if (_currentMode == MODE.WORLD) _currentMode = MODE.LOCAL;
                }

                if (Input.KeyPressed(Keys.R)) _currentOperation = OPERATION.ROTATE;

                if (Input.KeyPressed(Keys.E)) _currentOperation = OPERATION.SCALE;

                if (Input.KeyPressed(Keys.W)) _currentOperation = OPERATION.TRANSLATE;


                ImGuizmo.Enable(true);

                if (Camera.CameraType == CameraTypes.ORTHO)
                    ImGuizmo.SetOrthographic(true);

                ImGuizmo.SetDrawlist();

                var pos = ImGui.GetCursorStartPos();

                ImGuizmo.SetRect(Origin.X, Origin.Y, Sz.X, Sz.Y);

                var view = Camera.GetViewMatrix();
                var projection = Camera.GetProjectionMatrix();
                var translation = selectedGo.GetComponent<Transform>().GetTranslation();

                ImGuizmo.Manipulate(ref view.M11, ref projection.M11,
                    _currentOperation, _currentMode, ref translation.M11);


                if (ImGuizmo.IsUsing())
                {
                    Matrix4x4.Decompose(translation, out var outScale,
                        out var q, out var outPos);

                    if (_currentOperation == OPERATION.TRANSLATE)
                        selectedGo.GetComponent<Transform>().Position = new Vector2(outPos.X, outPos.Y);

                    if (_currentOperation == OPERATION.ROTATE)
                        selectedGo.GetComponent<Transform>().SetRotation(q);

                    if (_currentOperation == OPERATION.SCALE)
                        selectedGo.GetComponent<Transform>().Size = new Vector2(outScale.X, outScale.Y);
                }
            }
        }
        catch
        {
        }
    }

    private void DrawGrid()
    {
        // DrawImGuiGrid();
    }

    internal override Vector2 GetVPSize()
    {
        var ws = ImGui.GetContentRegionAvail();

        var targetAspectRatio = 16f / 9f;

        var aspectWidth = ws.X;
        var aspectHeight = aspectWidth / targetAspectRatio;

        if (aspectHeight > ws.Y)
        {
            aspectHeight = ws.Y;
            aspectWidth = aspectHeight * targetAspectRatio;
        }

        return new Vector2(aspectWidth, aspectHeight);
    }

    private void DrawImGuiGrid()
    {
        var color = 4278235392U;
        var viewProjection = Camera.GetProjectionMatrix() * Camera.GetViewMatrix();

        var gridSize = 1;

        var drawList = ImGui.GetWindowDrawList();

        var origin_tile_pos = new Vector2(
            MathF.Floor(mCanvas.top_left.X / mCanvas.grid_size.X),
            MathF.Floor(mCanvas.top_left.Y / mCanvas.grid_size.Y));

        var origin_col = (int)origin_tile_pos.X;
        var origin_row = (int)origin_tile_pos.Y;

        var begin_row = origin_row - 1;
        var begin_col = origin_col - 1;

        var end_row = origin_row + mCanvas.tiles_in_viewport_y + 1;
        var end_col = origin_col + mCanvas.tiles_in_viewport_x + 1;

        mCanvas.grid_size = new Vector2(1, 1);

        // This offset ensures that the rendered grid is aligned over the underlying grid
        Vector2 offset = new
        (
            mCanvas.origin.X % mCanvas.grid_size.X,
            mCanvas.origin.Y % mCanvas.grid_size.Y
        );

        var end_x = end_col * mCanvas.grid_size.X + offset.X;
        var end_y = end_row * mCanvas.grid_size.Y + offset.Y;


        for (var row = begin_row; row < end_row; ++row)
        {
            var row_y = row * mCanvas.grid_size.Y + offset.Y;

            var pt1 = new Vector2(0, row_y);
            var pt2 = new Vector2(end_x, row_y);

            pt1 = Input.screenToWorld(pt1, Camera);
            pt2 = Input.screenToWorld(pt2, Camera);


            drawList.AddLine(pt1, pt2, color);
        }

        var size = 32;
        for (var col = begin_col; col < end_col; col++)
        {
            var col_x = col * mCanvas.grid_size.X + offset.X;

            var pt1 = new Vector2(col_x, 0);
            var pt2 = new Vector2(col_x, end_y);

            pt1 = Input.screenToWorld(pt1, Camera);
            pt2 = Input.screenToWorld(pt2, Camera);

            drawList.AddLine(pt1, pt2, color);
        }
    }
}