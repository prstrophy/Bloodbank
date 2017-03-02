namespace BloodBank.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BloodRequest")]
    public partial class BloodRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PatientName { get; set; }

        [Required]
        [StringLength(50)]
        public string BloodGroup { get; set; }

        [Required]
        [StringLength(50)]
        public string Location { get; set; }

        [Column("Hospital Name")]
        [Required]
        [StringLength(50)]
        public string Hospital_Name { get; set; }

        [Column("Hospital Address")]
        [Required]
        [StringLength(50)]
        public string Hospital_Address { get; set; }

        [Required]
        [StringLength(50)]
        public string ContactName { get; set; }

        [StringLength(50)]
        public string EmailId { get; set; }

        [Required]
        [StringLength(200)]
        public string ContactNumber { get; set; }

        public DateTime RequiredDate { get; set; }
    }
}
