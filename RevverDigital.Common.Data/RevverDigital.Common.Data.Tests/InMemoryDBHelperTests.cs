using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace RevverDigital.Common.Data.Tests;

public class InMemoryDBHelperTests
{
    Fixture _fixture;

    public InMemoryDBHelperTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Clear();

        var allowedRecursionDepth = 1;
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior(allowedRecursionDepth));
    }

    [Fact(DisplayName = "InMemoryDBHelper creates db creates usable in-memory db context")]
    public async Task InMemoryDBHelperTests_HappyPath1()
    {
        var db = new InMemoryDBHelper<TestDbContext>();

        var fakeProductList = _fixture
            .Build<Product>()
            .CreateMany(10)
            .ToList();

        var dbSet = db.Context.Set<Product>();

        await dbSet.AddRangeAsync(fakeProductList);
        db.Context.SaveChanges();

        var firstProduct = await dbSet.FirstOrDefaultAsync();
        firstProduct.Should().NotBeNull();
        firstProduct.Should().BeEquivalentTo(fakeProductList[0]);

        dbSet.Remove(firstProduct);
        db.Context.SaveChanges();

        var remainingProducts = await dbSet.ToListAsync();
        remainingProducts.Should().NotBeNull();
        remainingProducts.Should().HaveCount(9);
    }

    [Fact(DisplayName = "Reset method removes all data from the db context")]
    public async Task Reset_HappyPath1()
    {
        var db = new InMemoryDBHelper<TestDbContext>();

        var fakeProductList = _fixture
            .Build<Product>()
            .CreateMany(10)
            .ToList();

        var dbSet = db.Context.Set<Product>();

        await dbSet.AddRangeAsync(fakeProductList);
        db.Context.SaveChanges();

        var startingProducts = await dbSet.ToListAsync();
        startingProducts.Should().NotBeNull();
        startingProducts.Should().HaveCount(10);

        db.Reset();

        var resetProducts = await dbSet.ToListAsync();
        resetProducts.Should().BeEmpty();
    }
}
