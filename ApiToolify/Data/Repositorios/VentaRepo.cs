using Microsoft.Data.SqlClient;
using System.Data;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Models;
using ApiToolify.Models.DTO;

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
        public Venta generarVentaCliente(VentaDTO ventaDto)
        {
            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Insertar la venta (sin ID porque es OUTPUT)
                        using (SqlCommand cmd = new SqlCommand("agregarVenta", conn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@ID_USUARIO", ventaDto.idUsuario);
                            cmd.Parameters.AddWithValue("@TOTAL", ventaDto.total);
                            cmd.Parameters.AddWithValue("@TIPO_VENTA", ventaDto.tipoVenta);

                            SqlParameter outputParam = new SqlParameter("@ID_VENTA", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmd.Parameters.Add(outputParam);

                            cmd.ExecuteNonQuery();

                            int idVentaGenerada = (int)outputParam.Value;

                            // Insertar detalles
                            foreach (var detalle in ventaDto.detalles)
                            {
                                using (SqlCommand cmdDetalle = new SqlCommand("detalleVenta", conn, transaction))
                                {
                                    cmdDetalle.CommandType = CommandType.StoredProcedure;

                                    cmdDetalle.Parameters.AddWithValue("@ID_VENTA", idVentaGenerada);
                                    cmdDetalle.Parameters.AddWithValue("@ID_PRODUCTO", detalle.idProducto);
                                    cmdDetalle.Parameters.AddWithValue("@CANTIDAD", detalle.cantidad);
                                    cmdDetalle.Parameters.AddWithValue("@SUB_TOTAL", detalle.subTotal);

                                    cmdDetalle.ExecuteNonQuery();
                                }

                                using (SqlCommand cmdStock = new SqlCommand(
                                    "UPDATE TB_PRODUCTO SET STOCK = STOCK - @CANTIDAD WHERE ID_PRODUCTO = @ID_PRODUCTO AND stock >= @CANTIDAD", conn, transaction))
                                {
                                    cmdStock.Parameters.AddWithValue("@ID_PRODUCTO", detalle.idProducto);
                                    cmdStock.Parameters.AddWithValue("@CANTIDAD", detalle.cantidad);

                                    int rowsAffected = cmdStock.ExecuteNonQuery();
                                    if (rowsAffected == 0)
                                    {
                                        throw new Exception("Stock insuficiente para el producto con ID: " + detalle.idProducto);
                                    }
                                }
                            }

                            transaction.Commit();

                            // Aquí podrías mapear y devolver un objeto Venta con info completa si quieres
                            return new Venta
                            {
                                idVenta = idVentaGenerada,
                                usuario = new Usuario { idUsuario = ventaDto.idUsuario }, // solo el idUsuario por ejemplo
                                total = ventaDto.total,
                                tipoVenta = ventaDto.tipoVenta,
                                estado = ventaDto.estado,
                                fecha = DateTime.Now,
                                Detalles = ventaDto.detalles.Select(d => new DetalleVenta
                                {
                                    producto = new Producto { idProducto = d.idProducto },
                                    cantidad = d.cantidad,
                                    subTotal = d.subTotal
                                }).ToList()
                            };
                        }
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
        public Venta obtenerVentaPorCliente(int idVenta, int idUsuario)
        {
            Venta venta = null;

            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand("obtenerCompraPorIdVenta", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_VENTA", idVenta);
                    cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (venta == null)
                            {
                                venta = new Venta
                                {
                                    idVenta = Convert.ToInt32(reader["ID_VENTA"]),
                                    fecha = reader.GetDateTime(reader.GetOrdinal("FECHA")),
                                    total = reader.GetDecimal(reader.GetOrdinal("TOTAL")),
                                    estado = reader.GetString(reader.GetOrdinal("ESTADO")),
                                    tipoVenta = reader.GetString(reader.GetOrdinal("TIPO_VENTA")),
                                    Detalles = new List<DetalleVenta>()
                                };
                            }
                            venta.usuario = new Usuario
                            {
                                idUsuario = Convert.ToInt32(reader["ID_USUARIO"]),
                                nombre = reader.GetString(reader.GetOrdinal("NOMBRE_USUARIO")),
                                apePaterno = reader.GetString(reader.GetOrdinal("APE_PATERNO")),
                                apeMaterno = reader.GetString(reader.GetOrdinal("APE_MATERNO")),
                                direccion = reader.GetString(reader.GetOrdinal("DIRECCION"))
                            };

                            if (reader["ID_PRODUCTO"] != DBNull.Value)
                            {
                                var detalle = new DetalleVenta
                                {
                                    producto = new Producto
                                    {
                                        idProducto = Convert.ToInt32(reader["ID_PRODUCTO"]),
                                        nombre = reader.GetString(reader.GetOrdinal("NOMBRE_PRODUCTO")),
                                        precio = reader.GetDecimal(reader.GetOrdinal("PRECIO"))
                                    },
                                    cantidad = Convert.ToInt32(reader["CANTIDAD"]),
                                    subTotal = reader.GetDecimal(reader.GetOrdinal("SUB_TOTAL"))
                                };

                                venta.Detalles.Add(detalle);
                            }
                        }
                    }
                }
            }

            return venta;
        }


    }
}
