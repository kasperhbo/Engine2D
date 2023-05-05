using Engine2D.Rendering;
using Engine2D.Scenes;
using ImGuiNET;
using KDBEngine.Core;
using OpenTK.Compute.OpenCL;
using System.Numerics;

namespace Engine2D.UI
{
    internal class GameViewport
    {
        private static bool isPlaying = false;
        private static float leftX, rightX, topY, bottomY;
        public static bool IsInViewport { get; private set; } = false;

        internal void OnGui(int TextureID)
        {
            ImGui.Begin("Game Viewport", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);

            Vector2 windowSize = getLargestSizeForViewport();
            Vector2 windowPos = getCenteredPositionForViewport(windowSize);

            ImGui.SetCursorPos(new Vector2(windowPos.X, windowPos.Y));            
            ImGui.Image((IntPtr)TextureID, new Vector2(windowSize.X, windowSize.Y), new Vector2(0, 1), new Vector2(1, 0));

            ImGui.End();

        }

        private static Vector2 getLargestSizeForViewport()
        {
            Vector2 windowSize = ImGui.GetContentRegionAvail();
            
            windowSize.X -= ImGui.GetScrollX();
            windowSize.Y -= ImGui.GetScrollY();

            float aspectWidth = windowSize.X;
            float aspectHeight = aspectWidth / Engine.Get().TargetAspectRatio;
            if (aspectHeight > windowSize.Y)
            {
                // We must switch to pillarbox mode
                aspectHeight = windowSize.Y;
                aspectWidth = aspectHeight * Engine.Get().TargetAspectRatio;
            }

            return new Vector2(aspectWidth, aspectHeight);
        }

        private static Vector2 getCenteredPositionForViewport(Vector2 aspectSize)
        {
            Vector2 windowSize = ImGui.GetContentRegionAvail();            
            windowSize.X -= ImGui.GetScrollX();
            windowSize.Y -= ImGui.GetScrollY();

            float viewportX = (windowSize.X / 2.0f) - (aspectSize.X / 2.0f);
            float viewportY = (windowSize.Y / 2.0f) - (aspectSize.Y / 2.0f);

            return new Vector2(viewportX + ImGui.GetCursorPosX(),
                    viewportY + ImGui.GetCursorPosY());
        }
    }
}
