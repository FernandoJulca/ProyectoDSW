namespace ProyectoDSWToolify.Services.Contratos
{
    public interface IReporteService
    {
        Task<long> ContarVentasPorMesAsync(string fechaMes);
        Task<long> ContarProductosVendidosPorMesAsync(string fechaMes);
        Task<long> ContarClientesAtendidosPorMesAsync(string fechaMes);
        Task<long> ObtenerTotalVentasAsync();
        Task<long> ObtenerTotalProductosVendidosAsync();
        Task<double> ObtenerIngresosTotalesAsync();
    }
}
