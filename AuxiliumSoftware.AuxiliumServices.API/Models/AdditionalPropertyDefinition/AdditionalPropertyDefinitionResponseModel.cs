using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyDefinition
{
    public class AdditionalPropertyDefinitionResponseModel
    {
        [JsonPropertyName("id")]
        public required Guid Id { get; set; }



        [JsonPropertyName("canonicalName")]
        public required string CanonicalName { get; set; }



        [JsonPropertyName("displayName")]
        public required string DisplayName { get; set; }



        [JsonPropertyName("description")]
        public string? Description { get; set; }



        [JsonPropertyName("dataType")]
        public required AdditionalPropertyDataTypeEnum DataType { get; set; }



        [JsonPropertyName("dataEnumeratorId")]
        public Guid? DataEnumeratorId { get; set; }



        [JsonPropertyName("defaultGroupId")]
        public Guid? DefaultGroupId { get; set; }



        [JsonPropertyName("isBuiltIn")]
        public required bool IsBuiltIn { get; set; }



        [JsonPropertyName("isActive")]
        public required bool IsActive { get; set; }



        [JsonPropertyName("sortOrder")]
        public required int SortOrder { get; set; }



        [JsonPropertyName("translations")]
        public required List<AdditionalPropertyDefinitionTranslationResponseModel> Translations { get; set; }



        [JsonPropertyName("aliases")]
        public required List<AdditionalPropertyDefinitionAliasResponseModel> Aliases { get; set; }



        [JsonPropertyName("createdAtUtc")]
        public required DateTime CreatedAtUtc { get; set; }



        [JsonPropertyName("lastUpdatedAtUtc")]
        public DateTime? LastUpdatedAtUtc { get; set; }
    }
}
