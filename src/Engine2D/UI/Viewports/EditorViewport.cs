using Engine2D.GameObjects;
using ImGuiNET;
using ImGuizmoNET;
using KDBEngine.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vulkan;
using MathHelper = Vortice.Mathematics.MathHelper;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;


namespace Engine2D.UI.Viewports;

public class EditorViewport : ViewportWindow
{
    
    OPERATION _currentOperation = OPERATION.TRANSLATE;
    
    public EditorViewport(string editorVp) : base(editorVp)
    {
        
    }

    protected override void BeforeImageRender()
    {
        base.BeforeImageRender();
        if (_cameraToRender != null)
        {
            if (_cameraToRender.projectionSize != (_windowSize.X, _windowSize.Y))
                _cameraToRender.adjustProjection((_windowSize.X, _windowSize.Y));
        }
    }

    protected override void AfterImageRender()
    {
        base.AfterImageRender();
        //_cameraToRender.Transform.SetRotationByDegrees(new OpenTK.Mathematics.Vector3(90,180,90));

        Gameobject go = (Gameobject)Engine.Get().CurrentSelectedAsset;
        if (go != null)
        {
           
            ImGuizmo.Enable(true);
            ImGuizmo.SetOrthographic(true);
            ImGuizmo.SetDrawlist();

            var origin = ImGui.GetItemRectMin();
            var sz = ImGui.GetItemRectSize();
            
            ImGuizmo.SetRect(origin.X, origin.Y, sz.X, sz.Y);
            
            Matrix4 view = _cameraToRender.getViewMatrix();
            Matrix4 projection = _cameraToRender.getProjectionMatrix();

            Matrix4 translation = go.Transform.GetTranslation();
            
            ImGuizmo.Manipulate(ref view.Row0.X, ref projection.Row0.X, 
                _currentOperation, MODE.WORLD, ref translation.Row0.X);

            if (Engine.Get().IsKeyPressed(Keys.Q))
            {
                _currentOperation = OPERATION.BOUNDS;
            }
            
            if (Engine.Get().IsKeyPressed(Keys.W))
            {
                _currentOperation = OPERATION.TRANSLATE;
            }
            
            if (Engine.Get().IsKeyPressed(Keys.E))
            {
                _currentOperation = OPERATION.SCALE;
            }
            
            if (Engine.Get().IsKeyPressed(Keys.R))
            {
                _currentOperation = OPERATION.ROTATE;
            }

            if (ImGuizmo.IsUsing())
            {
                go.Transform.position = new(translation.ExtractTranslation().X, 
                    translation.ExtractTranslation().Y);
                _cameraToRender.Transform.position = go.Transform.position;
                go.Transform.size = new(translation.ExtractScale().X, translation.ExtractScale().Y);
                go.Transform.SetRotation(translation.ExtractRotation());
            }
        }
    }
}