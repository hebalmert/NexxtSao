namespace NexxtSao.Models
{
    using Models.MVC;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;
    using System.Web;

    public class NexxtSaoContext : DbContext
    {
        public NexxtSaoContext() : base("DefaultConnection")
        {
        }

        //Estas lineas evitan el borrado en Cascada de un registro con Relacion de Tablas
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public DbSet<LevelPrice> LevelPrices { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Zone> Zones { get; set; }

        public DbSet<Identification> Identifications { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Tax> Taxes { get; set; }

        public DbSet<HeadText> HeadTexts { get; set; }

        public DbSet<Register> Registers { get; set; }

        public DbSet<TreatmentCategory> TreatmentCategories { get; set; }

        public DbSet<Treatment> Treatments { get; set; }

        public DbSet<DentistSpecialty> DentistSpecialties { get; set; }

        public DbSet<Dentist> Dentists { get; set; }

        public DbSet<DentistPercentage> DentistPercentages { get; set; }
    }
}