using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
    public class TransaccionController : Controller
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;

        public TransaccionController(IRepositorioTransacciones repositorioTransacciones, IServicioUsuarios servicioUsuarios, IRepositorioCuentas repositorioCuentas)
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
        }
        public async Task<IActionResult> Crear()
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var modelo = new TransaccionCreacionViewModel();
            modelo.Cuentas = await ObtenerCuentas(idUsuario);
            return View(modelo);
        }
        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int idUsuario)
        {
            var cuentas = await repositorioCuentas.Buscar(idUsuario);
            return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}
