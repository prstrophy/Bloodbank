namespace BloodBank.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BloodGroup")]
    public partial class BloodGroup
    {
        [Key]
        public int Id { get; set; }

       
        [StringLength(50)]
        public string Description { get; set; }
    }
}
