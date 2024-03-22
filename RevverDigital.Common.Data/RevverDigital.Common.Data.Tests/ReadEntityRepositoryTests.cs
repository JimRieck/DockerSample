using AutoFixture;
using FluentAssertions;
using RevverDigital.Common.Data.Repositories;

namespace RevverDigital.Common.Data.Tests;

public class ReadEntityRepositoryTests
{
    Fixture _fixture;

    public ReadEntityRepositoryTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Clear();

        var allowedRecursionDepth = 1;
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior(allowedRecursionDepth));
    }

    [Fact(DisplayName = "ExistsAsync method returns true when there is a matching id")]
    public async Task ExistsAsync_HappyPath1()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var id = Guid.NewGuid();
        var fakeProduct = _fixture
            .Build<Product>()
            .With(x => x.Id, id)
            .Without(x => x.Category)
            .With(x => x.IsDeleted, false)
            .Create();

        db.Context.Add(fakeProduct);
        db.Context.SaveChanges();

        var productExists = await repo.ExistsAsync(id);

        productExists.Should().BeTrue();
    }

    [Fact(DisplayName = "ExistsAsync method returns false when there is no matching id")]
    public async Task ExistsAsync_HappyPath2()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var fakeProduct = _fixture
            .Build<Product>()
            .Without(x => x.Category)
            .With(x => x.IsDeleted, false)
            .Create();

        db.Context.Add(fakeProduct);
        db.Context.SaveChanges();

        var productExists = await repo.ExistsAsync(Guid.NewGuid());

        productExists.Should().BeFalse();
    }

    [Fact(DisplayName = "ExistsAsync method throws exception when Id is null")]
    public async Task ExistsAsync_SadPath1()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Category, string>(db.Context);

        var action = async () => await repo.ExistsAsync(null);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "ExistsAsync method throws exception when Id is a default value")]
    public async Task ExistsAsync_SadPath2()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var action = async () => await repo.ExistsAsync(Guid.Empty);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }


    [Fact(DisplayName = "GetAllAsync returns all Products without Category when not expanded")]
    public async Task GetAll_HappyPath1()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var numItems = 5;
        var fakeCategoryList = _fixture
            .Build<Category>()
            .With(x => x.IsDeleted, false)
            .Without(x => x.Products)
            .CreateMany(numItems)
            .ToList();

        var fakeProductList = _fixture
            .Build<Product>()
            .With(x => x.IsDeleted, false)
            .Without(x => x.Category)
            .CreateMany(numItems)
            .ToList();

        for (int i = 0; i < numItems; i++)
            fakeProductList[i].Category = fakeCategoryList[i]; 

        db.Context.AddRange(fakeProductList);
        db.Context.SaveChanges();

        var products = (await repo.GetAllAsync()).ToList();

        products.Should().BeEquivalentTo(fakeProductList, options => options.Excluding(x => x.Category));
        products.ForEach(x => x.Category.Should().BeNull());
    }

    [Fact(DisplayName = "GetAllAsync returns all Products with Categories populated when expand is 'category'")]
    public async Task GetAll_HappyPath2()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var numItems = 5;
        var fakeCategoryList = _fixture
            .Build<Category>()
            .With(x => x.IsDeleted, false)
            .Without(x => x.Products)
            .CreateMany(numItems)
            .ToList();

        var fakeProductList = _fixture
            .Build<Product>()
            .With(x => x.IsDeleted, false)
            .Without(x => x.Category)
            .CreateMany(numItems)
            .ToList();

        for (int i = 0; i < numItems; i++)
            fakeProductList[i].Category = fakeCategoryList[i];

        db.Context.AddRange(fakeProductList);
        db.Context.SaveChanges();

        var products = (await repo.GetAllAsync("category")).ToList();

        products.Should().BeEquivalentTo(fakeProductList, options => options.Excluding(x => x.Category));
        products.ForEach(x => x.Category.Should().NotBeNull());

    }

    [Fact(DisplayName = "GetAllAsync returns only non-deleted Products")]
    public async Task GetAll_HappyPath3()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var numItems = 5;
        var fakeProductList = _fixture
            .Build<Product>()
            .With(x => x.IsDeleted, false)
            .Without(x => x.Category)
            .CreateMany(numItems)
            .ToList();

        var fakeProductListDeleted = _fixture
            .Build<Product>()
            .With(x => x.IsDeleted, true)
            .Without(x => x.Category)
            .CreateMany(numItems)
            .ToList();

        db.Context.AddRange(fakeProductList);
        db.Context.AddRange(fakeProductListDeleted);
        db.Context.SaveChanges();

        var products = (await repo.GetAllAsync()).ToList();

        products.Should().BeEquivalentTo(fakeProductList);
    }


    [Fact(DisplayName = "GetByIdAsync returns Product if it exists and is not deleted")]
    public async Task GetByIdAsync_HappyPath1()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var id = Guid.NewGuid();

        var fakeProduct = _fixture
            .Build<Product>()
            .Without(x => x.Category)
            .With(x => x.Id, id)
            .With(x => x.IsDeleted, false)
            .Create();

        db.Context.Add(fakeProduct);
        db.Context.SaveChanges();

        var product = await repo.GetByIdAsync(id);

        product.Should().BeEquivalentTo(fakeProduct);
    }

    [Fact(DisplayName = "GetByIdAsync returns null if Product exists and is deleted")]
    public async Task GetByIdAsync_HappyPath2()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var id = Guid.NewGuid();

        var fakeProduct = _fixture
            .Build<Product>()
            .Without(x => x.Category)
            .With(x => x.Id, id)
            .With(x => x.IsDeleted, true)
            .Create();

        db.Context.Add(fakeProduct);
        db.Context.SaveChanges();

        var product = await repo.GetByIdAsync(id);

        product.Should().BeNull();
    }

    [Fact(DisplayName = "GetByIdAsync returns null if Product does not exist")]
    public async Task GetByIdAsync_HappyPath3()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var fakeProduct = _fixture
            .Build<Product>()
            .Without(x => x.Category)
            .With(x => x.IsDeleted, true)
            .Create();

        db.Context.Add(fakeProduct);
        db.Context.SaveChanges();

        var product = await repo.GetByIdAsync(Guid.NewGuid());

        product.Should().BeNull();
    }

    [Fact(DisplayName = "GetByIdAsync returns Product without Category if expand is null")]
    public async Task GetByIdAsync_HappyPath4()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var id = Guid.NewGuid();

        var fakeProduct = _fixture
            .Build<Product>()
            .With(x => x.Id, id)
            .With(x => x.IsDeleted, false)
            .Create();

        db.Context.Add(fakeProduct);
        db.Context.SaveChanges();

        var product = await repo.GetByIdAsync(id);

        product.Should().BeEquivalentTo(fakeProduct, options => options.Excluding(x => x.Category));
        product.Category.Should().BeNull();
    }

    [Fact(DisplayName = "GetByIdAsync returns Product with Category if expand is 'category'")]
    public async Task GetByIdAsync_HappyPath5()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var id = Guid.NewGuid();

        var fakeProduct = _fixture
            .Build<Product>()
            .With(x => x.Id, id)
            .With(x => x.IsDeleted, false)
            .Create();

        db.Context.Add(fakeProduct);
        db.Context.SaveChanges();

        var product = await repo.GetByIdAsync(id, "category");

        product.Should().BeEquivalentTo(fakeProduct, options => options.Excluding(x => x.Category));
        product.Category.Should().NotBeNull();
    }

    [Fact(DisplayName = "GetByIdAsync throws exception if id is null")]
    public async Task GetByIdAsync_SadPath1()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Category, string>(db.Context);

        var action = async () => await repo.GetByIdAsync(null);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "GetByIdAsync throws exception if id is default value")]
    public async Task GetByIdAsync_SadPath2()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var action = async () => await repo.GetByIdAsync(Guid.Empty);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    
    [Fact(DisplayName = "SearchAsync returns all Products matching the query")]
    public async Task SearchAsync_HappyPath1()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var numItems = 5;
        var fakeProductList = _fixture
            .Build<Product>()
            .With(x => x.Price, 12.97M)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Category)
            .CreateMany(numItems)
            .ToList();

        var fakeProductListMatching = _fixture
            .Build<Product>()
            .With(x => x.Price, 10.15M)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Category)
            .CreateMany(numItems)
            .ToList();

        db.Context.AddRange(fakeProductList);
        db.Context.AddRange(fakeProductListMatching);
        db.Context.SaveChanges();

        var products = (await repo.SearchAsync(x => x.Price == 10.15M)).ToList();

        products.Should().BeEquivalentTo(fakeProductListMatching);
    }

    [Fact(DisplayName = "SearchAsync returns empty list if nothing matches query")]
    public async Task SearchAsync_HappyPath2()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var numItems = 5;
        var fakeProductList = _fixture
            .Build<Product>()
            .With(x => x.Price, 12.97M)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Category)
            .CreateMany(numItems)
            .ToList();

        db.Context.AddRange(fakeProductList);
        db.Context.SaveChanges();

        var products = (await repo.SearchAsync(x => x.Price == 10.15M)).ToList();

        products.Should().BeEmpty();
    }

    [Fact(DisplayName = "SearchAsync returns all Products with no Category, matching the query")]
    public async Task SearchAsync_HappyPath3()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var numItems = 5;
        var fakeProductList = _fixture
            .Build<Product>()
            .With(x => x.Price, 12.97M)
            .With(x => x.IsDeleted, false)
            .CreateMany(numItems)
            .ToList();

        var fakeProductListMatching = _fixture
            .Build<Product>()
            .With(x => x.Price, 10.15M)
            .With(x => x.IsDeleted, false)
            .CreateMany(numItems)
            .ToList();

        db.Context.AddRange(fakeProductList);
        db.Context.AddRange(fakeProductListMatching);
        db.Context.SaveChanges();

        var products = (await repo.SearchAsync(x => x.Price == 10.15M)).ToList();

        products.Should().BeEquivalentTo(fakeProductListMatching, options => options.Excluding(x => x.Category));
        products.ForEach(x => x.Category.Should().BeNull());
    }

    [Fact(DisplayName = "SearchAsync returns all Products with Category, matching the query, when expand is 'category'")]
    public async Task SearchAsync_HappyPath4()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var numItems = 5;
        var fakeProductList = _fixture
            .Build<Product>()
            .With(x => x.Price, 12.97M)
            .With(x => x.IsDeleted, false)
            .CreateMany(numItems)
            .ToList();

        var fakeProductListMatching = _fixture
            .Build<Product>()
            .With(x => x.Price, 10.15M)
            .With(x => x.IsDeleted, false)
            .CreateMany(numItems)
            .ToList();

        db.Context.AddRange(fakeProductList);
        db.Context.AddRange(fakeProductListMatching);
        db.Context.SaveChanges();

        var products = (await repo.SearchAsync(x => x.Price == 10.15M, "category")).ToList();

        products.Should().BeEquivalentTo(fakeProductListMatching, options => options.Excluding(x => x.Category));
        products.ForEach(x => x.Category.Should().NotBeNull());
    }

    [Fact(DisplayName = "SearchAsync returns all non-deleted Products, matching the query")]
    public async Task SearchAsync_HappyPath5()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new ReadEntityRepository<Product>(db.Context);

        var numItems = 5;
        var fakeProductList = _fixture
            .Build<Product>()
            .With(x => x.Price, 12.97M)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Category)
            .CreateMany(numItems)
            .ToList();

        var fakeProductListMatching = _fixture
            .Build<Product>()
            .With(x => x.Price, 10.15M)
            .With(x => x.IsDeleted, false)
            .Without(x => x.Category)
            .CreateMany(numItems)
            .ToList();

        var fakeProductListMatchingDeleted = _fixture
            .Build<Product>()
            .With(x => x.Price, 10.15M)
            .With(x => x.IsDeleted, true)
            .Without(x => x.Category)
            .CreateMany(numItems)
            .ToList();

        db.Context.AddRange(fakeProductList);
        db.Context.AddRange(fakeProductListMatching);
        db.Context.SaveChanges();

        var products = (await repo.SearchAsync(x => x.Price == 10.15M)).ToList();

        products.Should().BeEquivalentTo(fakeProductListMatching);
    }
}
