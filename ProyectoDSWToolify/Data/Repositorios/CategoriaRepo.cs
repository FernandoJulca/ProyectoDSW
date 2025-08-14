using Microsoft.Data.SqlClient;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;

namespace ProyectoDSWToolify.Data.Repositorios
{
    public class CategoriaRepo : ICategoria
    {
        private readonly IConfiguration config;
        private readonly string cadenaConexion;

        public CategoriaRepo(IConfiguration configuration)
        {

            config = configuration;
            cadenaConexion = config["ConnectionStrings:DB"];
        }
        public List<Categoria> listCategoriasCliente()
        {
            var list = new List<Categoria>();
            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var cmd = new SqlCommand("listarCategorias", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                list.Add(new Categoria()
                                {
                                    idCategoria = rdr.GetInt32(0),
                                    descripcion = rdr.GetString(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                list = new List<Categoria>();
            }
            return list;
        }

        public List<CategoriaVendidaViewModel> top4CategoriasMasVendidas()
        {
            var list = new List<CategoriaVendidaViewModel>();
            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var cmd = new SqlCommand("top4CategoriasMasVendidas", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new CategoriaVendidaViewModel()
                                {
                                    IdCategoria = reader.GetInt32(0),
                                    Categoria = reader.GetString(1),
                                    CantidadVendida = reader.GetInt32(2),
                                    CantidadProductos = reader.GetInt32(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                list = new List<CategoriaVendidaViewModel>();
            }
            return list;
        }
    }
}
