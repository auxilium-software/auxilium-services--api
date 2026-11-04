using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyGroup
{
    public class AdditionalPropertyGroupCreationRequestModel
    {
        [Required]
        [JsonPropertyName("canonicalName")]
        public required string CanonicalName { get; set; }



        [JsonPropertyName("translations")]
        public Dictionary<string, string>? Translations { get; set; }
    }
}
