using Serilog;
using System;
using System.Web.Mvc;

namespace MDMProject.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected ILogger _logger;

        public ControllerBase()
        {
            _logger = Log.Logger;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            //Log the error!!
            var errorId = Guid.NewGuid();
            _logger.Error(filterContext.Exception, "Error occurred ({ErrorId})", errorId);

            ViewResult viewResult = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml"
            };
            viewResult.ViewBag.ErrorId = errorId.ToString();

            filterContext.Result = viewResult;
        }

        protected virtual void LogErrorMessage(string errorMessage)
        {
            var errorId = Guid.NewGuid();
            _logger.Error("Error occurred ({ErrorId}): " + errorMessage, errorId);
        }

        protected virtual string LogError(Exception ex)
        {
            //Log the error!!
            var errorId = Guid.NewGuid();
            _logger.Error(ex, "Error occurred ({ErrorId})", errorId);

            return errorId.ToString();
        }

        protected virtual ViewResult GetErrorView(Exception ex)
        {
            var errorId = Guid.NewGuid();
            _logger.Error(ex, "Error occurred ({ErrorId})", errorId);

            ViewResult viewResult = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml"
            };
            viewResult.ViewBag.ErrorId = errorId.ToString();

            return viewResult;
        }
    }
}