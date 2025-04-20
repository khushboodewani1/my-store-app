using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MyStore.Filters
{
    public class AllowedRolesAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AllowedRolesAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(new
                {
                    controller = "Account",
                    action = "Login"
                });
                return;
            }

            if (!_roles.Any(role => user.IsInRole(role)))
            {
                context.Result = new RedirectToRouteResult(new
                {
                    controller = "Account",
                    action = "AccessDenied"
                });
            }
        }
    }
}
