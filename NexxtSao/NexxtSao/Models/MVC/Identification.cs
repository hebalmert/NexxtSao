using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class Identification
    {
        [Key]
        public int IdentificationId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("Identification_TipoDocumento_Company_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Identification_Model_Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Index("Identification_TipoDocumento_Company_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Identification_Model_TipoDocumento")]
        public string TipoDocumento { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        public virtual ICollection<Dentist> Dentists { get; set; }
    }
}