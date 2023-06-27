using Engine2D.Cameras;
using Engine2D.Components.TransformComponents;
using Engine2D.GameObjects;
using Engine2D.UI;
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
}