namespace DigitizedDallet.Models;

public partial class ConjugationAspectModel
{    
    public List<ArticleModel> FirstSingularForms { get; set; } = new List<ArticleModel>();
    public List<ArticleModel> SecondSingularForms { get; set; } = new List<ArticleModel>();
    public List<ArticleModel> ThirdSingularForms { get; set; } = new List<ArticleModel>();
    public List<ArticleModel> ThirdSingularFeminineForms { get; set; } = new List<ArticleModel>();
    public List<ArticleModel> FirstPluralForms { get; set; } = new List<ArticleModel>();
    public List<ArticleModel> SecondPluralForms { get; set; } = new List<ArticleModel>();
    public List<ArticleModel> SecondPluralFeminineForms { get; set; } = new List<ArticleModel>();
    public List<ArticleModel> ThirdPluralForms { get; set; } = new List<ArticleModel>();
    public List<ArticleModel> ThirdPluralFeminineForms { get; set; } = new List<ArticleModel>();
}