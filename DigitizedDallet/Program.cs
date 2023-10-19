using DigitizedDallet;
using DigitizedDallet.Helpers;
using DigitizedDallet.Utils;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddMvc(options =>
{
    options.Filters.Add<ViewBagActionFilter>();
}).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
.AddDataAnnotationsLocalization();

var supportedScripts
#if DEBUG
    = new string[] { "Latn", "Tfng", "Arab", "Hebr", "Cyrl", "Grek" };
#else
   = new string[] { "Latn" };
#endif
var defaultSupportedScript = supportedScripts.First();

var supportedCultures
#if DEBUG
    = new string[] { "en", "fr", "kab", "ar" };
#else
   = new string[] { "fr" };
#endif

var cultures = new List<CultureInfo>();
foreach(var supportedCulture in supportedCultures)
{
    try
    {
        cultures.Add(new CultureInfo(supportedCulture));
    }
    catch
    {
    //https://stackoverflow.com/questions/24332304/why-do-i-get-culture-is-not-supported-and-what-if-anything-should-i-do-abou
    }    
}

supportedCultures = cultures.Select(x=> x.Name).ToArray();

var defaultSupportedCulture = supportedCultures.First();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture(defaultSupportedCulture)
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

var requestLocalizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(supportedCultures.First()),
    SupportedCultures = cultures,
    SupportedUICultures = cultures,
    ApplyCurrentCultureToResponseHeaders = true, // ?
};

requestLocalizationOptions.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider());
app.UseRequestLocalization(requestLocalizationOptions); // needs to be after UseRouting

app.UseAuthentication();
app.UseAuthorization();

// I couldn't achieve functionality with the framework, so I resorted to doing it manually.
app.Use(async (context, next) =>
{
    var cookieOrBrowserCulture = context.GetCookieOrBrowserCulture(supportedCultures) ?? defaultSupportedCulture;
    var cookieScript = context.GetCookieScript(supportedScripts) ?? defaultSupportedScript;

    if (!context.Request.Path.HasValue || context.Request.Path == "/")
    {
        var pathBase = context.Request.PathBase.ToUriComponent() ?? string.Empty;
        context.Response.Redirect($"{pathBase}/{cookieOrBrowserCulture}/{cookieScript}/Home/Index", false);
        return;
    }

    if (context.Request.Path.HasValue)
    {
        var routedCulture = context.Request.RouteValues["culture"] as string;
        if (routedCulture != null && supportedCultures.Contains(routedCulture))
        {
            if (routedCulture != cookieOrBrowserCulture)
            { 
                context.SaveCookieCulture(routedCulture);
            }
        }
        
        var routedScript = context.Request.RouteValues["script"] as string;
        if (routedScript != null && supportedScripts.Contains(routedScript))
        {
            if (routedScript != cookieScript)
            {
                context.SaveCookieScript(routedScript);
            }
        }      
    }

    await next();
});

app.MapControllerRoute(name: "default", pattern: "{culture=en}/{script=Latn}/{controller=Home}/{action=Index}/{id?}");

DicoRepository.Init(app.Environment.WebRootPath);

app.Run();
