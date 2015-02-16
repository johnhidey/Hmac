using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hdy.Owin.Security.Hmac
{
    public static class HmacAuthenticationExtensions
    {
        public static IAppBuilder UseHmacAuthentication(this IAppBuilder app, HmacAuthenticationOptions options)
        {
            app.Use(typeof(HmacAuthenticationMiddleware), app, options);
            return app;
        }
    }
}
