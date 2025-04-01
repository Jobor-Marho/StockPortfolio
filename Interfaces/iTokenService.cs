using Identity.Models;
namespace identity.interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}