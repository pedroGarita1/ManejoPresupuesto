namespace ManejoPresupuesto.Models
{
    public class TransaccionesActualizacionViewModel: TransaccionCreacionViewModel
    {
        public int IdCuentaAnterior { get; set; }
        public decimal MontoAnterior { get; set; }
        public string UrlRetorno { get; set; }
    }
}
