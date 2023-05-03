using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Scenes
{
    internal abstract class SceneInitializer
    {
        public abstract void Init(Scene scene);
        public abstract void ImGUi();
    }
}
