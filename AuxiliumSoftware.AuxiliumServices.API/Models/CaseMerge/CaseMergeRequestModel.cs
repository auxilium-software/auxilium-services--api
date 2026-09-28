using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.CaseMerge
{
    public class CaseMergeRequestModel
    {
        [Required]
        [JsonPropertyName("duplicateCaseId")]
        public required Guid DuplicateCaseId { get; init; }

        [Required]
        [JsonPropertyName("fingerprint")]
        public required string Fingerprint { get; init; }

        [JsonPropertyName("fieldChoices")]
        public Dictionary<string, DataMergeSourceEnum>? FieldChoices { get; init; }

        [Required]
        [JsonPropertyName("justification")]
        public required string Justification { get; init; }
    }

}
