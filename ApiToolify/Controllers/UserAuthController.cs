using ApiToolify.Data.Contratos;
using ApiToolify.Data.Repositorios;
using ApiToolify.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ProyectoDSWToolify.Models;

namespace ApiToolify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly IUserAuth userAuth;

        public UserAuthController(IUserAuth userAuth)
        {
            this.userAuth = userAuth;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginResponse request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Clave))
            {
                return BadRequest("Correo y clave son requeridos.");
            }

            var usuario = userAuth.iniciarSession(request.Correo, request.Clave);

            if (usuario == null)
            {
                return Unauthorized("Credenciales incorrectas.");
            }

            return Ok(usuario);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] Usuario user)
        {
            if (user == null)
                return BadRequest(new { message = "Datos de usuario inválidos." });

            if (string.IsNullOrWhiteSpace(user.correo) || string.IsNullOrWhiteSpace(user.clave))
                return BadRequest(new { message = "Correo y clave son obligatorios." });

            try
            {
                var nuevoUsuario = userAuth.registrarCliente(user);

                if (nuevoUsuario == null)
                    return StatusCode(500, new { message = "Error al registrar usuario." });

                nuevoUsuario.clave = null;

                return Ok(new
                {
                    message = "Usuario registrado correctamente.",
                    usuario = nuevoUsuario
                });
            }
            catch (Exception ex)
            {
                // Devuelve BadRequest con el mensaje amigable de la excepción
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
