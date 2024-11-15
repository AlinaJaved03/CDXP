using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CDXPWeb.Models;
using Newtonsoft.Json;
using System.IO;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;


namespace CDXPWeb.Controllers
{
    public class NPCCREPORTSController : Controller
    {
        // GET: NPCCREPORTS
        // - Report Despatch Instruction - Start
        private clsSQLCore Fn = new clsSQLCore();
        private clsSQLCoreOld FnO = new clsSQLCoreOld();
        [SharePointContextFilter]
        public void sessionCheck()
        {
            string Returnvls = "";

            if (Convert.ToString(Session["UserName"]) == "")
            {
                Session.Add("UserName", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["UserName"]).Value)));
            }
            if (Convert.ToString(Session["usr"]) == "")
            {
                Session.Add("usr", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["usr"]).Value)));
            }

            if (Convert.ToString(Session["UserName"]) != "")
            {
                //Converting Json to a List 

                Returnvls = Fn.Data2Json(@"SELECT CDXP.FN_WP_PORTAL_USERS_ID_BY_USER_NAME('" + Convert.ToString(Session["UserName"]) + @"')");

                var jToken = JToken.Parse(Returnvls);
                var users = jToken.ToObject<List<dynamic>>(); //Converts the Json to a List<Usermodel>
                var user = users[0];

                //JContainer is the base class
                var jObj = (JObject)user;
                string userid = String.Empty;
                foreach (JToken token in jObj.Children())
                {
                    if (token is JProperty)
                    {
                        var prop = token as JProperty;
                        userid = prop.Value.ToString();
                    }
                }
                Session.Add("UserID", userid);
            }
            //if (Convert.ToString(Session["contextTokenString"]) == "")
            //{
            //    Session.Add("contextTokenString", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["contextTokenString"]).Value)));
            //}
            //if (Convert.ToString(Session["SPHostUrl"]) == "")
            //{
            //    Session.Add("SPHostUrl", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["SPHostUrl"]).Value)));
            //}

        }

        public void Load_Notifications()
        {
            string notifications = "";
            var userName = Convert.ToString(Session["UserName"]);

            var userType = Convert.ToInt32(Fn.ExenID("SELECT distinct WPU.WP_SETUP_USER_TYPES_ID FROM CDXP.WP_PORTAL_USERS wpu  WHERE wpu.USER_NAME = '" + userName + "'"));

            if (userType == 10)
            {
                notifications = Fn.Data2Json(@"SELECT Notification_ID, nt.Name+CDXP.NOTIFICATIONS.Vendor_Name as Notification,nt.Path ,Table_PK_ID, FORMAT(Created_On,'dd-MMM-yyyy HH:mm') [CreatedOn]
  FROM CDXP.NOTIFICATIONS 
  INNER JOIN CDXP.NOTIFICATION_TYPE nt ON NOTIFICATIONS.Notification_Type_ID = nt.Notification_Type_ID
  INNER JOIN CDXP.AP_SUPPLIERS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.NOTIFICATIONS.Vendor_ID 

  INNER JOIN CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID 
WHERE Is_View='False' and Submission_For='2' and (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')");

                var jToken = JToken.Parse(notifications);
                var users = jToken.ToObject<List<dynamic>>();
                ViewBag.Notifications = users;
                ViewBag.NotificationsCount = users.Count;

            }
            else if ((userType == 7) || (userType == 9))
            {
                notifications = Fn.Data2Json(@"SELECT Notification_ID, nt.Name+CDXP.NOTIFICATIONS.Vendor_Name as Notification,nt.Path ,Table_PK_ID, FORMAT(Created_On,'dd-MMM-yyyy HH:mm') [CreatedOn] FROM CDXP.NOTIFICATIONS   INNER JOIN CDXP.NOTIFICATION_TYPE nt ON NOTIFICATIONS.Notification_Type_ID = nt.Notification_Type_ID
  INNER JOIN CDXP.AP_SUPPLIERS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.NOTIFICATIONS.Vendor_ID 

  INNER JOIN CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID 
 WHERE Is_View='False' and Submission_For='1' and (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')");

                var jToken = JToken.Parse(notifications);
                var users = jToken.ToObject<List<dynamic>>();
                ViewBag.Notifications = users;
                ViewBag.NotificationsCount = users.Count;

            }
            else
            {
                notifications = Fn.Data2Json(@"SELECT Notification_ID, nt.Name+CDXP.NOTIFICATIONS.Vendor_Name as Notification,nt.Path ,Table_PK_ID, FORMAT(Created_On,'dd-MMM-yyyy HH:mm') [CreatedOn] FROM CDXP.NOTIFICATIONS   INNER JOIN CDXP.NOTIFICATION_TYPE nt ON NOTIFICATIONS.Notification_Type_ID = nt.Notification_Type_ID
  INNER JOIN CDXP.AP_SUPPLIERS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.NOTIFICATIONS.Vendor_ID 

  INNER JOIN CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID 
 WHERE Is_View='False' and Submission_For='3'");

                var jToken = JToken.Parse(notifications);
                var users = jToken.ToObject<List<dynamic>>();
                ViewBag.Notifications = users;
                ViewBag.NotificationsCount = users.Count;

            }




        }

        // [HttpPost]
        public ActionResult DespatchInstructionReportList()
        {
            sessionCheck(); Load_Notifications();

            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");
            ViewBag.currentDate = serverDate;
            var frmdata = HttpContext.Request.Form["vls"];
            string id = frmdata;
            var userName = Convert.ToString(Session["UserName"]);

            string vls = HttpUtility.UrlEncode(Fn.Data2Json("EXEC [CDXP].[SP_MENU_BY_USER_ID] '" + Session["UserName"] + "'"));
            ViewBag.LayoutStr = vls;

            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                ViewBag.name = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_NAME;
                ViewBag.designation = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_DESIGNATION;
                ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                ViewBag.IPP_Category = Fn.Data2DropdownSQL(@"SELECT LOOKUP_CODE,MEANING FROM CDXP.FND_LOOKUP_VALUES WHERE LOOKUP_TYPE = 'CPPA_PPA_IPP_CATEGORY_LK' ORDER BY MEANING");
            }
            if (id != null)
            {
                ViewBag.tg = id;
            }
            else
            {
                ViewBag.tg = "0";
            }
            return View();
        }

        // Report Despatch Instruction - End

        // - Report Events - Start

        // [HttpPost]
        public ActionResult EventsReportsList()
        {
            sessionCheck(); Load_Notifications();

            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");
            ViewBag.currentDate = serverDate;
            var frmdata = HttpContext.Request.Form["vls"];
            string id = frmdata;
            var userName = Convert.ToString(Session["UserName"]);

            string vls = HttpUtility.UrlEncode(Fn.Data2Json("EXEC [CDXP].[SP_MENU_BY_USER_ID] '" + Session["UserName"] + "'"));
            ViewBag.LayoutStr = vls;

            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                ViewBag.name = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_NAME;
                ViewBag.designation = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_DESIGNATION;
                ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                ViewBag.IPP_Category = Fn.Data2DropdownSQL(@"SELECT LOOKUP_CODE,MEANING FROM CDXP.FND_LOOKUP_VALUES WHERE LOOKUP_TYPE = 'CPPA_PPA_IPP_CATEGORY_LK' ORDER BY MEANING");
            }
            if (id != null)
            {
                ViewBag.tg = id;
            }
            else
            {
                ViewBag.tg = "0";
            }
            return View();
        }

        // - Report Events - End

        // Despatch Instruction Set - Start

        public ActionResult DespatchInstructionSet()
        {
            sessionCheck(); Load_Notifications();

            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");
            ViewBag.currentDate = serverDate;
            var frmdata = HttpContext.Request.Form["vls"];
            string id = frmdata;
            var userName = Convert.ToString(Session["UserName"]);

            string vls = HttpUtility.UrlEncode(Fn.Data2Json("EXEC [CDXP].[SP_MENU_BY_USER_ID] '" + Session["UserName"] + "'"));
            ViewBag.LayoutStr = vls;

            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                ViewBag.name = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_NAME;
                ViewBag.designation = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_DESIGNATION;
                ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                ViewBag.IPP_Category = Fn.Data2DropdownSQL(@"SELECT LOOKUP_CODE,MEANING FROM CDXP.FND_LOOKUP_VALUES WHERE LOOKUP_TYPE = 'CPPA_PPA_IPP_CATEGORY_LK' ORDER BY MEANING");
            }
            if (id != null)
            {
                ViewBag.tg = id;
            }
            else
            {
                ViewBag.tg = "0";
            }
            return View();
        }

        // Despatch Instruction Set Code - End

        // Load Curtailment List - START

        // [HttpPost]
        public ActionResult LoadCurtailmentReport()
        {
            sessionCheck(); Load_Notifications();
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");
            ViewBag.currentDate = serverDate;

            string vls = HttpUtility.UrlEncode(Fn.Data2Json("EXEC [CDXP].[SP_MENU_BY_USER_ID] '" + Session["UserName"] + "'"));
            ViewBag.LayoutStr = vls;

            var frmdata = HttpContext.Request.Form["vls"];
            string id = frmdata;
            var userName = Convert.ToString(Session["UserName"]);
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                ViewBag.name = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_NAME;
                ViewBag.designation = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_DESIGNATION;
                ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
            }
            if (id != null)
            {
                ViewBag.tg = id;
            }
            else
            {
                ViewBag.tg = "0";
            }
            return View();
        }

        // Load Curtailment Report - END
    }
}