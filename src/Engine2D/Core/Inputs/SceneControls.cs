using System.Numerics;
using Engine2D.Cameras;
using Engine2D.Components.TransformComponents;
using Engine2D.GameObjects;
using Engine2D.UI;
using ImGuiNET;
using ImGuizmoNET;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SixLabors.ImageSharp.Formats.Gif;

namespace Engine2D.Core.Inputs;

public static class SceneControls
{
    public static void Update(FrameEventArgs args)
    {
       KeyControls();
       MouseControls();
    }
    
    private static void KeyControls(){
        Camera editorCamera = Engine.Get().CurrentScene.EditorCamera; 
        
        if (Engine.Get().CurrentSelectedAsset != null)
        {
            if (Input.KeyPressed(Keys.Delete))
            {
                Console.WriteLine("Try to delete asset");

                Engine.Get().CurrentSelectedAsset.IsDead = true;
            }
            
            if(editorCamera != null)
            {
                var go = (Gameobject)Engine.Get().CurrentSelectedAsset;
                if (Input.KeyPressed(Keys.F))
                {
                    if (Input.KeyDown(Keys.LeftControl))
                    {
                        go.Transform.Position = editorCamera.Parent.Transform.Position;
                    }
                    else
                    {
                         editorCamera.Parent.Transform.Position = go.Transform.Position;
                    }
                }

                if (Input.KeyDown(Keys.LeftControl))
                {
                    if (Input.KeyPressed(Keys.C))
                    {
                        Gameobject go2 = (Gameobject)go.Clone();
                        go2.Name = go.Name + " Clone";
                        Engine.Get().CurrentScene.AddGameObjectToScene(go2);
                    }
                }
            }
        }


    }

    private static void MouseControls()
    {
        if (Input.MousePressed(MouseButton.Left))
        {
            if(UiRenderer.CurrentEditorViewport.GetWantCaptureMouse())
            {
                Console.WriteLine("want captuse mouse");
                var mouseScreenPos = Input.MouseEditorPos;
                Console.WriteLine("mouseScreenPos: " + mouseScreenPos);
                for (int i = 0; i < Engine.Get().CurrentScene.GameObjects.Count; i++)
                {
                    var go = Engine.Get().CurrentScene.GameObjects[i];
                    if (go.AABB(mouseScreenPos.X, mouseScreenPos.Y))
                    {
                        Console.WriteLine(go.Name);
                        Engine.Get().CurrentSelectedAsset = go;
                        break;
                    }
                }
            }
        }
    }


    private static MODE _currentMode = MODE.WORLD;
    private static OPERATION _currentOperation = OPERATION.TRANSLATE;
    
    internal static void GuizmoControls(Vector2 vpOrigin, Vector2 vpSize, Camera vpCam)
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
                
                if (vpCam?.CameraType == CameraTypes.ORTHO)
                    ImGuizmo.SetOrthographic(true);
        
                ImGuizmo.SetDrawlist();
        
                var pos = ImGui.GetCursorStartPos();
                
                //Set rect for imgui
                ImGuizmo.SetRect(vpOrigin.X, vpOrigin.Y, vpSize.X, vpSize.Y);
        
                var view = vpCam.GetViewMatrix();
                var projection = vpCam.GetProjectionMatrix();
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
                    
                    if(Input.KeyDown(Keys.LeftShift))
                    {
                        //Snap to grid
                        outPos.X = (int)(outPos.X / Settings.GRID_WIDTH) * Settings.GRID_WIDTH;
                        outPos.Y = (int)(outPos.Y / Settings.GRID_HEIGHT) * Settings.GRID_HEIGHT;
                    }
                    
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
}