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

    public static float xPos;
    public static float yPos;
    public static Vector2 pos;

    public static float lastX;
    public static float lastY;

    private static KeyboardState? keyboardState;
    private static MouseState? mouseState;
    public static bool Focussed { get; set; } = true;

    public static void Init()
    {
        xPos = 0.0f;
        yPos = 0.0f;
        pos = new Vector2(0.0f, 0.0f);

        lastX = 0.0f;
        lastY = 0.0f;
        lastPos = new Vector2(0.0f, 0.0f);

        worldX = 0.0f;
        worldY = 0.0f;
        world = new Vector2(0.0f, 0.0f);

        lastWorldX = 0.0f;
        lastWorldY = 0.0f;
        lastWorld = new Vector2(0.0f, 0.0f);

        viewportPos = new Vector2();
        viewportSize = new Vector2(Engine.Get().getWidth(), Engine.Get().getHeight());
    }

    public static void mousePosCallback(MouseState mouse, KeyboardState keyboard)
    {
        keyboardState = keyboard;
        mouseState = mouse;

        lastPos = pos;
        lastX = xPos; 
        lastY = yPos; 

        lastWorldX = worldX; 
        lastWorldY = worldY; 

        pos = new Vector2(mouseState.Position.X, mouseState.Position.Y);
<<<<<<< HEAD

=======
        
        // float currentY = getY();
        //
        // Console.WriteLine();
        
>>>>>>> parent of efcdaf4... AUTO REFACTORIO
        xPos = mouseState.Position.X; // Delete
        yPos = mouseState.Position.Y; // Delete

        calcOrtho();
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

    public static void SetMousePos(Vector2 position)
    {
        Engine.Get().MousePosition = position;
    }
}