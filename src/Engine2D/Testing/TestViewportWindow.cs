using Engine2D.SavingLoading;
using Engine2D.UI;
using GlmNet;
using ImGuiNET;
using KDBEngine.Core;
using System.Numerics;

namespace Engine2D.Testing
{
    internal class TestViewportWindow
    {
        private static Vector2 viewportPos = new();
        private static Vector2 viewportSize = new();

        public void OnGui()
        {
            ImGui.Begin("Game Viewport", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse
                | ImGuiWindowFlags.MenuBar);

            ImGui.BeginMenuBar();
            if (!Engine.Get()._currentScene.IsPlaying) { 
                if (ImGui.MenuItem("Play"))
                {
                    Engine.Get()._currentScene.IsPlaying = true;
                }
            }
            else { 
                if (ImGui.MenuItem("Stop"))
                {
                    Engine.Get()._currentScene.IsPlaying = false;
                }
            }
            ImGui.EndMenuBar();

            Vector2 windowSize = getLargestSizeForViewport();
            Vector2 windowPos = getCenteredPositionForViewport(windowSize);

            //Vector2 topLeft = new Vector2();
            Vector2 topLeft = ImGui.GetCursorScreenPos();
            topLeft.X -= ImGui.GetScrollX();
            topLeft.Y -= ImGui.GetScrollY();

            //leftX = topLeft.X;
            //bottomY = topLeft.Y;
            //rightX = topLeft.X + windowSize.X;
            //topY = topLeft.Y + windowSize.Y;

            viewportPos = new(topLeft.X, topLeft.Y);
            viewportSize = new(windowSize.X, windowSize.Y);


            int textureId = Engine.Get().getFramebuffer().GetTextureID;
            ImGui.Image((IntPtr)textureId, new Vector2(windowSize.X, windowSize.Y), new( 0, 1), new(1, 0));
            if (ImGui.BeginDragDropTarget())
            {
                if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                {
                    SaveLoad.LoadScene(AssetBrowser.CurrentDraggingFileName);
                }

                ImGui.EndDragDropTarget();
            }


            TestInput.setViewportPos(new OpenTK.Mathematics.Vector2(topLeft.X, topLeft.Y));
            TestInput.setViewportSize(new OpenTK.Mathematics.Vector2(windowSize.X, windowSize.Y));

            ImGui.End();
        }

        private Vector2 getCenteredPositionForViewport(Vector2 aspectSize)
        {            
            Vector2 windowSize = ImGui.GetContentRegionAvail();
            windowSize.X -= ImGui.GetScrollX();
            windowSize.Y -= ImGui.GetScrollY();

            float viewportX = (windowSize.X / 2.0f) - (aspectSize.X / 2.0f);
            float viewportY = (windowSize.Y / 2.0f) - (aspectSize.Y / 2.0f);

            return new (viewportX + ImGui.GetCursorPosX(),
                    viewportY + ImGui.GetCursorPosY());
        }

        private Vector2 getLargestSizeForViewport()
        {
            Vector2 windowSize = ImGui.GetContentRegionAvail();
            windowSize.X -= ImGui.GetScrollX();
            windowSize.Y -= ImGui.GetScrollY();

            float aspectWidth = windowSize.X;
            float aspectHeight = aspectWidth / Engine.Get().getTargetAspectRatio();
            if (aspectHeight > windowSize.Y)
            {
                // We must switch to pillarbox mode
                aspectHeight = windowSize.Y;
                aspectWidth = aspectHeight * Engine.Get().getTargetAspectRatio();
            }

            return new(aspectWidth, aspectHeight);
        }

        public static bool IsMouseInsideViewport()
        {
            return
                   TestInput.getX() >= viewportPos.X
                && TestInput.getX() <= viewportPos.X + viewportSize.X
                && TestInput.getY() >= viewportPos.Y
                && TestInput.getY() <= viewportPos.Y + viewportSize.Y;
        }

    }
}
