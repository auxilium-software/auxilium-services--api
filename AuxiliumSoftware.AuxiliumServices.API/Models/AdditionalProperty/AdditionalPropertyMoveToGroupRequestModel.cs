using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalProperty
{
    public class AdditionalPropertyMoveToGroupRequestModel
    {
        [JsonPropertyName("groupId")]
        public Guid? GroupId { get; set; }
    }
}
