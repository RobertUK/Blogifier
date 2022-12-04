using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blogifier.Core.Security
{
    public  static class AdminAuthorizationPolicy
    {
        public static string Name => "Admin";

        public static void Build(AuthorizationPolicyBuilder builder) =>
            builder.RequireClaim(ClaimTypes.Role, "Admin");
    }
}
