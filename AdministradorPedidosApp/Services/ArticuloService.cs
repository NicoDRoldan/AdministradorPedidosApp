using AdministradorPedidosApp.Data;
using AdministradorPedidosApp.Interfaces;
using AdministradorPedidosApp.Models;
using AdministradorPedidosApp.Models.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AdministradorPedidosApp.Services
{
    public class ArticuloService : IArticuloService
    {
        private readonly AdministradorPedidosAppContext _context;

        public ArticuloService (AdministradorPedidosAppContext context)
        {
            _context = context;
        }

        public async Task<List<ArticuloDTO>> Index()
        {
            var articulosDTO = _context.Articulos
                .Include(a => a.Rubro)
                .Include(a => a.Precio)
                .Include(a => a.Articulos_Categorias)
                    .ThenInclude(c => c.Categoria)
                .Select(a => new ArticuloDTO
                {
                    Id_Articulo = a.Id_Articulo,
                    Nombre = a.Nombre,
                    Descripcion = a.Descripcion,
                    Activo = a.Activo,
                    FechaCreacion = a.FechaCreacion,
                    Id_Rubro = a.Id_Rubro,
                    Url_Imagen = a.Url_Imagen,
                    Rubro = new RubroDTO
                    {
                        Id_Rubro = a.Rubro.Id_Rubro,
                        Nombre = a.Rubro.Nombre
                    },
                    Precio = a.Precio != null ? new PrecioDTO
                    {
                        Id_Articulo = a.Precio.Id_Articulo,
                        Precio = a.Precio.Precio
                    } : null,
                    Categorias = a.Articulos_Categorias.Select(ac => new CategoriaDTO
                    {
                        Id_Categoria = ac.Categoria.Id_Categoria,
                        Nombre = ac.Categoria.Nombre
                    }).ToList()
                }).ToList();

            return articulosDTO;
        }

        public async Task<ArticuloDTO> Details(int? id)
        {
            var articuloModel = await _context.Articulos
                .Include(a => a.Rubro)
                .Include(a => a.Precio)
                .Include(a => a.Articulos_Categorias)
                    .ThenInclude(c => c.Categoria)
                .Select(a => new ArticuloDTO
                {
                    Id_Articulo = a.Id_Articulo,
                    Nombre = a.Nombre,
                    Descripcion = a.Descripcion,
                    Activo = a.Activo,
                    FechaCreacion = a.FechaCreacion,
                    Id_Rubro = a.Id_Rubro,
                    Url_Imagen = a.Url_Imagen,
                    Rubro = new RubroDTO
                    {
                        Id_Rubro = a.Rubro.Id_Rubro,
                        Nombre = a.Rubro.Nombre
                    },
                    Precio = a.Precio != null ? new PrecioDTO
                    {
                        Id_Articulo = a.Precio.Id_Articulo,
                        Precio = a.Precio.Precio
                    } : null,
                    Categorias = a.Articulos_Categorias.Select(ac => new CategoriaDTO
                    {
                        Id_Categoria = ac.Categoria.Id_Categoria,
                        Nombre = ac.Categoria.Nombre
                    }).ToList()
                })
                .FirstOrDefaultAsync(m => m.Id_Articulo == id);

            return articuloModel;
        }

        public async Task Create(ArticuloModel articuloModel, IFormFile imagen, List<int> categoriasSeleccionadas)
        {
            if (imagen != null)
            {
                //var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                var uploadsFolder = "C:\\Repositorio\\Proyecto MVC\\PedidosApp\\PedidosApp\\wwwroot\\images";
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
                articuloModel.Url_Imagen = "/images/" + uniqueFileName;
            }
            articuloModel.FechaCreacion = DateTime.Now;

            _context.Add(articuloModel);
            await _context.SaveChangesAsync();

            if (categoriasSeleccionadas.Any())
            {
                foreach(var id_Categoria in categoriasSeleccionadas)
                {
                    Articulos_CategoriasModel articulos_Categorias = new Articulos_CategoriasModel
                    {
                        Id_Articulo = articuloModel.Id_Articulo,
                        Id_Categoria = id_Categoria
                    };
                    _context.Add(articulos_Categorias);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task Edit(int id, ArticuloModel articuloModel, IFormFile imagen, List<int> categoriasSeleccionadas)
        {
            // Obtener las categorias existentes
            var articulosCategoriasExistentes = await _context.Articulos_Categorias
                .Where(ac => ac.Id_Articulo == id)
                .ToListAsync();

            // Obtener el id de todas las categorias
            List<int> idCategorias = await _context.Categorias
                .Select(ac => ac.Id_Categoria)
                .ToListAsync();

            // Si la imagen es diferente a null
            if (imagen != null)
            {
                var uploadsFolder = "C:\\Repositorio\\Proyecto MVC\\PedidosApp\\PedidosApp\\wwwroot\\images"; // Carpeta en donde se guarda la imagen
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
                articuloModel.Url_Imagen = "/images/" + uniqueFileName; // Se le asigna la dirección de la imagen en base de datos
            }
            else articuloModel.Url_Imagen = await _context.Articulos.AsNoTracking().Where(a => a.Id_Articulo == id).Select(a => a.Url_Imagen).FirstOrDefaultAsync();

            articuloModel.FechaCreacion = await _context.Articulos.AsNoTracking().Where(a => a.Id_Articulo == id).Select(a => a.FechaCreacion).FirstOrDefaultAsync();

            if (categoriasSeleccionadas.Any()) // Si la lista de categorias seleccionadas tiene por lo menos un elemento
            {
                foreach (var id_CategoriaSeleccionada in categoriasSeleccionadas) // Se recorren las categorias seleccionadas
                {
                    foreach (var id_Categoria in idCategorias) // Se recorren los ids de todas las categorias
                    {
                        // Se busca si el registro Articulo_Categoria existe, buscando por Id_Categoria y Id_Articulo
                        var artCategoriaEntity = await _context.Articulos_Categorias.Where(ac => ac.Id_Categoria == id_Categoria && ac.Id_Articulo == id).FirstOrDefaultAsync();

                        // Si el Id de la categoria es igual a la categoria seleccionada, y el registro Articulo_Categoria no existe, lo inserta
                        if (id_Categoria == id_CategoriaSeleccionada && artCategoriaEntity is null)
                        {
                            Articulos_CategoriasModel articulos_Categorias = new Articulos_CategoriasModel
                            {
                                Id_Articulo = id,
                                Id_Categoria = id_Categoria
                            };
                            _context.Add(articulos_Categorias);
                        }

                        // Si el Id de la categoria no está dentro de las categorias selccionadas y el Articulo_Categoria registro existe, elimina el registro
                        else if (!categoriasSeleccionadas.Contains(id_Categoria) && artCategoriaEntity != null)
                        {
                            _context.Remove(artCategoriaEntity);
                        }
                    }
                }
            }
            else // Si la lista de categorias seleccionadas está vacía, se eliminan todos los registros correspondientes a ese artículo
            {
                _context.RemoveRange(articulosCategoriasExistentes);
            }

            var precioExt = await _context.Precios.AsNoTracking().FirstOrDefaultAsync(p => p.Id_Articulo == id);

            if (articuloModel.Precio != null && precioExt != null)
            {
                articuloModel.Precio.Id_Articulo = id;
                _context.Entry(articuloModel.Precio).State = EntityState.Modified;
            }
            else if (articuloModel.Precio != null && precioExt == null)
            {
                articuloModel.Precio.Id_Articulo = id;
                _context.Add(articuloModel.Precio);
            }
        }
    }
}