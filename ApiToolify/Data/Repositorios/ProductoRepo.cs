using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Data.Repositorios
{
    public class ProductoRepo : IProducto
    {
        private readonly IConfiguration config;
        private readonly string cadenaConexion;

        public ProductoRepo(IConfiguration configuration)
        {

            config = configuration;
            cadenaConexion = config["ConnectionStrings:DB"];
        }

        public int contadorProductos()
        {
            return listProductosCliente().Count();
        }

        public List<Producto> listProductosCliente()
        {
            var list = new List<Producto>();
            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var cmd = new SqlCommand("listarProductosCliente", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                list.Add(new Producto()
                                {
                                    idProducto = rdr.GetInt32(rdr.GetOrdinal("ID_PRODUCTO")),
                                    nombre = rdr.GetString(rdr.GetOrdinal("NOMBRE")),
                                    descripcion = rdr.GetString(rdr.GetOrdinal("DESCRIPCION_P")),
                                    proveedor = new Proveedor()
                                    {
                                        idProveedor = rdr.GetInt32(rdr.GetOrdinal("ID_PROVEEDOR")),
                                        razonSocial = rdr.GetString(rdr.GetOrdinal("RAZON_SOCIAL")),
                                    },
                                    categoria = new Categoria()
                                    {
                                        idCategoria = rdr.GetInt32(rdr.GetOrdinal("ID_CATEGORIA")),
                                        descripcion = rdr.GetString(rdr.GetOrdinal("DESCRIPCION_C")),
                                    },
                                    precio = rdr.GetDecimal(rdr.GetOrdinal("PRECIO")),
                                    stock = rdr.GetInt32(rdr.GetOrdinal("STOCK")),
                                    imagen = rdr.IsDBNull(rdr.GetOrdinal("IMAGEN")) ? "" : rdr.GetString(rdr.GetOrdinal("IMAGEN")),
                                    fechaRegistro = rdr.GetDateTime(rdr.GetOrdinal("FECHA_REGISTRO")),
                                    estado = rdr.GetBoolean(rdr.GetOrdinal("ESTADO"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en listProductosCliente: " + ex.Message);
                list = new List<Producto>();
            }
            return list;
        }

        public List<Producto> listProductosVendedor()
        {
            var list = new List<Producto>();
            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var cmd = new SqlCommand("usp_listarProductosVendedor", cnx))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                list.Add(new Producto()
                                {
                                    idProducto = rdr.GetInt32(rdr.GetOrdinal("ID_PRODUCTO")),
                                    nombre = rdr.GetString(rdr.GetOrdinal("NOMBRE")),
                                    categoria = new Categoria()
                                    {
                                        idCategoria = rdr.GetInt32(rdr.GetOrdinal("ID_CATEGORIA")),
                                        descripcion = rdr.GetString(rdr.GetOrdinal("DESCRIPCION")),
                                    },
                                    precio = rdr.GetDecimal(rdr.GetOrdinal("PRECIO")),
                                    stock = rdr.GetInt32(rdr.GetOrdinal("STOCK"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en usp_listarProductosVendedor: " + ex.Message);
                list = new List<Producto>();
            }
            return list;
        }

        public Producto obtenerPorId(int id)
        {
            return listProductosCliente().FirstOrDefault(producto => producto.idProducto == id);
        }
    }
}
