using NikKit.IEnumerableExtensions;

namespace NikKit.IEnumerableExtensionsTests;

public class CollectionComparerTests
{
    private readonly List<MainRecord> _list1;
    private readonly List<MainRecord> _list2;

    public CollectionComparerTests()
    {
        _list1 =
        [
            new(1, "First", [new(1, "Item1"), new(2, "Item2")]),
            new(2, "Second", [new(3, "Item3")]),
            new(3, "Third", [new(4, "Item4")])
        ];

        _list2 =
        [
            new(1, "First", [new(1, "Item1"), new(2, "Item2")]),
            new(2, "SecondModified", [new(3, "Item3")]),
            new(4, "Fourth", [new(5, "Item5")])
        ];
    }

    // Пустые коллекции → различий нет
    [Fact]
    public void FindDifferences_WhenBothCollectionsEmpty_ReturnsEmpty()
    {
        // Act
        var differences = Enumerable.Empty<MainRecord>().FindDifferences([]);

        // Assert
        Assert.Empty(differences);
    }

    // Одинаковые записи → различий нет
    [Fact]
    public void FindDifferences_WhenCollectionsAreEqual_ReturnsEmpty()
    {
        // Arrange
        var copyOfList1 = new List<MainRecord>(_list1);

        // Act
        var differences = _list1.FindDifferences(copyOfList1);

        // Assert
        Assert.Empty(differences);
    }

    // Разные простые поля (Name) → возвращает изменённые записи
    [Fact]
    public void FindDifferences_WhenSimpleFieldsDiffer_ReturnsChangedRecords()
    {
        // Arrange
        List<MainRecord> modifiedList = new(_list1)
                                        {
                                            [1] = _list1[1] with { Name = "SecondModified" }
                                        };

        // Act
        var differences = _list1.FindDifferences(modifiedList).AsList();

        // Assert
        Assert.Equivalent(2, differences.Count);    // Оригинал и изменённая версия
        Assert.Contains(_list1[1], differences);
        Assert.Contains(modifiedList[1], differences);
    }

    // Разные вложенные коллекции → возвращает записи с разными Items
    [Fact]
    public void FindDifferences_WhenNestedCollectionsDiffer_ReturnsRecordsWithDifferentItems()
    {
        // Arrange
        List<MainRecord> modifiedList = new(_list1)
                                        {
                                            [0] = _list1[0] with { Items = [new(1, "Item1Changed")] }
                                        };

        // Act
        var differences = _list1.FindDifferences(modifiedList).AsList();

        // Assert
        Assert.Equivalent(2, differences.Count);    // Оригинал и изменённая версия
        Assert.Contains(_list1[0], differences);
        Assert.Contains(modifiedList[0], differences);
    }

    // Одинаковые данные, но разный порядок в Items → считается одинаковым (если порядок не важен)
    [Fact]
    public void FindDifferences_WhenItemsOrderDiffersButContentSame_ReturnsEmpty()
    {
        // Arrange
        List<MainRecord> listWithReorderedItems = [_list1[0] with { Items = [new(2, "Item2"), new(1, "Item1")] }];
        List<MainRecord> orderedList = [_list1[0]];

        // Act
        var differences = orderedList.FindDifferences(listWithReorderedItems);

        // Assert
        Assert.Empty(differences);
    }

    // Одинаковые данные, но разный порядок в Items → считается одинаковым (если порядок не важен)
    [Fact]
    public void FindDifferences_WhenDifferentCountOfNestedCollection_ReturnsEmpty()
    {
        // Arrange
        List<MainRecord> cuttedList = [_list1[0]];

        // Act
        var differences = _list1.FindDifferences(cuttedList).AsList();

        // Assert
        Assert.Equivalent(2, differences.Count);
        Assert.Contains(differences[0], _list1);
        Assert.Contains(differences[1], _list1);
    }

    // Частичное совпадение → возвращает только различающиеся элементы
    [Fact]
    public void FindDifferences_WhenPartialOverlap_ReturnsNonMatchingRecords()
    {
        // Act
        var differences = _list1.FindDifferences(_list2).AsList();

        // Assert
        Assert.Equivalent(4, differences.Count);
        Assert.Contains(_list1[1], differences);
        Assert.Contains(_list2[1], differences);
        Assert.Contains(_list1[2], differences);
        Assert.Contains(_list2[2], differences);
    }

    [Fact]
    public void GetChanges_ForList_ReturnsEmpty()
    {
        // Arrange.
        List<int> a = [1, 2, 3];
        List<int> b = [3, 2, 1];

        // Act.
        bool areEqual = a.FindDifferences(b).Any();

        // Assert.
        Assert.False(areEqual);
    }

    [Fact]
    public void GetChanges_ForArray_ReturnsEmpty()
    {
        // Arrange.
        int[] a = [1, 2, 3];
        int[] b = [3, 2, 1];

        // Act.
        bool areEqual = a.FindDifferences(b).Any();

        // Assert.
        Assert.False(areEqual);
    }

    [Fact]
    public void GetChanges_ForHashSet_ReturnsEmpty()
    {
        // Arrange.
        HashSet<int> a = [1, 2, 3];
        HashSet<int> b = [3, 2, 1];

        // Act.
        bool areEqual = a.FindDifferences(b).Any();

        // Assert.
        Assert.False(areEqual);
    }
}

internal record Item(int Id, string Name);
internal record MainRecord(int Id, string Name, List<Item> Items);