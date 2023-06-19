using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Engine2D.Logging;

internal static class Log
{
    
    internal static void Message([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
    {
        Message(string.Format(format, arg0));
    }

    internal static void Message(string? message, [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, bool showFile = true,
        bool showLine = true, bool showFunction = true)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(GetString(showFile, showLine, showFunction, message, memberName, sourceFilePath,
            sourceLineNumber));
        Console.ForegroundColor = ConsoleColor.White;
    }

    internal static void Succes([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
    {
        Succes(string.Format(format, arg0));
    }
    
    internal static void Succes(string? message, [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, bool showFile = true,
        bool showLine = true, bool showFunction = true)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(GetString(showFile, showLine, showFunction, message, memberName, sourceFilePath,
            sourceLineNumber));
        Console.ForegroundColor = ConsoleColor.White;
    }
    
    internal static void Warning([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
    {
        Warning(string.Format(format, arg0));
    }

    internal static void Warning(string? message, [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, bool showFile = false,
        bool showLine = false, bool showFunction = false)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(GetString(showFile, showLine, showFunction, message, memberName, sourceFilePath,
            sourceLineNumber));
        Console.ForegroundColor = ConsoleColor.White;
    }

    internal static void Error([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, object? arg0)
    {
        Error(string.Format(format, arg0));
    }
    
    internal static void Error(string? message, [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, bool showFile = true,
        bool showLine = true, bool showFunction = true)
    {
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