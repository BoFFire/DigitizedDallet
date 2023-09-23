namespace DigitizedDallet.Models;

public class LetterModel
{
    public string Name { get; set; } = null!;
    public List<RootModel> Roots { get; set; } = new List<RootModel>();

    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public DocumentModel Doc { get; set; } = null!;
}
