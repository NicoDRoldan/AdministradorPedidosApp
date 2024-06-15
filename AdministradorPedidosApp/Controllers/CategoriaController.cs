using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdministradorPedidosApp.Data;
using AdministradorPedidosApp.Models;

namespace AdministradorPedidosApp.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly AdministradorPedidosAppContext _context;

        public CategoriaController(AdministradorPedidosAppContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Categorias.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriasModel = await _context.Categorias
                .FirstOrDefaultAsync(m => m.Id_Categoria == id);
            if (categoriasModel == null)
            {
                return NotFound();
            }

            return View(categoriasModel);
        }

        public IActionResult Create()
        {
            var entityModel = new CategoriasModel();
            ViewBag.Articulos = _context.Articulos.ToList();
            return View(entityModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_Categoria,Nombre")] CategoriasModel categoriasModel)
        {
            var articulosSeleccionadas = Request.Form["ArticulosSeleccionados"].Select(x => int.Parse(x)).ToList();

            try
            {
                _context.Add(categoriasModel);
                await _context.SaveChangesAsync();

                if (articulosSeleccionadas.Any())
                {
                    foreach (var id_Articulo in articulosSeleccionadas)
                    {
                        Articulos_CategoriasModel articulos_Categorias = new Articulos_CategoriasModel
                        {
                            Id_Articulo = id_Articulo,
                            Id_Categoria = categoriasModel.Id_Categoria
                        };
                        _context.Add(articulos_Categorias);
                    }
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                var entityModel = new CategoriasModel();
                ViewBag.Articulos = _context.Articulos.ToList();
                return View(categoriasModel);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var entityModel = new CategoriasModel();
            ViewBag.Articulos = _context.Articulos.ToList();

            if (id == null)
            {
                return NotFound();
            }

            var categoriasModel = await _context.Categorias
                .Include(c => c.Articulos_Categorias)
                    .ThenInclude(ca => ca.Articulo)
                    .Where(c => c.Id_Categoria == id)
                    .FirstOrDefaultAsync();

            categoriasModel.ArticulosSeleccionados = categoriasModel
                .Articulos_Categorias
                .Select(ac => ac.Articulo.Id_Articulo).ToList();

            if (categoriasModel == null)
            {
                return NotFound();
            }
            return View(categoriasModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_Categoria,Nombre")] CategoriasModel categoriasModel)
        {
            if (id != categoriasModel.Id_Categoria)
            {
                return NotFound();
            }
            try
            {
                var articulosSeleccionados = Request.Form["ArticulosSeleccionados"].Select(x => int.Parse(x)).ToList();

                var articulosCategoriasExistentes = await _context.Articulos_Categorias
                    .Where(ac => ac.Id_Categoria == id)
                    .ToListAsync();

                List<int> idArticulos = await _context.Articulos
                    .Select(a => a.Id_Articulo)
                    .ToListAsync();

                if(articulosSeleccionados.Any())
                {
                    foreach (var id_ArticuloSeleccionado in articulosSeleccionados)
                    {
                        foreach (var id_Articulo in idArticulos)
                        {
                            var artCategoriaEntity = await _context.Articulos_Categorias.Where(ac => ac.Id_Categoria == id && ac.Id_Articulo == id_Articulo).FirstOrDefaultAsync();

                            if(id_Articulo == id_ArticuloSeleccionado &&  artCategoriaEntity is null)
                            {
                                Articulos_CategoriasModel articulos_Categorias = new Articulos_CategoriasModel
                                {
                                    Id_Articulo = id_Articulo,
                                    Id_Categoria = id
                                };
                                _context.Add(articulos_Categorias);
                            }
                            else if(!articulosSeleccionados.Contains(id_Articulo) && artCategoriaEntity != null)
                            {
                                _context.Remove(artCategoriaEntity);
                            }
                        }
                    }
                }
                else
                {
                    _context.RemoveRange(articulosCategoriasExistentes);
                }

                _context.Update(categoriasModel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriasModelExists(categoriasModel.Id_Categoria))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriasModel = await _context.Categorias
                .FirstOrDefaultAsync(m => m.Id_Categoria == id);
            if (categoriasModel == null)
            {
                return NotFound();
            }

            return View(categoriasModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoriasModel = await _context.Categorias.FindAsync(id);
            if (categoriasModel != null)
            {
                _context.Categorias.Remove(categoriasModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriasModelExists(int id)
        {
            return _context.Categorias.Any(e => e.Id_Categoria == id);
        }
    }
}
