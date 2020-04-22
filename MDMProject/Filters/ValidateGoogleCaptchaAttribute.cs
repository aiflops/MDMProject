using MDMProject.Helpers;
using MDMProject.Resources;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web.Mvc;

namespace MDMProject.Filters
{
    public class ValidateGoogleCaptchaAttribute : ActionFilterAttribute
    {
        private ILogger _logger;

        public ValidateGoogleCaptchaAttribute()
        {
            _logger = Log.Logger;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var urlToPost = ConfigurationManager.AppSettings["GoogleRecaptchaVerifyUrl"];
            var secretKey = ConfigurationManager.AppSettings["GoogleRecaptchaSecretKey"];
            var allowedScore = decimal.Parse(ConfigurationManager.AppSettings["GoogleRecaptchaAllowedScore"], CultureInfo.InvariantCulture);
            var captchaResponse = filterContext.HttpContext.Request.Form[GoogleCaptchaHelper.TOKEN_FIELD_NAME];

            if (string.IsNullOrWhiteSpace(captchaResponse))
            {
                LogWarning("Captcha response is empty");
                AddErrorAndRedirectToGetAction(filterContext);
            }

            var tokenResponse = ValidateFromGoogle(urlToPost, secretKey, captchaResponse);
            if (!tokenResponse.Success)
            {
                LogWarning("Token response is unsuccessful; Error codes: " + string.Join(", ", tokenResponse.ErrorCodes));
                AddErrorAndRedirectToGetAction(filterContext);
            }
            else if (tokenResponse.Score < allowedScore)
            {
                LogWarning("Score is less than allowed; Allowed: " + allowedScore + "; Actual: " + tokenResponse.Score);
                AddErrorAndRedirectToGetAction(filterContext);
            }

            base.OnActionExecuting(filterContext);
        }

        private static void AddErrorAndRedirectToGetAction(ActionExecutingContext filterContext)
        {
            filterContext.Controller.ViewData.ModelState.AddModelError("", ValidationMessages.CaptchaIsIncorrect);
        }

        private static ReCaptchaResponse ValidateFromGoogle(string urlToPost, string secretKey, string captchaResponse)
        {
            var postData = "secret=" + secretKey + "&response=" + captchaResponse;

            var request = (HttpWebRequest)WebRequest.Create(urlToPost);
            request.Method = "POST";
            request.ContentLength = postData.Length;
            request.ContentType = "application/x-www-form-urlencoded";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                streamWriter.Write(postData);

            string result;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                    result = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<ReCaptchaResponse>(result);
        }

        private void LogWarning(string message)
        {
            _logger.Warning(message);
        }

        private class ReCaptchaResponse
        {
            // Response Model from Google Recaptcha V3 Verify API
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("score")]
            public decimal Score { get; set; }

            [JsonProperty("action")]
            public string Action { get; set; }

            [JsonProperty("error-codes")]
            public List<string> ErrorCodes { get; set; }
        }
    }
}