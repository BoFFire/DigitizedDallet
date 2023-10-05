using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DigitizedDallet.Models;
using DigitizedDallet.Utils;
using DigitizedDallet.ViewModels;
using Microsoft.AspNetCore.Localization;

namespace DigitizedDallet.Controllers;

public class HomeController : Controller
{
    static public DocumentModel Doc => DicoRepository.Doc;
    public string RequestedLanguage => Request.HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture.Name ?? "en";

    public IActionResult About()
    {
        ViewData["Message"] = "Your application description page.";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    public ActionResult Index(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            id = TempData["letter"]?.ToString();
        }

        if (string.IsNullOrWhiteSpace(id))
        {
            id = "A";
        }

        ViewBag.Letter = id.First().ToString();

        if (id.Length > 1)
        {
            var path = Url.Action("Index", "Home", new { id = ViewBag.Letter })!;

            path = string.Join("/", path.Split("/").Select(s => System.Net.WebUtility.UrlEncode(s))) + "#" + System.Net.WebUtility.UrlEncode(id);

            return Redirect(path);
        }

        ViewBag.Letters = Doc.Letters.Select(x => x.Name).ToList();
        ViewBag.RequestedLanguage = this.RequestedLanguage;

        return View(Doc.Letters.Where(x => x.Name == id).First());
    }

    public ActionResult Article(string? value, string? name, string? guid)
    {
        if (!string.IsNullOrEmpty(value))
        {
            name = value;
        }

        if (name == null)
        {
            return new BadRequestResult();
        }

        name = name.ToLower().Trim();

        var articles = Doc.Articles.Where(x => x.Name == name && !x.IsRedirected).ToList();

        if (!string.IsNullOrWhiteSpace(guid))
        {
            articles = articles.Where(x => x.Id == guid).ToList();
        }

        if (!articles.Any())
        {
            return NotFound();
        }

        ViewBag.RequestedLanguage = this.RequestedLanguage;

        return View(articles);
    }

#if DEBUG
    public IActionResult Report() => View(DicoRepository.GenerateReport());

    public IActionResult Reload()
    {
        DicoRepository.Reset();

        var referer = Request.GetTypedHeaders().Referer;

        return referer != null ? Redirect(referer.ToString()) : RedirectToAction("Index");
    }

    public ActionResult Edit(string id)
    {
        if (id == null)
        {
            return new BadRequestResult();
        }

        var entry = Doc.Articles.SingleOrDefault(x => x.Id == id);

        if (entry == null)
        {
            return NotFound();
        }

        return View(entry);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(ArticleModel entry)
    {
        if (!ModelState.IsValid)
        {
            return View(entry);
        }

        var stored_article = Doc.Articles.SingleOrDefault(x => x.Id == entry.Id);

        if (stored_article == null)
        {
            return NotFound();
        }

        stored_article.Annexation = entry.Annexation;
        stored_article.Prefix = entry.Prefix;
        stored_article.Name = entry.Name;
        stored_article.Nature = entry.Nature;
        stored_article.Note = entry.Note;
        stored_article.Gender = entry.Gender;
        stored_article.Info = entry.Info;

        stored_article.See = entry.See;

        if (!string.IsNullOrEmpty(entry.RedirectToId?.Trim()))
        {
            var target_article = Doc.Articles.SingleOrDefault(x => x.Id == entry.RedirectToId);

            if (target_article == null)
            {
                return NotFound();
            }

            stored_article.RedirectToId = entry.RedirectToId;

            stored_article.Duplicates = 0;
            target_article.Duplicates -= 1;


            stored_article.Annexation = null;
            stored_article.Prefix = null;
            stored_article.Nature = null;
            stored_article.Note = null;
            stored_article.Gender = null;
            stored_article.Info = null;
            stored_article.See = null;

            stored_article.EtymologicalReferences.Clear();

            stored_article.Conjugations.Clear();
            stored_article.Meanings.Clear();
            stored_article.AlternativeForms.Clear();
            stored_article.SubArticles.Clear();
            stored_article.PluralForms.Clear();
            stored_article.FemininePluralForms.Clear();
            stored_article.FeminineForms.Clear();
            stored_article.SingularForms.Clear();
            stored_article.DalletNames.Clear();
            stored_article.VerbalNouns.Clear();
        }


        DicoRepository.Save();

        return View(stored_article);

    }

    public ActionResult RedirectToHomonym(string id)
    {
        if (id == null)
        {
            return new BadRequestResult();
        }

        var stored_article = Doc.Articles.Single(x => x.Id == id);

        var target_article = Doc.Articles
            .FirstOrDefault(x => x.Name == stored_article.Name
                                && x.RedirectTo == null
                                && x != stored_article);

        if (target_article == null)
        {
            return NotFound();
        }

        stored_article.RedirectToId = target_article.Id;

        stored_article.Duplicates = 0;
        target_article.Duplicates -= 1;

        stored_article.Annexation = null;
        stored_article.Prefix = null;
        stored_article.Nature = null;
        stored_article.Note = null;
        stored_article.Gender = null;
        stored_article.Info = null;        
        stored_article.See = null;

        stored_article.EtymologicalReferences.Clear();

        stored_article.Conjugations.Clear();
        stored_article.Meanings.Clear();
        stored_article.AlternativeForms.Clear();
        stored_article.SubArticles.Clear();
        stored_article.PluralForms.Clear();
        stored_article.FemininePluralForms.Clear();
        stored_article.FeminineForms.Clear();
        stored_article.SingularForms.Clear();
        stored_article.DalletNames.Clear();
        stored_article.VerbalNouns.Clear();

        DicoRepository.Save();

        return RedirectToAction("Article", "Home", new { name = stored_article.Name, culture = RequestedLanguage });
    }

    [HttpPost]
    public ActionResult QuickFix(string? id)
    {
        var stored_article = DicoRepository.Doc.Articles.SingleOrDefault(x => x.Id == id);

        if (stored_article == null)
        {
            return NotFound();
        }

        if (stored_article.DalletNames.Count != 1)
        {
            return BadRequest();
        }

        var dalletName = stored_article.DalletNames.First();

        if (dalletName.Contains("tţ"))
        {
            dalletName = dalletName.Replace("tţ", "ţ");
        }
        else
        {
            dalletName = "e" + dalletName;
        }


        stored_article.DalletNames = new List<string> { dalletName };

        DicoRepository.QuickSave();

        return Ok();
    }


    [HttpPost]
    public ActionResult AddDallet(string? id)
    {
        var stored_article = DicoRepository.Doc.Articles.SingleOrDefault(x => x.Id == id);

        if (stored_article == null)
        {
            return NotFound();
        }

        stored_article.DalletNames = new List<string> { stored_article.Name };

        DicoRepository.QuickSave();

        return Ok();
    }


    [HttpPost]
    public ActionResult ToggleEdit(EditableArticleModel input)
    {
        var ids = input.Id.Split("+");
        var id = ids[0];
        var index = int.Parse(ids[1]);

        var stored_article = DicoRepository.Doc.Articles.SingleOrDefault(x => x.Id == id);

        if (stored_article == null)
        {
            return NotFound();
        }

        stored_article.DalletNames[index] = input.Text;

        DicoRepository.QuickSave();

        return Ok();
    }
#endif
}
