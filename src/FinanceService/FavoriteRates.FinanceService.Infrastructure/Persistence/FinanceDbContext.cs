using FavoriteRates.FinanceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FavoriteRates.FinanceService.Infrastructure.Persistence;

public class FinanceDbContext(string connectionString, ILoggerFactory loggerFactory) : DbContext
{
    private const string Schema = "finance";
    
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<UserFavorite> UserFavorites => Set<UserFavorite>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(connectionString, o => o.MigrationsHistoryTable("__EFMigrationsHistory", Schema))
            .UseLoggerFactory(loggerFactory);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceDbContext).Assembly);
    }
}