using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BloodBank.Models
{
    public class City
    {
        public int Id { get; set; }
        
        public string Description { get; set; }

        [ForeignKey("District")]
        public int DistrictId { get; set; }
        public virtual District District { get; set; }

    }
}


