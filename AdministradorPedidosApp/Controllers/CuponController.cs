using AdministradorPedidosApp.Data;
using AdministradorPedidosApp.Interfaces;
using AdministradorPedidosApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace AdministradorPedidosApp.Controllers
{
    public class CuponController : Controller
    {
        private readonly ICuponService _cuponService;
        private readonly AdministradorPedidosAppContext _context;

        public CuponController(ICuponService cuponService, AdministradorPedidosAppContext context)
        {
            _cuponService = cuponService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var cuponModel = new List<CuponModel>();
                cuponModel = await _cuponService.Index();
                return View(cuponModel);
            }
            catch(Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View();
            }
        }

        public async Task<IActionResult> Create()
        {
            var cuponModel = new CuponModel();

            ViewBag.Articulos = await _context.Articulos
                .Include(a => a.Precio)
                .Where(a => a.Activo == true
                    && a.Precio.Precio != 0)
                .ToListAsync();

            return View(cuponModel);
        }

        [HttpPost]
        public async Task<IActionResult> AltaCupon([FromForm] CuponModel cupon, [FromForm] string? detalle = null, IFormFile? imagen = null)
        {
            try
            {
                var result = await _cuponService.AltaCupon(cupon, detalle, imagen);
                return Json(new { success = true, message = result });
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = ex.Message});
            }
        }
    }
}
