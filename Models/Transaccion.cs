using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Transaccion
    {
        public int Id { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una cuenta")]
        [Display(Name = "Tipo Operacion")]
        public int IdTipoOperacion { get; set; }
        public int IdUsuario { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una cuenta")]
        public int IdCuenta { get; set; }
        [Range(1, maximum: int.MaxValue, ErrorMessage ="Debe seleccionar una categoria")]
        public int IdCategoria { get; set; }
        [StringLength(maximumLength:1000, ErrorMessage ="La nota no puede pasar de {1} caracteres")]
        public string Nota { get; set; }
        public decimal Monto { get; set; }
        [Display(Name ="Fecha Transaccion")]
        [DataType(DataType.Date)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Today;
        public DateTime Create_at { get; set; }
    }
}
