using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.CaseMerge
{
    public class CaseMergePreviewRequestModel
    {
        [Required]
        [JsonPropertyName("duplicateCaseId")]
        public required Guid DuplicateCaseId { get; init; }
    }
}
