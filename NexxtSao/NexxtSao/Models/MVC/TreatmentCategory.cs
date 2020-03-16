using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class TreatmentCategory
    {
        [Key]
        public int TreatmentCategoryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("TreatmentCategory_Company_CategoriaTratamiento_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "CategoryTreatments_Model_Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Index("TreatmentCategory_Company_CategoriaTratamiento_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "CategoryTreatments_Model_Category")]
        public string CategoriaTratamiento { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<Treatment> Treatments { get; set; }

        public virtual ICollection<DentistPercentage> DentistPercentages { get; set; }
    }
}