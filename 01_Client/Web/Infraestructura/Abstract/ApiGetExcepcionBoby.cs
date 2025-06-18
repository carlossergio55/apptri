using System.Collections.Generic;
using System.Text.Json;

namespace Infraestructura.Abstract
{
    public class ApiGetExcepcionBoby
    {
        public static Dictionary<string, List<string>> ExtrarApiExcepcion(string json)
        {
            var resp = new Dictionary<string, List<string>>();
            var jsonElem = JsonSerializer.Deserialize<JsonElement>(json);
            var errorsJson = jsonElem.GetProperty("errors");
            foreach (var campoError in errorsJson.EnumerateObject())
            {
                var campo = campoError.Name;
                var errores = new List<string>();
                foreach (var errorkind in campoError.Value.EnumerateArray())
                {
                    var error = errorkind.GetString();
                    errores.Add(error);
                }
                resp.Add(campo, errores);
            }
            return resp;
        }
      
    }
}
