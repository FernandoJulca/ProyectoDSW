using Newtonsoft.Json;
using ProyectoDSWToolify.Services.Contratos;

namespace ProyectoDSWToolify.Services.Implementacion
{
    public class ReporteService : IReporteService
    {
        private readonly HttpClient _httpClient;
        public ReporteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<long> ContarVentasPorMesAsync(string fechaMes)
        {
            var response = await _httpClient.GetAsync($"reporte/mensual/ventas/{fechaMes}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<long>(json);
        }

        public async Task<long> ContarProductosVendidosPorMesAsync(string fechaMes)
        {
            var response = await _httpClient.GetAsync($"reporte/mensual/productos/{fechaMes}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<long>(json);
        }

        public async Task<long> ContarClientesAtendidosPorMesAsync(string fechaMes)
        {
            var response = await _httpClient.GetAsync($"reporte/mensual/clientes/{fechaMes}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<long>(json);
        }

        public async Task<long> ObtenerTotalVentasAsync()
        {
            var response = await _httpClient.GetAsync("reporte/total/ventas");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<long>(json);
        }

        public async Task<long> ObtenerTotalProductosVendidosAsync()
        {
            var response = await _httpClient.GetAsync("reporte/total/productos");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<long>(json);
        }

        public async Task<double> ObtenerIngresosTotalesAsync()
        {
            var response = await _httpClient.GetAsync("reporte/total/ingresos");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<double>(json);
        }
    }
}
