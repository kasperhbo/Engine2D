namespace Engine2D.Flags;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ShowUIAttribute : Attribute
{
    public bool show = true;
}