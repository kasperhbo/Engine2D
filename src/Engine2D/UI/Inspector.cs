using Engine2D.GameObjects;
using KDBEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.UI
{
    internal class Inspector : UIElemenet
    {
        internal Gameobject _currentSelectedGameObject;

        internal Inspector() {
            this.Title = "Inspector";
            this._flags = ImGuiNET.ImGuiWindowFlags.None;
            this._windowContents = () =>
            {
                _currentSelectedGameObject?.ImGuiFields();
            };
        }
    }
}
