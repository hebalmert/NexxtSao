using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class DentistSpecialty
    {
        [Key]
        public int DentistSpecialtyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("DentistSpecialty_Company_Especialidad_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "DentistSpecialty_Model_Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Index("DentistSpecialty_Company_Especialidad_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "DentistSpecialty_Model_Especialidad")]
        public string Especialidad { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<Dentist> Dentists { get; set; }
    }
}