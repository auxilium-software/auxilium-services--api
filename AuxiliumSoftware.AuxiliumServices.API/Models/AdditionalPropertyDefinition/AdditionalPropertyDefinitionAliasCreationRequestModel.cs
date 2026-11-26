using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyDefinition
{
    public class AdditionalPropertyDefinitionAliasCreationRequestModel
    {
        [Required]
        [JsonPropertyName("alias")]
        public required string Alias { get; set; }
    }
}
