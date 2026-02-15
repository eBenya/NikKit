using NikKit.IEnumerableExtensions;

namespace NikKit.IEnumerableExtensionsTests;

public class CastingExtensionTests
{
    [Fact]
    public void AsList_CallWithNotNullEnumeration_ReturnsThisList()
    {
        // Arrange...
        IEnumerable<int> enumeration = new List<int> { 1 };

        // Act...
        var sut = enumeration.AsList();

        // Assert...
        
        Assert.NotEmpty(sut);
    }

    [Fact]
    public void AsList_CallWithNullEnumeration_ReturnsThisEmptyList()
    {
        // Arrange...
        IEnumerable<int>? enumeration = null;

        // Act...
        var sut = enumeration.AsList();

        // Assert...
        Assert.Empty(sut);
    }

    [Fact]
    public void AsList_CallWithArray_ReturnsNewList()
    {
        // Arrange...
        int[] enumeration = [1];

        // Act...
        var sut = enumeration.AsList();

        // Assert...
        Assert.Equivalent(enumeration, sut);
    }

    [Fact]
    public void AsArray_CallWithNotNullEnumeration_ReturnsThisArray()
    {
        // Arrange...
        IEnumerable<int> enumeration = [1];

        // Act...
        var sut = enumeration.AsArray();

        // Assert...
        Assert.NotEmpty(sut);
    }

    [Fact]
    public void AsArray_CallWithNullEnumeration_ReturnsThisEmptyArray()
    {
        // Arrange...
        IEnumerable<int>? enumeration = null;

        // Act...
        var sut = enumeration.AsArray();

        // Assert...
        Assert.Empty(sut);
    }

    [Fact]
    public void AsArray_CallWithAList_ReturnsNewArray()
    {
        // Arrange...
        List<int> enumeration = [1];

        // Act...
        var sut = enumeration.AsArray();

        // Assert...
        Assert.Equivalent(enumeration, sut);
    }
    
    [Fact]
    public void AsIList_CallWithNotNullEnumeration_ReturnsThisArray()
    {
        // Arrange...
        IEnumerable<int> enumeration = [1];

        // Act...
        var sut = enumeration.AsIList();

        // Assert...
        Assert.NotEmpty(sut);
    }

    [Fact]
    public void AsIList_CallWithNullEnumeration_ReturnsThisEmptyArray()
    {
        // Arrange...
        IEnumerable<int>? enumeration = null;

        // Act...
        var sut = enumeration.AsIList();

        // Assert...
        Assert.Empty(sut);
    }

    [Fact]
    public void AsIList_CallWithAList_ReturnsNewArray()
    {
        // Arrange...
        List<int> enumeration = [1];

        // Act...
        var sut = enumeration.AsIList();

        // Assert...
        Assert.Equivalent(enumeration, sut);
    }
}