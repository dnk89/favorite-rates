using FavoriteRates.FinanceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FavoriteRates.FinanceService.Infrastructure.Persistence.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("currencies");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(Currency.IdLength);
        builder.Property(x => x.Name).HasMaxLength(Currency.NameLength);
    }
}