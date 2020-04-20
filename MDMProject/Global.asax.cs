using MDMProject.Data;
using Serilog;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MDMProject
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Database.SetInitializer(new DatabaseInitializer());

            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.MSSqlServer(ConfigurationManager.ConnectionStrings["ApplicationDbConnection"].ConnectionString, "Logs", autoCreateSqlTable: true, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
                .Enrich.WithHttpRequestClientHostIP()
                .Enrich.WithHttpRequestClientHostName()
                .Enrich.WithHttpRequestRawUrl()
                .Enrich.WithHttpRequestType()
                .Enrich.WithHttpRequestUrl()
                .Enrich.WithHttpRequestUrlReferrer()
                .Enrich.WithHttpRequestUserAgent()
                .Enrich.WithMvcActionName()
                .Enrich.WithMvcControllerName()
                .Enrich.WithMvcRouteData()
                .Enrich.WithMvcRouteTemplate()
                .Enrich.WithUserName()
                .CreateLogger();
        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            //log the error!
            Log.Logger.Error(ex, "Global application error occured");
        }
    }
}