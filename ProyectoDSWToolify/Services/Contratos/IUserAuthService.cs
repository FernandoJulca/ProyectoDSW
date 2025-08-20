using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;

namespace ProyectoDSWToolify.Services.Contratos
{
    public interface IUserAuthService
    {
        Task<Usuario> iniciarSession(LoginResponse responselogin);
        Task<Usuario> registrarCliente(Usuario u);
    }
}
