using AccesoriosCelular.Data;
using AccesoriosCelular.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace AccesoriosCelular.Controllers;

[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) { _db = db; }

    public async Task<IActionResult> Index()
    {
        var vm = new DashboardVM
        {
            TotalVentas = await _db.Pedidos.SumAsync(p => (decimal?)p.Total) ?? 0,
            Pedidos = await _db.Pedidos.CountAsync(),
            Usuarios = await _db.Usuarios.CountAsync(),
            Productos = await _db.Productos.CountAsync(),
            Top = await _db.DetallesPedido.GroupBy(d => d.Nombre)
                .Select(g => new TopProducto { Nombre = g.Key, Unidades = g.Sum(d => d.Cantidad) })
                .OrderByDescending(x => x.Unidades).Take(5).ToListAsync(),
            StockBajo = await _db.Productos.Where(p => p.Stock < 10).OrderBy(p => p.Stock).ToListAsync()
        };
        return View(vm);
    }
}
