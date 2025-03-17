using Microsoft.AspNetCore.Http;

namespace AdminDashboard.Server.Service
{
    public class JwtTokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtTokenService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Get JWT Token from the cookie
        public string GetToken()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["YourJwtCookieName"];
            return token;
        }
    }
}
