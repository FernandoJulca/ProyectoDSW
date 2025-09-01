using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using ProyectoDSWToolify.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProyectoDSWToolify.Controllers
{
    [Authorize(Roles = "A")]
    public class ProductoController : Controller
    {

        private readonly IConfiguration _configuration;

        public ProductoController(IConfiguration config)
        {
            this._configuration = config;
        }

        #region Metodos Json

        private async Task<List<Producto>> ListaCompleta()
        {
            var listaProducto = new List<Producto>();

            using (var httpCliente = new HttpClient())
            {

                httpCliente.BaseAddress = new Uri(_configuration["Services:URL_API"]);

                var msg = await httpCliente.GetAsync("Producto");
                var data = await msg.Content.ReadAsStringAsync();

                listaProducto = JsonConvert.DeserializeObject<List<Producto>>(data);
            }

            return listaProducto;
        }

        //LISTA CATEGORIA-DISTRITO
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
        private async Task<List<Proveedor>> ListaProveedor()
        {

            var listaProveedor = new List<Proveedor>();
            using (var ClienteHttp = new HttpClient())
            {
                ClienteHttp.BaseAddress = new Uri(_configuration["Services:URL_API"]);

                var msg = await ClienteHttp.GetAsync("Proveedor");
                var data = await msg.Content.ReadAsStringAsync();

                listaProveedor = JsonConvert.DeserializeObject<List<Proveedor>>(data);
            }
            return listaProveedor;
        }
        private async Task<Producto> ObtenerIdProducto(int id)
        {
            var producto = new Producto();
            using (var httpCliente = new HttpClient())
            {
                httpCliente.BaseAddress = new Uri(_configuration["Services:URL_API"]);

                var msg = await httpCliente.GetAsync($"producto/{id}");
                var data = await msg.Content.ReadAsStringAsync();

                producto = JsonConvert.DeserializeObject<Producto>(data);
            }
            return producto;
        }
        private async Task<Producto> RegistrarProducto(Producto producto)
        {
            Producto prdRegistrado = null;

            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_configuration["Services:URL_API"]);

                    using (var form = new MultipartFormDataContent())/*Creamos un contenido multipart/Form-data para q registre la imagen*/
                    {
                        //agregamos los campos a registrar 
                        form.Add(new StringContent(producto.nombre), "nombre");
                        form.Add(new StringContent(producto.descripcion), "descripcion");
                        form.Add(new StringContent(producto.proveedor.idProveedor.ToString()), "proveedor.idProveedor");
                        form.Add(new StringContent(producto.categoria.idCategoria.ToString()), "categoria.idCategoria");
                        form.Add(new StringContent(producto.precio.ToString()), "precio");
                        form.Add(new StringContent(producto.stock.ToString()), "stock");
                        form.Add(new StringContent(producto.fechaRegistro.ToString("yyyy-MM-dd HH:mm:ss")), "fechaRegistro");

                        //si hemos subido una imagen se realiza el sgnt metodo
                        if (producto.file != null)
                        {
                            var stream = producto.file.OpenReadStream(); //Lee el contido de la imagen

                            var fileContent = new StreamContent(stream); //lo convierte en Http

                            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(producto.file.ContentType);

                            form.Add(fileContent, "file", producto.file.FileName); //lo agregamos al contenido con su nombre
                        }
                        else //caso contratio no se sube nada y se registra sin imagen
                        {
                            System.Diagnostics.Debug.WriteLine("No se recibió ningúna imagen");
                        }


                        var msg = await httpCliente.PostAsync("producto", form);

                        var data = await msg.Content.ReadAsStringAsync();

                        prdRegistrado = JsonConvert.DeserializeObject<Producto>(data);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en RegistrarProducto: " + ex.Message);
            }
            return prdRegistrado;
        }
        private async Task<Producto> ActualizarProducto(Producto producto)
        {
            Producto prdActualizado = null;
            try
            {
                using (var httpCliente = new HttpClient())
                {
                    httpCliente.BaseAddress = new Uri(_configuration["Services:URL_API"]);

                    using (var formData = new MultipartFormDataContent())
                    {

                        formData.Add(new StringContent(producto.idProducto.ToString()), "idProducto");
                        formData.Add(new StringContent(producto.nombre), "nombre");
                        formData.Add(new StringContent(producto.descripcion), "descripcion");
                        formData.Add(new StringContent(producto.proveedor.idProveedor.ToString()), "proveedor.idProveedor");
                        formData.Add(new StringContent(producto.categoria.idCategoria.ToString()), "categoria.idCategoria");
                        formData.Add(new StringContent(producto.precio.ToString()), "precio");
                        formData.Add(new StringContent(producto.stock.ToString()), "stock");
                        formData.Add(new StringContent(producto.fechaRegistro.ToString("yyyy-MM-dd HH:mm:ss")), "fechaRegistro");


                        //actualizar solo si estamos pasando una imagen, si no se envia nada la 
                        //imagen seguira siendo la misma 
                        if (producto.file != null && producto.file.Length > 0)
                        {
                            var fileContent = new StreamContent(producto.file.OpenReadStream());
                            fileContent.Headers.ContentType = new MediaTypeHeaderValue(producto.file.ContentType);
                            formData.Add(fileContent, "file", producto.file.FileName);
                        }
                        else if (!string.IsNullOrEmpty(producto.imagen))
                        {
                            formData.Add(new StringContent(producto.imagen), "imagen");
                        }

                        var msg = await httpCliente.PutAsync($"producto/{producto.idProducto}", formData);

                        var data = await msg.Content.ReadAsStringAsync();


                        prdActualizado = JsonConvert.DeserializeObject<Producto>(data);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en Actualizar Producto: " + ex.Message);
            }
            return prdActualizado;
        }
        private async Task<int> DesactivarProducto(int id)
        {
            Producto producto = null;

            using (var httpCliente = new HttpClient())
            {

                httpCliente.BaseAddress = new Uri(_configuration["Services:URL_API"]);

                var msg = await httpCliente.DeleteAsync($"producto/{id}");

                var data = await msg.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<int>(data);
            }
        }

        #endregion

        #region Metodos Vistas

        [HttpGet]
        public IActionResult Index(int pag = 1, int proveedor = 0, int categoria = 0)
        {
            var listado = ListaCompleta().Result;
            var nombreProveedor = "";
            var nombreCategoria = "";

            //Listado de proveedores y categorias
            ViewBag.Proveedores = new SelectList(ListaProveedor().Result, "idProveedor", "razonSocial", proveedor);
            ViewBag.Categorias = new SelectList(ListaCategoria().Result, "idCategoria", "descripcion", categoria);


            if (proveedor > 0)
            {
                listado = listado.Where(x => x.proveedor.idProveedor == proveedor).ToList();
                nombreProveedor = listado.FirstOrDefault(x => x.proveedor.idProveedor == proveedor)?.proveedor.razonSocial;

            }
            if (categoria > 0)
            {
                listado = listado.Where(x => x.categoria.idCategoria == categoria).ToList();
                nombreCategoria = listado.FirstOrDefault(x => x.categoria.idCategoria == categoria)?.categoria.descripcion;
            }

            if (proveedor > 0 || categoria > 0)
            {

                var mensaje = "Se filtro por ";

                if (proveedor != 0)
                {
                    mensaje += nombreProveedor;
                }
                if (categoria != 0)
                {
                    if (proveedor != 0)

                        mensaje += ",";
                    mensaje += nombreCategoria;
                }
                if (listado.Count != 0)
                {
                    TempData["ExitoFiltros"] = mensaje;
                }   
                else
                {
                    listado = ListaCompleta().Result;
                    TempData["FalloFiltros"] = ($"No hay Productos para esos Filtros");
                }
                /*   
                   TempData["ExitoFiltros"] = (
                   $@"Se filtro por 
                   {(proveedor != 0 ? nombreProveedor : ", ")} 
                   {(categoria != 0 ? nombreCategoria : null)}"
                   );
                */
            }

            TempData["ExitoListado"] = ($"Se obtuvo {listado.Count} productos");

            var paginasTotales = listado.Count;
            var paginasMax = 9;
            var numeroPag = (int)Math.Ceiling((double)paginasTotales / paginasMax);
            ViewBag.pagActual = pag;
            ViewBag.numeroPag = numeroPag;
            var skip = (pag - 1) * paginasMax;


            return View(listado.Skip(skip).Take(paginasMax));
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categorias = new SelectList(ListaCategoria().Result, "idCategoria", "descripcion");
            ViewBag.Proveedores = new SelectList(ListaProveedor().Result, "idProveedor", "razonSocial");
            return View(new Producto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Producto producto)
        {
            var proveedorRegistrado = await RegistrarProducto(producto);
                TempData["ExitoCreate"] = ($"Se Registro el producto {proveedorRegistrado.descripcion} codigo:{proveedorRegistrado.idProducto}");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Producto prdEncontrado = ObtenerIdProducto(id).Result;
            ViewBag.Categorias = new SelectList(ListaCategoria().Result, "idCategoria", "descripcion", prdEncontrado.categoria.idCategoria);
            ViewBag.Proveedores = new SelectList(ListaProveedor().Result, "idProveedor", "razonSocial", prdEncontrado.proveedor.idProveedor);
            return View(prdEncontrado);
        }

        [HttpPost]
        public IActionResult Edit(Producto producto)
        {

            var prdActualizado = ActualizarProducto(producto).Result;
            TempData["ExitoActualizar"] = ($"Se Actualizo el producto {producto.descripcion} codigo:{producto.idProducto}");

            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var prdEncontrado = ObtenerIdProducto(id).Result;

            return View(prdEncontrado);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var prdEnocntrado = ObtenerIdProducto(id).Result;

            return View(prdEnocntrado);
        }

        [HttpPost]
        [ActionName(name: "Delete")]
        public ActionResult Delete_Confirm(int id)
        {
            var prdEliminado = DesactivarProducto(id).Result;
            Producto prdEncotnrad = ObtenerIdProducto(id).Result; //lo usamos para obtener info del producto
            TempData["ExitoDesactivar"] = ($"Se Desactivo el producto {prdEncotnrad.descripcion} codigo:{prdEncotnrad.idProducto}");
            return RedirectToAction("Index");
        }

        #endregion


    }
}
