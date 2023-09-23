namespace DigitizedDallet.Models;

public class AmyagPage
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Translation { get; set; }
    public bool HasDirectionalParticle { get; set; }

    /* AoristAndCo */
    public AmyagAspect? Imperative { get; set; }
    public AmyagAspect? Aorist { get; set; }
    public AmyagAspect? Preterite { get; set; }
    public AmyagAspect? NegativePreterite { get; set; }

    public List<string> AoristParticiple { get; set; } = new List<string>();
    public List<string> PreteriteParticiple { get; set; } = new List<string>();
    public List<string> NegativePreteriteParticiple { get; set; } = new List<string>();

    public List<AmyagIntensiveSection> IntensiveForms { get; set; } = new List<AmyagIntensiveSection>();

    public bool IsIrregular { get; set; }
    public bool IsDerived { get; set; }
    public AmyagPattern? Pattern { get; set; }

    public IEnumerable<AmyagAspect> GetAllAspects() 
        => new List<AmyagAspect?>{ Imperative , Aorist , Preterite, NegativePreterite }.Where(x => x is not null).Select(x=> x!)
        .Concat(IntensiveForms.Select(x=> x.IntensiveImperative))
        .Concat(IntensiveForms.Select(x => x.IntensiveAorist));

    public IEnumerable<string> GetAll() => GetAllAspects().SelectMany(x=> x.GetAll())
    .Concat(AoristParticiple)
    .Concat(PreteriteParticiple)
    .Concat(NegativePreteriteParticiple)
    .Concat(IntensiveForms.SelectMany(x=> x.IntensiveAoristParticiple))
    .Concat(IntensiveForms.SelectMany(x => x.NegativeIntensiveAoristParticiple));
}


public class AmyagPattern
{
    public string Id { get; set; } = null!;
    public string Verb { get; set; } = null!;
    public string Number { get; set; } = null!;
}

public class AmyagIntensiveSection
{
    public string Name { get; set; } = null!;
    
    public AmyagAspect IntensiveImperative { get; set; } = null!;
    public AmyagAspect IntensiveAorist { get; set; } = null!;

    public List<string> IntensiveAoristParticiple { get; set; } = new List<string>();
    public List<string> NegativeIntensiveAoristParticiple { get; set; } = new List<string>();    
}

public class AmyagAspect
{
    public List<string> FirstSingular { get; set; } = new List<string>();
    public List<string> SecondSingular { get; set; } = new List<string>();
    public List<string> ThirdSingular { get; set; } = new List<string>();
    public List<string> ThirdSingularFeminine { get; set; } = new List<string>();
    public List<string> FirstPlural { get; set; } = new List<string>();
    public List<string> SecondPlural { get; set; } = new List<string>();
    public List<string> SecondPluralFeminine { get; set; } = new List<string>();
    public List<string> ThirdPlural { get; set; } = new List<string>();
    public List<string> ThirdPluralFeminine { get; set; } = new List<string>();

    public bool HasAny() => FirstSingular.Any()
            || SecondSingular.Any()
            || ThirdSingular.Any()
            || ThirdSingularFeminine.Any()
            || FirstPlural.Any()
            || SecondPlural.Any()
            || SecondPluralFeminine.Any()
            || ThirdPlural.Any()
            || ThirdPluralFeminine.Any();

    public IEnumerable<string> GetAll() => FirstSingular
        .Concat(SecondSingular)
        .Concat(ThirdSingular)
        .Concat(ThirdSingularFeminine)
        .Concat(FirstPlural)
        .Concat(SecondPlural)
        .Concat(SecondPluralFeminine)
        .Concat(ThirdPlural)
        .Concat(ThirdPluralFeminine);

    public List<string>[] AsArray() => new List<string>[] 
    {  
        FirstSingular,
        SecondSingular,
        ThirdSingular,
        ThirdSingularFeminine,
        FirstPlural,
        SecondPlural,
        SecondPluralFeminine,
        ThirdPlural,
        ThirdPluralFeminine 
    };
}
