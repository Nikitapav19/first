using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CustomCollection<T> : IEnumerable<T>
{
    private readonly List<T> _items = new();

    public void Add(T item) => _items.Add(item);
    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<T> GetReverseEnumerator()
    {
        for (int i=_items.Count - 1; i>=0; i--)
        {
            yield return _items[i];
        }
    }
    public static IEnumerable<int> GenerateSequence(int start, int count)
    {
        for (int i=start; i<start+count; i++)
        {
            yield return i;
        }
    }

    public IEnumerable<T> FilterAndSort(Func<T, bool> predicate, Func<T, IComparable> keySelector)
    {
        var sorted_item = _items.Where(predicate).OrderBy(keySelector);
        foreach (var item in sorted_item)
        {
            yield return item;
        }
    }

}