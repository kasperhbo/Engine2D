using KDBEngine.Core;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine2D.Testing;

internal class TestInput
{
    private static Vector2 lastPos;
    private static double worldX;
    private static double worldY;
    private static Vector2 world;
    private static double lastWorldX;
    private static double lastWorldY;
    private static Vector2 lastWorld;

    public static Vector2 viewportPos;
    public static Vector2 viewportSize;

    private static double xPos;
    private static double yPos;
    private static Vector2 pos;

    private static double lastX;
    private static double lastY;

    private static KeyboardState? keyboardState;
    private static MouseState? mouseState;
    public static bool Focussed { get; set; } = true;

    public static void Init()
    {
        xPos = 0.0;
        yPos = 0.0;
        pos = new Vector2(0.0f, 0.0f);

        lastX = 0.0;
        lastY = 0.0;
        lastPos = new Vector2(0.0f, 0.0f);

        worldX = 0.0;
        worldY = 0.0;
        world = new Vector2(0.0f, 0.0f);

        lastWorldX = 0.0;
        lastWorldY = 0.0;
        lastWorld = new Vector2(0.0f, 0.0f);

        viewportPos = new Vector2();
        viewportSize = new Vector2(Engine.Get().getWidth(), Engine.Get().getHeight());
    }

    public static void mousePosCallback(MouseState mouse, KeyboardState keyboard)
    {
        //if (.mouseButtonDown > 0)
        //{
        //    isDragging = true;
        //}
        keyboardState = keyboard;
        mouseState = mouse;

        lastPos = pos; // Useless since endFrame() ??
        lastX = xPos; // Useless since endFrame() ??
        lastY = yPos; // Useless since endFrame() ??

        lastPos = pos; // Useless since endFrame() ??
        lastWorldX = worldX; // Useless since endFrame() ??
        lastWorldY = worldY; // Useless since endFrame() ??

        pos = new Vector2(mouseState.Position.X, mouseState.Position.Y);

        // float currentY = getY();
        //
        // Console.WriteLine();

        xPos = mouseState.Position.X; // Delete
        yPos = mouseState.Position.Y; // Delete

        calcOrtho();
    }

    public static void endFrame()
    {
        lastPos = pos;
        lastX = xPos; // Delete
        lastY = yPos; // Delete

        lastWorld = world;
        lastWorldX = worldX; // Delete
        lastWorldY = worldY; // Delete
    }

    public static bool KeyPress(Keys key)
    {
        return keyboardState!.IsKeyPressed(key);
    }

    public static bool KeyReleased(Keys key)
    {
        return keyboardState!.IsKeyReleased(key);
    }

    public static bool KeyDown(Keys key)
    {
        return keyboardState!.IsKeyDown(key);
    }


    public static float getX()
    {
        //return (float) get().xPos;
        return pos.X;
    }

    public static float getY()
    {
        //return (float) get().yPos;
        return pos.Y;
    }

    private static void calcOrtho()
    {
        var currentX = getX() - viewportPos.X;
        var currentY = getY() - viewportPos.Y;
        //System.out.println(get().viewportSize.x);
        currentX = currentX / viewportSize.X * 2.0f - 1.0f;
        currentY = -(currentY / viewportSize.Y * 2.0f - 1.0f);

        // vec.w must be 1! (Vector multiplication)
        var tmp = new Vector4(currentX, currentY, 0.0f, 1.0f);

        var camera = Engine.Get()._currentScene.EditorCamera;
        var viewProjection = new Matrix4();

        viewProjection = Matrix4.Mult(camera.getInverseViewMatrix(), camera.getInverseProjection());
        var t = MathUtils.Multiply(viewProjection, tmp);
        world = new Vector2(t.X, t.Y);
    }

    public static void setViewportPos(Vector2 gameViewportPos)
    {
        viewportPos = gameViewportPos;
    }

    public static void setViewportSize(Vector2 gameViewportSize)
    {
        viewportSize = gameViewportSize;
    }

    public static Vector2 getWorld()
    {
        return world;
    }

    public static bool MouseReleased(MouseButton button)
    {
        if (mouseState == null) return false;
        return mouseState!.IsButtonReleased(button);
    }

    public static bool MousePressed(MouseButton button)
    {
        if (mouseState == null) return false;
        return mouseState.IsButtonPressed(button);
    }

    public static bool MouseDown(MouseButton button)
    {
        if (mouseState == null) return false;
        return mouseState!.IsButtonDown(button);
    }
}