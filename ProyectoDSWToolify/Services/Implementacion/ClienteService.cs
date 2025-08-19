using Newtonsoft.Json;
using ProyectoDSWToolify.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ProyectoDSWToolify.Models.ViewModels;
using ProyectoDSWToolify.Services.Contratos;

namespace ProyectoDSWToolify.Services.Implementacion
{
    public class ClienteService : IClienteService
    {
        private readonly HttpClient _httpClient;

        public ClienteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<Categoria>> ObtenerCategorias()
        {
            var response = await _httpClient.GetAsync("cliente/categorias");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Categoria>>(json);
        }

        public async Task<List<Producto>> ObtenerProductosAsync(List<int> categorias, string orden, int pagina)
        {
            var queryCategorias = categorias != null && categorias.Any() ? string.Join("&categorias=", categorias) : "";
            var url = $"cliente/productos?orden={orden}&pagina={pagina}";

            if (!string.IsNullOrEmpty(queryCategorias))
                url += $"&categorias={queryCategorias}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var resultado = JsonConvert.DeserializeObject<dynamic>(json);

            return resultado.productos.ToObject<List<Producto>>();
        }

        public async Task<ProductoDetallesViewModel> ObtenerProductoPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"cliente/producto/{id}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProductoDetallesViewModel>(json);
        }

        public async Task<List<Venta>> ObtenerVentasClienteAsync(int idCliente)
        {
            var response = await _httpClient.GetAsync($"cliente/ventas/{idCliente}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Venta>>(json);
        }
        public async Task<int> ContarProductosAsync(List<int> categorias)
        {
            var queryCategorias = categorias != null && categorias.Any() ? string.Join("&categorias=", categorias) : "";
            var url = $"cliente/productos/count";

            if (!string.IsNullOrEmpty(queryCategorias))
                url += $"?categorias={queryCategorias}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            int count = JsonConvert.DeserializeObject<int>(json);

            return count;
        }

        public async Task<IndexViewModel> ObtenerResumenAsync()
        {
            var response = await _httpClient.GetAsync("cliente/resumen");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IndexViewModel>(json);
        }
    }
}
