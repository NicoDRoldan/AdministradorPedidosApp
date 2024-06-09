using AdministradorPedidosApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AdministradorPedidosApp.Data
{
    public class AdministradorPedidosAppContext : DbContext
    {
        public AdministradorPedidosAppContext(DbContextOptions<AdministradorPedidosAppContext> options) : base(options) { }

        public DbSet<AdministradorPedidosApp.Models.ArticuloModel> Articulos { get; set; }
        public DbSet<AdministradorPedidosApp.Models.CategoriasModel> Categorias { get; set; }
        public DbSet<AdministradorPedidosApp.Models.Articulos_CategoriasModel> Articulos_Categorias { get; set; }
        public DbSet<AdministradorPedidosApp.Models.DireccionModel> Direccion { get; set; }
        public DbSet<AdministradorPedidosApp.Models.LocalidadModel> Localidades { get; set; }
        public DbSet<AdministradorPedidosApp.Models.PedidoDetalleModel> PedidosDetalle { get; set; }
        public DbSet<AdministradorPedidosApp.Models.PedidoModel> Pedidos { get; set; }
        public DbSet<AdministradorPedidosApp.Models.PrecioModel> Precios { get; set; }
        public DbSet<AdministradorPedidosApp.Models.PromocionDetalleModel> PromocionesDetalles { get; set; }
        public DbSet<AdministradorPedidosApp.Models.PromocionModel> Promociones { get; set; }
        public DbSet<AdministradorPedidosApp.Models.ProvinciaModel> Provincias { get; set; }
        public DbSet<AdministradorPedidosApp.Models.RubroModel> Rubros { get; set; }
        public DbSet<AdministradorPedidosApp.Models.SucursalModel> Sucursales { get; set; }
        public DbSet<AdministradorPedidosApp.Models.TipoSucursalModel> TipoSucursal { get; set; }
        public DbSet<AdministradorPedidosApp.Models.UsuarioModel> Usuarios { get; set; }
        public DbSet<AdministradorPedidosApp.Models.RolModel> Rol { get; set; }
        public DbSet<AdministradorPedidosApp.Models.CodigoRecuperacionModel> CodigosRecuperacion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PedidoDetalleModel>()
                .HasKey(pd => new { pd.NumPedido, pd.Renglon });

            modelBuilder.Entity<PromocionDetalleModel>()
                .HasKey(pd => new { pd.Id_Promocion, pd.Id_Articulo});
            
            // Configuración de la relación muchos a muchos entre Articulos y Categorias
            modelBuilder.Entity<Articulos_CategoriasModel>()
                .HasOne(ac => ac.Articulo)
                .WithMany(a => a.Articulos_Categorias)
                .HasForeignKey(ac => ac.Id_Articulo);

            //modelBuilder.Entity<Articulos_CategoriasModel>()
            //    .HasOne(ac => ac.Categoria)
            //    .WithMany(c => c.Articulos)
            //    .HasForeignKey(ac => ac.Id_Categoria);

            base.OnModelCreating(modelBuilder);
        }
    }
}
