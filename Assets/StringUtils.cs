public class StringUtils
{
    public static string Capitalized(string text)
    {
        return char.ToUpper(text[0]) + text[1..];
    }
}