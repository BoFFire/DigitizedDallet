using DigitizedDallet.Utils;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace DigitizedDallet.Helpers;

public static class WikiHelper
{
    public static IEnumerable<string> ToDalletNotation(this IEnumerable<string> ls) => ls.Select(x => x.ToDalletNotation());

    public static string ToDalletNotation(this string text) =>
        text.Replace("ċ", "c̣")
              .Replace("ḱ", "k̇")
              .Replace("ɉ", "j̣")
              .Replace("ṥ", "ş̣")
              .Replace("ž", "z̧")
              .Replace("ż", "ż̧")
              .Replace("ḓ", "ḏ̣");
    


    public static IHtmlContent RenderWikiMarkup(this IHtmlHelper htmlHelper, string? text)
    {
        if (text == null)
        {
            return new HtmlContentBuilder();
        }

        text = text.ToDalletNotation();

        var content = new HtmlContentBuilder();

        var lines = text.Split(new string[] { "<br />", "<br/>", "<br>" }, StringSplitOptions.None);

        for (int i = 0; i < lines.Length; i++)
        {
            htmlHelper.RenderPureWikiMarkup(content, lines[i]);

            if (i != lines.Length - 1)
            {
                content.AppendHtml("<br />");
            }
        }

        return content;
    }    

    public static IHtmlContent RenderPureWikiMarkup(this IHtmlHelper htmlHelper, HtmlContentBuilder content, string text)
    {   

        foreach (var innerLinks in GetTuples(text, '[', ']')) // Wikilinks
        {
            if (innerLinks.Success)
            {
                if (innerLinks.Value.Contains(':'))
                {
                    var splitted = innerLinks.Value.Split(':');
                    var namespace_link = splitted.First();
                    var value_link = splitted.Skip(1).FirstOrDefault() ?? string.Empty;

                    if (namespace_link.Equals("root", StringComparison.OrdinalIgnoreCase))
                    {
                        var root = value_link.ToUpper().Replace(" ", ""); // root:b c d => BCD
                        var letter = root.First().ToString(); // B
                        var linkText = string.Join(" ", root.ToLower().ToCharArray()); //b c d

                        if (root.Length == 1)
                        {
                            root = null;
                        }
                        
                        content.AppendHtml("<i>");
                        content.AppendHtml(htmlHelper.ActionLink(linkText, "Index", "Home", protocol: null, hostname: null, fragment: root, routeValues: new { id = letter }, htmlAttributes: null));
                        content.AppendHtml("</i>");
                    }
                }
                else
                {
                    content.AppendHtml("<i>");
                    content.AppendHtml(htmlHelper.ActionLink(innerLinks.Value, "Article", new { name = innerLinks.Value }));
                    content.AppendHtml("</i>");
                }
            }
            else
            {
                foreach (var italicsTuples in GetTuples(innerLinks.Value, '\'', '\''))
                {
                    if (italicsTuples.Success)
                    {
                        content.AppendHtml($"<i>{italicsTuples.Value}</i>");
                    }
                    else
                    {
                        foreach (var templateTuples in GetTuples(italicsTuples.Value, '{', '}'))
                        {
                            if (templateTuples.Success)
                            {
                                content.AppendHtml($"<u>{templateTuples.Value.ToLongName()}</u>");
                            }
                            else
                            {
                                content.Append(templateTuples.Value);
                            }
                        }
                    }
                }
            }
        }

        return content;
    }

    public static string ToLongName(this string key) => DicoRepository.Doc.Abbreviations.ContainsKey(key) ? DicoRepository.Doc.Abbreviations[key] : key;

    public class TupleResult
    {
        public required string Value { get; set; }
        public required bool Success { get; set; }
    }

    public static List<TupleResult> GetTuples(string text, char begin, char end)
    {
        var result = new List<TupleResult>();
        bool parsing_link = false;
        var builder = new StringBuilder();
        char? previous_c = null;

        foreach (var c in text)
        {
            if (c == begin
                && previous_c == begin
                && !parsing_link)
            {
                var value = builder.ToString();
                result.Add(new TupleResult 
                { 
                    Value = value[0..^1],
                    Success = parsing_link 
                });
                builder.Clear();

                parsing_link = true;
            }
            else if (c == end
                && previous_c == end
                && parsing_link)
            {
                var value = builder.ToString();
                result.Add(new TupleResult 
                { 
                    Value = value[0..^1],
                    Success = parsing_link 
                });
                builder.Clear();
                parsing_link = false;
            }
            else
            {
                builder.Append(c);
            }

            previous_c = c;
        }

        result.Add(new TupleResult 
        { 
            Value = builder.ToString(),
            Success = false 
        });

        return result;
    }
}
