using Engine2D.Cameras;
using Engine2D.Testing;
using ImGuiNET;
using Engine2D.Core;
using OpenTK.Mathematics;
using Veldrid;
using Vector2 = System.Numerics.Vector2;

namespace Engine2D.UI.Viewports;

public abstract class ViewportWindow
{
    public abstract void OnGui(Camera? cameraToRender, string title);
    public abstract Vector2 GetVPSize();
    // private TestFrameBuffer? _testFrameBuffer = null;
    // protected TestCamera? _cameraToRender = null;
    //
    // protected Vector2 _windowSize = new();
    // protected Vector2 _windowPos = new();
    //
    // private string _title = "Unnamed viewport";
    //
    // public ViewportWindow(string title)
    // {
    //     _title = title;
    // }
    //
    //
    // public ViewportWindow(TestFrameBuffer? testFrameBuffer, TestCamera? cameraToRender, string title)
    // {
    //     _testFrameBuffer = testFrameBuffer;
    //     _cameraToRender = cameraToRender;
    //     _title = title;
    // }
    //
    // public void OnGui(TestFrameBuffer? testFrameBuffer, TestCamera? camera)
    // {
    //     _testFrameBuffer = testFrameBuffer;
    //     _cameraToRender = camera;
    //     OnGui();
    // }
    //
    // protected Vector2 origin = new();
    // protected Vector2 sz = new();
    // protected Vector2 cursorPos = new();
    // public void OnGui()
    // {
    //     ImGui.Begin(_title);
    //
    //     _windowSize = GetLargestSizeForVP();
    //     _windowPos = GetCenteredViewportPos(_windowSize);
    //
    //     var topLeft = ImGui.GetCursorScreenPos();
    //     topLeft.X -= ImGui.GetScrollX(); 
    //     topLeft.Y -= ImGui.GetScrollY();
    //
    //     ImGui.SetCursorPos(ImGui.GetCursorPos() + (ImGui.GetContentRegionAvail() - _windowSize) * 0.5f);
    //     
    //     cursorPos = ImGui.GetCursorPos();
    //     
    //     BeforeImageRender();
    //
    //     ImGui.SetCursorPos(cursorPos);
    //     ImGui.Image(GetImageToRender(), _windowSize, new Vector2(0,1), new Vector2(1,0));
    //     origin = ImGui.GetItemRectMin();
    //     sz = ImGui.GetItemRectSize();
    //     
    //     AfterImageRender();
    //     
    //     ImGui.End();
    // }
    //
    // protected IntPtr GetImageToRender()
    // {
    //     return ((IntPtr)_testFrameBuffer?.Texture.TexID)!;
    // }
    //
    // private Vector2 GetCenteredViewportPos(Vector2 aspectSize)
    // {
    //     var windowSize = ImGui.GetContentRegionAvail();
    //     windowSize.X -= ImGui.GetScrollX();
    //     windowSize.Y -= ImGui.GetScrollY();
    //
    //     var viewportX = windowSize.X / 2.0f - aspectSize.X / 2.0f;
    //     var viewportY = windowSize.Y / 2.0f - aspectSize.Y / 2.0f;
    //     
    //     return new Vector2(viewportX + ImGui.GetCursorPosX(),
    //         viewportY + ImGui.GetCursorPosY());
    // }
    //
    // protected virtual Vector2 GetLargestSizeForVP()
    // {
    //     var windowSize = ImGui.GetContentRegionAvail();
    //     
    //     windowSize.X -= ImGui.GetScrollX();
    //     windowSize.Y -= ImGui.GetScrollY();
    //     
    //     float targetAspectRatio = 16/9;
    //     
    //     if(_cameraToRender != null)
    //         targetAspectRatio = _cameraToRender.ProjectionSize.X / _cameraToRender.ProjectionSize.Y;
    //     
    //     var aspectWidth = windowSize.X;
    //     var aspectHeight = aspectWidth / targetAspectRatio;
    //     
    //     if (aspectHeight > windowSize.Y)
    //     {
    //         // We must switch to pillarbox mode
    //         aspectHeight = windowSize.Y;
    //         aspectWidth = aspectHeight * targetAspectRatio;
    //     }
    //     
    //     return new Vector2(aspectWidth, aspectHeight);
    // }
    //
    //
    // protected virtual void BeforeImageRender()
    // {
    //     
    // }
    //
    // protected virtual void AfterImageRender(bool recieveInput = true)
    // {
    // }




}