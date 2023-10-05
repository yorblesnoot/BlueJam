using System.Runtime.InteropServices;
using UnityEngine;

public static class StringHelper
{
    public static string FirstToUpper(this string s)
    {
        // Check for empty string.
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        // Return char and concat substring.
        return char.ToUpper(s[0]) + s.Substring(1);
    }

    public static bool CharacterIsVowel(this string s, int index)
    {
        var chars = s.ToCharArray();
        if ("aeiouAEIOU".Contains(chars[index])) return true;
        else return false;
    }

    static string[] colors = { "red", "orange", "yellow", "green", "blue", "purple", "#4B0082" };
    public static string GenerateRainbowText(this string text)
    {
        var chars = text.ToCharArray();
        string output = "";
        int position = 0;
        while (position < text.Length)
        {
            output += $"<color={colors[position % colors.Length]}>{chars[position]}</color>";
            position++;
        }
        return output;
    }
}
