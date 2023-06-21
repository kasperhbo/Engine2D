namespace Engine2D.Flags;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
internal class ShowUIAttribute : Attribute
{
    public bool show = true;
}