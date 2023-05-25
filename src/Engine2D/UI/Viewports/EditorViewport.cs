using Engine2D.GameObjects;
using ImGuiNET;
using ImGuizmoNET;
using KDBEngine.Core;
using OpenTK.Mathematics;
using Vulkan;
using Vector2 = System.Numerics.Vector2;


namespace Engine2D.UI.Viewports;

public class EditorViewport : ViewportWindow
{
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
        Gameobject go = (Gameobject)Engine.Get().CurrentSelectedAsset;
        if (go != null)
        {
           
            ImGuizmo.Enable(true);
            ImGuizmo.SetOrthographic(true);
            ImGuizmo.SetDrawlist();
            
            
            ImGuizmo.SetRect(ImGui.GetWindowPos().X + _windowPos.X, ImGui.GetWindowPos().Y,
                _windowSize.X, _windowSize.Y);
            
            Matrix4 view = _cameraToRender.getViewMatrix();
            Matrix4 projection = _cameraToRender.getProjectionMatrix();

            // Matrix4 translation = go.Transform.GetTranslation();
            Matrix4 translation = go.Transform.GetTranslation(new System.Numerics.Vector2(0,
                -_cameraToRender.zoom *50));
            
            var operation = OPERATION.SCALE;
            
            ImGuizmo.Manipulate(ref view.Row0.X, ref projection.Row0.X, 
                operation, MODE.WORLD, ref translation.Row0.X);
            
            if (ImGuizmo.IsUsing())
            {
                go.Transform.position = new(translation.ExtractTranslation().X, 
                    translation.ExtractTranslation().Y + _cameraToRender.zoom * 50);
                
                go.Transform.size = new(translation.ExtractScale().X, translation.ExtractScale().Y);
            }
        }
    }
}