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
        public async Task<byte[]> DescargarVentaPdf(int idCliente, int idVenta)
        {
            var response = await _httpClient.GetAsync($"venta/ventas/{idCliente}/pdf/{idVenta}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                throw new Exception($"Error al descargar el PDF: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
    }
}
