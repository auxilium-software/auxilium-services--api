using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyDefinition
{
    public class AdditionalPropertyDefinitionAliasResponseModel
    {
        [JsonPropertyName("id")]
        public required Guid Id { get; set; }



        [JsonPropertyName("alias")]
        public required string Alias { get; set; }



        [JsonPropertyName("normalisedAlias")]
        public required string NormalisedAlias { get; set; }
    }
}
