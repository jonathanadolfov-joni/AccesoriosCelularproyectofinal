using AccesoriosCelular.Data;
using AccesoriosCelular.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace AccesoriosCelular.Controllers;

[Authorize]
public class ProductosController : Controller
{
    private readonly AppDbContext _db;
    public ProductosController(AppDbContext db) { _db = db; }

    private void CargarCategorias(int? sel = null) =>
        ViewBag.Categorias = new SelectList(_db.Categorias.ToList(), "Id", "Nombre", sel);

    public async Task<IActionResult> Index() =>
        View(await _db.Productos.Include(p => p.Categoria).OrderBy(p => p.Nombre).ToListAsync());

    [Authorize(Roles = "Admin")]
    public IActionResult Create() { CargarCategorias(); return View(new Producto()); }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Producto p)
    {
        if (!ModelState.IsValid) { CargarCategorias(p.CategoriaId); return View(p); }
        _db.Productos.Add(p);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var p = await _db.Productos.FindAsync(id);
        if (p == null) return NotFound();
        CargarCategorias(p.CategoriaId);
        return View(p);
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Producto p)
    {
        if (id != p.Id) return NotFound();
        if (!ModelState.IsValid) { CargarCategorias(p.CategoriaId); return View(p); }
        _db.Productos.Update(p);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Productos.Include(x => x.Categoria).FirstOrDefaultAsync(x => x.Id == id);
        return p == null ? NotFound() : View(p);
    }

    [HttpPost, ActionName("Delete"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var p = await _db.Productos.FindAsync(id);
        if (p != null) { _db.Productos.Remove(p); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
