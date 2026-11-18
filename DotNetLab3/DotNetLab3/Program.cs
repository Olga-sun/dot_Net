using System.Text;
using DotNetLab3.Extensions;
using DotNetLab3.Generics;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("=== 1. Демонстрація методів розширення для String ===");
string sampleText = "Програмування на C#";
char searchChar = 'а';

Console.WriteLine($"Оригінальний рядок: {sampleText}");
Console.WriteLine($"Інвертований рядок: {sampleText.ReverseString()}");
Console.WriteLine($"Кількість входжень символу '{searchChar}': {sampleText.CountCharOccurrences(searchChar)}");
Console.WriteLine();

Console.WriteLine("=== 2. Демонстрація методів розширення для одновимірних масивів ===");
int[] numbers = { 1, 4, 2, 4, 7, 1, 9, 4 };
string[] words = { "apple", "banana", "apple", "orange", "banana" };

Console.WriteLine($"Масив чисел: [{string.Join(", ", numbers)}]");
Console.WriteLine($"Число 4 зустрічається: {numbers.CountOccurrences(4)} рази(ів)");
Console.WriteLine($"Унікальні числа: [{string.Join(", ", numbers.GetUniqueElements())}]");

Console.WriteLine($"\nМасив слів: [{string.Join(", ", words)}]");
Console.WriteLine($"Слово 'apple' зустрічається: {words.CountOccurrences("apple")} рази(ів)");
Console.WriteLine($"Унікальні слова: [{string.Join(", ", words.GetUniqueElements())}]");
Console.WriteLine();

Console.WriteLine("=== 3. Демонстрація роботи ExtendedDictionary<T, U, V> ===");
var studentDict = new ExtendedDictionary<int, string, double>();

// Додавання елементів
studentDict.Add(101, "Іваненко Олена", 92.5);
studentDict.Add(102, "Петренко Максим", 85.0);
studentDict.Add(103, "Сидоренко Анна", 98.2);

Console.WriteLine($"Кількість елементів у словнику: {studentDict.Count}");

// Перебір через foreach
Console.WriteLine("\nВміст словника (через foreach):");
foreach (var elem in studentDict)
{
    Console.WriteLine($"  Ключ: {elem.Key}, Студент: {elem.Value1}, Середній бал: {elem.Value2}");
}

// Доступ за індексатором
Console.WriteLine($"\nПошук за індексатором [102]: {studentDict[102]}");

// Перевірка наявності ключа та значень
Console.WriteLine($"Чи є ключ 101? {studentDict.ContainsKey(101)}");
Console.WriteLine($"Чи є ключ 999? {studentDict.ContainsKey(999)}");
Console.WriteLine($"Чи є пара значень ('Сидоренко Анна', 98.2)? {studentDict.ContainsValues("Сидоренко Анна", 98.2)}");

// Видалення елемента
bool isRemoved = studentDict.Remove(102);
Console.WriteLine($"\nВидалення ключа 102 успішне: {isRemoved}");
Console.WriteLine($"Кількість елементів після видалення: {studentDict.Count}");

Console.WriteLine("\nОновлений словник:");
foreach (var elem in studentDict)
{
    Console.WriteLine($"  {elem}");
}