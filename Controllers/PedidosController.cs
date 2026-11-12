using System.Security.Claims;
using AccesoriosCelular.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace AccesoriosCelular.Controllers;

[Authorize]
public class PedidosController : Controller
{
    private readonly AppDbContext _db;
    public PedidosController(AppDbContext db) { _db = db; }

    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var q = _db.Pedidos.Include(p => p.Usuario).AsQueryable();
        if (!User.IsInRole("Admin")) q = q.Where(p => p.UsuarioId == UsuarioId);
        return View(await q.OrderByDescending(p => p.Fecha).ToListAsync());
    }

    public async Task<IActionResult> Ticket(int id)
    {
        var p = await _db.Pedidos.Include(x => x.Usuario).Include(x => x.Detalles).FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return NotFound();
        if (!User.IsInRole("Admin") && p.UsuarioId != UsuarioId) return Forbid();
        return View(p);
    }
}
