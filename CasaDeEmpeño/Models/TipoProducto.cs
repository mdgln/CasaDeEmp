using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CasaDeEmpeño.Models
{
    public class TipoProducto
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "La descripción del tipo producto debe contar con mínimo {2} caracteres y máximo {1}.")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public bool Estatus { get; set; }
        public string MensajeError { get; set; }
    }
}
