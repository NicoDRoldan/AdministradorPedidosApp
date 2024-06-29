using AdministradorPedidosApp.Models;
using AdministradorPedidosApp.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AdministradorPedidosApp.Interfaces
{
    public interface ICuponService
    {
        // GET
        Task<List<CuponModel>> Index();

        // GET
        Task<List<CategoriaDTO>> TraerCategorias();

        // GET
        Task<CuponModel> ObtenerCuponPorId(int id);

        // GET
        Task<List<ArticuloCuponDTO>> TraerArticulosSeleccionados(CuponModel cuponModel);

        Task<string> AltaOEditCupon([FromForm] CuponModel cupon, string endPoint, [FromForm] string? detalle = null,
            [FromForm] string? categoriasSeleccionadas = null, [FromForm] IFormFile? imagen = null);

        Task SubirImagenCupon(int id_Cupon, [FromForm] IFormFile? imagen = null);
    }
}
