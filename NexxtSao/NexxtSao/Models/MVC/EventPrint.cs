using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class EventPrint
    {
        [Key]
        public int EventPrintId { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Event_Model_Professional")]
        public string Compania { get; set; }

        [MaxLength(25, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Event_Model_Professional")]
        public string Rif { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Logo")]
        public string Logo { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resource), Name = "Event_Model_DateStart")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "Event_Model_Professional")]
        public int DentistId { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Event_Model_Professional")]
        public string Odontologo { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Event_Model_Cliente")]
        public string Cliente { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Event_Model_Motivo")]
        public string Subject { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Resource), Name = "Event_Model_Description")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resource), Name = "Event_Model_DateStart")]
        public DateTime Start { get; set; }

        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "Event_Model_Hora")]
        public int HourId { get; set; }

        [MaxLength(10, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Event_Model_Motivo")]
        public string Hora { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Resource), Name = "Event_Model_Description")]
        public string TipoDocumento { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Resource), Name = "Event_Model_Description")]
        public string HeadText { get; set; }

    }
}