namespace Engine2D.UI;

public interface IFocussable
{
    public bool IsFocused();
    public void OnFocus();
    public void OnUnfocus();
    public void OnHover();
    public void OnUnHover();
}