using System.Text.Json.Serialization;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.Me
{
    public class ProfileUpdateRequestModel
    {
        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }


        [JsonPropertyName("languagePreference")]
        public string? LanguagePreference { get; set; }
    }
}
