using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class Color
    {
        [Key]
        public int ColorId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [MaxLength(20, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Color_Model_ColorDate")]
        [Index("Company_Color_Index", IsUnique = true)]
        public string ColorDate { get; set; }

        public virtual ICollection<Event> Events { get; set; }

    }
}