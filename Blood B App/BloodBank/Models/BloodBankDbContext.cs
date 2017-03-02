namespace BloodBank.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class BloodBankDbContext : DbContext
    {
        public BloodBankDbContext()
            : base("name=BloodBankDbContext")
        {
        }

        public virtual DbSet<BloodGroup> BloodGroup { get; set; }
        public virtual DbSet<BloodRequest> BloodRequest { get; set; }
        public virtual DbSet<Users> users { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {


        }

        public System.Data.Entity.DbSet<BloodBank.Models.RegisterViewModel> RegisterViewModels { get; set; }
    }
}