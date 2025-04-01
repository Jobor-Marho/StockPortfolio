using System.Security.Claims;

namespace Identity.Extensions
{
    public static class ClaimExtensions
    {
        private const string GivenName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.Claims.SingleOrDefault(x => x.Type.Equals(GivenName))?.Value;
        }
    }
}