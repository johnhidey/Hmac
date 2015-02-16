using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hdy.Owin.Security.Hmac
{
    public class HmacAuthenticationMiddleware : AuthenticationMiddleware<HmacAuthenticationOptions>
    {
        private readonly ILogger _logger;

        public HmacAuthenticationMiddleware(OwinMiddleware next,
                                            IAppBuilder app,
                                            HmacAuthenticationOptions options) : base(next, options) 
        {
            _logger = app.CreateLogger<HmacAuthenticationHandler>();
        }

        protected override AuthenticationHandler<HmacAuthenticationOptions> CreateHandler()
        {
            return new HmacAuthenticationHandler(_logger);
        }
    }
}
