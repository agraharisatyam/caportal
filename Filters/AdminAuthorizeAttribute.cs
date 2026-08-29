using caportal.Services.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace caportal.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public string? Roles { get; set; }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Allow Anonymous if explicit AllowAnonymous is applied
        if (context.ActionDescriptor.EndpointMetadata.OfType<Microsoft.AspNetCore.Authorization.IAllowAnonymous>().Any())
        {
            return;
        }

        var authService = context.HttpContext.RequestServices.GetService<AdminAuthService>();
        if (authService == null)
        {
            context.Result = new StatusCodeResult(500);
            return;
        }

        if (!authService.IsAuthenticated(context.HttpContext, Roles))
        {
            var isAjax = context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                         context.HttpContext.Request.Headers["Accept"].ToString().Contains("application/json");

            if (isAjax)
            {
                context.Result = new JsonResult(new { success = false, message = "Authentication required. Please log in." })
                {
                    StatusCode = 401
                };
            }
            else
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("Login", "Auth", new { area = "Admin", returnUrl });
            }
        }
    }
}
