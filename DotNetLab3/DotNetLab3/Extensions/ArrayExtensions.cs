namespace DotNetLab3.Extensions;

public static class ArrayExtensions
{
    // Метод, що визначає скільки разів зустрічається задане значення у масиві
    public static int CountOccurrences<T>(this T[] array, T value) where T : IEquatable<T>
    {
        if (array == null || array.Length == 0)
            return 0;

        int count = 0;
        foreach (T item in array)
        {
            if (item != null && item.Equals(value))
            {
                count++;
            }
        }
        return count;
    }

    // Метод, що повертає новий масив з унікальних елементів (видаляє повтори)
    public static T[] GetUniqueElements<T>(this T[] array) where T : notnull, IEquatable<T>
    {
        if (array == null || array.Length == 0)
            return Array.Empty<T>();

        HashSet<T> uniqueSet = new HashSet<T>(array);
        T[] result = new T[uniqueSet.Count];
        uniqueSet.CopyTo(result);

        return result;
    }
}