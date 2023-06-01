using System.Numerics;
using Engine2D.Cameras;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Testing;
using ImGuiNET;
using ImGuizmoNET;
using OpenTK.Mathematics;
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
        if (Camera.ProjectionSize.X != WindowSize.ToVector2().X ||
            Camera.ProjectionSize.Y != WindowSize.ToVector2().Y)
        {
            Camera.AdjustProjection(WindowSize.X, WindowSize.Y);
        }
        Guizmo();
    }
    
    OPERATION _currentOperation = OPERATION.TRANSLATE;
    MODE _currentMode = MODE.WORLD;
    
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

    public override Vector2 GetVPSize()
    {
        return ImGui.GetContentRegionAvail();
    }
}