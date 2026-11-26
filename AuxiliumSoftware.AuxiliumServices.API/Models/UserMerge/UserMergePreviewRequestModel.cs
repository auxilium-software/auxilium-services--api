using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.UserMerge
{
    public class UserMergePreviewRequestModel
    {
        [Required]
        [JsonPropertyName("duplicateUserId")]
        public required Guid DuplicateUserId { get; init; }
    }
}
