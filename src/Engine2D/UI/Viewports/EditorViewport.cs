using System.Drawing.Drawing2D;
using System.Numerics;
using Engine2D.Cameras;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.Core.Inputs;
using Engine2D.GameObjects;
using Engine2D.Logging;
using ImGuiNET;
using ImGuizmoNET;
using ImPlotNET;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Quaternion = System.Numerics.Quaternion;
using Vector2 = System.Numerics.Vector2;

namespace Engine2D.UI.Viewports;

public class EditorViewport : ViewportWindow
{
    public bool IsWindowHovered = false;
    
    OPERATION _currentOperation = OPERATION.TRANSLATE;
    MODE _currentMode = MODE.WORLD;
    
    public EditorViewport()
    {
    }
    
    public override void BeforeImageRender()
    {
    }
    
    public override void AfterImageRender()
    {
        DrawGrid();
        IsWindowHovered = ImGui.IsItemHovered();
        
        Lines();
        Guizmo();
    }
    
    private void Lines()
    {
        ImPlot.ShowDemoWindow();
    }
    
    private void Guizmo()
    {
        try
        {
            Gameobject selectedGo = (Gameobject)Engine.Get().CurrentSelectedAsset;


            if (selectedGo != null)
            {
                if (Input.KeyPressed(Keys.Q))
                {
                    if(_currentMode == MODE.LOCAL) _currentMode = MODE.WORLD;
                    else if(_currentMode == MODE.WORLD) _currentMode = MODE.LOCAL;
                }

                if (Input.KeyPressed(Keys.R))
                {
                    _currentOperation = OPERATION.ROTATE;
                }

                if (Input.KeyPressed(Keys.E))
                {
                    _currentOperation = OPERATION.SCALE;
                }
                
                if (Input.KeyPressed(Keys.W))
                {
                    _currentOperation = OPERATION.TRANSLATE;
                }
                    
                
                ImGuizmo.Enable(true);

                if (Camera.CameraType == CameraTypes.ORTHO)
                    ImGuizmo.SetOrthographic(true);

                ImGuizmo.SetDrawlist();

                var pos = ImGui.GetCursorStartPos();

                ImGuizmo.SetRect(Origin.X, Origin.Y, Sz.X, Sz.Y);

                Matrix4x4 view = Camera.GetViewMatrix();
                Matrix4x4 projection = Camera.GetProjectionMatrix();
                Matrix4x4 translation = selectedGo.GetComponent<Transform>().GetTranslation();
        
                ImGuizmo.Manipulate(ref view.M11, ref projection.M11,
                    _currentOperation, _currentMode, ref translation.M11);

                
                if (ImGuizmo.IsUsing())
                {
                    Matrix4x4.Decompose(translation, out Vector3 outScale,
                        out Quaternion q, out Vector3 outPos);

                    if (_currentOperation == OPERATION.TRANSLATE)
                        selectedGo.GetComponent<Transform>().Position = new(outPos.X, outPos.Y);
                    
                    if (_currentOperation == OPERATION.ROTATE)
                        selectedGo.GetComponent<Transform>().SetRotation(q);
                    
                    if (_currentOperation == OPERATION.SCALE)
                        selectedGo.GetComponent<Transform>().Size = new(outScale.X, outScale.Y);
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

    public override Vector2 GetVPSize()
    {
        var ws = ImGui.GetContentRegionAvail();

        float targetAspectRatio = 16.0f / 9.0f;
        
        float aspectWidth = ws.X;
        float aspectHeight = aspectWidth / targetAspectRatio;
        
        if (aspectHeight > ws.Y) {
            // We must switch to pillarbox mode
            aspectHeight = ws.Y;
            aspectWidth = aspectHeight * targetAspectRatio;
        }

        return new Vector2(aspectWidth, aspectHeight);
    }

    private void DrawImGuiGrid()
    {
        var color = 4278235392U;
        Matrix4x4 viewProjection = Camera.GetProjectionMatrix() * Camera.GetViewMatrix();

        int gridSize = 1;

        var drawList = ImGui.GetWindowDrawList();

        var origin_tile_pos = new Vector2(
            MathF.Floor(mCanvas.top_left.X / mCanvas.grid_size.X), 
            MathF.Floor(mCanvas.top_left.Y / mCanvas.grid_size.Y));
        
        var origin_col = (int)(origin_tile_pos.X);
        var origin_row = (int)(origin_tile_pos.Y);

        var begin_row = origin_row - 1;
        var begin_col = origin_col - 1;

        var end_row = origin_row + mCanvas.tiles_in_viewport_y + 1;
        var end_col = origin_col + mCanvas.tiles_in_viewport_x + 1;

        mCanvas.grid_size = new Vector2(1, 1);

        // This offset ensures that the rendered grid is aligned over the underlying grid
        Vector2 offset = new 
        (
            (mCanvas.origin.X % mCanvas.grid_size.X),
            (mCanvas.origin.Y % mCanvas.grid_size.Y)
        );

        var end_x = ((float)(end_col) * mCanvas.grid_size.X) + offset.X;
        var end_y = ((float)(end_row) * mCanvas.grid_size.Y) + offset.Y;
        

        for (var row = begin_row; row < end_row; ++row) {
            var row_y = ((float)(row) * mCanvas.grid_size.Y) + offset.Y;

            var pt1 = new Vector2(0, row_y);
            var pt2 = new Vector2(end_x, row_y);
            
            pt1 = Input.screenToWorld(pt1, Camera);
            pt2 = Input.screenToWorld(pt2, Camera);

            
            drawList.AddLine(pt1, pt2, (color));
        }

        int size = 32;
        for (var col = begin_col; col < end_col; col++) {
            var col_x = ((float)(col) * mCanvas.grid_size.X) + offset.X;

            var pt1 = new Vector2(col_x, 0);
            var pt2 = new Vector2(col_x, end_y);

            pt1 = Input.screenToWorld(pt1, Camera);
            pt2 = Input.screenToWorld(pt2, Camera);
            
            drawList.AddLine(pt1, pt2, (color));
        }
    }
}