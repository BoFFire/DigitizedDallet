using DigitizedDallet.Models;

namespace DigitizedDallet.ViewModels;

public class ReportViewModel
{
    public List<string> InvalidLinks { get; set; } = new List<string>();
    public List<string> InvalidTemplates { get; set; } = new List<string>();

    public int MainArticlesCount { get; set; }

    public int ArticlesCount { get; set; }

    public int ArticlesWithDalletNamesCount { get; set; }
    public int ArticlesWithoutDalletNamesCount { get; set; }
    public int StandardizedFormsCount { get; set; }

    public int DistinctArticlesCount { get; set; }

    public int RedirectsCount { get; set; }

    public List<string> ArticlesWithSubArticles { get; set; } = new List<string>();

    public List<string> ArticlesWithRedirectedSubArticles { get; set; } = new List<string>();

    public List<string> ArticlesWithSubMeanings { get; set; } = new List<string>();
    public List<string> ArticlesWithCases { get; set; } = new List<string>();

    public List<string> ArticlesWithPunctuation { get; set; } = new List<string>();

    public List<string> ArticlesWithHyphen { get; set; } = new List<string>();

    public List<ArticleModel> ArticlesWithMark { get; set; } = new List<ArticleModel>();


    public int ArticlesWithMoreThanOneTranslationCount { get; set; }
    public List<ArticleModel> ArticlesWithMoreThanOneTranslationAndOnlyOneMeaning { get; set; } = new List<ArticleModel>();
    public List<ArticleModel> ArticlesWithMoreThanOneTranslationAndOnlyOneMeaningAndOneExample { get; set; } = new List<ArticleModel>();


    public List<string> ArticlesWithSpecialChars { get; set; } = new List<string>();
}