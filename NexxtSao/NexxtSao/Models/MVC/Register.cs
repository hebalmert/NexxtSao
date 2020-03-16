using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class Register
    {
        [Key]
        public int RegisterId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "Register_Model_Estimate")]
        public int Estimate { get; set; }

        public virtual Company Company { get; set; }
    }
}