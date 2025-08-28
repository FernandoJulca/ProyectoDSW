using Newtonsoft.Json;
using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Services.Contratos;
using System.Net.Http;

namespace ProyectoDSWToolify.Services.Implementacion
{
    public class VendedorService : IVendedorService
    {
        private readonly HttpClient _httpClient;
        public VendedorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task EditarEstadoVenta(int idVenta, string nuevoEstado)
        {
            var response = await _httpClient.PutAsync($"vendedor/{idVenta}?nuevoEstado={nuevoEstado}", null);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al actualizar el estado de la venta: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }

        public async Task<List<Categoria>> ListCategoriasAsync()
        {
            var response = await _httpClient.GetAsync("vendedor/categorias");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Categoria>>(json);
        }

        public async Task<List<Producto>> ListProductosVendedorAsync()
        {
            var response = await _httpClient.GetAsync($"vendedor/productos");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Producto>>(json);
        }

        public async Task<List<Venta>> ObtenerLstPedidosAsync()
        {
            var response = await _httpClient.GetAsync($"vendedor/pedidos");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Venta>>(json);
        }

        public async Task<List<Venta>> ObtenerLstVentasAsync(int idUser)
        {
            var response = await _httpClient.GetAsync($"vendedor/ventas/{idUser}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Venta>>(json);
        }

        public async Task<Venta> ObtenerVentaPorId(int idVenta)
        {
            var response = await _httpClient.GetAsync($"vendedor/{idVenta}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Venta>(json);
        }
    }
}
