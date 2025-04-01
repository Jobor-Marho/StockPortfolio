using identity.DTO.Account;
using identity.interfaces;
using Identity.Models;


namespace identity.Service{
    public class AccountService
    {
    private readonly ITokenService _tokenService;

    public AccountService(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public NewUserDTO GenLoginToken(AppUser user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be null.");

        return new NewUserDTO(
            user.UserName,
            user.Email,
            _tokenService.CreateToken(user)
        );
    }
    }

}