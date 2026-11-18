namespace DotNetLab3.Extensions;

public static class StringExtensions
{
    // Інвертування рядка
    public static string ReverseString(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        char[] charArray = str.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    // Підрахунок кількості входжень заданого символу у рядок
    public static int CountCharOccurrences(this string str, char target)
    {
        if (string.IsNullOrEmpty(str))
            return 0;

        int count = 0;
        foreach (char c in str)
        {
            if (c == target)
            {
                count++;
            }
        }
        return count;
    }
}