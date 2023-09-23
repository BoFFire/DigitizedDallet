namespace DigitizedDallet.Models;

public class ReadmeModel
{
    public int DataModelVersion { get; set; } = 1;
    public string FileDescription { get; set; } = "Dallet's dictionary of Kabyle, digitized & structured";
    public string FileUrl { get; set; } = "https://github.com/sferhah/DigitizedDallet/blob/master/DigitizedDallet/wwwroot/dictionary.json";

    public string ProjectUrl { get; set; } = "https://github.com/sferhah/DigitizedDallet";
    public string ProjectWebsiteDemoUrl { get; set; } = "http://digitized-dallet.com";
    public string ProjectCreatorEmail { get; set; } = "sferhah@gmail.com";

    public string[] ToDoTasks { get; set; } = new string[]
    {
        "occlusives are missing in example.dalletText",
        "restore all article.dalletNames",
        "create articles for annexed state",
        "combine origin w/ biliography or create congnate list"
    };
}
