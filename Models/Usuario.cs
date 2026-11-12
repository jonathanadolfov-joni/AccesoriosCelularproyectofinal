using System.ComponentModel.DataAnnotations;
namespace AccesoriosCelular.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Correo { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Rol { get; set; } = "Usuario";
}

public class LoginVM
{
    [Required(ErrorMessage = "Escribe tu correo"), EmailAddress]
    public string Correo { get; set; } = "";
    [Required(ErrorMessage = "Escribe tu contraseña"), DataType(DataType.Password)]
    public string Password { get; set; } = "";
}

public class RegistroVM
{
    [Required(ErrorMessage = "Escribe tu nombre")]
    public string Nombre { get; set; } = "";
    [Required(ErrorMessage = "Escribe tu correo"), EmailAddress]
    public string Correo { get; set; } = "";
    [Required, MinLength(6, ErrorMessage = "Mínimo 6 caracteres"), DataType(DataType.Password)]
    public string Password { get; set; } = "";
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden"), DataType(DataType.Password)]
    public string Confirmar { get; set; } = "";
}
