using identity.DTO.Account;
using identity.interfaces;
using Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace identity.Extensions
{
    public static class SignInExtensions
    {

        public static NewUserDTO GenLoginToken(this SignInManager<AppUser> signInManager, AppUser user, ITokenService tokenService)
        {
            if (signInManager == null)
                throw new ArgumentNullException(nameof(signInManager), "SignInManager cannot be null.");

            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            return new NewUserDTO(
                user.UserName,
                user.Email,
                tokenService.CreateToken(user)
            );
        }

    }
}
