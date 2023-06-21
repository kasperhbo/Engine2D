#region

using System.Numerics;
using Engine2D.Cameras;
using Engine2D.UI.Viewports;
using Engine2D.Utilities;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

#endregion

namespace Engine2D.Core.Inputs;

internal static class Input
{
    #region mouse

    internal static bool MousePressed(MouseButton button)
    {
        return Engine.Get().IsMouseButtonPressed(button);
    }

    internal static bool MouseReleased(MouseButton button)
    {
        return Engine.Get().IsMouseButtonReleased(button);
    }

    internal static bool MouseDown(MouseButton button)
    {
        return Engine.Get().IsMouseButtonDown(button);
    }

    internal static bool IsAnyMouseButtonDown()
    {
        return Engine.Get().IsAnyMouseButtonDown;
    }

    internal static Vector2 MouseScreenPos()
    {
        return new Vector2(ScreenX(), ScreenY());
    }

    internal static Vector2 MousePrevPos()
    {
        return new Vector2(ScreenLastX(), ScreenLastY());
    }

    internal static float ScreenX()
    {
        return Engine.Get().MouseState.Position.X;
    }

    internal static float ScreenY()
    {
        return Engine.Get().MouseState.Position.Y;
    }

    internal static float ScreenLastX()
    {
        return Engine.Get().MouseState.PreviousX;
    }

    internal static float ScreenLastY()
    {
        return Engine.Get().MouseState.PreviousY;
    }


    internal static Vector2 MouseDelta()
    {
        var delta = new Vector2(Engine.Get().MouseState.Delta.X,
            Engine.Get().MouseState.Delta.X);

        return delta;
    }


    internal static bool MouseScroll()
    {
        return Engine.Get().MouseState.ScrollDelta != OpenTK.Mathematics.Vector2.Zero;
    }

    internal static Vector2 MouseScrollDelta()
    {
        return new Vector2(Engine.Get().MouseState.ScrollDelta.X, Engine.Get().MouseState.ScrollDelta.Y);
    }

    #region World

    internal static Vector2 GetWorld(ViewportWindow? vp, Camera camera)
    {
        if (Settings.s_IsEngine || vp == null) return screenToWorld(MouseScreenPos(), camera);

        return screenToWorld(MouseScreenPos(), camera, vp);
    }

    internal static Vector2 worldToScreen(Vector2 worldCoords, Camera camera)
    {
        var ndcSpacePos = new Vector4(worldCoords.X, worldCoords.Y, 0, 1);
        var view = camera.GetViewMatrix();
        var projection = camera.GetProjectionMatrix();

        var viewprojection = Matrix4x4.Multiply(projection, view);
        var end = MathUtils.Multiply(viewprojection, ndcSpacePos);

        var windowSpace = new Vector2(end.X, end.Y) * (1.0f / end.W);

        windowSpace += new Vector2(1.0f, 1.0f);
        windowSpace /= 2f;
        windowSpace *= new Vector2(Engine.Get().Size.X, Engine.Get().Size.Y);

        return windowSpace;
    }

    internal static Vector2 screenToWorld(Vector2 screenCoords, Camera camera)
    {
        var normalizedScreenCords = new Vector2(
            screenCoords.X / Engine.Get().Size.X,
            screenCoords.Y / Engine.Get().Size.Y
        );

        normalizedScreenCords *= new Vector2(2.0f);
        normalizedScreenCords -= new Vector2(1.0f, 1.0f);


        var tmp = new Vector4(normalizedScreenCords.X, normalizedScreenCords.Y,
            0.0f, 1.0f);

        var inverseView = camera.getInverseView();
        var inverseProjection = camera.getInverseProjection();

        var inverseViewTK = MathUtils.NumericsToTK(inverseView);
        var inverseProjectionTK = MathUtils.NumericsToTK(inverseProjection);

        var viewProjectionTK = Matrix4.Mult(inverseViewTK, inverseProjectionTK);

        var end = MathUtils.Multiply(viewProjectionTK, new OpenTK.Mathematics.Vector4(tmp.X, tmp.Y, tmp.Z, tmp.W));

        return new Vector2(end.X, end.Y);
    }

    internal static Vector2 screenToWorld(Vector2 screenCoords, Camera camera, ViewportWindow vp)
    {
        var currentX = screenCoords.X - vp.gameViewportPos.X;
        currentX = 2.0f * (currentX / vp.gameViewportSize.X) - 1.0f;

        var currentY = screenCoords.Y - vp.gameViewportPos.Y;
        currentY = 2.0f * (1.0f - currentY / vp.gameViewportSize.Y) - 1;

        var tmp = new Vector4(currentX, currentY, 0, 1);
        var inverseView = camera.getInverseView();
        var inverseProjection = camera.getInverseProjection();
        var viewProjection = Matrix4x4.Multiply(inverseView, inverseProjection);
        var end = MathUtils.Multiply(viewProjection, tmp);
        return new Vector2(end.X, end.Y);
    }

    #endregion

    #endregion

    #region keyboard

    internal static bool KeyDown(Keys key)
    {
        return Engine.Get().IsKeyDown(key);
    }

    internal static bool KeyPressed(Keys key)
    {
        return Engine.Get().IsKeyPressed(key);
    }

    internal static bool KeyReleased(Keys key)
    {
        return Engine.Get().IsKeyReleased(key);
    }

    internal static bool AnyKeyDown()
    {
        return Engine.Get().IsAnyKeyDown;
    }

    #endregion
}