using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class City
    {
        [Key]
        public int CityId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("City_Company_Ciudad_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Compania")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Index("City_Company_Ciudad_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "City_Model_City")]
        public string Ciudad { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<Zone> Zones { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        public virtual ICollection<Dentist> Dentists { get; set; }
    }
}