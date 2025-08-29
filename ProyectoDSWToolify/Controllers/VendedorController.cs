using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;
using ProyectoDSWToolify.Services.Contratos;
using ProyectoDSWToolify.Services.Implementacion;

namespace ProyectoDSWToolify.Controllers
{
    public class VendedorController : Controller
    {
        private readonly IVendedorService _vendedorService;
        private readonly IVentaService _ventaService;
        private readonly IReporteService _reporteService;

        public VendedorController(IVendedorService vendedorService, IVentaService ventaService, IReporteService reporteService)
        {
            _vendedorService = vendedorService;
            _ventaService = ventaService;
            _reporteService = reporteService;
        }

        #region

        [HttpGet]
        public async Task<IActionResult> DatosMensuales()
        {
            var fechaActual = DateTime.Now.ToString("yyyy-MM");

            var metricas = new
            {
                ventasMensuales = await _reporteService.ContarVentasPorMesAsync(fechaActual),
                productosMensuales = await _reporteService.ContarProductosVendidosPorMesAsync(fechaActual),
                clientesMensuales = await _reporteService.ContarClientesAtendidosPorMesAsync(fechaActual),
                ingresosMensuales = await _reporteService.ObtenerIngresosTotalesAsync()
            };

            return Json(metricas);
        }

        [HttpGet]
        public async Task<IActionResult> DatosTotales()
        {
            var metricas = new
            {
                totalProductosVendidos = await _reporteService.ObtenerTotalProductosVendidosAsync(),
                totalVentas = await _reporteService.ObtenerTotalVentasAsync(),
                ingresosTotales = await _reporteService.ObtenerIngresosTotalesAsync()
            };

            return Json(metricas);
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Ventas()
        {
            return View();
        }

        public IActionResult Historial()
        {
            /*var usuarioJson = HttpContext.Session.GetString("usuario");
            if (string.IsNullOrEmpty(usuarioJson))
            {
                TempData["ErrorMessage"] = "Necesitas iniciar sesión para acceder a tu perfil.";
                return RedirectToAction("Login", "UserAuth");
            }

            var usuario = JsonSerializer.Deserialize<Usuario>(usuarioJson);
            int id = usuario.idUsuario;

            var ventas = await _vendedorService.ObtenerLstVentasAsync(id);
            */

            return View();
        }

        public IActionResult Pedidos()
        {
            return View();
        }
    }
}
