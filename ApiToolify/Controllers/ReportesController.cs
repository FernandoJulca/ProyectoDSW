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
        public IActionResult ListadoPorMesAndTipoVenta(DateTime fechaInicio, DateTime fechaFin, string? tipo = null) {
            var listadoRetornado = _dataReportes.ListadoPorMesAndTipoVenta(fechaInicio, fechaFin, tipo);
            System.Diagnostics.Debug.WriteLine("Listados retornados: " + listadoRetornado.Count);
            return Ok(listadoRetornado);
        }

        [HttpGet("ListarProductosPorCategoria")]
        public IActionResult ListarProductosPorCategoria(int idCategoria, string? orden)
        {
            var listadoRetornado = _dataReportes.ListarProductosPorCategoria(idCategoria, orden);
            System.Diagnostics.Debug.WriteLine("Listados retornados: " + listadoRetornado.Count);
            return Ok(listadoRetornado);
        }
    }
}
