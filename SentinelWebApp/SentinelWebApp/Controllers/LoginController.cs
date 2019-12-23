using SentinelWebApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace SentinelWebApp.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {

            return View();
        }

      
        public ActionResult Authenticate(LoginModel user)
        {
            
            MongoCRUD.GetInstance().ConnectToDB("Serials");

            if (MongoCRUD.GetInstance().DBConnectionStatus())
            {
                Trace.WriteLine("CONNECTED BROHAM");

           
                if (MongoCRUD.GetInstance().RecordExists<LoginModel>("Users",user.UserName, "UserName"))
                {
                    List<LoginModel> users = MongoCRUD.GetInstance().LoadRecords<LoginModel>("Users", "UserName", user.UserName);

                    if (users[0].Password == user.Password)
                    {
                        return Redirect("/Main");

                    }
                    else
                    {
                        user.Message = "Invalid UserName/Password";
                        return RedirectToAction("Index");

                    }


                }
                else
                {
                    user.Message = "Invalid UserName/Password";
                    return RedirectToAction("Index");
                }
              
                //MongoCRUD.GetInstance().InsertRecord<LoginModel>("Users", user, user.UserName, null);

            }
            else
            {
                Trace.WriteLine("NOT NOT NOT NOT CONNECTED BROHAM");
                return RedirectToAction("Index");
            }
        }


    }
}