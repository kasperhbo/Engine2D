using System.Drawing.Drawing2D;
using System.Numerics;
using Engine2D.Cameras;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.GameObjects;
using ImGuiNET;
using ImGuizmoNET;
using ImPlotNET;
using Quaternion = System.Numerics.Quaternion;
using Vector2 = System.Numerics.Vector2;

namespace Engine2D.UI.Viewports;

public class EditorViewport : ViewportWindow
{
    public EditorViewport()
    {
    }
    
    public override void BeforeImageRender()
    {
    }
    
    public override void AfterImageRender()
    {
        if (Camera == null) return;
        Camera.AdjustProjection(1920,1080);
        // if (Camera.ProjectionSize.X != WindowSize.ToVector2().X ||
        //     Camera.ProjectionSize.Y != WindowSize.ToVector2().Y)
        // {
        //     Camera.AdjustProjection(WindowSize.X, WindowSize.Y);
        // }
        Lines();
        Guizmo();
    }
    
    OPERATION _currentOperation = OPERATION.TRANSLATE;
    MODE _currentMode = MODE.WORLD;

    private void Lines()
    {
        ImPlot.ShowDemoWindow();
        // float[] xs1 = new float[1001];
        // float[] ys1 = new float[1001];
        //
        // for (int i = 0; i < 1001; ++i) {
        //     xs1[i] = i * 0.001f;
        //     ys1[i] = i * 0.01f;;
        // }
        //
        // double[] xs2 = new double[20];
        // double[] ys2 = new double[20];
        //
        // for (int i = 0; i < 20; ++i) {
        //     xs2[i] = i * 1/19.0f;
        //     ys2[i] = xs2[i] * xs2[i];
        // }

        //ImPlot.CreateContext();

        // if (ImPlot.BeginPlot("Line Plots")) {
        //     float y = 0;
        //     float x = 100;
        //     var min = new ImPlotPoint();
        //     var max = new ImPlotPoint();
        //     
        //     min.x = 0;
        //     min.y = 0;
        //     max.x = WindowSize.X;
        //     max.y = WindowSize.Y;
        //     
        //     ImPlot.PlotImage("vp", _frameBuffer.TextureID, min, max);
        //     // ImPlot.PlotLine("f(x)", ref x, ref y, 1);
        //     // ImPlot.SetNextMarkerStyle(ImPlotMarker_Circle);
        //     // ImPlot.PlotLine("g(x)", ref xs2, ref ys2, 20);
        //     ImPlot.EndPlot();
        // }
    }
    
    private void Guizmo()
    {
        try
        {
            Gameobject selectedGo = (Gameobject)Engine.Get().CurrentSelectedAsset;


            if (selectedGo != null)
            {
                ImGuizmo.Enable(true);

                if (Camera.CameraType == CameraTypes.ORTHO)
                    ImGuizmo.SetOrthographic(true);

                ImGuizmo.SetDrawlist();

                var pos = ImGui.GetCursorStartPos();

                ImGuizmo.SetRect(Origin.X, Origin.Y, Sz.X, Sz.Y);

                Matrix4x4 view = Camera.GetViewMatrix();
                Matrix4x4 projection = Camera.GetProjectionMatrix();
                Matrix4x4 translation = selectedGo.Transform.GetTranslation();
        
                CreateGrid(projection, view);
                
                
                ImGuizmo.Manipulate(ref view.M11, ref projection.M11,
                    _currentOperation, _currentMode, ref translation.M11);

                
                if (ImGuizmo.IsUsing())
                {
                    Matrix4x4.Decompose(translation, out System.Numerics.Vector3 outScale,
                        out Quaternion q, out System.Numerics.Vector3 outPos);

                    EulerDegrees deg = new EulerDegrees(q);

                    if (_currentOperation == OPERATION.TRANSLATE)
                        selectedGo.Transform.Position = new(outPos.X, outPos.Y);
                    if (_currentOperation == OPERATION.ROTATE)
                        selectedGo.Transform.Rotation.SetRotation(deg);
                    if (_currentOperation == OPERATION.SCALE)
                        selectedGo.Transform.Size = new(outScale.X, outScale.Y);
                }

            }
        }
        catch
        {
        }
    }

    private void CreateGrid(Matrix4x4 projection, Matrix4x4 view)
    {
        
    }

    public override Vector2 GetVPSize()
    {
        var windowSize = ImGui.GetContentRegionAvail();
        windowSize.X -= ImGui.GetScrollX();
        windowSize.Y -= ImGui.GetScrollY();
        float targetAspectRatio = 16/9;
        
        if(Camera != null)
            targetAspectRatio = Camera.ProjectionSize.X / Camera.ProjectionSize.Y;
        
        var aspectWidth = windowSize.X;
        var aspectHeight = aspectWidth / targetAspectRatio;
        if (aspectHeight > windowSize.Y)
        {
            // We must switch to pillarbox mode
            aspectHeight = windowSize.Y;
            aspectWidth = aspectHeight * targetAspectRatio;
        }

        return new Vector2(aspectWidth, aspectHeight);

    }
}