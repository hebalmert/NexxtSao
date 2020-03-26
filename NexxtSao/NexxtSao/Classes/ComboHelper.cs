using NexxtSao.Models;
using NexxtSao.Models.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NexxtSao.Classes
{
    public class ComboHelper : IDisposable
    {
        private static NexxtSaoContext db = new NexxtSaoContext();

        //Preparacion de Lista de Paises
        public static List<Country> GetCountries()
        {
            var paises = db.Countries.ToList();
            paises.Add(new Country
            {
                CountryId = 0,
                Pais = @Resources.Resource.ComboSelect,
            });
            return paises.OrderBy(o => o.Pais).ToList();
        }

        //Combos de Compañias
        public static List<Company> GetCompanies()
        {
            var companies = db.Companies.ToList();
            companies.Add(new Company
            {
                CompanyId = 0,
                Name = @Resources.Resource.ComboSelect,
            });
            return companies.OrderBy(d => d.Name).ToList();
        }

        //Combos de Ciudades
        public static List<City> GetCities(int companyid)
        {
            var cities = db.Cities.Where(c => c.CompanyId == companyid).ToList();
            cities.Add(new City
            {
                CityId = 0,
                Ciudad = @Resources.Resource.ComboSelect,
            });
            return cities.OrderBy(d => d.Ciudad).ToList();
        }

        //Combos de Ciudades
        public static List<Zone> GetZone(int companyid)
        {
            var zona = db.Zones.Where(c => c.CompanyId == companyid).ToList();
            zona.Add(new Zone
            {
                ZoneId = 0,
                Zona = @Resources.Resource.ComboSelect,
            });
            return zona.OrderBy(d => d.Zona).ToList();
        }

        //Combos de Tipo de Identificacion
        public static List<Identification> GetIdentification(int companyid)
        {
            var identificacion = db.Identifications.Where(c => c.CompanyId == companyid).ToList();
            identificacion.Add(new Identification
            {
                IdentificationId = 0,
                TipoDocumento = @Resources.Resource.ComboSelect,
            });
            return identificacion.OrderBy(d => d.TipoDocumento).ToList();
        }

        //Combos de Tipo de Identificacion
        public static List<Tax> GetTax(int companyid)
        {
            var impuesto = db.Taxes.Where(c => c.CompanyId == companyid).ToList();
            impuesto.Add(new Tax
            {
                TaxId = 0,
                Impuesto = @Resources.Resource.ComboSelect,
            });
            return impuesto.OrderBy(d => d.Impuesto).ToList();
        }

        //Combos de Tipo de tratamientos
        public static List<TreatmentCategory> GetTreatmentCategory(int companyid)
        {
            var categoriatratamiento = db.TreatmentCategories.Where(c => c.CompanyId == companyid).ToList();
            categoriatratamiento.Add(new TreatmentCategory
            {
                TreatmentCategoryId = 0,
                CategoriaTratamiento = @Resources.Resource.ComboSelect,
            });
            return categoriatratamiento.OrderBy(d => d.CategoriaTratamiento).ToList();
        }

        //Combos de Tipo de tratamientos
        public static List<Treatment> GetTreatmen(int companyid)
        {
            var tratamiento = db.Treatments.Where(c => c.CompanyId == companyid).ToList();
            tratamiento.Add(new Treatment
            {
                TreatmentId = 0,
                Servicio = @Resources.Resource.ComboSelect,
            });
            return tratamiento.OrderBy(d => d.Servicio).ToList();
        }

        //Combos de Tipo de Especialidades
        public static List<DentistSpecialty> GetDentistSpecialty(int companyid)
        {
            var especial = db.DentistSpecialties.Where(c => c.CompanyId == companyid).ToList();
            especial.Add(new DentistSpecialty
            {
                DentistSpecialtyId = 0,
                Especialidad = @Resources.Resource.ComboSelect,
            });
            return especial.OrderBy(d => d.Especialidad).ToList();
        }

        //Preparacion de Lista de Level Price
        public static List<LevelPrice> GetPrice()
        {
            var nivelprecio = db.LevelPrices.ToList();
            nivelprecio.Add(new LevelPrice
            {
                LevelPriceId = 0,
                NivelPrecio = @Resources.Resource.ComboSelect,
            });
            return nivelprecio.OrderBy(o => o.NivelPrecio).ToList();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}