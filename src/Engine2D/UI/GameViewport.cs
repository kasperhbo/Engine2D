using Engine2D.Core;
using Engine2D.Rendering;
using Engine2D.Scenes;
using ImGuiNET;
using KDBEngine.Core;
using OpenTK.Compute.OpenCL;
using System.Numerics;
using System.Runtime.Intrinsics.X86;

namespace Engine2D.UI
{
    internal class GameViewport
    {
        private static bool isPlaying = false;
        private static float leftX, rightX, topY, bottomY;
        public static bool IsInViewport { get; private set; } = false;
        
        
        internal unsafe void OnGui(int TextureID, Action actions)
        {
            ImGui.Begin("TEMP MENU");
            
            if (ImGui.Button("PLAY"))
            {
                Engine.Get()._currentScene.IsPlaying = true;
            }
            if (ImGui.Button("STOP"))
            {
                Engine.Get()._currentScene.IsPlaying = false;
            }
            ImGui.DragFloat2("camera pos", ref GameRenderer.S_CurrentCamera.Position);
            ImGui.End();

            ImGui.Begin("Game Viewport", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);

            OpenTK.Mathematics.Vector2 windowSize = getLargestSizeForViewport();
            OpenTK.Mathematics.Vector2 windowPos = getCenteredPositionForViewport(new Vector2(windowSize.X,windowSize.Y));

            ImGui.SetCursorPos(new Vector2(windowPos.X, windowPos.Y));            
            ImGui.Image((IntPtr)TextureID, new Vector2(windowSize.X, windowSize.Y), new Vector2(0, 1), new Vector2(1, 0));

            if (ImGui.BeginDragDropTarget())
            {
                if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                {
                    Engine.LoadScene(AssetBrowser.CurrentDraggingFileName);
                }

                ImGui.EndDragDropTarget();
            }

            Input.GameViewportPos = windowPos with { X = (windowPos.X), Y = windowPos.Y };
            Input.GameViewportSize = windowPos with { X = (windowSize.X), Y = windowSize.Y };

            ImGui.End();

        }

        private static OpenTK.Mathematics.Vector2 getLargestSizeForViewport()
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

            return new OpenTK.Mathematics.Vector2(aspectWidth, aspectHeight);
        }

        private static OpenTK.Mathematics.Vector2 getCenteredPositionForViewport(Vector2 aspectSize)
        {
            Vector2 windowSize = ImGui.GetContentRegionAvail();            
            windowSize.X -= ImGui.GetScrollX();
            windowSize.Y -= ImGui.GetScrollY();

            float viewportX = (windowSize.X / 2.0f) - (aspectSize.X / 2.0f);
            float viewportY = (windowSize.Y / 2.0f) - (aspectSize.Y / 2.0f);

            return new OpenTK.Mathematics.Vector2(viewportX + ImGui.GetCursorPosX(),
                    viewportY + ImGui.GetCursorPosY());
        }
    }
}
