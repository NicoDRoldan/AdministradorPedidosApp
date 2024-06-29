namespace AdministradorPedidosApp.Models.DTOs
{
    public class ArticuloCuponDTO
    {
        public int Id_Articulo { get; set; }

        public string Nombre { get; set; }

        public bool ArticuloSeleccionado { get; set; }

        public int Cantidad { get; set; }
    }
}
