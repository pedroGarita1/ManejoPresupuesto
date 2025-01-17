using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ClientModel.Primitives;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ManejoPresupuesto.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IMapper mapper;

        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios, IRepositorioCuentas repositorioCuentas, IMapper mapper) 
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var cuentasConTipo = await repositorioCuentas.Buscar(idUsuario);
            var modelo = cuentasConTipo.GroupBy(x => x.TipoCuenta).Select(grupo => new IndiceTipoCuentasViewModel {
                TipoCuenta = grupo.Key,
                Cuentas = grupo.AsEnumerable()
            }).ToList();
            return View(modelo);
        }

        public async Task<IActionResult> Crear() 
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var modelo = new CuentaCreacionViewModel();
            modelo.TiposCuentas = await ObtenerTiposCuentas(idUsuario);
            return View(modelo);
        }
        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(cuenta.IdTipoCuenta, idUsuario);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(idUsuario);
                return View(cuenta);
            }
            await repositorioCuentas.Crear(cuenta);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Editar(int id)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, idUsuario);
            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var modelo = mapper.Map<CuentaCreacionViewModel>(cuenta);
            modelo.TiposCuentas = await ObtenerTiposCuentas(idUsuario);
            return View(modelo);
        }
        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var cuenta = await repositorioCuentas.ObtenerPorId(cuentaEditar.Id, idUsuario);
            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(cuentaEditar.IdTipoCuenta, idUsuario);
            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioCuentas.Actualizar(cuentaEditar);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Borrar(int id)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, idUsuario);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(cuenta);
        }
        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, idUsuario);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioCuentas.Borrar(id);
            return RedirectToAction("Index");
        }
        public async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int idUsuario)
        {
            var tiposCuentas = await repositorioTiposCuentas.Obtener(idUsuario);
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}
