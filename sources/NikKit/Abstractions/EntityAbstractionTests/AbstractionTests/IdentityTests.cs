using AutoBogus;
using NikKit.EntityAbstractionTests.ExampleModels;

namespace NikKit.EntityAbstractionTests.AbstractionTests;

public class IdentityTests
{
    [Fact]
    public void IdentityShouldBeCreatedCorrectly()
    {
        // Arrange
        var faker = new AutoFaker<Country>()
                    .UseSeed(12345);
        
        var entity = faker.Generate();

        // Act
        var identity = entity.Identity;

        // Assert
        Assert.Equal(entity.IsoCode, identity.IsoCode);
    }
}