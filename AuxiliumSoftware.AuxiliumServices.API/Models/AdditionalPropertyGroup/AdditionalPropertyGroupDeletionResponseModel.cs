using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyGroup
{
    public class AdditionalPropertyGroupDeletionResponseModel
    {
        [Required]
        [JsonPropertyName("status")]
        public string Status { get; } = "success";



        [JsonPropertyName("ungroupedPropertyCount")]
        public required int UngroupedPropertyCount { get; set; }
    }
}
