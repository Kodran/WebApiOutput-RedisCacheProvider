using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebAPI.Output.RedisCache.Controllers
{
    public class ClientController : ApiController
    {
        [RedisCacheProvider.CacheOutput(ServerExpirationTime = 40)]
        public IHttpActionResult Get(int clientId)
        {
            var response = new { clientId = clientId };
            return Ok(response);
        }
        [RedisCacheProvider.CacheOutput(ServerExpirationTime = 40)]
        public IHttpActionResult Post([FromBody]Models.Client client)
        {            
            return Ok(client);
        }
    }
}
