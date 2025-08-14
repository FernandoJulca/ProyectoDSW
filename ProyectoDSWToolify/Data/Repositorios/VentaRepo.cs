using Microsoft.Data.SqlClient;
using System.Data;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Models;

namespace ProyectoDSWToolify.Data.Repositorios
{
    public class VentaRepo : IVenta
    {
        private readonly IConfiguration config;
        private readonly string cadenaConexion;

        public VentaRepo(IConfiguration configuration)
        {

            config = configuration;
            cadenaConexion = config["ConnectionStrings:DB"];
        }
        public Venta generarVentaCliente(Venta venta)
        {
            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Insertar la venta
                        using (SqlCommand cmd = new SqlCommand("agregarVenta", conn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@ID_USUARIO", venta.usuario.idUsuario);
                            cmd.Parameters.AddWithValue("@TOTAL", venta.total);
                            cmd.Parameters.AddWithValue("@TIPO_VENTA", venta.tipoVenta);

                            SqlParameter outputParam = new SqlParameter("@ID_VENTA", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmd.Parameters.Add(outputParam);

                            cmd.ExecuteNonQuery();

                            venta.idVenta = (int)outputParam.Value;
                        }

                        foreach (var detalle in venta.Detalles)
                        {
                            using (SqlCommand cmd = new SqlCommand("detalleVenta", conn, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.AddWithValue("@ID_VENTA", venta.idVenta);
                                cmd.Parameters.AddWithValue("@ID_PRODUCTO", detalle.producto.idProducto);
                                cmd.Parameters.AddWithValue("@CANTIDAD", detalle.cantidad);
                                cmd.Parameters.AddWithValue("@SUB_TOTAL", detalle.subTotal);
                                cmd.ExecuteNonQuery();
                            }

                            using (SqlCommand cmdStock = new SqlCommand(
                                "UPDATE TB_PRODUCTO SET STOCK = STOCK - @CANTIDAD WHERE ID_PRODUCTO = @ID_PRODUCTO AND stock >= @CANTIDAD", conn, transaction))
                            {
                                cmdStock.Parameters.AddWithValue("@ID_PRODUCTO", detalle.producto.idProducto);
                                cmdStock.Parameters.AddWithValue("@CANTIDAD", detalle.cantidad);

                                int rowsAffected = cmdStock.ExecuteNonQuery();
                                if (rowsAffected == 0)
                                {
                                    throw new Exception("Stock insuficiente para el producto con ID: " + detalle.producto.idProducto);
                                }
                            }
                        }

                        transaction.Commit();
                        venta.Fecha = DateTime.Now;
                        return venta;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error al generar la venta: " + ex.Message);
                    }
                }
            }
        }

        public List<Venta> obtenerPorCliente(int idUsuario)
        {
            var ventas = new List<Venta>();

            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand("obtenerCompraPorIdCliente", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Dictionary<int, Venta> ventasMap = new();

                        while (reader.Read())
                        {
                            int idVenta = Convert.ToInt32(reader["ID_VENTA"]);

                            if (!ventasMap.ContainsKey(idVenta))
                            {
                                var venta = new Venta
                                {
                                    idVenta = idVenta,
                                    fecha = Convert.ToDateTime(reader["FECHA"]),
                                    total = Convert.ToDecimal(reader["TOTAL"]),
                                    estado = Convert.ToString(reader["ESTADO"].ToString()),
                                    tipoVenta = reader["TIPO_VENTA"].ToString()
                                };
                                ventasMap[idVenta] = venta;
                            }

                            // Si el SP devuelve los detalles
                            if (reader["ID_PRODUCTO"] != DBNull.Value)
                            {
                                var detalle = new DetalleVenta
                                {
                                    producto = new Producto()
                                    {
                                        idProducto = Convert.ToInt32(reader["ID_PRODUCTO"]),
                                        nombre = Convert.ToString(reader["NOMBRE_PRODUCTO"]),
                                        precio = Convert.ToDecimal(reader["PRECIO"]),
                                    },
                                    cantidad = Convert.ToInt32(reader["CANTIDAD"]),
                                    subTotal = Convert.ToDecimal(reader["SUB_TOTAL"])
                                };

                                ventasMap[idVenta].Detalles.Add(detalle);
                            }
                        }

                        ventas = ventasMap.Values.ToList();
                    }
                }
            }

            return ventas;
        }

    }
}
