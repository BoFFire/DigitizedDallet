using DigitizedDallet.Models;

namespace DigitizedDallet.ViewModels;

public class AmyagConjugationViewModel
{
    static readonly string[] pronouns = new string[] { "nekk", "kečč", "netta", "nettat", "nekni", "kunwi", "kunnemti", "nutni", "nutenti" };

    public AmyagConjugationViewModel(AmyagPage model)
    {
        Name = model.Name;
        ShowImperative = model.Imperative?.HasAny() ?? false;
        ShowAorist = model.Aorist?.HasAny() ?? false;
        ShowPreterite = model.Preterite?.HasAny() ?? false;
        ShowNegativePreterite = model.NegativePreterite?.HasAny() ?? false;

        var potentialParticle = model.HasDirectionalParticle ? "a" : "ad";
        var prefixedDirectionalParticle = model.HasDirectionalParticle ? "d-" : string.Empty;
        var suffixedDirectionalParticle = model.HasDirectionalParticle ? "-d" : string.Empty;

        var imperativeArray = model.Imperative?.AsArray();
        var aoristArray = model.Aorist?.AsArray();
        var preteriteArray = model.Preterite?.AsArray();
        var negativePreteriteArray = model.NegativePreterite?.AsArray();

        for (int i = 0; i < pronouns.Length; i++)
        {
            List<string>? imperativeForms = imperativeArray?[i];
            List<string>? aoristForms = aoristArray?[i];
            List<string>? preteriteForms = preteriteArray?[i];
            List<string>? negativePreteriteForms = negativePreteriteArray?[i];

            var line = new AoristAndCoLineViewModel
            {
                Pronoun = pronouns[i],
            };

            if (imperativeForms != null)
            {
                line.Imperative = string.Join(" / ", imperativeForms.Select(x => $"{x}{suffixedDirectionalParticle}"));
            }
            if (aoristForms != null)
            {
                line.Aorist = string.Join(" / ", aoristForms.Select(x => $"{potentialParticle} {prefixedDirectionalParticle}{x}"));
            }
            if (preteriteForms != null)
            {
                line.Preterite = string.Join(" / ", preteriteForms.Select(x => $"{x}{suffixedDirectionalParticle}"));
            }
            if (negativePreteriteForms != null)
            {
                line.NegativePreterite = string.Join(" / ", negativePreteriteForms.Select(x => $"ur {prefixedDirectionalParticle}{x} ara"));
            }

            Lines.Add(line);
        }

        AoristParticiple = string.Join(" / ", model.AoristParticiple.Select(x => $"ara {prefixedDirectionalParticle}{x}"));
        PreteriteParticiple = string.Join(" / ", model.PreteriteParticiple.Select(x => $"{prefixedDirectionalParticle}{x}"));
        NegativePreteriteParticiple = string.Join(" / ", model.NegativePreteriteParticiple.Select(x => $"ur {prefixedDirectionalParticle}{x} ara"));

        foreach (var item in model.IntensiveForms)
        {
            var section = new IntensiveViewModel
            {
                Name = item.Name,
                IntensiveAoristParticiple = string.Join(" / ", item.IntensiveAoristParticiple.Select(x => $"{prefixedDirectionalParticle}{x}")),
                NegativeIntensiveAoristParticiple = string.Join(" / ", item.NegativeIntensiveAoristParticiple.Select(x => $"ur {prefixedDirectionalParticle}{x} ara")),
            };

            var intensiveImperativeArray = item.IntensiveImperative.AsArray();
            var intensiveAorist = item.IntensiveAorist.AsArray();

            for (int i = 0; i < pronouns.Length; i++)
            {
                section.Lines.Add(new IntensiveLineViewModel
                {
                    Pronoun = pronouns[i],
                    IntensiveImperative = string.Join(" / ", intensiveImperativeArray[i].Select(x => $"{x}{suffixedDirectionalParticle}")),
                    IntensiveAorist = string.Join(" / ", intensiveAorist[i].Select(x => $"{x}{suffixedDirectionalParticle}")),
                });
            }

            IntensiveSections.Add(section);
        }
    }

    public string Name { get; init; }

    public bool ShowImperative { get; init; }
    public bool ShowAorist { get; init; }
    public bool ShowPreterite { get; init; }
    public bool ShowNegativePreterite { get; init; }

    public List<AoristAndCoLineViewModel> Lines { get; } = new List<AoristAndCoLineViewModel>();
    public List<IntensiveViewModel> IntensiveSections { get; } = new List<IntensiveViewModel>();

    public string AoristParticiple { get; init; }
    public string PreteriteParticiple { get; init; }
    public string NegativePreteriteParticiple { get; init; }
}

public class AoristAndCoLineViewModel
{
    public required string Pronoun { get; set; }
    public string? Imperative { get; set; }
    public string? Aorist { get; set; }
    public string? Preterite { get; set; }
    public string? NegativePreterite { get; set; }
}

public class IntensiveViewModel
{
    public required string Name { get; set; }
    public required string IntensiveAoristParticiple { get; set; }
    public required string NegativeIntensiveAoristParticiple { get; set; }
    public List<IntensiveLineViewModel> Lines { get; } = new List<IntensiveLineViewModel>();
}

public class IntensiveLineViewModel
{
    public required string Pronoun { get; set; }
    public required string IntensiveImperative { get; set; }
    public required string IntensiveAorist { get; set; }
}
