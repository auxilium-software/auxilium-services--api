using AuxiliumSoftware.AuxiliumServices.API.Common.ControllerBases;
using AuxiliumSoftware.AuxiliumServices.API.Mappers;
using AuxiliumSoftware.AuxiliumServices.API.Models;
using AuxiliumSoftware.AuxiliumServices.API.Models.AdditionalPropertyGroup;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels;
using AuxiliumSoftware.AuxiliumServices.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuxiliumSoftware.AuxiliumServices.API.Controllers;

[ApiController]
[Route("/api/v3/users/additional_property_groups")]
[Tags("Users")]
public class UserAdditionalPropertyGroupsController : LoggedInControllerBase
{
    private readonly IUserAdditionalPropertyGroupService _groupService;

    public UserAdditionalPropertyGroupsController(
        ISystemSettingsService systemSettingsService,
        IConfiguration configuration,
        AuxiliumDbContext db,
        IWebApplicationFirewallService waf,
        ILogger<UserAdditionalPropertyGroupsController> logger,
        ITotpService totpService,

        IUserAdditionalPropertyGroupService groupService
    )
        : base(systemSettingsService, configuration, db, waf, logger, totpService)
    {
        _groupService = groupService;
    }

    #region ========================= READ =========================
    [HttpGet("")]
    [ProducesResponseType(typeof(List<AdditionalPropertyGroupResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<AdditionalPropertyGroupResponseModel>>> GetGroups(
        [FromQuery] string? locale = null,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;

            var resolvedLocale = locale ?? user!.LanguagePreference;
            List<UserAdditionalPropertyGroupEntityModel> groups = await _groupService.GetGroupsAsync(ct);

            return StatusCode(StatusCodes.Status200OK, groups
                .Select(g => AdditionalPropertyGroupMapper.ToResponseModel(g, resolvedLocale))
                .ToList()
            );
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get user additional property groups");
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }

    [HttpGet("{groupId:guid}")]
    [ProducesResponseType(typeof(AdditionalPropertyGroupResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdditionalPropertyGroupResponseModel>> GetGroup(
        [FromRoute] Guid groupId,
        [FromQuery] string? locale = null,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;

            var group = await _groupService.GetGroupAsync(groupId, ct);
            if (group == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new FailureResponseModel { Detail = "Group not found." });
            }

            return StatusCode(StatusCodes.Status200OK, AdditionalPropertyGroupMapper.ToResponseModel(group, locale ?? user!.LanguagePreference));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get user additional property group {GroupId}", groupId);
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }
    #endregion
    #region ========================= GROUPS =========================
    [HttpPost("")]
    [ProducesResponseType(typeof(AdditionalPropertyGroupResponseModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdditionalPropertyGroupResponseModel>> CreateGroup(
        [FromBody] AdditionalPropertyGroupCreationRequestModel request,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;
            var adminError = await RequireAdminAsync();
            if (adminError != null) return adminError;

            UserAdditionalPropertyGroupEntityModel group = await _groupService.CreateGroupAsync(user!.Id, request.CanonicalName, request.Translations, ct);

            Logger.LogInformation("Created user additional property group {GroupId} by {CurrentUserId}", group.Id, user.Id);

            return StatusCode(StatusCodes.Status201Created, AdditionalPropertyGroupMapper.ToResponseModel(group, user.LanguagePreference));
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
            Logger.LogError(ex, "Failed to create user additional property group");
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }

    [HttpPatch("{groupId:guid}")]
    [ProducesResponseType(typeof(AdditionalPropertyGroupResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdditionalPropertyGroupResponseModel>> RenameGroup(
        [FromRoute] Guid groupId,
        [FromBody] AdditionalPropertyGroupRenameRequestModel request,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;
            var adminError = await RequireAdminAsync();
            if (adminError != null) return adminError;

            await _groupService.RenameGroupAsync(user!.Id, groupId, request.CanonicalName, ct);

            UserAdditionalPropertyGroupEntityModel? group = await _groupService.GetGroupAsync(groupId, ct);

            Logger.LogInformation("Renamed user additional property group {GroupId} by {CurrentUserId}", groupId, user.Id);

            return StatusCode(StatusCodes.Status200OK, AdditionalPropertyGroupMapper.ToResponseModel(group!, user.LanguagePreference));
        }
        catch (KeyNotFoundException)
        {
            return StatusCode(StatusCodes.Status404NotFound, new FailureResponseModel { Detail = "Group not found." });
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
            Logger.LogError(ex, "Failed to rename user additional property group {GroupId}", groupId);
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }

    [HttpPut("order")]
    [ProducesResponseType(typeof(List<AdditionalPropertyGroupResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<AdditionalPropertyGroupResponseModel>>> ReorderGroups(
        [FromBody] AdditionalPropertyGroupReorderRequestModel request,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;
            var adminError = await RequireAdminAsync();
            if (adminError != null) return adminError;

            await _groupService.ReorderGroupsAsync(user!.Id, request.OrderedGroupIds, ct);

            var groups = await _groupService.GetGroupsAsync(ct);

            Logger.LogInformation("Reordered user additional property groups by {CurrentUserId}", user.Id);

            return StatusCode(StatusCodes.Status200OK, groups
                .Select(g => AdditionalPropertyGroupMapper.ToResponseModel(g, user.LanguagePreference))
                .ToList()
            );
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new FailureResponseModel { Detail = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to reorder user additional property groups");
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }

    [HttpDelete("{groupId:guid}")]
    [ProducesResponseType(typeof(AdditionalPropertyGroupDeletionResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdditionalPropertyGroupDeletionResponseModel>> DeleteGroup(
        [FromRoute] Guid groupId,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;
            var adminError = await RequireAdminAsync();
            if (adminError != null) return adminError;

            int ungroupedCount = await _groupService.DeleteGroupAsync(user!.Id, groupId, ct);

            Logger.LogInformation(
                "Deleted user additional property group {GroupId} by {CurrentUserId}, ungrouping {Count} properties",
                groupId, user.Id, ungroupedCount
            );

            return StatusCode(StatusCodes.Status200OK, new AdditionalPropertyGroupDeletionResponseModel
            {
                UngroupedPropertyCount = ungroupedCount
            });
        }
        catch (KeyNotFoundException)
        {
            return StatusCode(StatusCodes.Status404NotFound, new FailureResponseModel { Detail = "Group not found." });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to delete user additional property group {GroupId}", groupId);
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }
    #endregion
    #region ========================= TRANSLATIONS =========================
    [HttpPut("{groupId:guid}/translations/{languageCode}")]
    [ProducesResponseType(typeof(AdditionalPropertyGroupResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdditionalPropertyGroupResponseModel>> SetTranslation(
        [FromRoute] Guid groupId,
        [FromRoute] string languageCode,
        [FromBody] AdditionalPropertyGroupTranslationSetRequestModel request,
        CancellationToken ct = default
    )
    {
        try
        {
            var (user, error) = await GetCurrentUserAsync();
            if (error != null) return error;
            var adminError = await RequireAdminAsync();
            if (adminError != null) return adminError;

            await _groupService.SetTranslationAsync(user!.Id, groupId, languageCode, request.Translation, ct);

            UserAdditionalPropertyGroupEntityModel? group = await _groupService.GetGroupAsync(groupId, ct);

            Logger.LogInformation(
                "Set {LanguageCode} translation on user additional property group {GroupId} by {CurrentUserId}",
                languageCode, groupId, user.Id
            );

            return StatusCode(StatusCodes.Status200OK,
                AdditionalPropertyGroupMapper.ToResponseModel(group!, user.LanguagePreference));
        }
        catch (KeyNotFoundException)
        {
            return StatusCode(StatusCodes.Status404NotFound, new FailureResponseModel { Detail = "Group not found." });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new FailureResponseModel { Detail = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to set {LanguageCode} translation on user additional property group {GroupId}", languageCode, groupId);
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }

    [HttpDelete("{groupId:guid}/translations/{languageCode}")]
    [ProducesResponseType(typeof(AdditionalPropertyGroupResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailureResponseModel), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdditionalPropertyGroupResponseModel>> RemoveTranslation(
        [FromRoute] Guid groupId,
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

            await _groupService.RemoveTranslationAsync(user!.Id, groupId, languageCode, ct);

            var group = await _groupService.GetGroupAsync(groupId, ct);

            Logger.LogInformation(
                "Removed {LanguageCode} translation from user additional property group {GroupId} by {CurrentUserId}",
                languageCode, groupId, user.Id
            );

            return StatusCode(StatusCodes.Status200OK, AdditionalPropertyGroupMapper.ToResponseModel(group!, user.LanguagePreference));
        }
        catch (KeyNotFoundException ex)
        {
            return StatusCode(StatusCodes.Status404NotFound, new FailureResponseModel { Detail = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new FailureResponseModel { Detail = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to remove {LanguageCode} translation from user additional property group {GroupId}", languageCode, groupId);
            return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
            {
                Detail = "An unexpected error occurred."
            });
        }
    }
    #endregion
}
