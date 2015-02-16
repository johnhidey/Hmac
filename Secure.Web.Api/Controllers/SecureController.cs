using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestEntities;

namespace Secure.Web.Api.Controllers
{
    [Authorize]
    public class SecureController : ApiController
    {
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public HttpResponseMessage Post(FamilyMember member)
        {
            return Request.CreateResponse(HttpStatusCode.Created);
        }

        public HttpResponseMessage Put(FamilyMember member)
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public HttpResponseMessage Delete(Int32 id)
        {
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
