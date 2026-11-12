using AccesoriosCelular.Data;
using AccesoriosCelular.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication("Cookies").AddCookie("Cookies", o =>
{
    o.LoginPath = "/Account/Login";
    o.AccessDeniedPath = "/Account/AccesoDenegado";
});

var app = builder.Build();

// Crea la BD y los datos iniciales la primera vez
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    // Si la BD ya existía de antes, crea las tablas nuevas de pedidos
    db.Database.ExecuteSqlRaw(@"IF OBJECT_ID(N'dbo.Pedidos') IS NULL CREATE TABLE dbo.Pedidos (Id INT IDENTITY PRIMARY KEY, UsuarioId INT NOT NULL REFERENCES dbo.Usuarios(Id), Fecha DATETIME2 NOT NULL, Total DECIMAL(10,2) NOT NULL)");
    db.Database.ExecuteSqlRaw(@"IF OBJECT_ID(N'dbo.DetallesPedido') IS NULL CREATE TABLE dbo.DetallesPedido (Id INT IDENTITY PRIMARY KEY, PedidoId INT NOT NULL REFERENCES dbo.Pedidos(Id) ON DELETE CASCADE, ProductoId INT NOT NULL, Nombre NVARCHAR(MAX) NOT NULL, PrecioUnitario DECIMAL(10,2) NOT NULL, Cantidad INT NOT NULL)");

    if (!db.Categorias.Any())
    {
        db.Categorias.AddRange(
            new Categoria { Nombre = "Fundas" },
            new Categoria { Nombre = "Cargadores y cables" },
            new Categoria { Nombre = "Audífonos" },
            new Categoria { Nombre = "Protectores de pantalla" },
            new Categoria { Nombre = "Soportes" },
            new Categoria { Nombre = "Baterías externas" });
        db.SaveChanges();
        db.Productos.AddRange(
            new Producto { Nombre = "Funda transparente antigolpes", Descripcion = "Esquinas reforzadas", Precio = 129, Stock = 40, CategoriaId = 1 },
            new Producto { Nombre = "Cable USB-C 1 m", Descripcion = "Carga rápida y datos", Precio = 89, Stock = 60, CategoriaId = 2 },
            new Producto { Nombre = "Audífonos inalámbricos", Descripcion = "Bluetooth 5.3", Precio = 349, Stock = 25, CategoriaId = 3 },
            new Producto { Nombre = "Mica de cristal templado", Descripcion = "Dureza 9H", Precio = 99, Stock = 80, CategoriaId = 4 });
        db.SaveChanges();
    }
    if (!db.Usuarios.Any())
    {
        var h = new PasswordHasher<Usuario>();
        var admin = new Usuario { Nombre = "Administrador", Correo = "admin@tienda.com", Rol = "Admin" };
        admin.PasswordHash = h.HashPassword(admin, "Admin123!");
        db.Usuarios.Add(admin);
        db.SaveChanges();
    }
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
