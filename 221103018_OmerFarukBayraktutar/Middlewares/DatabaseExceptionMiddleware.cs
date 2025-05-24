using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace _221103018_OmerFarukBayraktutar.Middlewares
{
    public class DatabaseExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DatabaseExceptionMiddleware> _logger;

        public DatabaseExceptionMiddleware(RequestDelegate next, ILogger<DatabaseExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) when (
                ex is SqlException ||
                ex is DbUpdateException ||
                ex is InvalidOperationException && ex.Message.Contains("database") ||
                ex is DbException)
            {
                _logger.LogError(ex, "Yakalanan veritabanı hatası");
                
                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    
                    // Eğer API isteği ise JSON yanıtı döndür
                    if (context.Request.Headers["Accept"].ToString().Contains("application/json"))
                    {
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("{\"error\":\"Veritabanı bağlantı hatası.\"}");
                    }
                    else
                    {
                        // Normal bir sayfa isteği ise hata sayfasına yönlendir
                        context.Response.Redirect("/Home/DbError");
                    }
                }
            }
        }
    }
} 