#region

using Engine2D.Core.Inputs;

#endregion

namespace Engine2D.UI.Viewports;

internal class EditorViewport : ViewportWindow
{
   
    internal override void BeforeImageRender()
    {
        // Camera.ProjectionSize = new Vector2(1920, 1080);
    }

    internal override void AfterImageRender()
    {
        Input.CalculateMouseEditor(this, Camera);
        SceneControls.GuizmoControls(Origin, Size, Camera);
    }
    
    public EditorViewport(string title) : base(title)
    {
    }
}