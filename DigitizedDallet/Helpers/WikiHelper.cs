using DigitizedDallet.Models;
using DigitizedDallet.Utils;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace DigitizedDallet.Helpers;

public static class WikiHelper
{
    public static string GetTransliteratedString(this string text, string? script)
    {
        return text;
        /*

#if !DEBUG
       return text;
#endif

        script ??= "Latn";

        if (script == "Latn")
        {
            return text;
        }

        switch (script)
        {
            case "Latn":
                script = "";
                break;
            case "Tfng":
                script = "Tifinagh";
                break;
            case "Arab":
                script = "Arabic";
                break;
            case "Hebr":
                script = "Hebrew";
                break;
            case "Cyrl":
                script = "Cyrillic";
                break;
            case "Grek":
                script = "Greek";
                break;
        }

        return TransliterationService.Transliterate(text, script);
        */
    }

    public static IEnumerable<string> ToDalletNotation(this IEnumerable<string> ls) => ls.Select(x => x.ToDalletNotation());

    public static string ToDalletNotation(this string text) =>
        text.Replace(DicoRepository.Doc.NonUnicodeCharacterSet.C_WITH_DOT_BELOW, "c̣")
              .Replace(DicoRepository.Doc.NonUnicodeCharacterSet.K_WITH_DOT_ABOVE, "k̇")
              .Replace(DicoRepository.Doc.NonUnicodeCharacterSet.J_WITH_DOT_BELOW, "j̣")
              .Replace(DicoRepository.Doc.NonUnicodeCharacterSet.S_WITH_CEDILLA_AND_DOT_BELOW, "ş̣")
              .Replace(DicoRepository.Doc.NonUnicodeCharacterSet.Z_WITH_CEDILLA_BELOW, "z̧")
              .Replace(DicoRepository.Doc.NonUnicodeCharacterSet.Z_WITH_CEDILLA_AND_DOT_BELOW, "ẓ̧")
              .Replace(DicoRepository.Doc.NonUnicodeCharacterSet.D_WITH_LINE_AND_DOT_BELOW, "ḏ̣");

    public static IHtmlContent RenderWikiMarkup(this IHtmlHelper htmlHelper, string? text)
    {
        if (text == null)
        {
            return new HtmlContentBuilder();
        }

        text = text.Replace("« ", "«\u00A0").Replace(" »", "\u00A0»").Replace(" ;", "\u00A0;").Replace(" !", "\u00A0!").Replace(" ?", "\u00A0?").Replace(" :", "\u00A0:");

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
        var requestedScript = htmlHelper.ViewBag.RequestedScript as string;

        foreach (var innerLink in GetTuples(text, '[', ']')) // Wikilinks
        {
            if (innerLink.Success)
            {
                if (innerLink.Value.Contains(':'))
                {
                    var splitted = innerLink.Value.Split(':');
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
                        content.AppendHtml(htmlHelper.ActionLink(linkText.GetTransliteratedString(requestedScript), "Index", "Article", protocol: null, hostname: null, fragment: root, routeValues: new { id = letter }, htmlAttributes: null));
                        content.AppendHtml("</i>");
                    }
                }
                else
                {
                    content.AppendHtml("<i>");
                    content.AppendHtml(htmlHelper.ActionLink(new ArticleModel { Name = innerLink.Value }, withDalletEdit: false));
                    content.AppendHtml("</i>");
                }
            }
            else
            {
                foreach (var italicsTuple in GetTuples(innerLink.Value, '\'', '\''))
                {
                    if (italicsTuple.Success)
                    {
                        content.AppendHtml($"<i>{italicsTuple.Value}</i>");
                    }
                    else
                    {
                        foreach (var templateTuples in GetTuples(italicsTuple.Value, '{', '}'))
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
