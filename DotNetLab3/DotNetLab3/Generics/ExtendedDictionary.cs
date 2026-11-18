using System.Collections;

namespace DotNetLab3.Generics;

public class ExtendedDictionary<T, U, V> : IEnumerable<ExtendedDictionaryElement<T, U, V>>
    where T : notnull
{
    private readonly List<ExtendedDictionaryElement<T, U, V>> _elements = new();

    // Властивість, що повертає кількість елементів
    public int Count => _elements.Count;

    // Операція індексування (повернення елемента за заданим ключем)
    public ExtendedDictionaryElement<T, U, V> this[T key]
    {
        get
        {
            foreach (var elem in _elements)
            {
                if (EqualityComparer<T>.Default.Equals(elem.Key, key))
                {
                    return elem;
                }
            }
            throw new KeyNotFoundException($"Елемент із ключем '{key}' не знайдено.");
        }
    }

    // Додавання елемента у словник
    public void Add(T key, U value1, V value2)
    {
        if (ContainsKey(key))
        {
            throw new ArgumentException($"Елемент із ключем '{key}' вже існує у словнику.");
        }

        _elements.Add(new ExtendedDictionaryElement<T, U, V>(key, value1, value2));
    }

    // Видалення елемента з словника за заданим ключем
    public bool Remove(T key)
    {
        for (int i = 0; i < _elements.Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(_elements[i].Key, key))
            {
                _elements.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    // Перевірка наявності елемента із заданим ключем
    public bool ContainsKey(T key)
    {
        foreach (var elem in _elements)
        {
            if (EqualityComparer<T>.Default.Equals(elem.Key, key))
            {
                return true;
            }
        }
        return false;
    }

    // Перевірка наявності елемента із заданим значенням (значення1 та значення2)
    public bool ContainsValues(U value1, V value2)
    {
        foreach (var elem in _elements)
        {
            bool isVal1Equal = EqualityComparer<U>.Default.Equals(elem.Value1, value1);
            bool isVal2Equal = EqualityComparer<V>.Default.Equals(elem.Value2, value2);

            if (isVal1Equal && isVal2Equal)
            {
                return true;
            }
        }
        return false;
    }

    // Реалізація IEnumerable для роботи в циклах foreach
    public IEnumerator<ExtendedDictionaryElement<T, U, V>> GetEnumerator()
    {
        return _elements.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}