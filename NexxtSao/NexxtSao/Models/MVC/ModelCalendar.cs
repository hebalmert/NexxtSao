using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NexxtSao.Models.MVC
{
    public class ModelCalendar
    {
        [Key]
        public int ModelCalendarId { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Debe Seleccionar un {0}")]
        [Display(Name = "Compania")]
        public int CompanyId { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Debe Seleccionar un {0}")]
        [Display(Name = "Profesional")]
        public int DentistId { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Debe Seleccionar un {0}")]
        [Display(Name = "Cliente")]
        public int ClientId { get; set; }

        [Display(Name = "Cliente")]
        public string Cliente { get; set; }

        [Display(Name = "Profesional")]
        public string Odontologo { get; set; }
    }
}