using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdministradorPedidosApp.Models
{
    [Table("Articulos_Categorias")]
    public class Articulos_CategoriasModel
    {
        [Key]
        public int Id_Articulos_Categorias { get; set; }

        public int Id_Articulo { get; set; }

        public int Id_Categoria { get; set; }

        [ForeignKey("Id_Articulo")]
        public virtual ArticuloModel Articulo { get; set; }

        [ForeignKey("Id_Categoria")]
        public virtual CategoriasModel Categoria { get; set; }
    }
}
