using System.Diagnostics;
using System.Numerics;
using Box2D.NetStandard.Common;
using Engine2D.Components.Cameras;
using Engine2D.Components.ENTT;
using ImGuiNET;
using ImGuizmoNET;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine2D.Core.Inputs;

public static class SceneControls
{
    public static void Update(FrameEventArgs args)
    {
        Camera editorCamera = Engine.Get().CurrentScene.GetEditorCamera();
        editorCamera.Update(args.Time);

       KeyControls(args);
       MouseControls();
    }

    private static void KeyControls(FrameEventArgs args)
    {
        
    }

    private static void MouseControls()
    {
       
    }


    private static MODE _currentMode = MODE.WORLD;
    private static OPERATION _currentOperation = OPERATION.TRANSLATE;
    
    /// <summary>
    /// ImGuizmo controls
    /// </summary>
    /// <param name="vpOrigin"></param>
    /// <param name="vpSize"></param>
    /// <param name="vpCam"></param>
    internal static void GuizmoControls(Vector2 vpOrigin, Vector2 vpSize, Camera vpCam)
    {
        try
        {
            var selectedGo = (Entity)Engine.Get().CurrentSelectedAsset;
            
            
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
                
                if (vpCam?.CameraType == CameraTypes.Ortho)
                    ImGuizmo.SetOrthographic(true);
            
                ImGuizmo.SetDrawlist();
            
                var pos = ImGui.GetCursorStartPos();
                
                //Set rect for imgui
                ImGuizmo.SetRect(vpOrigin.X, vpOrigin.Y, vpSize.X, vpSize.Y);
            
                var view = vpCam.GetViewMatrix();
                var projection = vpCam.GetProjectionMatrix();
                
                var transform = selectedGo.GetComponent<ENTTTransformComponent>();
                
                var transformTransform = transform.Transform();
                ImGuizmo.Manipulate(ref view.M11, ref projection.M11,
                    _currentOperation, _currentMode, ref transformTransform.M11);
            
            
                if (ImGuizmo.IsUsing())
                {
                    Matrix4x4.Decompose(transformTransform, out var outScale,
                        out var q, out var outPos);
                    
                    if(Input.KeyDown(Keys.LeftShift))
                    {
                        //Snap to grid
                        outPos.X = (int)(outPos.X / Settings.GRID_WIDTH) * Settings.GRID_WIDTH;
                        outPos.Y = (int)(outPos.Y / Settings.GRID_HEIGHT) * Settings.GRID_HEIGHT;
                    }

                    transform.Position = new Vector2(outPos.X, outPos.Y);
                    transform.Rotation = q;
                    transform.Scale = new Vector2(outScale.X, outScale.Y);
                    
                    if (_currentOperation == OPERATION.TRANSLATE)
                    {
                        selectedGo.SetComponent<ENTTTransformComponent>(transform);
                    }
                    
                    if (_currentOperation == OPERATION.SCALE)
                    {
                        selectedGo.SetComponent<ENTTTransformComponent>(transform);
                    }

                    if (_currentOperation == OPERATION.ROTATE)
                    {
                        selectedGo.SetComponent<ENTTTransformComponent>(transform);
                    }
                    //
                    // if (_currentOperation == OPERATION.SCALE)
                    //     selectedGo.GetComponent<Transform>().Size = new Vector2(outScale.X, outScale.Y);
                }
            }
        }
        catch
        {
            Console.WriteLine("Something went wrong");
        }
    }

}