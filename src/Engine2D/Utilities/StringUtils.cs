namespace Engine2D.Utilities;

internal static class StringUtils
{
    internal static string CharToString(char[] chars)
    {
        var text = "";

        for (var i = 0; i < chars.Length; i++) text += chars[i];

        return text;
    }
}