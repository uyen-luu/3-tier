using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ApplicationTier.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApplicationTier.WebAPI.Middlewares
{
	public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<ExceptionMiddleware> logger)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                return;
            }

            // Get root cause of the exception
            var baseEx = ex.GetBaseException();
            string message;
            if (baseEx is UnauthorizedAccessException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                message = baseEx.Message;
            }
            else if (baseEx is FileNotFoundException || baseEx is DirectoryNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                message = baseEx.Message;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                // Show less info when run on the production mode
                message = !_env.IsProduction()  ? baseEx.Message : "InternalServerError";
            }
            
            // Convert to custom error model
            var error = new ErrorViewModel(message, !_env.IsProduction() ? baseEx.StackTrace : null);
            _logger.LogError(ex, ex.Message);
            //
            // Configure how response format users will see.
            // Return as json
            context.Response.ContentType = "application/json";
            var camelCaseOptions = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            var bodyJson = JsonSerializer.Serialize(error, camelCaseOptions);
            await context.Response.WriteAsync(bodyJson);
        }
    }
}
