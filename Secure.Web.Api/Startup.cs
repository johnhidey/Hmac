using Hdy.Owin.Security.Hmac;
using Microsoft.Owin.Security;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Secure.Web.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.SetLoggerFactory(new ConsoleLoggerFactory());

            app.UseHmacAuthentication(new HmacAuthenticationOptions
            {
                Scheme = "hdy",
                AuthenticationMode = AuthenticationMode.Active,
                MaxRequestAgeInSeconds = 300,
                AllowedApps = new Dictionary<string, string> {
                    { "abc670d15a584f4baf0ba48455d3b155", "jDEf7bMcJVFnqrPd599aSIbhC0IasxLBpGAJeW3Fzh4=" }
                }
            });

        }
    }
}