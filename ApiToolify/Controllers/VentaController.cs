using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Models;

namespace ApiToolify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        private readonly IVenta ventarepo;

        public VentaController(IVenta ventarepo)
        {
            this.ventarepo = ventarepo;
        }

        [HttpPost("confirmar-compra")]
        public IActionResult ConfirmarCompra([FromBody] Venta venta)
        {
            try
            {
                var resultado = ventarepo.generarVentaCliente(venta);
                return Ok(new { mensaje = "Compra realizada con éxito", idVenta = resultado.idVenta });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error: " + ex.Message });
            }
        }

        [HttpGet("ventas-usuario/{idUsuario}")]
        public IActionResult ObtenerVentasPorUsuario(int idUsuario)
        {
            var ventas = ventarepo.obtenerPorCliente(idUsuario);
            return Ok(ventas);
        }
    }
}
