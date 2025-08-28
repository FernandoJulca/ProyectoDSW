using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Data.Repositorios;

namespace ApiToolify.Controllers
{
    [Route("api/vendedor")]
    [ApiController]
    public class VendedorController : ControllerBase
    {
        private readonly ICategoria categoriarepo;
        private readonly IProducto productorepo;
        private readonly IVenta ventarepo;


        public VendedorController(ICategoria categoriarepo, IProducto productorepo, IVenta ventarepo)
        {
            this.categoriarepo = categoriarepo;
            this.productorepo = productorepo;
            this.ventarepo = ventarepo;
        }

        [HttpGet("categorias")]
        public IActionResult ListarCategorias()
        {
            var categorias = categoriarepo.listCategoriasCliente();
            return Ok(categorias);
        }

        [HttpGet("productos")]
        public IActionResult ListarProductosByCategorias([FromQuery] int? idCategoria)
        {
            var productos = productorepo.listProductosVendedor().ToList();

            if (idCategoria.HasValue)
            {
                productos = productos
                    .Where(p => p.categoria.idCategoria == idCategoria.Value)
                    .ToList();
            }
            // 204 (No Content)
            if (!productos.Any())
            {
                return NoContent();
            }
            return Ok(productos);
        }

        [HttpGet("ventas/{idVendedor}")]
        public IActionResult ObtenerVentasPorVendedore(int idVendedor)
        {
            var ventas = ventarepo.obtenerPorVendedor(idVendedor);
            return Ok(ventas);
        }

        [HttpGet("historial/{idVendedor}/{idVenta}")]
        public IActionResult ObtenerInfoVenta(int idVenta, int idVendedor)
        {
            var ventas = ventarepo.obtenerVentaPorUsuario(idVenta, idVendedor);
            return Ok(ventas);
        }

        [HttpGet("{idVenta}")]
        public IActionResult ObtenerVentaPorId(int idVenta)
        {
            var venta = ventarepo.obtenerVentaPorId(idVenta);
            return Ok(venta);
        }

        
        [HttpGet("pedidos")]
        public IActionResult ObtenerListaPedidos()
        {
            var ventas = ventarepo.obtenerLstPedidos();
            return Ok(ventas);
        }

        [HttpPut("{idVenta}")]
        public IActionResult CambiarEstadoVenta(int idVenta, string nuevoEstado)
        {
            var venta = ventarepo.editarEstadoVenta(idVenta, nuevoEstado);
            return Ok(venta);
        }

    }
}
