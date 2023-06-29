#region

using System.Numerics;
using Engine2D.Cameras;
using Engine2D.UI.Viewports;
using ImGuiNET;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector2 = System.Numerics.Vector2;

#endregion

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
        return new Vector2(ScreenX(), ScreenY());
    }

    public static Vector2 MousePrevPos()
    {
        return new Vector2(ScreenLastX(), ScreenLastY());
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
        return new Vector2(Engine.Get().MouseState.ScrollDelta.X, Engine.Get().MouseState.ScrollDelta.Y);
    }
        
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
    
    #region World

    public static Vector2 MouseEditorPos = new();
    public static Vector2 MouseGamePos = new();
    
    internal static Vector2 CalculateMouseEditor(EditorViewport vp, Camera camera)
    {
        MouseEditorPos = CalculateMousePosVP(vp, camera);
        return MouseEditorPos;
    }
    
    internal static Vector2 CalculateMouseGame(GameViewport vp, Camera camera)
    {
        MouseGamePos = CalculateMousePosVP(vp, camera);
        return MouseGamePos;
    }

    private static Vector2 CalculateMousePosVP(ViewportWindow vp, Camera camera)
    {
        // Calculate the mouse position relative to the viewport
        Vector2 mousePosition = ImGui.GetMousePos();
        Vector2 viewportPosition = ImGui.GetItemRectMin();
        Vector2 mouseViewportPosition = mousePosition - viewportPosition;

        // Calculate the normalized device coordinates (NDC) of the mouse position
        Vector2 normalizedMousePos = new Vector2(
            (mouseViewportPosition.X / vp.WindowSize.X) * 2 - 1,
            1 - (mouseViewportPosition.Y / vp.WindowSize.Y) * 2
        );

        // Calculate the projection matrix and view matrix of the camera
        Matrix4x4 projectionMatrix = camera?.GetProjectionMatrix() ?? Matrix4x4.Identity;
        Matrix4x4 viewMatrix = camera?.GetViewMatrix() ?? Matrix4x4.Identity;

        // Calculate the inverse of the projection matrix and view matrix
        Matrix4x4.Invert(projectionMatrix, out var inverseProjectionMatrix);
        Matrix4x4.Invert(viewMatrix, out var inverseViewMatrix);

        // Calculate the mouse position in clip space
        Vector4 clipSpacePos = new Vector4(normalizedMousePos, -1, 1);

        // Transform the mouse position from clip space to eye space
        Vector4 eyeSpacePos = Vector4.Transform(clipSpacePos, inverseProjectionMatrix);

        // Transform the mouse position from eye space to world space
        Vector4 worldSpacePos = Vector4.Transform(eyeSpacePos, inverseViewMatrix);

        // The world coordinates of the mouse position
        Vector3 mouseWorldPos = new Vector3(worldSpacePos.X, worldSpacePos.Y, worldSpacePos.Z) / worldSpacePos.W;
        return new(mouseWorldPos.X, mouseWorldPos.Y);
    }

    #endregion


}