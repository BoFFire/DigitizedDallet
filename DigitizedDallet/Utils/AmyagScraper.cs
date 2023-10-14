using HtmlAgilityPack;
using DigitizedDallet.Models;

namespace DigitizedDallet.Utils;

public static class AmyagScraper
{
    public static AmyagPage Scrape(string id, string htmlText)
    {
        var doc = new HtmlDocument
        {
            OptionFixNestedTags = true
        };

        doc.LoadHtml(htmlText);

        var content = doc.DocumentNode.SelectSingleNode("//div[@id='content']");

        var amyagPage = new AmyagPage
        {
            Id = id,
            Name = content.SelectSingleNode(".//h3").FirstChild.GetDeEntitizedInnerText(),
            Translation = content.SelectSingleNode(".//div[@class='senses']").GetDeEntitizedInnerText().Trim() 
        };

        if (string.IsNullOrWhiteSpace(amyagPage.Translation))
        {
            amyagPage.Translation = null;
        }

        foreach (var sect in content.SelectSingleNode(".//div[@class='sections']").SelectNodes(".//div[@class='section']") ?? new HtmlNodeCollection(null))
        {
            var section = new AmyagIntensiveSection();

            foreach (var asp in (sect.SelectNodes(".//ul") ?? new HtmlNodeCollection(null)).Where(x => x.Attributes["data-aspect"] != null))
            {
                var conjugation = new Dictionary<string, string?>();

                foreach (var kvs in (asp.SelectNodes(".//li") ?? new HtmlNodeCollection(null))
                    .Where(x => x.Attributes["class"] == null)
                    .Select(li => li.SelectNodes("span")))
                {   
                    conjugation.Add(kvs[0].GetDeEntitizedInnerText(), kvs.Count > 1 ? kvs[1].GetDeEntitizedInnerText() : null);
                }

                var aspect = new AmyagAspect();
                aspect.Conjugate(conjugation);

                var aspectName = asp.Attributes["data-aspect"].Value;
                switch (aspectName)
                {
                    case "aorimp":
                        amyagPage.Imperative = aspect;
                        break;
                    case "aorfut":
                        amyagPage.Aorist = aspect;
                        amyagPage.Aorist.TrimFuture();
                        break;
                    case "pre":
                        amyagPage.Preterite = aspect;
                        break;
                    case "pren":
                        amyagPage.NegativePreterite = aspect;
                        amyagPage.NegativePreterite.TrimNegative();
                        break;
                    case "para":
                        amyagPage.AoristParticiple = conjugation.Conjugate("").SplitBySemicolon().Select(x=> x.TrimStart("ara ")).ToList();
                        break;
                    case "parpp":
                        amyagPage.PreteriteParticiple = conjugation.Conjugate("").SplitBySemicolon();
                        break;
                    case "parpn":
                        amyagPage.NegativePreteriteParticiple = conjugation.Conjugate("").SplitBySemicolon().TrimNegative();
                        break;
                    case "impit":
                        section.IntensiveImperative = aspect;
                        break;
                    case "aorit":
                        section.IntensiveAorist = aspect;
                        break;
                    case "paraitp":
                        section.IntensiveAoristParticiple = conjugation.Conjugate("").SplitBySemicolon();
                        break;
                    case "paraitn":
                        section.NegativeIntensiveAoristParticiple = conjugation.Conjugate("").SplitBySemicolon().TrimNegative();
                        break;
                    default:
                        throw new Exception();
                }
            }
            
            if (section.IntensiveImperative != null
                || section.IntensiveAorist != null

                || section.IntensiveAoristParticiple.Any()
                || section.NegativeIntensiveAoristParticiple.Any())
            {
                if(section.IntensiveImperative is null || section.IntensiveAorist is null)
                {
                    throw new Exception();
                }

                section.Name = sect.SelectSingleNode(".//h4")?.GetDeEntitizedInnerText() ?? throw new Exception();
                amyagPage.IntensiveForms.Add(section);
            }
        }     

        var notes = content.SelectSingleNode(".//ul[@class='notes']/li/strong");
        var regularityAndDerivation = notes.FirstChild.GetDeEntitizedInnerText();

        var span = notes.SelectSingleNode(".//span");

        if (span != null && span.GetDeEntitizedInnerText() != "0")
        {
            var a = span.SelectSingleNode(".//a");                
            var verbLink = a.Attributes["href"].Value.Replace("https://www.amyag.com", "").Replace("/f/","").Split('/');

            amyagPage.Pattern = new AmyagPattern
            {
                Id = verbLink[0],
                Verb = a.GetDeEntitizedInnerText(),
                Number = a.NextSibling.GetDeEntitizedInnerText().Replace("(", "").Replace(")", "").Trim(),
            };

            if (span.NextSibling != null)
            {
                regularityAndDerivation += span.NextSibling.GetDeEntitizedInnerText();
            }
        }

        var regularityAndDerivationArray = regularityAndDerivation.Split('-', StringSplitOptions.RemoveEmptyEntries);
     
        var regularity = regularityAndDerivationArray.First().Trim();        

        if (regularity != "verbe irrégulier"
            && regularity != "verbe régulier"
           /* && _regularity != "strong verb"
            && _regularity != "weak verb"
            && _regularity != "amyag arlugan"
            && _regularity != "amyag alugan"*/)
        {
            throw new Exception();
        }

        var derivation = regularityAndDerivationArray.Skip(1).FirstOrDefault()?.Trim();

        //if (_derivation is not null
        //    && _derivation != "dérivé")
        //{
        //    throw new Exception();
        //}

        amyagPage.IsIrregular = regularity == "verbe irrégulier" || regularity == "strong verb" || regularity == "amyag arlugan";
        amyagPage.IsDerived = derivation is not null;
        amyagPage.HasDirectionalParticle = (amyagPage.Imperative?.GetAll() ?? Enumerable.Empty<string>())
            .Concat(amyagPage.Preterite?.GetAll() ?? Enumerable.Empty<string>())
            .Any(x=> x.Contains('-'));

        if (amyagPage.HasDirectionalParticle)
        {
            amyagPage.AoristParticiple = amyagPage.AoristParticiple.TrimParticle();
            amyagPage.PreteriteParticiple = amyagPage.PreteriteParticiple.TrimParticle();
            amyagPage.NegativePreteriteParticiple = amyagPage.NegativePreteriteParticiple.TrimParticle();

            foreach (var aspect in amyagPage.GetAllAspects())
            {
                aspect.TrimParticle();
            }

            foreach (var intensiveForm in amyagPage.IntensiveForms)
            {
                intensiveForm.IntensiveAoristParticiple = intensiveForm.IntensiveAoristParticiple.TrimParticle();
                intensiveForm.NegativeIntensiveAoristParticiple = intensiveForm.NegativeIntensiveAoristParticiple.TrimParticle();
            }
        }

        return amyagPage;
    }
}


