﻿using AdministradorPedidosApp.Models;
using AdministradorPedidosApp.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AdministradorPedidosApp.Interfaces
{
    public interface ICuponService
    {
        Task<List<CuponModel>> Index();

        // Llamado a api
        Task<string> AltaCupon([FromForm] CuponModel cupon, [FromForm] string? detalle = null, [FromForm] IFormFile? imagen = null);
        //Task<string> AltaCupon(CuponModel cupon);

        Task SubirImagenCupon(int id_Cupon, [FromForm] IFormFile? imagen = null);
    }
}