#if DEBUG
using Microsoft.AspNetCore.Mvc;
using DigitizedDallet.ExcelTool;
using DigitizedDallet.Utils;
using DigitizedDallet.ViewModels;

namespace DigitizedDallet.Controllers;

public class ExampleController : Controller
{
    public ActionResult Index(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            id = "A";
        }

        ViewBag.Letter = id.First().ToString();
        ViewBag.Letters = DicoRepository.Doc.Letters.Select(x => x.Name).ToList();

        return View(DicoRepository.Doc.Articles.Where(x => x.Root.Letter.Name == id)
            .SelectMany(x => x.AllExamples
            .Select(y => new EditableExampleModel
            {
                ArticleName = x.Name,
                Id = y.Id,
                Text = y.DalletText,
                Checked = !string.IsNullOrWhiteSpace(y.DalletText),
                Translation = y.Translation,
                Literal = y.Literal,
            })));
    }

    public ActionResult Toggle(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            id = "A";
        }

        ViewBag.Letter = id.First().ToString();
        ViewBag.Letters = DicoRepository.Doc.Letters.Select(x => x.Name).ToList();

        return View(DicoRepository.Doc.Articles.Where(x => x.Root.Letter.Name == id)
            .SelectMany(x => x.AllExamples
            .Select(y => new EditableExampleModel
            {
                ArticleName = x.Name,
                Id = y.Id,
                Text = y.DalletText,
                New = string.IsNullOrWhiteSpace(y.DalletTextNew) ? y.DalletText : y.DalletTextNew,
                Checked = !string.IsNullOrWhiteSpace(y.DalletText),
                Translation = y.Translation,
                Literal = y.Literal,
            })));
    }

    public ActionResult Edit(string id)
    {
        if (id == null)
        {
            return new BadRequestResult();
        }

        var example = DicoRepository.Doc.Articles.SelectMany(x => x.AllExamples.Select(y => new EditableExampleModel
        {
            ArticleName = x.Name,
            Id = y.Id,
            Text = y.DalletText,
            Checked = !string.IsNullOrWhiteSpace(y.DalletText),
            Translation = y.Translation,
            Literal = y.Literal,
            Letter = x.Root.Letter.Name,
        })).SingleOrDefault(x => x.Id == id);

        if (example == null)
        {
            return NotFound();
        }

        return View(example);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(EditableExampleModel example)
    {
        var stored_example = DicoRepository.Doc.Articles.SelectMany(x => x.AllExamples).SingleOrDefault(x => x.Id == example.Id);

        if (stored_example == null)
        {
            return NotFound();
        }

        stored_example.DalletText = example.Text ?? string.Empty;

        DicoRepository.QuickSave();

        var article = DicoRepository.Doc.Articles.Single(x => x.AllExamples.Any(y => y.Id == example.Id));

        return View(new EditableExampleModel
        {
            ArticleName = article.Name,
            Id = stored_example.Id,
            Text = stored_example.DalletText,
            Checked = !string.IsNullOrWhiteSpace(stored_example.DalletText),
            Translation = stored_example.Translation,
            Literal = stored_example.Literal,
            Letter = article.Root.Letter.Name,
        });
    }


    [HttpPost]
    public ActionResult ToggleEdit(EditableExampleModel example)
    {
        var stored_example = DicoRepository.Doc.Articles.SelectMany(x => x.AllExamples).SingleOrDefault(x => x.Id == example.Id);

        if (stored_example == null)
        {
            return NotFound();
        }

        stored_example.DalletTextNew = example.Text;

        DicoRepository.QuickSave();

        var article = DicoRepository.Doc.Articles.Single(x => x.AllExamples.Any(y => y.Id == example.Id));

        return Redirect(Url.Action("Toggle", new { id = article.Root.Letter.Name }) + "#" + example.Id);
    }

    public ActionResult Export(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            id = "A";
        }

        var doc = DicoRepository.Doc;

        List<ExampleXlsxModel> ls = new List<ExampleXlsxModel>();

        foreach (var article in doc.Articles.Where(x => x.Root.Letter.Name == id))
        {
            ls.AddRange(article.AllExamples
                .Select(example => new ExampleXlsxModel
                {
                    Id = example.Id,
                    Letter = article.Root.Letter.Name,
                    Root = article.Root.Name,
                    Entry = article.Name,
                    Phonetic = example.DalletTextNew ?? example.DalletText,
                    Fra = example.Translation,
                }));
        }

        var data = XlsReportExporter.ExportAsStream(ls).ToArray();

        return File(data,
            "application/vnd.openxmlformats",
            id + ".xlsx");
    }

}
#endif