public static class HtmlAgilityPackExtensions
{
    public static string GetDeEntitizedInnerText(this HtmlNode item) => System.Web.HttpUtility.HtmlDecode(item.InnerText) ?? string.Empty;
}

public static class AmyagAspectExtensions
{
    public static List<string> SplitBySemicolon(this string? item) => item?.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new List<string>();

    public static void TrimNegative(this AmyagAspect aspect)
    {
        aspect.FirstSingular = aspect.FirstSingular.TrimNegative();
        aspect.SecondSingular = aspect.SecondSingular.TrimNegative();
        aspect.ThirdSingular = aspect.ThirdSingular.TrimNegative();
        aspect.ThirdSingularFeminine = aspect.ThirdSingularFeminine.TrimNegative();
        aspect.FirstPlural = aspect.FirstPlural.TrimNegative();
        aspect.SecondPlural = aspect.SecondPlural.TrimNegative();
        aspect.SecondPluralFeminine = aspect.SecondPluralFeminine.TrimNegative();
        aspect.ThirdPlural = aspect.ThirdPlural.TrimNegative();
        aspect.ThirdPluralFeminine = aspect.ThirdPluralFeminine.TrimNegative();
    }

    public static void TrimFuture(this AmyagAspect aspect)
    {
        aspect.FirstSingular = aspect.FirstSingular.TrimFuture();
        aspect.SecondSingular = aspect.SecondSingular.TrimFuture();
        aspect.ThirdSingular = aspect.ThirdSingular.TrimFuture();
        aspect.ThirdSingularFeminine = aspect.ThirdSingularFeminine.TrimFuture();
        aspect.FirstPlural = aspect.FirstPlural.TrimFuture();
        aspect.SecondPlural = aspect.SecondPlural.TrimFuture();
        aspect.SecondPluralFeminine = aspect.SecondPluralFeminine.TrimFuture();
        aspect.ThirdPlural = aspect.ThirdPlural.TrimFuture();
        aspect.ThirdPluralFeminine = aspect.ThirdPluralFeminine.TrimFuture();
    }

