using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdministradorPedidosApp.Models
{
    [Table("PromocionesDetalles")]
    public class PromocionDetalleModel
    {
        [Key]
        public int Id_Promocion { get; set; }

        [Key]
        public int Id_Articulo { get; set; }
    }
}
