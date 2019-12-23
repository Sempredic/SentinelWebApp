using SentinelWebApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace SentinelWebApp.Controllers
{
    [Log]
    public class ValuesController : Controller
    {

        
        public ValuesController()
        {
            MongoCRUD.GetInstance().ConnectToDB("Serials");

            if (MongoCRUD.GetInstance().DBConnectionStatus())
            {
                Trace.WriteLine("CONNECTED BROHAM");
            }
            else
            {
                Trace.WriteLine("NOT NOT NOT NOT CONNECTED BROHAM");
            }
        }

        // GET api/values

        public List<AreaInfo> Get(string id)
        {


            List<AreaInfo> areas = MongoCRUD.GetInstance().LoadRecords<AreaInfo>(id, null, null);
     
            Trace.WriteLine("IM ACCESED");

            return areas;
        }
        

        // POST api/values
        public void Post([FromBody]string value)
        {

        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {

        }


    }
}
