namespace DigitizedDallet.Models;

public class DocumentModel
{
    public ReadmeModel Readme { get; set; } = new ReadmeModel();
    public NonUnicodeCharacterSet NonUnicodeCharacterSet { get; set; } = new NonUnicodeCharacterSet();
    public Dictionary<string, string> Abbreviations { get; set; } = new Dictionary<string, string>();    
    public List<LetterModel> Letters { get; set; } = new List<LetterModel>();

    private List<ArticleModel>? _Articles;
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public List<ArticleModel> Articles => _Articles ??= Letters.SelectMany(x => x.Roots)
        .SelectMany(x => x.Articles)
        .Select(x => new List<ArticleModel> { x }.Concat(x.AllForms))
        .SelectMany(x => x)
        .ToList();


    private List<ArticleModel>? _MainArticlesWithAlternatives;
    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public List<ArticleModel> ArticlesWithoutRedirects => _MainArticlesWithAlternatives ??=
        (Articles
        .Where(x => x.RedirectTo == null)
        .ToList());


    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public List<ArticleModel> MainArticles => Letters.SelectMany(x => x.Roots).SelectMany(x => x.Articles).Where(x => !x.IsRedirected).ToList();

    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public Dictionary<string, ArticleModel> ArticlesById { get; } = new Dictionary<string, ArticleModel>();

    public ArticleModel? GetArticle(string id) => ArticlesById.TryGetValue(id, out var article) ? article : null;

}
