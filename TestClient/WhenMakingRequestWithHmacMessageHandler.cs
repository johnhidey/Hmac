using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TestEntities;

namespace TestClient
{
    [TestFixture]
    public class WhenMakingRequestHmacMessageHandler
    {
        HttpClient client;

        FamilyMember sheila = new FamilyMember { Name = "Sheila Hidey", Relationship = "Wife" };
        FamilyMember kayla = new FamilyMember { Id = 2, Name = "Kayla Hidey", Relationship = "Daughter" };
        FamilyMember korina = new FamilyMember { Id = 3, Name = "Korina Hidey", Relationship = "Daughter" };
        FamilyMember doug = new FamilyMember { Id = 4, Name = "Doug Imposter", Relationship = "Unknown" };

        [TestFixtureSetUp]
        public void Setup()
        {
            var apiKey = "jDEf7bMcJVFnqrPd599aSIbhC0IasxLBpGAJeW3Fzh4=";
            var appId = "abc670d15a584f4baf0ba48455d3b155";

            client = new HttpClient(new HmacSecureMessageHandler(apiKey, appId));
            client.BaseAddress = new Uri("http://localhost:4321/api/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Test]
        public async void Expect200HttpStatusCodeForGetRequestsAsync()
        {
            var getResponseAuthenticated = await client.GetAsync("secure");
            Assert.AreEqual(HttpStatusCode.OK, getResponseAuthenticated.StatusCode);
        }

        [Test]
        public async void Expect201HttpStatusCodeForPostRequestsAsync()
        {
            var postResponseAuthenticated = await client.PostAsync<FamilyMember>("secure", sheila, new JsonMediaTypeFormatter());
            Assert.AreEqual(HttpStatusCode.Created, postResponseAuthenticated.StatusCode);
        }

        [Test]
        public async void Expect200HttpStatusCodeForPutRequestsAsync()
        {
            var putResponseAuthenticated = await client.PutAsync<FamilyMember>("secure/2", kayla, new JsonMediaTypeFormatter());
            Assert.AreEqual(HttpStatusCode.OK, putResponseAuthenticated.StatusCode);
        }

        [Test]
        public async void Expect204HttpStatusCodeForDeleteRequestsAsync()
        {
            var deleteResponseAuthenticated = await client.DeleteAsync("secure/4");
            Assert.AreEqual(HttpStatusCode.NoContent, deleteResponseAuthenticated.StatusCode);
        }

    }
}
