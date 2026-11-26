using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.UserMerge
{
    public class UserMergeRequestModel
    {
        [Required]
        [JsonPropertyName("duplicateUserId")]
        public required Guid DuplicateUserId { get; init; }

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
