using Microsoft.AspNetCore.Mvc;
namespace AccesoriosCelular.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
