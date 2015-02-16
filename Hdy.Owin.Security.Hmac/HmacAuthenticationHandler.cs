using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hdy.Owin.Security.Hmac
{
    public class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationOptions>
    {
        private readonly ILogger _logger;
        private readonly UInt64 requestMaxAgeInSeconds = 300;  //5 mins

        public HmacAuthenticationHandler(ILogger logger)
        {
            _logger = logger;
        }

        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            AuthenticationTicket ticket = HmacAuthentication.EmptyTicket();

            var header = Request.Headers.Get("authorization");
            if (header != null)
            {
                var hmac = new HmacAuthentication(Options);
                var valid = hmac.Validate(Request);

                if (valid)
                {
                    var principal = new ClaimsPrincipal(new ClaimsIdentity("HMAC"));
                    ticket = new AuthenticationTicket((ClaimsIdentity)principal.Identity, null);
                }
            }

            return Task.FromResult(ticket);
        }

        protected override Task ApplyResponseChallengeAsync()
        {
            return base.ApplyResponseChallengeAsync();
        }

    }
}
