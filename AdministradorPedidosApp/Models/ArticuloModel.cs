using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace AdministradorPedidosApp.Models
{
    [Table("Articulos")]
    public class ArticuloModel
    {
        [Key]
        public int Id_Articulo { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int Id_Rubro { get; set; }

        public string? Url_Imagen { get; set; }

        [ForeignKey("Id_Rubro")]
        public virtual RubroModel Rubro { get; set; }

        public virtual ICollection<Articulos_CategoriasModel> Articulos_Categorias { get; set; }

        public virtual PrecioModel? Precio { get; set; }

        [NotMapped]
        public List<int> CategoriasSeleccionadas { get; set; }

        public ArticuloModel()
        {
            CategoriasSeleccionadas = new List<int>();
        }
    }
}
