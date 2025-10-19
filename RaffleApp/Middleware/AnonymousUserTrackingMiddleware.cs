using System.Security.Claims;

namespace RaffleApp.Middleware
{
    public class AnonymousUserTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private const string UserIdCookieName = "AnonymousUserId";

        public AnonymousUserTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string userId;

            if (!context.Request.Cookies.ContainsKey(UserIdCookieName))
            {
                // First request - generate new ID
                userId = Guid.NewGuid().ToString();

                context.Response.Cookies.Append(UserIdCookieName, userId, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    IsEssential = true
                });
            }
            else
            {
                // Returning user - read existing ID
                userId = context.Request.Cookies[UserIdCookieName]!;
            }

            // Create claims for the anonymous user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userId),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("anonymous_user", "true")
            };

            var identity = new ClaimsIdentity(claims, "AnonymousCookie");
            var principal = new ClaimsPrincipal(identity);

            // Assign to HttpContext.User
            context.User = principal;

            await _next(context);
        }
    }
}
