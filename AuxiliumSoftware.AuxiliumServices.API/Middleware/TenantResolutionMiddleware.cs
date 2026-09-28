using AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.Enumerators;
using AuxiliumSoftware.AuxiliumServices.Common.Tenancy;
using AuxiliumSoftware.AuxiliumServices.Common.Tenancy.TenantLookup;

namespace AuxiliumSoftware.AuxiliumServices.API.Middleware;

public sealed class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;

    public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
    {
        this._next = next;
        this._logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantLookup tenantLookup,
        ITenantContextSetter tenantContextSetter)
    {
        // swagger and health endpoints are served before a tenant exists
        // `/api/v3/server/ping` is intentionally not on this list
        var path = context.Request.Path;
        if (
            path.StartsWithSegments("/swagger")
            || path == "/"
        )
        {
            await this._next(context);
            return;
        }

        var host = context.Request.Host.Host;
        var tenant = await tenantLookup.FindByDomainAsync(host, context.RequestAborted);

        if (tenant is null)
        {
            this._logger.LogWarning("Rejected request for unknown tenant domain {Host}", host);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Unknown Host",
                message = "No tenant is configured for this hostname.",
                statusCode = 404
            });
            return;
        }

        if (tenant.LifecycleStatus != TenantLifecycleStatusEnum.Active)
        {
            this._logger.LogWarning(
                "Rejected request for {Status} tenant {TenantId} ({Host})",
                tenant.LifecycleStatus, tenant.TenantId, host);

            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Service Unavailable",
                message = "This service is not currently available.",
                statusCode = 503
            });
            return;
        }

        tenantContextSetter.SetTenant(tenant.TenantId);
        await this._next(context);
    }
}
