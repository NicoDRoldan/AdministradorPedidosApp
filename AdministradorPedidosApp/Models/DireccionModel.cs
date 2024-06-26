﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdministradorPedidosApp.Models
{
    [Table("Direccion")]
    public class DireccionModel
    {
        [Key]
        public int Id_Direccion { get; set; }

        public string Calle { get; set; }

        public int Numero { get; set; }

        public int Id_Provincia { get; set; }

        public int Id_Localidad { get; set; }


        [ForeignKey("Id_Provincia")]
        public virtual ProvinciaModel ProvinciaModel { get; set; }

        [ForeignKey("Id_Localidad")]
        public virtual LocalidadModel LocalidadModel { get; set; }
    }
}
