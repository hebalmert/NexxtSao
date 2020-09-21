using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class Hour
    {
        [Key]
        public int HourId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(20, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Hora_Model_Horas")]
        [Index("Company_Hora_Index", IsUnique = true)]
        public string Hora { get; set; }

        [MaxLength(5, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Hora_Model_Orden")]
        [Index("Company_Oden_Index", IsUnique = true)]
        public string Orden { get; set; }
    }
}