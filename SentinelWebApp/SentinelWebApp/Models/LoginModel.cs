using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SentinelWebApp.Models
{
    
    public class LoginModel
    {
        [BsonId]
        public Guid ID;
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Message { get; set; }
    }
}