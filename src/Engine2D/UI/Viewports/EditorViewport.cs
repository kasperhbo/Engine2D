using Engine2D.Cameras;
using Engine2D.Core;
using Engine2D.Testing;
using ImGuiNET;
using OpenTK.Mathematics;
using Vector2 = System.Numerics.Vector2;

namespace Engine2D.UI.Viewports;

public class EditorViewport : ViewportWindow
{    
    
    public EditorViewport()
    {
        
         Engine.Get().CurrentScene.Renderer.EditorGameBuffer = new TestFrameBuffer(new Vector2i(1920,1080));
    }
    
    public override void OnGui(Camera? cameraToRender, string title)
    {
        ImGui.Begin(title);
        {
            var viewportOffset = ImGui.GetCursorPos(); // includes tab bar
            Vector2 viewportSize = ImGui.GetContentRegionAvail();
            
            Vector2i vp = new OpenTK.Mathematics.Vector2i((int)viewportSize.X, (int)viewportSize.Y);
            
            ImGui.Image(Engine.Get().CurrentScene.Renderer.EditorGameBuffer.TextureID, new(vp.X, vp.Y),
                new(0, 1), new(1, 0));
            
            if (Engine.Get().CurrentScene.Renderer.EditorGameBuffer.Size.X !=  vp.ToVector2().X ||
                Engine.Get().CurrentScene.Renderer.EditorGameBuffer.Size.Y != vp.ToVector2().Y)
            {
                cameraToRender.AdjustProjection(vp.X, vp.Y);
            }
        }
        
        ImGui.End();

    }
    public override Vector2 GetVPSize()
    {
        return ImGui.GetContentRegionAvail();
    }
}