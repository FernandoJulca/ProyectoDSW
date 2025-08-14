using Microsoft.Data.SqlClient;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Data.Repositorios
{
    public class CateRepo : ICrud<Categoria>
    {
        private readonly IConfiguration config;
        private readonly string cadenaConexion;

        public CateRepo(IConfiguration configuration)
        {
            config = configuration;
            cadenaConexion = config["ConnectionStrings:DB"];
        }

        public Categoria Actualizar(string tipo, Categoria entidad)
        {
            throw new NotImplementedException();
        }

        public int Eliminar(string tipo, int id)
        {
            throw new NotImplementedException();
        }

        public List<Categoria> ListaCompleta()
        {
            List<Categoria> listadoCategoria = new List<Categoria>();
            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand("SELECT * FROM TB_CATEGORIA", cn))
                {


                    using (SqlDataReader r = cm.ExecuteReader())
                    {
                        if (r != null && r.HasRows)
                        {
                            while (r.Read())
                            {
                                listadoCategoria.Add(new Categoria()
                                {
                                    idCategoria = r.IsDBNull(0) ? 0 : r.GetInt32(0),
                                    descripcion = r.IsDBNull(1) ? "" : r.GetString(1)
                                });
                            }
                        }
                    }
                }
            }
            return listadoCategoria;
        }

        public Categoria ObtenerId(string tipo, int id)
        {
            throw new NotImplementedException();
        }

        public Categoria Registrar(string tipo, Categoria entidad)
        {
            throw new NotImplementedException();
        }
    }
}
