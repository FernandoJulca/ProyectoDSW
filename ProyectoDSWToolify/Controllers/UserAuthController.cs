using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;
using ProyectoDSWToolify.Services.Contratos;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Net;
using System.Threading.Tasks;



namespace ProyectoDSWToolify.Controllers
{
    public class UserAuthController : Controller
    {
        private readonly IUserAuthService userAuthService;
        private readonly IConfiguration _config;

        public UserAuthController(IUserAuthService userAuthService, IConfiguration config)
        {
            this.userAuthService = userAuthService;
            _config = config;
        }
        #region
        private async Task<List<Distrito>> obtenerListaDistritos()
        {

            var listaDistrito = new List<Distrito>();
            using (var ClienteHttp = new HttpClient())
            {
                ClienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var msg = await ClienteHttp.GetAsync("Distrito");
                var data = await msg.Content.ReadAsStringAsync();

                listaDistrito = JsonConvert.DeserializeObject<List<Distrito>>(data);
            }
            return listaDistrito;
        }
        private async Task CargarViewBags()
        {
            var distritos = await obtenerListaDistritos();
            ViewBag.distritos = new SelectList(distritos, "idDistrito", "nombre");
        }

        #endregion
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginResponse responselogin)
        {
            var usuario = await userAuthService.iniciarSession(responselogin);

            if (usuario == null)
            {
                TempData["ErrorMessage"] = "Credenciales inválidas";
                return View();
            }

            /*Agregando Cookies*/
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name , usuario.nombre),
                new Claim ("Apellido", usuario.apePaterno),
                new Claim("Correo" , usuario.correo),
                new Claim("UserId", usuario.idUsuario.ToString())
            };

            claims.Add(new Claim(ClaimTypes.Role, usuario.rol.descripcion));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));



            HttpContext.Session.SetString("usuario", JsonConvert.SerializeObject(usuario));
            TempData["GoodMessage"] = $"¡Bienvenido, {usuario.nombre}!";
            switch (usuario.rol.idRol)
            {
                case 1:
                    return RedirectToAction("Index", "Admin");
                case 2:
                    return RedirectToAction("Index", "Cliente");
                case 3:
                    return RedirectToAction("Index", "Vendedor");
                case 4:
                    return RedirectToAction("Index", "Repartidor");
                default:
                    TempData["ErrorMessage"] = "Rol no reconocido.";
                    return View();
            }
        }

        public async Task<IActionResult> RegisterAsync()
        {
            await CargarViewBags();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(Usuario model)
        {
            model.rol = new Rol { idRol = 2 };
            if (!ModelState.IsValid)
            {
                await CargarViewBags();
                TempData["ErrorMessage"] = "Datos inválidos.";
                return View("Register", model);
            }

            try
            {
                var nuevoUsuario = await userAuthService.registrarCliente(model);
                TempData["GoodMessage"] = $"¡Registro exitoso, {nuevoUsuario.nombre}!";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                await CargarViewBags();
                TempData["ErrorMessage"] = "Error al registrar: " + ex.Message;
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {

            //ELIMINAMOS LA COOKIE CREADA
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Session.Clear();
            TempData["GoodMessage"] = "Sesión cerrada exitosamente.";
            return RedirectToAction("Index", "Cliente");
        }

    }
}
