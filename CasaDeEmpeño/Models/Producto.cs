using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CasaDeEmpeño.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre del producto debe contar con mínimo {2} caracteres y máximo {1}.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Estado")]
        public int EstadoId { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Fecha de Ingreso")]
        [DataType(DataType.Date)]
        public DateTime FechaIngreso { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Range(0.1, 9999999.99)]
        public decimal Valor { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Tipo")]
        public int TipoId { get; set; }

        public int Editable { get; set; } = 1;
        public string MensajeError { get; set; }

    }
}
