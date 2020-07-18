using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace NexxtSao.Models.MVC
{
    public class Estimate
    {
        [Key]
        public int EstimateId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("Estimate_ClientId_Company_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Estimate_Model_Company")]
        public int CompanyId { get; set; }
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "Estimate_Model_Estimado")]
        public int Estimado { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resource), Name = "Estimate_Model_Date")]
        public DateTime Date { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("Estimate_ClientId_Company_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Estimate_Model_Client")]
        public int ClientId { get; set; }
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)] //Currency es formato de Moneda del pais IP
        [Display(ResourceType = typeof(Resource), Name = "Estimate_Model_SubTotal")]
        public double SubTotal { get; set; }
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)] //Currency es formato de Moneda del pais IP
        [Display(ResourceType = typeof(Resource), Name = "Estimate_Model_Impuesto")]
        public double Iva { get; set; }
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)] //Currency es formato de Moneda del pais IP
        [Display(ResourceType = typeof(Resource), Name = "Estimate_Model_Total")]
        public double Total { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Debe Seleccionar un {0}")]
        [Display(ResourceType = typeof(Resource), Name = "Estimate_Model_Encabezado")]
        public int HeadTextId { get; set; }
        public virtual Company Company { get; set; }
        public virtual Client Client { get; set; }
        public virtual ICollection<EstimateDetail> EstimateDetails { get; set; }
        public virtual ICollection<Odontodiagrama> Odontodiagramas { get; set; }
        public virtual ICollection<EstimateDetailAdd> EstimateDetailAdds { get; set; }
        public virtual ICollection<EvolutionDetails> EvolutionDetails { get; set; }
        public virtual ICollection<ImgPanoramicEstimate> ImgPanoramicEstimates { get; set; }
        public virtual ICollection<ImgPeripicalEstimate> ImgPeripicalEstimates { get; set; }
        public virtual ICollection<ImgFotoEstimate> ImgFotoEstimates { get; set; }
    }
}