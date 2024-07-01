using AdministradorPedidosApp.Controllers;
using AdministradorPedidosApp.Data;
using AdministradorPedidosApp.Interfaces;
using AdministradorPedidosApp.Models.DTOs;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace AdministradorPedidosApp.Services
{
    public class CategoriaCuponService : ICategoriaCuponService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AdministradorPedidosAppContext _context;

        public CategoriaCuponService(IHttpClientFactory httpClientFactory, AdministradorPedidosAppContext context)
        {
            _httpClientFactory = httpClientFactory;
            _context = context;
        }

        public async Task<List<CategoriaDTO>> GetCategoriasCupones()
        {
            try
            {
                var categorias = new List<CategoriaDTO>();
                var wsCuponesClient = _httpClientFactory.CreateClient("WSCuponesClient");
                wsCuponesClient.DefaultRequestHeaders.Accept.Clear();
                wsCuponesClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers
                    .MediaTypeWithQualityHeaderValue("application/json"));

                var response = await wsCuponesClient.GetAsync("api/Categoria");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    categorias = JsonConvert.DeserializeObject<List<CategoriaDTO>>(jsonResponse);
                    return categorias;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();

                    if (error.Contains("UseSqlServer"))
                        error = "Error: No se pudo establecer la conexión con la base de datos";

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

        public async Task<string> AltaCategoriaCupon(CategoriaDTO categoriaCupon)
        {
            try
            {
                var wsCuponesClient = _httpClientFactory.CreateClient("WSCuponesClient");
                wsCuponesClient.DefaultRequestHeaders.Accept.Clear();
                wsCuponesClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers
                    .MediaTypeWithQualityHeaderValue("application/json"));

                var json = JsonConvert.SerializeObject(categoriaCupon);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await wsCuponesClient.PostAsync("api/Categoria/CrearCategoria", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    dynamic responseObject = JsonConvert.DeserializeObject(responseData);

                    return responseObject.message;
                }
                else
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}
