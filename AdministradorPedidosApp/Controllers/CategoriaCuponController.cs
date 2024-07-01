using AdministradorPedidosApp.Interfaces;
using AdministradorPedidosApp.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AdministradorPedidosApp.Controllers
{
    public class CategoriaCuponController : Controller
    {
        private readonly ICategoriaCuponService _categoriaCuponService;

        public CategoriaCuponController(ICategoriaCuponService categoriaCuponService)
        {
            _categoriaCuponService = categoriaCuponService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _categoriaCuponService.GetCategoriasCupones());
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View();
            }
        }

        public async Task<IActionResult> Create()
        {
            var categoriaCuponModel = new CategoriaDTO();
            return View(categoriaCuponModel);
        }

        [HttpPost]
        public async Task<IActionResult> AltaCategoriaCupon([FromForm] CategoriaDTO categoriaCupon)
        {
            try
            {
                var result = await _categoriaCuponService.AltaCategoriaCupon(categoriaCupon);
                return Json(new { success = true, message = result });
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
