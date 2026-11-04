using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyGroup
{
    public class AdditionalPropertyGroupTranslationSetRequestModel
    {
        [Required]
        [JsonPropertyName("translation")]
        public required string Translation { get; set; }
    }
}
