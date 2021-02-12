using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NETCoreTemplate.Entity.Models;
using NETCoreTemplate.Entity.Utilities;

namespace NETCoreTemplate.WebAPI.Middlewares
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

            string message;
            if (ex is UnauthorizedAccessException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                message = ex.Message;
            }
            else if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                message = ex.Message;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //
                // Convert to model
                message = !_env.IsProduction() || ex is AppException ? ex.GetErrorMessages() : "InternalServerError";
            }
            
            var error = new ErrorViewModel(message, !_env.IsProduction() ? ex.StackTrace : null);
            _logger.LogError(ex, ex.Message);
            //
            // Return as json
            context.Response.ContentType = "application/json";
            var camelCaseOptions = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            var bodyJson = JsonSerializer.Serialize(error, camelCaseOptions);
            await context.Response.WriteAsync(bodyJson);
        }
    }
}
