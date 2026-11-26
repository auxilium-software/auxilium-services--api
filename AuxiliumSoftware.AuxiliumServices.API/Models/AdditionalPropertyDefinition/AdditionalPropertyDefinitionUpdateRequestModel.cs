using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyDefinition
{
    public class AdditionalPropertyDefinitionUpdateRequestModel
    {
        [Required]
        [JsonPropertyName("canonicalName")]
        public required string CanonicalName { get; set; }



        [Required]
        [JsonPropertyName("dataType")]
        public required AdditionalPropertyDataTypeEnum DataType { get; set; }



        [JsonPropertyName("description")]
        public string? Description { get; set; }



        [JsonPropertyName("dataEnumeratorId")]
        public Guid? DataEnumeratorId { get; set; }



        [JsonPropertyName("defaultGroupId")]
        public Guid? DefaultGroupId { get; set; }



        [Required]
        [JsonPropertyName("isActive")]
        public required bool IsActive { get; set; }
    }
}
