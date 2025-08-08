using Microsoft.Data.SqlClient;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Data.Repositorios
{
    public class ProveRepo : ICrud<Proveedor>
    {
        private readonly IConfiguration config;
        private readonly string cadenaConexion;

        public ProveRepo(IConfiguration configuration)
        {

            config = configuration;
            cadenaConexion = config["ConnectionStrings:DB"];
        }


        public List<Proveedor> ListaCompleta()
        {
            List<Proveedor> listaProveedor = new List<Proveedor>();
            using (SqlConnection con = new SqlConnection(cadenaConexion))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("listarProveedores", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r != null && r.HasRows)
                        {
                            while (r.Read())
                            {
                                listaProveedor.Add(new Proveedor()
                                {
                                    idProveedor = r.IsDBNull(0) ? 0 : r.GetInt32(0),
                                    ruc = r.IsDBNull(1) ? "" : r.GetString(1),
                                    razonSocial = r.IsDBNull(2) ? "" : r.GetString(2),
                                    telefono = r.IsDBNull(3) ? "" : r.GetString(3),
                                    direccion = r.IsDBNull(4) ? "" : r.GetString(4),
                                    distrito = new Distrito() { 
                                        nombre = r.IsDBNull(5) ? "" : r.GetString(5),
                                    },
                                   fechaRegistro = r.IsDBNull(6) ? new DateTime() : r.GetDateTime(6),
                                   estado = r.IsDBNull(7) ? true : r.GetBoolean(7),
                                });
                            }
                        }
                    }
                }
            }

            return listaProveedor;
        }

        public int Registrar(string tipo, Proveedor proveedor)
        {
            int idCreado = 0;
            using (SqlConnection cn = new SqlConnection(cadenaConexion)) {
                cn.Open();
                using (SqlCommand cm = new SqlCommand("crudProveedores", cn)) {
                    cm.CommandType = System.Data.CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@tipo",tipo);
                    cm.Parameters.AddWithValue("@ruc", proveedor.ruc);
                    cm.Parameters.AddWithValue("@razon", proveedor.razonSocial);
                    cm.Parameters.AddWithValue("@telefono", proveedor.telefono);
                    cm.Parameters.AddWithValue("@direccion", proveedor.direccion);
                    cm.Parameters.AddWithValue("@idDistrito", proveedor.distrito.idDistrito);
                    cm.Parameters.AddWithValue("@fecha", proveedor.fechaRegistro);

                    idCreado = Convert.ToInt32(cm.ExecuteScalar());
                }
            }
            return idCreado;
        }
        public bool Actualizar(string tipo, Proveedor proveedor)
        {
            bool actualizado = false;
            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand("crudProveedores", cn))
                {
                    cm.CommandType = System.Data.CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@tipo", tipo);
                    cm.Parameters.AddWithValue("@idProveedor", proveedor.idProveedor);
                    cm.Parameters.AddWithValue("@ruc", proveedor.ruc);
                    cm.Parameters.AddWithValue("@razon", proveedor.razonSocial);
                    cm.Parameters.AddWithValue("@telefono", proveedor.telefono);
                    cm.Parameters.AddWithValue("@direccion", proveedor.direccion);
                    cm.Parameters.AddWithValue("@idDistrito", proveedor.distrito.idDistrito);
                    cm.Parameters.AddWithValue("@fecha", proveedor.fechaRegistro);

                    int filasAfectadas = cm.ExecuteNonQuery();
                    actualizado = filasAfectadas > 0;
                }
            }
            return actualizado;
        }

        public bool Eliminar(string tipo, int id)
        {
            bool eliminado = false;

            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand("crudProveedores", cn))
                {
                    cm.CommandType = System.Data.CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@tipo", tipo);
                    cm.Parameters.AddWithValue("@idProveedor", id);

                    int filasAfectadas = cm.ExecuteNonQuery();
                    eliminado = filasAfectadas > 0;
                }
            }
            return eliminado;
        }

        public Proveedor ObtenerId(string tipo, int id)
        {
            Proveedor provEncotrado = null;

            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand("crudProveedores", cn))
                {
                    cm.CommandType = System.Data.CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@tipo", tipo);
                    cm.Parameters.AddWithValue("@idProveedor", id);

                    using (SqlDataReader r = cm.ExecuteReader()) {
                        if (r != null && r.HasRows) {
                            while (r.Read()) { 
                               provEncotrado = new Proveedor() 
                               { 
                                idProveedor = r.GetInt32(0),
                                ruc = r.GetString(1),
                                razonSocial = r.GetString(2),
                                telefono = r.GetString(3),
                                direccion = r.GetString(4),
                                distrito = new Distrito() { 
                                    idDistrito = r.GetInt32(5),
                                },
                                fechaRegistro = r.GetDateTime(6),
                                estado = r.GetBoolean(7)
                               };
                            }
                        }
                    }
                }
            }
            return provEncotrado;                
        }


    }
}
