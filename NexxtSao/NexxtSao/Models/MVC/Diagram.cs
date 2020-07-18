using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace NexxtSao.Models.MVC
{
    public class Diagram
    {
        [Key]
        public int DiagramId { get; set; }
        [Display(Name = "Numero de diente")]
        public double ToothNumber { get; set; }
        [Display(Name = "Activo")]
        public bool Active { get; set; }
        [Display(Name = "Color")]
        public string Color { get; set; }

        [Required(ErrorMessage = "El Campo {0} es Requerido")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe Seleccionar un {0}")]
        [Display(Name = "Odontodiagrama")]
        public int OdontodiagramaId { get; set; }

        public virtual Odontodiagrama Odontodiagrama { get; set; }
    }
}