using System.Net.Http.Headers;
using System.Net;
using System.Text;

namespace DigitizedDallet.Utils;

public static class AmyagApiClient
{
    static readonly string _uri = "aHR0cHM6Ly9pYjNpdWt4MjA2LWRzbi5hbGdvbGlhLm5ldC8xL2luZGV4ZXMvcHJvZF9hbXlhZy9xdWVyeT94LWFsZ29saWEtYWdlbnQ9QWxnb2xpYSUyMGZvciUyMHZhbmlsbGElMjBKYXZhU2NyaXB0JTIwMy4yMS4xJngtYWxnb2xpYS1hcHBsaWNhdGlvbi1pZD1JQjNJVUtYMjA2JngtYWxnb2xpYS1hcGkta2V5PWZmOTYwNWE1MWUyZjEzNDRhMjk4YzVhOGJhMTI5MGY2";    

    public static async Task<List<string>> FindAmyagId(string verb)
    {
        var decodedUri = Encoding.UTF8.GetString(Convert.FromBase64String(_uri));

        var content = new StringContent("{\"params\":\"query=" + verb + "&hitsPerPage=20&typoTolerance=false&minWordSizefor1Typo=6\"}", System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
        
        var resp = await HttpClient.PostAsync(decodedUri, content);

        var stringRespons = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

        var obj = CustomJsonSerializer.DeserializeObject<RootObject>(stringRespons);

        return obj!.hits.Where(x => x.verb == verb).Select(x => x.objectID).ToList() ?? new List<string>();
    }

    static HttpClient? httpClient;
    public static HttpClient HttpClient
    {
        get
        {
            if(httpClient != null)
            {
                return httpClient;
            }

            httpClient = new HttpClient(new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            })
            {
                Timeout = new TimeSpan(0, 3, 0),
            };

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.AcceptEncoding.Clear();
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            httpClient.DefaultRequestHeaders.UserAgent.Clear();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.130 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "cross-site");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "https://www.amyag.com/f/NTA4Ng/xdem");


            httpClient.DefaultRequestHeaders.Add("Origin", "https://www.amyag.com");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }
    }
   
}

public class Verb
{
    public string? value { get; set; }
    public string? matchLevel { get; set; }
    public bool fullyHighlighted { get; set; }
    public List<string> matchedWords { get; set; } = new List<string>();
}

public class Themes
{
    public string? value { get; set; }
    public string? matchLevel { get; set; }
    public bool fullyHighlighted { get; set; }
    public List<string> matchedWords { get; set; } = new List<string>();
}

public class Senses
{
    public string? value { get; set; }
    public string? matchLevel { get; set; }
    public List<object> matchedWords { get; set; } = new List<object>();
}

public class HighlightResult
{
    public Verb? verb { get; set; }
    public Themes? themes { get; set; }
    public Senses? senses { get; set; }
}

public class Hit
{
    public string? id { get; set; }
    public string? verb { get; set; }
    public string? themes { get; set; }
    public string? rendred { get; set; }
    public string? senses { get; set; }
    public string objectID { get; set; } = null!;
    public HighlightResult? _highlightResult { get; set; }
}

public class RootObject
{
    public List<Hit> hits { get; set; } = new List<Hit>();
    public int nbHits { get; set; }
    public int page { get; set; }
    public int nbPages { get; set; }
    public int hitsPerPage { get; set; }
    public bool exhaustiveNbHits { get; set; }
    public string? query { get; set; }
    public string? @params { get; set; }
    public int processingTimeMS { get; set; }
}
