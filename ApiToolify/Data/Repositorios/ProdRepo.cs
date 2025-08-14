using Microsoft.Data.SqlClient;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Data.Repositorios
{
    public class ProdRepo : ICrud<Producto>
    {
        private readonly IConfiguration config;
        private readonly string cadenaConexion;

        public ProdRepo(IConfiguration configuration)
        {

            config = configuration;
            cadenaConexion = config["ConnectionStrings:DB"];
        }

        public Producto Actualizar(string tipo, Producto producto)
        {
           Producto prdActualizado = new Producto();
            using (SqlConnection cn = new SqlConnection(cadenaConexion)) { 
                cn.Open();
                using (SqlCommand cm = new SqlCommand("crudProductos", cn)) { 
                    cm.CommandType = System.Data.CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@tipo", tipo);
                    cm.Parameters.AddWithValue("@idProducto", producto.idProducto);
                    cm.Parameters.AddWithValue("@nombre", producto.nombre);
                    cm.Parameters.AddWithValue("@descripcion", producto.descripcion);
                    cm.Parameters.AddWithValue("@idProveedor", producto.proveedor.idProveedor);
                    cm.Parameters.AddWithValue("@idCategoria", producto.categoria.idCategoria);
                    cm.Parameters.AddWithValue("@precio", producto.precio);
                    cm.Parameters.AddWithValue("@stock", producto.stock);
                    cm.Parameters.AddWithValue("@imagen", producto.imagen);
                    cm.Parameters.AddWithValue("@fecha", producto.fechaRegistro);

                    int filasAfectadas = cm.ExecuteNonQuery();
                    
                }
            }
            return prdActualizado = producto;
        }

        public int Eliminar(string tipo, int id)
        {
            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand("crudProductos", cn))
                {
                    cm.CommandType = System.Data.CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@tipo", tipo);
                    cm.Parameters.AddWithValue("@idProducto", id);

                    int filasAfectadas = cm.ExecuteNonQuery();
                    return filasAfectadas;
                }
            }
        }

        public List<Producto> ListaCompleta()
        {
            List<Producto> listaProductos = new List<Producto>();
            using (SqlConnection con = new SqlConnection(cadenaConexion))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("listarProductos", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r != null && r.HasRows)
                        {
                            while (r.Read())
                            {
                                
                                listaProductos.Add(new Producto()
                                {
                                    idProducto = r.IsDBNull(0) ? 0 : r.GetInt32(0),

                                    nombre = r.IsDBNull(1) ? "" : r.GetString(1),
                                    descripcion = r.IsDBNull(2) ? "" : r.GetString(2),
                                    proveedor = new Proveedor()
                                    {
                                        razonSocial = r.IsDBNull(3) ? "" : r.GetString(3),
                                    },
                                    categoria = new Categoria()
                                    {
                                        descripcion = r.IsDBNull(4) ? "" : r.GetString(4),
                                    },
                                    precio = r.IsDBNull(5) ? 0 : r.GetDecimal(5),
                                    stock = r.IsDBNull(6) ? 0 : r.GetInt32(6),
                                });
                                
                            }
                           
                        }
                    }
                }
            }
           
            return listaProductos;
        }

       /* public Producto ObtenerId(string tipo, int id)
        {
            Producto prodEncontrado = new Producto();

            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand("crudProductos", cn))
                {
                    cm.CommandType = System.Data.CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@tipo", tipo);
                    cm.Parameters.AddWithValue("@idProducto", id);

                    using (SqlDataReader r = cm.ExecuteReader())
                    {
                        if (r != null && r.HasRows)
                        {
                            while (r.Read())
                            {
                                prodEncontrado = new Producto()
                                {
                                    idProducto = r.IsDBNull(0) ? 0 : r.GetInt32(0),

                                    nombre = r.IsDBNull(1) ? "" : r.GetString(1),
                                    descripcion = r.IsDBNull(2) ? "" : r.GetString(2),
                                    proveedor = new Proveedor()
                                    {
                                        razonSocial = r.IsDBNull(3) ? "" : r.GetString(3),
                                    },
                                    categoria = new Categoria()
                                    {
                                        descripcion = r.IsDBNull(4) ? "" : r.GetString(4),
                                    },
                                    precio = r.IsDBNull(5) ? 0 : r.GetDecimal(5),
                                    stock = r.IsDBNull(6) ? 0 : r.GetInt32(6),
                                    imagen = r.IsDBNull(7) ? "" : r.GetString(7),
                                    fechaRegistro = r.IsDBNull(8) ? new DateTime() : r.GetDateTime(8),
                                    estado = r.IsDBNull(9) ? true : r.GetBoolean(9),
                                };
                            }
                        }
                    }
                }
            }
            return prodEncontrado;
        }
       */
        public Producto ObtenerId(string tipo, int id)
        {
             Producto prodEncontrado = new Producto();

            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand("crudProductos", cn))
                {
                    cm.CommandType = System.Data.CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@tipo", tipo);
                    cm.Parameters.AddWithValue("@idProducto", id);

                    using (SqlDataReader r = cm.ExecuteReader()) {
                        if (r != null && r.HasRows) {
                            while (r.Read()) {
                                prodEncontrado = new Producto()
                                {
                                    idProducto = r.IsDBNull(0) ? 0 : r.GetInt32(0),

                                    nombre = r.IsDBNull(1) ? "" : r.GetString(1),
                                    descripcion = r.IsDBNull(2) ? "" : r.GetString(2),
                                    proveedor = new Proveedor()
                                    {
                                        idProveedor = r.IsDBNull(3) ? 0 : r.GetInt32(3),
                                        razonSocial = r.IsDBNull(4) ? "" : r.GetString(4),
                                    },
                                    categoria = new Categoria()
                                    {
                                        idCategoria = r.IsDBNull (5) ? 0 : r.GetInt32(5),
                                        descripcion = r.IsDBNull(6) ? "" : r.GetString(6),
                                    },
                                    precio = r.IsDBNull(7) ? 0 : r.GetDecimal(7),
                                    stock = r.IsDBNull(8) ? 0 : r.GetInt32(8),
                                    imagen = r.IsDBNull(9) ? "" : r.GetString(9),
                                    fechaRegistro = r.IsDBNull(10) ? new DateTime() : r.GetDateTime(10),
                                    estado = r.IsDBNull(11) ? true : r.GetBoolean(11)
                                };
                            }
                        }
                    }
                }
            }
            return prodEncontrado;  
        }

        public Producto Registrar(string tipo, Producto producto)
        {
            Producto prodGuardodo = new Producto(); 
            int idRegistrado;
            using (SqlConnection cn = new SqlConnection(cadenaConexion))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand("crudProductos", cn))
                {
                    cm.CommandType = System.Data.CommandType.StoredProcedure;
                    cm.Parameters.AddWithValue("@tipo", tipo);
                    cm.Parameters.AddWithValue("@nombre", producto.nombre);
                    cm.Parameters.AddWithValue("@descripcion", producto.descripcion);
                    cm.Parameters.AddWithValue("@idProveedor", producto.proveedor.idProveedor);
                    cm.Parameters.AddWithValue("@idCategoria", producto.categoria.idCategoria);
                    cm.Parameters.AddWithValue("@precio", producto.precio);
                    cm.Parameters.AddWithValue("@stock", producto.stock);
                    cm.Parameters.AddWithValue("@imagen", producto.imagen);
                    cm.Parameters.AddWithValue("@fecha", producto.fechaRegistro);
                  

                    idRegistrado = Convert.ToInt32(cm.ExecuteScalar());
                }
               

            }
            prodGuardodo = producto;

            return prodGuardodo;
        }
    }
}
