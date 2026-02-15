using System;
using System.Collections.Generic;

namespace NikKit.IEnumerableExtensions;

public static class EnumerableRandomExtension
{
    private static readonly Random Random = new();

    /// <summary>
    /// Get random element from collection
    /// </summary>
    /// <returns></returns>
    public static TSource GetRandomElement<TSource>(this IEnumerable<TSource> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source), "Collection cannot be null");

        // Упрощенный подход для остальных случаев
        var tempList = source.AsIList();
        if (tempList.Count == 0)
            throw new ArgumentException("Collection cannot be empty");

        return tempList[Random.Next(tempList.Count)];
    }
}