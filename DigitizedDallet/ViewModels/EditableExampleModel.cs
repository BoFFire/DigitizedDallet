namespace DigitizedDallet.ViewModels;

public class EditableExampleModel
{
    public required string ArticleName { get; init; }
    public required string Id { get; init; }
    public string? Text { get; set; }
    public string? Translation { get; set; }
    public string? Literal { get; set; }
    public string? Letter { get; set; }
    public bool Checked { get; set; }
}

