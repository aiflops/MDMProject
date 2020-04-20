using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MDMProject.Models;
using MDMProject.Data;
using MDMProject.Services.Identity;
using System.Net.Mail;
using System.Configuration;
using MDMProject.Resources;
using System.Net;

namespace MDMProject
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            // Get configuration settings
            var emailFrom = ConfigurationManager.AppSettings["EmailFrom"];
            var emailSmtpHost = ConfigurationManager.AppSettings["EmailSmtpHost"];
            var emailSmtpPort = int.Parse(ConfigurationManager.AppSettings["EmailSmtpPort"]);
            var emailUserName = ConfigurationManager.AppSettings["EmailUserName"];
            var emailPassword = ConfigurationManager.AppSettings["EmailPassword"];
            var emailEnableSSL = bool.Parse(ConfigurationManager.AppSettings["EmailEnableSSL"]);

            using (SmtpClient client = new SmtpClient())
            {
                // Configure smtp server
                client.Host = emailSmtpHost;
                client.Port = emailSmtpPort;
                client.Credentials = new NetworkCredential(emailUserName, emailPassword);
                client.EnableSsl = emailEnableSSL;

                // Create message
                var from = new MailAddress(emailFrom, EmailResources.EmailFrom);

                var mailMessage = new MailMessage();
                
                mailMessage.From = from;
                mailMessage.Sender = from;
                mailMessage.To.Add(message.Destination);
                mailMessage.Subject = message.Subject;
                mailMessage.Body = message.Body;
                mailMessage.IsBodyHtml = true;

                // Send message
                await client.SendMailAsync(mailMessage);
            }
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<User, int>
    {
        public ApplicationUserManager(IUserStore<User, int> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<User, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<User, int>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<User, int>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            return UserIdentityGenerator.GenerateUserIdentityAsync(user, (ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
