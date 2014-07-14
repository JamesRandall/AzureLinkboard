using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using AzureLinkboard.Web.Api.Providers;
using AzureLinkboard.Web.Api.Models;

namespace AzureLinkboard.Web.Api
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            ApplicationOAuthProvider oauthProvider = new ApplicationOAuthProvider(PublicClientId);

            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = oauthProvider,
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true,
                
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            /*string facebookAppId = ConfigurationManager.AppSettings["com.hushhush.external-identity-providers.setting.facebook-app-id"];
            string facebookAppSecret = ConfigurationManager.AppSettings["com.hushhush.external-identity-providers.setting.facebook-app-secret"];
            FacebookAuthenticationOptions options = new FacebookAuthenticationOptions
            {
                AppId = facebookAppId,
                AppSecret = facebookAppSecret,
                SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType()
            };
            options.Scope.Add("email");
            options.Scope.Add("publish_actions");
            app.UseFacebookAuthentication(options);*/

            //app.UseGoogleAuthentication();
        }
    }
}
