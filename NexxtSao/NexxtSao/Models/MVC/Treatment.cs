using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class Treatment
    {
        [Key]
        public int TreatmentId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("Service_CompanyId_Servicio_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Treatment_Model_Compania")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "Treatment_Model_Categoria")]
        public int TreatmentCategoryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Index("Service_CompanyId_Servicio_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Treatment_Model_Tratamiento")]
        public string Servicio { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "Treatment_Model_Impuesto")]
        public int TaxId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]  //Currency es formato de Moneda del pais IP
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)] //Formato Numero simbolo del Pais con 2 decimales
        [Display(ResourceType = typeof(Resource), Name = "Treatment_Model_Precio1")]
        public decimal Precio1 { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]  //Currency es formato de Moneda del pais IP
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)] //Formato Numero simbolo del Pais con 2 decimales
        [Display(ResourceType = typeof(Resource), Name = "Treatment_Model_Precio2")]
        public decimal Precio2 { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]  //Currency es formato de Moneda del pais IP
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)] //Formato Numero simbolo del Pais con 2 decimales
        [Display(ResourceType = typeof(Resource), Name = "Treatment_Model_Precio3")]
        public decimal Precio3 { get; set; }

        [MaxLength(250, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Resource), Name = "Treatment_Model_Detalle")]
        public string Detalle { get; set; }

        public virtual Company Company { get; set; }

        public virtual TreatmentCategory TreatmentCategory { get; set; }

        public virtual Tax Tax { get; set; }

        public virtual ICollection<EstimateDetailAdd> EstimateDetailAdds { get; set; }

        public virtual ICollection<EstimateDetail> EstimateDetails { get; set; }


    }
}