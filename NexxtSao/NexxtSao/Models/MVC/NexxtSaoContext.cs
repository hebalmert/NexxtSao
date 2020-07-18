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

        public DbSet<ClientHistory> ClientHistories { get; set; }

        public DbSet<Estimate> Estimates { get; set; }

        public DbSet<EstimateDetail> EstimateDetails { get; set; }

        public DbSet<DirectPayment> DirectPayments { get; set; }

        public DbSet<PaymentsGeneral> PaymentsGenerals { get; set; }

        public DbSet<PayDentist> PayDentists { get; set; }

        public DbSet<PayDentistDetail> PayDentistDetails { get; set; }

        public DbSet<EvolutionDetails> EvolutionDetails { get; set; }

        public DbSet<Orthodontic> Orthodontics { get; set; }

        public DbSet<OrthodonticDetail> OrthodonticDetails { get; set; }

        public System.Data.Entity.DbSet<NexxtSao.Models.MVC.ImgPanoramic> ImgPanoramics { get; set; }

        public System.Data.Entity.DbSet<NexxtSao.Models.MVC.ImgOrthodon> ImgOrthodons { get; set; }

        public System.Data.Entity.DbSet<NexxtSao.Models.MVC.ImgPanoramicEstimate> ImgPanoramicEstimates { get; set; }

        public System.Data.Entity.DbSet<NexxtSao.Models.MVC.ImgPeripicalEstimate> ImgPeripicalEstimates { get; set; }

        public System.Data.Entity.DbSet<NexxtSao.Models.MVC.ImgFotoEstimate> ImgFotoEstimates { get; set; }

        public System.Data.Entity.DbSet<NexxtSao.Models.MVC.Event> Events { get; set; }

        public System.Data.Entity.DbSet<NexxtSao.Models.MVC.Color> Colors { get; set; }

        public System.Data.Entity.DbSet<NexxtSao.Models.MVC.Hour> Hours { get; set; }

        public DbSet<Odontodiagrama> Odontodiagramas { get; set; }

        public DbSet<Diagram> Diagrams { get; set; }
    }
}