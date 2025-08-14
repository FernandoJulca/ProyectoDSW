using Microsoft.AspNetCore.Mvc;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Models;

namespace ApiToolify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : Controller
    {

        private readonly ICrud<Proveedor> proveData;

        public ProveedorController(ICrud<Proveedor> inyecProve)
        {
            this.proveData = inyecProve;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(proveData.ListaCompleta());
        }


        [HttpGet]
        [Route("{id}")]
        public IActionResult ObtenerIdProveedor(int id ) {

            var clienteEncontrado = proveData.ObtenerId("detalle",id);
            if (clienteEncontrado == null)
            {
                return BadRequest();
            }
            return Ok(clienteEncontrado);

        }

        [HttpPost]
        public IActionResult registrarProveedor(Proveedor proveedor) {
            var provedorGuardado = proveData.Registrar("registrar", proveedor);
            return Ok(provedorGuardado);
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult actualizarProveedor(Proveedor proveedor) {
            var actualizado = proveData.Actualizar("actualizar", proveedor);

            return Ok(actualizado); 
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Desactivar(int id)
        {
            var proveedorDesactivado = proveData.Eliminar("desactivar", id);
            return Ok(proveedorDesactivado);    
        }

    }
}
