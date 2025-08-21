using Microsoft.AspNetCore.Mvc;

namespace ProyectoDSWToolify.Controllers
{
    public class VendedorController : Controller
    {
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
            return View();
        }
    }
}
