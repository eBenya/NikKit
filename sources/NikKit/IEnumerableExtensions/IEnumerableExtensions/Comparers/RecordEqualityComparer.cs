using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NikKit.IEnumerableExtensions.Comparers;

/// <summary>
/// Comparison of records with internal collections without reference to order.
/// </summary>
/// <typeparam name="T"></typeparam>
public class RecordEqualityComparer<T> : IEqualityComparer<T>
{
    /// <inheritdoc />
    public bool Equals(T? x, T? y)
    {
        if (ReferenceEquals(x, y)) { return true; }

        if (object.Equals(x, default(T)) || object.Equals(y, default(T))) { return false; }

        var type = typeof(T);
        foreach (var property in type.GetProperties())
        {
            var valueX = property.GetValue(x);
            var valueY = property.GetValue(y);

            if (property.PropertyType != typeof(string)
             && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                // Сравнение коллекций как множеств (без учёта порядка)
                var collectionX = valueX as IEnumerable<object>;
                var collectionY = valueY as IEnumerable<object>;

                if (collectionX == null && collectionY == null)
                    continue;
                if (collectionX == null || collectionY == null)
                    return false;

                if (!CollectionEquals(collectionX.ToList(), collectionY.ToList()))
                    return false;
            }
            else if (!Equals(valueX, valueY))
            {
                return false;
            }
        }

        return true;
    }

    private static bool CollectionEquals(IEnumerable<object>? x, IEnumerable<object>? y)
    {
        if (x == null && y == null) { return true; }
        if (x == null || y == null) { return false; }

        var setX = new HashSet<object>(x);
        var setY = new HashSet<object>(y);

        return setX.SetEquals(setY);
    }

    /// <inheritdoc />
    public int GetHashCode(T obj)
    {
        if (object.Equals(obj, default(T))) { return 0; }

        unchecked
        {
            var hash = 17;
            var type = typeof(T);
            foreach (var property in type.GetProperties())
            {
                var value = property.GetValue(obj);
                if (value == null) { continue; }

                if (property.PropertyType != typeof(string)
                 && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    // Хеш для коллекции, без привязки к порядку
                    var collection = (IEnumerable<object>)value;
                    var collectionHash = 0;
                    foreach (var item in collection.OrderBy(x => x.GetHashCode()))
                    {
                        collectionHash = (collectionHash * 31) ^ (item.GetHashCode());
                    }
                    hash = (hash * 23) + collectionHash;
                }
                else
                {
                    hash = (hash * 23) + value.GetHashCode();
                }
            }
            return hash;
        }
    }
}