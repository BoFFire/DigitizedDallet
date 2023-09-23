#if DEBUG
using DigitizedDallet.Utils;
using Microsoft.AspNetCore.Mvc;

namespace DigitizedDallet.Controllers;

public class ConjugationController : Controller
{   
    public ActionResult Details(string id)
    {
        if (id == null)
        {
            return new BadRequestResult();
        }

        var page = DicoRepository.AmyagPages.Single(x => x.Id == id);

        return View(page);
    }
}
#endif