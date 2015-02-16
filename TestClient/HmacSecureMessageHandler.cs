using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestClient
{
    public class HmacSecureMessageHandler : DelegatingHandler
    {
        //Obtained from the server earlier, APIKey MUST be stored securely and in App.Config
        private readonly String _apiKey;
        private readonly String _appId;

        public HmacSecureMessageHandler(String apiKey, String appId)
        {
            _apiKey = apiKey;
            _appId = appId;

            //have to do this for PortableRest
            this.InnerHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true
            };
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            string requestContentBase64String = string.Empty;

            string requestUri = WebUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());

            string requestHttpMethod = request.Method.Method;

            //Calculate UNIX time
            string requestTimeStamp = GetRequestTimeStamp();

            //create random nonce for each request
            string nonce = Guid.NewGuid().ToString("N");

            //Checking if the request contains body, usually will be null wiht HTTP GET and DELETE
            requestContentBase64String = await GetRequestContentHashAsBase64(request, requestContentBase64String);

            //Creating the raw signature string
            byte[] signature = GetRequestSignature(requestContentBase64String, requestUri, requestHttpMethod, requestTimeStamp, nonce);

            SetRequestAuthorizationHeader(request, requestTimeStamp, nonce, signature);

            response = await base.SendAsync(request, cancellationToken);

            return response;
        }

        private static string GetRequestTimeStamp()
        {
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();
            return requestTimeStamp;
        }

        private void SetRequestAuthorizationHeader(HttpRequestMessage request, string requestTimeStamp, string nonce, byte[] signature)
        {
            var secretKeyByteArray = Convert.FromBase64String(_apiKey);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyByteArray))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);
                string requestSignatureBase64String = Convert.ToBase64String(signatureBytes);
                //Setting the values in the Authorization header using custom scheme (amx)
                request.Headers.Authorization = new AuthenticationHeaderValue("hdy", string.Format("{0}:{1}:{2}:{3}", _appId, requestSignatureBase64String, nonce, requestTimeStamp));
            }
        }

        private byte[] GetRequestSignature(string requestContentBase64String, string requestUri, string requestHttpMethod, string requestTimeStamp, string nonce)
        {
            string signatureRawData = String.Format("{0}{1}{2}{3}{4}{5}", _appId, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);

            byte[] signature = Encoding.UTF8.GetBytes(signatureRawData);
            return signature;
        }

        private static async Task<string> GetRequestContentHashAsBase64(HttpRequestMessage request, string requestContentBase64String)
        {
            if (request.Content != null)
            {
                byte[] content = await request.Content.ReadAsByteArrayAsync();
                MD5 md5 = MD5.Create();
                //Hashing the request body, any change in request body will result in different hash, we'll insure message integrity
                byte[] requestContentHash = md5.ComputeHash(content);
                requestContentBase64String = Convert.ToBase64String(requestContentHash);
            }
            return requestContentBase64String;
        }
    }
}
