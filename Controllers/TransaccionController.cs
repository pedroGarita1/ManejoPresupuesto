using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using System.Transactions;

namespace ManejoPresupuesto.Controllers
{
    public class TransaccionController : Controller
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IMapper mapper;

        public TransaccionController(IRepositorioTransacciones repositorioTransacciones, IServicioUsuarios servicioUsuarios, IRepositorioCuentas repositorioCuentas, IRepositorioCategorias repositorioCategorias, IMapper mapper)
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioCategorias = repositorioCategorias;
            this.mapper = mapper;
        }
        public async Task<IActionResult> Index(int mes, int year)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            DateTime fechaInicio;
            DateTime fechaFin;
            if (mes <= 0 || mes > 12 || year <= 1900)
            {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            }
            else
            {
                fechaInicio = new DateTime(year, mes, 1);
            }
            fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                IdUsuario = idUsuario,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
            };
            var transacciones = await repositorioTransacciones.ObtenerPorIdUsuario(parametro);
            var modelo = new ReporteTransaccionesDetalladas();
            var transaccionesPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion).GroupBy(x => x.FechaTransaccion).Select(grupo => new ReporteTransaccionesDetalladas.TransaccionesPorFecha() { FechaTransaccion = grupo.Key, Transacciones = grupo.AsEnumerable() });
            modelo.TransaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = fechaInicio;
            modelo.FechaFin = fechaFin;

            ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;
            ViewBag.yearAnterior = fechaInicio.AddMonths(-1).Year;
            ViewBag.mesPosterior = fechaInicio.AddMonths(1).Month;
            ViewBag.yearPosterior = fechaInicio.AddMonths(1).Year;
            ViewBag.urlRetorno = HttpContext.Request.Path + HttpContext.Request.QueryString;
            return View(modelo);
        }
        public async Task<IActionResult> Crear()
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var modelo = new TransaccionCreacionViewModel();
            modelo.Cuenta = await ObtenerCuentas(idUsuario);
            modelo.Categorias = await ObtenerCategorias(idUsuario, modelo.IdTipoOperacion);
            return View(modelo);
        }
        [HttpPost]
        public async Task<IActionResult> Crear(TransaccionCreacionViewModel modelo)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            if (!ModelState.IsValid)
            {
                modelo.Cuenta = await ObtenerCuentas(idUsuario);
                modelo.Categorias = await ObtenerCategorias(idUsuario, modelo.IdTipoOperacion);
                return View(modelo);
            }
            var cuenta = await repositorioCuentas.ObtenerPorId(modelo.IdCuenta, idUsuario);
            if( cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var categoria = await repositorioCategorias.Buscar(modelo.IdCategoria, idUsuario);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            modelo.IdUsuario = idUsuario;
            if(modelo.IdTipoOperacion == TipoOperacion.Gasto)
            {
                modelo.Monto *= -1;
            }
            await repositorioTransacciones.Crear(modelo);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Editar(int id, string urlRetorno = null)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var transaccion = await repositorioTransacciones.ObtenerPorId(id, idUsuario);
            if (transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var modelo = mapper.Map<TransaccionesActualizacionViewModel>(transaccion);
            modelo.MontoAnterior = modelo.Monto;
            if (modelo.IdTipoOperacion == TipoOperacion.Gasto)
            {
                modelo.MontoAnterior = modelo.Monto * -1;
            }
            modelo.IdCuentaAnterior = transaccion.IdCuenta;
            modelo.Categorias = await ObtenerCategorias(idUsuario,transaccion.IdTipoOperacion);
            modelo.Cuenta = await ObtenerCuentas(idUsuario);
            modelo.UrlRetorno = urlRetorno;
            return View(modelo);
        }
        [HttpPost]
        public async Task<IActionResult> Editar(TransaccionesActualizacionViewModel modelo)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            if (!ModelState.IsValid)
            {
                modelo.Cuenta = await ObtenerCuentas(idUsuario);
                modelo.Categorias = await ObtenerCategorias(idUsuario, modelo.IdTipoOperacion);
                return View(modelo);
            }
            var cuenta = await repositorioCuentas.ObtenerPorId(modelo.IdCuenta, idUsuario);
            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var categoria = await repositorioCategorias.Buscar(modelo.IdCategoria, idUsuario);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var transaccion = mapper.Map<Transaccion>(modelo);
            if (modelo.IdTipoOperacion == TipoOperacion.Gasto)
            {
                transaccion.Monto *= -1;
            }
            await repositorioTransacciones.Actualizar(transaccion, modelo.MontoAnterior, modelo.IdCuentaAnterior);
            if (string.IsNullOrEmpty(modelo.UrlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(modelo.UrlRetorno);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Borrar(int id, string urlRetorno = null)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var transaccion = await repositorioTransacciones.ObtenerPorId(id, idUsuario);
            if (transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTransacciones.Borrar(id);

            if (string.IsNullOrEmpty(urlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(urlRetorno);
            }
            return RedirectToAction("Index");
        }
        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int idUsuario)
        {
            var cuentas = await repositorioCuentas.Buscar(idUsuario);
            return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int idUsuario, TipoOperacion tipoOperacion)
        {
            var categorias = await repositorioCategorias.Obtener(idUsuario, tipoOperacion);
            return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var categorias = await ObtenerCategorias(idUsuario, tipoOperacion);
            return Ok(categorias);
        }
    }
}
