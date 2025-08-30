using Newtonsoft.Json;
using System.Text;
using ProyectoDSWToolify.Services.Contratos;
using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;

namespace ProyectoDSWToolify.Services.Implementacion
{
    public class VentaService : IVentaService
    {
        private readonly HttpClient _httpClient;

        public VentaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<VentaViewModel> ConfirmarCompra(VentaViewModel venta)
        {
            var json = JsonConvert.SerializeObject(venta);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("venta/confirmar-compra", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var ventaConfirmada = JsonConvert.DeserializeObject<VentaViewModel>(responseJson);
                return ventaConfirmada;
            }
            else
            {
                throw new Exception("Error al confirmar la compra: " + response.ReasonPhrase);
            }
        }
        public async Task<byte[]> DescargarVentaPdf(int idUsuario, int idVenta)
        {
            var response = await _httpClient.GetAsync($"venta/ventas/{idUsuario}/pdf/{idVenta}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                throw new Exception($"Error al descargar el PDF: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }

        public async Task<List<Venta>> ListarVentasRemotas()
        {
            var response = await _httpClient.GetAsync("venta/ventasPendientesyTransportada");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var ventas = JsonConvert.DeserializeObject<List<Venta>>(json);
                return ventas;
            }
            else
            {
                throw new Exception($"Error al obtener las ventas remotas: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }

        public async Task<List<Venta>> ListarVentasRemotasPendientes()
        {
            var response = await _httpClient.GetAsync("venta/ventasPendientes");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var ventas = JsonConvert.DeserializeObject<List<Venta>>(json);
                return ventas;
            }
            else
            {
                throw new Exception($"Error al obtener las ventas remotas: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
        public async Task ActualizarEstadoVenta(int idVenta)
        {
            var response = await _httpClient.PostAsync($"venta/cambiarEstado?idVenta={idVenta}", null);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al actualizar el estado de la venta: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }

        public async Task GenerarEstadoVenta(int idVenta)
        {
            var response = await _httpClient.PostAsync($"venta/generarEstado?idVenta={idVenta}", null);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al actualizar el estado de la venta: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }

        public async Task CancelarEstadoVenta(int idVenta)
        {
            var response = await _httpClient.PostAsync($"venta/cancelarEstado?idVenta={idVenta}", null);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al actualizar el estado de la venta: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }

        public async Task<int> ContarPendientes()
        {
            var response = await _httpClient.GetAsync("venta/pendientescount");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error al obtener pendientes: {response.StatusCode} - {response.ReasonPhrase}");

            var json = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
            return obj["totalPendientes"];
        }

        public async Task<int> ContarGeneradas()
        {
            var response = await _httpClient.GetAsync($"venta/entregadascount");
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error al obtener generadas: {response.StatusCode} - {response.ReasonPhrase}");

            var json = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
            return obj["totalEntregadas"];
        }

        public async Task<Venta> ObtenerVentaPorUsuario(int idVenta)
        {
            var lista = await ListarVentasRemotas();
            return lista.FirstOrDefault(v => v.idVenta == idVenta);
        }

        public async Task<VentaViewModel> GenerarVentaVendedor(VentaViewModel v)
        {
            var json = JsonConvert.SerializeObject(v);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("venta/venta-generada", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var respuestaApi = JsonConvert.DeserializeObject<RespuestaVentaApi>(responseJson);

                return new VentaViewModel { idVenta = respuestaApi.idVenta };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al confirmar la compra: {response.ReasonPhrase}. Detalles: {errorContent}");
            }
        }
    }
}
