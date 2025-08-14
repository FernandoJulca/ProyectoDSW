using Microsoft.Data.SqlClient;
using ProyectoDSWToolify.Data.Contratos;
using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Models.ViewModels;

namespace ProyectoDSWToolify.Data.Repositorios
{
    public class UsuarioRepo : IUsuario
    {
        private readonly IConfiguration config;
        private readonly string cadenaConexion;

        public UsuarioRepo(IConfiguration configuration)
        {

            config = configuration;
            cadenaConexion = config["ConnectionStrings:DB"];
        }
        public int contadorClientes()
        {
            return listCliente().Count();
        }

        public List<Usuario> listCliente()
        {
            var list = new List<Usuario>();
            try
            {
                using (var cnx = new SqlConnection(cadenaConexion))
                {
                    cnx.Open();
                    using (var cmd = new SqlCommand("listCliente", cnx))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new Usuario()
                                {
                                    idUsuario = reader.GetInt32(reader.GetOrdinal("ID_USUARIO")),
                                    nombre = reader.GetString(reader.GetOrdinal("NOMBRES")),
                                    apeMaterno = reader.GetString(reader.GetOrdinal("APE_MATERNO")),
                                    apePaterno = reader.GetString(reader.GetOrdinal("APE_PATERNO")),
                                    correo = reader.GetString(reader.GetOrdinal("CORREO")),
                                    clave = reader.GetString(reader.GetOrdinal("CLAVE")),
                                    nroDoc = reader.GetString(reader.GetOrdinal("NRO_DOC")),
                                    direccion = reader.GetString(reader.GetOrdinal("DIRECCION")),
                                    distrito = new Distrito
                                    {
                                        idDistrito = reader.GetInt32(reader.GetOrdinal("ID_DISTRITO")),
                                        nombre = reader.GetString(reader.GetOrdinal("NOMBRE"))
                                    },
                                    telefono = reader.GetString(reader.GetOrdinal("TELEFONO")),
                                    rol = new Rol
                                    {
                                        idRol = reader.GetInt32(reader.GetOrdinal("ROL")),
                                        descripcion = reader.GetString(reader.GetOrdinal("DESCRIPCION")),
                                    },
                                    fechaRegistro = reader.GetDateTime(reader.GetOrdinal("FECHA_REGISTRO"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                list = new List<Usuario>();
            }
            return list;
        }
    }
}
