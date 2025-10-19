namespace RaffleApp.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Gets the anonymous user ID from the current HttpContext.User.
        /// </summary>
        /// <param name="context">The HttpContext instance.</param>
        /// <returns>The anonymous user ID.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the anonymous user ID is not found.</exception>
        public static string GetAnonymousUserId(this HttpContext context)
        {
            return context.User?.Identity?.Name 
                ?? throw new InvalidOperationException("Anonymous user ID not found");
        }

        /// <summary>
        /// Checks if the current user is an anonymous user (not authenticated).
        /// </summary>
        /// <param name="context">The HttpContext instance.</param>
        /// <returns>True if the user is anonymous, false otherwise.</returns>
        public static bool IsAnonymousUser(this HttpContext context)
        {
            return context.User?.HasClaim("anonymous_user", "true") ?? false;
        }
    }
}
