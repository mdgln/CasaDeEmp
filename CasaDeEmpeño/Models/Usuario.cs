using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CasaDeEmpeño.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(25, MinimumLength = 4, ErrorMessage = "El nombre de usuario debe tener mínimo {2} caracteres y máximo {1}.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(25, MinimumLength = 8, ErrorMessage = "La contraseña debe tener mínimo {2} caracteres y máximo {1}.")]
        public string Password { get; set; }
        public string MensajeError { get; set; }
    }
}