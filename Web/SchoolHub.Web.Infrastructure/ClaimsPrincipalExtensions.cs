namespace SchoolHub.Web.Infrastructure
{
    using System.Security.Claims;

    using SchoolHub.Common;

    public static class ClaimsPrincipalExtensions
    {
        public static bool IsAdmin(this ClaimsPrincipal user)
            => user.IsInRole(GlobalConstants.AdministratorRoleName);
    }
}
