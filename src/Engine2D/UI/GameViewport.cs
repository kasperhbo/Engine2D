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

        internal void OnGui()
        {
            ImGui.Begin("Game Viewport", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);

            Vector2 windowSize = getLargestSizeForViewport();
            Vector2 windowPos = getCenteredPositionForViewport(windowSize);

            ImGui.SetCursorPos(new Vector2(windowPos.X, windowPos.Y));
            int textureId = Engine.Get()._currentScene.FrameBuffer.TextureID;
            ImGui.Image((IntPtr)textureId, new Vector2(windowSize.X, windowSize.Y), new Vector2(0, 1), new Vector2(1, 0));

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

        //private static Vector2 GetLargestSizeForViewport()
        //{
        //    Vector2 windowSize = ImGui.GetContentRegionAvail();

        //    float aspectWidth = windowSize.X;
        //    float aspectHeight = aspectWidth / Window.Get().TargetAspectRatio;

        //    if (aspectHeight > windowSize.Y)
        //    {
        //        // We must switch to pillarbox mode
        //        aspectHeight = windowSize.Y;
        //        aspectWidth = aspectHeight * Window.Get().TargetAspectRatio;
        //    }

        //    return new Vector2(aspectWidth, aspectHeight);
        //}

        //private static Vector2 GetCenteredPosForViewport(Vector2 aspectSize)
        //{
        //    Vector2 windowSize = new Vector2();

        //    windowSize = ImGui.GetContentRegionAvail();

        //    float viewPortX = (windowSize.X / 2.0F) - (aspectSize.X / 2.0F);
        //    float viewPortY = (windowSize.Y / 2.0F) - (aspectSize.Y / 2.0F);

        //    return new Vector2(viewPortX + ImGui.GetCursorPosX(), viewPortY + ImGui.GetCursorPosY());
        //}
    }


    }
