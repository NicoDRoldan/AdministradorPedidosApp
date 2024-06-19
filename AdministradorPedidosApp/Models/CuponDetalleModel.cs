namespace AdministradorPedidosApp.Models
{
    public class CuponDetalleModel
    {
        public int Id_Cupon { get; set; }

        public int Id_ArticuloAsociado { get; set; }

        public int Cantidad { get; set; }

        public virtual ArticuloModel Articulo { get; set; }
    }
}
