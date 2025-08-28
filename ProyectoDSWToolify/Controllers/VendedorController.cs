using Microsoft.AspNetCore.Mvc;
using ProyectoDSWToolify.Services.Contratos;

namespace ProyectoDSWToolify.Controllers
{
    public class VendedorController : Controller
    {
        private readonly IVendedorService _vendedorService;
        private readonly IVentaService _ventaService;

        public VendedorController(IVendedorService vendedorService, IVentaService ventaService)
        {
            _vendedorService = vendedorService;
            _ventaService = ventaService;
        }
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
            //Hacer un ReportViewModel que almacene los datos Long
            return View();
        }

        public IActionResult Pedidos()
        {
            return View();
        }
    }
}
