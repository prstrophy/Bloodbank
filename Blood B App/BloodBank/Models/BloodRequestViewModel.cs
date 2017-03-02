using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BloodBank.Models
{
    public class BloodRequestViewModel
    {
        public List<BloodRequest> ListBloodRequest { get; set; }
        public BloodRequest BloodRequest { get; set; }
        public Users Users { get; set; }
        public List<Users> ListUsers { get; set; }
    }
}