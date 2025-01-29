using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CasaDeEmpeño.Models
{
    public class Devolucion
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Producto")]
        public int ProductoId { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "La descripción del tipo producto debe contar con mínimo {2} caracteres y máximo {1}.")]
        public string Comentario { get; set; }

        public string NombreProducto { get; set; }
        public string MensajeError { get; set; }
    }
}