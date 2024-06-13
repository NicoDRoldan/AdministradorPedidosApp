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
    public class RubroController : Controller
    {
        private readonly AdministradorPedidosAppContext _context;

        public RubroController(AdministradorPedidosAppContext context)
        {
            _context = context;
        }

        // GET: Rubro
        public async Task<IActionResult> Index()
        {
            return View(await _context.Rubros.ToListAsync());
        }

        // GET: Rubro/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rubroModel = await _context.Rubros
                .FirstOrDefaultAsync(m => m.Id_Rubro == id);
            if (rubroModel == null)
            {
                return NotFound();
            }

            return View(rubroModel);
        }

        // GET: Rubro/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rubro/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_Rubro,Nombre")] RubroModel rubroModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rubroModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rubroModel);
        }

        // GET: Rubro/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rubroModel = await _context.Rubros.FindAsync(id);
            if (rubroModel == null)
            {
                return NotFound();
            }
            return View(rubroModel);
        }

        // POST: Rubro/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_Rubro,Nombre")] RubroModel rubroModel)
        {
            if (id != rubroModel.Id_Rubro)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rubroModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RubroModelExists(rubroModel.Id_Rubro))
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
            return View(rubroModel);
        }

        // GET: Rubro/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rubroModel = await _context.Rubros
                .FirstOrDefaultAsync(m => m.Id_Rubro == id);
            if (rubroModel == null)
            {
                return NotFound();
            }

            return View(rubroModel);
        }

        // POST: Rubro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rubroModel = await _context.Rubros.FindAsync(id);
            if (rubroModel != null)
            {
                _context.Rubros.Remove(rubroModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RubroModelExists(int id)
        {
            return _context.Rubros.Any(e => e.Id_Rubro == id);
        }
    }
}
