using ApiToolify.Models.DTO;
using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Data.Contratos
{
    public interface ICategoria
    {
        List<Categoria> listCategoriasCliente();
        List<CategoriaDTO> top4CategoriasMasVendidas();
    }
}
