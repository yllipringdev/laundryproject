using System.ComponentModel.DataAnnotations;

namespace Laundry.Models.DTO
{
        public class RegisterDTO
        {
            [Required]
            public string Email { get; set; }
            [Required]
            public string Password { get; set; }
            [Required]
            public string ConfirmPassword { get; set; }
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            public string Username { get; set; }
            [Required]
             public string PhoneNumber { get; set; }
            [Required]
            public string City { get; set; }
            [Required]
            public string Country { get; set; }
            [Required]
            public string Address { get; set; }
        }
    }
