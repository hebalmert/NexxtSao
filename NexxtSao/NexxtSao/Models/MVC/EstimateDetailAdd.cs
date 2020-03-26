using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class EstimateDetailAdd
    {
        [Key]
        public int EstimateDetailAddId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "EstimadeDetailAdd_Model_Estimate")]
        public int EstimateId { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "EstimadeDetailAdd_Model_Estimate")]
        public int EstimateDetailId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "EstimadeDetailAdd_Model_Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "EstimadeDetailAdd_Model_Categoria")]
        public int TreatmentCategoryId { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "EstimateDetail_Model_Tratamiento")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        public string Categoria { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "EstimadeDetailAdd_Model_Tratamiento")]
        public int TreatmentId { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "EstimateDetail_Model_Tratamiento")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        public string tratamiento { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "EstimadeDetailAdd_Model_LevelPrice")]
        public int LevelPriceId { get; set; }

        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        [Display(ResourceType = typeof(Resource), Name = "EstimadeDetailAdd_Model_Unitario")]
        public double Unitario { get; set; }

        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "EstimadeDetailAdd_Model_Cantidad")]
        public int Cantidad { get; set; }

        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)] 
        [Display(ResourceType = typeof(Resource), Name = "EstimadeDetailAdd_Model_Total")]
        public double Total { get; set; }

        [Range(0, 1, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = false)] 
        [Display(ResourceType = typeof(Resource), Name = "EstimadeDetailAdd_Model_Tasa")]
        public double Tasa { get; set; }

        public virtual Company Company { get; set; }

        public virtual Estimate Estimate { get; set; }

        public virtual TreatmentCategory TreatmentCategory { get; set; }

        public virtual Treatment Treatment { get; set; }

        public virtual LevelPrice LevelPrice { get; set; }
    }
}