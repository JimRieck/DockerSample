using Microsoft.EntityFrameworkCore;
using RevverDigital.Common.Data.Models.Interfaces;

namespace RevverDigital.Common.Data.Tests;

public partial class TestDbContext : DbContext
{
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Product> Products { get; set; }

    public TestDbContext(DbContextOptions options) : base(options)
    {
    }
}

public class Category : ICommonEntity<string>
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<Product> Products { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string UpdatedBy { get; set; }
}

public class Product : ICommonEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string CategoryId { get; set; }
    public bool IsDeleted { get; set; }
    public Category Category { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string UpdatedBy { get; set; }
}
