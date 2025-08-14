using Microsoft.AspNetCore.Mvc;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;

namespace ProyectoDSWToolify.Controllers
{
    public class ClienteController : Controller
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

        public IActionResult Index()
        {
            var totalProductos = productorepo.contadorProductos();
            var totalClientes = usuariorepo.contadorClientes();
            var categoriasMasVendidas = categoriarepo.top4CategoriasMasVendidas();
            var vm = new IndexViewModel
            {
                CategoriasMasVendidas = categoriasMasVendidas,
                TotalProductos = totalProductos,
                TotalClientes = totalClientes
            };

            return View(vm);
        }
        public IActionResult Producto(List<int> categorias = null, string orden = "asc", int pagina = 1)
        {
            var todasCategorias = categoriarepo.listCategoriasCliente().ToList();
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
            int totalProductos = todosProductos.Count();
            int totalPaginas = (int)Math.Ceiling((double)totalProductos / tamañoPagina);
            var productosPaginados = todosProductos
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToList();

            var modelo = new ProductosViewModel
            {
                Categorias = todasCategorias,
                Productos = productosPaginados,
                IdCategoriasSeleccionadas = categorias ?? new List<int>(),
                OrdenPrecio = orden,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas,
                TamañoPagina = tamañoPagina
            };

            return View(modelo);
        }
        [HttpGet]
        public IActionResult ObtenerProductoPorId(int id)
        {
            var producto = productorepo.obtenerPorId(id); 
            if (producto == null) return NotFound();

            return Json(new
            {
                id = producto.idProducto,
                nombre = producto.nombre,
                descripcion = producto.descripcion,
                categoria = producto.categoria.descripcion,
                precio = producto.precio.ToString("F2"),
                stock = producto.stock,
                imagen = string.IsNullOrEmpty(producto.imagenBase64)
                    ? Url.Content("~/assets/productos/P" + producto.idProducto + ".jpg")
                    : "data:image/png;base64," + producto.imagenBase64
            });
        }
        public IActionResult Nosotros()
        {
            var totalProductos = productorepo.contadorProductos();
            var totalClientes = usuariorepo.contadorClientes();
            var vm = new IndexViewModel
            {
                TotalProductos = totalProductos,
                TotalClientes = totalClientes
            };
            return View(vm);
        }
        public IActionResult Contacto()
        {
            return View();
        }

        public IActionResult Perfil()
        {
            int id = 2; 

            var ventas = ventarepo.obtenerPorCliente(id);

            var viewModel = new PerfilClienteViewModel
            {
                idCliente = id,
                Nombre = "María Ramírez", // Simulado
                Email = "maria.ramirez@example.com", // Simulado
                HistorialVentas = ventas
            };

            return View(viewModel);
        }

    }
}
