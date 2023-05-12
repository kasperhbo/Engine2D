﻿using ImGuiNET;

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

    public string Title { get; protected set; }

    public UIElemenet()
    {
        _flags = SetWindowFlags();
        Title = SetWindowTitle();
        _windowContents = SetWindowContent();
    }
    
    protected abstract  string SetWindowTitle();
    protected abstract  ImGuiWindowFlags  SetWindowFlags();
    protected abstract  Action SetWindowContent();
    
    public void Render()
    {
        if (!_visibility) return;

        ImGui.Begin(Title, _flags);

        _windowContents?.Invoke();

        ImGui.End();
    }

    #region Setters

    public virtual void SetVisibility(bool visibility)
    {
        _visibility = visibility;
    }

    #endregion
}