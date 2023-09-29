using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

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

        if (referer != null)
        {
            referer = referer.SetParameter("culture", culture);
        }

        return referer != null ? Redirect(referer.ToString()) : RedirectToAction("Index");
    }
}


public static class UrlExtensions
{
    public static string SetUrlParameter(this string url, string paramName, string value)
    {
        return new Uri(url).SetParameter(paramName, value).ToString();
    }

    public static Uri SetParameter(this Uri url, string paramName, string value)
    {
        var queryParts = HttpUtility.ParseQueryString(url.Query);
        queryParts[paramName] = value;
        return new Uri(url.AbsoluteUriExcludingQuery() + '?' + queryParts.ToString());
    }

    public static string AbsoluteUriExcludingQuery(this Uri url)
    {
        return url.AbsoluteUri.Split('?').FirstOrDefault() ?? String.Empty;
    }
}