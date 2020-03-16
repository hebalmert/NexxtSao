using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class Zone
    {
        [Key]
        public int ZoneId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("Zone_Ciudad_Zona_Company_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Compania")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("Zone_Ciudad_Zona_Company_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "City_Model_City")]
        public int CityId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Index("Zone_Ciudad_Zona_Company_Index", 3, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Zone_Model_Zone")]
        public string Zona { get; set; }

        public virtual City City { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        public virtual ICollection<Dentist> Dentists { get; set; }

    }
}