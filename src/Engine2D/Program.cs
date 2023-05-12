//using ImGuiNET;

using KDBEngine.Core;

namespace Engine2D;

public class Program
{
    public static void Main()
    {
        Engine.Get();
        Engine.Get().Run();
    }
}