using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DigitizedDallet.Utils;
using DigitizedDallet.ViewModels;

namespace DigitizedDallet.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Article", new { id = "A" });
    }

    public IActionResult About()
    {
        ViewData["Message"] = "Your application description page.";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

#if DEBUG
    public IActionResult Report() => View(DicoRepository.GenerateReport());

    public IActionResult Reload()
    {
        DicoRepository.Reset();
        return Ok();
    }  
#endif
}
