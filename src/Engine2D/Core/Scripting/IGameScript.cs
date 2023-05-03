using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Core.Scripting
{
    public interface IGameScript
    {
        void Init();
        void Start();
        void Update(double dt);
    }
}
