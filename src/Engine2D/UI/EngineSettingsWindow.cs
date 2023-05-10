using ImGuiNET;
using KDBEngine.Core;
using KDBEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.UI
{
    internal class EngineSettingsWindow : UIElemenet
    {
        internal EngineSettingsWindow()
        {
            this._flags = ImGuiWindowFlags.None;
            this.Title =  "Engine Settings";
            this.SetVisibility(false);
            this._windowContents = () =>
            {
                if(ImGui.Button("Close"))
                    SetVisibility(false);

                if (OpenTKUIHelper.DragFloat("Font Size", ref EngineSettings.GlobalScale,dragSpeed: 0.001f))
                {
                    ImGui.GetIO().FontGlobalScale = EngineSettings.GlobalScale;
                }
            };
        }
    }
}
