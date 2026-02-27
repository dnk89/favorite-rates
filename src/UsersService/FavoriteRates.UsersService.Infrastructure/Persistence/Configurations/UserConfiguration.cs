using FavoriteRates.UsersService.Domain.Entities;
using FavoriteRates.UsersService.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FavoriteRates.UsersService.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name)
            .HasMaxLength(User.MaxNameLength)
            .IsRequired();
        builder.HasIndex(x => x.Name)
            .IsUnique();
        builder.Property(x => x.PasswordHash)
            .HasMaxLength(PasswordHasher.MaxHashStringLength)
            .IsRequired();
    }
}