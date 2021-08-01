using System;
using System.Net;
using FileManager.Common.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Task = System.Threading.Tasks.Task;

namespace FileManager.Common.Exceptions
{
    /// <summary>
    /// Exception Handler Middleware
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IConfiguration _configuration;
        private readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        public ExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
            _logger = loggerFactory?.CreateLogger<ExceptionMiddleware>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
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


            var serializerSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            var exception = new
            {
                ex.Message,
                ex.StackTrace,
                ex.InnerException
            };

            var exceptionJson = JsonConvert.SerializeObject(exception, serializerSettings);

            context.Response.ContentType = "application/json";

            string detailedExeptionMessage = $"----------Exception---------{exceptionJson}---------";
            Log.ForContext("Message", exception.Message)
            .Error(detailedExeptionMessage);

            if (typeof(BaseException).IsAssignableFrom(ex.GetType()))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseResult() { Message = ex.Message }));
            }
            else if (typeof(UnauthorizedAccessException).IsAssignableFrom(ex.GetType()))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new ResponseResult
                { 
                    Message = "Unauthorized", 
                    Status = HttpStatusCode.Unauthorized }));
            }
            else
            {

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await context.Response.WriteAsync(JsonConvert.SerializeObject( new ResponseResult()
                {
                    Message = _configuration["Enable_Stack_Trace"] == "TRUE" ? exceptionJson : ex.Message

                }));
            }
        }
    }
}


