# NikKit.IEnumerableExtensions

A lightweight C# library designed to provide powerful, performance-oriented extension methods for IEnumerable. This library focuses on efficient casting, random selection, and deep comparison of collections.

---

## 🚀 Key Features

* **Smart Casting**: Convert to List, Array, or IList without redundant allocations if the source is already of that type.
* **Deep Difference Discovery**: Find differences between two collections using a specialized record equality comparer.
* **Order-Independent Comparison**: Compare objects containing nested collections where the order of items doesn't matter.
* **Random Access**: Easily grab a random element from any enumerable with built-in validation.

---

## 🛠 Usage

### 1. Efficient Casting (CastingExtensions.cs)
Avoid unnecessary .ToList() or .ToArray() calls. If the object is already the target type, it returns the original reference; otherwise, it creates a copy using collection expressions.

- **AsList()**: Returns source as List or creates a new one.
- **AsArray()**: Returns source as Array or creates a new one.
- **AsIList()**: Returns source as IList, supporting existing arrays and lists.

### 2. Random Elements (EnumerableRandomExtension.cs)
Quickly pick a random item from a collection. Throws ArgumentNullException if the source is null and ArgumentException if it is empty.

### 3. Finding Differences (ComparerExtension.cs)
Find the symmetric difference between two collections (items in A not in B, and vice versa). It identifies unique elements regardless of order.

### 4. Deep Record Comparison (RecordEqualityComparer.cs)
A specialized IEqualityComparer that uses reflection to compare properties. It handles nested IEnumerable properties as sets.

* **Property Reflection**: Iterates through all public properties of the type to compare values.
* **Collection Support**: Specifically handles IEnumerable (excluding strings) by comparing contents as sets (ignoring order).
* **Consistent Hashing**: Generates hash codes by ordering collection elements, ensuring that [1, 2] and [2, 1] produce the same hash.

---
