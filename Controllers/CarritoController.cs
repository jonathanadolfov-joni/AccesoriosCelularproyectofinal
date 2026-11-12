using System.Security.Claims;
using AccesoriosCelular.Data;
using AccesoriosCelular.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace AccesoriosCelular.Controllers;

[Authorize]
public class CarritoController : Controller
{
    private const string Key = "carrito";
    private readonly AppDbContext _db;
    public CarritoController(AppDbContext db) { _db = db; }

    private List<ItemCarrito> Leer() => HttpContext.Session.Leer<List<ItemCarrito>>(Key) ?? new();

    public async Task<IActionResult> Index()
    {
        var items = Leer();
        var ids = items.Select(i => i.ProductoId).ToList();
        var prods = await _db.Productos.Where(p => ids.Contains(p.Id)).ToListAsync();
        var lineas = items.Where(i => prods.Any(p => p.Id == i.ProductoId)).Select(i =>
        {
            var p = prods.First(x => x.Id == i.ProductoId);
            return new LineaCarrito { ProductoId = p.Id, Nombre = p.Nombre, Precio = p.Precio, Cantidad = i.Cantidad };
        }).ToList();
        return View(lineas);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(int id)
    {
        var p = await _db.Productos.FindAsync(id);
        if (p == null) return NotFound();
        var items = Leer();
        var item = items.FirstOrDefault(i => i.ProductoId == id);
        var actual = item?.Cantidad ?? 0;
        if (actual + 1 > p.Stock)
            TempData["Mensaje"] = $"No hay más existencias de \"{p.Nombre}\".";
        else
        {
            if (item == null) items.Add(new ItemCarrito { ProductoId = id, Cantidad = 1 }); else item.Cantidad++;
            HttpContext.Session.Guardar(Key, items);
            TempData["Mensaje"] = $"\"{p.Nombre}\" se agregó al carrito.";
        }
        return RedirectToAction("Index", "Productos");
    }

    [HttpPost]
    public IActionResult Quitar(int id)
    {
        var items = Leer();
        items.RemoveAll(i => i.ProductoId == id);
        HttpContext.Session.Guardar(Key, items);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Confirmar()
    {
        var items = Leer();
        if (!items.Any()) return RedirectToAction(nameof(Index));
        var ids = items.Select(i => i.ProductoId).ToList();
        var prods = await _db.Productos.Where(p => ids.Contains(p.Id)).ToListAsync();

        var pedido = new Pedido
        {
            UsuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            Fecha = DateTime.Now
        };
        foreach (var i in items)
        {
            var p = prods.FirstOrDefault(x => x.Id == i.ProductoId);
            if (p == null) continue;
            if (i.Cantidad > p.Stock)
            {
                TempData["Mensaje"] = $"No hay suficientes existencias de \"{p.Nombre}\". Quita unidades e intenta de nuevo.";
                return RedirectToAction(nameof(Index));
            }
            p.Stock -= i.Cantidad;
            pedido.Detalles.Add(new DetallePedido { ProductoId = p.Id, Nombre = p.Nombre, PrecioUnitario = p.Precio, Cantidad = i.Cantidad });
        }
        pedido.Total = pedido.Detalles.Sum(d => d.PrecioUnitario * d.Cantidad);
        _db.Pedidos.Add(pedido);
        await _db.SaveChangesAsync();
        HttpContext.Session.Remove(Key);
        return RedirectToAction("Ticket", "Pedidos", new { id = pedido.Id });
    }
}
