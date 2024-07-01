using AdministradorPedidosApp.Controllers;
using AdministradorPedidosApp.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AdministradorPedidosApp.Interfaces
{
    public interface ICategoriaCuponService
    {
        Task<List<CategoriaDTO>> GetCategoriasCupones();

        Task<string> AltaCategoriaCupon(CategoriaDTO categoriaCupon);
    }
}
