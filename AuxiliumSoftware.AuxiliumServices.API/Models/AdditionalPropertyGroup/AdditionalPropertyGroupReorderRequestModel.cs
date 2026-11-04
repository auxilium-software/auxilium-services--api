using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyGroup
{
    public class AdditionalPropertyGroupReorderRequestModel
    {
        [Required]
        [JsonPropertyName("orderedGroupIds")]
        public required List<Guid> OrderedGroupIds { get; set; }
    }
}
