using AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;

namespace AuxiliumSoftware.AuxiliumServices.API.Mappers
{
    public static class AdditionalPropertyMapper
    {
        #region ========================= CASE PROPERTIES =========================
        public static AdditionalPropertySubStructureDTO ToSubStructureDTO(CaseAdditionalPropertyEntityModel property)
        {
            return new AdditionalPropertySubStructureDTO
            {
                Id = property.Id,
                CreatedAt = property.CreatedAtUtc,
                CreatedBy = property.CreatedByUserId,
                UpdatedAt = property.LastUpdatedAtUtc,
                LastUpdatedBy = property.LastUpdatedByUserId,
                DisplayName = property.DisplayName,
                Content = property.Content,
                ContentType = property.ContentType,
                GroupId = property.GroupId,
            };
        }

        public static Dictionary<string, AdditionalPropertySubStructureDTO> ToSubStructureDictionary(
            IEnumerable<CaseAdditionalPropertyEntityModel>? properties
        )
        {
            return properties?.ToDictionary(p => p.Id.ToString(), p => ToSubStructureDTO(p))
                ?? new Dictionary<string, AdditionalPropertySubStructureDTO>();
        }
        #endregion

        #region ========================= USER PROPERTIES =========================
        public static AdditionalPropertySubStructureDTO ToSubStructureDTO(UserAdditionalPropertyEntityModel property)
        {
            return new AdditionalPropertySubStructureDTO
            {
                Id = property.Id,
                CreatedAt = property.CreatedAtUtc,
                CreatedBy = property.CreatedByUserId,
                UpdatedAt = property.LastUpdatedAtUtc,
                LastUpdatedBy = property.LastUpdatedByUserId,
                DisplayName = property.DisplayName,
                Content = property.Content,
                ContentType = property.ContentType,
                GroupId = property.GroupId,
            };
        }

        public static Dictionary<string, AdditionalPropertySubStructureDTO> ToSubStructureDictionary(
            IEnumerable<UserAdditionalPropertyEntityModel>? properties
        )
        {
            return properties?.ToDictionary(p => p.Id.ToString(), p => ToSubStructureDTO(p))
                ?? new Dictionary<string, AdditionalPropertySubStructureDTO>();
        }
        #endregion
    }
}
