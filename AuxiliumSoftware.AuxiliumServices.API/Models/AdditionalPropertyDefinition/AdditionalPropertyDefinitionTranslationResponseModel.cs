using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyDefinition
{
    public class AdditionalPropertyDefinitionTranslationResponseModel
    {
        [JsonPropertyName("languageCode")]
        public required string LanguageCode { get; set; }



        [JsonPropertyName("translation")]
        public required string Translation { get; set; }
    }
}
