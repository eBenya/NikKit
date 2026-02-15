using System.Collections.Generic;
using System.Linq;
using NikKit.IEnumerableExtensions.Comparers;

namespace NikKit.IEnumerableExtensions;

/// <summary>
///
/// </summary>
public static class ComparerExtension
{
    /// <summary>
    /// Find difference between collections
    /// </summary>
    /// <param name="second"></param>
    /// <param name="source"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IEnumerable<TSource> FindDifferences<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> second)
    {
        IEnumerable<TSource> enumerable1 = source.AsArray();
        IEnumerable<TSource> enumerable2 = second.AsArray();
        var comparer = new RecordEqualityComparer<TSource>();
        return enumerable1.Except(enumerable2, comparer)
                          .Concat(enumerable2.Except(enumerable1, comparer))
                          .Distinct();
    }
}