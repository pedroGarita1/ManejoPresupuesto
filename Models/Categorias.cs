using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Categorias
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        [Display(Name = "Tipo Operacion")]
        public TipoOperacion IdTipoOperacion { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength:50, ErrorMessage ="No puede ser mayor a {1} caracteres")]
        public string Nombre { get; set; }
        public DateTime Create_at { get; set; }
    }
}
