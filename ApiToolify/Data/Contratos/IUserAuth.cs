using ProyectoDSWToolify.Models;

namespace ApiToolify.Data.Contratos
{
    public interface IUserAuth
    {
        Usuario iniciarSession(string correo, string clave);
        Usuario registrarCliente(Usuario u);
    }
}
