using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hdy.Owin.Security.Hmac
{
    public class HmacAuthenticationOptions : AuthenticationOptions
    {
        public const String authenticationType = "HMAC";

        public HmacAuthenticationOptions() : base(authenticationType) 
        {
            MaxRequestAgeInSeconds = 300;
            AuthenticationMode = AuthenticationMode.Active;
        }

        public String Scheme { get; set; }

        public UInt64 MaxRequestAgeInSeconds { get; set; }

        public Dictionary<String, String> AllowedApps { get; set; }
    }
}
