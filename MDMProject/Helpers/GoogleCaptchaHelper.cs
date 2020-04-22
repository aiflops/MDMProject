using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace MDMProject.Helpers
{
    public static class GoogleCaptchaHelper
    {
        public const string TOKEN_FIELD_NAME = "__GoogleCaptchaToken";

        public static IHtmlString GoogleCaptchaLibraryScript(this HtmlHelper helper)
        {
            string scriptUrl = ConfigurationManager.AppSettings["GoogleRecaptchaUrl"];
            string publicSiteKey = ConfigurationManager.AppSettings["GoogleRecaptchaSiteKey"];

            return MvcHtmlString.Create($"<script src={scriptUrl}{publicSiteKey}></script>");
        }

        public static IHtmlString GoogleCaptchaVerifyScript(this HtmlHelper helper, string action = "send_form")
        {
            var publicSiteKey = ConfigurationManager.AppSettings["GoogleRecaptchaSiteKey"];

            var execScript = $"<script>\n" +
                $"grecaptcha.ready(function() {{\n" +
                $"grecaptcha.execute('{publicSiteKey}', {{ action: '{action}'}}).then(function(token) {{\n" +
                $"document.getElementById('{TOKEN_FIELD_NAME}').value = token;\n" +
                $"\n}});\n" +
                $"\n}});\n" +
                $"</script>\n";

            return MvcHtmlString.Create($"{execScript}\n");
        }

        public static IHtmlString GoogleCaptchaToken(this HtmlHelper helper)
        {
            var inputTag = $"<input type=\"hidden\" id=\"{TOKEN_FIELD_NAME}\" name=\"{TOKEN_FIELD_NAME}\" value=\"\" />";

            return MvcHtmlString.Create($"{inputTag}\n");
        }
    }
}