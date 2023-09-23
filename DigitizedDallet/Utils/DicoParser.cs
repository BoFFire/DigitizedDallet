using DigitizedDallet.Models;

namespace DigitizedDallet.Utils;

public class DicoParser
{
    public static DocumentModel ParseDoc(string filePath)
    {
        DocumentModel doc = CustomJsonSerializer.DeserializeObject<DocumentModel>(File.ReadAllText(filePath));

        foreach (var l in doc.Letters)
        {
            l.Doc = doc;

            foreach (var sl in l.Roots)
            {
                sl.Letter = l;

                foreach (var entry in sl.Articles)
                {
                    entry.Root = sl;

                    foreach (var form in entry.AllForms)
                    {
                        form.Root = entry.Root;

                        foreach (var subform in form.AllForms)
                        {
                            subform.Root = entry.Root;
                        }
                    }
                }
            }
        }


        Dictionary<string, string> dico = new Dictionary<string, string>();

        foreach (var entry in doc.Articles)
        {
            //if (!string.IsNullOrWhiteSpace(e.Meanings.FirstOrDefault()?.SameAsId))
            //{
            //    e.Meanings.FirstOrDefault().SameAs = doc.Articles.Single(x => x.Id == e.Meanings.FirstOrDefault().SameAsId);                    
            //}
           

            dico.Add(entry.Id, entry.Name);

            entry.Meanings.ForEach(x => x.AssignParent());
            entry.Meanings.ForEach(x => x.Article = entry);

            foreach (var form in entry.AlternativeForms)
            {
                form.AlternativeFormOf = entry;
            }

            foreach (var form in entry.SubArticles)
            {
                form.SubArticleOf = entry;

                if (form.SubArticles.Any())
                {
                    throw new Exception("SubArticles must not have SubArticles");
                }

                if (string.IsNullOrWhiteSpace(form.RedirectToId) // not a redirect
                    && form.Meanings.Count != 1)
                {
                    throw new Exception("SubArticles must have one meaning, no more, no less.");
                }

                if (string.IsNullOrWhiteSpace(form.RedirectToId) // not a redirect
                    && form.Meanings.First().Translations.Count != 1)
                {
                    throw new Exception("SubArticles must have one translation, no more, no less.");
                }

                if (string.IsNullOrWhiteSpace(form.RedirectToId) // not a redirect
                    && form.Meanings.First().Meanings.Count != 0)
                {
                    throw new Exception("SubArticles must not have sub meanings.");
                }

                if (form.Info is not null)
                {
                    throw new Exception("SubArticles' Info must be null");
                }

                if (form.AlternativeForms.Any(x => x.Meanings.Any()))
                {
                    throw new Exception("AlternativeForms of SubArticles must not have meanings");
                }

                if (form.AlternativeForms.Any(x => !string.IsNullOrWhiteSpace(x.RedirectToId)))
                {
                    throw new Exception("AlternativeForms of SubArticles must not be redirected");
                }
            }

            foreach (var form in entry.PluralForms)
            {
                form.PluralFormOf = entry;
            }

            foreach (var form in entry.FemininePluralForms)
            {
                form.FemininePluralFormOf = entry;
            }

            foreach (var form in entry.FeminineForms)
            {
                form.FeminineFormOf = entry;
            }

            foreach (var form in entry.SingularForms)
            {
                form.SingularFormOf = entry;
            }

            foreach (var form in entry.VerbalNouns)
            {
                form.VerbalNounOf = entry;
            }

            foreach (var conjugation in entry.Conjugations)
            {
                foreach (var form in conjugation.IntensiveThirdSingularForms)
                {
                    form.IntensiveThirdSingularFormOf = entry;
                }                
                foreach (var form in conjugation.IntensiveThirdPluralForms)
                {
                    form.IntensiveThirdPluralFormOf = entry;
                }
                foreach (var form in conjugation.IntensiveThirdSingularFeminineForms)
                {
                    form.IntensiveThirdSingularFeminineFormOf = entry;
                }
                foreach (var form in conjugation.PreteriteThirdPluralForms)
                {
                    form.PreteriteThirdPluralFormOf = entry;
                }
                foreach (var form in conjugation.PreteriteThirdSingularFeminineForms)
                {
                    form.PreteriteThirdSingularFeminineFormOf = entry;
                }
                foreach (var form in conjugation.PreteriteThirdSingularForms)
                {
                    form.PreteriteThirdSingularFormOf = entry;
                }
                foreach (var form in conjugation.PreteriteFirstSingularForms)
                {
                    form.PreteriteFirstSingularFormOf = entry;
                }
                foreach (var form in conjugation.NegativePreteriteThirdSingularForms)
                {
                    form.NegativePreteriteThirdSingularFormOf = entry;
                }
                foreach (var form in conjugation.NegativePreteriteThirdPluralForms)
                {
                    form.NegativePreteriteThirdPluralFormOf = entry;
                }

                foreach (var form in conjugation.Imperative?.SecondPluralForms ?? Enumerable.Empty<ArticleModel>())
                {
                    form.ImperativePluralFormOf = entry;
                }
                foreach (var form in conjugation.Imperative?.SecondPluralFeminineForms ?? Enumerable.Empty<ArticleModel>())
                {
                    form.ImperativePluralFeminineFormOf = entry;
                }

                foreach (var form in conjugation.IntensiveImperativeForms)
                {
                    form.IntensiveImperativeFormOf = entry;
                }
            }


            if (!string.IsNullOrWhiteSpace(entry.RedirectToId))
            {
                entry.RedirectTo = doc.Articles.Single(x => x.Id == entry.RedirectToId);

                if (entry.AlternativeForms.Any())
                {
                    throw new Exception("Redirects must not have AlternativeForms");
                }
            }

            if (!string.IsNullOrWhiteSpace(entry.Prefix)
                && entry != entry.Root.Articles.First())
            {
                entry.PrefixedArticle = entry.Root.Articles.First();

                foreach (var form in entry.AlternativeForms)
                {
                    form.PrefixedArticle = entry.PrefixedArticle;
                }
            }
        }

        return doc;
    }
}
