﻿using AdministradorPedidosApp.Data;
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
        private readonly IMemoryCache _memoryCache;

        public ArticuloService (AdministradorPedidosAppContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
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
            var articulosCategoriasExistentes = _context.Articulos_Categorias
                .Where(ac => ac.Id_Articulo == id)
                .ToList();

            List<int> idCategorias = _context.Categorias
                .Select(ac => ac.Id_Categoria)
                .ToList();

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
            
            if(_memoryCache.TryGetValue("FechaCreacionArt", out DateTime fechaCreacion)) articuloModel.FechaCreacion = fechaCreacion;

            if(categoriasSeleccionadas.Any())
            {
                foreach (var id_CategoriaSeleccionada in categoriasSeleccionadas)
                {
                    foreach (var id_Categoria in idCategorias)
                    {
                        var artCategoriaEntity = _context.Articulos_Categorias.Where(ac => ac.Id_Categoria == id_Categoria && ac.Id_Articulo == id).FirstOrDefault();

                        if (id_Categoria == id_CategoriaSeleccionada && artCategoriaEntity is null)
                        {
                            Articulos_CategoriasModel articulos_Categorias = new Articulos_CategoriasModel
                            {
                                Id_Articulo = id,
                                Id_Categoria = id_Categoria
                            };
                            _context.Add(articulos_Categorias);
                        }
                        else if(!categoriasSeleccionadas.Contains(id_Categoria) && artCategoriaEntity != null)
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

            if (articuloModel.Precio != null)
            {
                articuloModel.Precio.Id_Articulo = id;
            }

            _context.Update(articuloModel);
            await _context.SaveChangesAsync();
        }
    }
}