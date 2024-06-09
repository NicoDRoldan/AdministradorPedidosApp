using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdministradorPedidosApp.Models
{
    [Table("Provincias")]
    public class ProvinciaModel
    {
        [Key]
        public int Id_Provincia { get; set; }

        public string Nombre { get; set; }
    }
}
