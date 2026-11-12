using System.ComponentModel.DataAnnotations.Schema;
namespace AccesoriosCelular.Models;

public class Pedido
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public DateTime Fecha { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal Total { get; set; }
    public List<DetallePedido> Detalles { get; set; } = new();
}

public class DetallePedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = "";
    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
}

public class ItemCarrito { public int ProductoId { get; set; } public int Cantidad { get; set; } }

public class LineaCarrito
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
    public decimal Subtotal => Precio * Cantidad;
}

public class TopProducto { public string Nombre { get; set; } = ""; public int Unidades { get; set; } }

public class DashboardVM
{
    public decimal TotalVentas { get; set; }
    public int Pedidos { get; set; }
    public int Usuarios { get; set; }
    public int Productos { get; set; }
    public List<TopProducto> Top { get; set; } = new();
    public List<Producto> StockBajo { get; set; } = new();
}
