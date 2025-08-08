using Microsoft.Data.SqlClient;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Data.Repositorios
{
    public class DistritoRepo : ICrud<Distrito>
    {
        private readonly IConfiguration config;
        private readonly string cadenaConexion;

        public DistritoRepo(IConfiguration configuration)
        {
            config = configuration;
            cadenaConexion = config["ConnectionStrings:DB"];
        }

        public List<Distrito> ListaCompleta()
        {
           List<Distrito> listaDistrito = new List<Distrito>();
            using (SqlConnection cn = new SqlConnection(cadenaConexion)) { 
                cn.Open();
                using (SqlCommand cm = new SqlCommand("SELECT * FROM TB_DISTRITO", cn)) {
                  

                    using (SqlDataReader r = cm.ExecuteReader()) { 
                        if( r != null && r.HasRows) {
                            while (r.Read())
                            {
                                listaDistrito.Add(new Distrito()
                                {
                                    idDistrito = r.IsDBNull(0) ? 0 : r.GetInt32(0),
                                    nombre = r.IsDBNull(1) ? "" : r.GetString(1)
                                });       
                            }
                        }
                    }
                }
            }
                return listaDistrito;
        }
        public bool Actualizar(string tipo, Distrito entidad)
        {
            throw new NotImplementedException();
        }
        public bool Eliminar(string tipo, int id)
        {
            throw new NotImplementedException();
        }
        public Distrito ObtenerId(string tipo, int id)
        {
            throw new NotImplementedException();
        }
        public int Registrar(string tipo, Distrito entidad)
        {
            throw new NotImplementedException();
        }
    }
}
