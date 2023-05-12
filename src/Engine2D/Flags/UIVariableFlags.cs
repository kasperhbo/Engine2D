using System.ComponentModel;

namespace Engine2D.Flags
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ShowUIAttribute : Attribute
    {
        public bool show = true;
    }
}
