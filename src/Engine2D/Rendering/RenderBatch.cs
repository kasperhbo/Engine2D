#region

using System.Numerics;
using Engine2D.Cameras;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using Engine2D.GameObjects;
using Engine2D.Managers;
using Engine2D.Utilities;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL;
using TextureUnit = OpenTK.Graphics.OpenGL4.TextureUnit;
using Vector4 = OpenTK.Mathematics.Vector4;

#endregion

namespace Engine2D.Rendering;

internal class RenderBatch : IComparable<RenderBatch>
{
    public int CompareTo(RenderBatch? other)
    {
        if (other == null) return 1;
        return 0;
    }
}