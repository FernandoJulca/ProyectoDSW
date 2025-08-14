using Microsoft.AspNetCore.Mvc;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Models;


namespace ApiToolify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistritoController : ControllerBase
    {

        private readonly ICrud<Distrito> distritoData;

        public DistritoController(ICrud<Distrito> inyec)
        {
            this.distritoData = inyec;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return Ok(distritoData.ListaCompleta());
        }

    

    }
}
