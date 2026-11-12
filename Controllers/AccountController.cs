using System.Security.Claims;
using AccesoriosCelular.Data;
using AccesoriosCelular.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace AccesoriosCelular.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<Usuario> _hasher = new();
    public AccountController(AppDbContext db) { _db = db; }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) { ViewData["ReturnUrl"] = returnUrl; return View(); }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM vm, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(vm);
        var u = await _db.Usuarios.FirstOrDefaultAsync(x => x.Correo == vm.Correo);
        if (u == null || _hasher.VerifyHashedPassword(u, u.PasswordHash, vm.Password) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Correo o contraseña incorrectos");
            return View(vm);
        }
        await Entrar(u);
        return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl!) : RedirectToAction("Index", "Productos");
    }

    [HttpGet] public IActionResult Registro() => View();

    [HttpPost]
    public async Task<IActionResult> Registro(RegistroVM vm)
    {
        if (!ModelState.IsValid) return View(vm);
        if (await _db.Usuarios.AnyAsync(x => x.Correo == vm.Correo))
        {
            ModelState.AddModelError("Correo", "Ese correo ya está registrado");
            return View(vm);
        }
        var u = new Usuario { Nombre = vm.Nombre, Correo = vm.Correo, Rol = "Usuario" };
        u.PasswordHash = _hasher.HashPassword(u, vm.Password);
        _db.Usuarios.Add(u);
        await _db.SaveChangesAsync();
        await Entrar(u);
        return RedirectToAction("Index", "Productos");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccesoDenegado() => View();

    private Task Entrar(Usuario u)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, u.Id.ToString()),
            new(ClaimTypes.Name, u.Nombre),
            new(ClaimTypes.Role, u.Rol)
        };
        return HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies")));
    }
}
