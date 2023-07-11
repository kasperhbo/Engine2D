#region

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