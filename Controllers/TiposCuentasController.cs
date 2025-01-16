using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController : Controller
    {
        private IRepositorioTiposCuentas repositorioTiposCuentas;
        private IServicioUsuarios servicioUsuarios;

        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios ServicioUsuarios)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = ServicioUsuarios;
        }
        public async Task<IActionResult> Index()
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var TipoCuenta = await repositorioTiposCuentas.Obtener(idUsuario);
            return View(TipoCuenta);
        }
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }
            tipoCuenta.IdUsuario = servicioUsuarios.ObtenerIdUsuario();
            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.IdUsuario);
            if (yaExisteTipoCuenta)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe.");
                return View(tipoCuenta);
            }
            await repositorioTiposCuentas.Crear(tipoCuenta);
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Editar(int id)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, idUsuario);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, idUsuario);
            if (tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTiposCuentas.Actualizar(tipoCuenta);
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Borrar(int id)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, idUsuario);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }
        [HttpPost]
        public async Task<ActionResult> BorrarTipoCuenta(int id)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(id, idUsuario);
            if (tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTiposCuentas.Borrar(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var yaExiste = await repositorioTiposCuentas.Existe(nombre, idUsuario);
            if (yaExiste)
            {
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);
        }
        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var tiposCuentas = await repositorioTiposCuentas.Obtener(idUsuario);
            var idsTiposCuentas = tiposCuentas.Select(x => x.Id);

            var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTiposCuentas).ToList();

            if (idsTiposCuentasNoPertenecenAlUsuario.Count() > 0)
            {
                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select((valor, indice) => new TipoCuenta() { Id = valor, Orden = indice + 1}).AsEnumerable();

            await repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);

            return Ok();
        }
    }
}
