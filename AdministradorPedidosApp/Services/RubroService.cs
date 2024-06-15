using AdministradorPedidosApp.Data;
using AdministradorPedidosApp.Interfaces;
using AdministradorPedidosApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AdministradorPedidosApp.Services
{
    public class RubroService : IRubroService
    {
        private readonly AdministradorPedidosAppContext _context;

        public RubroService(AdministradorPedidosAppContext context)
        {
            _context = context;
        }

        public async Task Create(RubroModel rubroModel, IFormFile imagen)
        {
            if (imagen != null)
            {
                //var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                var uploadsFolder = "C:\\Repositorio\\Proyecto MVC\\PedidosApp\\PedidosApp\\wwwroot\\images\\rubros";
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + imagen.FileName;
                var path = Path.Combine(uploadsFolder, uniqueFileName);

                if (!Directory.Exists(uploadsFolder))
                {
                    DirectoryInfo di = Directory.CreateDirectory(uploadsFolder);
                }

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }
                rubroModel.Url_Imagen = "/images/rubros/" + uniqueFileName;
            }

            _context.Add(rubroModel);
            await _context.SaveChangesAsync();
        }

        public async Task Edit(int id, RubroModel rubroModel, IFormFile imagen)
        {

            if (imagen != null)
            {
                var uploadsFolder = "C:\\Repositorio\\Proyecto MVC\\PedidosApp\\PedidosApp\\wwwroot\\images\\rubros"; // Carpeta en donde se guarda la imagen
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + imagen.FileName;
                var path = Path.Combine(uploadsFolder, uniqueFileName); // Se combina la ruta junto con el nombre del artículo

                if (!Directory.Exists(uploadsFolder))
                {
                    DirectoryInfo di = Directory.CreateDirectory(uploadsFolder); // Si la carpeta no existe la crea
                }

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream); // Se guarda la imagen en la ruta
                }
                rubroModel.Url_Imagen = "/images/rubros/" + uniqueFileName; // Se le asigna la dirección de la imagen en base de datos
            }
            else rubroModel.Url_Imagen = await _context.Articulos.AsNoTracking().Where(a => a.Id_Articulo == id).Select(a => a.Url_Imagen).FirstOrDefaultAsync();

            _context.Update(rubroModel);
            await _context.SaveChangesAsync();
        }
    }
}
