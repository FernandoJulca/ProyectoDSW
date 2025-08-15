using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Models;

namespace ApiToolify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICrud<Categoria> dataCategoria;

        public CategoriaController(ICrud<Categoria> inyecData)
        {
            this.dataCategoria = inyecData;
        }

        [HttpGet]
        public IActionResult ListaCategoria() {
            return Ok(dataCategoria.ListaCompleta());
        }
    }
}
