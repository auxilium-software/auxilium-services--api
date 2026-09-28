using AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.CaseMerge
{
    public class CaseMergePreviewResponseModel
    {
        [Required]
        [JsonPropertyName("survivorId")]
        public required Guid SurvivorId { get; init; }



        [Required]
        [JsonPropertyName("duplicateId")]
        public required Guid DuplicateId { get; init; }



        [Required]
        [JsonPropertyName("fingerprint")]
        public required string Fingerprint { get; init; }



        [Required]
        [JsonPropertyName("fields")]
        public required List<DataMergeFieldConflictDTO> Fields { get; init; }



        [Required]
        [JsonPropertyName("references")]
        public required List<DataMergeReferenceSummaryDTO> References { get; init; }
    }
}
