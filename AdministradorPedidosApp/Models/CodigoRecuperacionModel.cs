using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdministradorPedidosApp.Models
{
    [Table("CodigosRecuperacion")]
    public class CodigoRecuperacionModel
    {
        [Key]
        public string Usuario { get; set; }
        public string CodRecuperacion { get; set; }
        public DateTime FechaExpiracion { get; set; }
    }
}
