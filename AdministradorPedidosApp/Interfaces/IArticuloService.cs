using AdministradorPedidosApp.Models;
using AdministradorPedidosApp.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AdministradorPedidosApp.Interfaces
{
    public interface IArticuloService
    {
        Task<List<ArticuloDTO>> Index();
        Task<ArticuloDTO> Details(int? id);

        Task Create(ArticuloModel articuloModel, IFormFile imagen, List<int> categoriasSeleccionadas);
    }
}
