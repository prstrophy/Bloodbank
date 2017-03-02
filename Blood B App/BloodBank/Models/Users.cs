using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BloodBank.Models
{
    public class Users
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]

        [ForeignKey("District")]
        public int DistrictId { get; set; }

        [ForeignKey("City")]
        public int CityId { get; set; }

        [ForeignKey("BloodGroup")]
        public int BloodGroupId { get; set; }
        public string Address { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Gender { get; set; }
        public string EmailConfirmationCode { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        public string Number { get; set; }
        public string UserType { get; set; }

        public virtual BloodGroup BloodGroup { get; set; }
        public virtual District District { get; set; }
        public virtual City City { get; set; }

    }
}