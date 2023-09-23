namespace DigitizedDallet.Models;

public partial class ConjugationAspectModel
{
    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public IEnumerable<ArticleModel> AllForms => FirstSingularForms
      .Concat(SecondSingularForms)
      .Concat(ThirdSingularForms)
      .Concat(ThirdSingularFeminineForms)
      .Concat(FirstPluralForms)
      .Concat(SecondPluralForms)
      .Concat(SecondPluralFeminineForms)
      .Concat(ThirdPluralForms)
      .Concat(ThirdPluralFeminineForms);
}