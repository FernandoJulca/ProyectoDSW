using System.Data;
using ApiToolify.Data.Contratos;
using ApiToolify.Models.DTO;
using Microsoft.Data.SqlClient;

namespace ApiToolify.Data.Repositorios
{
    public class RepositorioReportes : IReportes
    {
        private readonly IConfiguration config;
        private readonly string cadenaConexion;
        public RepositorioReportes(IConfiguration configuration)
        {
            config = configuration;
            cadenaConexion = config["ConnectionStrings:DB"];
        }
        public List<ListadoVentaFechaAndTipoVentaDTO> ListadoPorMesAndTipoVenta(DateTime? fechaInicio, DateTime? fechaFin, string? tipoVenta)
        {
            List<ListadoVentaFechaAndTipoVentaDTO> listado = new List<ListadoVentaFechaAndTipoVentaDTO>();
            try
            {
                using (SqlConnection cn = new SqlConnection(cadenaConexion))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("ListadoVentaFechaAndTipoVenta", cn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                       
                        cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                        cmd.Parameters.AddWithValue("@fechaFin", fechaFin);
                        cmd.Parameters.AddWithValue("@tipoVenta", tipoVenta);


                        System.Diagnostics.Debug.WriteLine("------------INICIANDO EL METODO ListadoPorMesAndTipoVenta API REPOSITORIO -----------------");
                        System.Diagnostics.Debug.WriteLine("fecha INICIO obetnida API  " + fechaInicio);
                        System.Diagnostics.Debug.WriteLine("Fecha FIN obtenida API: " + fechaFin);
                        System.Diagnostics.Debug.WriteLine("TIPO VENTA  obtenida API: " + tipoVenta);
                        System.Diagnostics.Debug.WriteLine("------------FIN EL METODO ListadoPorMesAndTipoVenta API REPOSITORIO -----------------");
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                while (r.Read())
                                {
                                    listado.Add(new ListadoVentaFechaAndTipoVentaDTO()
                                    {
                                        idVenta = r.IsDBNull(0) ? 0 : r.GetInt32(0),
                                        nombresCompletos = r.IsDBNull(1) ? "" : r.GetString(1),
                                        direccion = r.IsDBNull(2) ? "" : r.GetString(2),
                                        fechaGenerada = r.IsDBNull(3) ? DateTime.Now : r.GetDateTime(3),
                                        total = r.IsDBNull(4) ? 0 : r.GetDecimal(4),
                                        estado = r.IsDBNull(5) ? "" : r.GetString(5),
                                        tipoVenta = r.IsDBNull(6) ? "" : r.GetString(6)
                                    });
                                }
                            }
                        }
                    }

                }
                return listado;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en CategoriaProducto: " + ex.Message);
                throw;
            }

        }

        public List<ListarProductosPorCategoriaDTO> ListarProductosPorCategoria(int? idCategoria, string? orden)
        {
            List<ListarProductosPorCategoriaDTO> listado = new List<ListarProductosPorCategoriaDTO>();
            try
            {
                using (SqlConnection cn = new SqlConnection(cadenaConexion))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("ListarProductosPorCategoria", cn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@idCategoria", idCategoria);
                        cmd.Parameters.AddWithValue("@orden", orden);


                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                while (r.Read())
                                {
                                    listado.Add(new ListarProductosPorCategoriaDTO()
                                    {
                                        idProducto = r.IsDBNull(0) ? 0 : r.GetInt32(0),
                                        nombre = r.IsDBNull(1) ? "" : r.GetString(1),
                                        descripcion = r.IsDBNull(2) ? "" : r.GetString(2),
                                        proveedor = r.IsDBNull(3) ? "" : r.GetString(3),
                                        categoria = r.IsDBNull(4) ? "" : r.GetString(4),
                                        precio = r.IsDBNull(5) ? 0 : r.GetDecimal(5),
                                        stock = r.IsDBNull(6) ? 0 : r.GetInt32(6),
                                        fechaRegistro = r.IsDBNull(7) ? DateTime.Now : r.GetDateTime(7)
                                    });
                                }
                            }
                        }
                    }

                }
                return listado;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en CategoriaProducto: " + ex.Message);
                throw;
            }
        }
    }
}
