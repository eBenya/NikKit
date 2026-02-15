using System.Collections.Generic;

namespace NikKit.IEnumerableExtensions;

public static class CastingExtensions
{
    /// <summary>
    /// Obtains the data as a list; if it is *already* a list, the original object is returned without
    /// any duplication; otherwise, ToList() is invoked.
    /// </summary>
    /// <param name="source">The enumerable to return as a list.</param>
    /// <typeparam name="T">The type of element in the list.</typeparam>
    public static List<T> AsList<T>(this IEnumerable<T>? source)
    {
        if(source == null)
            return [];

        return source as List<T> ?? [..source];
    }

    /// <summary>
    /// Obtains the data as an array; if it is *already* a list, the original object is returned without
    /// any duplication; otherwise, ToArray() is invoked.
    /// </summary>
    /// <param name="source">The enumerable to return as a list.</param>
    /// <typeparam name="T">The type of element in the list.</typeparam>
    public static T[] AsArray<T>(this IEnumerable<T>? source)
    {
        if(source == null)
            return [];

        return source as T[] ?? [..source];
    }

    /// <summary>
    /// Obtains the data as an IList; if it is *already* a list, the original object is returned without
    /// any duplication; otherwise, ToList() is invoked.
    /// </summary>
    /// <param name="source">The enumerable to return as a list.</param>
    /// <typeparam name="T">The type of element in the list.</typeparam>
    /// <returns></returns>
    public static IList<T> AsIList<T>(this IEnumerable<T>? source)
    {
        if(source == null)
            return [];

        return source switch
        {
            T[] array     => array,
            List<T> list  => list,
            IList<T> list => list,
            _             => [..source]
        };
    }
}