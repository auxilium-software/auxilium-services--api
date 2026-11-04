using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyGroup
{
    public class AdditionalPropertyGroupTranslationResponseModel
    {
        [JsonPropertyName("languageCode")]
        public required string LanguageCode { get; set; }



        [JsonPropertyName("translation")]
        public required string Translation { get; set; }
    }
}
