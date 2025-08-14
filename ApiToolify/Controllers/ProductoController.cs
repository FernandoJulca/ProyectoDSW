using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Models;

namespace ApiToolify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {

        private readonly ICrud<Producto> dataProducto;

        public ProductoController(ICrud<Producto> inyecData)
        {
            this.dataProducto = inyecData;
        }

        [HttpGet]
        public IActionResult ListaCompleta() {

            return Ok(dataProducto.ListaCompleta());
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult ObtenerIdProducto(int id) {

            var prdObtenido = dataProducto.ObtenerId("detalle", id);
            if (prdObtenido == null)
            {
                return NotFound();  
            }

            return Ok(prdObtenido);
        }

        [HttpPost]
        public IActionResult RegistrarProducto([FromForm] Producto producto) {

            try 
            {
                if (producto.file != null && producto.file.Length > 0) {
                    //Guardamos la imagen en la carpeta wwwroot del proyecto WEB
                    string rutaProyecto = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName,
                        "ProyectoDSWToolify", // ->nombre del proyecto WEB
                        "wwwroot", //carpeta donde se va a crear una nueva carpeta
                        "uploads"
                        );

                    if (!Directory.Exists(rutaProyecto)) //creamos la capeta si es no existe 
                    Directory.CreateDirectory(rutaProyecto);

                    string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(producto.file.FileName); // nombre para la imagen

                    string rutaArchivo = Path.Combine(rutaProyecto, nombreArchivo); // ruta donde se guardara la imagen
                   
                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        producto.file.CopyTo(stream);
                    }

                    //guardar la propieda en la Base De Datos (usaremos esto en en el cshtml para mostrar)
                    producto.imagen = "/uploads/" + nombreArchivo;
                }
                //registramos el producto
                var prdRegistrado = dataProducto.Registrar("registrar", producto);
                return Ok(prdRegistrado);
            } 
            catch (Exception ex) 
            {
                return BadRequest("error en subir / guardar la imagen" + ex);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult ActualizarProducto([FromForm] Producto producto) {

          

            if (producto.file != null && producto.file.Length > 0)
            {
                string rutaProyecto = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName,
                    "ProyectoDSWToolify",
                    "wwwroot",
                    "uploads"
                );

                if (!Directory.Exists(rutaProyecto))
                    Directory.CreateDirectory(rutaProyecto);

                string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(producto.file.FileName);
                string rutaArchivo = Path.Combine(rutaProyecto, nombreArchivo);

                using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                {
                    producto.file.CopyTo(stream);
                }

                // Guardamos la ruta relativa para la BD
                producto.imagen = "/uploads/" + nombreArchivo;
            }
            var prdActualizado = dataProducto.Actualizar("actualizar", producto);
            var resultado = new
            {
                prdActualizado.nombre,
                prdActualizado.descripcion,
                prdActualizado.proveedor.idProveedor,
                prdActualizado.categoria.idCategoria,
                prdActualizado.precio,
                prdActualizado.stock,
                prdActualizado.imagen,
                prdActualizado.fechaRegistro

            };

            return Ok(resultado);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DesactivarProducto(int id) {

            var prdDesactivado = dataProducto.Eliminar("desactivar", id);

            return Ok(prdDesactivado);
        }




    }
}