    public static void TrimParticle(this AmyagAspect aspect)
    {
        aspect.FirstSingular = aspect.FirstSingular.TrimParticle();
        aspect.SecondSingular = aspect.SecondSingular.TrimParticle();
        aspect.ThirdSingular = aspect.ThirdSingular.TrimParticle();
        aspect.ThirdSingularFeminine = aspect.ThirdSingularFeminine.TrimParticle();
        aspect.FirstPlural = aspect.FirstPlural.TrimParticle();
        aspect.SecondPlural = aspect.SecondPlural.TrimParticle();
        aspect.SecondPluralFeminine = aspect.SecondPluralFeminine.TrimParticle();
        aspect.ThirdPlural = aspect.ThirdPlural.TrimParticle();
        aspect.ThirdPluralFeminine = aspect.ThirdPluralFeminine.TrimParticle();
    }

    public static List<string> TrimNegative(this List<string> items) => items.Select(x => x.TrimNegative()).ToList();
    public static string TrimNegative(this string item) => item.TrimStart("ur ").TrimEnd(" ara");

    public static List<string> TrimFuture(this List<string> items) => items.Select(x => x.TrimFuture()).ToList();
    public static string TrimFuture(this string item) => item.TrimStart("ad ").TrimStart("a ");

    public static List<string> TrimParticle(this List<string> items) => items.Select(x => x.TrimParticle()).ToList();
    public static string TrimParticle(this string item) => item.TrimStart("d-").TrimEnd("-d");

    public static string TrimStart(this string target, string trimString)
    {
        if (string.IsNullOrEmpty(trimString)) return target;

        string result = target;
        while (result.StartsWith(trimString))
        {
            result = result.Substring(trimString.Length);
        }

        return result;
    }

    public static string TrimEnd(this string target, string trimString)
    {
        if (string.IsNullOrEmpty(trimString)) return target;

        string result = target;
        while (result.EndsWith(trimString))
        {
            result = result.Substring(0, result.Length - trimString.Length);
        }

        return result;
    }

    public static void Conjugate(this AmyagAspect aspect, Dictionary<string, string?> conjugation)
    {
        aspect.FirstSingular = conjugation.Conjugate("1 si.").SplitBySemicolon();
        aspect.SecondSingular = conjugation.Conjugate("2 si.").SplitBySemicolon();
        aspect.ThirdSingular = conjugation.Conjugate("3 si. m.").SplitBySemicolon();
        aspect.ThirdSingularFeminine = conjugation.Conjugate("3 si. f.").SplitBySemicolon();
        aspect.FirstPlural = conjugation.Conjugate("1 pl.").SplitBySemicolon();
        aspect.SecondPlural = conjugation.Conjugate("2 pl. m.").SplitBySemicolon();
        aspect.SecondPluralFeminine = conjugation.Conjugate("2 pl. f.").SplitBySemicolon();
        aspect.ThirdPlural = conjugation.Conjugate("3 pl. m.").SplitBySemicolon();
        aspect.ThirdPluralFeminine = conjugation.Conjugate("3 pl. f.").SplitBySemicolon();
    }

    public static string? Conjugate(this Dictionary<string, string?> conjugation, string prn)
    {
        conjugation.TryGetValue(prn, out string? value);
        return value;
    }
}
