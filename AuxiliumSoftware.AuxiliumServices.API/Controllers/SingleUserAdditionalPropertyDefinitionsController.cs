using AuxiliumSoftware.AuxiliumServices.API.Common.ControllerBases;
using AuxiliumSoftware.AuxiliumServices.API.Mappers;
using AuxiliumSoftware.AuxiliumServices.API.Models;
using AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyDefinition;
using AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuxiliumSoftware.AuxiliumServices.API.Controllers;

[ApiController]
[Route("/api/v3/users/additional_property_definitions")]
[Tags("Users")]
public class UserAdditionalPropertyDefinitionsController : LoggedInControllerBase
{
    private readonly IUserAdditionalPropertyDefinitionService _definitionService;

    public UserAdditionalPropertyDefinitionsController(
        ISystemSettingsService systemSettingsService,
        IConfiguration configuration,
        AuxiliumDbContext db,
        IWebApplicationFirewallService waf,
        ILogger<UserAdditionalPropertyDefinitionsController> logger,
        ITotpService totpService,

        IUserAdditionalPropertyDefinitionService definitionService
    )
        : base(systemSettingsService, configuration, db, waf, logger, totpService)
    {
        _definitionService = definitionService;
    }

    #region ========================= READ =========================
    [HttpGet("")]
    [ProducesResponseType(typeof(List<AdditionalPropertyDefinitionResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<AdditionalPropertyDefinitionResponseModel>>> GetDefinitions(
        [FromQuery] bool includeInactive = false,
        [FromQuery] string? locale = null,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;

            var resolvedLocale = locale ?? user!.LanguagePreference;
            var definitions = await _definitionService.GetDefinitionsAsync(includeInactive, ct);

            return StatusCode(StatusCodes.Status200OK, definitions
                .Select(d => AdditionalPropertyDefinitionMapper.ToResponseModel(d, resolvedLocale))
                .ToList()
            );
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get user additional property definitions");
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }

    [HttpGet("{definitionId:guid}")]
    [ProducesResponseType(typeof(AdditionalPropertyDefinitionResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdditionalPropertyDefinitionResponseModel>> GetDefinition(
        [FromRoute] Guid definitionId,
        [FromQuery] string? locale = null,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;

            var definition = await _definitionService.GetDefinitionAsync(definitionId, ct);
            if (definition == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new FailureResponseModel { Detail = "Definition not found." });
            }

            return StatusCode(StatusCodes.Status200OK, AdditionalPropertyDefinitionMapper.ToResponseModel(definition, locale ?? user!.LanguagePreference));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get user additional property definition {DefinitionId}", definitionId);
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }

    [HttpGet("unlinked_names")]
    [ProducesResponseType(typeof(List<UnlinkedAdditionalPropertyNameDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<UnlinkedAdditionalPropertyNameDTO>>> GetUnlinkedPropertyNames(
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;
            var adminError = await RequireAdminAsync();
            if (adminError != null) return adminError;

            return StatusCode(StatusCodes.Status200OK, await _definitionService.GetUnlinkedPropertyNamesAsync(ct));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get unlinked user additional property names");
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }
    #endregion
    #region ========================= DEFINITIONS =========================
    [HttpPost("")]
    [ProducesResponseType(typeof(AdditionalPropertyDefinitionResponseModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdditionalPropertyDefinitionResponseModel>> CreateDefinition(
        [FromBody] AdditionalPropertyDefinitionCreationRequestModel request,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;
            var adminError = await RequireAdminAsync();
            if (adminError != null) return adminError;

            var created = await _definitionService.CreateDefinitionAsync(
                user!.Id,
                request.CanonicalName,
                request.DataType,
                request.Description,
                request.DataEnumeratorId,
                request.DefaultGroupId,
                request.Translations,
                request.Aliases,
                ct
            );

            Logger.LogInformation("Created user additional property definition {DefinitionId} by {CurrentUserId}", created.Id, user.Id);

            var definition = await _definitionService.GetDefinitionAsync(created.Id, ct);

            return StatusCode(StatusCodes.Status201Created, AdditionalPropertyDefinitionMapper.ToResponseModel(definition!, user.LanguagePreference));
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new FailureResponseModel { Detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status409Conflict, new FailureResponseModel { Detail = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to create user additional property definition");
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }

    [HttpPut("{definitionId:guid}")]
    [ProducesResponseType(typeof(AdditionalPropertyDefinitionResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdditionalPropertyDefinitionResponseModel>> UpdateDefinition(
        [FromRoute] Guid definitionId,
        [FromBody] AdditionalPropertyDefinitionUpdateRequestModel request,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;
            var adminError = await RequireAdminAsync();
            if (adminError != null) return adminError;

            await _definitionService.UpdateDefinitionAsync(
                user!.Id,
                definitionId,
                request.CanonicalName,
                request.DataType,
                request.Description,
                request.DataEnumeratorId,
                request.DefaultGroupId,
                request.IsActive,
                ct
            );

            var definition = await _definitionService.GetDefinitionAsync(definitionId, ct);

            Logger.LogInformation("Updated user additional property definition {DefinitionId} by {CurrentUserId}", definitionId, user.Id);

            return StatusCode(StatusCodes.Status200OK, AdditionalPropertyDefinitionMapper.ToResponseModel(definition!, user.LanguagePreference));
        }
        catch (KeyNotFoundException)
        {
            return StatusCode(StatusCodes.Status404NotFound, new FailureResponseModel { Detail = "Definition not found." });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new FailureResponseModel { Detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status409Conflict, new FailureResponseModel { Detail = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to update user additional property definition {DefinitionId}", definitionId);
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }

    [HttpPut("order")]
    [ProducesResponseType(typeof(List<AdditionalPropertyDefinitionResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<AdditionalPropertyDefinitionResponseModel>>> ReorderDefinitions(
        [FromBody] AdditionalPropertyDefinitionReorderRequestModel request,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;
            var adminError = await RequireAdminAsync();
            if (adminError != null) return adminError;

            await _definitionService.ReorderDefinitionsAsync(user!.Id, request.OrderedDefinitionIds, ct);

            var definitions = await _definitionService.GetDefinitionsAsync(includeInactive: true, ct);

            Logger.LogInformation("Reordered user additional property definitions by {CurrentUserId}", user.Id);

            return StatusCode(StatusCodes.Status200OK, definitions
                .Select(d => AdditionalPropertyDefinitionMapper.ToResponseModel(d, user.LanguagePreference))
                .ToList()
            );
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new FailureResponseModel { Detail = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to reorder user additional property definitions");
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }

    [HttpDelete("{definitionId:guid}")]
    [ProducesResponseType(typeof(AdditionalPropertyDefinitionDeletionResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdditionalPropertyDefinitionDeletionResponseModel>> DeleteDefinition(
        [FromRoute] Guid definitionId,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;
            var adminError = await RequireAdminAsync();
            if (adminError != null) return adminError;

            int unlinkedCount = await _definitionService.DeleteDefinitionAsync(user!.Id, definitionId, ct);

            Logger.LogInformation(
                "Deleted user additional property definition {DefinitionId} by {CurrentUserId}, unlinking {Count} properties",
                definitionId, user.Id, unlinkedCount
            );

            return StatusCode(StatusCodes.Status200OK, new AdditionalPropertyDefinitionDeletionResponseModel
            {
                UnlinkedPropertyCount = unlinkedCount
            });
        }
        catch (KeyNotFoundException)
        {
            return StatusCode(StatusCodes.Status404NotFound, new FailureResponseModel { Detail = "Definition not found." });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status409Conflict, new FailureResponseModel { Detail = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to delete user additional property definition {DefinitionId}", definitionId);
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }

    [HttpPost("seed_built_ins")]
    [ProducesResponseType(typeof(AdditionalPropertyDefinitionSeedResultDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdditionalPropertyDefinitionSeedResultDTO>> SeedBuiltInDefinitions(
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;
            var adminError = await RequireAdminAsync();
            if (adminError != null) return adminError;

            var result = await _definitionService.SeedBuiltInDefinitionsAsync(user!.Id, ct);

            Logger.LogInformation("Seeded built-in user additional property definitions by {CurrentUserId}", user.Id);

            return StatusCode(StatusCodes.Status200OK, result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to seed built-in user additional property definitions");
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }
    #endregion
    #region ========================= TRANSLATIONS =========================
    [HttpPut("{definitionId:guid}/translations/{languageCode}")]
    [ProducesResponseType(typeof(AdditionalPropertyDefinitionResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdditionalPropertyDefinitionResponseModel>> SetTranslation(
        [FromRoute] Guid definitionId,
        [FromRoute] string languageCode,
        [FromBody] AdditionalPropertyDefinitionTranslationSetRequestModel request,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;
            var adminError = await RequireAdminAsync();
            if (adminError != null) return adminError;

            await _definitionService.SetTranslationAsync(user!.Id, definitionId, languageCode, request.Translation, ct);

            var definition = await _definitionService.GetDefinitionAsync(definitionId, ct);

            Logger.LogInformation(
                "Set {LanguageCode} translation on user additional property definition {DefinitionId} by {CurrentUserId}",
                languageCode, definitionId, user.Id
            );

            return StatusCode(StatusCodes.Status200OK, AdditionalPropertyDefinitionMapper.ToResponseModel(definition!, user.LanguagePreference));
        }
        catch (KeyNotFoundException)
        {
            return StatusCode(StatusCodes.Status404NotFound, new FailureResponseModel { Detail = "Definition not found." });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new FailureResponseModel { Detail = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to set {LanguageCode} translation on user additional property definition {DefinitionId}", languageCode, definitionId);
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }

    [HttpDelete("{definitionId:guid}/translations/{languageCode}")]
    [ProducesResponseType(typeof(AdditionalPropertyDefinitionResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdditionalPropertyDefinitionResponseModel>> RemoveTranslation(
        [FromRoute] Guid definitionId,
        [FromRoute] string languageCode,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;
            var adminError = await RequireAdminAsync();
            if (adminError != null) return adminError;

            await _definitionService.RemoveTranslationAsync(user!.Id, definitionId, languageCode, ct);

            var definition = await _definitionService.GetDefinitionAsync(definitionId, ct);

            Logger.LogInformation(
                "Removed {LanguageCode} translation from user additional property definition {DefinitionId} by {CurrentUserId}",
                languageCode, definitionId, user.Id
            );

            return StatusCode(StatusCodes.Status200OK, AdditionalPropertyDefinitionMapper.ToResponseModel(definition!, user.LanguagePreference));
        }
        catch (KeyNotFoundException ex)
        {
            return StatusCode(StatusCodes.Status404NotFound, new FailureResponseModel { Detail = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to remove {LanguageCode} translation from user additional property definition {DefinitionId}", languageCode, definitionId);
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }
    #endregion
    #region ========================= ALIASES =========================
    [HttpPost("{definitionId:guid}/aliases")]
    [ProducesResponseType(typeof(AdditionalPropertyDefinitionAliasCreationResponseModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdditionalPropertyDefinitionAliasCreationResponseModel>> AddAlias(
        [FromRoute] Guid definitionId,
        [FromBody] AdditionalPropertyDefinitionAliasCreationRequestModel request,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;
            var adminError = await RequireAdminAsync();
            if (adminError != null) return adminError;

            var (alias, linkedCount) = await _definitionService.AddAliasAsync(user!.Id, definitionId, request.Alias, ct);

            var definition = await _definitionService.GetDefinitionAsync(definitionId, ct);

            Logger.LogInformation(
                "Added alias {AliasId} to user additional property definition {DefinitionId} by {CurrentUserId}, linking {Count} properties",
                alias.Id, definitionId, user.Id, linkedCount
            );

            return StatusCode(StatusCodes.Status201Created, new AdditionalPropertyDefinitionAliasCreationResponseModel
            {
                Alias = AdditionalPropertyDefinitionMapper.ToResponseModel(alias),
                LinkedPropertyCount = linkedCount,
                Definition = AdditionalPropertyDefinitionMapper.ToResponseModel(definition!, user.LanguagePreference),
            });
        }
        catch (KeyNotFoundException)
        {
            return StatusCode(StatusCodes.Status404NotFound, new FailureResponseModel { Detail = "Definition not found." });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new FailureResponseModel { Detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status409Conflict, new FailureResponseModel { Detail = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to add alias to user additional property definition {DefinitionId}", definitionId);
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }

    [HttpDelete("{definitionId:guid}/aliases/{aliasId:guid}")]
    [ProducesResponseType(typeof(AdditionalPropertyDefinitionResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdditionalPropertyDefinitionResponseModel>> RemoveAlias(
        [FromRoute] Guid definitionId,
        [FromRoute] Guid aliasId,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;
            var adminError = await RequireAdminAsync();
            if (adminError != null) return adminError;

            await _definitionService.RemoveAliasAsync(user!.Id, definitionId, aliasId, ct);

            var definition = await _definitionService.GetDefinitionAsync(definitionId, ct);

            Logger.LogInformation(
                "Removed alias {AliasId} from user additional property definition {DefinitionId} by {CurrentUserId}",
                aliasId, definitionId, user.Id
            );

            return StatusCode(StatusCodes.Status200OK, AdditionalPropertyDefinitionMapper.ToResponseModel(definition!, user.LanguagePreference));
        }
        catch (KeyNotFoundException ex)
        {
            return StatusCode(StatusCodes.Status404NotFound, new FailureResponseModel { Detail = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to remove alias {AliasId} from user additional property definition {DefinitionId}", aliasId, definitionId);
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }
    #endregion
}
