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
    
    internal override void BeforeImageRender()
    {
        // Camera.ProjectionSize = new Vector2(1920, 1080);
    }

    internal override void AfterImageRender()
    {
        Input.CalculateMouseEditor(this, Camera);
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
                
                if (Camera?.CameraType == CameraTypes.ORTHO)
                    ImGuizmo.SetOrthographic(true);
        
                ImGuizmo.SetDrawlist();
        
                var pos = ImGui.GetCursorStartPos();
                
                //Set rect for imgui
                ImGuizmo.SetRect(Origin.X, Origin.Y, Size.X, Size.Y);
        
                var view = Camera.GetViewMatrix();
                var projection = Camera.GetProjectionMatrix();
                var translation = Matrix4x4.Identity;
                
                if(selectedGo.GetComponent<SpriteRenderer>() == null)
                    translation = selectedGo.GetComponent<Transform>().GetTranslation();
                else if (selectedGo.GetComponent<SpriteRenderer>().Sprite != null)
                {
                    SpriteRenderer spr = selectedGo.GetComponent<SpriteRenderer>();
                    translation = selectedGo.GetComponent<Transform>()
                        .GetTranslation(spr.Sprite.Width, spr.Sprite.Height);
                }
                else
                {
                    translation = selectedGo.GetComponent<Transform>().GetTranslation();
                }
                
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
            Console.WriteLine("Something went wrong");
        }
    }

    private void DrawGrid()
    {
        // DrawImGuiGrid();
    }

    public EditorViewport(string title) : base(title)
    {
    }
}