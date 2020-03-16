using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Required")]
        [Index("Country_Pais_Index", IsUnique = true)]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Country_Model_Pais")]
        public string Pais { get; set; }

        public virtual ICollection<Company> Companies { get; set; }

    }
}