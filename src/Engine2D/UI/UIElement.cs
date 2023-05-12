using ImGuiNET;

namespace KDBEngine.UI;

public abstract class UIElemenet
{
    protected ImGuiWindowFlags _flags;
    protected bool _visibility = true;
    protected Action _windowContents;

    /// <summary>
    ///     An Simple UI Window
    /// </summary>
    /// <param name="title">The Title of the ui Window</param>
    /// <param name="flags">The ImGuiWindowFlags.</param>
    /// <param name="windowContents">
    ///     The Window contents like text, buttons and other elements
    /// </param>
    public UIElemenet()
    {
    }

    public string Title { get; protected set; }

    public void Render()
    {
        if (!_visibility) return;

        ImGui.Begin(Title, _flags);

        _windowContents?.Invoke();

        ImGui.End();
    }

    #region Setters

    public void SetWindowContent(Action windowContents)
    {
        _windowContents = windowContents;
    }

    public void SetWindowTitle(string title)
    {
        Title = title;
    }

    public void SetWindowFlags(ImGuiWindowFlags flags)
    {
        _flags = flags;
    }

    public virtual void SetVisibility(bool visibility)
    {
        _visibility = visibility;
    }

    #endregion
}