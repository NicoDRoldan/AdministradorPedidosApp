using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdministradorPedidosApp.Models
{
    [Table("Categorias")]
    public class CategoriasModel
    {
        [Key]
        public int Id_Categoria { get; set; }

        public string Nombre { get; set; }

        public virtual ICollection<Articulos_CategoriasModel> Articulos_Categorias { get; set; }

        [NotMapped]
        public List<int> ArticulosSeleccionados { get; set; }

        public CategoriasModel()
        {
            ArticulosSeleccionados = new List<int>();
        }
    }
}
