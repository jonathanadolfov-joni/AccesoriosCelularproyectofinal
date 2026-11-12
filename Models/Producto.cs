using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AccesoriosCelular.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public List<Producto> Productos { get; set; } = new();
}

public class Producto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Escribe el nombre")]
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    [Range(0.01, 999999, ErrorMessage = "Precio no válido"), Column(TypeName = "decimal(10,2)")]
    public decimal Precio { get; set; }
    [Range(0, 100000, ErrorMessage = "Stock no válido")]
    public int Stock { get; set; }
    public string? ImagenUrl { get; set; }
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
}
