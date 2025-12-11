using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VideoGameStore.Dtos;

namespace VideoGameStore.Exceptions
{

    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {

            var response = context.Exception switch
            {
                EntityNotFound => new ApiErrorResponse(404, context.Exception.Message),
                _ => new ApiErrorResponse( 500, "Internal server error")
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };

            context.ExceptionHandled = true;
        }
    }
}
