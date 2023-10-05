namespace DigitizedDallet.Models;

public partial class ConjugationModel
{  
    public string? Note { get; set; }
    public string? AmyagId { get; set; }    
    public List<ConjugationIntensiveSectionModel> IntensiveForms { get; set; } = new List<ConjugationIntensiveSectionModel>();   
    public ConjugationAspectModel? Imperative { get; set; }
    public ConjugationAspectModel? Preterite { get; set; }  
    public ConjugationAspectModel? NegativePreterite { get; set; }
}
