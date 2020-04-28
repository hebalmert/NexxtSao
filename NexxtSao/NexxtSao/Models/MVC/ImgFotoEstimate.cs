using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class ImgFotoEstimate
    {
        [Key]
        public int ImgFotoEstimateId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        //[Index("Estimate_ClientId_Company_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Estimate_Model_Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "EstimateDetail_Model_EstemateId")]
        public int EstimateId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        //[Index("Estimate_ClientId_Company_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Estimate_Model_Client")]
        public int ClientId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resource), Name = "Estimate_Model_Date")]
        public DateTime Date { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(ResourceType = typeof(Resource), Name = "User_Model_Photo")]
        public string Photo { get; set; }

        [MaxLength(512, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Resource), Name = "PayDentist_Model_Detalle")]
        public string Detalle { get; set; }


        [NotMapped]
        public HttpPostedFileBase PhotoFile { get; set; }


        public virtual Company Company { get; set; }

        public virtual Estimate Estimate { get; set; }

        public virtual Client Client { get; set; }
    }
}