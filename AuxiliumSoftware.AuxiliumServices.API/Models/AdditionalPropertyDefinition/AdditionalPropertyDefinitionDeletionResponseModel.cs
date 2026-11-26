using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyDefinition
{
    public class AdditionalPropertyDefinitionDeletionResponseModel
    {
        [JsonPropertyName("unlinkedPropertyCount")]
        public required int UnlinkedPropertyCount { get; set; }
    }
}
