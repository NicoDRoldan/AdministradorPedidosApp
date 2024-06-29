using AdministradorPedidosApp.Data;
using AdministradorPedidosApp.Interfaces;
using AdministradorPedidosApp.Models;
using AdministradorPedidosApp.Models.DTOs;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<List<CategoriaDTO>> TraerCategorias()
        {
            try
            {
                var categoriaEntity = new List<CategoriaDTO>();

                var wsCuponesClient = _httpClientFactory.CreateClient("WSCuponesClient");
                wsCuponesClient.DefaultRequestHeaders.Accept.Clear();
                wsCuponesClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers
                    .MediaTypeWithQualityHeaderValue("appication/json"));

                var response = await wsCuponesClient.GetAsync("api/Categoria");

                if (response.IsSuccessStatusCode)
                {
                    var categoriaJson = await response.Content.ReadAsStringAsync();
                    categoriaEntity = JsonConvert.DeserializeObject<List<CategoriaDTO>>(categoriaJson);
                    return categoriaEntity;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();

                    throw new Exception(error);
                }
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> AltaOEditCupon([FromForm] CuponModel cupon, string endPoint, [FromForm] string? detalle = null,
            [FromForm] string? categoriasSeleccionadas = null, [FromForm] IFormFile? imagen = null)
        {
            if (detalle != null)
                cupon.Detalle = JsonConvert.DeserializeObject<List<CuponDetalleModel>>(detalle);

            if (categoriasSeleccionadas != null)
                cupon.CategoriasSeleccionadas = categoriasSeleccionadas.Split(',')
                                                    .Select(int.Parse)
                                                    .ToList();

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

                var response = new HttpResponseMessage();

                if (endPoint == "CrearCupon")
                {
                    response = await cuponClient.PostAsync($"api/Cupones/CrearCupon", content);
                }
                if(endPoint == "EditarCupon")
                {
                    response = await cuponClient.PutAsync($"api/Cupones/EditarCupon/{cupon.Id_Cupon}", content);
                }

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var cuponCreado = JsonConvert.DeserializeObject<CuponModel>(responseData);

                    dynamic responseObject = JsonConvert.DeserializeObject(responseData);

                    await SubirImagenCupon((int)responseObject.cupon.id_Cupon, imagen);

                    return responseObject.message;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception(error);
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

                if(imagen != null)
                {
                    using (var formData = new MultipartFormDataContent())
                    {
                        var streamContent = new StreamContent(imagen.OpenReadStream());
                        streamContent.Headers.ContentType = new System.Net.Http.Headers
                            .MediaTypeHeaderValue(imagen.ContentType);
                        formData.Add(streamContent, "imagen", imagen.FileName);

                        await cuponClient.PostAsync($"api/Cupones/SubirImagenCupon/{id_Cupon}", formData);
                    }
                }
                else
                {
                    await cuponClient.PostAsync($"api/Cupones/SubirImagenCupon/{id_Cupon}", null);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar la imagen del cupón: {ex.Message}");
            }
        }

        public async Task<CuponModel> ObtenerCuponPorId(int id)
        {
            try
            {
                var cuponClient = _httpClientFactory.CreateClient("WSCuponesClient");
                cuponClient.DefaultRequestHeaders.Accept.Clear();
                cuponClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers
                    .MediaTypeWithQualityHeaderValue("application/json"));

                var response = await cuponClient.GetAsync($"api/Cupones/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var cuponJson = await response.Content.ReadAsStringAsync();
                    var cuponModel = JsonConvert.DeserializeObject<CuponModel>(cuponJson);
                    return cuponModel;
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
                if (ex.Message.Contains("El equipo de destino denegó expresame dicha"))
                    error = "Error: No se pudo establecer conexión con el servidor.";

                throw new Exception(error);
            }
        }

        public async Task<List<ArticuloCuponDTO>> TraerArticulosSeleccionados(CuponModel cuponModel)
        {
            try
            {
                var articulos = await _context.Articulos
                    .Include(a => a.Precio)
                    .Where(a => a.Activo == true
                        && a.Precio.Precio != 0)
                    .ToListAsync();

                var diccionarioArticulosSeleccionados = cuponModel.Detalle
                    .ToDictionary(d => d.Id_ArticuloAsociado, d => d.Cantidad);

                var articulosSeleccionados = articulos.Select(a => new ArticuloCuponDTO
                {
                    Id_Articulo = a.Id_Articulo,
                    Nombre = a.Nombre,
                    ArticuloSeleccionado = diccionarioArticulosSeleccionados.ContainsKey(a.Id_Articulo) ? true : false,
                    Cantidad = diccionarioArticulosSeleccionados.ContainsKey(a.Id_Articulo) ? diccionarioArticulosSeleccionados[a.Id_Articulo] : 1
                }).ToList();

                return articulosSeleccionados;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
