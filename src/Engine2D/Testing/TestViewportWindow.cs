using System.Numerics;
using System.Runtime.InteropServices;
using Engine2D.Core;
using Engine2D.Rendering;
using ImGuiNET;
using KDBEngine.Core;

namespace Engine2D.Testing;

internal class TestViewportWindow
{
    private static Vector2 viewportPos;
    public static Vector2 ViewportSize;

    private TestFrameBuffer frameBufferToRenderer = null;


    public void OnGui()
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

        //Vector2 topLeft = new Vector2();
        var topLeft = ImGui.GetCursorScreenPos();
        topLeft.X -= ImGui.GetScrollX();
        topLeft.Y -= ImGui.GetScrollY();

        viewportPos = new Vector2(topLeft.X, topLeft.Y);
        ViewportSize = new Vector2(windowSize.X, windowSize.Y);


        // var textureId = GameRenderer.FrameBufferToRenderer();
        if (Renderer.GameBuffer != null)
            ImGui.Image((IntPtr)Renderer.GameBuffer.GetTextureID, new Vector2(windowSize.X, windowSize.Y),
                new Vector2(0, 1), new Vector2(1, 0));
        else
            //TODO: MAKE ERROR TEXTUYRE
            ImGui.Image((IntPtr)0, new Vector2(windowSize.X, windowSize.Y),
                new Vector2(0, 1), new Vector2(1, 0));

        if (ImGui.BeginDragDropTarget())
        {
            var payload = ImGui.AcceptDragDropPayload("Scene_Drop");
            if (payload.IsValidPayload())
            {
                var filename = (string)GCHandle.FromIntPtr(payload.Data).Target;
                Console.WriteLine("Opening scene: " + filename);
                //Window.Get().ChangeScene(new LevelEditorScene(), filename);
            }

            ImGui.EndDragDropTarget();
        }

        TestInput.setViewportPos(new OpenTK.Mathematics.Vector2(topLeft.X, topLeft.Y));
        TestInput.setViewportSize(new OpenTK.Mathematics.Vector2(windowSize.X, windowSize.Y));

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
        var aspectHeight = aspectWidth / Engine.Get().getTargetAspectRatio();
        if (aspectHeight > windowSize.Y)
        {
            // We must switch to pillarbox mode
            aspectHeight = windowSize.Y;
            aspectWidth = aspectHeight * Engine.Get().getTargetAspectRatio();
        }

        return new Vector2(aspectWidth, aspectHeight);
    }

    public static bool IsMouseInsideViewport()
    {
        return
            TestInput.getX() >= viewportPos.X
            && TestInput.getX() <= viewportPos.X + ViewportSize.X
            && TestInput.getY() >= viewportPos.Y
            && TestInput.getY() <= viewportPos.Y + ViewportSize.Y;
    }
}