using Engine2D.GameObjects;
using KDBEngine.Core;

public static class Program
{
    static void Main()
    {
        Settings.s_IsEngine = true;

        Engine.Get().Run();
    }
}