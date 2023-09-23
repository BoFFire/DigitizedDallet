using DigitizedDallet.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace DigitizedDallet.ExcelTool;

public static class XlsReportExporter
{
    public static MemoryStream ExportAsStream(List<ExampleXlsxModel> examples)
    {
        IWorkbook workbook = new XSSFWorkbook();
        var mainSheet = workbook.CreateSheet("Examples"
            , new string[] { "Id", "Letter", "Root", "Entry", "Kab", "Fra" }
            , new int[] { 2_000, 1_500, 1_500, 5_000, 30_000, 50_000 });

        examples.ForEach(w => mainSheet.CreateRowWithCells(w.Id, w.Letter, w.Root, w.Entry, w.Phonetic, w.Fra));


        var ms = new MemoryStream();
        workbook.Write(ms, false);
        return ms;
    }

    public static void Export(List<ExampleXlsxModel> examples, string path, bool withPhonetic = false)
    {
        IWorkbook workbook = ConvertToWorkBook(examples, withPhonetic);

        using var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        workbook.Write(file, false);
    }

    public static IWorkbook ConvertToWorkBook(List<ExampleXlsxModel> examples, bool withPhonetic = false)
    {
        IWorkbook workbook = new XSSFWorkbook();
        var mainSheet = withPhonetic ?
            workbook.CreateSheet("Examples"
            , new string[] { "Id", "Type", "Kab", "Fra" }
            , new int[] { 2_000, 1000, 30_000, 50_000 })
            : workbook.CreateSheet("Examples"
            , new string[] { "Id", "Letter", "Root", "Entry", "Kab", "Fra" }
            , new int[] { 2_000, 1_500, 1_500, 5_000, 30_000, 50_000 });

        if (withPhonetic)
        {
            foreach (ExampleXlsxModel w in examples)
            {
                mainSheet.CreateRowWithCells(w.Id, "P", w.Phonetic, w.Fra);
                mainSheet.CreateRowWithCells(w.Id, "T", w.Kab, w.Fra);
            }
        }
        else
        {
            examples.ForEach(w => mainSheet.CreateRowWithCells(w.Id, w.Letter, w.Root, w.Entry, w.Kab, w.Fra));
        }

        return workbook;
    }


    public static void Export(List<ConjugationXlsxModel> items, string path, bool withParsing = false)
    {
        IWorkbook workbook = new XSSFWorkbook();
        var mainSheet = workbook.CreateSheet("Examples"
            , new string[] { "Name", "Id", "Raw", "AmyagId", "Intensive", "Preterite", "Preterite_1st", "Nagative" }
            , new int[] { 5_000, 2_000, 15_000, 3_000, 10_000, 5_000, 5_000, 5_000 });

        if (withParsing)
        {
            foreach (ConjugationXlsxModel w in items)
            {
                mainSheet.CreateRowWithCells(w.Name, w.Id, w.Raw, w.AmyagId, w.Intensive_Amyag, w.Preterite_Amyag, w.Preterite_1st_Amyag, w.Nagative_Amyag);
              //  mainSheet.CreateRowWithCells(w.Name, w.Id, w.Raw, w.AmyagId, w.Intensive, w.ParsedPreterite?.Third ?? w.ParsedPreterite?.Raw, w.ParsedPreterite?.FirstPerson, w.ParsedPreterite?.Nagative);
            }
        }
        else
        {
            items.ForEach(w => mainSheet.CreateRowWithCells(w.Name, w.Id, w.Raw, w.AmyagId, w.Intensive_Amyag, w.Preterite_Amyag, w.Preterite_1st_Amyag, w.Nagative_Amyag));
        }

        using var file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        workbook.Write(file, false);
    }
}

public class ExampleXlsxModel
{
    public required string Id { get; set; }
    public required string Letter { get; set; }
    public required string Root { get; set; }
    public required string Entry { get; set; }
    public required string Phonetic { get; set; }
    public string? Type { get; set; }
    public string? Kab { get; set; }
    public required string Fra { get; set; }
}


public class ConjugationXlsxModel
{   
    public string Name { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string? Raw { get; set; }
    public string? Intensive { get; set; }
    public string? Preterite { get; set; }
    public string? Intensive_Amyag { get; set; }
    public string? Preterite_Amyag { get; set; }
    public string? Preterite_1st_Amyag { get; set; }
    public string? Nagative_Amyag { get; set; }
    public string? AmyagId { get; set; }
    public bool IntensiveIsDiff { get; set; }
    public AmyagPage? AmyagPage { get; set; }
}