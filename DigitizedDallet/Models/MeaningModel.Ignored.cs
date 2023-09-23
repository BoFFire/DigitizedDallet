namespace DigitizedDallet.Models;

public partial class MeaningModel
{

    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public ArticleModel? SameAs { get; set; }   

    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public IEnumerable<MeaningModel> AllMeanings => Meanings.Concat(Meanings.SelectMany(x => x.AllMeanings));

    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public IEnumerable<ExampleModel> AllExamples => Examples.Concat(Meanings.SelectMany(x => x.AllExamples));

    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public MeaningModel? Parent { get; set; }

    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public ArticleModel? Article { get; set; }

    [System.Text.Json.Serialization.JsonIgnore][Newtonsoft.Json.JsonIgnore]
    public string StringIndex
    {
        get
        {
            var currentParent = Parent;
            var currentChild = this;

            var ls = new List<int>();            

            while (currentParent != null)
            {
                ls.Add(currentParent.Meanings.IndexOf(currentChild) + 1);                

                currentChild = currentParent;
                currentParent = currentParent.Parent;                
            }

            if (currentChild.Article != null)
            {
                ls.Add(currentChild.Article!.Meanings.IndexOf(currentChild) + 1);
            }

            ls.Reverse();

            return string.Join('.', ls);
        }
    }    

    public void AssignParent()
    {
        foreach(var meaning in Meanings)
        {
            meaning.Parent = this;
            meaning.AssignParent();
        }
    }
}
