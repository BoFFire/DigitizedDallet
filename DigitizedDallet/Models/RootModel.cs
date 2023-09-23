namespace DigitizedDallet.Models;

public class RootModel
{
    public string Name { get; set; } = null!;

   // [System.Text.Json.Serialization.JsonPropertyName("articles")]
    public List<ArticleModel> Articles { get; set; } = new List<ArticleModel>();

    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public LetterModel Letter { get; set; } = null!; 
}
