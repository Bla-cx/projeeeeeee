using Microsoft.AspNetCore.Builder;

namespace _221103018_OmerFarukBayraktutar.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseDatabaseExceptionMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DatabaseExceptionMiddleware>();
        }
    }
} 