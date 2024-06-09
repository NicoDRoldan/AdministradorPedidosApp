using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdministradorPedidosApp.Data;
using AdministradorPedidosApp.Models;
using AdministradorPedidosApp.Models.DTOs;
using AdministradorPedidosApp.Services;
using AdministradorPedidosApp.Interfaces;
using NuGet.Protocol.Core.Types;

namespace AdministradorPedidosApp.Controllers
{
    public class ArticuloController : Controller
    {
        private readonly AdministradorPedidosAppContext _context;
        private readonly IArticuloService _articuloService;

        public ArticuloController(AdministradorPedidosAppContext context, IArticuloService articuloService)
        {
            _context = context;
            _articuloService = articuloService;
        }

        // GET: Articulo
        public async Task<IActionResult> Index()
        {
            var articulosDTO = await _articuloService.Index();

            return View(articulosDTO);
        }

        // GET: Articulo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articuloModel = await _articuloService.Details(id);

            if (articuloModel == null)
            {
                return NotFound();
            }

            return View(articuloModel);
        }

        // GET: Articulo/Create
        public IActionResult Create()
        {
            var entityModel = new ArticuloModel();
            ViewBag.Categorias = _context.Categorias.ToList();
            ViewData["Id_Rubro"] = new SelectList(_context.Rubros, "Id_Rubro", "Nombre");
            return View(entityModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_Articulo,Nombre,Descripcion,Activo,FechaCreacion,Id_Rubro,Url_Imagen,Precio,Categorias")]
            ArticuloModel articuloModel, IFormFile imagen)
        {
            var categoriasSeleccionadas = Request.Form["CategoriasSeleccionadas"].Select(x => int.Parse(x)).ToList();

            await _articuloService.Create(articuloModel, imagen, categoriasSeleccionadas);

            return RedirectToAction(nameof(Index));
        }

        // GET: Articulo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articuloModel = await _context.Articulos.FindAsync(id);
            if (articuloModel == null)
            {
                return NotFound();
            }
            ViewData["Id_Rubro"] = new SelectList(_context.Rubros, "Id_Rubro", "Id_Rubro", articuloModel.Id_Rubro);
            return View(articuloModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_Articulo,Nombre,Descripcion,Activo,FechaCreacion,Id_Rubro,Url_Imagen")] ArticuloModel articuloModel)
        {
            if (id != articuloModel.Id_Articulo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(articuloModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticuloModelExists(articuloModel.Id_Articulo))
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
            ViewData["Id_Rubro"] = new SelectList(_context.Rubros, "Id_Rubro", "Id_Rubro", articuloModel.Id_Rubro);
            return View(articuloModel);
        }

        // GET: Articulo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articuloModel = await _context.Articulos
                .Include(a => a.Rubro)
                .FirstOrDefaultAsync(m => m.Id_Articulo == id);
            if (articuloModel == null)
            {
                return NotFound();
            }

            return View(articuloModel);
        }

        // POST: Articulo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var articuloModel = await _context.Articulos.FindAsync(id);
            var precioModel = await _context.Precios.FindAsync(id);
            var articuloCategoriaModel = await _context.Articulos_Categorias
                .Where(ac => ac.Id_Articulo == id)
                .ToListAsync();

            if(articuloModel != null)
            {
                if(articuloCategoriaModel != null)
                {
                    foreach (var artCat in articuloCategoriaModel)
                    {
                        _context.Articulos_Categorias.Remove(artCat);
                    }
                }
                if(precioModel != null)
                {
                    _context.Precios.Remove(precioModel);
                }
                _context.Articulos.Remove(articuloModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticuloModelExists(int id)
        {
            return _context.Articulos.Any(e => e.Id_Articulo == id);
        }
    }
}
