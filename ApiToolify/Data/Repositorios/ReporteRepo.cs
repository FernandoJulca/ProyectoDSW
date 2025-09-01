using ApiToolify.Data.Contratos;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ApiToolify.Data.Repositorios
{
    public class ReporteRepo : IReporte
    {
        private readonly IConfiguration _config;
        private readonly string cadenaConexion;

        public ReporteRepo(IConfiguration config)
        {
            _config = config;
            this.cadenaConexion = _config["ConnectionStrings:DB"];
        }

        public long ContarClientesAtendidosPorMes(string fechaMes)
        {
            long resultado = 0;
            using (var con = new SqlConnection(cadenaConexion))
            {
                con.Open();
                using (var cmd = new SqlCommand("usp_contarClientesAtendidosPorMes", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FechaMes", fechaMes);

                    var conteo = cmd.ExecuteScalar();
                    if (conteo != null)
                    {
                        resultado = Convert.ToInt64(conteo);
                    }
                }
            }
            return resultado;
        }

        public long ContarProductosVendidosPorMes(string fechaMes)
        {
            long resultado = 0;
            using (var con = new SqlConnection(cadenaConexion))
            {
                con.Open();
                using (var cmd = new SqlCommand("usp_contarProductosVendidosPorMes", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FechaMes", fechaMes);

                    var conteo = cmd.ExecuteScalar();
                    if (conteo != null)
                    {
                        resultado = Convert.ToInt64(conteo);
                    }
                }
            }
            return resultado;
        }

        public long ContarVentasPorMes(string fechaMes)
        {
            long resultado = 0;
            using (var con = new SqlConnection(cadenaConexion))
            {
                con.Open();
                using (var cmd = new SqlCommand("usp_contarVentasPorMes", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FechaMes", fechaMes);

                    var conteo = cmd.ExecuteScalar();
                    if (conteo != null)
                    {
                        resultado = Convert.ToInt64(conteo);
                    }
                }
            }
            return resultado;
        }

        public double ObtenerIngresosTotales()
        {
            long resultado = 0;
            using (var con = new SqlConnection(cadenaConexion))
            {
                con.Open();
                using (var cmd = new SqlCommand("usp_obtenerIngresosTotales", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    var conteo = cmd.ExecuteScalar();
                    if (conteo != null)
                    {
                        resultado = Convert.ToInt64(conteo);
                    }
                }
            }
            return resultado;
        }

        public long ObtenerTotalProductosVendidos()
        {
            long resultado = 0;
            using (var con = new SqlConnection(cadenaConexion))
            {
                con.Open();
                using (var cmd = new SqlCommand("usp_obtenerTotalProductosVendidos", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    var conteo = cmd.ExecuteScalar();
                    if (conteo != null)
                    {
                        resultado = Convert.ToInt64(conteo);
                    }
                }
            }
            return resultado;
        }

        public long ObtenerTotalVentas()
        {
            long resultado = 0;
            using (var con = new SqlConnection(cadenaConexion))
            {
                con.Open();
                using (var cmd = new SqlCommand("usp_obtenerTotalVentas", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    var conteo = cmd.ExecuteScalar();
                    if (conteo != null)
                    {
                        resultado = Convert.ToInt64(conteo);
                    }
                }
            }
            return resultado;
        }
    }
}
