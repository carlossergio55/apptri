using System.Data;
using System.Linq;

namespace Persistencia.Helpers
{
    public static class CHelper
    {
        public static string ToUnderscoreCase(this string str) =>
               string.Concat(str.Select((x, i) => (i > 0 && char.IsUpper(x) && (char.IsLower(str[i - 1]) || char.IsLower(str[i + 1])))
               ? "_" + x.ToString() : x.ToString())).ToLower();
    }


    public enum ESTADO_CUENTA
    {
        ACTIVO = 1,
        INACTIVO = 0
    }
}
