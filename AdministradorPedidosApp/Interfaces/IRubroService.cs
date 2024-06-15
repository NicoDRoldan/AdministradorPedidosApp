using AdministradorPedidosApp.Models;

namespace AdministradorPedidosApp.Interfaces
{
    public interface IRubroService
    {
        Task Create(RubroModel rubroModel, IFormFile imagen);

        Task Edit(int id, RubroModel rubroModel, IFormFile imagen);
    }
}
