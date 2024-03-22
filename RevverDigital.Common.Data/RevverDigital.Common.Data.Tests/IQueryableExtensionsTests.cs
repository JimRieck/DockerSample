using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RevverDigital.Common.Data.Repositories;
using RevverDigital.Common.Data.Extensions;

namespace RevverDigital.Common.Data.Tests;

public class IQueryableExtensionsTests
{
    Fixture _fixture;

    public IQueryableExtensionsTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Clear();

        var allowedRecursionDepth = 1;
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior(allowedRecursionDepth));
    }

    [Fact(DisplayName = "ExpandNavigationProperties populates Category objects when nagivation properties and expand both list category property")]
    public async Task ExpandNavigationProperties_HappyPath1()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var fakeProductList = _fixture
            .Build<Product>()
            .CreateMany(10)
            .ToList();

        db.Context.AddRange(fakeProductList);
        db.Context.SaveChanges();
        
        var products = await db.Context
            .Set<Product>()
            .ExpandNavigationProperties(new Dictionary<string, string> { { "category", "Category" } }, "category")
            .AsNoTracking()
            .ToListAsync();

        products.ForEach(x => x.Category.Should().NotBeNull());
    }

    [Fact(DisplayName = "ExpandNavigationProperties does not populate Category objects when navigation properties is null")]
    public async Task ExpandNavigationProperties_HappyPath2()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var fakeProductList = _fixture
            .Build<Product>()
            .CreateMany(10)
            .ToList();

        db.Context.AddRange(fakeProductList);
        db.Context.SaveChanges();

        var products = await db.Context
            .Set<Product>()
            .ExpandNavigationProperties(null, "category")
            .AsNoTracking()
            .ToListAsync();

        products.ForEach(x => x.Category.Should().BeNull());
    }

    [Fact(DisplayName = "ExpandNavigationProperties does not populate Category objects when expand is null")]
    public async Task ExpandNavigationProperties_HappyPath3()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var fakeProductList = _fixture
            .Build<Product>()
            .CreateMany(10)
            .ToList();

        db.Context.AddRange(fakeProductList);
        db.Context.SaveChanges();

        var products = await db.Context
            .Set<Product>()
            .ExpandNavigationProperties(new Dictionary<string, string> { { "category", "Category" } }, null)
            .AsNoTracking()
            .ToListAsync();

        products.ForEach(x => x.Category.Should().BeNull());
    }

    [Fact(DisplayName = "ExpandNavigationProperties populates Category objects when nagivation properties does not contain expand property")]
    public async Task ExpandNavigationProperties_HappyPath4()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var fakeProductList = _fixture
            .Build<Product>()
            .CreateMany(10)
            .ToList();

        db.Context.AddRange(fakeProductList);
        db.Context.SaveChanges();

        var products = await db.Context
            .Set<Product>()
            .ExpandNavigationProperties(new Dictionary<string, string> { { "category", "Category" } }, "notcategory")
            .AsNoTracking()
            .ToListAsync();

        products.ForEach(x => x.Category.Should().BeNull());
    }
}
