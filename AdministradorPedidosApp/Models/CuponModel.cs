using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AdministradorPedidosApp.Models
{
    public class CuponModel
    {
        public int Id_Cupon { get; set; }

        public string? Descripcion { get; set; }

        public decimal PorcentajeDto { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public string TipoCupon { get; set; }

        public string? Url_Imagen { get; set; }

        public bool Activo { get; set; }

        public virtual ICollection<CuponDetalleModel>? Detalle { get; set; }

        [NotMapped]
        [JsonIgnore]
        public List<int> ArticulosSeleccionados { get; set; }

        [NotMapped]
        [JsonIgnore]
        public List<int> CategoriasSeleccionadas { get; set; }

        public CuponModel()
        {
            ArticulosSeleccionados = new List<int>();
            CategoriasSeleccionadas = new List<int>();
        }
    }
}
