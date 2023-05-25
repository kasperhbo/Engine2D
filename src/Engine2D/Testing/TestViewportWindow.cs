using System.Drawing.Drawing2D;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Engine2D.Components;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Rendering;
using Engine2D.UI;
using GlmSharp;
using ImGuiNET;
using ImGuizmoNET;
using KDBEngine.Core;
using OpenTK.Mathematics;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector2 = System.Numerics.Vector2;

namespace Engine2D.Testing;

internal class TestViewportWindow
{
    private static Vector2 viewportPos;
    public static Vector2 ViewportSize;

    private TestFrameBuffer frameBufferToRenderer = null;
    
    public void OnGui(Renderer renderer)
    {
        ImGui.Begin("Game Viewport", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse
                                                                  | ImGuiWindowFlags.MenuBar);

        ImGui.BeginMenuBar();
        if (!Engine.Get()._currentScene.IsPlaying)
        {
            if (ImGui.MenuItem("Play")) Engine.Get()._currentScene.IsPlaying = true;
        }
        else
        {
            if (ImGui.MenuItem("Stop")) Engine.Get()._currentScene.IsPlaying = false;
        }

        ImGui.EndMenuBar();

        var windowSize = getLargestSizeForViewport();
        var windowPos = getCenteredPositionForViewport(windowSize);

        var cam = Engine.Get()._currentScene.EditorCamera;
        if(cam.projectionSize != (windowSize.X, windowSize.Y))
            cam.adjustProjection((windowSize.X, windowSize.Y));
        
        //Vector2 topLeft = new Vector2();
        var topLeft = ImGui.GetCursorScreenPos();
        topLeft.X -= ImGui.GetScrollX();
        topLeft.Y -= ImGui.GetScrollY();

        viewportPos = new Vector2(topLeft.X, topLeft.Y);
        ViewportSize = new Vector2(windowSize.X, windowSize.Y);

        TestInput.setViewportPos(new OpenTK.Mathematics.Vector2(topLeft.X, topLeft.Y));
        TestInput.setViewportSize(new OpenTK.Mathematics.Vector2(windowSize.X, windowSize.Y));

        
        ImGui.Image((IntPtr)renderer.GameBuffer.GetTextureID, new Vector2(windowSize.X, windowSize.Y),
            Vector2.UnitY, Vector2.UnitX);

        //ImGuizmo
        Gameobject go = (Gameobject)Engine.Get().CurrentSelectedAsset;
        if (go != null)
        {
            {
                ImGuizmo.Enable(true);
                ImGuizmo.SetOrthographic(true);
                ImGuizmo.SetDrawlist();
                ImGuizmo.SetRect(ImGui.GetWindowPos().X, ImGui.GetWindowPos().Y,windowSize.X, windowSize.Y);
                
                var operation = OPERATION.SCALE;
                
                Matrix4 view = cam.getViewMatrix();
                Matrix4 projection = cam.getProjectionMatrix();

                Matrix4 translation = go.Transform.GetTranslation(new Vector2(0,0));
                
                 ImGuizmo.Manipulate(ref view.Row0.X, ref projection.Row0.X, 
                     operation, MODE.WORLD, ref translation.Row0.X);
                
                 
                 if (ImGuizmo.IsUsing())
                 {
                     go.Transform.position = new(translation.ExtractTranslation().X, translation.ExtractTranslation().Y+50);
                     go.Transform.size = new(translation.ExtractScale().X, translation.ExtractScale().Y);
                 }
                 
                 ImGui.Begin("t");
                 ImGui.End();
            }
        }

        ImGui.End();
    }

    private Vector2 getCenteredPositionForViewport(Vector2 aspectSize)
    {
        var windowSize = ImGui.GetContentRegionAvail();
        windowSize.X -= ImGui.GetScrollX();
        windowSize.Y -= ImGui.GetScrollY();

        var viewportX = windowSize.X / 2.0f - aspectSize.X / 2.0f;
        var viewportY = windowSize.Y / 2.0f - aspectSize.Y / 2.0f;

        return new Vector2(viewportX + ImGui.GetCursorPosX(),
            viewportY + ImGui.GetCursorPosY());
    }

    public static Vector2 getLargestSizeForViewport()
    {
        var windowSize = ImGui.GetContentRegionAvail();
        windowSize.X -= ImGui.GetScrollX();
        windowSize.Y -= ImGui.GetScrollY();

        var aspectWidth = windowSize.X;
        var aspectHeight = aspectWidth / Engine.Get().TargetAspectRatio();
        if (aspectHeight > windowSize.Y)
        {
            // We must switch to pillarbox mode
            aspectHeight = windowSize.Y;
            aspectWidth = aspectHeight * Engine.Get().TargetAspectRatio();
        }

        return new Vector2(aspectWidth, aspectHeight);
    }

    public bool IsInViewport(Vector2 other)
    {
        return
               other.X >= viewportPos.X
            && other.X <= viewportPos.X + ViewportSize.X
            && other.Y >= viewportPos.Y
            && other.Y <= viewportPos.Y + ViewportSize.Y;
    }
}