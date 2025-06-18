using Blazored.LocalStorage;
using Infraestructura.Abstract;
using Infraestructura.Models.Authentication;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infraestructura.Services
{
    public class ManagerRest : IManagerRest
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalStorageService _localStorageService;

        public ManagerRest(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
        {
            _httpClientFactory = httpClientFactory;
            _localStorageService = localStorageService;
        }

        public async Task<ResponseEntity<T>> GetAsyncFromQuery<T>(string pControlador, object parametros)
        {
            HttpResponseMessage result = new();
            var value = string.Empty;
            var cliente = await GetCliente();

            var jObj = JsonSerializer.Serialize<object>(parametros);
            foreach (var item in JsonSerializer.Deserialize<JsonElement>(jObj).EnumerateObject())
            {
                value += item.Name + "=" + item.Value.ToString() + "&";
            };
            value = value.Substring(0, value.Length - 1);
            result = await cliente.GetAsync(pControlador + "?" + value);

            if (result.IsSuccessStatusCode)
            {
                using var responseStream = await result.Content.ReadAsStreamAsync();
                var data = await JsonSerializer.DeserializeAsync<ResponseEntity<T>>(responseStream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                data.State = data.Succeeded == true ? State.Success : State.Warning;
                return data;
            }
            else
            {
                var errorres = await result.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseEntity<T>>(errorres);
            }
        }
        public async Task<ResponseEntity<T>> GetAsyncFromPath<T>(string pControlador, object parametros)
        {
            HttpResponseMessage result = new();
            var value = string.Empty;
            var cliente = await GetCliente();

            if (Convert.GetTypeCode(parametros) == TypeCode.Object)
            {
                var jObj = JsonSerializer.Serialize<object>(parametros);
                foreach (var item in JsonSerializer.Deserialize<JsonElement>(jObj).EnumerateObject())
                {
                    value += "/" + item.Value.ToString();
                };
            }
            else
            {
                value = "/" + parametros;
            }

            Console.WriteLine(pControlador + value);

            result = await cliente.GetAsync(pControlador + value);

            if (result.IsSuccessStatusCode)
            {
                using var responseStream = await result.Content.ReadAsStreamAsync();
                var data = await JsonSerializer.DeserializeAsync<ResponseEntity<T>>(responseStream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                data.State = data.Succeeded == true ? State.Success : State.Warning;
                return data;
            }
            else
            {
                var errorres = await result.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseEntity<T>>(errorres);
            }
        }
        public async Task<ResponseEntity<T>> GetAsync<T>(string pControlador)
        {

            HttpResponseMessage result = new();
            var value = string.Empty;
            var cliente = await GetCliente();

            result = await cliente.GetAsync(pControlador);

            if (result.IsSuccessStatusCode)
            {
                using var responseStream = await result.Content.ReadAsStreamAsync();
                var data = await JsonSerializer.DeserializeAsync<ResponseEntity<T>>(responseStream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                data.State = data.Succeeded == true ? State.Success : State.Warning;
                return data;
            }
            else
            {
                var errorres = await result.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseEntity<T>>(errorres);
            }
        }

        public async Task<T> GetPlainAsync<T>(string controllerUrl)
        {
            var cliente = await GetCliente(); // Tu HttpClient con la base URL, tokens, cabeceras, etc.
            var result = await cliente.GetAsync(controllerUrl);

            if (!result.IsSuccessStatusCode)
            {
                // Manejar el error según tus necesidades, por ejemplo, registrar el error y devolver default(T)
                return default;
            }

            using var responseStream = await result.Content.ReadAsStreamAsync();
            var data = await JsonSerializer.DeserializeAsync<T>(
                responseStream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return data;
        }


        public async Task<ResponseEntity<T>> PostAsync<T>(string pControlador, object parametros)
        {
            // Obtenemos el HttpClient
            var cliente = await GetCliente();

            // Hacemos la petición POST
            HttpResponseMessage result = await cliente.PostAsJsonAsync(pControlador, parametros);

            // --- Aquí agregas tu “punto de quiebre” (breakpoint) o depuración ---
            var statusCode = result.StatusCode;
            var responseString = await result.Content.ReadAsStringAsync();

            // Si usas Debug o Console, puedes mostrarlo (en Visual Studio puedes simplemente poner breakpoints):
            Console.WriteLine($"StatusCode: {statusCode}");
            Console.WriteLine($"Body: {responseString}");

            // Ahora vemos si la respuesta fue exitosa (código 2xx)
            if (result.IsSuccessStatusCode)
            {
                // Deserializamos desde la cadena que ya leímos (responseString) 
                // en vez de volver a leer el Stream
                using var responseStream = new MemoryStream(Encoding.UTF8.GetBytes(responseString));
                var data = await JsonSerializer.DeserializeAsync<ResponseEntity<T>>(
                    responseStream,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
                );

                // Ajustamos el estado según la propiedad Succeeded
                data.State = data.Succeeded == true ? State.Success : State.Warning;
                return data;
            }
            else
            {
                // En caso de error, asumimos que el servidor igual nos manda un JSON compatible con ResponseEntity<T>
                // Si no lo hiciera (por ejemplo, un HTML de error 401), fallaría aquí. 
                return JsonSerializer.Deserialize<ResponseEntity<T>>(
                    responseString,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
                );
            }
        }

        public async Task<ResponseEntity<T>> PutAsync<T>(string pControlador, object parametros, int id)
        {
            var cliente = await GetCliente();
            HttpResponseMessage result = new();
            result = await cliente.PutAsJsonAsync(pControlador + "/" + id, parametros);

            if (result.IsSuccessStatusCode)
            {
                using var responseStream = await result.Content.ReadAsStreamAsync();
                var data = await JsonSerializer.DeserializeAsync<ResponseEntity<T>>(responseStream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                data.State = data.Succeeded == true ? State.Success : State.Warning;
                data.Message = data.Succeeded == true ? $"Se actualizó el registro Nro : { id } correctamente." : data.Message;
                return data;
            }
            else
            {
                var errorres = await result.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseEntity<T>>(errorres);
            }
        }


        public async Task<ResponseEntity<T>> PatchAsync<T>(string pControlador, object parametros, int id)
        {
            var cliente = await GetCliente();
            HttpResponseMessage result = new();
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{pControlador}/{id}")
            {
                Content = JsonContent.Create(parametros)
            };

            result = await cliente.SendAsync(request);

            if (result.IsSuccessStatusCode)
            {
                using var responseStream = await result.Content.ReadAsStreamAsync();
                var data = await JsonSerializer.DeserializeAsync<ResponseEntity<T>>(
                    responseStream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                data.State = data.Succeeded ? State.Success : State.Warning;
                data.Message = data.Succeeded ? $"Se actualizó el registro Nro: {id} correctamente." : data.Message;
                return data;
            }
            else
            {
                var errorres = await result.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseEntity<T>>(errorres);
            }
        }


        public async Task<ResponseEntity<T>> DeleteAsync<T>(string pControlador, int id)
        {
            var cliente = await GetCliente();
            HttpResponseMessage result = new();
            result = await cliente.DeleteAsync(pControlador + "/" + id);

            if (result.IsSuccessStatusCode)
            {
                using var responseStream = await result.Content.ReadAsStreamAsync();
                var data = await JsonSerializer.DeserializeAsync<ResponseEntity<T>>(responseStream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });


                data.State = data.Succeeded == true ? State.Success : State.Warning;
                data.Message = data.Succeeded == true ? $"Se Elimino el registro { id } correctamente" : data.Message;

                return data;
            }
            else
            {
                var errorres = await result.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ResponseEntity<T>>(errorres);
            }
        }

        /// <summary>
        /// Servicio utilizaado solo para acceso a seguridad
        /// </summary>
        /// <param name="api">Url api</param>
        /// <param name="vmodel">Modelo de envio por body</param>
        /// <returns>Token</returns>
        public async Task<SegResponse> PostAsync(string pControlador, LoginRequest vmodel)
        {
            var cliente = _httpClientFactory.CreateClient("ApiSeg");
            var vresp = await cliente.PostAsJsonAsync(pControlador, vmodel);

            if (vresp.IsSuccessStatusCode)
            {
                using var responseStream = await vresp.Content.ReadAsStreamAsync();
                var data = await JsonSerializer.DeserializeAsync<SegResponse>(responseStream);
                data.StatusCode = vresp.StatusCode;
                return data;
            }
            else
            {
                return new SegResponse
                {
                    message = vresp.StatusCode == HttpStatusCode.Unauthorized ? "Acceso denegado, Usuaro o Contraseña incorrectos" : vresp.ReasonPhrase,
                    StatusCode = vresp.StatusCode
                };
            }
        }

        private async Task<HttpClient> GetCliente()
        {
            var cliente = _httpClientFactory.CreateClient("Api");
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _localStorageService.GetItemAsStringAsync("KEYACCES"));
            return cliente;
        }

    }


}
