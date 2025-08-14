using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoDSWToolify.Data.Contratos;

namespace ApiToolify.Controllers
{
    [Route("api/cliente")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly ICategoria categoriarepo;
        private readonly IProducto productorepo;
        private readonly IUsuario usuariorepo;
        private readonly IVenta ventarepo;

        public ClienteController(ICategoria categoriarepo, IProducto productorepo, IUsuario usuariorepo, IVenta ventarepo)
        {
            this.categoriarepo = categoriarepo;
            this.productorepo = productorepo;
            this.usuariorepo = usuariorepo;
            this.ventarepo = ventarepo;
        }

        [HttpGet("categorias")]
        public IActionResult ObtenerCategorias()
        {
            var categorias = categoriarepo.listCategoriasCliente();
            return Ok(categorias);
        }
        [HttpGet("productos/count")]
        public IActionResult ContarProductos([FromQuery] List<int> categorias)
        {
            var productos = productorepo.listProductosCliente().ToList();

            if (categorias != null && categorias.Any())
            {
                productos = productos.Where(p => categorias.Contains(p.categoria.idCategoria)).ToList();
            }

            return Ok(productos.Count);
        }


        // GET: api/clienteapi/productos
        [HttpGet("productos")]
        public IActionResult ObtenerProductos([FromQuery] List<int> categorias, [FromQuery] string orden = "asc", [FromQuery] int pagina = 1)
        {
            var todosProductos = productorepo.listProductosCliente().ToList();

            if (categorias != null && categorias.Any())
            {
                todosProductos = todosProductos
                    .Where(p => categorias.Contains(p.categoria.idCategoria))
                    .ToList();
            }

            todosProductos = orden == "desc"
                ? todosProductos.OrderByDescending(p => p.precio).ToList()
                : todosProductos.OrderBy(p => p.precio).ToList();

            int tamañoPagina = 12;
            var productosPaginados = todosProductos
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToList();

            return Ok(new
            {
                productos = productosPaginados,
                total = todosProductos.Count,
                paginaActual = pagina
            });
        }

        // GET: api/clienteapi/producto/5
        [HttpGet("producto/{id}")]
        public IActionResult ObtenerProductoPorId(int id)
        {
            Console.WriteLine($"Buscando producto con ID: {id}");
            var producto = productorepo.obtenerPorId(id);
            if (producto == null) return NotFound();

            return Ok(new
            {
                id = producto.idProducto,
                nombre = producto.nombre,
                descripcion = producto.descripcion,
                categoria = producto.categoria.descripcion,
                precio = producto.precio.ToString("F2"),
                stock = producto.stock,
                imagen = string.IsNullOrEmpty(producto.imagen)
                    ? Url.Content("~/assets/productos/P" + producto.idProducto + ".jpg")
                    : "data:image/png;base64," + producto.imagen
            });
        }

        // GET: api/clienteapi/ventas/2
        [HttpGet("ventas/{idCliente}")]
        public IActionResult ObtenerVentasCliente(int idCliente)
        {
            var ventas = ventarepo.obtenerPorCliente(idCliente);
            return Ok(ventas);
        }

        // GET: api/clienteapi/resumen
        [HttpGet("resumen")]
        public IActionResult ObtenerResumen()
        {
            var totalProductos = productorepo.contadorProductos();
            var totalClientes = usuariorepo.contadorClientes();
            var categoriasMasVendidas = categoriarepo.top4CategoriasMasVendidas();

            return Ok(new
            {
                totalProductos,
                totalClientes,
                categoriasMasVendidas
            });
        }
    }
}
