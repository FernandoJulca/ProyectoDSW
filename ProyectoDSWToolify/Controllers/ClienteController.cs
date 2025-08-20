namespace ProyectoDSWToolify.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using global::ProyectoDSWToolify.Services.Contratos;
    using global::ProyectoDSWToolify.Models.ViewModels;
    using System.Text.Json;
    using global::ProyectoDSWToolify.Models;

    namespace ProyectoDSWToolify.Controllers
    {
        public class ClienteController : Controller
        {
            private readonly IClienteService _clienteService;
            private readonly IVentaService _ventaService;

            public ClienteController(IClienteService clienteService, IVentaService ventaService)
            {
                _clienteService = clienteService;
                _ventaService = ventaService;
            }

            // El método ahora es async porque consume API async
            public async Task<IActionResult> Index()
            {
                var resumen = await _clienteService.ObtenerResumenAsync();

                var vm = new IndexViewModel
                {
                    TotalProductos = resumen.TotalProductos,
                    TotalClientes = resumen.TotalClientes,
                    CategoriasMasVendidas = resumen.CategoriasMasVendidas
                };

                return View(vm);
            }

            public async Task<IActionResult> Producto(List<int> categorias = null, string orden = "asc", int pagina = 1)
            {
                categorias = categorias?.Distinct().ToList() ?? new List<int>();

                if (pagina < 1) pagina = 1;
                var todasCategorias = await _clienteService.ObtenerCategorias();

                var productos = await _clienteService.ObtenerProductosAsync(categorias, orden, pagina);

                int tamañoPagina = 12;
                int totalProductos = await _clienteService.ContarProductosAsync(categorias);
                int totalPaginas = (int)System.Math.Ceiling((double)totalProductos / tamañoPagina);

                var modelo = new ProductosViewModel
                {
                    Categorias = todasCategorias,
                    Productos = productos,
                    IdCategoriasSeleccionadas = categorias ?? new List<int>(),
                    OrdenPrecio = orden,
                    PaginaActual = pagina,
                    TotalPaginas = totalPaginas,
                    TamañoPagina = tamañoPagina
                };

                return View(modelo);
            }

            [HttpGet]
            public async Task<IActionResult> ObtenerProductoPorId(int id)
            {
                var producto = await _clienteService.ObtenerProductoPorIdAsync(id);
                if (producto == null) return NotFound();

                string imagenUrl;

                if (string.IsNullOrEmpty(producto.imagen))
                {
                    imagenUrl = Url.Content("~/assets/productos/P" + producto.id + ".jpg");
                }
                else
                {
                    imagenUrl = producto.imagen;
                }

                return Json(new
                {
                    id = producto.id,
                    nombre = producto.nombre,
                    descripcion = producto.descripcion,
                    categoria = producto.categoria,
                    precio = producto.precio.ToString("F2"),
                    stock = producto.stock,
                    imagen = imagenUrl
                });
            }

            public async Task<IActionResult> Nosotros()
            {
                var resumen = await _clienteService.ObtenerResumenAsync();

                var vm = new IndexViewModel
                {
                    TotalProductos = resumen.TotalProductos,
                    TotalClientes = resumen.TotalClientes,
                    CategoriasMasVendidas = resumen.CategoriasMasVendidas
                };

                return View(vm);
            }

            public IActionResult Contacto()
            {
                return View();
            }
            [HttpGet]
            public async Task<IActionResult> Perfil()
            {
                var usuarioJson = HttpContext.Session.GetString("usuario");
                if (string.IsNullOrEmpty(usuarioJson))
                {
                    TempData["ErrorMessage"] = "Necesitas iniciar sesión para acceder a tu perfil.";
                    return RedirectToAction("Login", "UserAuth");
                }

                var usuario = JsonSerializer.Deserialize<Usuario>(usuarioJson);

                int id = usuario.idUsuario;

                var ventas = await _clienteService.ObtenerVentasClienteAsync(id);

                var viewModel = new PerfilClienteViewModel
                {
                    idCliente = id,
                    Nombre = $"{usuario.nombre} {usuario.apePaterno} {usuario.apeMaterno}",
                    Email = usuario.correo,
                    HistorialVentas = ventas
                };

                return View(viewModel);
            }

            public async Task<IActionResult> DescargarVentaPdf(int idCliente, int idVenta)
            {
                try
                {
                    var pdfBytes = await _ventaService.DescargarVentaPdf(idCliente, idVenta);
                    return File(pdfBytes, "application/pdf", $"venta_{idVenta}.pdf");
                }
                catch (Exception ex)
                {
                    // Maneja error (por ejemplo, mostrar mensaje)
                    return NotFound(ex.Message);
                }
            }

        }
    }
}
