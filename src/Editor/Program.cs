using System.Reflection;
using Engine2D.Core;

public class Program
{
    static void Main()
    {
        Settings.s_IsEngine = true;        
        Engine.Get().Run();
    }
}