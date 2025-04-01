using System.ComponentModel.DataAnnotations;

namespace Identity.DTO.Account
{
    public record LoginDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

    }
}