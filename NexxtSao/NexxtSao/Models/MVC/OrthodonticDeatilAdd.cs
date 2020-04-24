using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class OrthodonticDeatilAdd
    {
        [Key]
        public int OrthodonticDetailAddId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "EstimadeDetailAdd_Model_Estimate")]
        public int OrthodonticId { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "EstimadeDetailAdd_Model_Estimate")]
        public int OrthodonticDetailId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "EstimadeDetailAdd_Model_Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "EstimadeDetailAdd_Model_Categoria")]
        public int TreatmentCategoryId { get; set; }


        //Se incrusta el Dentista y la tasa de ganancia para mostrar al usuario si tiene o no porcentaje el Odontologo

        //[Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "DentistPercentage_Model_Dentist")]
        public int DentistId { get; set; }

        [Range(0, 100, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]  //Porcentaje entre 0 y 1, 12%= 0.12
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = false)] //Formato Porcentaje con 2 decimales
        [Display(ResourceType = typeof(Resource), Name = "DirectGeneral_Model_TasaProfesional")]
        public double TasaProfesional { get; set; }
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


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
        [Display(ResourceType = typeof(Resource), Name = "EstimateDetail_Model_Diente")]
        [MaxLength(10, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        public string Diente { get; set; }

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

        //public virtual Company Company { get; set; }

        //public virtual Orthodontic Orthodontic { get; set; }

        //public virtual TreatmentCategory TreatmentCategory { get; set; }

        //public virtual Treatment Treatment { get; set; }

        //public virtual LevelPrice LevelPrice { get; set; }
    }
}