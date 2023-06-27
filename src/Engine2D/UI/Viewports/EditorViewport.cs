#region

using Engine2D.Core.Inputs;

#endregion

namespace Engine2D.UI.Viewports;

internal class EditorViewport : ViewportWindow
{
   
    protected override void BeforeImageRender()
    {
    }

    protected override void AfterImageRender()
    {
        Input.CalculateMouseEditor(this, Camera);
        SceneControls.GuizmoControls(Origin, Size, Camera);
    }
    
    public EditorViewport(string title) : base(title)
    {
    }
}