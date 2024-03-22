using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RevverDigital.Common.Data.Models.Interfaces;
using RevverDigital.Common.Data.Repositories;

namespace RevverDigital.Common.Data.Tests;

public class WriteEntityRepositoryTests
{
    Fixture _fixture;

    public WriteEntityRepositoryTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Clear();

        var allowedRecursionDepth = 1;
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior(allowedRecursionDepth));
    }

    [Fact(DisplayName = "InsertAsync method adds product to the db context")]
    public async Task InsertAsync_HappyPath1()
    {
        var userName = "SomeUserName";
        var userMock = CreateUserMock(userName);
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new WriteEntityRepository<Product>(db.Context, userMock.Object);

        var id = Guid.NewGuid();
        var fakeProduct = _fixture
            .Build<Product>()
            .With(x => x.Id, id)
            .Without(x => x.Category)
            .With(x => x.IsDeleted, false)
            .Create();

        var insertedProduct = await repo.InsertAsync(fakeProduct);

        var contextProduct = await db.Context.Set<Product>().SingleOrDefaultAsync(x => id == x.Id);

        insertedProduct.Should().BeEquivalentTo(fakeProduct);
        contextProduct.Should().BeEquivalentTo(fakeProduct);
    }

    [Fact(DisplayName = "InsertAsync method add updates IUserCreatable and IUserUpdatable fields")]
    public async Task InsertAsync_HappyPath2()
    {
        var userName = "SomeUserName";
        var userMock = CreateUserMock(userName);
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new WriteEntityRepository<Product>(db.Context, userMock.Object);

        var id = Guid.NewGuid();
        var fakeProduct = _fixture
            .Build<Product>()
            .Without(x => x.Id)
            .Without(x => x.Category)
            .Without(x => x.CreatedBy)
            .Without(x => x.CreatedDate)
            .Without(x => x.UpdatedBy)
            .Without(x => x.UpdatedDate)
            .With(x => x.IsDeleted, false)
            .Create();

        var now = DateTime.Now;
        var insertedProduct = await repo.InsertAsync(fakeProduct);

        insertedProduct.CreatedBy.Should().Be(userName);
        insertedProduct.CreatedDate.Should().BeCloseTo(now, TimeSpan.FromMilliseconds(100));
        insertedProduct.UpdatedBy.Should().Be(userName);
        insertedProduct.UpdatedDate.Should().BeCloseTo(now, TimeSpan.FromMilliseconds(100));
    }

    [Fact(DisplayName = "InsertAsync method throws exception when entity is null")]
    public async Task InsertAsync_SadPath1()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new WriteEntityRepository<Product>(db.Context, new Mock<IUser>().Object);

        var action = async () => await repo.InsertAsync(null);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "UpdateAsync method updates an existing Product")]
    public async Task UpdateAsync_HappyPath1()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new WriteEntityRepository<Product>(db.Context, new Mock<IUser>().Object);

        var id = Guid.NewGuid();
        var fakeProductOriginal = _fixture
            .Build<Product>()
            .With(x => x.Id, id)
            .With(x => x.Name, "OriginalName")
            .Without(x => x.Category)
            .With(x => x.IsDeleted, false)
            .Create();

        db.Context.Add(fakeProductOriginal);
        db.Context.SaveChanges();

        var updatedName = "UpdatedName";
        var fakeProductUpdated = _fixture
            .Build<Product>()
            .With(x => x.Id, id)
            .With(x => x.Name, updatedName)
            .Without(x => x.Category)
            .With(x => x.IsDeleted, false)
            .Create();

        var updatedProduct = await repo.UpdateAsync(fakeProductUpdated);

        var contextProduct = await db.Context.Set<Product>().SingleOrDefaultAsync(x => id == x.Id);

        contextProduct.Name.Should().Be(updatedName);
    }

    [Fact(DisplayName = "UpdateAsync method updates IUserUpdateable properties")]
    public async Task UpdateAsync_HappyPath2()
    {
        var userName = "SomeUserName";
        var userMock = CreateUserMock(userName);

        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new WriteEntityRepository<Product>(db.Context, userMock.Object);

        var id = Guid.NewGuid();
        var fakeProductOriginal = _fixture
            .Build<Product>()
            .With(x => x.Id, id)
            .With(x => x.Name, "OriginalName")
            .Without(x => x.Category)
            .With(x => x.IsDeleted, false)
            .With(x => x.UpdatedBy, "OriginallyUpdatedBy")
            .With(x => x.UpdatedDate, DateTime.Now.AddDays(-1))
            .Create();

        db.Context.Add(fakeProductOriginal);
        db.Context.SaveChanges();

        var updatedName = "UpdatedName";
        var fakeProductUpdated = _fixture
            .Build<Product>()
            .With(x => x.Id, id)
            .With(x => x.Name, updatedName)
            .Without(x => x.Category)
            .With(x => x.IsDeleted, false)
            .Create();

        var updatedProduct = await repo.UpdateAsync(fakeProductUpdated);

        var contextProduct = await db.Context.Set<Product>().SingleOrDefaultAsync(x => id == x.Id);

        contextProduct.UpdatedBy.Should().Be(userName);
        contextProduct.UpdatedDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(100));
    }

    [Fact(DisplayName = "UpdateAsync method throws exception when entity is null")]
    public async Task UpdateAsync_SadPath1()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new WriteEntityRepository<Product>(db.Context, new Mock<IUser>().Object);

        var action = async () => await repo.UpdateAsync(null);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "UpdateAsync method throws exception when no Product exists with the id")]
    public async Task UpdateAsync_SadPath2()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new WriteEntityRepository<Product>(db.Context, new Mock<IUser>().Object);

        var id = Guid.NewGuid();
        var fakeProductOriginal = _fixture
            .Build<Product>()
            .With(x => x.Name, "OriginalName")
            .Without(x => x.Category)
            .With(x => x.IsDeleted, false)
            .With(x => x.UpdatedBy, "OriginallyUpdatedBy")
            .With(x => x.UpdatedDate, DateTime.Now.AddDays(-1))
            .Create();

        db.Context.Add(fakeProductOriginal);
        db.Context.SaveChanges();

        var action = async () => await repo.UpdateAsync(new Product { Id = Guid.NewGuid() });

        await action.Should().ThrowAsync<Exception>().WithMessage("Entity cannot be updated. Not entity exists with the given id.");
    }

    [Fact(DisplayName = "DeleteByIdAsync method sets IsDeleted")]
    public async Task DeleteByIdAsync_HappyPath1()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new WriteEntityRepository<Product>(db.Context, new Mock<IUser>().Object);

        var id = Guid.NewGuid();
        var fakeProduct = _fixture
            .Build<Product>()
            .With(x => x.Id, id)
            .Without(x => x.Category)
            .With(x => x.IsDeleted, false)
            .Create();

        db.Context.Add(fakeProduct);
        db.Context.SaveChanges();

        await repo.DeleteByIdAsync(id);

        var contextProduct = await db.Context.Set<Product>().SingleOrDefaultAsync(x => id == x.Id);

        contextProduct.IsDeleted.Should().BeTrue();
    }

    [Fact(DisplayName = "DeleteByIdAsync method updates IUserUpdateable properties")]
    public async Task DeleteByIdAsync_HappyPath2()
    {
        var userName = "SomeUserName";
        var userMock = CreateUserMock(userName);

        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new WriteEntityRepository<Product>(db.Context, userMock.Object);

        var id = Guid.NewGuid();
        var fakeProduct = _fixture
            .Build<Product>()
            .With(x => x.Id, id)
            .Without(x => x.Category)
            .With(x => x.IsDeleted, false)
            .Create();

        db.Context.Add(fakeProduct);
        db.Context.SaveChanges();

        await repo.DeleteByIdAsync(id);

        var contextProduct = await db.Context.Set<Product>().SingleOrDefaultAsync(x => id == x.Id);

        contextProduct.UpdatedBy.Should().Be(userName);
        contextProduct.UpdatedDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(100));
    }

    [Fact(DisplayName = "DeleteByIdAsync method throws exception when id is null")]
    public async Task DeleteByIdAsync_SadPath1()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new WriteEntityRepository<Category, string>(db.Context, new Mock<IUser>().Object);

        var action = async () => await repo.DeleteByIdAsync(null);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "DeleteByIdAsync method throws exception when id is default value")]
    public async Task DeleteByIdAsync_SadPath2()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new WriteEntityRepository<Product>(db.Context, new Mock<IUser>().Object);

        var action = async () => await repo.DeleteByIdAsync(Guid.Empty);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "DeleteByIdAsync method throws exception when no Product exists with the id")]
    public async Task DeleteByIdAsync_SadPath3()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new WriteEntityRepository<Product>(db.Context, new Mock<IUser>().Object);

        var id = Guid.NewGuid();
        var fakeProductOriginal = _fixture
            .Build<Product>()
            .Without(x => x.Category)
            .With(x => x.IsDeleted, false)
            .CreateMany(5);

        db.Context.AddRange(fakeProductOriginal);
        db.Context.SaveChanges();

        var action = async () => await repo.DeleteByIdAsync(Guid.NewGuid());

        await action.Should().ThrowAsync<Exception>().WithMessage("Entity cannot be deleted. Not entity exists with the given id.");
    }

    [Fact(DisplayName = "HardDeleteByIdAsync method removes existing Product completely")]
    public async Task HardDeleteByIdAsync_HappyPath1()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new WriteEntityRepository<Product>(db.Context, new Mock<IUser>().Object);

        var id = Guid.NewGuid();
        var fakeProduct = _fixture
            .Build<Product>()
            .With(x => x.Id, id)
            .Without(x => x.Category)
            .With(x => x.IsDeleted, false)
            .Create();
        var fakeProductList = _fixture
            .Build<Product>()
            .Without(x => x.Category)
            .With(x => x.IsDeleted, false)
            .CreateMany(5);

        db.Context.Add(fakeProduct);
        db.Context.AddRange(fakeProductList);
        db.Context.SaveChanges();

        await repo.HardDeleteByIdAsync(id);

        var contextProduct = await db.Context.Set<Product>().SingleOrDefaultAsync(x => id == x.Id);

        contextProduct.Should().BeNull();
    }

    [Fact(DisplayName = "HardDeleteByIdAsync method throws exception when id is null")]
    public async Task HardDeleteByIdAsync_SadPath1()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new WriteEntityRepository<Category, string>(db.Context, new Mock<IUser>().Object);

        var action = async () => await repo.HardDeleteByIdAsync(null);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "HardDeleteByIdAsync method throws exception when id is default value")]
    public async Task HardDeleteByIdAsync_SadPath2()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new WriteEntityRepository<Product>(db.Context, new Mock<IUser>().Object);

        var action = async () => await repo.HardDeleteByIdAsync(Guid.Empty);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "HardDeleteByIdAsync method throws exception when no Product exists with the id")]
    public async Task HardDeleteByIdAsync_SadPath3()
    {
        var db = new InMemoryDBHelper<TestDbContext>();
        var repo = new WriteEntityRepository<Product>(db.Context, new Mock<IUser>().Object);

        var id = Guid.NewGuid();
        var fakeProductOriginal = _fixture
            .Build<Product>()
            .Without(x => x.Category)
            .With(x => x.IsDeleted, false)
            .CreateMany(5);

        db.Context.AddRange(fakeProductOriginal);
        db.Context.SaveChanges();

        var action = async () => await repo.HardDeleteByIdAsync(Guid.NewGuid());

        await action.Should().ThrowAsync<Exception>().WithMessage("Entity cannot be deleted. Not entity exists with the given id.");
    }

    private static Mock<IUser> CreateUserMock(string userName)
    {
        var userMock = new Mock<IUser>();
        userMock
            .Setup(um => um.UserName)
            .Returns(userName);

        return userMock;
    }
}
