using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyDefinition
{
    public class AdditionalPropertyDefinitionReorderRequestModel
    {
        [Required]
        [JsonPropertyName("orderedDefinitionIds")]
        public required List<Guid> OrderedDefinitionIds { get; set; }
    }
}
