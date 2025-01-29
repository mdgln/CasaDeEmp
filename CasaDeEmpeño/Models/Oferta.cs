using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CasaDeEmpeño.Models
{
    public class Oferta
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "El nombre de la persona debe tener mínimo {2} caracteres y máximo {1}.")]
        [Display(Name = "Nombre de Persona")]
        public string NombrePersona { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "El número celular debe tener debe estar formado por {1} dígitos.")]
        [Display(Name = "Número Celular")]
        public string NumeroCelular { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Range(0.1, 9999999)]
        public decimal Monto { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Producto")]
        public int ProductoId { get; set; }

        public string NombreProducto { get; set; }
        public decimal PrecioProducto { get; set; }
    }
}
