using Microsoft.AspNetCore.Identity.Data;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using ProyectoDSWToolify.Models;
using ProyectoDSWToolify.Services.Contratos;
using ProyectoDSWToolify.Models.ViewModels;

namespace ProyectoDSWToolify.Services.Implementacion
{
    public class UserAuthService : IUserAuthService
    {
        private readonly HttpClient _httpClient;


        public UserAuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<Usuario> iniciarSession(LoginResponse responselogin)
        {
            var json = JsonConvert.SerializeObject(responselogin);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("UserAuth/login", content); 

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var usuario = JsonConvert.DeserializeObject<Usuario>(responseJson);
                return usuario;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return null; 
            }
            else
            {
                throw new Exception("Error al iniciar sesión: " + response.ReasonPhrase);
            }
        }

        public async Task<Usuario> registrarCliente(Usuario u)
        {
            var json = JsonConvert.SerializeObject(u);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("UserAuth/register", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(responseJson);
                var usuarioJson = data.usuario.ToString();
                var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);
                return usuario;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                dynamic errorData = JsonConvert.DeserializeObject(errorResponse);
                string mensaje = errorData?.message ?? "Error desconocido";

                throw new Exception(mensaje);
            }
        }


    }
}
