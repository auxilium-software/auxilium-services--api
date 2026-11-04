using AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyGroup;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;

namespace AuxiliumSoftware.AuxiliumServices.API.Mappers
{
    public static class AdditionalPropertyGroupMapper
    {
        #region ========================= CASE GROUPS =========================
        public static AdditionalPropertyGroupResponseModel ToResponseModel(
            CaseAdditionalPropertyGroupEntityModel entity,
            string? locale
        )
        {
            var translations = (entity.Translations ?? [])
                .Select(t => (t.LanguageCode, t.Translation))
                .ToList();

            return Build(entity.Id, entity.CanonicalName, entity.SortOrder, entity.CreatedAtUtc, entity.LastUpdatedAtUtc, translations, locale);
        }

        public static string? ResolveDisplayName(CaseAdditionalPropertyGroupEntityModel? entity, string? locale)
        {
            if (entity == null)
            {
                return null;
            }

            return ResolveDisplayName(
                entity.CanonicalName,
                (entity.Translations ?? []).Select(t => (t.LanguageCode, t.Translation)),
                locale
            );
        }
        #endregion
        #region ========================= USER GROUPS =========================
        public static AdditionalPropertyGroupResponseModel ToResponseModel(
            UserAdditionalPropertyGroupEntityModel entity,
            string? locale
        )
        {
            var translations = (entity.Translations ?? [])
                .Select(t => (t.LanguageCode, t.Translation))
                .ToList();

            return Build(entity.Id, entity.CanonicalName, entity.SortOrder, entity.CreatedAtUtc, entity.LastUpdatedAtUtc, translations, locale);
        }

        public static string? ResolveDisplayName(UserAdditionalPropertyGroupEntityModel? entity, string? locale)
        {
            if (entity == null)
            {
                return null;
            }

            return ResolveDisplayName(
                entity.CanonicalName,
                (entity.Translations ?? []).Select(t => (t.LanguageCode, t.Translation)),
                locale
            );
        }
        #endregion
        #region ========================= SHARED =========================
        private static AdditionalPropertyGroupResponseModel Build(
            Guid id,
            string canonicalName,
            int sortOrder,
            DateTime createdAtUtc,
            DateTime? lastUpdatedAtUtc,
            List<(string LanguageCode, string Translation)> translations,
            string? locale
        )
        {
            return new AdditionalPropertyGroupResponseModel
            {
                Id = id,
                CanonicalName = canonicalName,
                DisplayName = ResolveDisplayName(canonicalName, translations, locale),
                SortOrder = sortOrder,
                Translations = translations
                    .OrderBy(t => t.LanguageCode, StringComparer.OrdinalIgnoreCase)
                    .Select(t => new AdditionalPropertyGroupTranslationResponseModel
                    {
                        LanguageCode = t.LanguageCode,
                        Translation = t.Translation,
                    })
                    .ToList(),
                CreatedAtUtc = createdAtUtc,
                LastUpdatedAtUtc = lastUpdatedAtUtc,
            };
        }

        private static string ResolveDisplayName(
            string canonicalName,
            IEnumerable<(string LanguageCode, string Translation)> translations,
            string? locale
        )
        {
            if (string.IsNullOrWhiteSpace(locale)) return canonicalName;

            var list = translations as IList<(string LanguageCode, string Translation)> ?? translations.ToList();

            var exact = list.FirstOrDefault(t => string.Equals(t.LanguageCode, locale, StringComparison.OrdinalIgnoreCase));
            if (exact.Translation != null) return exact.Translation;

            var language = LanguageOf(locale);
            var sameLanguage = list.FirstOrDefault(t => string.Equals(LanguageOf(t.LanguageCode), language, StringComparison.OrdinalIgnoreCase));
            if (sameLanguage.Translation != null) return sameLanguage.Translation;

            return canonicalName;
        }

        private static string LanguageOf(string languageCode)
        {
            int separator = languageCode.IndexOfAny(['-', '_']);
            return separator < 0 ? languageCode : languageCode[..separator];
        }
        #endregion
    }
}
