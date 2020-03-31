using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class DentistPercentage
    {
        [Key]
        public int DentistPercentageId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("DentistPercentage_DentistId_TreatmentCategoryId_CompanyId_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "DentistPercentage_Model_Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("DentistPercentage_DentistId_TreatmentCategoryId_CompanyId_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "DentistPercentage_Model_Dentist")]
        public int DentistId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("DentistPercentage_DentistId_TreatmentCategoryId_CompanyId_Index", 3, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "DentistPercentage_Model_Category")]
        public int TreatmentCategoryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(0, 1, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]  //Porcentaje entre 0 y 1, 12%= 0.12
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = false)] //Formato Porcentaje con 2 decimales
        [Display(ResourceType = typeof(Resource), Name = "DentistPercentage_Model_Rate")]
        public double Porcentaje { get; set; }

        public virtual Company Company { get; set; }

        public virtual Dentist Dentist { get; set; }

        public virtual TreatmentCategory TreatmentCategory { get; set; }
    }
}