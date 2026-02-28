using FavoriteRates.UsersService.Application.Abstractions;
using FavoriteRates.UsersService.Infrastructure.Authentication;

namespace FavoriteRates.UsersService.UnitTests.Infrastructure;

public class PasswordHasherTests
{
    private readonly IPasswordHasher _sut = new PasswordHasher();
    
    [Fact]
    public void Hash_Returns_Valid_Hash()
    {
        const string password = "12345678";
        var hash = _sut.Hash(password);
        
        var valid = _sut.Verify(password, hash);
        
        Assert.True(valid);
    }
}