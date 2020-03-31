using NexxtSao.Models.MVC;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NexxtSao.Models
{
    public class LevelPrice
    {
        [Key]
        public int LevelPriceId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(20, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "LevelPrice_Modal_NivelPrecio")]
        public string NivelPrecio { get; set; }

        public virtual ICollection<EstimateDetailAdd> EstimateDetailAdds { get; set; }

        public virtual ICollection<EstimateDetail> EstimateDetails { get; set; }

        public virtual ICollection<DirectPayment> DirectPayments { get; set; }
    }
}