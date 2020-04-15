using Serilog;
using System.Linq;
using System.Web.Http;

namespace MDMProject.WebAPI_Controllers
{
    public class LogController : ApiController
    {
        [HttpPost]
        public void Post(LogViewModel viewModel)
        {
            string message = string.Format("Error: {0}\nurl:{1}\nline: {2}\ncolumn: {3}\nstack: {4}\n", viewModel.msg, viewModel.url, viewModel.line, viewModel.col, viewModel.stack);
            if (viewModel.extra != null)
            {
                message += string.Join("\n", viewModel.extra.Select(x => x.Key + ": " + x.Value ?? ""));
            }

            Log.Logger.Error("JS Error occured: " + message, viewModel);
        }
    }
}