using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;
using ProyectoDSWToolify.Services.Contratos;

namespace ProyectoDSWToolify.Controllers
{
    public class RepartidorController : Controller
    {
        private readonly IVentaService ventaService;

        public RepartidorController(IVentaService ventaService)
        {
            this.ventaService = ventaService;

        }

        public async Task<IActionResult> Index()
        {
            var usuarioJson = HttpContext.Session.GetString("usuario");
            if (string.IsNullOrEmpty(usuarioJson))
            {
                TempData["ErrorMessage"] = "Necesitas iniciar sesión para acceder a tu perfil.";
                return RedirectToAction("Login", "UserAuth");
            }

            var usuario = JsonSerializer.Deserialize<Usuario>(usuarioJson);
            int id = usuario.idUsuario;

            var totalP = await ventaService.ContarPendientes();
            var totalG = await ventaService.ContarGeneradas();
            var pedidosPendientes = await ventaService.ListarVentasRemotasPendientes();

            Venta ventaEnRuta = null;
            var ventaEnRutaJson = HttpContext.Session.GetString("ventaEnRuta");
            if (!string.IsNullOrEmpty(ventaEnRutaJson))
            {
                ventaEnRuta = JsonSerializer.Deserialize<Venta>(ventaEnRutaJson);

                var ventaActualizada = await ventaService.ObtenerVentaPorUsuario(ventaEnRuta.idVenta);

                if (ventaActualizada == null)
                {
                    HttpContext.Session.Remove("ventaEnRuta");
                    ventaEnRuta = null;
                }
                else
                {

                    if (!ventaActualizada.estado.Equals("T", StringComparison.OrdinalIgnoreCase))
                    {
                        HttpContext.Session.Remove("ventaEnRuta");
                        ventaEnRuta = null;
                    }
                    else
                    {
                        ventaEnRuta = ventaActualizada; 
                    }
                }
            }

            var model = new RepartidorIndexViewModel
            {
                TotalPedidosEntregados = totalG,
                TotalPedidosPendientes = totalP,
                PedidosPendientes = pedidosPendientes,
                VentaEnRuta = ventaEnRuta,
                Usuario = usuario
            };

            return View(model);
        }

        public IActionResult ListarVentas()
        {
            var ventas = ventaService.ListarVentasRemotas().Result;
            return View(ventas);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstadoVenta(int idVenta)
        {
            try
            {
                await ventaService.ActualizarEstadoVenta(idVenta);

                var ventaActualizada = await ventaService.ObtenerVentaPorUsuario(idVenta);

                if (ventaActualizada != null)
                {
                    var ventaJson = JsonSerializer.Serialize(ventaActualizada);
                    HttpContext.Session.SetString("ventaEnRuta", ventaJson);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerarVenta(int idVenta)
        {
            try
            {
                await ventaService.GenerarEstadoVenta(idVenta);
                TempData["GoodMessage"] = $"Venta #{idVenta} entregada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Cancelar(int idVenta)
        {
            try
            {
                await ventaService.CancelarEstadoVenta(idVenta);
                TempData["GoodMessage"] = $"Venta #{idVenta} cancelada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Error");
            }
        }
    }
}
