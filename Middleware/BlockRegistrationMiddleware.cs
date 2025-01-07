namespace FeedHiveAuth.Middleware
{
    public class BlockRegistrationMiddleware
    {
        private readonly RequestDelegate _next;

        public BlockRegistrationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/Identity/Account/Register"))
            {
                // Redirect users away from registration page
                context.Response.Redirect("/Home/Welcome"); // Or another appropriate page
                return;
            }

            await _next(context);
        }
    }
}
