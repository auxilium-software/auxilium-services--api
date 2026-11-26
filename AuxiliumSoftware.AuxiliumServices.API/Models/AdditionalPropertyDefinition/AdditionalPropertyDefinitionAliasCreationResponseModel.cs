using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyDefinition
{
    public class AdditionalPropertyDefinitionAliasCreationResponseModel
    {
        [JsonPropertyName("alias")]
        public required AdditionalPropertyDefinitionAliasResponseModel Alias { get; set; }



        [JsonPropertyName("linkedPropertyCount")]
        public required int LinkedPropertyCount { get; set; }



        [JsonPropertyName("definition")]
        public required AdditionalPropertyDefinitionResponseModel Definition { get; set; }
    }
}
