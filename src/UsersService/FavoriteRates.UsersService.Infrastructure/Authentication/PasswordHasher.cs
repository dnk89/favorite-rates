using FavoriteRates.UsersService.Application.Abstractions;
using Isopoh.Cryptography.Argon2;

namespace FavoriteRates.UsersService.Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
    public const int MaxHashStringLength = 255;
    
    public string Hash(string password) => Argon2.Hash(password);
    public bool Verify(string password, string hashedPassword) => Argon2.Verify(hashedPassword, password);
}