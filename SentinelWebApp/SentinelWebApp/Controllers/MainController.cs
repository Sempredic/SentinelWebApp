using Quobject.SocketIoClientDotNet.Client;
using SentinelWebApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SentinelWebApp.Controllers
{
    public class MainController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Main Page";

            dynamic model = new ExpandoObject();
            model.SerialInfoModel = new SerialInfo();



            return View(model);
        }

        public ActionResult GetShit(string id)
        {
            List<AreaInfo> areas = null;
            MongoCRUD.GetInstance().ConnectToDB("Serials");

            if (MongoCRUD.GetInstance().DBConnectionStatus())
            {
                areas = MongoCRUD.GetInstance().LoadRecords<AreaInfo>(id, null, null);

                Trace.WriteLine("IM ACCESED " + id);
            }


            return Json(areas, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSerialInfo(string serialID)
        {
            Trace.WriteLine("IM ACCESED SEralinfo " + serialID);
            List<Dictionary<string, string>> li = new List<Dictionary<string, string>>();
            List<SerialInfo> list = null;

            MongoCRUD.GetInstance().ConnectToDB("Serials");

            if (MongoCRUD.GetInstance().DBConnectionStatus())
            {
                list = MongoCRUD.GetInstance().LoadRecords<SerialInfo>("Serial", "serial", serialID);

                if (list.Count != 0)
                {
                    foreach (SerialInfo si in list)
                    {
                        List<LocationData> tempLD = si.locationData;
                        Trace.WriteLine(tempLD);
                        foreach (LocationData ld in tempLD)
                        {
                            Dictionary<string, string> item = new Dictionary<string, string>();
                            item.Add("Date", ld.date);
                            item.Add("CaseID", ld.curCase);
                            item.Add("Location", ld.location);
                            item.Add("UserID", ld.userID);
                            item.Add("Time", ld.time);
                            item.Add("LastLoc", ld.lastLocation.ToString());

                            li.Add(item);
                        }

                    }

                }

            }



            return Json(li, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveSerial(string serialID)
        {
            bool success = false;

            if (MongoCRUD.GetInstance().RecordExists<SerialInfo>("Serial", serialID, "serial"))
            {
                List<SerialInfo>list = MongoCRUD.GetInstance().LoadRecords<SerialInfo>("Serial", "serial", serialID);

                if (list!=null)
                {
                    SerialInfo si = list[0];

                    success = MongoCRUD.GetInstance().RemoveRecord<SerialInfo>("Serial", si.ID);

                }
            }

            return Json(success, JsonRequestBehavior.AllowGet);
        }

        public void CheckInCase(string areaName, string locName, string caseID, string serialID)
        {
            SerialInfo si = new SerialInfo();

            si.serial = serialID;

            LocationData d = new LocationData();
            d.curCase = caseID;
            d.date = DateTime.UtcNow.Date.ToString("dd/MM/yyyy");
            d.time = DateTime.Now.ToString("h:mm:ss tt");
            d.location = areaName;
            d.lastLocation = true;
            d.userID = "311015";


            si.locationData.Add(d);



            if (MongoCRUD.GetInstance().RecordExists<SerialInfo>("Serial", serialID, "serial"))
            {
                MongoCRUD.GetInstance().AppendRecord<SerialInfo>("Serial", serialID, d);
            }
            else
            {


                MongoCRUD.GetInstance().InsertRecord("Serial", si, serialID, caseID);
            }

            CaseInfo ci = new CaseInfo();
            ci.caseID = caseID;
            ci.curLoc = areaName;
            ci.ageInfo = DateTime.Now.ToString("MM-dd-yyyy hh: mm tt");

            MongoCRUD.GetInstance().InsertRecord("Cases", ci, caseID, null);

            List<AreaInfo> areas = MongoCRUD.GetInstance().LoadRecords<AreaInfo>("Areas", "areaName", areaName);

            if (areas.Count != 0)
            {
                foreach (LocationObject lo in areas[0].locationsList)
                {
                    if (lo.locName == ci.curLoc)
                    {
                        if (!lo.casesList.Contains(ci))
                        {
                            MongoCRUD.GetInstance().UpdateLocationCases(lo, areas[0], ci);
                        }
                    }

                }

            }

        }

        public void ValidateCase(string areaName, string locName, string caseID, List<string> serialList)
        {
            

            foreach (string item in serialList)
            {
                SerialInfo si = new SerialInfo();

                si.serial = item;

                LocationData d = new LocationData();
                d.curCase = caseID;
                d.date = DateTime.UtcNow.Date.ToString("dd/MM/yyyy");
                d.time = DateTime.Now.ToString("h:mm:ss tt");
                d.location = locName;
                d.lastLocation = true;
                d.userID = "311015";


                si.locationData.Add(d);



                if (MongoCRUD.GetInstance().RecordExists<SerialInfo>("Serial", item, "serial"))
                {
                    MongoCRUD.GetInstance().AppendRecord<SerialInfo>("Serial", item, d);
                }
                else
                {


                    MongoCRUD.GetInstance().InsertRecord("Serial", si, item, caseID);
                }

            }

            CaseInfo ci = new CaseInfo();
            ci.caseID = caseID;
            ci.curLoc = locName;
            ci.ageInfo = DateTime.Now.ToString("MM-dd-yyyy hh: mm tt");

            MongoCRUD.GetInstance().InsertRecord("Cases", ci, caseID, null);

            List<AreaInfo> areas = MongoCRUD.GetInstance().LoadRecords<AreaInfo>("Areas", "areaName", areaName);

            if (areas.Count != 0)
            {
                foreach (LocationObject lo in areas[0].locationsList)
                {
                    if (lo.locName == ci.curLoc)
                    {
                        if (!lo.casesList.Contains(ci))
                        {
                            MongoCRUD.GetInstance().UpdateLocationCases(lo, areas[0], ci);
                        }
                    }

                }

            }

        }

        public ActionResult GetCaseSerials(string caseID)
        {
            

            List<SerialInfo> list = MongoCRUD.GetInstance().LoadRecords<SerialInfo>("Serial", "caseID", caseID);

            foreach (SerialInfo li in list)
            {
                Trace.WriteLine(li.serial);
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerifyCaseExists(string caseID)
        {
            Trace.WriteLine("VERIFYCASE CALLED " + caseID);
            bool exists = false;
            List<CaseInfo> cases = null;

            if (MongoCRUD.GetInstance().DBConnectionStatus())
            {
                cases = MongoCRUD.GetInstance().LoadRecords<CaseInfo>("Cases", "caseID", caseID);

                if (cases != null)
                {
                    foreach (CaseInfo ci in cases)
                    {
                        if (ci.caseID == caseID)
                        {
                            exists = true;
                        }

                        Trace.WriteLine(ci.caseID);
                    }
                }
            }


            return Json(exists, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerifyLocation(string areaName, string locName)
        {
            Trace.WriteLine("I AM ACCESSED VERIFY");
            bool exists = false;
            List<AreaInfo> areas = null;

            if (MongoCRUD.GetInstance().DBConnectionStatus())
            {
                areas = MongoCRUD.GetInstance().LoadRecords<AreaInfo>("Areas", "areaName", areaName);

                if (areas!=null)
                {
                    foreach (AreaInfo ai in areas)
                    {
                        foreach (LocationObject ld in ai.locationsList)
                        {
                            if (ld.locName == locName)
                            {
                                exists = true;
                            }
                        }

                        Trace.WriteLine(ai.areaName);
                    }
                }
            }

            return Json(exists, JsonRequestBehavior.AllowGet);
        }

    }
}
