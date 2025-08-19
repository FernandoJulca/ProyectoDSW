using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Data.Contratos
{
    public interface IUsuario
    {
        List<Usuario> listCliente();
        int contadorClientes();
    }
}
