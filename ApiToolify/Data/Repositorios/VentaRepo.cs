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

        public Venta obtenerVentaPorUsuario(int idVenta, int idUsuario)
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
                                direccion = reader.GetString(reader.GetOrdinal("DIRECCION")),
                                rol = new Rol
                                {
                                    idRol = Convert.ToInt32(reader["ID_ROL"]),
                                    descripcion = reader.GetString(reader.GetOrdinal("DESCRIPCION"))
                                }
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

        public Venta generarVentaVendedor(VentaDTO ventaDto)
        {
            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_agregarVenta", conn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@ID_USUARIO", ventaDto.idUsuario);
                            cmd.Parameters.AddWithValue("@TOTAL", ventaDto.total);

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
                                using (SqlCommand cmdDetalle = new SqlCommand("usp_detalleVenta", conn, transaction))
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

                            return new Venta
                            {
                                idVenta = idVentaGenerada,
                                usuario = new Usuario { idUsuario = ventaDto.idUsuario },
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

        public List<Venta> obtenerPorVendedor(int idUsuario)
        {
            var ventas = new List<Venta>();

            using (SqlConnection con = new SqlConnection(cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand("usp_obtenerHistorialVentas", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_USUARIO", idUsuario);

                    con.Open();

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
                                    usuario = new Usuario()
                                    {
                                        nombre = reader["NOMBRES"].ToString()
                                    },
                                    total = Convert.ToDecimal(reader["TOTAL"]),
                                    estado = reader["ESTADO"].ToString(),
                                    tipoVenta = reader["TIPO_VENTA"].ToString(),
                                    Detalles = new List<DetalleVenta>()
                                };
                                ventasMap[idVenta] = venta;
                            }

                            if (reader["ID_PRODUCTO"] != DBNull.Value)
                            {
                                var detalle = new DetalleVenta
                                {
                                    producto = new Producto()
                                    {
                                        idProducto = Convert.ToInt32(reader["ID_PRODUCTO"]),
                                        nombre = reader["NOMBRE_PRODUCTO"].ToString(),
                                        precio = Convert.ToDecimal(reader["PRECIO"])
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
        public Venta obtenerVentaPorId(int id)
        {
            Venta venta = null;

            using (SqlConnection con = new SqlConnection(cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand("usp_obtenerVentaPorId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_VENTA", id);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (venta == null)
                            {
                                venta = new Venta
                                {
                                    idVenta = Convert.ToInt32(reader["ID_VENTA"]),
                                    fecha = Convert.ToDateTime(reader["FECHA"]),
                                    usuario = new Usuario()
                                    {
                                        nombre = reader["NOMBRES"].ToString(),
                                        apePaterno = reader["APE_PATERNO"].ToString(),
                                        apeMaterno = reader["APE_MATERNO"].ToString(),
                                        nroDoc = reader["NRO_DOC"].ToString()
                                    },
                                    total = Convert.ToDecimal(reader["TOTAL"]),
                                    estado = reader["ESTADO"].ToString(),
                                    tipoVenta = reader["TIPO_VENTA"].ToString(),
                                    Detalles = new List<DetalleVenta>()
                                };
                            }

                            if (reader["ID_PRODUCTO"] != DBNull.Value)
                            {
                                var detalle = new DetalleVenta
                                {
                                    producto = new Producto()
                                    {
                                        idProducto = Convert.ToInt32(reader["ID_PRODUCTO"]),
                                        nombre = reader["NOMBRE_PRODUCTO"].ToString(),
                                        precio = Convert.ToDecimal(reader["PRECIO"])
                                    },
                                    cantidad = Convert.ToInt32(reader["CANTIDAD"]),
                                    subTotal = Convert.ToDecimal(reader["SUB_TOTAL"])
                                };

                                venta.Detalles.Add(detalle);
                            }
                        }
                    }
                }
            }

            return venta;
        }
        public List<Venta> obtenerLstPedidos()
        {
            List<Venta> lista = new List<Venta>();

            using (SqlConnection con = new SqlConnection(cadenaConexion))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("usp_obtenerListadoPedidos", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        Dictionary<int, Venta> dictVentas = new Dictionary<int, Venta>();

                        while (dr.Read())
                        {
                            int idVenta = Convert.ToInt32(dr["ID_VENTA"]);

                            if (!dictVentas.ContainsKey(idVenta))
                            {
                                Venta v = new Venta()
                                {
                                    idVenta = idVenta,
                                    fecha = Convert.ToDateTime(dr["FECHA"]),
                                    usuario = new Usuario()
                                    {
                                        nombre = dr["NOMBRES"].ToString()
                                    },
                                    total = Convert.ToDecimal(dr["TOTAL"]),
                                    estado = dr["ESTADO"].ToString(),
                                    tipoVenta = dr["TIPO_VENTA"].ToString(),
                                    Detalles = new List<DetalleVenta>()
                                };

                                dictVentas.Add(idVenta, v);
                            }

                            var detalle = new DetalleVenta
                            {
                                producto = new Producto()
                                {
                                    idProducto = Convert.ToInt32(dr["ID_PRODUCTO"]),
                                    nombre = dr["NOMBRE_PRODUCTO"].ToString(),
                                    precio = Convert.ToDecimal(dr["PRECIO"])
                                },
                                cantidad = Convert.ToInt32(dr["CANTIDAD"]),
                                subTotal = Convert.ToDecimal(dr["SUB_TOTAL"])
                            };

                            dictVentas[idVenta].Detalles.Add(detalle);
                        }

                        lista = dictVentas.Values.ToList();
                    }
                }
            }

            return lista;
        }
        public Venta editarEstadoVenta(int idVenta, string nuevoEstado)
        {
            using (var con = new SqlConnection(cadenaConexion))
            {
                con.Open();
                using (var cmd = new SqlCommand("usp_editarEstadoVenta", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_VENTA", idVenta);
                    cmd.Parameters.AddWithValue("@ESTADO", nuevoEstado);
                    cmd.ExecuteNonQuery();
                }
            }

            return obtenerVentaPorId(idVenta);
        }
        public List<Venta> obtenerVentasRemota()
        {
            var ventas = new List<Venta>();

            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand("listarVentasRemotasPendientes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var venta = new Venta
                            {
                                idVenta = Convert.ToInt32(reader["ID_VENTA"]),
                                fecha = Convert.ToDateTime(reader["FECHA"]),
                                total = Convert.ToDecimal(reader["TOTAL"]),
                                estado = reader["ESTADO"].ToString(),
                                tipoVenta = reader["TIPO_VENTA"].ToString(),
                                usuario = new Usuario
                                {
                                    idUsuario = Convert.ToInt32(reader["ID_USUARIO"]),
                                    nombre = reader["NOMBRES"].ToString(),
                                    apePaterno = reader["APE_PATERNO"].ToString(),
                                    apeMaterno = reader["APE_MATERNO"].ToString(),
                                    direccion = reader["DIRECCION"].ToString()
                                },
                                Detalles = new List<DetalleVenta>()
                            };

                            ventas.Add(venta);
                        }
                    }
                }
            }

            return ventas;
        }

        public List<Venta> obtenerVentasRemotaPendientes()
        {
            var ventas = new List<Venta>();

            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand("listarVentasRemotasPendientes", conn))
                {
                    cmd.Parameters.AddWithValue("@filtrarEstado", "P");
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var venta = new Venta
                            {
                                idVenta = Convert.ToInt32(reader["ID_VENTA"]),
                                fecha = Convert.ToDateTime(reader["FECHA"]),
                                total = Convert.ToDecimal(reader["TOTAL"]),
                                estado = reader["ESTADO"].ToString(),
                                tipoVenta = reader["TIPO_VENTA"].ToString(),
                                usuario = new Usuario
                                {
                                    idUsuario = Convert.ToInt32(reader["ID_USUARIO"]),
                                    nombre = reader["NOMBRES"].ToString(),
                                    apePaterno = reader["APE_PATERNO"].ToString(),
                                    apeMaterno = reader["APE_MATERNO"].ToString(),
                                    direccion = reader["DIRECCION"].ToString()
                                },
                                Detalles = new List<DetalleVenta>()
                            };

                            ventas.Add(venta);
                        }
                    }
                }
            }

            return ventas;
        }
        public void CambiarEstadoVenta(int idVenta, string estado)
        {
            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("cambiarEstadoVenta", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_VENTA", idVenta);
                    cmd.Parameters.AddWithValue("@ESTADO", estado);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public int ContarRemotas(string estado)
        {
            int total = 0;

            using (SqlConnection conn = new SqlConnection(cadenaConexion))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("countRemotasPendientes", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Estado", estado);


                    total = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return total;
        }
    }
}
