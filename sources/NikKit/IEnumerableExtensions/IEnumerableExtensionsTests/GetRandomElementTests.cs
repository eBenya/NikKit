using System.Diagnostics;
using NikKit.IEnumerableExtensions;

namespace NikKit.IEnumerableExtensionsTests;

public sealed class GetRandomElementTests
{
    [Fact]
    public void GetRandomElement_FromList_ReturnsElementFromCollection()
    {
        // Arrange
        List<int> list = [1, 2, 3, 4, 5];

        // Act
        var result = list.GetRandomElement();

        // Assert
        Assert.Contains(result, list);
    }

    [Fact]
    public void GetRandomElement_FromArray_ReturnsElementFromCollection()
    {
        // Arrange
        string[] array = ["a", "b", "c", "d"];

        // Act
        var result = array.GetRandomElement();

        // Assert
        Assert.Contains(result, array);
    }

    [Fact]
    public void GetRandomElement_FromReadOnlyList_ReturnsElementFromCollection()
    {
        // Arrange
        IReadOnlyList<string> readOnlyList = ["x", "y", "z"];

        // Act
        var result = readOnlyList.GetRandomElement();

        // Assert
        Assert.Contains(result, readOnlyList);
    }

    [Fact]
    public void GetRandomElement_FromSingleElementCollection_ReturnsThatElement()
    {
        // Arrange
        var singleElementList = new List<string> { "only_one" };

        // Act
        var result = singleElementList.GetRandomElement();

        // Assert
        Assert.Equivalent("only_one", result);
    }

    [Fact]
    public void GetRandomElement_FromEmptyCollection_ThrowsException()
    {
        // Arrange
        List<int> emptyList = [];

        // Act & Assert
        Assert.Throws<ArgumentException>(() => emptyList.GetRandomElement());
    }

    [Fact]
    public void GetRandomElement_FromNullCollection_ThrowsException()
    {
        // Arrange
        List<int>? nullList = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => nullList!.GetRandomElement());
    }

    [Fact]
    public void GetRandomElement_Distribution_ShouldBeUniform()
    {
        // Arrange
        var collection = new[] { 1, 2, 3 };
        var results = new Dictionary<int, int>
        {
            [1] = 0,
            [2] = 0,
            [3] = 0
        };
        const int iterations = 10000;

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var result = collection.GetRandomElement();
            results[result]++;
        }

        // Assert - проверяем равномерность распределения для ВСЕХ элементов
        var expectedCount = iterations / 3;
        var tolerance = expectedCount * 0.1; // 10% допуск

        foreach (var count in results.Values)
        {
            Assert.InRange(count, expectedCount - tolerance, expectedCount + tolerance);
        }
    }

    [Fact]
    public void GetRandomElement_WithLargeCollection_PerformsAcceptably()
    {
        // Arrange
        var largeList = Enumerable.Range(0, 100000).ToList();
        var stopwatch = new Stopwatch();

        // Act & Assert - не должно занимать больше 100ms
        stopwatch.Start();
        for (var i = 0; i < 1000; i++)
        {
            largeList.GetRandomElement();
        }

        stopwatch.Stop();
        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds <= 100,
                    $"Operation took {stopwatch.ElapsedMilliseconds}ms, expected <= 100ms");
    }

    [Fact]
    public void GetRandomElement_FromNullEnumerable_ThrowsArgumentException()
    {
        // Arrange
        IEnumerable<int> emptyEnumerable = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => emptyEnumerable.GetRandomElement());
    }

    [Fact]
    public void GetRandomElement_FromEmptyEnumerable_ThrowsArgumentException()
    {
        // Arrange
        var emptyEnumerable = Enumerable.Empty<object>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => emptyEnumerable.GetRandomElement());
    }


    [Fact]
    public void GetRandomElement_WithComplexObjects_WorksCorrectly()
    {
        // Arrange
        var persons = new List<Person>
        {
            new() { Id = 1, Name = "John" },
            new() { Id = 2, Name = "Jane" },
            new() { Id = 3, Name = "Bob" }
        };

        // Act
        var result = persons.GetRandomElement();

        // Assert
        Assert.Contains(persons, p => p.Id == result.Id && p.Name == result.Name);
    }

    private class Person
    {
        public int Id { get; init; }
        public required string Name { get; init; }
    }
}