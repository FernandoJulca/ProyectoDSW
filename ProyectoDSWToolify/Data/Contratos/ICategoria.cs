using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;

namespace ProyectoDSWToolify.Data.Contratos
{
    public interface ICategoria
    {
        List<Categoria> listCategoriasCliente();
        List<CategoriaVendidaViewModel> top4CategoriasMasVendidas();
    }
}
