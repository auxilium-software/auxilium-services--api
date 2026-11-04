using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyGroup
{
    public class AdditionalPropertyGroupResponseModel
    {
        [JsonPropertyName("id")]
        public required Guid Id { get; set; }



        [JsonPropertyName("canonicalName")]
        public required string CanonicalName { get; set; }



        [JsonPropertyName("displayName")]
        public required string DisplayName { get; set; }



        [JsonPropertyName("sortOrder")]
        public required int SortOrder { get; set; }



        [JsonPropertyName("translations")]
        public required List<AdditionalPropertyGroupTranslationResponseModel> Translations { get; set; }



        [JsonPropertyName("createdAtUtc")]
        public required DateTime CreatedAtUtc { get; set; }



        [JsonPropertyName("lastUpdatedAtUtc")]
        public DateTime? LastUpdatedAtUtc { get; set; }
    }
}
