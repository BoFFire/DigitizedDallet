using Newtonsoft.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Collections;

namespace DigitizedDallet.Utils;

//The use of two libs is a small trick to rename properties.
//ex: [System.Text.Json.Serialization.JsonPropertyName("newName")]
public static class CustomJsonSerializer
{
    public static TValue DeserializeObject<TValue>(string value) => JsonConvert.DeserializeObject<TValue>(value) ?? throw new Exception();

    public static string Serialize<TValue>(TValue value) => System.Text.Json.JsonSerializer.Serialize(value, new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        //Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), // I want my apostrophes
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IgnoreReadOnlyProperties = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers = { DefaultValueModifier }
        },
    });

    private static void DefaultValueModifier(JsonTypeInfo type_info)
    {
        foreach (var property in type_info.Properties.Where(p => typeof(ICollection).IsAssignableFrom(p.PropertyType)))
        {
            property.ShouldSerialize = (_, val) => val is ICollection collection && collection.Count > 0;
        }
    }
}