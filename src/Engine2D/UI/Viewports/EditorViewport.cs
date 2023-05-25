using Engine2D.GameObjects;
using ImGuiNET;
using ImGuizmoNET;
using KDBEngine.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector2 = System.Numerics.Vector2;

namespace Engine2D.UI.Viewports;

public class EditorViewport : ViewportWindow
{
    
    OPERATION _currentOperation = OPERATION.TRANSLATE;
    MODE _currentMode = MODE.WORLD;

    private Vector2 _gridStart = new();
    
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

        _gridStart = ImGui.GetCursorScreenPos();
    }

    protected override void AfterImageRender(bool recieveInput = true)
    {
        base.AfterImageRender();
        
        // //Draw grid
        // ImGui.GetWindowDrawList().AddLine(_gridStart, new(_gridStart.X + 50, _gridStart.Y + 50), 
        //     ImGui.GetColorU32(ImGuiCol.Text), 3.0f);
        
        //Draw ImGuizmo
        Gameobject go = (Gameobject)Engine.Get().CurrentSelectedAsset;
        if (go != null)
        {
           
            ImGuizmo.Enable(true);
            ImGuizmo.SetOrthographic(true);
            ImGuizmo.SetDrawlist();

            origin = ImGui.GetItemRectMin() ;
            sz =  ImGui.GetItemRectSize();
            
            ImGuizmo.SetRect(origin.X, origin.Y, sz.X, sz.Y);
            
            Matrix4 view = _cameraToRender.getViewMatrix();
            Matrix4 projection = _cameraToRender.getProjectionMatrix();
            Matrix4 translation = go.Transform.GetTranslation();
            
            ImGuizmo.Manipulate(ref view.Row0.X, ref projection.Row0.X, 
                _currentOperation, _currentMode, ref translation.Row0.X);
            if(recieveInput)
            {
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
                    
                    go.Transform.size = new(translation.ExtractScale().X, translation.ExtractScale().Y);
                    go.Transform.SetRotation(translation.ExtractRotation());
                }
            }
        }
    }
}