using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hdy.Owin.Security.Hmac
{
    public class HmacAuthentication
    {
        protected HmacAuthenticationOptions Options;

        public HmacAuthentication(HmacAuthenticationOptions options)
        {
            Options = options;
        }

        public Boolean Validate(IOwinRequest request)
        {
            var header = request.Headers.Get("authorization");
            var authenticationHeader = AuthenticationHeaderValue.Parse(header);
            if (Options.Scheme.Equals(authenticationHeader.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                var rawAuthenticationHeader = authenticationHeader.Parameter;
                var authenticationHeaderArray = GetAuthenticationValues(rawAuthenticationHeader);

                if (authenticationHeaderArray != null)
                {
                    var AppId = authenticationHeaderArray[0];
                    var incomingBase64Signature = authenticationHeaderArray[1];
                    var nonce = authenticationHeaderArray[2];
                    var requestTimeStamp = authenticationHeaderArray[3];

                    var isValid = isValidRequest(request, AppId, incomingBase64Signature, nonce, requestTimeStamp);
                }
            }

            return true;
        }

        private bool isValidRequest(IOwinRequest req, string AppId, string incomingBase64Signature, string nonce, string requestTimeStamp)
        {
            string requestContentBase64String = "";
            string requestUri = WebUtility.UrlEncode(req.Uri.AbsoluteUri.ToLower());
            string requestHttpMethod = req.Method;

            if (!Options.AllowedApps.ContainsKey(AppId))
            {
                return false;
            }

            var sharedKey = Options.AllowedApps[AppId];

            if (IsReplayRequest(nonce, requestTimeStamp))
            {
                return false;
            }

            byte[] hash = ComputeHash(req.Body);

            if (hash != null)
            {
                requestContentBase64String = Convert.ToBase64String(hash);
            }

            string data = String.Format("{0}{1}{2}{3}{4}{5}", AppId, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);

            var secretKeyBytes = Convert.FromBase64String(sharedKey);

            byte[] signature = Encoding.UTF8.GetBytes(data);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);

                return (incomingBase64Signature.Equals(Convert.ToBase64String(signatureBytes), StringComparison.Ordinal));
            }

        }

        private string[] GetAuthenticationValues(string rawAuthenticationHeader)
        {
            var credArray = rawAuthenticationHeader.Split(':');

            if (credArray.Length == 4)
            {
                return credArray;
            }
            else
            {
                return null;
            }
        }

        private bool IsReplayRequest(string nonce, string requestTimeStamp)
        {
            if (MemoryCache.Default.Contains(nonce))
            {
                return true;
            }

            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan currentTs = DateTime.UtcNow - epochStart;

            var serverTotalSeconds = Convert.ToUInt64(currentTs.TotalSeconds);
            var requestTotalSeconds = Convert.ToUInt64(requestTimeStamp);

            if ((serverTotalSeconds - requestTotalSeconds) > Options.MaxRequestAgeInSeconds)
            {
                return true;
            }

            MemoryCache.Default.Add(nonce, requestTimeStamp, DateTimeOffset.UtcNow.AddSeconds(Options.MaxRequestAgeInSeconds));

            return false;
        }

        private static byte[] ComputeHash(Stream body)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = null;
                var content = ReadFully(body);
                if (content.Length != 0)
                {
                    hash = md5.ComputeHash(content);
                }
                return hash;
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static AuthenticationTicket EmptyTicket()
        {
            return new AuthenticationTicket(null, (AuthenticationProperties)null);
        }

    }
}
