using System.ComponentModel.DataAnnotations;

namespace AuxiliumSoftware.AuxiliumServices.API.Models.User
{
    public class UpdateUserRequestModel
    {
        public string? FullName { get; set; }


        // [EmailAddress(ErrorMessage = "Invalid email address")]
        // public string? EmailAddress { get; set; }


        public string? LanguagePreference { get; set; }
    }
}
