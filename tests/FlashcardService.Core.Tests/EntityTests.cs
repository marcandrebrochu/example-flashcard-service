using FlashcardService.Core.Entities;

namespace FlashcardService.Core.Tests;

public class EntityTests
{
    private class ConcreteType1(Guid id) : Entity(id)
    {
        public int SomeProp { get; set; } = 0;
    }
    
    [Fact]
    public void TwoEntitiesOfSameTypeWithSameIdsShouldBeEqual()
    {
        var id = Guid.NewGuid();
        var x = new ConcreteType1(id);
        var y = new ConcreteType1(id);
        var z = new ConcreteType1(id) { SomeProp = 1 };
        
        Assert.Equal(x, y);
        Assert.Equal(x, z);
        Assert.True(x == y);
        Assert.True(x == z);
        Assert.False(x != y);
        Assert.False(x != z);
    }

    [Fact]
    public void TwoEntitiesOfSameTypeWithDifferentIdShouldNotBeEqual()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var x = new ConcreteType1(id1);
        var y = new ConcreteType1(id2);
        
        Assert.NotEqual(x, y);
        Assert.False(x == y);
        Assert.True(x != y);
    }
}