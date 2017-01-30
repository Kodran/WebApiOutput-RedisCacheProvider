using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Output.RedisCache.Models
{
    public class Client
    {
        public int clientId { get; set; }
        public string name { get; set; }
        public string birthDate { get; set; }
    }
}