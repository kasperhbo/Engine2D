namespace Engine2D.Utilities;

public static class StringUtils
{
    public static string CharToString(char[] chars)
    {
        string text = "";

        for (int i = 0; i < chars.Length; i++)
        {
            text += chars[i];
        }
        
        return text;
    }

}