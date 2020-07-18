using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace NexxtSao.Models.MVC
{
    public class Odontodiagrama
    {
        [Key]
        public int OdontodiagramaId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha Ingreso")]
        public DateTime DateEntry { get; set; }

        [Display(Name = "Descripcion")]
        public string Description { get; set; }

        [Required(ErrorMessage = "El Campo {0} es Requerido")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe Seleccionar un {0}")]
        [Display(Name = "Paciente")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "El Campo {0} es Requerido")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe Seleccionar un {0}")]
        [Display(Name = "Paciente")]
        public int EstimateId { get; set; }

        public virtual Client Client { get; set; }

        public virtual Estimate Estimate { get; set; }

        public virtual ICollection<Diagram> Diagrams { get; set; }
    }
}