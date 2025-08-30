using System.Data;
using ApiToolify.Data.Contratos;
using ApiToolify.Models.DTO;
using Microsoft.Data.SqlClient;

namespace ApiToolify.Data.Repositorios
{
    public class GraficoRepositorio : IGrafico
    {
        private readonly IConfiguration config;
        private readonly string cadenaConexion;
        public GraficoRepositorio(IConfiguration configuration)
        {
            config = configuration;
            cadenaConexion = config["ConnectionStrings:DB"];
        }

        public List<CategoriaProductoDTO> CategoriaProducto(string consulta)
        {
            List<CategoriaProductoDTO> lst = new List<CategoriaProductoDTO>();
            try
            {
                using (SqlConnection cn = new SqlConnection(cadenaConexion))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("GraficosDatosProcedure", cn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("consulta", consulta);
                        using (SqlDataReader r = cmd.ExecuteReader()) {
                            if (r.HasRows)
                            {
                                while (r.Read()) 
                                {
                                    lst.Add(new CategoriaProductoDTO()
                                    {
                                        descripcion = r.IsDBNull(0)  ? "" : r.GetString(0),
                                        totalProductos = r.IsDBNull(1) ? 1 : r.GetInt32(1)
                                    });    
                                }
                            }
                        }
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                 Console.WriteLine("Error en CategoriaProducto: " + ex.Message);
                    throw;
            }
        }


        public List<ProveedorProductoDTO> ProveedorProducto(string consulta)
        {
            List<ProveedorProductoDTO> lst = new List<ProveedorProductoDTO>();
            try
            {
                using (SqlConnection cn = new SqlConnection(cadenaConexion))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("GraficosDatosProcedure", cn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("consulta", consulta);

                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                while (r.Read())
                                {
                                    lst.Add(new ProveedorProductoDTO()
                                    {
                                        razonSocial = r.IsDBNull(0) ? "" : r.GetString(0),
                                        totalProductos = r.IsDBNull(1) ? 1 : r.GetInt32(1)
                                    });
                                }
                            }
                        }
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CategoriaProducto: " + ex.Message);
                throw;
            }
        }

        public List<VentaPorDistritoDTO> VentaPorDistrito(string consulta)
        {
            List<VentaPorDistritoDTO> lst = new List<VentaPorDistritoDTO>();
            try
            {
                using (SqlConnection cn = new SqlConnection(cadenaConexion))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("GraficosDatosProcedure", cn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("consulta", consulta);

                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                while (r.Read())
                                {
                                    lst.Add(new VentaPorDistritoDTO()
                                    {
                                        distrito = r.IsDBNull(0) ? "" : r.GetString(0),
                                        ventasTotales = r.IsDBNull(1) ? 1 : r.GetInt32(1)
                                    });
                                }
                            }
                        }
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CategoriaProducto: " + ex.Message);
                throw;
            }
        }

        public List<VentaPorMesDTO> VentaPorMes(string consulta)
        {
            List<VentaPorMesDTO> lst = new List<VentaPorMesDTO>();
            try
            {
                using (SqlConnection cn = new SqlConnection(cadenaConexion))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("GraficosDatosProcedure", cn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("consulta", consulta);

                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                while (r.Read())
                                {
                                    lst.Add(new VentaPorMesDTO()
                                    {
                                        mes = r.IsDBNull(0) ? "" : r.GetString(0),
                                        ventasTotales = r.IsDBNull(1) ? 1 : r.GetInt32(1)
                                    });
                                }
                            }
                        }
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CategoriaProducto: " + ex.Message);
                throw;
            }
        }

        public List<VentaPorMesAndTipoVentaDTO> VentaPorMesAndTipoVenta(string consulta)
        {
            List<VentaPorMesAndTipoVentaDTO> lst = new List<VentaPorMesAndTipoVentaDTO>();
            try
            {
                using (SqlConnection cn = new SqlConnection(cadenaConexion))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("GraficosDatosProcedure", cn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("consulta", consulta);

                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                while (r.Read())
                                {
                                    lst.Add(new VentaPorMesAndTipoVentaDTO()
                                    {
                                        mes = r.IsDBNull(0) ? "" : r.GetString(0),
                                        tipoVenta = r.IsDBNull(1) ? "" : r.GetString(1),
                                        cantidadVentas = r.IsDBNull(2) ? 0 : r.GetInt32(2)
                                    });
                                }
                            }
                        }
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en CategoriaProducto: " + ex.Message);
                throw;
            }
        }
    }
}
