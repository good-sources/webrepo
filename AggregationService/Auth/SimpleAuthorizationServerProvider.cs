namespace AggregationService.Auth
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using NLog;
    using Microsoft.Owin.Security.OAuth;

    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.CompletedTask;
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            if (!AuthenticationService.ValidateCredentials(context.UserName, context.Password))
            {
                Logger.Warn("Authentication failed for user '{Username}'", context.UserName);
                context.SetError("invalid_grant", "The username or password is incorrect.");
                return Task.CompletedTask;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, "User"));

            context.Validated(identity);
            Logger.Info("User '{Username}' authenticated successfully", context.UserName);
            return Task.CompletedTask;
        }
    }
}
