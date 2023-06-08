using Engine2D.Logging;
using ImGuiNET;
using OpenTK.Windowing.Common;

namespace Engine2D.UI;

public abstract class UiElemenet
{
    protected ImGuiWindowFlags _flags;
    protected bool _visibility = true;
    protected Action _windowContents;
    
    public bool IsHovering { get; private set; } = false;
    public string Title { get; private set; }

    /// <summary>
    ///     An Simple UI Window
    /// </summary>
    /// <param name="title">The Title of the ui Window</param>
    /// <param name="flags">The ImGuiWindowFlags.</param>
    /// <param name="windowContents">
    ///     The Window contents like text, buttons and other elements
    /// </param>

    public UiElemenet()
    {
        _flags = GetWindowFlags();
        Title = GetWindowTitle();
        _windowContents = GetWindowContent();
    }

    
    protected abstract string GetWindowTitle();
    protected abstract ImGuiWindowFlags GetWindowFlags();

    protected virtual Action GetMenuBarContent()
    {
        return () => { };
    }
    
    protected abstract Action GetWindowContent();
    
    public void Render()
    {
        if (!_visibility) return;

        ImGui.Begin(Title, _flags);
        GetMenuBarContent().Invoke();
       
        _windowContents?.Invoke();
        IsHovering = ImGui.IsWindowHovered();

        ImGui.End();

    }

    public virtual void OnFileDrop(FileDropEventArgs args)
    {
        if (IsHovering)
            foreach (var file in args.FileNames)
            {
                Log.Message(string.Format("Dropped Files {0} On To {1}", file, Title));
            }
    }

    public virtual void SetVisibility(bool visibility)
    {
        _visibility = visibility;
    }
}