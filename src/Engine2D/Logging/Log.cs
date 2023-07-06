#region

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

#endregion

namespace Engine2D.Logging;

public static class Log
{
    private static bool debugging = true;
    public static void Message([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
    {
        if (!debugging) return;
        Message(string.Format(format, arg0));
    }

    public static void Message(string? message, [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, bool showFile = true,
        bool showLine = true, bool showFunction = true)
    {
        if (!debugging) return;
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(GetString(showFile, showLine, showFunction, message, memberName, sourceFilePath,
            sourceLineNumber));
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void Succes([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
    {
        if (!debugging) return;
        Succes(string.Format(format, arg0));
    }

    public static void Succes(string? message, [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, bool showFile = true,
        bool showLine = true, bool showFunction = true)
    {
        if (!debugging) return;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(GetString(showFile, showLine, showFunction, message, memberName, sourceFilePath,
            sourceLineNumber));
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void Warning([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
    {
        if (!debugging) return;
        Warning(string.Format(format, arg0));
    }

    public static void Warning(string? message, [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, bool showFile = true,
        bool showLine = true, bool showFunction = true)
    {
        if (!debugging) return;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(GetString(showFile, showLine, showFunction, message, memberName, sourceFilePath,
            sourceLineNumber));
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void Error([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
    {
        if (!debugging) return;
        Error(string.Format(format, arg0));
    }

    public static void Error(string? message, [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, bool showFile = true,
        bool showLine = true, bool showFunction = true)
    {
        if (!debugging) return;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(GetString(showFile, showLine, showFunction, message, memberName, sourceFilePath,
            sourceLineNumber));
        Console.ForegroundColor = ConsoleColor.White;
    }


    private static string GetString(bool showFile, bool showLine, bool showFunction, string? message, string memberName,
        string sourceFilePath, int sourceLineNumber)
    {
        
        var sb = new StringBuilder();
        sb.Append("[" + "time: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm") + "]");

        if (showFunction)
            sb.Append(" | " + memberName);
        if (showFile)
            sb.Append(" | " + sourceFilePath);
        if (showLine)
            sb.Append(" | " + sourceLineNumber);

        sb.Append(" | " + message);

        return sb.ToString();
    }
}