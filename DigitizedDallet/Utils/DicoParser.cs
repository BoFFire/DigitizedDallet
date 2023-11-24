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
                    }
                }
            }
        }

        var redirectedArticles = new List<ArticleModel>();

        foreach (var entry in doc.Articles)
        {
            //if (!string.IsNullOrWhiteSpace(e.Meanings.FirstOrDefault()?.SameAsId))
            //{
            //    e.Meanings.FirstOrDefault().SameAs = doc.Articles.Single(x => x.Id == e.Meanings.FirstOrDefault().SameAsId);                    
            //}

            doc.ArticlesById.Add(entry.Id, entry);

            entry.Meanings.ForEach(x => x.AssignParent());
            entry.Meanings.ForEach(x => x.Article = entry);

            if (entry.StandardizedForm is not null)
            {
                entry.StandardizedForm.StandardizedFormOf = entry;
            }

            foreach (var form in entry.AlternativeForms)
            {
                form.AlternativeFormOf = entry;
            }

            foreach (var form in entry.ReducedForms)
            {
                form.ReducedFormOf = entry;
            }

            foreach (var form in entry.SubArticles)
            {
                form.SubArticleOf = entry;

                if (form.SubArticles.Any())
                {
                    throw new Exception("SubArticles must not have SubArticles");
                }

                if (string.IsNullOrWhiteSpace(form.RedirectToId) // not a redirect
                    && form.Meanings.Count < 1)
                {
                    throw new Exception("SubArticles must have at least one meaning");
                }

                if (string.IsNullOrWhiteSpace(form.RedirectToId) // not a redirect
                    && form.Meanings.Any(x => x.Meanings.Count != 0))
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

                if (form.AlternativeForms.Any())
                {
                    throw new Exception();
                }

            }

            foreach (var form in entry.FemininePluralForms)
            {
                form.FemininePluralFormOf = entry;

                if (form.AlternativeForms.Any())
                {
                    throw new Exception();
                }

            }

            foreach (var form in entry.FeminineForms)
            {
                form.FeminineFormOf = entry;

                if (form.AlternativeForms.Any())
                {
                    throw new Exception();
                }
            }

            foreach (var form in entry.SingularForms)
            {
                form.SingularFormOf = entry;

                if (form.AlternativeForms.Any())
                {
                    throw new Exception();
                }
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

            if (!string.IsNullOrWhiteSpace(entry.Prefix)
                && entry != entry.Root.Articles.First())
            {
                entry.PrefixedArticle = entry.Root.Articles.First();

                foreach (var form in entry.AlternativeForms)
                {
                    form.PrefixedArticle = entry.PrefixedArticle;
                }
            }

            if (!string.IsNullOrWhiteSpace(entry.RedirectToId))
            {
                if (entry.AlternativeForms.Any())
                {
                    throw new Exception("Redirects must not have AlternativeForms");
                }

                if (entry.Info is not null)
                {
                    throw new Exception("redirectedArticles' Info must be null");
                }

                if (entry.Note is not null)
                {
                    throw new Exception("redirectedArticles' Note must be null");
                }

                redirectedArticles.Add(entry);
            }
        }

        redirectedArticles.ForEach(x => x.RedirectTo = doc.ArticlesById[x.RedirectToId!]);

        return doc;
    }
}
