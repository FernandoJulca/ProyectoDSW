using ApiToolify.Data.Contratos;
using Microsoft.Data.SqlClient;
using ProyectoDSWToolify.Models;

namespace ApiToolify.Data.Repositorios
{
    public class UserAuthRepository : IUserAuth
    {
        private readonly IConfiguration config;
        private readonly string cadenaConexion;

        public UserAuthRepository(IConfiguration configuration)
        {

            config = configuration;
            cadenaConexion = config["ConnectionStrings:DB"];
        }
        public Usuario iniciarSession(string correo, string clave)
        {
            Usuario user = null;
            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var cmd = new SqlCommand("iniciarSession", cnx))
                    {
                        cmd.Parameters.AddWithValue("@CORREO", correo);
                        cmd.Parameters.AddWithValue("@CLAVE", clave);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new Usuario
                                {
                                    idUsuario = Convert.ToInt32(reader["ID_USUARIO"]),
                                    nombre = reader["NOMBRES"].ToString(),
                                    apeMaterno = reader["APE_MATERNO"].ToString(),
                                    apePaterno = reader["APE_PATERNO"].ToString(),
                                    correo = reader["CORREO"].ToString(),
                                    clave = reader["CLAVE"].ToString(),
                                    nroDoc = reader["NRO_DOC"].ToString(),
                                    direccion = reader["DIRECCION"].ToString(),
                                    telefono = reader["TELEFONO"].ToString(),
                                    fechaRegistro = Convert.ToDateTime(reader["FECHA_REGISTRO"]),
                                    distrito = new Distrito
                                    {
                                        idDistrito = Convert.ToInt32(reader["ID_DISTRITO"]),
                                        nombre = reader["NOMBRE"].ToString()
                                    },
                                    rol = new Rol
                                    {
                                        idRol = Convert.ToInt32(reader["ROL"]),
                                        descripcion = reader["DESCRIPCION"].ToString()
                                    }
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return user;
        }

        public Usuario registrarCliente(Usuario u)
        {
            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();

                    string queryValidacion = @"SELECT COUNT(*) FROM TB_USUARIO WHERE CORREO = @CORREO OR NRO_DOC = @NRO_DOC";

                    using (var cmdValidacion = new SqlCommand(queryValidacion, cnx))
                    {
                        cmdValidacion.Parameters.AddWithValue("@CORREO", u.correo);
                        cmdValidacion.Parameters.AddWithValue("@NRO_DOC", u.nroDoc);

                        int count = (int)cmdValidacion.ExecuteScalar();

                        if (count > 0)
                        {
                            throw new Exception("Ya existe un usuario con ese correo o número de documento.");
                        }
                    }

                    using (var cmd = new SqlCommand("registrarCliente", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@NOMBRES", u.nombre);
                        cmd.Parameters.AddWithValue("@APE_MATERNO", u.apeMaterno);
                        cmd.Parameters.AddWithValue("@APE_PATERNO", u.apePaterno);
                        cmd.Parameters.AddWithValue("@CORREO", u.correo);
                        cmd.Parameters.AddWithValue("@CLAVE", u.clave);
                        cmd.Parameters.AddWithValue("@NRO_DOC", u.nroDoc);
                        cmd.Parameters.AddWithValue("@DIRECCION", u.direccion ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ID_DISTRITO", u.distrito.idDistrito);
                        cmd.Parameters.AddWithValue("@TELEFONO", u.telefono);

                        var newId = cmd.ExecuteScalar();

                        if (newId != null)
                        {
                            u.idUsuario = Convert.ToInt32(newId);
                            u.rol = new Rol { idRol = 2, descripcion = "Cliente" };
                            u.fechaRegistro = DateTime.Now;
                            return u;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar cliente: {ex.Message}");
                throw;  
            }

            return null;
        }
    }
}
