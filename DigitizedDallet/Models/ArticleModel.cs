namespace DigitizedDallet.Models;

public partial class ArticleModel
{
    public string Id { get; set; } = null!;
    public string? RedirectToId { get; set; }
    public string? Nature { get; set; }
    public string? Prefix { get; set; }
    public string Name { get; set; } = null!;
    public string? Mark { get; set; }
    public string? Annexation { get; set; }

    // [System.Text.Json.Serialization.JsonPropertyName("dalletNames")]
    public List<string> DalletNames { get; set; } = new List<string>();
    
    public List<ArticleModel> AlternativeForms { get; set; } = new List<ArticleModel>();

    public string? Info { get; set; }
    public string? Note { get; set; }
    
    public List<string> EtymologicalReferences { get; set; } = new List<string>();
    public string? See { get; set; }

    public List<ConjugationModel> Conjugations { get; set; } = new List<ConjugationModel>();

    public List<ArticleModel> VerbalNouns { get; set; } = new List<ArticleModel>();
    public List<ArticleModel> PluralForms { get; set; } = new List<ArticleModel>();
    public List<ArticleModel> FeminineForms { get; set; } = new List<ArticleModel>();
    public List<ArticleModel> FemininePluralForms { get; set; } = new List<ArticleModel>();    

    public List<ArticleModel> SingularForms { get; set; } = new List<ArticleModel>();    

    public string? Compound { get; set; }
    public string? Gender { get; set; }
    public List<MeaningModel> Meanings { get; set; } = new List<MeaningModel>();    
    public List<ArticleModel> SubArticles { get; set; } = new List<ArticleModel>();    
}
