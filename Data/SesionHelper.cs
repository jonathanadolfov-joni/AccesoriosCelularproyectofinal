using System.Text.Json;
using Microsoft.AspNetCore.Http;
namespace AccesoriosCelular.Data;

public static class SesionHelper
{
    public static void Guardar<T>(this ISession s, string key, T value) => s.SetString(key, JsonSerializer.Serialize(value));
    public static T? Leer<T>(this ISession s, string key)
    {
        var v = s.GetString(key);
        return v == null ? default : JsonSerializer.Deserialize<T>(v);
    }
}
