namespace DigitizedDallet.Models;

public partial class MeaningModel
{
    public string? WikidataId { get; set; }
    public string? SameAsId { get; set; } 
    public string? Case { get; set; }
    public List<string> Translations { get; set; } = new List<string>();
    public string? Note { get; set; }
    public List<ExampleModel> Examples { get; set; } = new List<ExampleModel>();
    public List<MeaningModel> Meanings { get; set; } = new List<MeaningModel>();
}
