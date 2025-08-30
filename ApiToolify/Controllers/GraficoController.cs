using ApiToolify.Data.Contratos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiToolify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraficoController : ControllerBase
    {

        private readonly IGrafico dataGrafico;

        public GraficoController(IGrafico inyect)
        {
            dataGrafico = inyect;
        }

        [HttpGet("CategoriaProducto")]
        public IActionResult CategoriaProducto(){

            return Ok(dataGrafico.CategoriaProducto("categoriaProducto"));
        }

        [HttpGet("ProveedorProducto")]
        public IActionResult ProveedorProducto()
        {

            return Ok(dataGrafico.ProveedorProducto("proveedorProducto"));
        }

        [HttpGet("VentaPorMes")]
        public IActionResult VentaPorMes()
        {

            return Ok(dataGrafico.VentaPorMes("ventaPorMes"));
        }

        [HttpGet("VentaPorMesAndTipoVenta")]
        public IActionResult VentaPorMesAndTipoVenta()
        {

            return Ok(dataGrafico.VentaPorMesAndTipoVenta("ventaPorMesAndTipoVenta"));
        }

        [HttpGet("VentaPorDistrito")]
        public IActionResult VentaPorDistrito()
        {

            return Ok(dataGrafico.VentaPorDistrito("ventaPorDistrito"));
        }
    }
}
