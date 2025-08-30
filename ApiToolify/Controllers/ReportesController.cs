using ApiToolify.Data.Contratos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiToolify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly IReportes _dataReportes;

        public ReportesController(IReportes inyect1)
        {
            this._dataReportes = inyect1;
        }


        [HttpGet("ListadoPorMesAndTipoVenta")]
        public IActionResult ListadoPorMesAndTipoVenta(DateTime? fechaInicio, DateTime? fechaFin, string? tipo) {
            var listadoRetornado = _dataReportes.ListadoPorMesAndTipoVenta(fechaInicio, fechaFin, tipo);

            System.Diagnostics.Debug.WriteLine("------------INICIANDO EL METODO ListadoPorMesAndTipoVenta API CONTROLLER -----------------");
            System.Diagnostics.Debug.WriteLine("Listados retornados: " + listadoRetornado.Count);
            System.Diagnostics.Debug.WriteLine("FECHA INICIO OBTENIDA API CONTROLLER " + fechaInicio);
            System.Diagnostics.Debug.WriteLine("FECHA FIN OBTENIDA API CONTROLLER " + fechaFin);
            System.Diagnostics.Debug.WriteLine("TIPO VENTA OBTENIDA API CONTROLLER " + tipo);

            System.Diagnostics.Debug.WriteLine("------------INICIANDO EL METODO ListadoPorMesAndTipoVenta API CONTROLLER -----------------");
            return Ok(listadoRetornado);
        }

        [HttpGet("ListarProductosPorCategoria")]
        public IActionResult ListarProductosPorCategoria(int? idCategoria = null, string? orden = null)
        {
            var listadoRetornado = _dataReportes.ListarProductosPorCategoria(idCategoria, orden);
            System.Diagnostics.Debug.WriteLine("Listados retornados: " + listadoRetornado.Count);
            return Ok(listadoRetornado);
        }
    }
}
