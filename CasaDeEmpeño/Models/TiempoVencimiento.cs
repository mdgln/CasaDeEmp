using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CasaDeEmpeño.Models
{
    public class TiempoVencimiento
    {
        [Display(Name = "Tiempo Vencimiento")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Tiempo { get; set; }
    }
}
