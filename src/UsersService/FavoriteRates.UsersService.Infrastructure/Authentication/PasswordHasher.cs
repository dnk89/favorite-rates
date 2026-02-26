using FavoriteRates.UsersService.Application.Abstractions;
using Isopoh.Cryptography.Argon2;

namespace FavoriteRates.UsersService.Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => Argon2.Hash(password);
}