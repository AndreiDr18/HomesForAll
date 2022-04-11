using HomesForAll.Utils.CustomExceptionUtil;
using Microsoft.AspNetCore.Http;
using HomesForAll.Utils.ServerResponse;
using Serilog;
using HomesForAll.Utils.ServerResponse.Models;
using System.Net;

namespace HomesForAll.Utils.Middleware
{
    public class ExceptionManager
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionManager(RequestDelegate next, ILogger logger)
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
            catch(Exception ex)
            {
                int statusCode;
                string message;
                bool success = false;
                
                _logger.Error("Encountered error with message {@Message}, innerException: {@InnerException}, stack trace: {@StackTrace}", ex.Message, ex.InnerException, ex.StackTrace);

                if (ex is CustomException customException)
                {
                    statusCode = (int)customException.StatusCode;
                    message = customException.Message;
                    if(customException.Success != null)
                        success = customException.Success.Value;
                    if(statusCode == (int)HttpStatusCode.InternalServerError)
                        _logger.Fatal("Unexpected error with message: {@Message}, integral exception: {@ex}", customException.Message, customException);
                }
                else
                {
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    message = "Internal Server Error";
                    _logger.Fatal("Unexpected error with message: {@Message}, integral exception: {@ex}", ex.Message, ex);
                }


                var res = new ResponseBase<EmptyResponseModel>
                {
                    Success = success,
                    Message = message
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(res.ToJson());
            }
        }

    }
}
