using AdministradorPedidosApp.Data;
using AdministradorPedidosApp.Interfaces;
using AdministradorPedidosApp.Models;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Text;

namespace AdministradorPedidosApp.Services
{
    public class CuponService : ICuponService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AdministradorPedidosAppContext _context;

        public CuponService (IHttpClientFactory httpClientFactory,  AdministradorPedidosAppContext context)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
        }

        public async Task<List<CuponModel>> Index()
        {
            try
            {
                var vMCupones = new List<CuponModel>();
                var wsCuponesClient = _httpClientFactory.CreateClient("WSCuponesClient");
                wsCuponesClient.DefaultRequestHeaders.Accept.Clear();
                wsCuponesClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers
                    .MediaTypeWithQualityHeaderValue("application/json"));

                var response = await wsCuponesClient.GetAsync("api/Cupones");

                if (response.IsSuccessStatusCode)
                {
                    var cuponJson = await response.Content.ReadAsStringAsync();
                    vMCupones = JsonConvert.DeserializeObject<List<CuponModel>>(cuponJson);
                    return vMCupones;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();

                    if (error.Contains("UseSqlServer"))
                        error = "Error: No se pudo establecer la conexión con la base de datos.";

                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;

                if (ex.Message.Contains("el equipo de destino denegó expresamente dicha conexión"))
                    error = "Error: No se pudo establecer la conexión con el servidor.";

                throw new Exception(error);
            }
        }

        public async Task<string> AltaCupon([FromForm] CuponModel cupon, [FromForm] string? detalle = null, [FromForm] IFormFile? imagen = null)
        {
            if (detalle != null)
            {
                cupon.Detalle = JsonConvert.DeserializeObject<List<CuponDetalleModel>>(detalle);
            }

            if ((!cupon.Detalle.Any()) && cupon.TipoCupon == "PROMO")
                throw new Exception("Por favor cargar artículos");

            if (cupon.Detalle.Any() && cupon.TipoCupon == "DESCUENTO")
                cupon.Detalle.Clear();

            try
            {
                var cuponClient = _httpClientFactory.CreateClient("WSCuponesClient");
                cuponClient.DefaultRequestHeaders.Accept.Clear();
                cuponClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers
                    .MediaTypeWithQualityHeaderValue("application/json"));

                var cuponJson = JsonConvert.SerializeObject(cupon);

                var content = new StringContent(cuponJson, Encoding.UTF8, "application/json");

                var response = await cuponClient.PostAsync("api/Cupones/CrearCupon", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var cuponCreado = JsonConvert.DeserializeObject<CuponModel>(responseData);
                    if(imagen != null)
                    {
                        await SubirImagenCupon(cuponCreado.Id_Cupon, imagen);
                    }
                    return $"Cupón de {cupon.TipoCupon.ToLower()} creado correctamente.";
                }
                else
                {
                    throw new Exception($"Error al crear el cupón: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task SubirImagenCupon(int id_Cupon, [FromForm] IFormFile? imagen = null)
        {
            try
            {
                var cuponClient = _httpClientFactory.CreateClient("WSCuponesClient");
                cuponClient.DefaultRequestHeaders.Accept.Clear();
                cuponClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers
                    .MediaTypeWithQualityHeaderValue("application/json"));

                using (var formData = new MultipartFormDataContent())
                {
                    // Modelo de Cupon
                    formData.Add(new StringContent(id_Cupon.ToString()), "id_Cupon");
                    // Imagen
                    if (imagen != null)
                    {
                        var streamContent = new StreamContent(imagen.OpenReadStream());
                        streamContent.Headers.ContentType = new System.Net.Http.Headers
                            .MediaTypeHeaderValue(imagen.ContentType);
                        formData.Add(streamContent, "imagen", imagen.FileName);
                    }
                    var response = await cuponClient.PostAsync($"api/Cupones/SubirImagenCupon/{id_Cupon}", formData);

                    //if (response.IsSuccessStatusCode)
                    //{
                    //}
                    //else
                    //{
                    //    throw new Exception($"Error al cargar la imagen del cupón: {response.ReasonPhrase}");
                    //}
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar la imagen del cupón: {ex.Message}");
            }
        }

        //public async Task<string> AltaCupon(CuponModel cupon)
        //{
        //    if ((!cupon.Detalle.Any()) && cupon.TipoCupon == "PROMO")
        //        throw new Exception("Por favor cargar artículos");

        //    if (cupon.Detalle.Any() && cupon.TipoCupon == "DESCUENTO")
        //        cupon.Detalle.Clear();

        //    try
        //    {
        //        var cuponClient = _httpClientFactory.CreateClient("WSCuponesClient");
        //        cuponClient.DefaultRequestHeaders.Accept.Clear();
        //        cuponClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers
        //            .MediaTypeWithQualityHeaderValue("application/json"));

        //        var cuponJson = JsonConvert.SerializeObject(cupon);

        //        var content = new StringContent(cuponJson, Encoding.UTF8, "application/json");

        //        var response = await cuponClient.PostAsync("api/Cupones/CrearCupon", content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            return $"Cupón de {cupon.TipoCupon.ToLower()} creado correctamente.";
        //        }
        //        else
        //        {
        //            throw new Exception($"Error al crear el cupón: {response.ReasonPhrase}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error: {ex.Message}");
        //    }
        //}
    }
}
