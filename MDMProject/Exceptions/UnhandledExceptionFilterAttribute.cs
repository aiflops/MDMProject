using Serilog;
using System.Web.Http.Filters;

namespace MDMProject.Exceptions
{
    public class UnhandledExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            Log.Logger.Error(context.Exception, "WebAPI error occurred");
        }
    }
}