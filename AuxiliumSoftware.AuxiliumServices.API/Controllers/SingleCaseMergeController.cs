using AuxiliumSoftware.AuxiliumServices.API.Common.ControllerBases;
using AuxiliumSoftware.AuxiliumServices.API.Models;
using AuxiliumSoftware.AuxiliumServices.API.Models.CaseMerge;
using AuxiliumSoftware.AuxiliumServices.Common.DataTransferObjects;
using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework;
using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Exceptions;
using AuxiliumSoftware.AuxiliumServices.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuxiliumSoftware.AuxiliumServices.API.Controllers
{
    [ApiController]
    [Route("/api/v3/cases/{caseId:guid}/merge")]
    [Tags("Cases")]
    public class SingleCaseMergeController : LoggedInControllerBase
    {
        private readonly ICaseMergeService _caseMergeService;

        public SingleCaseMergeController(
            ISystemSettingsService systemSettingsService,
            IConfiguration configuration,
            AuxiliumDbContext db,
            IWebApplicationFirewallService waf,
            ILogger<SingleCaseMergeController> logger,
            ITotpService totpService,

            ICaseMergeService caseMergeService
            )
            : base(systemSettingsService, configuration, db, waf, logger, totpService)
        {
            _caseMergeService = caseMergeService;
        }





        [HttpPost("preview")]
        [ProducesResponseType(typeof(DataMergePreviewDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DataMergePreviewDTO>> PreviewCaseMerge(
            Guid caseId,
            [FromBody] CaseMergePreviewRequestModel request,
            CancellationToken ct
        )
        {
            try
            {
                var (user, error) = await GetCurrentUserAsync();
                if (error != null) return error;

                var adminError = await this.RequireAdminAsync();
                if (adminError != null) return adminError;

                DataMergePreviewDTO preview = await _caseMergeService.PreviewAsync(caseId, request.DuplicateCaseId, ct);

                CaseMergePreviewResponseModel responseModel = new()
                {
                    SurvivorId = preview.SurvivorId,
                    DuplicateId = preview.DuplicateId,
                    Fingerprint = preview.Fingerprint,
                    Fields = preview.Fields,
                    References = preview.References,
                };
                return StatusCode(StatusCodes.Status200OK, responseModel);
            }
            catch (DataMergeRejectedException ex)
            {
                return Rejected(ex);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to preview merging case {DuplicateCaseId} into {CaseId}", request.DuplicateCaseId, caseId);
                return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
                {
                    Detail = "Failed to preview the case merge"
                });
            }
        }


        [HttpPost("")]
        [ProducesResponseType(typeof(DataMergeResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DataMergeResultDTO>> MergeCase(
            Guid caseId,
            [FromBody] CaseMergeRequestModel request,
            CancellationToken ct
        )
        {
            try
            {
                var (user, error) = await GetCurrentUserAsync();
                if (error != null) return error;

                var adminError = await this.RequireAdminAsync();
                if (adminError != null) return adminError;

                var totpError = await RequireTotpAsync(user.Id);
                if (totpError != null) return totpError;

                DataMergeResultDTO result = await _caseMergeService.MergeAsync(
                    survivorCaseId: caseId,
                    duplicateCaseId: request.DuplicateCaseId,
                    fingerprint: request.Fingerprint,
                    fieldChoices: request.FieldChoices,
                    justification: request.Justification,
                    actorUserId: user.Id,
                    ct: ct
                );


                CaseMergeSuccessResponseModel responseModel = new()
                {
                    MergeEventId = result.MergeEventId,
                    SurvivorCaseId = result.SurvivorId,
                    TombstoneCaseId = result.DuplicateId,
                    RowsMoved = result.RowsMoved,
                    RowsDiscarded = result.RowsDiscarded,
                };
                return StatusCode(StatusCodes.Status200OK, responseModel);
            }
            catch (DataMergeRejectedException ex)
            {
                return Rejected(ex);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to merge case {DuplicateCaseId} into {CaseId}", request.DuplicateCaseId, caseId);
                return StatusCode(StatusCodes.Status500InternalServerError, new FailureResponseModel
                {
                    Detail = "Failed to merge the cases. Nothing has been changed."
                });
            }
        }




        private ObjectResult Rejected(DataMergeRejectedException ex)
        {
            var status = ex.Reason switch
            {
                DataMergeRejectionReasonEnum.NotFound => StatusCodes.Status404NotFound,
                DataMergeRejectionReasonEnum.Stale => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest,
            };

            return StatusCode(status, new FailureResponseModel { Detail = ex.Message });
        }
    }
}
