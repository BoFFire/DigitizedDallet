namespace DigitizedDallet.Models;

public partial class ConjugationModel
{
    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public IEnumerable<ArticleModel> IntensiveThirdSingularForms
        => IntensiveForms.SelectMany(x=> x.IntensiveAorist?.ThirdSingularForms ?? Enumerable.Empty<ArticleModel>());
    
    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public IEnumerable<ArticleModel> IntensiveThirdPluralForms
        => IntensiveForms.SelectMany(x => x.IntensiveAorist?.ThirdPluralForms ?? Enumerable.Empty<ArticleModel>());
    
    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public IEnumerable<ArticleModel> IntensiveThirdSingularFeminineForms
        => IntensiveForms.SelectMany(x => x.IntensiveAorist?.ThirdSingularFeminineForms ?? Enumerable.Empty<ArticleModel>());

    // max 1
    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public IEnumerable<ArticleModel> PreteriteThirdPluralForms => Preterite?.ThirdPluralForms ?? Enumerable.Empty<ArticleModel>();

    // max 1
    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public IEnumerable<ArticleModel> PreteriteThirdSingularFeminineForms => Preterite?.ThirdSingularFeminineForms ?? Enumerable.Empty<ArticleModel>();

    // 1 instance having 2 :  yessemlal - isemlal
    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public IEnumerable<ArticleModel> PreteriteThirdSingularForms => Preterite?.ThirdSingularForms ?? Enumerable.Empty<ArticleModel>();

    // 1 instance having 2
    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public IEnumerable<ArticleModel> PreteriteFirstSingularForms => Preterite?.FirstSingularForms ?? Enumerable.Empty<ArticleModel>();
    

    // 4 instances (3 having 2, 1 having 3)
    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public IEnumerable<ArticleModel> NegativePreteriteThirdSingularForms => NegativePreterite?.ThirdSingularForms ?? Enumerable.Empty<ArticleModel>();

    // 1 instance having 2
    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public IEnumerable<ArticleModel> NegativePreteriteThirdPluralForms => NegativePreterite?.ThirdPluralForms ?? Enumerable.Empty<ArticleModel>();


    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public AmyagPage? AmyagPage { get; set; }

    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public IEnumerable<ArticleModel> AllForms => IntensiveThirdSingularForms
            .Concat(IntensiveThirdPluralForms)
            .Concat(IntensiveThirdSingularFeminineForms)
            .Concat(PreteriteThirdPluralForms)
            .Concat(PreteriteThirdSingularFeminineForms)
            .Concat(PreteriteThirdSingularForms)
            .Concat(PreteriteFirstSingularForms)
            .Concat(NegativePreteriteThirdSingularForms)
            .Concat(NegativePreteriteThirdPluralForms)
            .Concat(ImperativeForms)
            .Concat(IntensiveImperativeForms);

    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public IEnumerable<ArticleModel> ImperativeForms 
        => (Imperative?.SecondPluralForms ?? Enumerable.Empty<ArticleModel>())
            .Concat(Imperative?.SecondPluralFeminineForms ?? Enumerable.Empty<ArticleModel>());

    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public IEnumerable<ArticleModel> IntensiveImperativeForms
        => IntensiveForms.SelectMany(x => x.IntensiveImperative?.SecondSingularForms ?? Enumerable.Empty<ArticleModel>());
}
