using NikKit.EntityAbstractionTests.ExampleModels;

namespace NikKit.EntityAbstractionTests;

public class EntityHelperTestsMatchBeforeAdd
{
    [Fact]
    public void Merge_DeepHierarchy_ShouldCorrectlyPropagateIdsViaSpecificMappings()
    {
        // Arrange
        var l1Id = Guid.NewGuid();
        var l2Id = Guid.NewGuid();

        var existingL2 = L2.Create(l2Id) with { L1Id = l1Id };
        var existingRoot = L1.Create(l1Id);
        existingRoot.Children.Add(existingL2);

        var database = new List<L1> { existingRoot };

        // Входящие данные: тот же корень, тот же L2, но у L2 появились новые L3 и L4
        var incomingRoot = L1.Create(Guid.NewGuid());
        var incomingL2 = L2.Create(Guid.NewGuid()) with { L1Id = incomingRoot.Id };
        var incomingL3 = L3.Create(Guid.NewGuid()) with { L1Id = incomingRoot.Id, L2Id = incomingL2.Id };
        var incomingL4 = L4.Create(Guid.NewGuid()) with
        {
            L1Id = incomingRoot.Id, L2Id = incomingL2.Id, L3Id = incomingL3.Id
        };

        incomingL3.Children.Add(incomingL4);

        incomingL2.Children.Add(incomingL3);

        incomingRoot.Children.Add(incomingL2);

        // Act
        existingRoot.Update(incomingRoot, DateTime.UtcNow, true);

        // Assert
        var resultRoot = database.First();
        var resultL2 = resultRoot.Children.First();
        var resultL3 = resultL2.Children.First();
        var resultL4 = resultL3.Children.First();

        Assert.Equal(resultRoot.Id, existingRoot.Id);

        Assert.Equal(resultL2.L1Id, l1Id);
        Assert.Equal(resultL2.Id, l2Id);

        Assert.Equal(resultL3.L1Id, l1Id);
        Assert.Equal(resultL3.L2Id, l2Id);
        Assert.Equal(resultL3.Id, Guid.Empty);

        Assert.Equal(resultL4.L1Id, l1Id);
        Assert.Equal(resultL4.L2Id, l2Id);
        Assert.Equal(resultL4.L3Id, Guid.Empty);
        Assert.Equal(resultL4.Id, Guid.Empty);
    }
}