using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDBEngine.UI
{
    public abstract class UIElemenet
    {
        public string Title { get; protected set; }        
        protected ImGuiWindowFlags _flags;
        protected Action _windowContents;
        protected bool _visibility = true;

        /// <summary>
        /// An Simple UI Window
        /// </summary>
        /// <param name="title">The Title of the ui Window</param>
        /// <param name="flags">The ImGuiWindowFlags.</param>
        /// <param name="windowContents">The Window contents like text, buttons and other elements
        /// </param>
        public UIElemenet()
        {
        }

        public void Render()
        {
            if (!_visibility) { return; }

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
}
