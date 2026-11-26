using AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyDefinition;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;

namespace AuxiliumSoftware.AuxiliumServices.API.Mappers
{
    public static class AdditionalPropertyDefinitionMapper
    {
        #region ========================= CASE DEFINITIONS =========================
        public static AdditionalPropertyDefinitionResponseModel ToResponseModel(
            CaseAdditionalPropertyDefinitionEntityModel entity,
            string? locale
        )
        {
            var translations = (entity.Translations ?? [])
                .Select(t => (t.LanguageCode, t.Translation))
                .ToList();

            return new AdditionalPropertyDefinitionResponseModel
            {
                Id = entity.Id,
                CanonicalName = entity.CanonicalName,
                DisplayName = AdditionalPropertyGroupMapper.ResolveDisplayName(entity.CanonicalName, translations, locale),
                Description = entity.Description,
                DataType = entity.DataType,
                DataEnumeratorId = entity.DataEnumeratorId,
                DefaultGroupId = entity.DefaultGroupId,
                IsBuiltIn = entity.IsBuiltIn,
                IsActive = entity.IsActive,
                SortOrder = entity.SortOrder,
                Translations = MapTranslations(translations),
                Aliases = (entity.Aliases ?? [])
                    .OrderBy(a => a.Alias, StringComparer.OrdinalIgnoreCase)
                    .Select(a => ToResponseModel(a))
                    .ToList(),
                CreatedAtUtc = entity.CreatedAtUtc,
                LastUpdatedAtUtc = entity.LastUpdatedAtUtc,
            };
        }

        public static AdditionalPropertyDefinitionAliasResponseModel ToResponseModel(CaseAdditionalPropertyDefinitionAliasEntityModel alias)
        {
            return new AdditionalPropertyDefinitionAliasResponseModel
            {
                Id = alias.Id,
                Alias = alias.Alias,
                NormalisedAlias = alias.NormalisedAlias,
            };
        }
        #endregion
        #region ========================= USER DEFINITIONS =========================
        public static AdditionalPropertyDefinitionResponseModel ToResponseModel(
            UserAdditionalPropertyDefinitionEntityModel entity,
            string? locale
        )
        {
            var translations = (entity.Translations ?? [])
                .Select(t => (t.LanguageCode, t.Translation))
                .ToList();

            return new AdditionalPropertyDefinitionResponseModel
            {
                Id = entity.Id,
                CanonicalName = entity.CanonicalName,
                DisplayName = AdditionalPropertyGroupMapper.ResolveDisplayName(entity.CanonicalName, translations, locale),
                Description = entity.Description,
                DataType = entity.DataType,
                DataEnumeratorId = entity.DataEnumeratorId,
                DefaultGroupId = entity.DefaultGroupId,
                IsBuiltIn = entity.IsBuiltIn,
                IsActive = entity.IsActive,
                SortOrder = entity.SortOrder,
                Translations = MapTranslations(translations),
                Aliases = (entity.Aliases ?? [])
                    .OrderBy(a => a.Alias, StringComparer.OrdinalIgnoreCase)
                    .Select(a => ToResponseModel(a))
                    .ToList(),
                CreatedAtUtc = entity.CreatedAtUtc,
                LastUpdatedAtUtc = entity.LastUpdatedAtUtc,
            };
        }

        public static AdditionalPropertyDefinitionAliasResponseModel ToResponseModel(UserAdditionalPropertyDefinitionAliasEntityModel alias)
        {
            return new AdditionalPropertyDefinitionAliasResponseModel
            {
                Id = alias.Id,
                Alias = alias.Alias,
                NormalisedAlias = alias.NormalisedAlias,
            };
        }
        #endregion
        #region ========================= SHARED =========================
        private static List<AdditionalPropertyDefinitionTranslationResponseModel> MapTranslations(
            List<(string LanguageCode, string Translation)> translations
        )
        {
            return translations
                .OrderBy(t => t.LanguageCode, StringComparer.OrdinalIgnoreCase)
                .Select(t => new AdditionalPropertyDefinitionTranslationResponseModel
                {
                    LanguageCode = t.LanguageCode,
                    Translation = t.Translation,
                })
                .ToList();
        }
        #endregion
    }
}
