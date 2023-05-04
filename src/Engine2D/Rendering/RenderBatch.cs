using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine2D.Rendering
{
    internal class RenderBatch
    {
        private int _batchCount = 0;
        private int _maxBatchSize = 20000;
        

        private RenderBatch() { }

        internal void Render() { }
        internal void AddSprite() { }
    }
}
