using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyDefinition
{
    public class AdditionalPropertyDefinitionTranslationSetRequestModel
    {
        [Required]
        [JsonPropertyName("translation")]
        public required string Translation { get; set; }
    }
}
