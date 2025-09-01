using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Controllers
{

    [Authorize(Roles = "A")] //SOLO ADMINISTRADOR
    public class ProveedorController : Controller
    {
        private readonly IConfiguration _config;

        public ProveedorController(IConfiguration configuration)
        {
            this._config = configuration;
        }



        #region  metodos Json

        /*TRAEMOS LA LISTA DE DISTRITOS DE LA API*/
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

        private async Task<List<Proveedor>> obtenerListadoProveedor()
        {

            var listaProveedor = new List<Proveedor>();
            using (var ClienteHttp = new HttpClient())
            {
                ClienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var msg = await ClienteHttp.GetAsync("Proveedor");
                var data = await msg.Content.ReadAsStringAsync();

                listaProveedor = JsonConvert.DeserializeObject<List<Proveedor>>(data);
            }
            return listaProveedor;
        }

        private async Task<Proveedor> ObtenerIdProveedor(int id)
        {

            var proveedor = new Proveedor();
            using (var ClienteHttp = new HttpClient())
            {
                ClienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var msg = await ClienteHttp.GetAsync($"proveedor/{id}");
                var data = await msg.Content.ReadAsStringAsync();

                proveedor = JsonConvert.DeserializeObject<Proveedor>(data);
            }
            return proveedor;
        }


        private async Task<Proveedor> RegistrarProveedor(Proveedor proveedor)
        {
            Proveedor provedorGuardado = null;
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                StringContent contenidoJson = new StringContent(JsonConvert.SerializeObject(proveedor),
                    System.Text.Encoding.UTF8, "application/json");

                var msg = await clienteHttp.PostAsync("proveedor", contenidoJson);
                var data = await msg.Content.ReadAsStringAsync();


                provedorGuardado = JsonConvert.DeserializeObject<Proveedor>(data);
            }


            return provedorGuardado;
        }



        private async Task<Proveedor> actualizarProveedor(Proveedor proveedor)
        {
            Proveedor provedorGuardado = null;
            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);
                StringContent contenidoJson = new StringContent(JsonConvert.SerializeObject(proveedor),
                    System.Text.Encoding.UTF8, "application/json");

                var msg = await clienteHttp.PutAsync($"proveedor/{proveedor.idProveedor}", contenidoJson);
                var data = await msg.Content.ReadAsStringAsync();

                provedorGuardado = JsonConvert.DeserializeObject<Proveedor>(data);
            }


            return provedorGuardado;
        }

        private async Task<int> desactivarProveedor(int id)
        {
            Proveedor proveedorDesactivado = null;

            using (var clienteHttp = new HttpClient())
            {
                clienteHttp.BaseAddress = new Uri(_config["Services:URL_API"]);

                var msg = await clienteHttp.DeleteAsync($"proveedor/{id}");
                var data = await msg.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<int>(data);
            }

        }


        #endregion


        #region Metodos Vista

        [HttpGet]
        public IActionResult Index()
        {
            var listado = obtenerListadoProveedor().Result;

            TempData["ExitoListado"] = ($"Se obtuvo {listado.Count} Proveedores");

            return View(listado);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Distritos = new SelectList(obtenerListaDistritos().Result, "idDistrito", "nombre");
            return View(new Proveedor());
        }

        [HttpPost]
        public IActionResult Create(Proveedor proveedor)
        {
            Proveedor proveGuardado = RegistrarProveedor(proveedor).Result;

            TempData["ExitoCreate"] = ($"Se obtuvo registro a {proveedor.razonSocial} codigo:{proveedor.idProveedor}");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Actualizar(int id = 0)
        {
            ViewBag.Distritos = new SelectList(obtenerListaDistritos().Result, "idDistrito", "nombre");

            var proveedorEncontrado = ObtenerIdProveedor(id).Result;
            return View(proveedorEncontrado);
        }

        [HttpPost]
        public IActionResult Actualizar(Proveedor proveedor)
        {

            proveedor.distrito = new Distrito { idDistrito = proveedor.distrito.idDistrito };
            var proveedorGuardado = actualizarProveedor(proveedor).Result;
            TempData["ExitoActualizar"] = ($"Se actualizo a {proveedor.razonSocial} codigo:{proveedor.idProveedor}");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id) { 
        
                return View(ObtenerIdProveedor(id).Result);
        }


        [HttpGet]
        public IActionResult Desactivar(int id)
        {
            var proveedorEncontrado = ObtenerIdProveedor(id).Result;
            return View(proveedorEncontrado);
        }

        [HttpPost]
        [ActionName(name: "Desactivar")]

        public IActionResult Desactivar_Confirmar(int id)
        {
            Proveedor prvEncontrado = ObtenerIdProveedor(id).Result;

            TempData["ExitoDesactivar"] = ($"Se desactivo a {prvEncontrado.razonSocial} codigo:{prvEncontrado.idProveedor} ");
            var proveedorDescativado = desactivarProveedor(id).Result;
            return RedirectToAction("Index");
        }


        #endregion

    }
}
