using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.CaseMerge
{
    public class CaseMergeSuccessResponseModel
    {
        [Required]
        [JsonPropertyName("mergeEventId")]
        public required Guid MergeEventId { get; init; }



        [Required]
        [JsonPropertyName("survivorCaseId")]
        public required Guid SurvivorCaseId { get; init; }



        [Required]
        [JsonPropertyName("tombstoneCaseId")]
        public required Guid TombstoneCaseId { get; init; }



        [Required]
        [JsonPropertyName("rowsMoved")]
        public required long RowsMoved { get; init; }



        [Required]
        [JsonPropertyName("rowsDiscarded")]
        public required long RowsDiscarded { get; init; }
    }
}
