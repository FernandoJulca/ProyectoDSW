using Microsoft.AspNetCore.Mvc;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Extensions;
using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;

namespace ProyectoDSWToolify.Controllers
{
    public class CarroController : Controller
    {
        private readonly IProducto productorepo;
        private readonly IVenta ventarepo;

        public CarroController(IProducto productorepo, IVenta ventarepo)
        {
            this.productorepo = productorepo;
            this.ventarepo = ventarepo;
        }

        // ---------- Vistas ----------

        // Mostrar página para finalizar compra con datos del usuario y carrito
        public IActionResult FinalizarCompra()
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<Carro>>("carrito") ?? new List<Carro>();

            var viewModel = new CarritoViewModel
            {
                Carrito = carrito,
                idUsuario = 2, 
                NombreUsuario = "María Ramírez",
                Correo = "maria.ramirez@example.com",
                Direccion = "Calle Real 456",
                Telefono = "987654322",
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
            var producto = productorepo.obtenerPorId(id);
            if (producto == null)
                return NotFound();

            var carrito = HttpContext.Session.GetObjectFromJson<List<Carro>>("carrito") ?? new List<Carro>();
            var itemExistente = carrito.FirstOrDefault(c => c.IdProducto == producto.idProducto);

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
                    IdProducto = producto.idProducto,
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
                imagenBase64 = c.Imagen != null ? $"data:image/png;base64,{Convert.ToBase64String(c.Imagen)}" : null
            });

            return Json(resultado);
        }

        // ---------- Compra ----------

        public IActionResult ConfirmarCompra()
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<Carro>>("carrito");
            int? idCliente = HttpContext.Session.GetInt32("ClienteId");

            if (carrito == null || carrito.Count == 0)
            {
                TempData["ErrorMessage"] = "No hay productos en el carrito.";
                return RedirectToAction("Productos", "Producto");
            }

            if (!idCliente.HasValue)
            {
                TempData["ErrorMessage"] = "Debe iniciar sesión para comprar.";
                return RedirectToAction("Login", "Usuario");
            }

            var venta = new Venta
            {
                usuario = new Usuario { idUsuario = idCliente.Value },
                total = carrito.Sum(c => c.Precio * c.Cantidad),
                Detalles = carrito.Select(c => new DetalleVenta
                {
                    producto = new Producto { idProducto = c.IdProducto },
                    cantidad = c.Cantidad,
                    subTotal = c.Precio * c.Cantidad
                }).ToList()
            };

            var resultado = ventarepo.generarVentaCliente(venta);

            if (resultado != null && resultado.idVenta > 0)
            {
                HttpContext.Session.Remove("carrito");
                TempData["GoodMessage"] = "Compra realizada con éxito. ID Venta: " + resultado.idVenta;
            }
            else
            {
                TempData["ErrorMessage"] = "Hubo un problema al procesar la compra.";
            }

            return RedirectToAction("Productos", "Producto");
        }

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

            var detalles = carrito.Select(item => new DetalleVenta
            {
                producto = new Producto { idProducto = item.IdProducto },
                cantidad = item.Cantidad,
                subTotal = item.Precio * item.Cantidad
            }).ToList();

            var venta = new Venta
            {
                usuario = new Usuario { idUsuario = IdUsuario },
                total = detalles.Sum(d => d.subTotal),
                tipoVenta = MetodoPago,
                Detalles = detalles
            };

            try
            {
                var ventaGenerada = ventarepo.generarVentaCliente(venta);
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
            var producto = productorepo.obtenerPorId(id);
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
