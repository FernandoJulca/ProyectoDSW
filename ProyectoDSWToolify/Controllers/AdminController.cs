using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;

namespace ProyectoDSWToolify.Controllers
{
    [Authorize(Roles = "A")]
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        public AdminController(IConfiguration inyect)
        {
            this._configuration = inyect;
        }

        #region Json

        private async Task<List<ListadoVentaFechaAndTipoVentaDTO>> ListadoVentaFechaAndTipoVenta(
            DateTime? fechaInicio, DateTime? fechaFin, string? tipo = null)
        {
            var listado = new List<ListadoVentaFechaAndTipoVentaDTO>();
            using (var httpCliente = new HttpClient())
            {
                httpCliente.BaseAddress = new Uri(_configuration["Services:URL_API"]);
                var Url = $"reportes/ListadoPorMesAndTipoVenta?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}";
                if (!string.IsNullOrEmpty(tipo)) { 
                    Url += $"&tipo={tipo}";
                }
                var msg = await httpCliente.GetAsync(Url);
                var data = await msg.Content.ReadAsStringAsync();

                listado = JsonConvert.DeserializeObject<List<ListadoVentaFechaAndTipoVentaDTO>>(data);
            }
            return listado;
        }

        private async Task<List<ListarProductosPorCategoriaDTO>> ListarProductosPorCategoria(
          int? idCategoria, string?orden )
        {
            var listado = new List<ListarProductosPorCategoriaDTO>();
            using (var httpCliente = new HttpClient())
            {
                httpCliente.BaseAddress = new Uri(_configuration["Services:URL_API"]);
                var Url = $"reportes/ListarProductosPorCategoria?idCategoria={idCategoria}";
                if (!string.IsNullOrEmpty(orden))
                {
                    Url += $"&orden={orden}";
                }
                var msg = await httpCliente.GetAsync(Url);
                var data = await msg.Content.ReadAsStringAsync();

                listado = JsonConvert.DeserializeObject<List<ListarProductosPorCategoriaDTO>>(data);
            }
            return listado;
        }
        private async Task<List<Categoria>> ListaCategoria()
        {
            var listaCategoria = new List<Categoria>();

            using (var httpCliente = new HttpClient())
            {
                httpCliente.BaseAddress = new Uri(_configuration["Services:URL_API"]);

                var msg = await httpCliente.GetAsync("Categoria");
                var data = await msg.Content.ReadAsStringAsync();

                listaCategoria = JsonConvert.DeserializeObject<List<Categoria>>(data);
            }
            return listaCategoria;
        }
        #endregion




        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Reporte1(DateTime? fechaInicio , DateTime? fechaFin, string? tipo ,int pag = 1) {


            var listado = ListadoVentaFechaAndTipoVenta(fechaInicio,fechaFin,tipo).Result;

            var FechaInicioParseado = fechaInicio?.ToString("yyyy-MM-dd");
            var FechaFinParseado = fechaFin?.ToString("yyyy-MM-dd");


            if (!listado.Any())
            {
                TempData["ErrorListado"] = ($"No hay Ventas para el rango de fecha {FechaInicioParseado}/{FechaFinParseado}");
                listado = ListadoVentaFechaAndTipoVenta(null, null, null).Result;
            }
            else {
                string mensaje = ($" desde {(fechaInicio.HasValue && fechaFin.HasValue ? $"{FechaInicioParseado}/{FechaFinParseado}":"")}");

                var mensajeAlert = ($"Filtrando ventas {mensaje} ");
                if (!string.IsNullOrEmpty(tipo)) {
                    mensajeAlert += $" tipo de venta {tipo}";
                }
                TempData["ExitoListado"] = mensajeAlert;
            }
            var paginasTotales = listado.Count;
            var paginasMax = 10;
            var numeroPag = (int)Math.Ceiling((double)paginasTotales / paginasMax);
            ViewBag.pagActual = pag;
            ViewBag.numeroPag = numeroPag;
            var skip = (pag - 1) * paginasMax;


            return View(listado.Skip(skip).Take(paginasMax));
        }

        public IActionResult Reporte2(int? idCategoria, string? orden, int pag = 1) 
        {
            var categorias = ListaCategoria().Result;
            ViewBag.Categorias = new SelectList(ListaCategoria().Result, "idCategoria", "descripcion", idCategoria);
            Categoria categoriObtenid = new Categoria();

            var nombreCategoria = categorias.FirstOrDefault(x => x.idCategoria == idCategoria)?.descripcion;
            var listadp = ListarProductosPorCategoria(idCategoria, orden).Result;

            if (!listadp.Any()) {
                TempData["ErrorListado"] = ($"No hay productos para la cat. {nombreCategoria}");
                listadp = ListarProductosPorCategoria(null, null).Result;
            }

            TempData["ExitoListado"] = ($"Mostrando {listadp.Count} productos Cat. {nombreCategoria}");
            
            var paginasTotales = listadp.Count;
            var paginasMax = 10;
            var numeroPag = (int)Math.Ceiling((double)paginasTotales / paginasMax);
            ViewBag.pagActual = pag;
            ViewBag.numeroPag = numeroPag;
            var skip = (pag - 1) * paginasMax;


            return View(listadp.Skip(skip).Take(paginasMax));
        }
    }
}
