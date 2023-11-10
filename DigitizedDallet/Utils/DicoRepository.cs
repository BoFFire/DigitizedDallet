using DigitizedDallet.Helpers;
using DigitizedDallet.Models;
using DigitizedDallet.ViewModels;

namespace DigitizedDallet.Utils;

public class DicoRepository
{
    static DocumentModel? _doc;
    static public DocumentModel Doc
    {
        get
        {
            if (_doc == null)
            {
                _doc = DicoParser.ParseDoc(DictionaryFilePath);

                foreach (var w in _doc.Articles)
                {
                    foreach (var conjugations in w.Conjugations.Where(x => x.AmyagId is not null))
                    {
                        conjugations.AmyagPage = AmyagPages.Single(x => x.Id == conjugations.AmyagId);
                    }
                }
            }

            return _doc;
        }
    }

    static List<AmyagPage>? _AmyagPages;
    static public List<AmyagPage> AmyagPages => _AmyagPages ??= CustomJsonSerializer.DeserializeObject<List<AmyagPage>>(File.ReadAllText(ConjugationFilePath));

    private static string DictionaryFilePath => Path.Combine(EnvPath ?? throw new Exception("You need to call init"), "dictionary.json");
    private static string ConjugationFilePath => Path.Combine(EnvPath ?? throw new Exception("You need to call init"), "conjugation.json");

    private static string? EnvPath;
    public static void Init(string path)
    {
        EnvPath = path;
    }

    public static void Save()
    {
        QuickSave();
        _doc = null;
    }

    public static void QuickSave()
    {
        Doc.Readme.LastModificationDate = DateTime.Now;
        File.WriteAllText(DictionaryFilePath, CustomJsonSerializer.Serialize(Doc));
    }

    public static void Reset()
    {
        _doc = null;
        _AmyagPages = null;
    }

    public static ReportViewModel GenerateReport() => new ReportViewModel
    {
        InvalidLinks = GetFrenchText().SelectMany(GetInvalidLinks).Distinct().ToList(),
        InvalidTemplates = GetFrenchText().SelectMany(GetInvalidTemplates).Distinct().ToList(),
        MainArticlesCount = Doc.MainArticles.Count,
        ArticlesCount = Doc.Articles.Where(x => !x.IsRedirected).Count(),
        ArticlesWithDalletNamesCount = Doc.Articles.Where(x => !x.IsRedirected && x.DalletNames.Any()).Count(),
        ArticlesWithoutDalletNamesCount = Doc.Articles.Where(x => !x.IsRedirected && !x.DalletNames.Any()).Count(),
        DistinctArticlesCount = Doc.Articles.Select(x => x.Name).Distinct().Count(),
        ArticlesWithSubArticles = Doc.Articles.Where(x => x.SubArticles.Any()).Select(x => x.Name).ToList(),
        ArticlesWithRedirectedSubArticles = Doc.Articles.Where(x => x.SubArticles.Any(y => y.IsRedirected)).Select(x => x.Name).ToList(),
        RedirectsCount = Doc.Articles.Where(x => x.IsRedirected).Count(),
        ArticlesWithSubMeanings = Doc.Articles.Where(x => x.Meanings.Any(y => y.Meanings.Any())).Select(x => x.Name).ToList(),
        ArticlesWithCases = Doc.Articles.Where(x => x.AllMeanings.Any(y => y.Case != null)).Select(x => x.Name).ToList(),
        ArticlesWithMoreThanOneTranslationCount = Doc.Articles.Where(x => x.AllMeanings.Any(y => y.Translations.Count > 1)).Count(),
        ArticlesWithMoreThanOneTranslationAndOnlyOneMeaning = Doc.Articles.Where(x => x.AllMeanings.Count() == 1 && x.AllMeanings.Any(y => y.Translations.Count > 1)).ToList(),
        ArticlesWithMoreThanOneTranslationAndOnlyOneMeaningAndOneExample = Doc.Articles.Where(x => x.AllMeanings.Count() == 1 && x.AllMeanings.Any(y => y.Translations.Count > 1) && x.AllExamples.Count() == 1).ToList(),
        ArticlesWithPunctuation = Doc.Articles.Where(x => !x.IsRedirected).Select(x => x.Name).Where(x => x.Contains('?') || x.Contains('!') || x.Contains('.') || x.Contains(',') || x.Contains(';') || x.Contains(':') || x.Contains('/') || x.Contains('(') || x.Contains(')')).ToList(),
        ArticlesWithHyphen = Doc.Articles.Where(x => !x.IsRedirected).Select(x => x.Name).Where(x => x.Contains('-') && !x.StartsWith('-') && !x.EndsWith('-')).ToList(),
        ArticlesWithMark = Doc.Articles.Where(x => x.Mark != null).ToList(),
        ArticlesWithSpecialChars = Doc.Articles.Where(x => x.Name.Any(ch => new char[] { 'ṛ', 'ċ', 'ḱ', 'ḵ', 'ɉ', 'ş', 'ṥ', 'ž', 'ḷ', 'ḇ', 'ḃ', 'ţ', 'ṫ', 'ṯ', 'ġ', 'ḋ', 'ḏ', 'ʷ', 'ⁱ', 'ᵃ', 'ᵘ', 'ᵉ' }.Contains(ch))).Select(x => x.Name).ToList(),
    };

    static IEnumerable<string> GetFrenchText()
    {
        foreach (var article in Doc.Articles)
        {
            if (article.See != null)
            {
                yield return article.See;
            }

            if (article.Note != null)
            {
                yield return article.Note;
            }

            if (article.Info != null)
            {
                yield return article.Info;
            }

            foreach (var example in article.AllExamples)
            {
                yield return example.Translation;

                if (example.Note != null)
                {
                    yield return example.Note;
                }

                if (example.Source != null)
                {
                    yield return example.Source;
                }
            }

            foreach (var meaning in article.AllMeanings)
            {
                foreach (var translation in meaning.Translations)
                {
                    yield return translation;
                }

                if (meaning.Note != null)
                {
                    yield return meaning.Note;
                }
            }
        }
    }

    static List<string> GetInvalidTemplates(string text)
        => WikiHelper.GetTuples(text, '{', '}')
            .Where(x => x.Success && !Doc.Abbreviations.ContainsKey(x.Value))
            .Select(x => x.Value)
            .ToList();

    static List<string> GetInvalidLinks(string text)
    {
        text = text.ToLower().Trim();

        return WikiHelper.GetTuples(text, '[', ']')
            .Where(x => x.Success
            && !x.Value.Contains(':')
            && Doc.Articles.FirstOrDefault(y => y.Name == x.Value) == null)
            .Select(x => x.Value)
            .ToList();
    }
}
