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

        // GET: Categoria
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categorias.ToListAsync());
        }

        // GET: Categoria/Details/5
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

        // GET: Categoria/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categoria/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_Categoria,Nombre")] CategoriasModel categoriasModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoriasModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoriasModel);
        }

        // GET: Categoria/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriasModel = await _context.Categorias.FindAsync(id);
            if (categoriasModel == null)
            {
                return NotFound();
            }
            return View(categoriasModel);
        }

        // POST: Categoria/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_Categoria,Nombre")] CategoriasModel categoriasModel)
        {
            if (id != categoriasModel.Id_Categoria)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            return View(categoriasModel);
        }

        // GET: Categoria/Delete/5
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

        // POST: Categoria/Delete/5
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
