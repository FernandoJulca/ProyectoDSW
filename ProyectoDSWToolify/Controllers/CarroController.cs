using Microsoft.AspNetCore.Mvc;
using ProyectoDSWToolify.Extensions;
using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;
using ProyectoDSWToolify.Services.Contratos;

namespace ProyectoDSWToolify.Controllers
{
    public class CarroController : Controller
    {
        private readonly IClienteService _clienteService;
        private readonly IVentaService ventaService;

        public CarroController(IClienteService clienteService, IVentaService ventaService)
        {
            _clienteService = clienteService;
            this.ventaService = ventaService;
        }
        // Mostrar página para finalizar compra con datos del usuario y carrito
        public IActionResult FinalizarCompra()
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<Carro>>("carrito") ?? new List<Carro>();

            var usuario = HttpContext.Session.GetObjectFromJson<Usuario>("usuario");

            if (usuario == null)
            {
                TempData["ErrorMessage"] = "Necesitas iniciar sesión para poder culminar tu compra.";
                return RedirectToAction("Login", "UserAuth");
            }

            var viewModel = new CarritoViewModel
            {
                Carrito = carrito,
                idUsuario = usuario.idUsuario,
                NombreUsuario = $"{usuario.nombre} {usuario.apePaterno} {usuario.apeMaterno}",
                Correo = usuario.correo,
                Direccion = usuario.direccion,
                Telefono = usuario.telefono,
                MetodoPago = "",
                Tarjeta = new TarjetaViewModel(),
                Meses = Enumerable.Range(1, 12).Select(m => m.ToString("D2")).ToList(),
                Anios = Enumerable.Range(DateTime.Now.Year, 10).Select(y => y.ToString()).ToList()
            };

            return View(viewModel);
        }


        // ---------- Carrito: Agregar, Quitar, Obtener ----------

        [HttpPost]
        public IActionResult AgregarAlCarrito(int id, int cantidad = 1)
        {
            var producto = _clienteService.ObtenerProductoPorIdAsync(id).Result;
            if (producto == null)
                return NotFound();

            var carrito = HttpContext.Session.GetObjectFromJson<List<Carro>>("carrito") ?? new List<Carro>();
            var itemExistente = carrito.FirstOrDefault(c => c.IdProducto == producto.id);

            int cantidadActual = itemExistente?.Cantidad ?? 0;
            int nuevaCantidad = cantidadActual + cantidad;

            if (nuevaCantidad > producto.stock)
            {
                return BadRequest(new
                {
                    mensaje = $"No hay suficiente stock disponible para agregar este producto. Stock máximo disponible: {producto.stock}."
                });
            }

            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Add(new Carro
                {
                    IdProducto = producto.id,
                    Nombre = producto.nombre,
                    Imagen = producto.imagen,
                    Precio = producto.precio,
                    Cantidad = cantidad
                });
            }

            HttpContext.Session.SetObjectAsJson("carrito", carrito);
            return Ok(new { mensaje = "Producto agregado" });
        }

        [HttpPost]
        public IActionResult QuitarDelCarrito(int idProducto)
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<Carro>>("carrito") ?? new List<Carro>();
            var item = carrito.FirstOrDefault(c => c.IdProducto == idProducto);

            if (item != null)
            {
                carrito.Remove(item);
                HttpContext.Session.SetObjectAsJson("carrito", carrito);
                return Json(new { success = true, mensaje = "Producto eliminado del carrito." });
            }

            return Json(new { success = false, mensaje = "Producto no encontrado en el carrito." });
        }

        [HttpGet]
        public IActionResult ObtenerCarrito()
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<Carro>>("carrito") ?? new List<Carro>();

            var resultado = carrito.Select(c => new
            {
                idProducto = c.IdProducto,
                nombre = c.Nombre,
                precio = c.Precio,
                cantidad = c.Cantidad,
                subTotal = c.Precio * c.Cantidad,
                imagen = string.IsNullOrEmpty(c.Imagen)
    ? Url.Content("~/assets/productos/P" + c.IdProducto + ".jpg")
    : Url.Content(c.Imagen)

            });

            return Json(resultado);
        }


        // ---------- Compra ----------



        [HttpPost]
        public IActionResult RealizarPago(
     int IdUsuario,
     string MetodoPago,
     string TarjetaNumero,
     string TarjetaMes,
     string TarjetaAnio,
     string TarjetaCVV)
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<Carro>>("carrito");

            if (carrito == null || !carrito.Any())
            {
                TempData["ErrorMessage"] = "El carrito está vacío.";
                return RedirectToAction("FinalizarCompra");
            }

            var detalles = carrito.Select(item => new DetalleVentaViewModel
            {
                idProducto = item.IdProducto,
                cantidad = item.Cantidad,
                subTotal = item.Precio * item.Cantidad
            }).ToList();

            var ventaViewModel = new VentaViewModel
            {
                idUsuario = IdUsuario,
                total = detalles.Sum(d => d.subTotal),
                tipoVenta = MetodoPago,
                estado = "P", // o lo que corresponda según tu lógica
                detalles = detalles
            };

            try
            {
                var ventaConfirmada = ventaService.ConfirmarCompra(ventaViewModel).Result;
                HttpContext.Session.Remove("carrito");

                TempData["GoodMessage"] = "Compra realizada exitosamente.";
                return RedirectToAction("Index", "Cliente");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un error al procesar la compra: " + ex.Message;
                return RedirectToAction("FinalizarCompra");
            }
        }


        // ---------- Modificar cantidades en carrito ----------

        [HttpPost]
        public IActionResult SumarCantidad(int id)
        {
            var producto = _clienteService.ObtenerProductoPorIdAsync(id).Result;
            if (producto == null)
                return NotFound();

            var carrito = HttpContext.Session.GetObjectFromJson<List<Carro>>("carrito") ?? new List<Carro>();
            var item = carrito.FirstOrDefault(c => c.IdProducto == id);

            if (item != null)
            {
                if (item.Cantidad + 1 > producto.stock)
                {
                    TempData["ErrorMessage"] = "No hay suficiente stock para agregar más unidades.";
                }
                else
                {
                    item.Cantidad += 1;
                }

                HttpContext.Session.SetObjectAsJson("carrito", carrito);
            }

            return RedirectToAction("FinalizarCompra");
        }

        [HttpPost]
        public IActionResult RestarCantidad(int id)
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<Carro>>("carrito") ?? new List<Carro>();
            var item = carrito.FirstOrDefault(c => c.IdProducto == id);

            if (item != null)
            {
                item.Cantidad -= 1;
                if (item.Cantidad <= 0)
                    carrito.Remove(item);

                HttpContext.Session.SetObjectAsJson("carrito", carrito);
            }

            return RedirectToAction("FinalizarCompra");
        }
    }
}
