using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace DigitizedDallet.Controllers;

public class LanguageController : Controller
{
    [HttpPost]
    public IActionResult SetLanguage(string culture)
    {
        Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });        

        var referer = Request.GetTypedHeaders().Referer;
        return referer != null ? Redirect(referer.ToString()) : RedirectToAction("Index");
    }
}