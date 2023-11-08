using System.Collections.Generic;
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

    public static string SplitCamelCase(this string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
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

    public static string GenerateOxfordList(this List<string> list)
    {
        if (list.Count == 0) return null;
        if(list.Count == 1)
        {
            return list[0];
        }
        if(list.Count == 2)
        {
            return $"{list[0]} and {list[1]}";
        }
        string output = "";
        int minusList = list.Count - 1;
        for (int i = 0; i < list.Count; i++)
        {
            if (i == minusList)
            {
                output += $" and {list[minusList]}";
            }
            else
                output += $"{list[i]},";
        }
        return output;
    }
}
