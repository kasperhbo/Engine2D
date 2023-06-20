using System.Numerics;
using Engine2D.Cameras;
using Engine2D.UI.Viewports;
using Engine2D.Utilities;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace Engine2D.Core.Inputs;

public static class Input
{
    

    #region mouse
    public static bool MousePressed(MouseButton button)
    {
        return Engine.Get().IsMouseButtonPressed(button);
    }
    
    public static bool MouseReleased(MouseButton button)
    {
        return Engine.Get().IsMouseButtonReleased(button);
    }
    
    public static bool MouseDown(MouseButton button)
    {
        return Engine.Get().IsMouseButtonDown(button);
    }

    public static bool IsAnyMouseButtonDown()
    {
        return Engine.Get().IsAnyMouseButtonDown;
    }
    
    public static Vector2 MouseScreenPos()
    {
        return new(ScreenX(), ScreenY());
    }
    
    public static Vector2 MousePrevPos()
    {
        return new(ScreenLastX(), ScreenLastY());
    }
    
    public static float ScreenX()
    {
        return Engine.Get().MouseState.Position.X;
    }

    public static float ScreenY()
    {
        return Engine.Get().MouseState.Position.Y;
    }

    public static float ScreenLastX()
    {
        return Engine.Get().MouseState.PreviousX;
    }
 
    public static float ScreenLastY()
    {
        return Engine.Get().MouseState.PreviousY;
    }

   
    public static Vector2 MouseDelta()
    {
        var delta = new Vector2(Engine.Get().MouseState.Delta.X,
            Engine.Get().MouseState.Delta.X);
        
        return delta;
    }
    
    
    public static bool MouseScroll()
    {
        return Engine.Get().MouseState.ScrollDelta != OpenTK.Mathematics.Vector2.Zero;
    }

    public static Vector2 MouseScrollDelta()
    {
        return new(Engine.Get().MouseState.ScrollDelta.X, Engine.Get().MouseState.ScrollDelta.Y);
    }
    
    #region World

    public static Vector2 GetWorld(ViewportWindow? vp, Camera camera)
    {
        if (Settings.s_IsEngine || vp == null)
        {
            return screenToWorld(MouseScreenPos(), camera);
        }

        return screenToWorld(MouseScreenPos(), camera, vp);
    }
    
    public static Vector2 worldToScreen(Vector2 worldCoords, Camera camera) {
        
        Vector4 ndcSpacePos = new Vector4(worldCoords.X, worldCoords.Y, 0, 1);
        Matrix4x4 view = camera.GetViewMatrix();
        Matrix4x4 projection = camera.GetProjectionMatrix();

        Matrix4x4 viewprojection = Matrix4x4.Multiply(projection, (view));
        Vector4 end = MathUtils.Multiply(viewprojection, ndcSpacePos) ;

        Vector2 windowSpace = new Vector2(end.X, end.Y) * (1.0f / end.W);

        windowSpace += (new Vector2(1.0f, 1.0f));
        windowSpace /=(2f);
        windowSpace *= new Vector2(Engine.Get().Size.X, Engine.Get().Size.Y);

        return windowSpace;
    }

    public static Vector2 screenToWorld(Vector2 screenCoords, Camera camera) {
        
        Vector2 normalizedScreenCords = new Vector2(
            screenCoords.X / Engine.Get().Size.X,
            screenCoords.Y / Engine.Get().Size.Y
        );

        normalizedScreenCords *= new Vector2(2.0f);
        normalizedScreenCords -= new Vector2(1.0f, 1.0f);
        
        
        Vector4 tmp = new Vector4(normalizedScreenCords.X, normalizedScreenCords.Y,
            0.0f, 1.0f);
        
        Matrix4x4 inverseView = (camera.getInverseView());
        Matrix4x4 inverseProjection = (camera.getInverseProjection());
        
        Matrix4 inverseViewTK = MathUtils.NumericsToTK(inverseView);
        Matrix4 inverseProjectionTK = MathUtils.NumericsToTK(inverseProjection);

        Matrix4 viewProjectionTK = Matrix4.Mult(inverseViewTK, inverseProjectionTK);
        
        var end = MathUtils.Multiply(viewProjectionTK, new OpenTK.Mathematics.Vector4(tmp.X, tmp.Y, tmp.Z, tmp.W));
        
        return new Vector2(end.X, end.Y);
    }
    
    public static Vector2 screenToWorld(Vector2 screenCoords, Camera camera, ViewportWindow vp) {
        float currentX = screenCoords.X - vp.gameViewportPos.X;
        currentX = (2.0f * (currentX / vp.gameViewportSize.X)) - 1.0f;
        
        float currentY = (screenCoords.Y - vp.gameViewportPos.Y);
        currentY = (2.0f * (1.0f - (currentY / vp.gameViewportSize.Y))) - 1;
        
        Vector4 tmp = new Vector4(currentX, currentY, 0, 1);
        Matrix4x4 inverseView = camera.getInverseView();
        Matrix4x4 inverseProjection = camera.getInverseProjection();
        Matrix4x4 viewProjection = Matrix4x4.Multiply(inverseView, inverseProjection);
        var end = MathUtils.Multiply(viewProjection, tmp);
        return new Vector2(end.X, end.Y);
    }

    #endregion
    
    #endregion
    
    #region keyboard

    public static bool KeyDown(Keys key)
    {
        return Engine.Get().IsKeyDown(key);
    }

    public static bool KeyPressed(Keys key)
    {
        return Engine.Get().IsKeyPressed(key);
    }

    public static bool KeyReleased(Keys key)
    {
        return Engine.Get().IsKeyReleased(key);
    }

    public static bool AnyKeyDown()
    {
        return Engine.Get().IsAnyKeyDown;
    }

    #endregion

}