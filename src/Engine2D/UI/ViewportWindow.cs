using System.Numerics;
using System.Runtime.InteropServices;
using Engine2D.Core;
using Engine2D.Rendering;
using ImGuiNET;
using KDBEngine.Core;
using KDBEngine.UI;

namespace Engine2D.Testing;

// public class ViewportWindow : UIElemenet
// {
//     private TestFrameBuffer _frameBufferToRender;
//     private static Vector2 viewportPos;
//     public static Vector2 ViewportSize;
//     
//     public ViewportWindow(string title, ImGuiWindowFlags flags, TestFrameBuffer bufferToRender) : base(title, flags)
//     {
//         _frameBufferToRender = bufferToRender;
//     }
//     
//     protected override Action SetWindowContent()
//     {
//         return () =>
//         {
//             ImGui.PushID(Title);
//             
//             if((_flags & ImGuiWindowFlags.MenuBar) != 0)
//             {
//                 ImGui.BeginMenuBar();
//                 if (!Engine.Get()._currentScene.IsPlaying)
//                 {
//                     if (ImGui.MenuItem("Play")) Engine.Get()._currentScene.IsPlaying = true;
//                 }
//                 else
//                 {
//                     if (ImGui.MenuItem("Stop")) Engine.Get()._currentScene.IsPlaying = false;
//                 }
//
//                 ImGui.EndMenuBar();
//             }
//             
//             var windowSize = getLargestSizeForViewport();
//             var windowPos = getCenteredPositionForViewport(windowSize);
//
//             var topLeft = ImGui.GetCursorScreenPos();
//             topLeft.X -= ImGui.GetScrollX();
//             topLeft.Y -= ImGui.GetScrollY();
//
//             viewportPos = new Vector2(topLeft.X, topLeft.Y);
//             ViewportSize = new Vector2(windowSize.X, windowSize.Y);
//             
//             if(_frameBufferToRender != null)
//             {
//                 ImGui.Image((IntPtr)_frameBufferToRender.Texture.TexID, new Vector2(windowSize.X, windowSize.Y),
//                     new Vector2(0, 1), new Vector2(1, 0));
//             }
//
//             if (ImGui.BeginDragDropTarget())
//             {
//                 var payload = ImGui.AcceptDragDropPayload("Scene_Drop");
//                 if (payload.IsValidPayload())
//                 {
//                     var filename = (string)GCHandle.FromIntPtr(payload.Data).Target;
//                     Console.WriteLine("Opening scene: " + filename);
//                     //Window.Get().ChangeScene(new LevelEditorScene(), filename);
//                 }
//
//                 ImGui.EndDragDropTarget();
//             }
//             
//             ImGui.PopID();
//         };
//     }
//     
//     private Vector2 getCenteredPositionForViewport(Vector2 aspectSize)
//     {
//         var windowSize = ImGui.GetContentRegionAvail();
//         windowSize.X -= ImGui.GetScrollX();
//         windowSize.Y -= ImGui.GetScrollY();
//
//         var viewportX = windowSize.X / 2.0f - aspectSize.X / 2.0f;
//         var viewportY = windowSize.Y / 2.0f - aspectSize.Y / 2.0f;
//
//         return new Vector2(viewportX + ImGui.GetCursorPosX(),
//             viewportY + ImGui.GetCursorPosY());
//     }
//
//     public Vector2 getLargestSizeForViewport()
//     {
//         var windowSize = ImGui.GetContentRegionAvail();
//         windowSize.X -= ImGui.GetScrollX();
//         windowSize.Y -= ImGui.GetScrollY();
//
//         var aspectWidth = windowSize.X;
//         var aspectHeight = aspectWidth / Engine.Get().getTargetAspectRatio();
//         if (aspectHeight > windowSize.Y)
//         {
//             // We must switch to pillarbox mode
//             aspectHeight = windowSize.Y;
//             aspectWidth = aspectHeight * Engine.Get().getTargetAspectRatio();
//         }
//
//         return new Vector2(aspectWidth, aspectHeight);
//     }
//
//     public bool IsMouseInsideViewport()
//     {
//         return
//             TestInput.getX() >= viewportPos.X
//             && TestInput.getX() <= viewportPos.X + ViewportSize.X
//             && TestInput.getY() >= viewportPos.Y
//             && TestInput.getY() <= viewportPos.Y + ViewportSize.Y;
//     }
// }