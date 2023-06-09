#region

using System.Numerics;
using Engine2D.Components.Cameras;
using Engine2D.Core;
using ImGuiNET;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

#endregion

namespace Engine2D.UI.Viewports;

internal abstract class ViewportWindow : IFocussable
{
    protected Camera Camera;
    protected Vector2 Origin;
    protected Vector2 Size;
    
    private bool _isHovering  = false;
    private bool _isFocussed  = false;
    
    private string _title;

    protected ViewportWindow(string title)
    {
        _title = title;
    }

    public Vector2 WindowSize { get; set; }
    public Vector2 WindowPos { get; set; }

    internal void Begin(string title, Camera cameraToRender)
    {
        Camera = cameraToRender;
        _title = title;

        BeforeImageRender();
        Render();
        AfterImageRender();
        End();
    }
    
    protected abstract void BeforeImageRender();

    private void Render()
    {
        ImGui.Begin(_title,
            ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.MenuBar);
        
        _isFocussed = GetIsFocused();
        
        ImGui.BeginMenuBar();
        if (ImGui.MenuItem("Play", "", Engine.Get().CurrentScene.IsPlaying, !Engine.Get().CurrentScene.IsPlaying))
        {
            Engine.Get().CurrentScene.IsPlaying = true;
        }

        if (ImGui.MenuItem("Stop", "", !Engine.Get().CurrentScene.IsPlaying, Engine.Get().CurrentScene.IsPlaying))
        {
            Engine.Get().CurrentScene.IsPlaying = false;
        }

        ImGui.EndMenuBar();
        
        ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPosX(), ImGui.GetCursorPosY()));
        WindowSize = GetLargestSizeForViewport();
        WindowPos = GetCenteredPositionForViewport(WindowSize);
        ImGui.SetCursorPos(new Vector2(WindowPos.X, WindowPos.Y));

        RenderImage();
       
        
        _isHovering = ImGui.IsItemHovered();
        
        Origin = ImGui.GetItemRectMin();
        Size = ImGui.GetItemRectSize();
    }

    protected abstract void RenderImage();

    protected abstract void AfterImageRender();

    private void End()
    {
        ImGui.End();
    }

    private bool GetIsFocused()
    {
        return !ImGui.GetCurrentWindow().Hidden;
    }

    private Vector2 GetLargestSizeForViewport()
    {
        var windowSize = ImGui.GetContentRegionAvail();

        float aspectWidth = windowSize.X;
        float aspectHeight = aspectWidth /  Engine.Get().GetTargetAspectRatio();
        if (aspectHeight > windowSize.Y) {
            // We must switch to pillarbox mode
            aspectHeight = windowSize.Y;
            aspectWidth = aspectHeight * Engine.Get().GetTargetAspectRatio();
        }

        return new(aspectWidth, aspectHeight);
    }

    private Vector2 GetCenteredPositionForViewport(Vector2 aspectSize)
    {
        var windowSize = ImGui.GetContentRegionAvail();

        float viewportX = (windowSize.X / 2.0f) - (aspectSize.X / 2.0f);
        float viewportY = (windowSize.Y / 2.0f) - (aspectSize.Y / 2.0f);

        return new(viewportX + ImGui.GetCursorPosX(), viewportY + ImGui.GetCursorPosY());
    }


    public bool IsFocused()
    {
        return _isFocussed;
    }

    public void OnFocus()
    {
    }

    public void OnUnfocus()
    {
    }

    public void OnHover()
    {
    }

    public void OnUnHover()
    {
    }

    public void OnTextInput(TextInputEventArgs args)
    {
        
    }
    
    public void OnKeyDown(KeyboardKeyEventArgs args)
    {
        if (args.Key == Keys.Delete)
        {
            Engine.Get().CurrentSelectedAsset.IsDead = true;
        }
    }

    public void OnKeyUp(KeyboardKeyEventArgs args)
    {
        
    }
}
