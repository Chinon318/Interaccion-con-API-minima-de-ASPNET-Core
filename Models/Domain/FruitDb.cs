using Microsoft.EntityFrameworkCore;

namespace MicrosoftAPI.Models.Domain;

public class FruitDb : DbContext
{
    public FruitDb(DbContextOptions<FruitDb> options): base(options){}

    public DbSet<Fruit> Fruits { get; set; }
}