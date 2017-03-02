using System.ComponentModel.DataAnnotations;

namespace BloodBank.Models
{
    public class RegisterViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email", Prompt = "Email Address..")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "First Name", Prompt = "First Name..")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name", Prompt = "First Name..")]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Password must contain Numeric,alphabets(g:!^&) and  charactar e", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", Prompt = "Password..")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password", Prompt = "Confirm password..")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public string Number { get; set; }
        public string Gender { get; set; }
        public int DistrictId { get; set; }
        public int CityId { get; set; }
        public int BloodGroupId { get; set; }
        public string UserType { get; set; }
    }
}