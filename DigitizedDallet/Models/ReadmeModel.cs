namespace DigitizedDallet.Models;

public class ReadmeModel
{
    public DateTime LastModificationDate { get; set; }
    public int DataModelVersion { get; set; } = 8;
    public string FileDescription { get; set; } = "Dallet's dictionary of Kabyle, digitized & structured";
    public string FileUrl { get; set; } = "https://github.com/sferhah/DigitizedDallet/blob/master/DigitizedDallet/wwwroot/dictionary.json";

    public string ProjectUrl { get; set; } = "https://github.com/sferhah/DigitizedDallet";
    public string ProjectWebsiteDemoUrl { get; set; } = "http://digitized-dallet.com";
    public string ProjectCreatorEmail { get; set; } = "sferhah@gmail.com";

    public string[] ToDoTasks { get; set; } = new string[]
    {   
        "restore all article.dalletNames",
        "create articles for annexed state"
    };
}

public class NonUnicodeCharacterSet
{
    public string C_WITH_DOT_BELOW { get; set; } = "ċ";
    public string K_WITH_DOT_ABOVE { get; set; } = "ḱ";
    public string J_WITH_DOT_BELOW { get; set; } = "ɉ";
    public string Z_WITH_CEDILLA_BELOW { get; set; } = "ž";
    public string Z_WITH_CEDILLA_AND_DOT_BELOW { get; set; } = "ż";
    public string S_WITH_CEDILLA_AND_DOT_BELOW { get; set; } = "ṥ";
    public string D_WITH_LINE_AND_DOT_BELOW { get; set; } = "ḓ";
}
