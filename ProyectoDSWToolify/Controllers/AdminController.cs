using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoDSWToolify.Controllers
{
    public class AdminController : Controller
    {
        //[Authorize(Roles = "A")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
