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
    public class WhenMakingRequestWithoutHmacMessageHandler
    {
        HttpClient client;

        FamilyMember sheila = new FamilyMember { Name = "Sheila Hidey", Relationship = "Wife" };
        FamilyMember kayla = new FamilyMember { Id = 2, Name = "Kayla Hidey", Relationship = "Daughter" };
        FamilyMember korina = new FamilyMember { Id = 3, Name = "Korina Hidey", Relationship = "Daughter" };
        FamilyMember doug = new FamilyMember { Id = 4, Name = "Doug Imposter", Relationship = "Unknown" };

        [TestFixtureSetUp]
        public void Setup()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:4321/api/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Test]
        public async void Expect401HttpStatusCodeForGetRequestsAsync()
        { 
            var getResponseUnauthenticated = await client.GetAsync("secure");

            Assert.AreEqual(HttpStatusCode.Unauthorized, getResponseUnauthenticated.StatusCode);
        }

        [Test]
        public async void Expect401HttpStatusCodeForPostRequestsAsync()
        { 
            var postResponseUnauthenticated = await client.PostAsync<FamilyMember>("secure", sheila, new JsonMediaTypeFormatter());
        }

        [Test]
        public async void Expect401HttpStatusCodeForPutRequestsAsync()
        { 
            var putResponseUnauthenticated = await client.PutAsync<FamilyMember>("secure/2", kayla, new JsonMediaTypeFormatter());
        }

        [Test]
        public async void Expect401HttpStatusCodeForDeleteRequestsAsync()
        { 
            var deleteResponseUnauthenticated = await client.DeleteAsync("secure/4");
        }

    }
}
