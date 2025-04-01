using identity.Models.JoinedTable;
using Microsoft.AspNetCore.Identity;

namespace Identity.Models
{
    public class AppUser : IdentityUser
    {
        public List<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
    }
}





