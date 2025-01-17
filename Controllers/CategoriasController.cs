using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IServicioUsuarios servicioUsuarios;

        public CategoriasController(IRepositorioCategorias repositorioCategorias, IServicioUsuarios servicioUsuarios)
        {
            this.repositorioCategorias = repositorioCategorias;
            this.servicioUsuarios = servicioUsuarios;
        }
        public async Task<IActionResult> Index()
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var categorias = await repositorioCategorias.Obtener(idUsuario);
            return View(categorias);
        }
        public IActionResult Crear() { 
            return View(); 
        }
        [HttpPost]
        public async Task<IActionResult> Crear(Categorias categoria)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }
            categoria.IdUsuario = idUsuario;
            await repositorioCategorias.Crear(categoria);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Editar(int id)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var categorias = await repositorioCategorias.Buscar(id, idUsuario);
            if(categorias is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(categorias);
        }
        public async Task<IActionResult> Actualizar(Categorias categoria)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var categorias = await repositorioCategorias.Buscar(categoria.Id, idUsuario);
            if (categorias is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioCategorias.Actualizar(categoria);
            return RedirectToAction("Index");
        }
        public async Task <IActionResult> Borrar(int id)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var categorias = await repositorioCategorias.Buscar(id, idUsuario);
            if (categorias is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(categorias);
        }
        public async Task <IActionResult> BorrarCategoria (int id)
        {
            var idUsuario = servicioUsuarios.ObtenerIdUsuario();
            var categorias = await repositorioCategorias.Buscar(id, idUsuario);
            if (categorias is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioCategorias.BorrarCat(id);
            return RedirectToAction("Index");
        }
    }
}
