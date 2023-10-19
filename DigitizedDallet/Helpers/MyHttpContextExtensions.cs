using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Net.Http.Headers;

namespace DigitizedDallet.Helpers;

public static class MyHttpContextExtensions
{
    public static string? GetUserCulture(this HttpContext context)
        => context.Features.Get<IRequestCultureFeature>()?.RequestCulture.UICulture.Name;

    public static string? GetCookieOrBrowserCulture(this HttpContext context, string[] supportedCultures)
    {
        var cookie = context.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];

        string? cookieOrBrowserCulture = null;
        if (cookie != null)
        {
            var roviderCultureResult = CookieRequestCultureProvider.ParseCookieValue(cookie);
            if (roviderCultureResult != null)
            {
                cookieOrBrowserCulture = roviderCultureResult.UICultures.FirstOrDefault().Value;
            }

            if (!supportedCultures.Contains(cookieOrBrowserCulture))
            {
                cookieOrBrowserCulture = null;
            }
        }

        if (cookieOrBrowserCulture == null)
        {
            var userLangs = context.Request.Headers["Accept-Language"].ToString() ?? string.Empty;
            var languages = userLangs.Split(',')
              .Select(StringWithQualityHeaderValue.Parse)
              .OrderByDescending(s => s.Quality.GetValueOrDefault(1))
              .Select(x => x.Value.Split('-').FirstOrDefault()).Distinct();

            cookieOrBrowserCulture = languages.FirstOrDefault(language => supportedCultures.Contains(language));
        }

        return cookieOrBrowserCulture;
    }

    public static void SaveCookieCulture(this HttpContext context, string culture)
    {
        context.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
              CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(CultureInfo.GetCultureInfo(culture))),
              new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
    }

    public static readonly string DefaultCookieScriptName = ".DigitizedDallet.Script";

    public static string? GetCookieScript(this HttpContext context, string[] supportedScripts)
    {
        var cookieScript = context.Request.Cookies[DefaultCookieScriptName];

        if (!supportedScripts.Contains(cookieScript))
        {
            cookieScript = null;
        }

        return cookieScript;
    }

    public static void SaveCookieScript(this HttpContext context, string script)
    {
        context.Response.Cookies.Append(DefaultCookieScriptName, script, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1)
        });
    }

}
