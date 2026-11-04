using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyGroup
{
    public class AdditionalPropertyGroupRenameRequestModel
    {
        [Required]
        [JsonPropertyName("canonicalName")]
        public required string CanonicalName { get; set; }
    }
}
