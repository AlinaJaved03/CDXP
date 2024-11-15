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
using System.Data.Entity;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace CDXPWeb.Controllers
{
    public class NPCCController : Controller
    {

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
            if (userName != "")
            {
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

        }

        public void LoadNotificationReadAll()
        {
            Fn.Exec(@"update CDXP.NOTIFICATIONS Is_View='True' where ");

        }

        // GET: NPCC
        public ActionResult Index()
        {
            sessionCheck(); Load_Notifications();

            return View();
        }


        public ActionResult NPCCDashboard()
        {
            sessionCheck(); Load_Notifications();

            return View();
        }

        public ActionResult ConsolidatedAvailabilityAndDR()
        {
            sessionCheck(); Load_Notifications();

            return View();
        }

        public ActionResult DailyOperationStatusReport()
        {
            sessionCheck(); Load_Notifications();

            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");
            ViewBag.currentDate = serverDate;

            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
            }

            return View();
        }




        [HttpPost]
        public ActionResult DespatchInstructionList()
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
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                ViewBag.name = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_NAME;
                ViewBag.designation = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_DESIGNATION;
                ViewBag.IPP = Fn.Data2DropdownSQL(@"WITH temp_table AS (SELECT DISTINCT LOOKUP_CODE,MEANING,ORDER_IPPCAT FROM CDXP.FND_LOOKUP_VALUES 
      INNER JOIN CDXP.PPA_HEADER ON CDXP.PPA_HEADER.IPP_CATEGORY = CDXP.FND_LOOKUP_VALUES.LOOKUP_CODE 
      INNER JOIN CDXP.AP_SUPPLIERS ON  CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.PPA_HEADER.VENDOR_ID_FK
      INNER JOIN CDXP.AP_SUPPLIER_SITE_ALL ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID=CDXP.PPA_HEADER.VENDOR_SITE_ID_FK
      INNER JOIN
      CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
      CDXP.PPA_HEADER.VENDOR_SITE_ID_FK  = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID    

      WHERE CDXP.FND_LOOKUP_VALUES.LOOKUP_TYPE = 'CPPA_PPA_IPP_CATEGORY_LK' and LOOKUP_CODE!='50' AND LOOKUP_CODE!='60'
                            AND (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')

       ) SELECT LOOKUP_CODE,MEANING FROM temp_table ORDER BY ORDER_IPPCAT");

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


        [HttpPost]
        public ActionResult DICreate()
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
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                ViewBag.name = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_NAME;
                ViewBag.designation = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_DESIGNATION;
            }
            if (id != null)
            {
                ViewBag.tg = id;
                ViewBag.IPP = Fn.Data2DropdownSQL(@"WITH temp_table AS (SELECT DISTINCT LOOKUP_CODE,MEANING,ORDER_IPPCAT FROM CDXP.FND_LOOKUP_VALUES 
      INNER JOIN CDXP.PPA_HEADER ON CDXP.PPA_HEADER.IPP_CATEGORY = CDXP.FND_LOOKUP_VALUES.LOOKUP_CODE 
      INNER JOIN CDXP.AP_SUPPLIERS ON  CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.PPA_HEADER.VENDOR_ID_FK
      INNER JOIN CDXP.AP_SUPPLIER_SITE_ALL ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID=CDXP.PPA_HEADER.VENDOR_SITE_ID_FK
      INNER JOIN
      CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
      CDXP.PPA_HEADER.VENDOR_SITE_ID_FK  = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID    

      WHERE CDXP.FND_LOOKUP_VALUES.LOOKUP_TYPE = 'CPPA_PPA_IPP_CATEGORY_LK' and LOOKUP_CODE!='50' AND LOOKUP_CODE!='60'
                            AND (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"') 
        )SELECT LOOKUP_CODE,MEANING FROM temp_table ORDER BY ORDER_IPPCAT");

            }
            else
            {
                ViewBag.tg = "0";
                ViewBag.IPP = Fn.Data2DropdownSQL(@"WITH temp_table AS (SELECT DISTINCT LOOKUP_CODE,MEANING,ORDER_IPPCAT FROM CDXP.FND_LOOKUP_VALUES 
      INNER JOIN CDXP.PPA_HEADER ON CDXP.PPA_HEADER.IPP_CATEGORY = CDXP.FND_LOOKUP_VALUES.LOOKUP_CODE 
      INNER JOIN CDXP.AP_SUPPLIERS ON  CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.PPA_HEADER.VENDOR_ID_FK
      INNER JOIN CDXP.AP_SUPPLIER_SITE_ALL ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID=CDXP.PPA_HEADER.VENDOR_SITE_ID_FK
      INNER JOIN
      CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
      CDXP.PPA_HEADER.VENDOR_SITE_ID_FK  = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID    

      WHERE CDXP.FND_LOOKUP_VALUES.LOOKUP_TYPE = 'CPPA_PPA_IPP_CATEGORY_LK' and LOOKUP_CODE!='50' AND LOOKUP_CODE!='60'
                            AND (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')

       ) SELECT LOOKUP_CODE,MEANING FROM temp_table ORDER BY ORDER_IPPCAT");
            }
            return View();
        }
        public ActionResult DespatchInstructionForm()
        {
            sessionCheck(); Load_Notifications();

            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");
            ViewBag.currentDate = serverDate;
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
            }
            return View();
        }

        public ActionResult DespatchInstructionSetSubmission()
        {
            sessionCheck(); Load_Notifications();
            return View();
        }

        public ActionResult FailedToAchieveDespatch()
        {
            sessionCheck(); Load_Notifications();

            var userName = Convert.ToString(Session["UserName"]);
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
            }
            return View();
        }

        public ActionResult FailedToAchieveDespatchCreate()
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
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                ViewBag.name = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_NAME;
                ViewBag.designation = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_DESIGNATION;
            }

            if (id != null)
            {
                ViewBag.tg = id;

                ViewBag.IPP = Fn.Data2DropdownSQL(@"SELECT LOOKUP_CODE,MEANING FROM CDXP.FND_LOOKUP_VALUES 
          WHERE LOOKUP_TYPE = 'CPPA_PPA_IPP_CATEGORY_LK' ORDER BY MEANING");

            }
            else
            {
                ViewBag.tg = "0";

                ViewBag.IPP = Fn.Data2DropdownSQL(@"SELECT LOOKUP_CODE,MEANING FROM CDXP.FND_LOOKUP_VALUES 
          WHERE LOOKUP_TYPE = 'CPPA_PPA_IPP_CATEGORY_LK' ORDER BY MEANING");

            }

            return View();
        }
        public ActionResult DayAheadDemand()
        {
            sessionCheck(); Load_Notifications();

            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                var userName = Convert.ToString(Session["UserName"]);
                //ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME)
                //    .Join(db.WP_GC_USER_ACCESS,
                //    s => s.VENDOR_ID,
                //    a => a.ENTITY_ID,
                //    (sup, acc) => new { sup.VENDOR_ID, sup.VENDOR_NAME, acc.WP_PORTAL_USERS_ID })
                //    .Join(db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName),
                //    x => x.WP_PORTAL_USERS_ID,
                //    y => y.WP_PORTAL_USERS_ID,
                //    (vend, user) => new { id = vend.VENDOR_ID, vend.VENDOR_NAME }).ToList());
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();

                var userType = Convert.ToInt32(Fn.ExenID("SELECT distinct WPU.WP_SETUP_USER_TYPES_ID FROM CDXP.WP_PORTAL_USERS wpu  WHERE wpu.USER_NAME = '" + userName + "'"));

                if (userType == 10)
                {
                    ViewBag.IPP = Fn.Data2DropdownSQL(@"SELECT 
                                                        CAST(AP_SUPPLIER_SITE_ALL_1.VENDOR_ID AS VARCHAR(100)) +'½'+ CAST           ( AP_SUPPLIER_SITE_ALL_1.VENDOR_SITE_ID  AS VARCHAR(100)),    
                                                        CASE WHEN T .CNT = 1 THEN CDXP.AP_SUPPLIERS.VENDOR_NAME ELSE CDXP.AP_SUPPLIERS.VENDOR_NAME +                        AP_SUPPLIER_SITE_ALL_1.VENDOR_SITE_CODE  END AS VAL 
                                                    FROM            
                                                        (SELECT COUNT(VENDOR_SITE_ID) AS CNT, VENDOR_ID
                                                            FROM CDXP.AP_SUPPLIER_SITE_ALL
                                                      GROUP BY VENDOR_ID) AS T INNER JOIN
                                                     CDXP.AP_SUPPLIERS ON T.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                                                     CDXP.AP_SUPPLIER_SITE_ALL AS AP_SUPPLIER_SITE_ALL_1 ON CDXP.AP_SUPPLIERS.VENDOR_ID = AP_SUPPLIER_SITE_ALL_1.VENDOR_ID JOIN CDXP.PPA_HEADER ph ON ph.VENDOR_ID_FK = CDXP.AP_SUPPLIERS.VENDOR_ID
						                             JOIN CDXP.WP_PORTAL_USERS wpu ON ph.HEADER_ID_PK = wpu.PPA_HEADER_ID_FK 
						                             WHERE wpu.USER_NAME = '" + userName + "' ORDER BY VAL");
                }
                else
                {
                    ViewBag.IPP = Fn.Data2DropdownSQL(@"SELECT     CAST(AP_SUPPLIER_SITE_ALL_1.VENDOR_ID AS VARCHAR(100)) +'½'+ CAST( AP_SUPPLIER_SITE_ALL_1.VENDOR_SITE_ID  AS VARCHAR(100)),    CASE WHEN T .CNT = 1 THEN CDXP.AP_SUPPLIERS.VENDOR_NAME ELSE CDXP.AP_SUPPLIERS.VENDOR_NAME + ' (' + AP_SUPPLIER_SITE_ALL_1.VENDOR_SITE_CODE + ')' END AS VAL 
                      FROM            (SELECT COUNT(VENDOR_SITE_ID) AS CNT, VENDOR_ID
                          FROM            CDXP.AP_SUPPLIER_SITE_ALL
                          GROUP BY VENDOR_ID) AS T INNER JOIN
                         CDXP.AP_SUPPLIERS ON T.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                         CDXP.AP_SUPPLIER_SITE_ALL AS AP_SUPPLIER_SITE_ALL_1 ON CDXP.AP_SUPPLIERS.VENDOR_ID = AP_SUPPLIER_SITE_ALL_1.VENDOR_ID
						 ORDER BY VAL");
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult DayAheadNotificationForm()
        {
            sessionCheck(); Load_Notifications();

            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");
            ViewBag.IntimationTime = Convert.ToString(serverDate);
            ViewBag.dacid = "0";
            ViewBag.sts = "";
            var frmdata = HttpContext.Request.Form["vls"];

            string id = frmdata;
            if (id != null)
            {
                ViewBag.WP_GC_HOURLY_DATA_HEADER_ID_PK = id.Split('½')[0];
                ViewBag.sts = id.Split('½')[0];
                ViewBag.dacid = id.Split('½')[2];
                ViewBag.IPP = "";
                var ViewDAD = id.Split('½')[4];
                if (ViewDAD != null || ViewDAD != "")
                {
                    ViewBag.Mode = id.Split('½')[4];
                }

            }
            else
            {
                ViewBag.WP_GC_HOURLY_DATA_HEADER_ID_PK = "0";
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    //ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                    ViewBag.IPP = Fn.Data2DropdownSQL(@"WITH temp_table AS (SELECT DISTINCT LOOKUP_CODE,MEANING,ORDER_IPPCAT FROM CDXP.FND_LOOKUP_VALUES 
      INNER JOIN CDXP.PPA_HEADER ON CDXP.PPA_HEADER.IPP_CATEGORY = CDXP.FND_LOOKUP_VALUES.LOOKUP_CODE 
      INNER JOIN CDXP.AP_SUPPLIERS ON  CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.PPA_HEADER.VENDOR_ID_FK
      INNER JOIN CDXP.AP_SUPPLIER_SITE_ALL ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID=CDXP.PPA_HEADER.VENDOR_SITE_ID_FK
      INNER JOIN
      CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
      CDXP.PPA_HEADER.VENDOR_SITE_ID_FK  = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID    

     WHERE CDXP.FND_LOOKUP_VALUES.LOOKUP_TYPE = 'CPPA_PPA_IPP_CATEGORY_LK' AND CDXP.FND_LOOKUP_VALUES.LOOKUP_CODE =20
                            AND (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')

       ) SELECT LOOKUP_CODE,MEANING FROM temp_table ORDER BY ORDER_IPPCAT");


                }
            }
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                var userName = Convert.ToString(Session["UserName"]);
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
            }
            return View();
        }

        [HttpPost]
        public ActionResult LoadCurtailmentList()
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
            var userType = Convert.ToInt32(Fn.ExenID("SELECT distinct WPU.WP_SETUP_USER_TYPES_ID FROM CDXP.WP_PORTAL_USERS wpu  WHERE wpu.USER_NAME = '" + userName + "'"));
            ViewBag.userType = userType;
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                ViewBag.name = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_NAME;
                ViewBag.designation = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_DESIGNATION;
                ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                if (userType == 10) //IPP
                {
                    //ViewBag.IPP = Fn.Data2DropdownSQL(@"SELECT 
                    //                                    CAST(AP_SUPPLIER_SITE_ALL_1.VENDOR_ID AS VARCHAR(100)) +'½'+ CAST           ( AP_SUPPLIER_SITE_ALL_1.VENDOR_SITE_ID  AS VARCHAR(100)),    
                    //                                    CASE WHEN T .CNT = 1 THEN CDXP.AP_SUPPLIERS.VENDOR_NAME ELSE CDXP.AP_SUPPLIERS.VENDOR_NAME +                        AP_SUPPLIER_SITE_ALL_1.VENDOR_SITE_CODE  END AS VAL 
                    //                                FROM            
                    //                                    (SELECT COUNT(VENDOR_SITE_ID) AS CNT, VENDOR_ID
                    //                                        FROM CDXP.AP_SUPPLIER_SITE_ALL
                    //                                  GROUP BY VENDOR_ID) AS T INNER JOIN
                    //                                 CDXP.AP_SUPPLIERS ON T.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                    //                                 CDXP.AP_SUPPLIER_SITE_ALL AS AP_SUPPLIER_SITE_ALL_1 ON CDXP.AP_SUPPLIERS.VENDOR_ID = AP_SUPPLIER_SITE_ALL_1.VENDOR_ID JOIN CDXP.PPA_HEADER ph ON ph.VENDOR_ID_FK = CDXP.AP_SUPPLIERS.VENDOR_ID
                    //                                 JOIN CDXP.WP_PORTAL_USERS wpu ON ph.HEADER_ID_PK = wpu.PPA_HEADER_ID_FK 
                    //                                 WHERE wpu.USER_NAME = '" + userName + "' ORDER BY VAL");

                    ViewBag.IPP = Fn.Data2DropdownSQL(@"SELECT A.VENDOR_ID,A.VENDOR_NAME FROM CDXP.AP_SUPPLIERS A JOIN CDXP.PPA_HEADER ph ON A.VENDOR_ID = ph.VENDOR_ID_FK JOIN CDXP.WP_PORTAL_USERS wpu ON ph.HEADER_ID_PK = wpu.PPA_HEADER_ID_FK WHERE wpu.USER_NAME = '" + userName + "'");
                    ViewBag.IPPType = "";
                }
                else //USER
                {
                    //ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                    ViewBag.IPPType = Fn.Data2DropdownSQL(@"SELECT FND_LOOKUP_VALUES_ID_PK AS VAL, MEANING FROM CDXP.FND_LOOKUP_VALUES  where (FND_LOOKUP_VALUES_ID_PK = 1168 OR FND_LOOKUP_VALUES_ID_PK = 1169) AND ENABLED_FLAG = 'Y'");

                    //ViewBag.IPP = Fn.Data2DropdownSQL(@"SELECT DISTINCT  CAST(AP_SUPPLIER_SITE_ALL_1.VENDOR_ID AS VARCHAR(100)) + '½' + CAST(AP_SUPPLIER_SITE_ALL_1.VENDOR_SITE_ID  AS VARCHAR(100)),    
                    //                                    CASE WHEN T.CNT = 1 THEN CDXP.AP_SUPPLIERS.VENDOR_NAME ELSE CDXP.AP_SUPPLIERS.VENDOR_NAME + AP_SUPPLIER_SITE_ALL_1.VENDOR_SITE_CODE  END AS VAL
                    //                                FROM
                    //                                    (SELECT COUNT(VENDOR_SITE_ID) AS CNT, VENDOR_ID
                    //                                        FROM CDXP.AP_SUPPLIER_SITE_ALL
                    //                                  GROUP BY VENDOR_ID) AS T INNER JOIN
                    //                                 CDXP.AP_SUPPLIERS ON T.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                    //                                 CDXP.AP_SUPPLIER_SITE_ALL AS AP_SUPPLIER_SITE_ALL_1 ON CDXP.AP_SUPPLIERS.VENDOR_ID = AP_SUPPLIER_SITE_ALL_1.VENDOR_ID JOIN CDXP.PPA_HEADER ph ON ph.VENDOR_ID_FK = CDXP.AP_SUPPLIERS.VENDOR_ID



                    //                                 JOIN CDXP.WP_PORTAL_USERS wpu ON ph.HEADER_ID_PK = wpu.PPA_HEADER_ID_FK



                    //                                 WHERE ph.IPP_CATEGORY IN('50','60') ORDER BY VAL");
                    ViewBag.IPP = "";
                }

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

        public ActionResult LoadCurtailmentForm()
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
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                ViewBag.name = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_NAME;
                ViewBag.designation = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_DESIGNATION;
            }

            if (id != null)
            {
                ViewBag.tg = id;
                var IdPk = id.Split('½');
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    ViewBag.IPPType = Fn.Data2DropdownSQL(@"SELECT FND_LOOKUP_VALUES_ID_PK AS VAL, MEANING FROM CDXP.FND_LOOKUP_VALUES  where (FND_LOOKUP_VALUES_ID_PK = (SELECT IPPTYPE FROM CDXP.WP_NPCC_LOAD_CURTAILMENT WHERE WP_NPCC_LOAD_CURTAILMENT_ID_PK =" + IdPk[0] + ")) AND ENABLED_FLAG = 'Y'");
                    //ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                }
            }
            else
            {
                ViewBag.tg = "0";
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    ViewBag.IPPType = Fn.Data2DropdownSQL(@"  SELECT FND_LOOKUP_VALUES_ID_PK AS VAL, MEANING FROM CDXP.FND_LOOKUP_VALUES  where (FND_LOOKUP_VALUES_ID_PK = 1168 OR FND_LOOKUP_VALUES_ID_PK = 1169) AND ENABLED_FLAG = 'Y'");
                    //ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                }
            }
            return View();
        }

        public ActionResult PlantEventsList()
        {
            sessionCheck(); Load_Notifications();
            var userName = Convert.ToString(Session["UserName"]);
            ViewBag.fromDate = "01-" + DateTime.Now.ToString("dd-MMM-yyyy").Split('-')[1] + "-" + DateTime.Now.ToString("dd-MMM-yyyy").Split('-')[2];
            ViewBag.toDate = DateTime.Now.ToString("dd-MMM-yyyy");
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
            }
            ViewBag.IPP = Fn.Data2DropdownSQL(@"WITH temp_table AS (SELECT DISTINCT LOOKUP_CODE,MEANING,ORDER_IPPCAT FROM CDXP.FND_LOOKUP_VALUES 
      INNER JOIN CDXP.PPA_HEADER ON CDXP.PPA_HEADER.IPP_CATEGORY = CDXP.FND_LOOKUP_VALUES.LOOKUP_CODE 
      INNER JOIN CDXP.AP_SUPPLIERS ON  CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.PPA_HEADER.VENDOR_ID_FK
      INNER JOIN CDXP.AP_SUPPLIER_SITE_ALL ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID=CDXP.PPA_HEADER.VENDOR_SITE_ID_FK
      INNER JOIN
      CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
      CDXP.PPA_HEADER.VENDOR_SITE_ID_FK  = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID    

      WHERE CDXP.FND_LOOKUP_VALUES.LOOKUP_TYPE = 'CPPA_PPA_IPP_CATEGORY_LK' 
      AND (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')

       ) SELECT LOOKUP_CODE,MEANING FROM temp_table ORDER BY ORDER_IPPCAT");


            //ViewBag.IPP = Fn.Data2DropdownSQL(@"SELECT DISTINCT CDXP.AP_SUPPLIERS.VENDOR_ID AS id, VENDOR_NAME FROM CDXP.AP_SUPPLIERS INNER JOIN CDXP.PPA_HEADER INNER JOIN CDXP.CPPA_PPA_PLT_BLK_FUEL 
            //ON PPA_HEADER.HEADER_ID_PK = CPPA_PPA_PLT_BLK_FUEL.HEADER_ID_FK ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.PPA_HEADER.VENDOR_ID_FK
            //INNER JOIN
            //CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
            //CDXP.PPA_HEADER.VENDOR_SITE_ID_FK  = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID    

            //WHERE  (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = CDXP.FN_WP_PORTAL_USERS_ID_BY_USER_NAME('" + Convert.ToString(Session["UserName"]) + @"')) ORDER BY VENDOR_NAME");

            return View();
        }

        public ActionResult PlantEventsForm()
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
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                ViewBag.name = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_NAME;
                ViewBag.designation = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault().DISPLAY_DESIGNATION;
            }
            if (id != null)
            {
                ViewBag.tg = id;
                ViewBag.IPP = Fn.Data2DropdownSQL(@"WITH temp_table AS (SELECT DISTINCT LOOKUP_CODE,MEANING,ORDER_IPPCAT FROM CDXP.FND_LOOKUP_VALUES 
      INNER JOIN CDXP.PPA_HEADER ON CDXP.PPA_HEADER.IPP_CATEGORY = CDXP.FND_LOOKUP_VALUES.LOOKUP_CODE 
      INNER JOIN CDXP.AP_SUPPLIERS ON  CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.PPA_HEADER.VENDOR_ID_FK
      INNER JOIN CDXP.AP_SUPPLIER_SITE_ALL ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID=CDXP.PPA_HEADER.VENDOR_SITE_ID_FK
      INNER JOIN
      CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
      CDXP.PPA_HEADER.VENDOR_SITE_ID_FK  = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID    

      WHERE CDXP.FND_LOOKUP_VALUES.LOOKUP_TYPE = 'CPPA_PPA_IPP_CATEGORY_LK' 
      AND (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')
    )SELECT LOOKUP_CODE,MEANING FROM temp_table ORDER BY ORDER_IPPCAT");



            }
            else
            {
                ViewBag.tg = "0";
                ViewBag.IPP = Fn.Data2DropdownSQL(@"WITH temp_table AS (SELECT DISTINCT LOOKUP_CODE,MEANING,ORDER_IPPCAT FROM CDXP.FND_LOOKUP_VALUES 
      INNER JOIN CDXP.PPA_HEADER ON CDXP.PPA_HEADER.IPP_CATEGORY = CDXP.FND_LOOKUP_VALUES.LOOKUP_CODE 
      INNER JOIN CDXP.AP_SUPPLIERS ON  CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.PPA_HEADER.VENDOR_ID_FK
      INNER JOIN CDXP.AP_SUPPLIER_SITE_ALL ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID=CDXP.PPA_HEADER.VENDOR_SITE_ID_FK
      INNER JOIN
      CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
      CDXP.PPA_HEADER.VENDOR_SITE_ID_FK  = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID    

      WHERE CDXP.FND_LOOKUP_VALUES.LOOKUP_TYPE = 'CPPA_PPA_IPP_CATEGORY_LK' 
                            AND (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')

       ) SELECT LOOKUP_CODE,MEANING FROM temp_table ORDER BY ORDER_IPPCAT");

            }

            return View();
        }

        public ActionResult VarficationOfInvoiceList()
        {
            sessionCheck(); Load_Notifications();
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                //ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                var userName = Convert.ToString(Session["UserName"]);
                ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME)
                    .Join(db.WP_GC_USER_ACCESS,
                        s => s.VENDOR_ID,
                        a => a.ENTITY_ID,
                        (sup, acc) => new { sup.VENDOR_ID, sup.VENDOR_NAME, acc.WP_PORTAL_USERS_ID })
                    .Join(db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName),
                        x => x.WP_PORTAL_USERS_ID,
                        y => y.WP_PORTAL_USERS_ID,
                        (vend, user) => new { id = vend.VENDOR_ID, vend.VENDOR_NAME }).ToList());
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
            }
            return View();
        }
        [HttpPost]
        public ActionResult VarficationOfInvoiceData()
        {
            var frmdata = HttpContext.Request.Form["vls"];
            string id = frmdata;

            sessionCheck(); Load_Notifications();
            return View();
        }

        [HttpPost]
        public ActionResult HourlyData()
        {
            var frmdata = HttpContext.Request.Form["vls"];
            string id = frmdata;
            ViewBag.diarynumber = id.Split('½')[1];
            sessionCheck(); Load_Notifications();
            return View();
        }
        public ActionResult PlantTechnicalLimit()
        {
            sessionCheck(); Load_Notifications();
            return View();
        }

        public ActionResult ForcedOutagesReports()
        {
            sessionCheck(); Load_Notifications();
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");
            ViewBag.currentDate = serverDate;

            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
            }

            return View();
        }

        public ActionResult ScheduledOutagesReport()
        {
            sessionCheck(); Load_Notifications();
            return View();
        }
        public ActionResult DeclarationOfAvailCapacityList()
        {
            sessionCheck(); Load_Notifications();
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                //ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());

                var userName = Convert.ToString(Session["UserName"]);
                var userType = Convert.ToInt32(Fn.ExenID("SELECT distinct WPU.WP_SETUP_USER_TYPES_ID FROM CDXP.WP_PORTAL_USERS wpu  WHERE wpu.USER_NAME = '" + userName + "'"));
                if (userType == 10)
                {
                    ViewBag.IPPType = Convert.ToInt32(Fn.ExenID("SELECT distinct ph.IPP_CATEGORY FROM CDXP.AP_SUPPLIERS [as] JOIN CDXP.PPA_HEADER ph ON [as].VENDOR_ID = ph.VENDOR_ID_FK JOIN CDXP.WP_PORTAL_USERS wpu ON ph.HEADER_ID_PK = wpu.PPA_HEADER_ID_FK WHERE wpu.USER_NAME = '" + userName + "'AND APPROVAL_STATUS = 'Approved'"));
                    ViewBag.IPP = Fn.Data2DropdownSQL(@"SELECT 
                                                        CAST(AP_SUPPLIER_SITE_ALL_1.VENDOR_ID AS VARCHAR(100)) +'½'+ CAST (AP_SUPPLIER_SITE_ALL_1.VENDOR_SITE_ID  AS VARCHAR(100)),    
                                                        CASE WHEN T .CNT = 1 THEN CDXP.AP_SUPPLIERS.VENDOR_NAME ELSE CDXP.AP_SUPPLIERS.VENDOR_NAME +                        AP_SUPPLIER_SITE_ALL_1.VENDOR_SITE_CODE  END AS VAL 
                                                    FROM            
                                                        (SELECT COUNT(VENDOR_SITE_ID) AS CNT, VENDOR_ID
                                                            FROM CDXP.AP_SUPPLIER_SITE_ALL
                                                      GROUP BY VENDOR_ID) AS T INNER JOIN
                                                     CDXP.AP_SUPPLIERS ON T.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                                                     CDXP.AP_SUPPLIER_SITE_ALL AS AP_SUPPLIER_SITE_ALL_1 ON CDXP.AP_SUPPLIERS.VENDOR_ID = AP_SUPPLIER_SITE_ALL_1.VENDOR_ID JOIN CDXP.PPA_HEADER ph ON ph.VENDOR_ID_FK = CDXP.AP_SUPPLIERS.VENDOR_ID
						                             JOIN CDXP.WP_PORTAL_USERS wpu ON ph.HEADER_ID_PK = wpu.PPA_HEADER_ID_FK 
						                             WHERE wpu.USER_NAME = '" + userName + "' ORDER BY VAL");
                }
                else
                {
                    ViewBag.IPPType = 0;
                    ViewBag.IPP = Fn.Data2DropdownSQL(@"SELECT     CAST(AP_SUPPLIER_SITE_ALL_1.VENDOR_ID AS VARCHAR(100)) +'½'+ CAST( AP_SUPPLIER_SITE_ALL_1.VENDOR_SITE_ID  AS VARCHAR(100)),    CASE WHEN T .CNT = 1 THEN CDXP.AP_SUPPLIERS.VENDOR_NAME ELSE CDXP.AP_SUPPLIERS.VENDOR_NAME + ' (' + AP_SUPPLIER_SITE_ALL_1.VENDOR_SITE_CODE + ')' END AS VAL 
 FROM            (SELECT        COUNT(VENDOR_SITE_ID) AS CNT, VENDOR_ID
                          FROM            CDXP.AP_SUPPLIER_SITE_ALL
                          GROUP BY VENDOR_ID) AS T INNER JOIN
                         CDXP.AP_SUPPLIERS ON T.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                         CDXP.AP_SUPPLIER_SITE_ALL AS AP_SUPPLIER_SITE_ALL_1 ON CDXP.AP_SUPPLIERS.VENDOR_ID = AP_SUPPLIER_SITE_ALL_1.VENDOR_ID
						 ORDER BY VAL");
                }

                //ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME)
                //    .Join(db.WP_GC_USER_ACCESS,
                //    s => s.VENDOR_ID,
                //    a => a.ENTITY_ID,
                //    (sup, acc) => new { sup.VENDOR_ID, sup.VENDOR_NAME, acc.WP_PORTAL_USERS_ID })
                //    .Join(db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName),
                //    x => x.WP_PORTAL_USERS_ID,
                //    y => y.WP_PORTAL_USERS_ID,
                //    (vend, user) => new { id = vend.VENDOR_ID, vend.VENDOR_NAME }).ToList());
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
            }
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");
            ViewBag.PkDateTime = serverDate;
            return View();

        }
        public ActionResult ViewAndEditDespatch()
        {
            sessionCheck(); Load_Notifications();
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                //ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                ViewBag.IPP = Fn.Data2DropdownSQL(@"SELECT     CAST(AP_SUPPLIER_SITE_ALL_1.VENDOR_ID AS VARCHAR(100)) +'½'+ CAST( AP_SUPPLIER_SITE_ALL_1.VENDOR_SITE_ID  AS VARCHAR(100)),    CASE WHEN T .CNT = 1 THEN CDXP.AP_SUPPLIERS.VENDOR_NAME ELSE CDXP.AP_SUPPLIERS.VENDOR_NAME + ' (' + AP_SUPPLIER_SITE_ALL_1.VENDOR_SITE_CODE + ')' END AS VAL 
 FROM            (SELECT        COUNT(VENDOR_SITE_ID) AS CNT, VENDOR_ID
                          FROM            CDXP.AP_SUPPLIER_SITE_ALL
                          GROUP BY VENDOR_ID) AS T INNER JOIN
                         CDXP.AP_SUPPLIERS ON T.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                         CDXP.AP_SUPPLIER_SITE_ALL AS AP_SUPPLIER_SITE_ALL_1 ON CDXP.AP_SUPPLIERS.VENDOR_ID = AP_SUPPLIER_SITE_ALL_1.VENDOR_ID
						 ORDER BY VAL");
                var userName = Convert.ToString(Session["UserName"]);
                //ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME)
                //    .Join(db.WP_GC_USER_ACCESS,
                //    s => s.VENDOR_ID,
                //    a => a.ENTITY_ID,
                //    (sup, acc) => new { sup.VENDOR_ID, sup.VENDOR_NAME, acc.WP_PORTAL_USERS_ID })
                //    .Join(db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName),
                //    x => x.WP_PORTAL_USERS_ID,
                //    y => y.WP_PORTAL_USERS_ID,
                //    (vend, user) => new { id = vend.VENDOR_ID, vend.VENDOR_NAME }).ToList());
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
            }
            return View();

        }


        public ActionResult PreviousDayDispatch()
        {
            sessionCheck(); Load_Notifications();
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            ViewBag.DateTime = pkdatetime.ToString("dd-MMM-yyyy HH:mm");// + " "+ pkdatetime.ToShortTimeString();
            ViewBag.NextEDate = pkdatetime.AddDays(1).ToString("dd-MMM-yyyy");
            var frmdata = HttpContext.Request.Form["vls"];
            string id = frmdata;
            ViewBag.sts = "";
            ViewBag.conn = "";

            if (id != null)
            {
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    var userName = Convert.ToString(Session["UserName"]);
                    ViewBag.WP_GC_HOURLY_DATA_HEADER_ID_PK = id.Split('½')[0];
                    ViewBag.sts = id.Split('½')[1];
                    ViewBag.IPP = "";
                    ViewBag.conn = id.Split('½')[id.Split('½').Length - 1];
                    if (ViewBag.conn == "N")
                    {
                        ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME)
                        .Join(db.WP_GC_USER_ACCESS,
                        s => s.VENDOR_ID,
                        a => a.ENTITY_ID,
                        (sup, acc) => new { sup.VENDOR_ID, sup.VENDOR_NAME, acc.WP_PORTAL_USERS_ID })
                        .Join(db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName),
                        x => x.WP_PORTAL_USERS_ID,
                        y => y.WP_PORTAL_USERS_ID,
                        (vend, user) => new { id = vend.VENDOR_ID, vend.VENDOR_NAME }).ToList());
                    }
                    ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                }
            }
            else
            {
                ViewBag.WP_GC_HOURLY_DATA_HEADER_ID_PK = "0";
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    //ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                    var userName = Convert.ToString(Session["UserName"]);
                    ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME)
                        .Join(db.WP_GC_USER_ACCESS,
                        s => s.VENDOR_ID,
                        a => a.ENTITY_ID,
                        (sup, acc) => new { sup.VENDOR_ID, sup.VENDOR_NAME, acc.WP_PORTAL_USERS_ID })
                        .Join(db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName),
                        x => x.WP_PORTAL_USERS_ID,
                        y => y.WP_PORTAL_USERS_ID,
                        (vend, user) => new { id = vend.VENDOR_ID, vend.VENDOR_NAME }).ToList());
                    ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                }
            }
            return View();

        }


        public ActionResult DeclarationOfAvailCapacityListForNPCC()
        {
            sessionCheck(); Load_Notifications();
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                //ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                var userName = Convert.ToString(Session["UserName"]);
                ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME)
                    .Join(db.WP_GC_USER_ACCESS,
                    s => s.VENDOR_ID,
                    a => a.ENTITY_ID,
                    (sup, acc) => new { sup.VENDOR_ID, sup.VENDOR_NAME, acc.WP_PORTAL_USERS_ID })
                    .Join(db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName),
                    x => x.WP_PORTAL_USERS_ID,
                    y => y.WP_PORTAL_USERS_ID,
                    (vend, user) => new { id = vend.VENDOR_ID, vend.VENDOR_NAME }).ToList());
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
            }
            return View();
        }


        [HttpPost]
        public ActionResult RDAC()
        {
            sessionCheck();
            Load_Notifications();
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");

            var frmdata = HttpContext.Request.Form["vls"];
            string id = frmdata;
            ViewBag.sts = "";
            ViewBag.conn = "";
            if (id != null)
            {

                var userName = Convert.ToString(Session["UserName"]);
                ViewBag.WP_GC_HOURLY_DATA_HEADER_ID_PK = id.Split('½')[0];
                ViewBag.sts = id.Split('½')[1];
                ViewBag.IPP = "";
                ViewBag.conn = id.Split('½')[id.Split('½').Length - 1];
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    if (ViewBag.conn == "V")
                    {
                        ViewBag.WP_GC_HOURLY_DATA_HEADER_ID_PK = id.Split('½')[0]; ;
                        ViewBag.dacid = 0;
                    }
                    else
                    {
                        ViewBag.sts = "";
                        ViewBag.WP_GC_HOURLY_DATA_HEADER_ID_PK = 0;
                        ViewBag.dacid = id.Split('½')[0];
                    }

                    var userType = Convert.ToInt32(Fn.ExenID("SELECT distinct WPU.WP_SETUP_USER_TYPES_ID FROM CDXP.WP_PORTAL_USERS wpu  WHERE wpu.USER_NAME = '" + userName + "'"));

                    var CreatedBy = Convert.ToInt32(Fn.ExenID("SELECT wghdh.CREATED_BY FROM CDXP.WP_GC_HOURLY_DATA_HEADER wghdh WHERE wghdh.WP_GC_HOURLY_DATA_HEADER_ID_PK  = " + id.Split('½')[0]));

                    if (userType == 10)
                    {
                        ViewBag.IPPType = Convert.ToInt32(Fn.ExenID("SELECT distinct ph.IPP_CATEGORY FROM CDXP.AP_SUPPLIERS [as] JOIN CDXP.PPA_HEADER ph ON [as].VENDOR_ID = ph.VENDOR_ID_FK JOIN CDXP.WP_PORTAL_USERS wpu ON ph.HEADER_ID_PK = wpu.PPA_HEADER_ID_FK WHERE wpu.USER_NAME = '" + userName + "'AND APPROVAL_STATUS = 'Approved'"));
                    }
                    else if (userType == 7 && ViewBag.conn == "V")
                    {
                        string IPPType = Fn.ExenID("SELECT B.IPP_CATEGORY FROM CDXP.WP_GC_HOURLY_DATA_HEADER A JOIN CDXP.PPA_HEADER B  ON A.VENDOR_SITE_ID_FK = B.VENDOR_SITE_ID_FK WHERE  APPROVAL_STATUS = 'Approved' AND A.WP_GC_HOURLY_DATA_HEADER_ID_PK = " + id.Split('½')[0]);
                        ViewBag.IPPType = Convert.ToInt32(IPPType);
                    }
                    else
                    {
                        ViewBag.IPPType = 0;
                    }
                    ViewBag.IPP = "";
                    ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                }
            }
            else
            {
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    ViewBag.WP_GC_HOURLY_DATA_HEADER_ID_PK = "0";
                    ViewBag.dacid = "0";
                    var userName = Convert.ToString(Session["UserName"]);
                    var userType = Convert.ToInt32(Fn.ExenID("SELECT distinct WPU.WP_SETUP_USER_TYPES_ID FROM CDXP.WP_PORTAL_USERS wpu  WHERE wpu.USER_NAME = '" + userName + "'"));
                    if (userType == 10)
                    {
                        ViewBag.IPPType = Convert.ToInt32(Fn.ExenID("SELECT distinct ph.IPP_CATEGORY FROM CDXP.AP_SUPPLIERS [as] JOIN CDXP.PPA_HEADER ph ON [as].VENDOR_ID = ph.VENDOR_ID_FK JOIN CDXP.WP_PORTAL_USERS wpu ON ph.HEADER_ID_PK = wpu.PPA_HEADER_ID_FK WHERE wpu.USER_NAME = '" + userName + "'AND APPROVAL_STATUS = 'Approved'"));
                    }
                    else
                    {
                        ViewBag.IPPType = 0;
                    }
                    ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME)
                        .Join(db.WP_GC_USER_ACCESS,
                        s => s.VENDOR_ID,
                        a => a.ENTITY_ID,
                        (sup, acc) => new { sup.VENDOR_ID, sup.VENDOR_NAME, acc.WP_PORTAL_USERS_ID })
                        .Join(db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName),
                        x => x.WP_PORTAL_USERS_ID,
                        y => y.WP_PORTAL_USERS_ID,
                        (vend, user) => new { id = vend.VENDOR_ID, vend.VENDOR_NAME }).ToList());
                    ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                }
            }
            ViewBag.IntimationTime = Convert.ToString(serverDate);
            ViewBag.pkdatetime = pkdatetime;
            return View();
        }



        [HttpPost]
        public ActionResult RDACForNPCC()
        {
            sessionCheck();
            Load_Notifications();
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");



            var frmdata = HttpContext.Request.Form["vls"];
            string id = frmdata;
            ViewBag.sts = "";
            ViewBag.conn = "";
            if (id != null)
            {



                var userName = Convert.ToString(Session["UserName"]);
                ViewBag.WP_GC_HOURLY_DATA_HEADER_ID_PK = id.Split('½')[0];
                ViewBag.sts = id.Split('½')[1];
                ViewBag.IPP = "";
                ViewBag.conn = id.Split('½')[id.Split('½').Length - 1];
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    if (ViewBag.conn == "V")
                    {
                        ViewBag.WP_GC_HOURLY_DATA_HEADER_ID_PK = id.Split('½')[0]; ;
                        ViewBag.dacid = 0;
                    }
                    else
                    {
                        ViewBag.sts = "";
                        ViewBag.WP_GC_HOURLY_DATA_HEADER_ID_PK = 0;
                        ViewBag.dacid = id.Split('½')[0];
                    }



                    var userType = Convert.ToInt32(Fn.ExenID("SELECT distinct WPU.WP_SETUP_USER_TYPES_ID FROM CDXP.WP_PORTAL_USERS wpu  WHERE wpu.USER_NAME = '" + userName + "'"));
                    if (userType == 10)
                    {
                        ViewBag.IPPType = Convert.ToInt32(Fn.ExenID("SELECT distinct ph.IPP_CATEGORY FROM CDXP.AP_SUPPLIERS [as] JOIN CDXP.PPA_HEADER ph ON [as].VENDOR_ID = ph.VENDOR_ID_FK JOIN CDXP.WP_PORTAL_USERS wpu ON ph.HEADER_ID_PK = wpu.PPA_HEADER_ID_FK WHERE wpu.USER_NAME = '" + userName + "'AND APPROVAL_STATUS = 'Approved'"));
                    }
                    else if (userType == 7)
                    {
                        String IPPType = Fn.ExenID("SELECT B.IPP_CATEGORY FROM CDXP.WP_GC_HOURLY_DATA_HEADER A JOIN CDXP.PPA_HEADER B  ON A.VENDOR_SITE_ID_FK = B.VENDOR_SITE_ID_FK WHERE  APPROVAL_STATUS = 'Approved' AND A.WP_GC_HOURLY_DATA_HEADER_ID_PK = " + id.Split('½')[0]);
                        ViewBag.IPPType = Convert.ToInt32(IPPType);
                    }
                    else
                    {
                        ViewBag.IPPType = 0;
                    }
                    ViewBag.IPP = "";
                    ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                }
            }
            else
            {
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    ViewBag.WP_GC_HOURLY_DATA_HEADER_ID_PK = "0";
                    ViewBag.dacid = "0";
                    var userName = Convert.ToString(Session["UserName"]);
                    var userType = Convert.ToInt32(Fn.ExenID("SELECT distinct WPU.WP_SETUP_USER_TYPES_ID FROM CDXP.WP_PORTAL_USERS wpu  WHERE wpu.USER_NAME = '" + userName + "'"));
                    if (userType == 10)
                    {
                        ViewBag.IPPType = Convert.ToInt32(Fn.ExenID("SELECT distinct ph.IPP_CATEGORY FROM CDXP.AP_SUPPLIERS [as] JOIN CDXP.PPA_HEADER ph ON [as].VENDOR_ID = ph.VENDOR_ID_FK JOIN CDXP.WP_PORTAL_USERS wpu ON ph.HEADER_ID_PK = wpu.PPA_HEADER_ID_FK WHERE wpu.USER_NAME = '" + userName + "'AND APPROVAL_STATUS = 'Approved'"));
                    }
                    else
                    {
                        ViewBag.IPPType = 0;
                    }
                    ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME)
                        .Join(db.WP_GC_USER_ACCESS,
                        s => s.VENDOR_ID,
                        a => a.ENTITY_ID,
                        (sup, acc) => new { sup.VENDOR_ID, sup.VENDOR_NAME, acc.WP_PORTAL_USERS_ID })
                        .Join(db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName),
                        x => x.WP_PORTAL_USERS_ID,
                        y => y.WP_PORTAL_USERS_ID,
                        (vend, user) => new { id = vend.VENDOR_ID, vend.VENDOR_NAME }).ToList());
                    ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                }
            }
            ViewBag.IntimationTime = Convert.ToString(serverDate);
            ViewBag.pkdatetime = pkdatetime;
            return View();
        }

        public ActionResult ViewDespatchDetail()
        {
            sessionCheck(); Load_Notifications();
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");

            var frmdata = HttpContext.Request.Form["vls"];
            string id = frmdata;
            ViewBag.sts = "";
            ViewBag.conn = "";
            if (id != null)
            {

                var userName = Convert.ToString(Session["UserName"]);
                ViewBag.DISPACTH_DETAIL_ID = id.Split('½')[0];
                ViewBag.sts = id.Split('½')[1];
                ViewBag.IPP = "";
                ViewBag.conn = id.Split('½')[id.Split('½').Length - 1];

                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    if (ViewBag.conn == "V")
                    {
                        ViewBag.DISPACTH_DETAIL_ID = id.Split('½')[0]; ;
                        ViewBag.dacid = 0;
                    }
                    else
                    {
                        ViewBag.sts = "";
                        ViewBag.DISPACTH_DETAIL_ID = 0;
                        ViewBag.dacid = id.Split('½')[0];
                    }

                    ViewBag.IPP = "";
                    ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                }
            }
            else
            {
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    ViewBag.DISPACTH_DETAIL_ID = "0";
                    ViewBag.dacid = "0";
                    var userName = Convert.ToString(Session["UserName"]);
                    ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME)
                        .Join(db.WP_GC_USER_ACCESS,
                        s => s.VENDOR_ID,
                        a => a.ENTITY_ID,
                        (sup, acc) => new { sup.VENDOR_ID, sup.VENDOR_NAME, acc.WP_PORTAL_USERS_ID })
                        .Join(db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName),
                        x => x.WP_PORTAL_USERS_ID,
                        y => y.WP_PORTAL_USERS_ID,
                        (vend, user) => new { id = vend.VENDOR_ID, vend.VENDOR_NAME }).ToList());
                    ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                }
            }
            ViewBag.IntimationTime = Convert.ToString(serverDate);
            return View();
        }


        [HttpPost]
        public ActionResult ResourseDeclarationOfAvailCapacityForNPCC()
        {
            sessionCheck(); Load_Notifications();
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            ViewBag.DateTime = pkdatetime.ToString("dd-MMM-yyyy HH:mm");// + " "+ pkdatetime.ToShortTimeString();
            ViewBag.NextEDate = pkdatetime.AddDays(1).ToString("dd-MMM-yyyy");
            var frmdata = HttpContext.Request.Form["vls"];
            string id = frmdata;
            ViewBag.sts = "";
            if (id != null)
            {
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    var userName = Convert.ToString(Session["UserName"]);
                    ViewBag.WP_GC_HOURLY_DATA_HEADER_ID_PK = id.Split('½')[0];
                    ViewBag.sts = id.Split('½')[1];
                    ViewBag.IPP = Fn.Data2DropdownSQL(@"SELECT     CAST(AP_SUPPLIER_SITE_ALL_1.VENDOR_ID AS VARCHAR(100)) +'½'+ CAST( AP_SUPPLIER_SITE_ALL_1.VENDOR_SITE_ID  AS VARCHAR(100)),    CASE WHEN T .CNT = 1 THEN CDXP.AP_SUPPLIERS.VENDOR_NAME ELSE CDXP.AP_SUPPLIERS.VENDOR_NAME + ' (' + AP_SUPPLIER_SITE_ALL_1.VENDOR_SITE_CODE + ')' END AS VAL 
 FROM            (SELECT        COUNT(VENDOR_SITE_ID) AS CNT, VENDOR_ID
                          FROM            CDXP.AP_SUPPLIER_SITE_ALL
                          GROUP BY VENDOR_ID) AS T INNER JOIN
                         CDXP.AP_SUPPLIERS ON T.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                         CDXP.AP_SUPPLIER_SITE_ALL AS AP_SUPPLIER_SITE_ALL_1 ON CDXP.AP_SUPPLIERS.VENDOR_ID = AP_SUPPLIER_SITE_ALL_1.VENDOR_ID
						 ORDER BY VAL");
                    ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                }
            }
            else
            {
                ViewBag.WP_GC_HOURLY_DATA_HEADER_ID_PK = "0";
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    //ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                    var userName = Convert.ToString(Session["UserName"]);
                    ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME)
                        .Join(db.WP_GC_USER_ACCESS,
                        s => s.VENDOR_ID,
                        a => a.ENTITY_ID,
                        (sup, acc) => new { sup.VENDOR_ID, sup.VENDOR_NAME, acc.WP_PORTAL_USERS_ID })
                        .Join(db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName),
                        x => x.WP_PORTAL_USERS_ID,
                        y => y.WP_PORTAL_USERS_ID,
                        (vend, user) => new { id = vend.VENDOR_ID, vend.VENDOR_NAME }).ToList());
                    ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                }
            }
            return View();
        }


        [HttpPost]
        public ActionResult ResourseDeclarationOfAvailCapacity()
        {
            sessionCheck(); Load_Notifications();
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            ViewBag.DateTime = pkdatetime.ToString("dd-MMM-yyyy HH:mm");// + " "+ pkdatetime.ToShortTimeString();
            ViewBag.NextEDate = pkdatetime.AddDays(1).ToString("dd-MMM-yyyy");
            var frmdata = HttpContext.Request.Form["vls"];
            string id = frmdata;
            ViewBag.sts = "";
            ViewBag.conn = "";

            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");
            ViewBag.PkDateTime = serverDate;
            if (id != null)
            {
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    var userName = Convert.ToString(Session["UserName"]);
                    var userType = Convert.ToInt32(Fn.ExenID("SELECT distinct WPU.WP_SETUP_USER_TYPES_ID FROM CDXP.WP_PORTAL_USERS wpu  WHERE wpu.USER_NAME = '" + userName + "'"));
                    if (userType == 10)
                    {
                        ViewBag.IPPType = Convert.ToInt32(Fn.ExenID("SELECT distinct ph.IPP_CATEGORY FROM CDXP.AP_SUPPLIERS [as] JOIN CDXP.PPA_HEADER ph ON [as].VENDOR_ID = ph.VENDOR_ID_FK JOIN CDXP.WP_PORTAL_USERS wpu ON ph.HEADER_ID_PK = wpu.PPA_HEADER_ID_FK WHERE wpu.USER_NAME = '" + userName + "'AND APPROVAL_STATUS = 'Approved'"));

                        ViewBag.DACHour = Convert.ToInt32(Fn.ExenID("SELECT distinct ph.DAC_Before_Sub_Hour FROM CDXP.AP_SUPPLIERS [as] JOIN CDXP.PPA_HEADER ph ON [as].VENDOR_ID = ph.VENDOR_ID_FK JOIN CDXP.WP_PORTAL_USERS wpu ON ph.HEADER_ID_PK = wpu.PPA_HEADER_ID_FK WHERE wpu.USER_NAME = '" + userName + "'AND APPROVAL_STATUS = 'Approved'"));
                    }
                    else
                    {
                        //ViewBag.IPPType = 0;
                        string DACID = id.Split('½')[0];
                        ViewBag.IPPType = Convert.ToInt32(Fn.ExenID("SELECT B.IPP_CATEGORY FROM CDXP.WP_GC_HOURLY_DATA_HEADER A JOIN CDXP.PPA_HEADER B  ON A.VENDOR_SITE_ID_FK = B.VENDOR_SITE_ID_FK WHERE  APPROVAL_STATUS = 'Approved' AND A.WP_GC_HOURLY_DATA_HEADER_ID_PK = " + DACID));
                        ViewBag.DACHour = 0;
                    }


                    ViewBag.WP_GC_HOURLY_DATA_HEADER_ID_PK = id.Split('½')[0];
                    ViewBag.sts = id.Split('½')[1];
                    ViewBag.IPP = "";
                    ViewBag.conn = id.Split('½')[id.Split('½').Length - 1];
                    if (ViewBag.conn == "N")
                    {
                        ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME)
                        .Join(db.WP_GC_USER_ACCESS,
                        s => s.VENDOR_ID,
                        a => a.ENTITY_ID,
                        (sup, acc) => new { sup.VENDOR_ID, sup.VENDOR_NAME, acc.WP_PORTAL_USERS_ID }).Distinct()
                        .Join(db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).Distinct(),
                        x => x.WP_PORTAL_USERS_ID,
                        y => y.WP_PORTAL_USERS_ID,
                        (vend, user) => new { id = vend.VENDOR_ID, vend.VENDOR_NAME }).ToList());
                        // ViewBag.IPP2 = Fn.Data2Dropdown(from dh in db.WP_NPCC_DESPATCH_HEADER
                        //  where dh.VENDOR_SITE_ID == SETUP_SITE_ID
                        // select new);
                    }
                    ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                }
            }
            else
            {
                ViewBag.WP_GC_HOURLY_DATA_HEADER_ID_PK = "0";
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    //ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                    var userName = Convert.ToString(Session["UserName"]);
                    var userType = Convert.ToInt32(Fn.ExenID("SELECT distinct WPU.WP_SETUP_USER_TYPES_ID FROM CDXP.WP_PORTAL_USERS wpu  WHERE wpu.USER_NAME = '" + userName + "'"));
                    if (userType == 10)
                    {
                        ViewBag.IPPType = Convert.ToInt32(Fn.ExenID("SELECT distinct ph.IPP_CATEGORY FROM CDXP.AP_SUPPLIERS [as] JOIN CDXP.PPA_HEADER ph ON [as].VENDOR_ID = ph.VENDOR_ID_FK JOIN CDXP.WP_PORTAL_USERS wpu ON ph.HEADER_ID_PK = wpu.PPA_HEADER_ID_FK WHERE wpu.USER_NAME = '" + userName + "'AND APPROVAL_STATUS = 'Approved'"));
                    }
                    else
                    {
                        ViewBag.IPPType = 0;
                    }
                    ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME)
                        .Join(db.WP_GC_USER_ACCESS,
                        s => s.VENDOR_ID,
                        a => a.ENTITY_ID,
                        (sup, acc) => new { sup.VENDOR_ID, sup.VENDOR_NAME, acc.WP_PORTAL_USERS_ID })
                        .Join(db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName),
                        x => x.WP_PORTAL_USERS_ID,
                        y => y.WP_PORTAL_USERS_ID,
                        (vend, user) => new { id = vend.VENDOR_ID, vend.VENDOR_NAME }).ToList());
                    ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
                }
            }
            return View();
        }

        [HttpPost]
        public string AjaxCall()
        {
            sessionCheck(); Load_Notifications();
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");
            var frmdata = HttpContext.Request.Form["vls"];
            //var frmdata = "21½0½Draft½1001½1001½219½Wind½30½2006½Wind½20-Mar-2020½0½34½5½6½6½7½8½8½9½7½65½54½87½87½89½78½87½87½87½67½56½4½68½78½Draft½IPP USER½(IPP GENERAL USER)½½";
            string[] dataID = new string[500];
            dataID = Fn.CleanSQL(HttpUtility.UrlDecode(frmdata)).Split('½');

            string Returnvls = "";
            try
            {
                switch (Convert.ToInt32(dataID[0]))
                {

                    case 901:

                        string dateFrom = dataID[5].ToString();
                        string dateTo = dataID[6].ToString();

                        if (dateFrom == "" && dateTo == "")
                        {
                            Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT CDXP.WP_NPCC_DESPATCH_HEADER.WP_NPCC_DESPATCH_HEADER_ID,''[Sr#],FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') [Date], FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') [Notification Time], CDXP.WP_NPCC_DESPATCH_HEADER.DEMAND_TYPE as [Demand Type], 
CDXP.WP_NPCC_DESPATCH_HEADER.EMERGENCY_TYPE as [Type of Ramp Up / Down], CDXP.WP_NPCC_DESPATCH_HEADER.DEMAND_TYPE as [Startup Type], 
[CDXP].[WP_NPCC_DESPATCH_DETAIL].SYNC_DESYNC_TIME as [Sync / Desync Time], 
TARGET_DATE_TIME as [Target Time], 
FORMAT([GC_COMP_ACHIEVE_DATE_TIME],'dd-MMM-yyyy HH:mm') as [Achievement Time], 
 CDXP.WP_NPCC_DESPATCH_HEADER.TARGET_DEMAND as [Target Demand(MW)]

FROM CDXP.WP_NPCC_DESPATCH_HEADER
INNER JOIN[CDXP].[AP_SUPPLIER_SITE_ALL] on CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_SITE_ID = [CDXP].[AP_SUPPLIER_SITE_ALL].[VENDOR_SITE_ID]
INNER JOIN[CDXP].[AP_SUPPLIERS] on[CDXP].[AP_SUPPLIER_SITE_ALL].vendor_id = [CDXP].[AP_SUPPLIERS].VENDOR_ID
LEFT OUTER JOIN[CDXP].[WP_NPCC_DESPATCH_DETAIL] ON CDXP.WP_NPCC_DESPATCH_HEADER.WP_NPCC_DESPATCH_HEADER_ID = [CDXP].[WP_NPCC_DESPATCH_DETAIL].WP_NPCC_DESPATCH_HEADER_ID WHERE(CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_ID = '" + dataID[1] + @"') AND (CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_SITE_ID = '" + dataID[2] + @"') AND (CDXP.WP_NPCC_DESPATCH_HEADER.STATUS = '" + dataID[3] + @"') AND (CDXP.WP_NPCC_DESPATCH_HEADER.DEMAND_TYPE like '%" + dataID[4] + @"%')", "tblJ1");

                        }
                        else
                        {
                            Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT CDXP.WP_NPCC_DESPATCH_HEADER.WP_NPCC_DESPATCH_HEADER_ID,''[Sr#],FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') [Date], FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') [Notification Time], CDXP.WP_NPCC_DESPATCH_HEADER.DEMAND_TYPE as [Demand Type], 
CDXP.WP_NPCC_DESPATCH_HEADER.EMERGENCY_TYPE as [Type of Ramp Up / Down], CDXP.WP_NPCC_DESPATCH_HEADER.DEMAND_TYPE as [Startup Type], 
[CDXP].[WP_NPCC_DESPATCH_DETAIL].SYNC_DESYNC_TIME as [Sync / Desync Time], 
TARGET_DATE_TIME as [Target Time], 
FORMAT([GC_COMP_ACHIEVE_DATE_TIME],'dd-MMM-yyyy HH:mm') as [Achievement Time], 
 CDXP.WP_NPCC_DESPATCH_HEADER.TARGET_DEMAND as [Target Demand(MW)]

FROM CDXP.WP_NPCC_DESPATCH_HEADER
INNER JOIN[CDXP].[AP_SUPPLIER_SITE_ALL] on CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_SITE_ID = [CDXP].[AP_SUPPLIER_SITE_ALL].[VENDOR_SITE_ID]
INNER JOIN[CDXP].[AP_SUPPLIERS] on[CDXP].[AP_SUPPLIER_SITE_ALL].vendor_id = [CDXP].[AP_SUPPLIERS].VENDOR_ID
LEFT OUTER JOIN[CDXP].[WP_NPCC_DESPATCH_DETAIL] ON CDXP.WP_NPCC_DESPATCH_HEADER.WP_NPCC_DESPATCH_HEADER_ID = [CDXP].[WP_NPCC_DESPATCH_DETAIL].WP_NPCC_DESPATCH_HEADER_ID WHERE(CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_ID = '" + dataID[1] + @"') AND (CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_SITE_ID = '" + dataID[2] + @"') AND (CDXP.WP_NPCC_DESPATCH_HEADER.STATUS = '" + dataID[3] + @"') AND (CDXP.WP_NPCC_DESPATCH_HEADER.DEMAND_TYPE like '%" + dataID[4] + @"%') AND (CDXP.WP_NPCC_DESPATCH_HEADER.INTIMATION_TIME between '" + dataID[5] + @"' and '" + dataID[6] + @"') ", "tblJ1");

                        }
                        break;

                    case 902:
                        Returnvls = Fn.Data2Json(@"SELECT CDXP.WP_NPCC_DESPATCH_HEADER.WP_NPCC_DESPATCH_HEADER_ID,''[Sr#],FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') [Notification Date & Time] , [CDXP].[AP_SUPPLIERS].VENDOR_NAME [Power Producer], 
[CDXP].[AP_SUPPLIER_SITE_ALL].ADDRESS_LINE1[Producer Site],
[Block / Complex / Unit] = CASE WHEN ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].ATTRIBUTE4, '') = '' THEN ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].BLOCK_UNIT_TITLE,'') ELSE
ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].BLOCK_UNIT_TITLE,'') +' (' + ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].ATTRIBUTE4,'') +')' END,
CDXP.WP_NPCC_DESPATCH_HEADER.DEMAND_TYPE[Demand Type],
CDXP.WP_NPCC_DESPATCH_HEADER.EMERGENCY_TYPE as [Type of Ramp Up / Down],
[CDXP].[WP_NPCC_DESPATCH_DETAIL].SYNC_DESYNC_TIME[Sync / Desync Time],
 CDXP.WP_NPCC_DESPATCH_HEADER.TARGET_DEMAND[Target Demand(MW)],
TARGET_DATE_TIME[Target Date & Time] ,
FORMAT([GC_COMP_ACHIEVE_DATE_TIME],'dd-MMM-yyyy HH:mm') [Achievement Date &Time] ,
STATUS
FROM CDXP.WP_NPCC_DESPATCH_HEADER
INNER JOIN[CDXP].[AP_SUPPLIER_SITE_ALL] on CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_SITE_ID = [CDXP].[AP_SUPPLIER_SITE_ALL].[VENDOR_SITE_ID]
INNER JOIN[CDXP].[AP_SUPPLIERS] on[CDXP].[AP_SUPPLIER_SITE_ALL].vendor_id = [CDXP].[AP_SUPPLIERS].VENDOR_ID
INNER JOIN[CDXP].[CPPA_PPA_PLT_BLK_FUEL] ON[CDXP].[CPPA_PPA_PLT_BLK_FUEL].PLT_BLK_FUEL_ID = CDXP.WP_NPCC_DESPATCH_HEADER.BLOCK_ID
LEFT OUTER JOIN[CDXP].[WP_NPCC_DESPATCH_DETAIL] ON CDXP.WP_NPCC_DESPATCH_HEADER.WP_NPCC_DESPATCH_HEADER_ID = [CDXP].[WP_NPCC_DESPATCH_DETAIL].WP_NPCC_DESPATCH_HEADER_ID WHERE(CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_ID = '" + dataID[1] + @"') AND (CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_SITE_ID = '" + dataID[2] + @"') AND (CDXP.WP_NPCC_DESPATCH_HEADER.STATUS = '" + dataID[3] + @"') AND (CDXP.WP_NPCC_DESPATCH_HEADER.DEMAND_TYPE like '%" + dataID[4] + @"%') AND(CDXP.WP_NPCC_DESPATCH_HEADER.INTIMATION_TIME between '" + dataID[5] + @"' and '" + dataID[6] + @"') AND (CDXP.WP_NPCC_DESPATCH_HEADER.WP_NPCC_DESPATCH_HEADER_ID = '" + dataID[5] + @"')");
                        break;

                    case 0:

                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            var VENDOR_ID0 = Convert.ToInt32(dataID[1]);
                            Returnvls = Fn.Data2Dropdown(db.AP_SUPPLIER_SITE_ALL.Where(w => w.VENDOR_ID == VENDOR_ID0).Select(s => new { VENDOR_SITE_ID = s.VENDOR_SITE_ID, ADDRESS_LINE1 = s.VENDOR_SITE_CODE + " - " + s.ADDRESS_LINE1 }).OrderBy(ord => ord.ADDRESS_LINE1).ToList());
                        }
                        break;

                    case 1: //11-23
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            double VENDOR_ID_FK0 = Convert.ToDouble(dataID[1]);
                            double VENDOR_SITE_ID_FK0 = Convert.ToDouble(dataID[2]);

                            // Returnvls = Fn.Data2Dropdown(
                            //db.PPA_HEADER.Where(hdr => hdr.APPROVAL_STATUS == "Approved" && hdr.VENDOR_SITE_ID_FK == VENDOR_SITE_ID_FK0 && hdr.VENDOR_ID_FK == VENDOR_ID_FK0)
                            //     .Join(db.CPPA_PPA_PLT_BLK_FUEL,
                            //     hdr => hdr.HEADER_ID_PK,
                            //     fuel => fuel.HEADER_ID_FK,
                            //     (Header, Fuel) => new { id = Fuel.PLT_BLK_FUEL_ID + "½" + Fuel.FUEL_LOOKUP_CODE + "½" + Header.CONTRACTED_CAPACITY + "½" + Header.POWER_POLICY, name = Fuel.BLOCK_UNIT_TITLE + " (" + Fuel.ATTRIBUTE4 + ")" }).ToList());
                            Returnvls = Fn.Data2DropdownSQL(@"SELECT CAST(CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID AS VARCHAR(100)) + '½' + CAST(CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS VARCHAR(100)) 
                         + '½' + CAST(CDXP.PPA_HEADER.CONTRACTED_CAPACITY AS VARCHAR(100)) + '½' + CAST(CDXP.PPA_HEADER.POWER_POLICY AS VARCHAR(100)) 
                         + '½' + CAST(CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS VARCHAR(100))+ '½' + CAST(CDXP.PPA_HEADER.IPP_CATEGORY AS VARCHAR(100))  AS id, CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE + ' ( ' + ISNULL(CDXP.CPPA_PPA_PLT_BLK_FUEL.ATTRIBUTE2, 
                         '--') + ' ) ' + CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS Expr1
                            FROM            CDXP.CPPA_PPA_PLT_BLK_FUEL INNER JOIN
                         CDXP.PPA_HEADER ON CDXP.CPPA_PPA_PLT_BLK_FUEL.HEADER_ID_FK = CDXP.PPA_HEADER.HEADER_ID_PK   
                            WHERE PPA_HEADER.APPROVAL_STATUS= 'Approved' AND PPA_HEADER.VENDOR_ID_FK=" + VENDOR_ID_FK0 + @" AND VENDOR_SITE_ID_FK=" + VENDOR_SITE_ID_FK0 + @" AND BLOCK_UNIT_TITLE ='COMPLEX' ORDER BY PLT_BLK_FUEL_ID");
                        }
                        break;
                    case 2: //DAC SAVE
                        var Dcurrent = serverDate.Split(' ');
                        var DcurrentDate = Dcurrent[0].Split('-');
                        var DcurrentTime = Dcurrent[1].Split(':');
                        var ippType = HttpContext.Request.Form["IPPTYPE"];
                        var DACHour = HttpContext.Request.Form["DACHour"];

                        DateTime CurrentTime = pkdatetime;
                        DateTime TargetDate = Convert.ToDateTime(dataID[6]);


                        //DateTime CDate = new DateTime();



                        //CDate = Convert.ToDateTime(pkdatetime.ToShortDateString());
                        //DateTime Tdate = TargetDate.Date;

                        if (DACHour != "0")
                        {
                            if (TargetDate.Date <= CurrentTime.Date)
                            {
                                Returnvls = "FALSE_ENTRY_TIME";
                                break;
                            }
                            else if (CurrentTime.Hour >= Convert.ToInt32(DACHour) && (CurrentTime.AddDays(1)).Date == TargetDate.Date && Convert.ToInt32(DACHour) < 24)
                            {
                                Returnvls = "FALSE_ENTRY_TIME";
                                break;
                            }
                            else if ((CurrentTime.AddDays(2)).Date > TargetDate.Date && Convert.ToInt32(DACHour) >= 24)
                            {
                                Returnvls = "FALSE_ENTRY_TIME";
                                break;
                            }
                        }
                        if (dataID[1] == "0")
                        {

                            int isAvailExists;
                            if (ippType != "20")
                            {
                                isAvailExists = Convert.ToInt32(Fn.ExenID("SELECT COUNT(WP_GC_HOURLY_DATA_HEADER_ID_PK) FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE VENDOR_SITE_ID_FK = " + dataID[4] + " AND TARGET_DATE = '" + dataID[6] + "' AND HOURLY_DATA_TYPE = 'DAC'"));
                            }
                            else
                            {
                                isAvailExists = Convert.ToInt32(Fn.ExenID("SELECT COUNT(WP_GC_HOURLY_DATA_HEADER_ID_PK) FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE VENDOR_SITE_ID_FK = " + dataID[4] + " AND TARGET_DATE = '" + dataID[6] + "' AND HOURLY_DATA_TYPE = 'DAC' AND PLT_BLK_FUEL_ID = " + dataID[11]));
                            }


                            if (isAvailExists >= 1)
                            {
                                Returnvls = "HOURLY_DATA_HEADER_DUPLICATE";
                                break;
                            }
                            string WP_GC_HOURLY_DATA_DETAIL_ID_PK = "0";

                            if (ippType == "10") //HYDEL
                            {


                                try
                                {
                                    WP_GC_HOURLY_DATA_DETAIL_ID_PK = Fn.ExenID(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_HEADER
						                                 (STATUS, HOURLY_DATA_TYPE, VENDOR_SITE_ID_FK, POWER_POLICIY, TARGET_DATE, INTIMATION_DATE_TIME, SENDER_NAME, SENDER_DESIGNATION, SENDER_REMARKS, PLT_BLK_FUEL_ID,  CREATION_DATE, CREATED_BY, SUBMIT_DATE, WITH_REF_AVAIL)
                                VALUES        ('" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"', CDXP.GETDATEpk() ,'" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"','" + dataID[11] + @"', CDXP.GETDATEpk() , [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END,'" + dataID[12] + "');  select SCOPE_IDENTITY();");

                                    var frmdata2 = HttpContext.Request.Form["dtl"];
                                    foreach (var item in frmdata2.Split('│'))
                                    {
                                        if (item != "")
                                        {
                                            string[] D2 = new string[500];
                                            D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                            //var AMBIENT_TEMPERATURE_NULLABLE = D2[1] == "" ? "NULL" : "'" + D2[1] + "'";
                                            var AvailRed = D2[4] == "" ? "NULL" : "'" + D2[4] + "'";

                                            var res = Fn.Exec(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_DETAIL
						            (WP_GC_HOURLY_DATA_HEADER_ID_FK, TARGET_HOUR, HEAD,INFLOW, AVAILABILITY, AVAILABILITY_AT_REF, REMARKS)
                                    VALUES('" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + @"','" + D2[0] + @"','" + D2[1] + @"','" + D2[2] + @"','" + D2[3] + @"'," + AvailRed + @",'" + D2[5] + @"')");
                                            if (res != "1")
                                            {
                                                throw new InvalidOperationException(res);
                                            }
                                        }

                                    }

                                }
                                catch (Exception ex)
                                {
                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK='" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + "'");
                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_DETAIL WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK='" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + "'");
                                    return Convert.ToString("2¼Please contact developer, Exception= " + ex);
                                }
                            }
                            else //NON-HYDEL 
                            {
                                try
                                {
                                    WP_GC_HOURLY_DATA_DETAIL_ID_PK = Fn.ExenID(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_HEADER
						                                     (STATUS, HOURLY_DATA_TYPE, VENDOR_SITE_ID_FK, POWER_POLICIY, TARGET_DATE, INTIMATION_DATE_TIME, SENDER_NAME, SENDER_DESIGNATION, SENDER_REMARKS,  PLT_BLK_FUEL_ID, CURRENT_AVAILABILITY, CREATION_DATE, CREATED_BY, SUBMIT_DATE, WITH_AMBIENT_TEMPERATURE)
                                    VALUES        ('" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"', CDXP.GETDATEpk() ,'" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"','" + dataID[11] + @"','" + dataID[12] + @"', CDXP.GETDATEpk() , [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END,'" + dataID[13] + @"');  select SCOPE_IDENTITY();");
                                    var frmdata2 = HttpContext.Request.Form["dtl"];
                                    foreach (var item in frmdata2.Split('│'))
                                    {
                                        if (item != "")
                                        {
                                            string[] D2 = new string[500];
                                            D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                            var AMBIENT_TEMPERATURE_NULLABLE = D2[1] == "" ? "NULL" : "'" + D2[1] + "'";
                                            var FUEL_TYPE = D2[4] == " " ? "NULL" : "'" + D2[4] + "'";
                                            var SCH_10_ID = D2[6] == " " || D2[6] == "undefined" ? "NULL" : "'" + D2[6] + "'";
                                            var AMBIENT_AVAIL_NULLABLE = D2[2] == "" ? "NULL" : "'" + D2[2] + "'";

                                            var res = Fn.Exec(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_DETAIL
						                (WP_GC_HOURLY_DATA_HEADER_ID_FK, TARGET_HOUR, AMBIENT_TEMPERATURE, AMBIENT_AVAILABILITY, AVAILABILITY, SCH_10_ID,FUEL_TYPE,REMARKS)
                                        VALUES('" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + @"','" + D2[0] + @"'," + AMBIENT_TEMPERATURE_NULLABLE + @"," + AMBIENT_AVAIL_NULLABLE + @",'" + D2[3] + @"'," + SCH_10_ID + @"," + FUEL_TYPE + @",'" + D2[5] + @"')");
                                            if (res != "1")
                                            {
                                                throw new InvalidOperationException(res);
                                            }
                                        }

                                    }
                                }

                                catch (Exception ex)
                                {
                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK='" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + "'");
                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_DETAIL WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK='" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + "'");
                                    return Convert.ToString("2¼Please contact developer, Exception= " + ex);
                                }
                            }


                            Returnvls = "1¼" + WP_GC_HOURLY_DATA_DETAIL_ID_PK;
                        }
                        else //update case 
                        {
                            var HourlyHeaderPk = dataID[1];
                            if (ippType == "10") //HYDEL
                            {

                                try
                                {
                                    Fn.Exec(@"UPDATE       CDXP.WP_GC_HOURLY_DATA_HEADER
                            SET                STATUS ='" + dataID[2] + @"', HOURLY_DATA_TYPE ='" + dataID[3] + @"', VENDOR_SITE_ID_FK ='" + dataID[4] + @"', POWER_POLICIY ='" + dataID[5] + @"', TARGET_DATE ='" + dataID[6] + @"', INTIMATION_DATE_TIME ='" + dataID[7] + "', SENDER_NAME ='" + dataID[8] + @"', SENDER_DESIGNATION ='" + dataID[9] + @"', SENDER_REMARKS ='" + dataID[10] + @"', LAST_UPDATE_DATE =CDXP.GETDATEpk(), LAST_UPDATED_BY =[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), SUBMIT_DATE =CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END
						     WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK=" + dataID[1]);


                                    var frmdata2 = HttpContext.Request.Form["dtl"];
                                    foreach (var item in frmdata2.Split('│'))
                                    {
                                        string[] D2 = new string[500];
                                        D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                        var AvailRed = D2[5] == "" ? "NULL" : "'" + D2[5] + "'";

                                        var res = Fn.Exec(@"UPDATE CDXP.WP_GC_HOURLY_DATA_DETAIL SET  AVAILABILITY_AT_REF =" + AvailRed + @", HEAD ='" + D2[2] + @"', INFLOW ='" + D2[3] + @"',AVAILABILITY ='" + D2[4] + @"',REMARKS ='" + D2[6] + @"' WHERE WP_GC_HOURLY_DATA_DETAIL_ID_PK = '" + D2[0] + @"'");
                                        if (res != "1")
                                        {
                                            return Convert.ToString("FALSE_ENTRY_DATA");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK='" + HourlyHeaderPk + "'");
                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_DETAIL WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK='" + HourlyHeaderPk + "'");
                                    return Convert.ToString("2¼Please contact developer, Exception= " + ex);
                                }

                            }
                            else
                            {
                                try
                                {
                                    Fn.Exec(@"UPDATE       CDXP.WP_GC_HOURLY_DATA_HEADER
                            SET                STATUS ='" + dataID[2] + @"', HOURLY_DATA_TYPE ='" + dataID[3] + @"', VENDOR_SITE_ID_FK ='" + dataID[4] + @"', POWER_POLICIY ='" + dataID[5] + @"', TARGET_DATE ='" + dataID[6] + @"', INTIMATION_DATE_TIME ='" + dataID[7] + "', SENDER_NAME ='" + dataID[8] + @"', SENDER_DESIGNATION ='" + dataID[9] + @"', SENDER_REMARKS ='" + dataID[10] + @"', PLT_BLK_FUEL_ID ='" + dataID[11] + @"', CURRENT_AVAILABILITY ='" + dataID[12] + @"', LAST_UPDATE_DATE =CDXP.GETDATEpk(), LAST_UPDATED_BY =[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), SUBMIT_DATE =CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END, WITH_AMBIENT_TEMPERATURE='" + dataID[13] + @"'
						     WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK=" + dataID[1]);


                                    var frmdata2 = HttpContext.Request.Form["dtl"];
                                    foreach (var item in frmdata2.Split('│'))
                                    {
                                        string[] D2 = new string[500];
                                        D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                        var AMBIENT_TEMPERATURE_NULLABLE = D2[1] == "" ? "NULL" : "'" + D2[1] + "'";
                                        var FUEL_TYPE = D2[4] == " " ? "NULL" : "'" + D2[4] + "'";
                                        var SCH_10_ID = D2[6] == " " || D2[6] == "undefined" ? "NULL" : "'" + D2[6] + "'";
                                        var AMBIENT_AVAIL_NULLABLE = D2[2] == "" ? "NULL" : "'" + D2[2] + "'";


                                        var res = Fn.Exec(@"UPDATE CDXP.WP_GC_HOURLY_DATA_DETAIL SET  AMBIENT_TEMPERATURE =" + AMBIENT_TEMPERATURE_NULLABLE + @", AMBIENT_AVAILABILITY =" + AMBIENT_AVAIL_NULLABLE + @", AVAILABILITY ='" + D2[3] + @"',REMARKS ='" + D2[5] + @"',FUEL_TYPE =" + FUEL_TYPE + @",SCH_10_ID = " + SCH_10_ID + @" WHERE WP_GC_HOURLY_DATA_DETAIL_ID_PK = '" + D2[0] + @"'");

                                        if (res != "1")
                                        {
                                            return Convert.ToString("FALSE_ENTRY_DATA");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK='" + HourlyHeaderPk + "'");
                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_DETAIL WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK='" + HourlyHeaderPk + "'");
                                    return Convert.ToString("2¼Please contact developer, Exception= " + ex);
                                }

                            }


                            Returnvls = "1¼" + HourlyHeaderPk;

                        }
                        break;
                    case 3: //Declaration of Available Capacity List, both for IPP & Admin
                        Fn.Exec("EXEC [CDXP].[SP_DEL_DRAFT_DAC_OLD]");
                        var ipp_Type = HttpContext.Request.Form["IPPTYPE"];
                        if (ipp_Type == "0") // Admin
                        {
                            Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT TOP(100) CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.WP_GC_HOURLY_DATA_HEADER_ID_PK, 0) AS VARCHAR(10)) +'½' + CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS, '') AS VARCHAR(100))
                           + '½' + CASE WHEN CDXP.PPA_HEADER.POWER_POLICY = '2002' OR
                        CDXP.PPA_HEADER.POWER_POLICY = '2015' THEN 'HourlyYes' ELSE 'No' END + '½' + CAST(CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE AS VARCHAR(100)) AS ID, '' AS Sr#,
            FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.TARGET_DATE,
                        'dd-MMM-yyyy') AS[Declaration For],
                        CDXP.AP_SUPPLIERS.VENDOR_NAME AS[Generation Company],
            CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE AS [Declaration Type]
                        , FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.SUBMIT_DATE, 'dd-MMM-yyyy HH:mm') AS [Submit Date],
                         CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS AS Status,
             Case when CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS = 'Acknowledged' then FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.AcknowledgementDateTime, 'dd-MMM-yyyy HH:mm') ELSE '' END AS[Acknowledgement Time],
             ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.SENDER_REMARKS, '')[Generation Company Remarks],
             ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.RECEIVER_REMARKS, '')[NPCC Remarks]
                           FROM
                        CDXP.AP_SUPPLIER_SITE_ALL INNER JOIN
                        CDXP.PPA_HEADER ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CDXP.PPA_HEADER.VENDOR_SITE_ID_FK AND
                        CDXP.PPA_HEADER.VENDOR_ID_FK = CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_ID INNER JOIN
                        CDXP.AP_SUPPLIERS ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                        CDXP.WP_GC_HOURLY_DATA_HEADER ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK  INNER JOIN
                        CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
                        CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID
                       WHERE CDXP.PPA_HEADER.APPROVAL_STATUS= 'Approved'  AND (CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE IN('DAC', 'RDAC', 'ADAC')) AND
    (CDXP.WP_GC_HOURLY_DATA_HEADER.Status = ( CASE WHEN CDXP.WP_GC_HOURLY_DATA_HEADER.CREATED_BY = '177' THEN CDXP.WP_GC_HOURLY_DATA_HEADER.Status END )
    or
    (CDXP.WP_GC_HOURLY_DATA_HEADER.Status <> ( CASE WHEN CDXP.WP_GC_HOURLY_DATA_HEADER.CREATED_BY <> '177' THEN 'Draft'  END )
    ))AND
                       (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')
                       ORDER BY CDXP.WP_GC_HOURLY_DATA_HEADER.CREATION_DATE DESC, CDXP.WP_GC_HOURLY_DATA_HEADER.LAST_UPDATE_DATE DESC", "tblJ1");
                        }
                        else if (ipp_Type == "10")
                        {
                            Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT TOP(10) CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.WP_GC_HOURLY_DATA_HEADER_ID_PK, 0) AS VARCHAR(10)) +'½' + CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS, '') AS VARCHAR(100))
                           + '½' + CASE WHEN CDXP.PPA_HEADER.POWER_POLICY = '2002' OR
                        CDXP.PPA_HEADER.POWER_POLICY = '2015' THEN 'HourlyYes' ELSE 'No' END + '½' + CAST(CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE AS VARCHAR(100)) AS ID, '' AS Sr#,
            FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.TARGET_DATE,
                        'dd-MMM-yyyy') AS[Declaration For],
                        CDXP.AP_SUPPLIERS.VENDOR_NAME AS[Generation Company],
            CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE AS [Declaration Type]
                        , FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.SUBMIT_DATE, 'dd-MMM-yyyy HH:mm') AS [Submit Date],
                         CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS AS Status,
             Case when CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS = 'Acknowledged' then FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.AcknowledgementDateTime, 'dd-MMM-yyyy HH:mm') ELSE '' END AS[Acknowledgement Time],
             ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.SENDER_REMARKS, '')[Generation Company Remarks],
             ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.RECEIVER_REMARKS, '')[NPCC Remarks]
                           FROM
                        CDXP.AP_SUPPLIER_SITE_ALL INNER JOIN
                        CDXP.PPA_HEADER ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CDXP.PPA_HEADER.VENDOR_SITE_ID_FK AND
                        CDXP.PPA_HEADER.VENDOR_ID_FK = CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_ID INNER JOIN
                        CDXP.AP_SUPPLIERS ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                        CDXP.WP_GC_HOURLY_DATA_HEADER ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK  INNER JOIN
                        CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
                        CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID
                       WHERE CDXP.PPA_HEADER.APPROVAL_STATUS= 'Approved' AND (CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE IN('DAC', 'RDAC', 'ADAC')) AND(CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS <>'" + dataID[1] + @"') AND
                       (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')
                       ORDER BY CDXP.WP_GC_HOURLY_DATA_HEADER.CREATION_DATE DESC, CDXP.WP_GC_HOURLY_DATA_HEADER.LAST_UPDATE_DATE DESC", "tblJ1");
                        }
                        else if (ipp_Type == "20" || ipp_Type == "30" || ipp_Type == "40" || ipp_Type == "50" || ipp_Type == "60") //Thermal - Nuclear - Bagasse - Solar - Wind
                        {
                            Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT        CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.WP_GC_HOURLY_DATA_HEADER_ID_PK, 0) AS VARCHAR(10)) + '½' + CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS, '') AS VARCHAR(100))
                        + '½' + CASE WHEN CDXP.PPA_HEADER.POWER_POLICY = '2002' OR
                        CDXP.PPA_HEADER.POWER_POLICY = '2015' THEN 'HourlyYes' ELSE 'No' END + '½' + CAST(CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE AS VARCHAR(100)) AS ID, '' AS Sr#,
              FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.TARGET_DATE,
                        'dd-MMM-yyyy') AS [Declaration For],
                        CDXP.AP_SUPPLIERS.VENDOR_NAME AS [Generation Company],

            CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE AS [Declareation Type]
                        , FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.SUBMIT_DATE, 'dd-MMM-yyyy HH:mm') AS[Submit Date],
                         CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS AS Status,
                       Case when CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS = 'Acknowledged' then FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.AcknowledgementDateTime, 'dd-MMM-yyyy HH:mm') ELSE '' END AS  [Acknowledgement Time],
             ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.SENDER_REMARKS,'') [Generation Company Remarks],
             ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.RECEIVER_REMARKS,'') [NPCC Remarks]
            FROM CDXP.AP_SUPPLIER_SITE_ALL INNER JOIN
                        CDXP.PPA_HEADER ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CDXP.PPA_HEADER.VENDOR_SITE_ID_FK AND
                        CDXP.PPA_HEADER.VENDOR_ID_FK = CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_ID INNER JOIN
                        CDXP.AP_SUPPLIERS ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                        CDXP.WP_GC_HOURLY_DATA_HEADER ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK INNER JOIN
                        CDXP.CPPA_PPA_PLT_BLK_FUEL ON CDXP.WP_GC_HOURLY_DATA_HEADER.PLT_BLK_FUEL_ID = CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID INNER JOIN
                        CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
                        CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID
                       WHERE    CDXP.PPA_HEADER.APPROVAL_STATUS= 'Approved' AND   (CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE IN ('DAC', 'RDAC', 'ADAC')) AND (CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS <> '" + dataID[1] + @"') AND
                        (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')
                       ORDER BY CDXP.WP_GC_HOURLY_DATA_HEADER.CREATION_DATE DESC ,CDXP.WP_GC_HOURLY_DATA_HEADER.LAST_UPDATE_DATE DESC", "tblJ1");
                        }

                        break;
                    case 4: //LOAD DAC

                        Returnvls = Fn.Data2Json("EXEC [CDXP].[sp_NPCC_Declaration_Avalable_Capacity_Load] " + dataID[1]);
                        break;

                    case 5:

                        Returnvls = Fn.Data2Json("EXEC [CDXP].[sp_NPCC_NEW_ADAC_RDAC] " + dataID[1]);
                        break;
                    case 6://Revision DAC/rdac
                        if (dataID[1] == "0")
                        {

                            if (dataID[3] == "ADAC")
                            {
                                string WP_GC_HOURLY_DATA_DETAIL_ID_PK = "0";
                                try
                                {
                                    WP_GC_HOURLY_DATA_DETAIL_ID_PK = Fn.ExenID(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_HEADER
						 (STATUS, HOURLY_DATA_TYPE, VENDOR_SITE_ID_FK, POWER_POLICIY, TARGET_DATE, INTIMATION_DATE_TIME, SENDER_NAME, SENDER_DESIGNATION, SENDER_REMARKS,  PLT_BLK_FUEL_ID , PARENT_ID, REVISION_TYPE, CREATION_DATE, CREATED_BY, SUBMIT_DATE)
                            VALUES        
                            ('" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"', CDXP.GETDATEpk() ,'" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"','" + dataID[11] + @"','" + dataID[12] + @"','" + dataID[13] + @"', CDXP.GETDATEpk() , [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END);  select SCOPE_IDENTITY();");
                                    var frmdata2 = HttpContext.Request.Form["dtl"];
                                    foreach (var item in frmdata2.Split('│'))
                                    {
                                        string[] D2 = new string[500];
                                        D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                        var AMBIENT_TEMPERATURE_NULLABLE = D2[1] == "" ? "NULL" : "'" + D2[1] + "'";
                                        var FUEL_TYPE = D2[4] == " " ? "NULL" : "'" + D2[4] + "'";
                                        var SCH_10_ID = D2[7] == " " || D2[7] == "undefined" ? "NULL" : "'" + D2[7] + "'";
                                        var AMBIENT_AVAIL_NULLABLE = D2[2] == "" ? "NULL" : "'" + D2[2] + "'";
                                        var res = Fn.Exec(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_DETAIL
						 (WP_GC_HOURLY_DATA_HEADER_ID_FK, TARGET_HOUR, AMBIENT_TEMPERATURE, AMBIENT_AVAILABILITY, AVAILABILITY, LAST_UPDATED, FUEL_TYPE, REMARKS,REVISED_TEMPERATURE,SCH_10_ID)
                            VALUES('" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + @"','" + D2[0] + @"'," + AMBIENT_TEMPERATURE_NULLABLE + @"," + AMBIENT_AVAIL_NULLABLE + @",'" + D2[5] + @"',CDXP.GETDATEpk()," + FUEL_TYPE + @",'" + D2[6] + @"','" + D2[3] + @"'," + SCH_10_ID + @")");

                                        if (res != "1")
                                        {
                                            throw new InvalidOperationException(res);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK='" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + "'");
                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_DETAIL WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK='" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + "'");
                                    return Convert.ToString("2¼Please contact developer, Exception= " + ex);
                                }



                                Returnvls = "1¼" + WP_GC_HOURLY_DATA_DETAIL_ID_PK;
                            }

                            else
                            { //rdac
                                string WP_GC_HOURLY_DATA_DETAIL_ID_PK = "0";
                                try
                                {
                                    WP_GC_HOURLY_DATA_DETAIL_ID_PK = Fn.ExenID(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_HEADER
						 (STATUS, HOURLY_DATA_TYPE, VENDOR_SITE_ID_FK, POWER_POLICIY, TARGET_DATE, INTIMATION_DATE_TIME, SENDER_NAME, SENDER_DESIGNATION, SENDER_REMARKS,  PLT_BLK_FUEL_ID , PARENT_ID, REVISION_TYPE, CREATION_DATE, CREATED_BY, SUBMIT_DATE)
                            VALUES        
                            ('" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"', CDXP.GETDATEpk() ,'" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"','" + dataID[11] + @"','" + dataID[12] + @"','" + dataID[13] + @"', CDXP.GETDATEpk() , [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END);  select SCOPE_IDENTITY();");
                                    var frmdata2 = HttpContext.Request.Form["dtl"];
                                    foreach (var item in frmdata2.Split('│'))
                                    {
                                        if (HttpContext.Request.Form["IPPTYPE"] == "10")
                                        {
                                            string[] D2 = new string[500];
                                            D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                            //var AMBIENT_TEMPERATURE_NULLABLE = D2[1] == "" ? "NULL" : "'" + D2[1] + "'";
                                            //var FUEL_TYPE = D2[4] == " " ? "NULL" : "'" + D2[4] + "'";
                                            var res = Fn.Exec(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_DETAIL
						                (WP_GC_HOURLY_DATA_HEADER_ID_FK, TARGET_HOUR, HEAD, INFLOW, AVAILABILITY, REMARKS)
                                        VALUES('" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + @"','" + D2[0] + @"'," + D2[1] + @",'" + D2[2] + @"','" + D2[3] + @"','" + D2[4] + @"')");

                                            if (res != "1")
                                            {
                                                throw new InvalidOperationException(res);
                                            }
                                        }
                                        else
                                        {
                                            string[] D2 = new string[500];
                                            D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                            var AMBIENT_TEMPERATURE_NULLABLE = D2[1] == "" ? "NULL" : "'" + D2[1] + "'";
                                            var FUEL_TYPE = D2[5] == " " ? "NULL" : "'" + D2[5] + "'";
                                            var SCH_10_ID = D2[6] == " " || D2[6] == "undefined" ? "NULL" : "'" + D2[6] + "'";
                                            var AMBIENT_AVAIL_NULLABLE = D2[2] == "" ? "NULL" : "'" + D2[2] + "'";

                                            var res = Fn.Exec(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_DETAIL
						 (WP_GC_HOURLY_DATA_HEADER_ID_FK, TARGET_HOUR, AMBIENT_TEMPERATURE, AMBIENT_AVAILABILITY, AVAILABILITY, LAST_UPDATED, FUEL_TYPE, REMARKS, SCH_10_ID)
                            VALUES('" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + @"','" + D2[0] + @"'," + AMBIENT_TEMPERATURE_NULLABLE + @"," + AMBIENT_AVAIL_NULLABLE + @",'" + D2[3] + @"',CDXP.GETDATEpk()," + FUEL_TYPE + @",'" + D2[4] + @"'," + SCH_10_ID + @")");

                                            if (res != "1")
                                            {
                                                throw new InvalidOperationException(res);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK='" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + "'");
                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_DETAIL WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK='" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + "'");
                                    return Convert.ToString("ERROR_WHILE_INSERTION½: Exception= " + ex);
                                }

                                Returnvls = "1¼" + WP_GC_HOURLY_DATA_DETAIL_ID_PK;


                            }
                        }
                        else //update (not implemented)
                        {
                            if (dataID[3] == "ADAC")
                            {
                                Fn.Exec(@"UPDATE       CDXP.WP_GC_HOURLY_DATA_HEADER
                            SET  STATUS ='" + dataID[2] + @"', HOURLY_DATA_TYPE ='" + dataID[3] + @"', VENDOR_SITE_ID_FK ='" + dataID[4] + @"', POWER_POLICIY ='" + dataID[5] + @"', TARGET_DATE ='" + dataID[6] + @"', INTIMATION_DATE_TIME ='" + dataID[7] + @"', SENDER_NAME ='" + dataID[8] + @"', SENDER_DESIGNATION ='" + dataID[9] + @"', SENDER_REMARKS ='" + dataID[10] + @"', PLT_BLK_FUEL_ID ='" + dataID[11] + @"'
						, LAST_UPDATE_DATE =CDXP.GETDATEpk(), LAST_UPDATED_BY =[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), SUBMIT_DATE =CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END
                           WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK=" + dataID[1]);


                                var frmdata2 = HttpContext.Request.Form["dtl"];
                                foreach (var item in frmdata2.Split('│'))
                                {
                                    string[] D2 = new string[500];
                                    D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                    var AMBIENT_TEMPERATURE_NULLABLE = D2[1] == "" ? "NULL" : "'" + D2[1] + "'";
                                    Fn.Exec(@"UPDATE CDXP.WP_GC_HOURLY_DATA_DETAIL SET  AMBIENT_TEMPERATURE =" + AMBIENT_TEMPERATURE_NULLABLE + @", AMBIENT_AVAILABILITY ='" + D2[2] + @"', AVAILABILITY ='" + D2[3] + @"',LAST_UPDATED = CDXP.GETDATEpk(), FUEL_TYPE= '" + D2[5] + @"' , REMARKS ='" + D2[4] + @"' WHERE WP_GC_HOURLY_DATA_DETAIL_ID_PK = '" + D2[0] + @"'");
                                }
                            }
                            else
                            {

                                Fn.Exec(@"UPDATE       CDXP.WP_GC_HOURLY_DATA_HEADER
                            SET  STATUS ='" + dataID[2] + @"', HOURLY_DATA_TYPE ='" + dataID[3] + @"', VENDOR_SITE_ID_FK ='" + dataID[4] + @"', POWER_POLICIY ='" + dataID[5] + @"', TARGET_DATE ='" + dataID[6] + @"', INTIMATION_DATE_TIME ='" + dataID[7] + @"', SENDER_NAME ='" + dataID[8] + @"', SENDER_DESIGNATION ='" + dataID[9] + @"', SENDER_REMARKS ='" + dataID[10] + @"', PLT_BLK_FUEL_ID ='" + dataID[11] + @"'
						, LAST_UPDATE_DATE =CDXP.GETDATEpk(), LAST_UPDATED_BY =[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), SUBMIT_DATE =CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END
                           WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK=" + dataID[1]);


                                var frmdata2 = HttpContext.Request.Form["dtl"];
                                foreach (var item in frmdata2.Split('│'))
                                {
                                    string[] D2 = new string[500];
                                    D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                    var AMBIENT_TEMPERATURE_NULLABLE = D2[1] == "" ? "NULL" : "'" + D2[1] + "'";
                                    Fn.Exec(@"UPDATE CDXP.WP_GC_HOURLY_DATA_DETAIL SET  AMBIENT_TEMPERATURE =" + AMBIENT_TEMPERATURE_NULLABLE + @", AMBIENT_AVAILABILITY ='" + D2[2] + @"', AVAILABILITY ='" + D2[3] + @"',LAST_UPDATED = CDXP.GETDATEpk(), FUEL_TYPE= '" + D2[5] + @"' , REMARKS ='" + D2[4] + @"' WHERE WP_GC_HOURLY_DATA_DETAIL_ID_PK = '" + D2[0] + @"'");
                                }

                                Returnvls = dataID[1];
                            }


                        }
                        break;
                    case 7:
                        Returnvls = Convert.ToString(pkdatetime.AddMinutes(Convert.ToInt32(dataID[1])).Hour);
                        break;

                    case 8:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.WP_GC_HOURLY_DATA_HEADER_ID_PK,0) AS VARCHAR(10))+'½'+ CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS,'') AS VARCHAR(100)) ID ,'' [Sr#], CDXP.AP_SUPPLIERS.VENDOR_NAME AS [Vendor Name], ISNULL(CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_CODE, '') 
						 + ' - ' + ISNULL(CDXP.AP_SUPPLIER_SITE_ALL.ADDRESS_LINE1, '') AS [Vendor Site], CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE [Block], CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE [Fuel], 
						  CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE [Hourly Type], 
						 CDXP.WP_GC_HOURLY_DATA_HEADER.POWER_POLICIY [Policy],
						 CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS, 
						 FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.TARGET_DATE,'dd-MMM-yyyy') [Target Date] 
						 , FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.INTIMATION_DATE_TIME,'dd-MMM-yyyy HH:mm') [INTIMATION Time], 
						 CDXP.WP_GC_HOURLY_DATA_HEADER.SENDER_NAME [Sender], CDXP.WP_GC_HOURLY_DATA_HEADER.SENDER_DESIGNATION [Sender Designation]
						 FROM            CDXP.AP_SUPPLIER_SITE_ALL INNER JOIN
						 CDXP.AP_SUPPLIERS ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
						 CDXP.WP_GC_HOURLY_DATA_HEADER ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK INNER JOIN
						 CDXP.CPPA_PPA_PLT_BLK_FUEL ON CDXP.WP_GC_HOURLY_DATA_HEADER.PLT_BLK_FUEL_ID = CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID WHERE CDXP.WP_GC_HOURLY_DATA_HEADER.[PARENT_ID] IS NULL AND CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS='Submit'", "tblJ1");
                        break;


                    case 9:
                        Returnvls = Fn.Data2Json(@"
                          SELECT CDXP.WP_GC_HOURLY_DATA_DETAIL.TARGET_HOUR,
                        [CDXP].[FN_LAST_REMARKS_BY_DACID](CDXP.WP_GC_HOURLY_DATA_HEADER.WP_GC_HOURLY_DATA_HEADER_ID_PK, CDXP.WP_GC_HOURLY_DATA_DETAIL.TARGET_HOUR) AS REMARKS,
                        ISNULL(CAST(CDXP.WP_GC_HOURLY_DATA_DETAIL.AMBIENT_TEMPERATURE as nvarchar(100)),'')  AMBIENT_TEMPERATURE,
						 ISNULL(CAST(CDXP.WP_GC_HOURLY_DATA_DETAIL.AMBIENT_AVAILABILITY as nvarchar(100)),'') AMBIENT_AVAILABILITY,
                         CDXP.WP_GC_HOURLY_DATA_DETAIL.AVAILABILITY AS DAC,                        
                         [CDXP].[FN_LAST_REVISION_BY_DACID](CDXP.WP_GC_HOURLY_DATA_HEADER.WP_GC_HOURLY_DATA_HEADER_ID_PK, CDXP.WP_GC_HOURLY_DATA_DETAIL.TARGET_HOUR) AS LAST_REVISON,
                         CDXP.WP_GC_HOURLY_DATA_DETAIL.WP_GC_HOURLY_DATA_HEADER_ID_FK
                         FROM            CDXP.WP_GC_HOURLY_DATA_DETAIL INNER JOIN
                         CDXP.WP_GC_HOURLY_DATA_HEADER ON
                         CDXP.WP_GC_HOURLY_DATA_DETAIL.WP_GC_HOURLY_DATA_HEADER_ID_FK = CDXP.WP_GC_HOURLY_DATA_HEADER.WP_GC_HOURLY_DATA_HEADER_ID_PK
                         where CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK = " + dataID[1] + @" and
                         CDXP.WP_GC_HOURLY_DATA_HEADER.PLT_BLK_FUEL_ID = " + dataID[2] + @" and
                         CDXP.WP_GC_HOURLY_DATA_HEADER.PARENT_ID is null and
                         FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.TARGET_DATE,'dd-MMM-yyyy') ='" + dataID[3] + @"'
                         ORDER BY CDXP.WP_GC_HOURLY_DATA_DETAIL.TARGET_HOUR");
                        break;


                    case 10:
                        //                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT ID, Sr#, [Vendor Name], [Vendor Site], Block, Fuel, [Hourly Type], Policy, STATUS, [Target Date], [Intimation Time] FROM (


                        //SELECT        CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.WP_GC_HOURLY_DATA_HEADER_ID_PK, 0) AS VARCHAR(10)) + '½' + CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS, '') AS VARCHAR(100)) 
                        //                         + '½' + CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.PARENT_ID, '') AS VARCHAR(100)) AS ID, '' AS Sr#, CDXP.AP_SUPPLIERS.VENDOR_NAME AS [Vendor Name], 
                        //                         ISNULL(CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_CODE, '') + ' - ' + ISNULL(CDXP.AP_SUPPLIER_SITE_ALL.ADDRESS_LINE1, '') AS [Vendor Site], CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE AS Block,
                        //                          CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS Fuel, CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE AS [Hourly Type], 
                        //                         CDXP.WP_GC_HOURLY_DATA_HEADER.POWER_POLICIY AS Policy, CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS, FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.TARGET_DATE, 'dd-MMM-yyyy') 
                        //                         AS [Target Date], FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.INTIMATION_DATE_TIME, 'dd-MMM-yyyy HH:mm') AS [Intimation Time], CDXP.WP_GC_HOURLY_DATA_HEADER.LAST_UPDATE_DATE
                        //FROM            CDXP.AP_SUPPLIER_SITE_ALL INNER JOIN
                        //                         CDXP.AP_SUPPLIERS ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                        //                         CDXP.WP_GC_HOURLY_DATA_HEADER ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK INNER JOIN
                        //                         CDXP.CPPA_PPA_PLT_BLK_FUEL ON CDXP.WP_GC_HOURLY_DATA_HEADER.PLT_BLK_FUEL_ID = CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID INNER JOIN
                        //                         CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND 
                        //                         CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID INNER JOIN
                        //                         CDXP.WP_PORTAL_USERS ON CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = CDXP.WP_PORTAL_USERS.WP_PORTAL_USERS_ID
                        //WHERE        (CDXP.WP_GC_HOURLY_DATA_HEADER.PARENT_ID IS NOT NULL) AND (CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS <> '" + dataID[1] + @"') AND 
                        //                         (CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE = 'DAD') AND (CDXP.WP_PORTAL_USERS.USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"')



                        //) AS F ORDER BY F.LAST_UPDATE_DATE DESC", "tblJ1");
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT TOP(10) CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.WP_GC_HOURLY_DATA_HEADER_ID_PK, 0) AS VARCHAR(10)) + '½' + CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS, '') AS VARCHAR(100)) 
                         + '½' + CASE WHEN CDXP.PPA_HEADER.POWER_POLICY = '2002' OR
                         CDXP.PPA_HEADER.POWER_POLICY = '2015' THEN 'HourlyYes' ELSE 'No' END + '½' + CAST(CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE AS VARCHAR(100)) AS ID, '' AS Sr#,
						 FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.TARGET_DATE, 
                         'dd-MMM-yyyy') AS [Demand For],
                         CDXP.AP_SUPPLIERS.VENDOR_NAME AS [Generation Company], 
						 ISNULL(CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_CODE, '') + ' - ' + ISNULL(CDXP.AP_SUPPLIER_SITE_ALL.ADDRESS_LINE1, '') AS [Generation Company Site], 
                         CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE AS [Block/Complex], 
						 CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS Fuel, CDXP.WP_GC_HOURLY_DATA_HEADER.POWER_POLICIY AS Policy,
						 CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE AS [Declareation Type]
						 ,  FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.INTIMATION_DATE_TIME, 'dd-MMM-yyyy HH:mm') AS [Intimation Time],
                          CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS AS Status,
						  ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.SENDER_REMARKS,'') [NPCC Remarks],
						  ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.RECEIVER_REMARKS,'') [Generation Company Remarks],
						  Case when CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS = 'Acknowledged' then FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.AcknowledgementDateTime, 'dd-MMM-yyyy HH:mm') ELSE '' END AS  [Acknowledgement Time]
FROM            CDXP.AP_SUPPLIER_SITE_ALL INNER JOIN
                         CDXP.PPA_HEADER ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CDXP.PPA_HEADER.VENDOR_SITE_ID_FK AND 
                         CDXP.PPA_HEADER.VENDOR_ID_FK = CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_ID INNER JOIN
                         CDXP.AP_SUPPLIERS ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                         CDXP.WP_GC_HOURLY_DATA_HEADER ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK INNER JOIN
                         CDXP.CPPA_PPA_PLT_BLK_FUEL ON CDXP.WP_GC_HOURLY_DATA_HEADER.PLT_BLK_FUEL_ID = CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID INNER JOIN
                         CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND 
                         CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID 
WHERE     APPROVAL_STATUS = 'Approved' AND   (CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE IN ('DAD')) AND (CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS <> '" + dataID[1] + @"') AND 
                         (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')
ORDER BY CDXP.WP_GC_HOURLY_DATA_HEADER.CREATION_DATE DESC ,CDXP.WP_GC_HOURLY_DATA_HEADER.LAST_UPDATE_DATE DESC", "tblJ1");
                        break;
                    case 11:
                        Returnvls = Fn.Data2Json("EXEC [CDXP].[sp_NPCC_Declaration_Avalable_Capacity_Load_DAD] " + dataID[1]);
                        break;
                    case 12:
                        Returnvls = Fn.Data2Json(@"UPDATE CDXP.WP_GC_HOURLY_DATA_HEADER
SET               AcknowledgementDateTime=CDXP.GETDATEpk(),  LAST_UPDATE_DATE =CDXP.GETDATEpk(), LAST_UPDATED_BY ='" + Convert.ToString(Session["UserID"]) + @"', RECEIVER_REMARKS ='" + dataID[4] + @"', RECEIVER_DESIGNATION ='" + dataID[3] + @"', RECEIVER_NAME ='" + dataID[2] + @"', STATUS ='" + dataID[5] + @"' WHERE (WP_GC_HOURLY_DATA_HEADER_ID_PK = '" + dataID[1] + @"')");
                        break;

                    case 13:
                        Returnvls = Fn.Data2Json(@"EXEC [CDXP].[SP_IMPORT_LAST_DAC] " + dataID[2] + ", '" + dataID[1] + "'");
                        break;

                    case 14:
                        Returnvls = Fn.Data2Json("EXEC [CDXP].[sp_NPCC_Declaration_Avalable_Capacity_LoadRDAC_ADAC_View_Clone] " + dataID[1]);
                        break;

                    case 15:
                        if (dataID[1] == "0")
                        {
                            int isAvailExist = Convert.ToInt32(Fn.ExenID("SELECT COUNT(WP_GC_HOURLY_DATA_HEADER_ID_PK) FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE VENDOR_SITE_ID_FK = " + dataID[4] + " AND TARGET_DATE = '" + dataID[6] + "' AND HOURLY_DATA_TYPE = 'DAD' AND PLT_BLK_FUEL_ID = " + dataID[12]));

                            if (isAvailExist >= 1)
                            {
                                Returnvls = "HOURLY_DATA_HEADER_DUPLICATE";
                                break;
                            }
                            string WP_GC_HOURLY_DATA_DETAIL_ID_PK = Fn.ExenID(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_HEADER
						 (STATUS, HOURLY_DATA_TYPE, VENDOR_SITE_ID_FK, POWER_POLICIY, TARGET_DATE, INTIMATION_DATE_TIME, IPP_CAT, SENDER_NAME, SENDER_DESIGNATION, SENDER_REMARKS,  PLT_BLK_FUEL_ID , PARENT_ID, REVISION_TYPE, CREATION_DATE, CREATED_BY, SUBMIT_DATE)
                         VALUES        ('" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"','" + dataID[7] + @"','" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"','" + dataID[11] + @"','" + dataID[12] + @"','" + dataID[13] + @"','" + dataID[14] + @"', CDXP.GETDATEpk() , [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END);  select SCOPE_IDENTITY();");
                            var frmdata2 = HttpContext.Request.Form["dtl"];

                            int isDemandExist = Convert.ToInt32(Fn.ExenID("SELECT COUNT(WP_GC_HOURLY_DATA_DETAIL_ID_PK) FROM CDXP.WP_GC_HOURLY_DATA_DETAIL WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK = " + WP_GC_HOURLY_DATA_DETAIL_ID_PK));

                            if (isDemandExist > 0)
                            {
                                Returnvls = "HOURLY_DATA_DETAIL_DUPLICATE";
                                break;
                            }

                            foreach (var item in frmdata2.Split('│'))
                            {
                                string[] D2 = new string[500];
                                D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                var AMBIENT_AVAIL_NULLABLE = D2[2] == "" ? "NULL" : "'" + D2[2] + "'";

                                Fn.Exec(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_DETAIL
						 (WP_GC_HOURLY_DATA_HEADER_ID_FK, TARGET_HOUR, AMBIENT_TEMPERATURE, AMBIENT_AVAILABILITY, AVAILABILITY, REMARKS)
                        VALUES('" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + @"','" + D2[0] + @"'," + D2[1] + @"," + AMBIENT_AVAIL_NULLABLE + @",'" + D2[3] + @"','" + D2[4] + @"')");
                            }

                            Returnvls = WP_GC_HOURLY_DATA_DETAIL_ID_PK;
                            Fn.Exec(@" CDXP.SP_Notification_Submission_Record '" + Returnvls + "','1','" + dataID[4] + @"','" + dataID[4] + @"','1','" + dataID[9] + @"','" + serverDate + "' ");


                        }
                        else
                        {
                            Fn.Exec(@"UPDATE       CDXP.WP_GC_HOURLY_DATA_HEADER
SET                STATUS ='" + dataID[2] + @"', HOURLY_DATA_TYPE ='" + dataID[3] + @"', VENDOR_SITE_ID_FK ='" + dataID[4] + @"', POWER_POLICIY ='" + dataID[5] + @"', TARGET_DATE ='" + dataID[6] + @"', INTIMATION_DATE_TIME ='" + dataID[7] + @"', SENDER_NAME ='" + dataID[9] + @"', SENDER_DESIGNATION ='" + dataID[10] + @"', SENDER_REMARKS ='" + dataID[11] + @"', PLT_BLK_FUEL_ID ='" + dataID[12] + @"'
						, LAST_UPDATE_DATE =CDXP.GETDATEpk(), LAST_UPDATED_BY =[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), SUBMIT_DATE =CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END
WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK=" + dataID[1]);

                            var frmdata2 = HttpContext.Request.Form["dtl"];
                            foreach (var item in frmdata2.Split('│'))
                            {
                                string[] D2 = new string[500];
                                D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                var AMBIENT_AVAIL_NULLABLE = D2[2] == "" ? "NULL" : "'" + D2[2] + "'";

                                Fn.Exec(@"UPDATE CDXP.WP_GC_HOURLY_DATA_DETAIL SET  AMBIENT_TEMPERATURE =" + D2[1] + @", AMBIENT_AVAILABILITY =" + AMBIENT_AVAIL_NULLABLE + @", AVAILABILITY ='" + D2[3] + @"', REMARKS ='" + D2[4] + @"' WHERE WP_GC_HOURLY_DATA_DETAIL_ID_PK = '" + D2[0] + @"'");
                            }

                            Returnvls = dataID[1];

                        }
                        break;

                    case 16:
                        Returnvls = Fn.Data2Json("EXEC [CDXP].[sp_NPCC_NEW_ADAC_RDAC] " + dataID[1]);
                        break;
                    case 17:
                        string Vendor_id_Cond, Dec_Type_Cond;
                        if (dataID[2] == "%")
                        {
                            Vendor_id_Cond = "";
                        }
                        else
                        {
                            Vendor_id_Cond = "and CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = '" + dataID[2] + "'";
                        }
                        if (dataID[3] == "%")
                        {
                            Dec_Type_Cond = "and CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE like '%" + dataID[3] + "%'";
                        }
                        else
                        {
                            Dec_Type_Cond = "and CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE ='" + dataID[3] + "'";
                        }
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.WP_GC_HOURLY_DATA_HEADER_ID_PK, 0) AS VARCHAR(10)) + '½' + CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS, '') AS VARCHAR(100))
                         + '½' + CASE WHEN CDXP.PPA_HEADER.POWER_POLICY = '2002' OR
                         CDXP.PPA_HEADER.POWER_POLICY = '2015' THEN 'HourlyYes' ELSE 'No' END + '½' + CAST(CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE AS VARCHAR(100)) AS ID, '' AS Sr#,

						 FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.TARGET_DATE,
                         'dd-MMM-yyyy') AS [Declaration For],
                         CDXP.AP_SUPPLIERS.VENDOR_NAME AS [Generation Company],
						 CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE AS [Declaration Type]
                         , FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.SUBMIT_DATE, 'dd-MMM-yyyy HH:mm') AS [Submit Date],
                          CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS AS Status,
                         Case when CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS = 'Acknowledged' then FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.AcknowledgementDateTime, 'dd-MMM-yyyy HH:mm') ELSE '' END AS  [Acknowledgement Time],
						  ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.SENDER_REMARKS,'') [Generation Company Remarks],
						  ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.RECEIVER_REMARKS,'') [NPCC Remarks]

FROM            CDXP.AP_SUPPLIER_SITE_ALL INNER JOIN
                         CDXP.PPA_HEADER ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CDXP.PPA_HEADER.VENDOR_SITE_ID_FK AND
                         CDXP.PPA_HEADER.VENDOR_ID_FK = CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_ID INNER JOIN
                         CDXP.AP_SUPPLIERS ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                         CDXP.WP_GC_HOURLY_DATA_HEADER ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK INNER JOIN
                         CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
                         CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID
WHERE    CDXP.PPA_HEADER.APPROVAL_STATUS= 'Approved' AND (CDXP.WP_GC_HOURLY_DATA_HEADER.TARGET_DATE between CAST('" + dataID[5] + "' AS DATETIME) and CAST('" + dataID[6] + "' AS DATETIME)) and   (CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE IN ('DAC', 'RDAC', 'ADAC')) " + Vendor_id_Cond + Dec_Type_Cond + " AND  (CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS  LIKE '%" + dataID[4] + @"%') AND
                         (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = CDXP.FN_WP_PORTAL_USERS_ID_BY_USER_NAME('" + Convert.ToString(Session["UserName"]) + @"'))
 ORDER BY CDXP.WP_GC_HOURLY_DATA_HEADER.CREATION_DATE DESC, CDXP.WP_GC_HOURLY_DATA_HEADER.LAST_UPDATE_DATE DESC", "tblJ1");
                        break;
                    case 18:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT        CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.WP_GC_HOURLY_DATA_HEADER_ID_PK, 0) AS VARCHAR(10)) + '½' + CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS, '') AS VARCHAR(100)) 
                         + '½' + CASE WHEN CDXP.PPA_HEADER.POWER_POLICY = '2002' OR
                         CDXP.PPA_HEADER.POWER_POLICY = '2015' THEN 'HourlyYes' ELSE 'No' END + '½' + CAST(CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE AS VARCHAR(100)) AS ID, '' AS Sr#,
						 FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.TARGET_DATE, 
                         'dd-MMM-yyyy') AS [Demand For],
                         CDXP.AP_SUPPLIERS.VENDOR_NAME AS [Generation Company], 
						 ISNULL(CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_CODE, '') + ' - ' + ISNULL(CDXP.AP_SUPPLIER_SITE_ALL.ADDRESS_LINE1, '') AS [Generation Company Site], 
                         CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE AS [Block/Complex], 
						 CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS Fuel, CDXP.WP_GC_HOURLY_DATA_HEADER.POWER_POLICIY AS Policy,
						 CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE AS [Declareation Type]
						 ,  FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.INTIMATION_DATE_TIME, 'dd-MMM-yyyy HH:mm') AS [Intimation Time],
                          CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS AS Status,
						  ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.SENDER_REMARKS,'') [NPCC Remarks],
						  ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.RECEIVER_REMARKS,'') [Generation Company Remarks],
						  Case when CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS = 'Acknowledged' then FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.AcknowledgementDateTime, 'dd-MMM-yyyy HH:mm') ELSE '' END AS  [Acknowledgement Time]
FROM            CDXP.AP_SUPPLIER_SITE_ALL INNER JOIN
                         CDXP.PPA_HEADER ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CDXP.PPA_HEADER.VENDOR_SITE_ID_FK AND 
                         CDXP.PPA_HEADER.VENDOR_ID_FK = CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_ID INNER JOIN
                         CDXP.AP_SUPPLIERS ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                         CDXP.WP_GC_HOURLY_DATA_HEADER ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK INNER JOIN
                         CDXP.CPPA_PPA_PLT_BLK_FUEL ON CDXP.WP_GC_HOURLY_DATA_HEADER.PLT_BLK_FUEL_ID = CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID INNER JOIN
                         CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND 
                         CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID 
 WHERE    CAST(CDXP.WP_GC_HOURLY_DATA_HEADER.PLT_BLK_FUEL_ID AS VARCHAR(100)) LIKE  '%" + dataID[4] + "%' AND  (CDXP.WP_GC_HOURLY_DATA_HEADER.TARGET_DATE between CAST('" + dataID[10] + "' AS DATETIME) and CAST('" + dataID[11] + "' AS DATETIME)) and   (CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE IN ('DAD')) and CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID='" + dataID[2] + "' and CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE like '%" + dataID[3] + "%' AND  (CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS  LIKE '%" + dataID[9] + @"%') AND 
                         (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')
ORDER BY CDXP.WP_GC_HOURLY_DATA_HEADER.CREATION_DATE DESC , CDXP.WP_GC_HOURLY_DATA_HEADER.LAST_UPDATE_DATE DESC", "tblJ1");
                        break;

                    case 19:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT CDXP.WP_NPCC_DESPATCH_HEADER.WP_NPCC_DESPATCH_HEADER_ID,'' [Sr#],FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') [Notification Date & Time] , [CDXP].[AP_SUPPLIERS].VENDOR_NAME [Power Producer], 
[CDXP].[AP_SUPPLIER_SITE_ALL].ADDRESS_LINE1 [Producer Site],
[Block / Complex / Unit] = CASE WHEN ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].ATTRIBUTE4,'') = '' THEN ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].BLOCK_UNIT_TITLE,'') ELSE
ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].BLOCK_UNIT_TITLE,'') + ' (' + ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].ATTRIBUTE4,'') + ')' END,
CDXP.WP_NPCC_DESPATCH_HEADER.DEMAND_TYPE [Demand Type],
CDXP.WP_NPCC_DESPATCH_HEADER.EMERGENCY_TYPE as [Type of Ramp Up/Down],
[dbo].[WP_NPCC_DESPATCH_DETAIL].SYNC_DESYNC_TIME [Sync/Desync Time],
 TARGET_DEMAND [Target Demand (MW)],
FORMAT(TARGET_DATE_TIME,'dd-MMM-yyyy HH:mm') [Target Date & Time] ,
FORMAT([GC_COMP_ACHIEVE_DATE_TIME],'dd-MMM-yyyy HH:mm') [Achievement Date & Time] ,
STATUS
FROM CDXP.WP_NPCC_DESPATCH_HEADER
INNER JOIN [CDXP].[AP_SUPPLIER_SITE_ALL] on CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_SITE_ID= [CDXP].[AP_SUPPLIER_SITE_ALL] .[VENDOR_SITE_ID]
INNER JOIN [CDXP].[AP_SUPPLIERS] on [CDXP].[AP_SUPPLIER_SITE_ALL].vendor_id = [CDXP].[AP_SUPPLIERS].VENDOR_ID
INNER JOIN [CDXP].[CPPA_PPA_PLT_BLK_FUEL] ON  [CDXP].[CPPA_PPA_PLT_BLK_FUEL].PLT_BLK_FUEL_ID =CDXP.WP_NPCC_DESPATCH_HEADER.BLOCK_ID
LEFT OUTER JOIN [dbo].[WP_NPCC_DESPATCH_DETAIL] ON CDXP.WP_NPCC_DESPATCH_HEADER.WP_NPCC_DESPATCH_HEADER_ID = [dbo].[WP_NPCC_DESPATCH_DETAIL].WP_NPCC_DESPATCH_HEADER_ID WHERE (CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_ID = '" + dataID[1] + @"') AND (CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_SITE_ID = '" + dataID[1] + @"') AND 
  (CDXP.WP_NPCC_DESPATCH_HEADER.BLOCK_ID like  '%" + dataID[3] + @"%') AND  (CDXP.WP_NPCC_DESPATCH_HEADER.DEMAND_TYPE like '%" + dataID[8] + @"%') AND (CDXP.WP_NPCC_DESPATCH_HEADER.INTIMATION_TIME between '" + dataID[10] + @"' and '" + dataID[11] + @"') ", "tblJ1");
                        break;

                    case 20:
                        Returnvls = FnO.HTMLTableWithID_TR_Tag(@"exec [dbo].[SP_HourlyInvoiceListForNPCC]  0", "tblJ1");
                        break;

                    case 21:
                        if (dataID[1] == "0")
                        {
                            // string DISPACTH_DETAIL_ID;

                            Fn.Exec(@"INSERT INTO dbo.PREVIOUS_DISPATCH_DETAIL
						 (VENDOR_ID_FK, VENDOR_SITE_ID_FK, PLT_BLK_FUEL_ID_FK, TARGET_DATE, HOUR_0, HOUR_01, HOUR_02, HOUR_03, HOUR_04, HOUR_05, HOUR_06, HOUR_07, HOUR_08, HOUR_09, HOUR_010, HOUR_011,HOUR_012, HOUR_013, HOUR_014, HOUR_015, HOUR_016, HOUR_017, HOUR_018, HOUR_019, HOUR_020, HOUR_021, HOUR_022, HOUR_023, STATUS)
            VALUES        ('" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"', '" + dataID[10] + @"','" + dataID[11] + @"','" + dataID[12] + @"','" + dataID[13] + @"','" + dataID[14] + @"','" + dataID[15] + @"','" + dataID[16] + @"','" + dataID[17] + @"','" + dataID[18] + @"','" + dataID[19] + @"','" + dataID[20] + @"','" + dataID[21] + @"','" + dataID[22] + @"','" + dataID[23] + @"','" + dataID[24] + @"','" + dataID[25] + @"','" + dataID[26] + @"','" + dataID[27] + @"','" + dataID[28] + @"','" + dataID[29] + @"','" + dataID[30] + @"', '" + dataID[31] + @"', '" + dataID[32] + @"', '" + dataID[33] + @"', '" + dataID[34] + @"',  '" + dataID[2] + @"') ");

                        }



                        else
                        {
                            Fn.Exec(@"UPDATE       dbo.PREVIOUS_DISPATCH_DETAIL
                            SET   STATUS ='" + dataID[2] + @"', VENDOR_ID_FK ='" + dataID[3] + @"', VENDOR_SITE_ID_FK ='" + dataID[4] + @"', PLT_BLK_FUEL_ID_FK ='" + dataID[5] + @"', TARGET_DATE ='" + dataID[10] + @"', HOUR_0 ='" + dataID[11] + @"', HOUR_01 ='" + dataID[12] + @"', HOUR_02 ='" + dataID[13] + @"', HOUR_03 ='" + dataID[14] + @"', HOUR_04 ='" + dataID[15] + @"', HOUR_05 ='" + dataID[16] + @"', HOUR_06 ='" + dataID[17] + @"', HOUR_07 ='" + dataID[18] + @"', HOUR_08 ='" + dataID[19] + @"', HOUR_09 ='" + dataID[20] + @"', HOUR_010 ='" + dataID[21] + @"', HOUR_011 ='" + dataID[22] + @"', HOUR_012 ='" + dataID[23] + @"', HOUR_013 ='" + dataID[24] + @"', HOUR_014 ='" + dataID[25] + @"', HOUR_015 ='" + dataID[26] + @"', HOUR_016 ='" + dataID[27] + @"', HOUR_017 ='" + dataID[28] + @"', HOUR_018 ='" + dataID[29] + @"', HOUR_019 ='" + dataID[30] + @"', HOUR_020 ='" + dataID[31] + @"', HOUR_021 ='" + dataID[32] + @"', HOUR_022 ='" + dataID[33] + @"', HOUR_023 ='" + dataID[34] + @"', LAST_UPDATE_DATE =CDXP.GETDATEpk(), LAST_UPDATED_BY =[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), SUBMIT_DATE =CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END
						 WHERE DISPACTH_DETAIL_ID=" + dataID[1]);

                            Returnvls = dataID[1];

                        }
                        break;


                    case 22:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT CAST(ISNULL(dbo.PREVIOUS_DISPATCH_DETAIL.DISPACTH_DETAIL_ID, 0) AS VARCHAR(10)) + '½' + CAST(ISNULL(dbo.PREVIOUS_DISPATCH_DETAIL.STATUS, '') AS VARCHAR(100)) AS ID, '' AS Sr#,
	                       FORMAT(dbo.PREVIOUS_DISPATCH_DETAIL.TARGET_DATE, 'dd-MMM-yyyy') AS [Target Date],
	                       CDXP.AP_SUPPLIERS.VENDOR_NAME AS [Generation Company], 
	                       ISNULL(CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_CODE, '') + ' - ' + ISNULL(CDXP.AP_SUPPLIER_SITE_ALL.ADDRESS_LINE1, '') AS [Generation Company Site], 
                           CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE AS [Block/Complex], 
	                       CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS Fuel,
	                       dbo.PREVIOUS_DISPATCH_DETAIL.STATUS AS Status
	   
		                    FROM dbo.PREVIOUS_DISPATCH_DETAIL INNER JOIN
                            CDXP.PPA_HEADER ON dbo.PREVIOUS_DISPATCH_DETAIL.VENDOR_SITE_ID_FK = CDXP.PPA_HEADER.VENDOR_SITE_ID_FK AND 
                            CDXP.PPA_HEADER.VENDOR_ID_FK = dbo.PREVIOUS_DISPATCH_DETAIL.VENDOR_ID_FK INNER JOIN
		                    CDXP.AP_SUPPLIER_SITE_ALL ON dbo.PREVIOUS_DISPATCH_DETAIL.VENDOR_SITE_ID_FK = CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID INNER JOIN
                            CDXP.AP_SUPPLIERS ON dbo.PREVIOUS_DISPATCH_DETAIL.VENDOR_ID_FK = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                            CDXP.CPPA_PPA_PLT_BLK_FUEL ON dbo.PREVIOUS_DISPATCH_DETAIL.PLT_BLK_FUEL_ID_FK = CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID 
		                    ORDER BY dbo.PREVIOUS_DISPATCH_DETAIL.DISPACTH_DETAIL_ID DESC", "tblJ1");
                        break;

                    case 23:
                        Returnvls = Fn.Data2Json("EXEC [CDXP].[sp_GET_DESPATCH_VALUES]" + dataID[1]);
                        break;

                    case 24:
                        Returnvls = Convert.ToString(pkdatetime.AddHours(4).Hour);
                        //AddMinutes(45).Hour);
                        break;

                    case 25:
                        Returnvls = Convert.ToString(pkdatetime.AddHours(Convert.ToDouble(dataID[1])).Hour);
                        break;
                    case 26:
                        DateTime Target_Date = Convert.ToDateTime(dataID[1]);
                        DateTime Currrent_Date = pkdatetime;

                        if (Target_Date.Date < Currrent_Date.Date)
                        {
                            Returnvls = "LESS_THAN";

                        }
                        else if (Target_Date.Date == Currrent_Date.Date)
                        {
                            Returnvls = "EQUAL_TO";

                        }
                        else if (Target_Date.Date > Currrent_Date.Date)
                        {
                            Returnvls = "GREATER_THAN";

                        }
                        break;

                    case 27:
                        // Returnvls = Convert.ToString(pkdatetime.AddHours(-4).Hour);
                        Returnvls = Convert.ToString(pkdatetime);
                        //AddMinutes(45).Hour);
                        break;
                    case 28:
                        Returnvls = Fn.Data2Json(@"EXEC [CDXP].[SP_IMPORT_LAST_DAC_HYDEL] " + dataID[1] + "," + dataID[2]);
                        break;
                    case 29:
                        Returnvls = Fn.Data2DropdownSQL(@"SELECT SCH_10_ID, CAST(FUEL_LOOKUP_CODE AS VARCHAR(100)) + ' - ' + CAST(SCH_10_TYPE AS VARCHAR(100)) AS A FROM [CDXP].[CPPA_PPA_SCH_10] WHERE VENDOR_ID =" + dataID[1]);
                        break;
                    case 30:
                        Returnvls = Fn.Data2Json(@"UPDATE CDXP.WP_GC_HOURLY_DATA_HEADER
                        SET AcknowledgementDateTime=CDXP.GETDATEpk(),  LAST_UPDATE_DATE =CDXP.GETDATEpk(), LAST_UPDATED_BY =[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), RECEIVER_DESIGNATION ='NPCC', RECEIVER_NAME ='ADMIN NPCC', STATUS ='Acknowledged' WHERE (WP_GC_HOURLY_DATA_HEADER_ID_PK = '" + dataID[1] + @"')");
                        break;
                    case 31:

                        var exists_SCH_10 = Convert.ToInt32(Fn.ExenID("SELECT COUNT(*) FROM  CDXP.CPPA_PPA_SCH_10 WHERE VENDOR_ID = " + dataID[1]));

                        if (exists_SCH_10 >= 1)
                        {
                            Returnvls = "SCH_10_EXISTS";
                            break;
                        }
                        else
                        {
                            Returnvls = "0";
                            break;
                        }
                    case 32:

                        Returnvls = Fn.ExenID("SELECT IPP_CATEGORY FROM CDXP.AP_SUPPLIERS JOIN CDXP.PPA_HEADER ON VENDOR_ID = VENDOR_ID_FK WHERE VENDOR_ID = " + dataID[1]);
                        break;

                    case 33: //DAC SAVE ADMIN
                        //var Dcurrent = serverDate.Split(' ');
                        //var DcurrentDate = Dcurrent[0].Split('-');
                        //var DcurrentTime = Dcurrent[1].Split(':');
                        var ippType_Admin = HttpContext.Request.Form["IPPTYPE"];
                        //var DACHour = HttpContext.Request.Form["DACHour"];

                        //DateTime CurrentTime = pkdatetime;
                        //DateTime TargetDate = Convert.ToDateTime(dataID[6]);

                        if (dataID[1] == "0")
                        {

                            int isAvailExists;
                            if (ippType_Admin != "20")
                            {
                                isAvailExists = Convert.ToInt32(Fn.ExenID("SELECT COUNT(WP_GC_HOURLY_DATA_HEADER_ID_PK) FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE VENDOR_SITE_ID_FK = " + dataID[4] + " AND TARGET_DATE = '" + dataID[6] + "' AND HOURLY_DATA_TYPE = 'DAC'"));
                            }
                            else
                            {
                                isAvailExists = Convert.ToInt32(Fn.ExenID("SELECT COUNT(WP_GC_HOURLY_DATA_HEADER_ID_PK) FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE VENDOR_SITE_ID_FK = " + dataID[4] + " AND TARGET_DATE = '" + dataID[6] + "' AND HOURLY_DATA_TYPE = 'DAC' AND PLT_BLK_FUEL_ID = " + dataID[15]));
                            }


                            if (isAvailExists >= 1)
                            {
                                Returnvls = "HOURLY_DATA_HEADER_DUPLICATE";
                                break;
                            }
                            string WP_GC_HOURLY_DATA_DETAIL_ID_PK = "0";

                            if (ippType_Admin == "10") //HYDEL
                            {


                                try
                                {
                                    WP_GC_HOURLY_DATA_DETAIL_ID_PK = Fn.ExenID(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_HEADER
						                                 (STATUS, HOURLY_DATA_TYPE, VENDOR_SITE_ID_FK, POWER_POLICIY, TARGET_DATE, INTIMATION_DATE_TIME, SENDER_NAME, SENDER_DESIGNATION, SENDER_REMARKS, PLT_BLK_FUEL_ID,  CREATION_DATE, CREATED_BY, SUBMIT_DATE,WITH_REF_AVAIL)
                                VALUES        ('" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"', CDXP.GETDATEpk() ,'" + dataID[12] + @"','" + dataID[13] + @"','" + dataID[14] + @"','" + dataID[15] + @"', CDXP.GETDATEpk() , [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END," + dataID[16] + @");  select SCOPE_IDENTITY();");

                                    var frmdata2 = HttpContext.Request.Form["dtl"];
                                    foreach (var item in frmdata2.Split('│'))
                                    {
                                        if (item != "")
                                        {
                                            string[] D2 = new string[500];
                                            D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                            //var AMBIENT_TEMPERATURE_NULLABLE = D2[1] == "" ? "NULL" : "'" + D2[1] + "'";
                                            // var FUEL_TYPE = D2[4] == " " ? "NULL" : "'" + D2[4] + "'";
                                            var AvailRed = D2[4] == "" ? "NULL" : "'" + D2[4] + "'";

                                            var res = Fn.Exec(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_DETAIL
						            (WP_GC_HOURLY_DATA_HEADER_ID_FK, TARGET_HOUR, HEAD,INFLOW, AVAILABILITY, AVAILABILITY_AT_REF, REMARKS)
                                    VALUES('" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + @"','" + D2[0] + @"','" + D2[1] + @"','" + D2[2] + @"','" + D2[3] + @"'," + AvailRed + @",'" + D2[5] + @"')");

                                            if (res != "1")
                                            {
                                                throw new InvalidOperationException(res);
                                            }
                                        }

                                    }

                                }
                                catch (Exception ex)
                                {
                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK='" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + "'");
                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_DETAIL WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK='" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + "'");
                                    return Convert.ToString("2¼Please contact developer, Exception= " + ex);
                                }
                            }
                            else //NON-HYDEL 
                            {
                                try
                                {
                                    WP_GC_HOURLY_DATA_DETAIL_ID_PK = Fn.ExenID(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_HEADER
						                                     (STATUS, HOURLY_DATA_TYPE, VENDOR_SITE_ID_FK, POWER_POLICIY, TARGET_DATE, INTIMATION_DATE_TIME, SENDER_NAME, SENDER_DESIGNATION, SENDER_REMARKS,  PLT_BLK_FUEL_ID, CREATION_DATE, CREATED_BY, SUBMIT_DATE)
                                    VALUES        ('" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"', CDXP.GETDATEpk() ,'" + dataID[12] + @"','" + dataID[13] + @"','" + dataID[14] + @"','" + dataID[15] + @"', CDXP.GETDATEpk() , [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END);  select SCOPE_IDENTITY();");
                                    var frmdata2 = HttpContext.Request.Form["dtl"];
                                    foreach (var item in frmdata2.Split('│'))
                                    {
                                        if (item != "")
                                        {
                                            string[] D2 = new string[500];
                                            D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                            var AMBIENT_TEMPERATURE_NULLABLE = D2[1] == "" ? "NULL" : "'" + D2[1] + "'";
                                            var FUEL_TYPE = D2[4] == " " ? "NULL" : "'" + D2[4] + "'";
                                            var SCH_10_ID = D2[6] == " " || D2[6] == "undefined" ? "NULL" : "'" + D2[6] + "'";
                                            var AMBIENT_AVAIL_NULLABLE = D2[2] == "" ? "NULL" : "'" + D2[2] + "'";

                                            var res = Fn.Exec(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_DETAIL
						                (WP_GC_HOURLY_DATA_HEADER_ID_FK, TARGET_HOUR, AMBIENT_TEMPERATURE, AMBIENT_AVAILABILITY, AVAILABILITY, SCH_10_ID,FUEL_TYPE,REMARKS)
                                        VALUES('" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + @"','" + D2[0] + @"'," + AMBIENT_TEMPERATURE_NULLABLE + @"," + AMBIENT_AVAIL_NULLABLE + @",'" + D2[3] + @"'," + SCH_10_ID + @"," + FUEL_TYPE + @",'" + D2[5] + @"')");
                                            if (res != "1")
                                            {
                                                throw new InvalidOperationException(res);
                                            }
                                        }

                                    }
                                }

                                catch (Exception ex)
                                {
                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK='" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + "'");
                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_DETAIL WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK='" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + "'");
                                    return Convert.ToString("2¼Please contact developer, Exception= " + ex);
                                }
                            }


                            Returnvls = "1¼" + WP_GC_HOURLY_DATA_DETAIL_ID_PK;
                        }
                        else //update case 
                        {
                            var HourlyHeaderPk = dataID[1];

                            if (ippType_Admin == "10") //HYDEL
                            {
                                try
                                {
                                    Fn.Exec(@"UPDATE       CDXP.WP_GC_HOURLY_DATA_HEADER
                            SET                STATUS ='" + dataID[2] + @"', HOURLY_DATA_TYPE ='" + dataID[3] + @"', VENDOR_SITE_ID_FK ='" + dataID[4] + @"', POWER_POLICIY ='" + dataID[5] + @"', TARGET_DATE ='" + dataID[6] + @"', INTIMATION_DATE_TIME ='" + dataID[7] + "', SENDER_NAME ='" + dataID[8] + @"', SENDER_DESIGNATION ='" + dataID[9] + @"', SENDER_REMARKS ='" + dataID[10] + @"', LAST_UPDATE_DATE =CDXP.GETDATEpk(), LAST_UPDATED_BY =[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), SUBMIT_DATE =CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END
						     WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK=" + dataID[1]);


                                    var frmdata2 = HttpContext.Request.Form["dtl"];
                                    foreach (var item in frmdata2.Split('│'))
                                    {
                                        string[] D2 = new string[500];
                                        D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');

                                        var res = Fn.Exec(@"UPDATE CDXP.WP_GC_HOURLY_DATA_DETAIL SET  HOURLY_CAP ='" + D2[2] + @"', HEAD ='" + D2[3] + @"', INFLOW ='" + D2[4] + @"',AVAILABILITY ='" + D2[5] + @"',REMARKS ='" + D2[6] + @"' WHERE WP_GC_HOURLY_DATA_DETAIL_ID_PK = '" + D2[0] + @"'");

                                        if (res != "1")
                                        {
                                            return Convert.ToString("FALSE_ENTRY_DATA");

                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK='" + HourlyHeaderPk + "'");
                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_DETAIL WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK='" + HourlyHeaderPk + "'");
                                    return Convert.ToString("2¼Please contact developer, Exception= " + ex);
                                }

                            }
                            else
                            {
                                try
                                {
                                    Fn.Exec(@"UPDATE       CDXP.WP_GC_HOURLY_DATA_HEADER
                            SET                STATUS ='" + dataID[2] + @"', HOURLY_DATA_TYPE ='" + dataID[3] + @"', VENDOR_SITE_ID_FK ='" + dataID[4] + @"', POWER_POLICIY ='" + dataID[5] + @"', TARGET_DATE ='" + dataID[6] + @"', INTIMATION_DATE_TIME ='" + dataID[7] + "', SENDER_NAME ='" + dataID[8] + @"', SENDER_DESIGNATION ='" + dataID[9] + @"', SENDER_REMARKS ='" + dataID[10] + @"',  LAST_UPDATE_DATE =CDXP.GETDATEpk(), LAST_UPDATED_BY =[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), SUBMIT_DATE =CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END, WITH_AMBIENT_TEMPERATURE='" + dataID[13] + @"'
						     WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK=" + dataID[1]);


                                    var frmdata2 = HttpContext.Request.Form["dtl"];
                                    foreach (var item in frmdata2.Split('│'))
                                    {
                                        string[] D2 = new string[500];
                                        D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                        var AMBIENT_TEMPERATURE_NULLABLE = D2[1] == "" ? "NULL" : "'" + D2[1] + "'";
                                        var FUEL_TYPE = D2[4] == " " ? "NULL" : "'" + D2[4] + "'";
                                        var SCH_10_ID = D2[6] == " " || D2[6] == "undefined" ? "NULL" : "'" + D2[6] + "'";
                                        var AMBIENT_AVAIL_NULLABLE = D2[2] == "" ? "NULL" : "'" + D2[2] + "'";


                                        var res = Fn.Exec(@"UPDATE CDXP.WP_GC_HOURLY_DATA_DETAIL SET  AMBIENT_TEMPERATURE =" + AMBIENT_TEMPERATURE_NULLABLE + @", AMBIENT_AVAILABILITY =" + AMBIENT_AVAIL_NULLABLE + @", AVAILABILITY ='" + D2[3] + @"',REMARKS ='" + D2[5] + @"',FUEL_TYPE =" + FUEL_TYPE + @",SCH_10_ID = " + SCH_10_ID + @" WHERE WP_GC_HOURLY_DATA_DETAIL_ID_PK = '" + D2[0] + @"'");
                                        if (res != "1")
                                        {
                                            return Convert.ToString("FALSE_ENTRY_DATA");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK='" + HourlyHeaderPk + "'");
                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_DETAIL WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK='" + HourlyHeaderPk + "'");
                                    return Convert.ToString("2¼Please contact developer, Exception= " + ex);
                                }

                            }


                            Returnvls = "1¼" + HourlyHeaderPk;

                        }
                        break;
                    case 1003:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            Returnvls = Fn.Data2DropdownSQL(@" SELECT DISTINCT CDXP.AP_SUPPLIERS.VENDOR_ID AS id, VENDOR_NAME FROM CDXP.AP_SUPPLIERS INNER JOIN CDXP.PPA_HEADER INNER JOIN CDXP.CPPA_PPA_PLT_BLK_FUEL
                            ON PPA_HEADER.HEADER_ID_PK = CPPA_PPA_PLT_BLK_FUEL.HEADER_ID_FK ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.PPA_HEADER.VENDOR_ID_FK

                            INNER JOIN
                            CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
                            CDXP.PPA_HEADER.VENDOR_SITE_ID_FK = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID
                            WHERE CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE != 'Wind' AND CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE != 'Solar' AND (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"') ORDER BY VENDOR_NAME");
                        }
                        break;

                    case 34://rdac adac submit case for admin
                        if (dataID[1] == "0")
                        {

                            if (dataID[3] == "ADAC")
                            {
                                string WP_GC_HOURLY_DATA_DETAIL_ID_PK = "0";
                                try
                                {
                                    WP_GC_HOURLY_DATA_DETAIL_ID_PK = Fn.ExenID(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_HEADER
                         (STATUS, HOURLY_DATA_TYPE, VENDOR_SITE_ID_FK, POWER_POLICIY, TARGET_DATE, INTIMATION_DATE_TIME, SENDER_NAME, SENDER_DESIGNATION, SENDER_REMARKS,  PLT_BLK_FUEL_ID , PARENT_ID, REVISION_TYPE, CREATION_DATE, CREATED_BY, SUBMIT_DATE)
                            VALUES        
                            ('" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"', CDXP.GETDATEpk() ,'" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"','" + dataID[11] + @"','" + dataID[12] + @"','" + dataID[13] + @"', CDXP.GETDATEpk() , [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END);  select SCOPE_IDENTITY();");
                                    var frmdata2 = HttpContext.Request.Form["dtl"];
                                    foreach (var item in frmdata2.Split('│'))
                                    {
                                        string[] D2 = new string[500];
                                        D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                        var AMBIENT_TEMPERATURE_NULLABLE = D2[1] == "" ? "NULL" : "'" + D2[1] + "'";
                                        var FUEL_TYPE = D2[4] == " " ? "NULL" : "'" + D2[4] + "'";
                                        var SCH_10_ID = D2[7] == " " || D2[7] == "undefined" ? "NULL" : "'" + D2[7] + "'";
                                        var AMBIENT_AVAIL_NULLABLE = D2[2] == "" ? "NULL" : "'" + D2[2] + "'";
                                        var res = Fn.Exec(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_DETAIL
                         (WP_GC_HOURLY_DATA_HEADER_ID_FK, TARGET_HOUR, AMBIENT_TEMPERATURE, AMBIENT_AVAILABILITY, AVAILABILITY, LAST_UPDATED, FUEL_TYPE, REMARKS,REVISED_TEMPERATURE,SCH_10_ID)
                            VALUES('" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + @"','" + D2[0] + @"'," + AMBIENT_TEMPERATURE_NULLABLE + @"," + AMBIENT_AVAIL_NULLABLE + @",'" + D2[5] + @"',CDXP.GETDATEpk()," + FUEL_TYPE + @",'" + D2[6] + @"','" + D2[3] + @"'," + SCH_10_ID + @")");

                                        if (res != "1")
                                        {
                                            throw new InvalidOperationException(res);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK='" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + "'");
                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_DETAIL WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK='" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + "'");
                                    return Convert.ToString("2¼Please contact developer, Exception= " + ex);
                                }


                                Returnvls = "1¼" + WP_GC_HOURLY_DATA_DETAIL_ID_PK;
                            }

                            else
                            { //rdac
                                string WP_GC_HOURLY_DATA_DETAIL_ID_PK = "0";
                                try
                                {
                                    String[] data1 = dataID;
                                    WP_GC_HOURLY_DATA_DETAIL_ID_PK = Fn.ExenID(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_HEADER
                         (STATUS, HOURLY_DATA_TYPE, VENDOR_SITE_ID_FK, POWER_POLICIY, TARGET_DATE, INTIMATION_DATE_TIME, SENDER_NAME, SENDER_DESIGNATION, SENDER_REMARKS,  PLT_BLK_FUEL_ID , PARENT_ID, REVISION_TYPE, CREATION_DATE, CREATED_BY, SUBMIT_DATE)
                            VALUES        
                            ('" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"', CDXP.GETDATEpk() ,'" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"','" + dataID[11] + @"','" + dataID[12] + @"','" + dataID[13] + @"', CDXP.GETDATEpk() , [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END);  select SCOPE_IDENTITY();");
                                    var frmdata2 = HttpContext.Request.Form["dtl"];
                                    foreach (var item in frmdata2.Split('│'))
                                    {
                                        if (HttpContext.Request.Form["IPPTYPE"] == "10")
                                        {
                                            string[] D2 = new string[500];
                                            D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                            //var AMBIENT_TEMPERATURE_NULLABLE = D2[1] == "" ? "NULL" : "'" + D2[1] + "'";
                                            //var FUEL_TYPE = D2[4] == " " ? "NULL" : "'" + D2[4] + "'";
                                            var res = Fn.Exec(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_DETAIL
						                (WP_GC_HOURLY_DATA_HEADER_ID_FK, TARGET_HOUR, HEAD, INFLOW, AVAILABILITY, REMARKS)
                                        VALUES('" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + @"','" + D2[0] + @"'," + D2[1] + @",'" + D2[2] + @"','" + D2[3] + @"','" + D2[4] + @"')");

                                            if (res != "1")
                                            {
                                                throw new InvalidOperationException(res);
                                            }
                                        }
                                        else
                                        {
                                            string[] D2 = new string[500];
                                            D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                            var AMBIENT_TEMPERATURE_NULLABLE = D2[1] == "" ? "NULL" : "'" + D2[1] + "'";
                                            var FUEL_TYPE = D2[5] == " " ? "NULL" : "'" + D2[5] + "'";
                                            var SCH_10_ID = D2[6] == " " || D2[6] == "undefined" ? "NULL" : "'" + D2[6] + "'";
                                            var AMBIENT_AVAIL_NULLABLE = D2[2] == "" ? "NULL" : "'" + D2[2] + "'";

                                            var res = Fn.Exec(@"INSERT INTO CDXP.WP_GC_HOURLY_DATA_DETAIL
                         (WP_GC_HOURLY_DATA_HEADER_ID_FK, TARGET_HOUR, AMBIENT_TEMPERATURE, AMBIENT_AVAILABILITY, AVAILABILITY, LAST_UPDATED, FUEL_TYPE, REMARKS, SCH_10_ID)
                            VALUES('" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + @"','" + D2[0] + @"'," + AMBIENT_TEMPERATURE_NULLABLE + @"," + AMBIENT_AVAIL_NULLABLE + @",'" + D2[3] + @"',CDXP.GETDATEpk()," + FUEL_TYPE + @",'" + D2[4] + @"'," + SCH_10_ID + @")");

                                            if (res != "1")
                                            {
                                                throw new InvalidOperationException(res);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK='" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + "'");
                                    Fn.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_DETAIL WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK='" + WP_GC_HOURLY_DATA_DETAIL_ID_PK + "'");
                                    return Convert.ToString("ERROR_WHILE_INSERTION½: Exception= " + ex);
                                }

                                Returnvls = "1¼" + WP_GC_HOURLY_DATA_DETAIL_ID_PK;


                            }
                        }
                        else //update (not implemented)
                        {
                            if (dataID[3] == "ADAC")
                            {
                                Fn.Exec(@"UPDATE       CDXP.WP_GC_HOURLY_DATA_HEADER
                            SET  STATUS ='" + dataID[2] + @"', HOURLY_DATA_TYPE ='" + dataID[3] + @"', VENDOR_SITE_ID_FK ='" + dataID[4] + @"', POWER_POLICIY ='" + dataID[5] + @"', TARGET_DATE ='" + dataID[6] + @"', INTIMATION_DATE_TIME ='" + dataID[7] + @"', SENDER_NAME ='" + dataID[8] + @"', SENDER_DESIGNATION ='" + dataID[9] + @"', SENDER_REMARKS ='" + dataID[10] + @"', PLT_BLK_FUEL_ID ='" + dataID[11] + @"'
                        , LAST_UPDATE_DATE =CDXP.GETDATEpk(), LAST_UPDATED_BY =[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), SUBMIT_DATE =CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END
                           WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK=" + dataID[1]);


                                var frmdata2 = HttpContext.Request.Form["dtl"];
                                foreach (var item in frmdata2.Split('│'))
                                {
                                    string[] D2 = new string[500];
                                    D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                    var AMBIENT_TEMPERATURE_NULLABLE = D2[1] == "" ? "NULL" : "'" + D2[1] + "'";
                                    Fn.Exec(@"UPDATE CDXP.WP_GC_HOURLY_DATA_DETAIL SET  AMBIENT_TEMPERATURE =" + AMBIENT_TEMPERATURE_NULLABLE + @", AMBIENT_AVAILABILITY ='" + D2[2] + @"', AVAILABILITY ='" + D2[3] + @"',LAST_UPDATED = CDXP.GETDATEpk(), FUEL_TYPE= '" + D2[5] + @"' , REMARKS ='" + D2[4] + @"' WHERE WP_GC_HOURLY_DATA_DETAIL_ID_PK = '" + D2[0] + @"'");
                                }
                            }
                            else
                            {

                                Fn.Exec(@"UPDATE       CDXP.WP_GC_HOURLY_DATA_HEADER
                            SET  STATUS ='" + dataID[2] + @"', HOURLY_DATA_TYPE ='" + dataID[3] + @"', VENDOR_SITE_ID_FK ='" + dataID[4] + @"', POWER_POLICIY ='" + dataID[5] + @"', TARGET_DATE ='" + dataID[6] + @"', INTIMATION_DATE_TIME ='" + dataID[7] + @"', SENDER_NAME ='" + dataID[8] + @"', SENDER_DESIGNATION ='" + dataID[9] + @"', SENDER_REMARKS ='" + dataID[10] + @"', PLT_BLK_FUEL_ID ='" + dataID[11] + @"'
                        , LAST_UPDATE_DATE =CDXP.GETDATEpk(), LAST_UPDATED_BY =[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), SUBMIT_DATE =CASE WHEN '" + dataID[2] + @"' = 'Submitted' then CDXP.GETDATEpk() ELSE NULL END
                           WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK=" + dataID[1]);


                                var frmdata2 = HttpContext.Request.Form["dtl"];
                                foreach (var item in frmdata2.Split('│'))
                                {
                                    string[] D2 = new string[500];
                                    D2 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                    var AMBIENT_TEMPERATURE_NULLABLE = D2[1] == "" ? "NULL" : "'" + D2[1] + "'";
                                    Fn.Exec(@"UPDATE CDXP.WP_GC_HOURLY_DATA_DETAIL SET  AMBIENT_TEMPERATURE =" + AMBIENT_TEMPERATURE_NULLABLE + @", AMBIENT_AVAILABILITY ='" + D2[2] + @"', AVAILABILITY ='" + D2[3] + @"',LAST_UPDATED = CDXP.GETDATEpk(), FUEL_TYPE= '" + D2[5] + @"' , REMARKS ='" + D2[4] + @"' WHERE WP_GC_HOURLY_DATA_DETAIL_ID_PK = '" + D2[0] + @"'");
                                }

                                Returnvls = dataID[1];
                            }


                        }
                        break;
                    case 100:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            Returnvls = Fn.Data2DropdownSQL(@" SELECT DISTINCT CDXP.AP_SUPPLIERS.VENDOR_ID AS id, VENDOR_NAME FROM CDXP.AP_SUPPLIERS INNER JOIN CDXP.PPA_HEADER INNER JOIN CDXP.CPPA_PPA_PLT_BLK_FUEL
                            ON PPA_HEADER.HEADER_ID_PK = CPPA_PPA_PLT_BLK_FUEL.HEADER_ID_FK ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.PPA_HEADER.VENDOR_ID_FK

                            INNER JOIN
                            CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
                            CDXP.PPA_HEADER.VENDOR_SITE_ID_FK = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID
                            WHERE CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE != 'Wind' AND CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE != 'Solar' AND PPA_HEADER.IPP_CATEGORY = " + dataID[1] + " AND (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"') ORDER BY VENDOR_NAME");
                        }
                        break;
                    case 101:
                        Returnvls = Fn.Data2DropdownSQL(@"EXEC Sync_Desync_DemandType_Units " + dataID[1] + "," + dataID[2] + ",'" + dataID[3] + "'");
                        break;
                    case 102:
                        Returnvls = Fn.Data2Json(@"EXEC getDiffEventTime_Unit " + dataID[1] + "," + dataID[2] + ",'" + dataID[3] + "','" + dataID[4] + "','" + dataID[5] + "'");
                        break;
                    case 103:
                        //Returnvls = Fn.Data2Json(@"SELECT b.STATE_NAME,a.NTS FROM DI_CLASSIFICATION_STARTS a INNER JOIN DBO.DI_CLASSIFICATION_STATE b
                        //  ON a.DI_CLASSIFICATION_STATE_ID_FK=b.DI_CLASSIFICATION_STATE_ID WHERE '" + dataID[5] + "' BETWEEN a.GREATER_THAN_AND_EQUAL_TO_HOUR_PRESENT_STATE AND a.LESS_THAN_HOUR_PRESENT_STATE AND PLT_BLK_FUEL_ID_FK='" + dataID[3] + "'");
                        Returnvls = Fn.Data2Json(@"EXEC NTS_STATE_NTSTIME " + dataID[5] + "," + dataID[3] + " ,'" + dataID[6] + "'");
                        break;


                    case 104:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            Returnvls = Fn.Data2DropdownSQL(@" SELECT DISTINCT CDXP.AP_SUPPLIERS.VENDOR_ID AS id, VENDOR_NAME FROM CDXP.AP_SUPPLIERS INNER JOIN CDXP.PPA_HEADER INNER JOIN CDXP.CPPA_PPA_PLT_BLK_FUEL
                            ON PPA_HEADER.HEADER_ID_PK = CPPA_PPA_PLT_BLK_FUEL.HEADER_ID_FK ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.PPA_HEADER.VENDOR_ID_FK
                            INNER JOIN
                            CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
                            CDXP.PPA_HEADER.VENDOR_SITE_ID_FK = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID
                            WHERE  PPA_HEADER.IPP_CATEGORY = " + dataID[1] + " AND (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"') ORDER BY VENDOR_NAME");
                        }
                        break;
                    case 105:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            String Ramprate = Fn.Data2Json(@"Exec get_ramp_rate " + dataID[1] + "," + dataID[2] + "," + dataID[3] + "," + dataID[4] + "," + dataID[5] + "");
                            var jToken = JToken.Parse(Ramprate);
                            var users = jToken.ToObject<List<dynamic>>(); //Converts the Json to a List<Usermodel>
                            var user = users[0];

                            //JContainer is the base class
                            var jObj = (JObject)user;
                            string rampmin = String.Empty;
                            foreach (JToken token in jObj.Children())
                            {
                                if (token is JProperty)
                                {
                                    var prop = token as JProperty;
                                    rampmin = prop.Value.ToString();
                                }
                            }

                            Returnvls = Fn.Data2Json(@"Exec LoadAcievementTimeConversion " + rampmin + ", '" + dataID[6] + "'," + dataID[7] + "");

                        }
                        break;

                    case 150:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            String Ramprate = Fn.Data2Json(@"Exec get_ramp_rate " + dataID[1] + "," + dataID[2] + "," + dataID[3] + "," + dataID[4] + "," + dataID[5] + "");
                            var jToken = JToken.Parse(Ramprate);
                            var users = jToken.ToObject<List<dynamic>>(); //Converts the Json to a List<Usermodel>
                            var user = users[0];

                            //JContainer is the base class
                            var jObj = (JObject)user;
                            string rampmin = String.Empty;
                            foreach (JToken token in jObj.Children())
                            {
                                if (token is JProperty)
                                {
                                    var prop = token as JProperty;
                                    rampmin = prop.Value.ToString();
                                }
                            }

                            Returnvls = Fn.Data2Json(@"Exec LoadAcievementTimeConversionDesync " + rampmin + ", '" + dataID[6] + "'");

                        }
                        break;
                    case 151:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            String Ramprate1 = Fn.Data2Json(@"Exec get_ramp_rate " + dataID[1] + "," + dataID[2] + "," + dataID[3] + "," + dataID[4] + "," + dataID[5] + "");
                            var jToken = JToken.Parse(Ramprate1);
                            var users = jToken.ToObject<List<dynamic>>(); //Converts the Json to a List<Usermodel>
                            var user = users[0];

                            //JContainer is the base class
                            var jObj = (JObject)user;
                            string rampmin = String.Empty;
                            foreach (JToken token in jObj.Children())
                            {
                                if (token is JProperty)
                                {
                                    var prop = token as JProperty;
                                    rampmin = prop.Value.ToString();
                                }
                            }

                            String Ramprate2 = Fn.Data2Json(@"Exec get_ramp_rate " + dataID[1] + "," + dataID[2] + "," + dataID[3] + "," + dataID[4] + "," + dataID[6] + "");
                            var jToken1 = JToken.Parse(Ramprate2);
                            var users1 = jToken1.ToObject<List<dynamic>>(); //Converts the Json to a List<Usermodel>
                            var user1 = users1[0];

                            //JContainer is the base class
                            var jObj1 = (JObject)user1;
                            string rampmin1 = String.Empty;
                            foreach (JToken token in jObj1.Children())
                            {
                                if (token is JProperty)
                                {
                                    var prop = token as JProperty;
                                    rampmin1 = prop.Value.ToString();
                                }
                            }


                            Returnvls = Fn.Data2Json(@"Exec LoadAcievementTimeConversionIDL " + rampmin + "," + rampmin1 + ", '" + dataID[7] + "'");

                        }
                        break;

                    case 106:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            Returnvls = Fn.Data2Json(@"SELECT distinct MINIMUM_LOAD_LIMIT AS min_load FROM DI_CLASSIFICATION_STARTS WHERE PLT_BLK_FUEL_ID_FK=" + dataID[1] + "");
                        }
                        break;
                    case 107:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            double VENDOR_ID_FK0 = Convert.ToDouble(dataID[1]);
                            double VENDOR_SITE_ID_FK0 = Convert.ToDouble(dataID[2]);

                            Returnvls = Fn.Data2DropdownSQL(@"SELECT CAST(CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID AS VARCHAR(100)) + '½' + CAST(CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS VARCHAR(100)) 
                             + '½' + CAST(CDXP.PPA_HEADER.CONTRACTED_CAPACITY AS VARCHAR(100)) + '½' + CAST(CDXP.PPA_HEADER.POWER_POLICY AS VARCHAR(100)) 
                             + '½' + CAST(CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS VARCHAR(100)) AS id, CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE + ' ( ' + ISNULL(CDXP.CPPA_PPA_PLT_BLK_FUEL.ATTRIBUTE2, 
                             '--') + ' ) ' + CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS Expr1
                             FROM            CDXP.CPPA_PPA_PLT_BLK_FUEL INNER JOIN
                             CDXP.PPA_HEADER ON CDXP.CPPA_PPA_PLT_BLK_FUEL.HEADER_ID_FK = CDXP.PPA_HEADER.HEADER_ID_PK   
                                WHERE PPA_HEADER.APPROVAL_STATUS= 'Approved' AND PPA_HEADER.VENDOR_ID_FK=" + VENDOR_ID_FK0 + @" AND VENDOR_SITE_ID_FK=" + VENDOR_SITE_ID_FK0 + @" 
                             order by case when CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE='Complex' then null else  CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE end asc");
                            //Returnvls = Fn.Data2DropdownSQL(@"SELECT  CAST(CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID AS VARCHAR(100)) AS id, CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE + ' ( ' + ISNULL(CDXP.CPPA_PPA_PLT_BLK_FUEL.ATTRIBUTE2, 
                            //             '--') + ' ) ' + CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS Expr1 ,CAST(CDXP.PPA_HEADER.POWER_POLICY AS VARCHAR(100)) AS PP ,CAST(CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS VARCHAR(100)) AS fuel
                            //             FROM            CDXP.CPPA_PPA_PLT_BLK_FUEL INNER JOIN
                            //             CDXP.PPA_HEADER ON CDXP.CPPA_PPA_PLT_BLK_FUEL.HEADER_ID_FK = CDXP.PPA_HEADER.HEADER_ID_PK   
                            //            WHERE PPA_HEADER.APPROVAL_STATUS= 'Approved' AND PPA_HEADER.VENDOR_ID_FK=" + VENDOR_ID_FK0 + @" AND VENDOR_SITE_ID_FK=" + VENDOR_SITE_ID_FK0 + @" 
                            //              AND cdxp.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE !='Complex'");
                        }
                        break;
                    case 109:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            Returnvls = Fn.Data2Json(@"EXEC Sync_DI_UnitsCount_IDLOAD " + dataID[1] + "," + dataID[2] + "");
                        }
                        break;

                    case 110:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            Returnvls = Fn.Data2Json(@"SELECT Dispatch_Instruction_Category as DI_ID FROM CDXP.PPA_HEADER WHERE VENDOR_ID_FK=" + dataID[1] + " AND VENDOR_SITE_ID_FK=" + dataID[2] + "");
                        }
                        break;
                    case 111:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            Returnvls = Fn.Data2Json(@"SELECT DISTINCT FULL_LOAD_TIME FROM DI_CLASSIFICATION_STARTS WHERE PLT_BLK_FUEL_ID_FK='" + dataID[1] + "' AND PRESENT_STATE='" + dataID[2] + "' ");
                        }
                        break;
                    case 112:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            Returnvls = Fn.Data2Json(@"SELECT DISTINCT FULL_LOAD_TIME,FULL_LOAD_TIME_WITH_ST FROM DI_CLASSIFICATION_STARTS WHERE PLT_BLK_FUEL_ID_FK='" + dataID[1] + "' AND PRESENT_STATE='" + dataID[2] + "' ");
                        }
                        break;
                    case 113:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            Returnvls = Fn.Data2DropdownSQL(@" (SELECT  PLT_BLK_FUEL_ID,block FROM CDXP.WP_NPCC_PLANT_EVENTS WHERE TYPE_OF_OUTAGE='Standby' AND WP_NPCC_PLANT_EVENTS_ID_PK IN ( 
                              SELECT WP_NPCC_PLANT_EVENTS_ID_PK FROM(SELECT WP_NPCC_PLANT_EVENTS_ID_PK, PLT_BLK_FUEL_ID,block ,INTIMATION_TIME AS D,event, 
                                ROW_NUMBER() OVER (partition BY PLT_BLK_FUEL_ID ORDER BY INTIMATION_TIME desc) AS RowNum FROM CDXP.WP_NPCC_PLANT_EVENTS WHERE VENDOR_ID='" + dataID[1] + "' AND SETUP_SITE_ID_FK='" + dataID[2] + "') AS t1 WHERE RowNum=1))UNION (SELECT PLT_BLK_FUEL_ID,cppbf.BLOCK_UNIT_TITLE FROM CDXP.CPPA_PPA_PLT_BLK_FUEL cppbf INNER JOIN CDXP.PPA_HEADER ph ON cppbf.HEADER_ID_FK = ph.HEADER_ID_PK  WHERE ph.VENDOR_ID_FK='" + dataID[1] + "' AND ph.VENDOR_SITE_ID_FK='" + dataID[2] + "' AND cppbf.BLOCK_UNIT_TITLE='Complex') ");
                        }
                        break;
                    case 114:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            Returnvls = Fn.Data2Json(@"SELECT CAPACITY FROM cdxp.CPPA_PPA_PLT_BLK_FUEL  WHERE PLT_BLK_FUEL_ID='" + dataID[1] + "'");
                        }
                        break;
                    case 115:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            Returnvls = Fn.Data2Json(@"EXEC Sync_DI_UnitsCount_IDLOAD_Cat2 " + dataID[1] + "," + dataID[2] + "");
                        }
                        break;
                    case 116:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            Returnvls = Fn.Data2Json(@"EXEC getDiffEventTime_4Complex_Cat2 '" + dataID[1] + "','" + dataID[2] + "','" + dataID[3] + "'");
                        }
                        break;


                    case 117:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            var Status = dataID[2];
                            var recordId = dataID[1];

                            Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                            [STATUS] = '" + Status + @"',
                                             GC_ACK_BY='" + dataID[3] + @"',
                                      GC_ACK_DESIGNATION ='" + dataID[4] + @"',
                                          GC_ACK_REMARKS ='" + dataID[5] + @"',
                                            GC_ACK_TIME='" + serverDate + @"'

                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");
                        }
                        break;
                    case 119:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            var Status = dataID[2];
                            var recordId = dataID[1];

                            if (dataID[3] == "Sync" || dataID[3] == "Desync")
                            {

                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                            [STATUS] = '" + Status + @"',
                                             GC_COMP_BY='" + dataID[4] + @"',
                                             GC_COMP_DESIGNATION ='" + dataID[5] + @"',
                                             GC_COMP_REMARKS ='" + dataID[6] + @"' ,                              
                                      
                                             GC_COMP_TIME='" + serverDate + @"' 
                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");


                                var mltRcd = HttpContext.Request.Form["nxt"];
                                //new edition code..
                                if (mltRcd != "")
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_DETAIL] SET  GC_COMP_DESYNC_SYNC_DATE_TIME='" + item.Split('½')[1] + @"', STATUS='Full Compliance' WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "' and BLOCK_ID ='" + item.Split('½')[0] + @"'");
                                        }
                                    }
                                }


                            }
                            if (dataID[3] == "Increase/Decrease Load")
                            {

                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                            [STATUS] = '" + Status + @"',
                                             GC_COMP_BY='" + dataID[6] + @"',
                                             GC_COMP_DESIGNATION ='" + dataID[7] + @"',
                                             GC_COMP_REMARKS ='" + dataID[8] + @"' ,                                                               
                                             GC_COMP_TIME='" + serverDate + @"',
                                                GC_COMP_TARGET_ACHIEVED='" + dataID[4] + @"',
                                              GC_COMP_ACHIEVE_DATE_TIME='" + dataID[5] + @"'
                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");

                            }

                        }
                        break;

                    case 120:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())

                        {
                            DateTime target_sync_time = Convert.ToDateTime(dataID[14]);
                            DateTime actual_sync_time = Convert.ToDateTime(dataID[15]);
                            DateTime target_achievement_time = Convert.ToDateTime(dataID[16]);
                            DateTime actual_demand_meet_time = Convert.ToDateTime(dataID[17]);

                            int actual_demand_meet = Convert.ToInt16(dataID[18]);
                            int perLessTarget_load = Convert.ToInt16(dataID[19]);
                            int perGreaterTarget_load = Convert.ToInt16(dataID[20]);


                            DateTime ServerDateTime = pkdatetime.AddMinutes(1);
                            string serverDateT = ServerDateTime.ToString("dd-MMM-yyyy HH:mm");

                            if (target_sync_time >= actual_sync_time)
                            {
                                Fn.Exec(@"EXEC SyncForcedOutageSyncEventDI '" + dataID[1] + "','" + dataID[2] + "','" + dataID[3] + "'," +
                                "'" + dataID[4] + "','" + dataID[5] + "','" + dataID[6] + "','" + dataID[7] + "','" + dataID[8] + "','" + dataID[9] + "'," +
                                "'" + dataID[10] + "','" + dataID[11] + "','" + dataID[12] + @"' ,'" + dataID[13] + @"' ");

                            }
                            else
                            {
                                Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_PLANT_EVENTS (STATUS, SETUP_SITE_ID_FK, VENDOR_ID, GENERATION_COMPANY,
                                SITE, BLOCK, PLT_BLK_FUEL_ID, FUEL, EVENT, TYPE_OF_OUTAGE, REASON, EVENT_TIME, INTIMATION_TIME, SENDER_NAME, SENDER_DESIGNATION, REMARKS, IPP_CATEGORY, CREATED_BY, CREATED_ON) 
                                VALUES ('" + dataID[1] + "','" + dataID[5] + "','" + dataID[3] + "','" + dataID[4] + "','" + dataID[6] + "','" + dataID[9] + "','" + dataID[8] + "','" + dataID[7] + "','Desync','Forced Outage','Late Sync','" + dataID[13] + @"' ,'" + serverDate + "','ADMIN NPCC','(NPCC)','','" + dataID[2] + "','System Generated','" + serverDate + "')");

                                Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_PLANT_EVENTS (STATUS, SETUP_SITE_ID_FK, VENDOR_ID, GENERATION_COMPANY,
                                SITE, BLOCK, PLT_BLK_FUEL_ID, FUEL, EVENT, TYPE_OF_OUTAGE, REASON, EVENT_TIME, INTIMATION_TIME, SENDER_NAME, SENDER_DESIGNATION, REMARKS, IPP_CATEGORY, CREATED_BY, CREATED_ON) 
                                VALUES ('" + dataID[1] + "','" + dataID[5] + "','" + dataID[3] + "','" + dataID[4] + "','" + dataID[6] + "','" + dataID[9] + "','" + dataID[8] + "','" + dataID[7] + "','Sync','','','" + dataID[12] + @"','" + serverDateT + "','System Generated','System','','" + dataID[2] + "','System Generated','" + serverDateT + "')");
                                Fn.Exec(@"UPDATE CDXP.WP_NPCC_DESPATCH_HEADER SET STATUS='Partial Compliance' where  WP_NPCC_DESPATCH_HEADER_ID='" + dataID[11] + "'");

                            }


                            if ((target_sync_time >= actual_sync_time) && (target_achievement_time >= actual_demand_meet_time) && ((actual_demand_meet >= perLessTarget_load) && (actual_demand_meet <= perGreaterTarget_load)))
                            {
                                Fn.Exec(@"UPDATE CDXP.WP_NPCC_DESPATCH_HEADER SET STATUS='Full Compliance' where  WP_NPCC_DESPATCH_HEADER_ID='" + dataID[11] + "'");

                                Returnvls = "Full Compliance";
                            }
                            else
                            {

                                Fn.Exec(@"UPDATE CDXP.WP_NPCC_DESPATCH_HEADER SET STATUS='Partial Compliance' where  WP_NPCC_DESPATCH_HEADER_ID='" + dataID[11] + "'");
                                Returnvls = "Partial Compliance";

                            }

                            var Status = dataID[23];
                            var recordId = dataID[22];

                            if (dataID[24] == "Sync")
                            {

                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                             GC_COMP_BY='" + dataID[25] + @"',
                                             GC_COMP_DESIGNATION ='" + dataID[26] + @"',
                                             GC_COMP_REMARKS ='" + dataID[27] + @"' ,                              
                                      
                                             GC_COMP_TIME='" + serverDate + @"' 
                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");


                                var mltRcd = HttpContext.Request.Form["nxt"];
                                //new edition code..
                                if (mltRcd != "")
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_DETAIL] SET  GC_COMP_DESYNC_SYNC_DATE_TIME='" + item.Split('½')[1] + @"',GC_COMP_ACHIEVE_DATE_TIME='" + item.Split('½')[2] + @"',GC_COMP_TARGET_ACHIEVED='" + item.Split('½')[3] + @"', STATUS='" + Returnvls + "' WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "' and BLOCK_ID ='" + item.Split('½')[0] + @"'");
                                        }
                                    }
                                }


                            }
                            if (dataID[24] == "Desync")
                            {

                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                             GC_COMP_BY='" + dataID[25] + @"',
                                             GC_COMP_DESIGNATION ='" + dataID[26] + @"',
                                             GC_COMP_REMARKS ='" + dataID[27] + @"' ,                              
                                      
                                             GC_COMP_TIME='" + serverDate + @"' 
                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");


                                var mltRcd = HttpContext.Request.Form["nxt"];
                                //new edition code..
                                if (mltRcd != "")
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_DETAIL] SET  GC_COMP_DESYNC_SYNC_DATE_TIME='" + item.Split('½')[1] + @"', STATUS='" + Returnvls + "' WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "' and BLOCK_ID ='" + item.Split('½')[0] + @"'");
                                        }
                                    }
                                }


                            }
                            if (dataID[24] == "Increase/Decrease Load")
                            {

                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                             GC_COMP_BY='" + dataID[6] + @"',
                                             GC_COMP_DESIGNATION ='" + dataID[7] + @"',
                                             GC_COMP_REMARKS ='" + dataID[8] + @"' ,                                                               
                                             GC_COMP_TIME='" + serverDate + @"',
                                                GC_COMP_TARGET_ACHIEVED='" + dataID[4] + @"',
                                              GC_COMP_ACHIEVE_DATE_TIME='" + dataID[5] + @"'
                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");

                            }



                        }

                        break;

                    case 118:

                        if (dataID[5] == "Sync")
                        {
                            Returnvls = Fn.HTMLTableWithID_TR_Tag(@"Select DI_Classification_Starts_ID,'' [Sr#],(SELECT TRIM('Complex' FROM block)) [Unit no.],
                            CLASSIFICATION_FORMULA [Length of Shut Down], PRESENT_STATE AS [State],NTS AS [NTS (min)],RAMP_RATE AS [Ramp Rate (MW/Min)], GREATER_THAN_AND_EQUAL_TO_RAMP_RATE_RANGE AS [From (MW)], LESS_THAN_RAMP_RATE AS [To (MW)]
                            from [dbo].[DI_CLASSIFICATION_STARTS] where VENDOR_SITE_ID_FK ='" + dataID[1] + @"' and PLT_BLK_FUEL_ID_FK ='" + dataID[3] + @"'
                             order by NTS", "tblTechnicalLimit");
                        }
                        if (dataID[5] == "Desync")
                        {
                            Returnvls = Fn.HTMLTableWithID_TR_Tag(@"Select DI_Classification_Starts_ID,'' [Sr#],(SELECT TRIM('Complex' FROM block)) [Unit no.],
                            CLASSIFICATION_FORMULA [Length of Shut Down], PRESENT_STATE AS [State],NTS,RAMP_RATE AS [Ramp Rate (MW/Min)], GREATER_THAN_AND_EQUAL_TO_RAMP_RATE_RANGE AS [From (MW)], LESS_THAN_RAMP_RATE AS [To (MW)]
                            from [dbo].[DI_CLASSIFICATION_STARTS] where VENDOR_SITE_ID_FK ='" + dataID[1] + @"' and PLT_BLK_FUEL_ID_FK ='" + dataID[2] + @"'
                             order by NTS", "tblTechnicalLimit");
                        }
                        else if (dataID[5] == "Increase/Decrease Load")
                        {
                            Returnvls = Fn.HTMLTableWithID_TR_Tag(@"Select DI_Classification_Starts_ID,'' [Sr#],(SELECT TRIM('Complex' FROM block)) [Unit no.], 
                            CLASSIFICATION_FORMULA [Length of Shut Down], PRESENT_STATE AS [State],NTS [NTS (min)],RAMP_RATE [Ramp Rate (MW/Min)], GREATER_THAN_AND_EQUAL_TO_RAMP_RATE_RANGE [From (MW)], LESS_THAN_RAMP_RATE AS [To (MW)]
                            from [dbo].[DI_CLASSIFICATION_STARTS] where VENDOR_SITE_ID_FK ='" + dataID[1] + @"' and PLT_BLK_FUEL_ID_FK ='" + dataID[4] + @"'
                             order by NTS", "tblTechnicalLimit");
                        }
                        break;

                    case 121:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            DateTime actual_sync_time = Convert.ToDateTime(dataID[12]);
                            Fn.Exec(@"EXEC SyncForcedOutageSyncEventDI '" + dataID[1] + "','" + dataID[2] + "','" + dataID[3] + "'," +
                                "'" + dataID[4] + "','" + dataID[5] + "','" + dataID[6] + "','" + dataID[7] + "','" + dataID[8] + "','" + dataID[9] + "'," +
                                "'" + dataID[10] + "','" + dataID[11] + "','" + dataID[12] + @"' ,'" + dataID[12] + @"' ");
                            Returnvls = "Full Compliance";

                            var Status = dataID[15];
                            var recordId = dataID[14];

                            if (dataID[16] == "Sync" || dataID[16] == "Desync")
                            {

                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                             GC_COMP_BY='" + dataID[17] + @"',
                                             GC_COMP_DESIGNATION ='" + dataID[18] + @"',
                                             GC_COMP_REMARKS ='" + dataID[19] + @"' ,                              
                                      
                                             GC_COMP_TIME='" + serverDate + @"' 
                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");


                                var mltRcd = HttpContext.Request.Form["nxt"];
                                //new edition code..
                                if (mltRcd != "")
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@"UPDATE[CDXP].[WP_NPCC_DESPATCH_DETAIL] SET  GC_COMP_DESYNC_SYNC_DATE_TIME='" + item.Split('½')[1] + @"' , STATUS='" + Returnvls + "' WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "' and BLOCK_ID ='" + item.Split('½')[0] + @"'");
                                        }
                                    }
                                }


                            }
                            if (dataID[16] == "Increase/Decrease Load")
                            {

                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                             GC_COMP_BY='" + dataID[17] + @"',
                                             GC_COMP_DESIGNATION ='" + dataID[18] + @"',
                                             GC_COMP_REMARKS ='" + dataID[19] + @"' ,                                                               
                                             GC_COMP_TIME='" + serverDate + @"',
                                                GC_COMP_TARGET_ACHIEVED='" + dataID[4] + @"',
                                              GC_COMP_ACHIEVE_DATE_TIME='" + dataID[5] + @"'
                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");

                            }


                            //Returnvls = Fn.Exec(@"UPDATE CDXP.WP_NPCC_DESPATCH_HEADER SET STATUS='" + dataID[1] + @"' where  WP_NPCC_DESPATCH_HEADER_ID='" + dataID[2] + @"'");
                        }


                        break;
                    case 122:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            DateTime ServerDateTime = pkdatetime.AddMinutes(1);
                            string serverDateT = ServerDateTime.ToString("dd-MMM-yyyy HH:mm");

                            Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_PLANT_EVENTS (STATUS, SETUP_SITE_ID_FK, VENDOR_ID, GENERATION_COMPANY,
                                SITE, BLOCK, PLT_BLK_FUEL_ID, FUEL, EVENT, TYPE_OF_OUTAGE, REASON, EVENT_TIME, INTIMATION_TIME, SENDER_NAME, SENDER_DESIGNATION, REMARKS, IPP_CATEGORY, CREATED_BY, CREATED_ON) 
                                VALUES ('" + dataID[1] + "','" + dataID[5] + "','" + dataID[3] + "','" + dataID[4] + "','" + dataID[6] + "','" + dataID[9] + "','" + dataID[8] + "','" + dataID[7] + "','Desync','Forced Outage','Late Sync',CAST('" + dataID[13] + @"' AS DATETIME),'" + serverDate + "','System Generated','System','','" + dataID[2] + "','System Generated','" + serverDate + "')");



                            Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_PLANT_EVENTS (STATUS, SETUP_SITE_ID_FK, VENDOR_ID, GENERATION_COMPANY,
                                SITE, BLOCK, PLT_BLK_FUEL_ID, FUEL, EVENT, TYPE_OF_OUTAGE, REASON, EVENT_TIME, INTIMATION_TIME, SENDER_NAME, SENDER_DESIGNATION, REMARKS, IPP_CATEGORY, CREATED_BY, CREATED_ON) 
                                VALUES ('" + dataID[1] + "','" + dataID[5] + "','" + dataID[3] + "','" + dataID[4] + "','" + dataID[6] + "','" + dataID[9] + "','" + dataID[8] + "','" + dataID[7] + "','Sync','','',CAST('" + dataID[12] + @"' AS DATETIME),'" + serverDateT + "','System Generated','System','','" + dataID[2] + "','System Generated','" + serverDateT + "')");


                            Fn.Exec(@"UPDATE CDXP.WP_NPCC_DESPATCH_HEADER SET STATUS='Partial Compliance' where  WP_NPCC_DESPATCH_HEADER_ID='" + dataID[11] + "'");
                        }
                        break;
                    case 123:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())

                        {

                            DateTime target_sync_time = Convert.ToDateTime(dataID[14]);
                            DateTime actual_sync_time = Convert.ToDateTime(dataID[12]);
                            DateTime target_achievement_time = Convert.ToDateTime(dataID[16]);
                            DateTime actual_demand_meet_time = Convert.ToDateTime(dataID[17]);

                            int actual_demand_meet = Convert.ToInt32(dataID[18]);
                            int perLessTarget_load = Convert.ToInt32(dataID[19]);
                            int perGreaterTarget_load = Convert.ToInt32(dataID[20]);


                            if ((target_achievement_time >= actual_demand_meet_time) && ((actual_demand_meet >= perLessTarget_load) && (actual_demand_meet <= perGreaterTarget_load)))
                            {
                                Fn.Exec(@"EXEC SyncForcedOutageSyncEventDI '" + dataID[1] + "','" + dataID[2] + "','" + dataID[3] + "'," +
                                "'" + dataID[4] + "','" + dataID[5] + "','" + dataID[6] + "','" + dataID[7] + "','" + dataID[8] + "','" + dataID[9] + "'," +
                                "'" + dataID[10] + "','" + dataID[11] + "','" + dataID[12] + @"' ,'" + dataID[12] + @"' ");
                                Fn.Exec(@"UPDATE CDXP.WP_NPCC_DESPATCH_HEADER SET STATUS='Full Compliance' where  WP_NPCC_DESPATCH_HEADER_ID='" + dataID[11] + "'");

                                Returnvls = "Full Compliance";
                            }
                            else
                            {

                                Fn.Exec(@"EXEC SyncForcedOutageSyncEventDI '" + dataID[1] + "','" + dataID[2] + "','" + dataID[3] + "'," +
                                "'" + dataID[4] + "','" + dataID[5] + "','" + dataID[6] + "','" + dataID[7] + "','" + dataID[8] + "','" + dataID[9] + "'," +
                                "'" + dataID[10] + "', '" + dataID[11] + "','" + dataID[12] + @"' ,'" + dataID[12] + @"' ");
                                Fn.Exec(@"UPDATE CDXP.WP_NPCC_DESPATCH_HEADER SET STATUS='Partial Compliance' where  WP_NPCC_DESPATCH_HEADER_ID='" + dataID[11] + "'");

                                Returnvls = "Partial Compliance";

                            }

                            var Status = dataID[23];
                            var recordId = dataID[22];

                            if (dataID[24] == "Sync" || dataID[24] == "Desync")
                            {

                                Fn.Exec(@"UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                             GC_COMP_BY='" + dataID[25] + @"',
                                             GC_COMP_DESIGNATION ='" + dataID[26] + @"',
                                             GC_COMP_REMARKS ='" + dataID[27] + @"' ,                              
                                      
                                             GC_COMP_TIME='" + serverDate + @"' 
                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");


                                var mltRcd = HttpContext.Request.Form["nxt"];
                                //new edition code..
                                if (mltRcd != "")
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@"UPDATE[CDXP].[WP_NPCC_DESPATCH_DETAIL] SET  GC_COMP_DESYNC_SYNC_DATE_TIME='" + item.Split('½')[1] + @"', STATUS='" + Returnvls + "' WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "' and BLOCK_ID ='" + item.Split('½')[0] + @"'");
                                        }
                                    }
                                }


                            }
                            if (dataID[24] == "Increase/Decrease Load")
                            {

                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                             GC_COMP_BY='" + dataID[6] + @"',
                                             GC_COMP_DESIGNATION ='" + dataID[7] + @"',
                                             GC_COMP_REMARKS ='" + dataID[8] + @"' ,                                                               
                                             GC_COMP_TIME='" + serverDate + @"',
                                                GC_COMP_TARGET_ACHIEVED='" + dataID[4] + @"',
                                              GC_COMP_ACHIEVE_DATE_TIME='" + dataID[5] + @"'
                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");

                            }



                        }
                        break;

                    case 124:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())

                        {

                            DateTime target_achievement_time = Convert.ToDateTime(dataID[13]);
                            DateTime actual_demand_meet_time = Convert.ToDateTime(dataID[12]);


                            if (target_achievement_time >= actual_demand_meet_time)
                            {
                                Fn.Exec(@"EXEC SyncForcedOutageSyncEventDI '" + dataID[1] + "','" + dataID[2] + "','" + dataID[3] + "'," +
                                "'" + dataID[4] + "','" + dataID[5] + "','" + dataID[6] + "','" + dataID[7] + "','" + dataID[8] + "','" + dataID[9] + "'," +
                                "'" + dataID[10] + "','" + dataID[11] + "','" + dataID[12] + @"' ,'" + dataID[12] + @"' ");
                                Returnvls = "Full Compliance";
                            }
                            else
                            {

                                Fn.Exec(@"EXEC SyncForcedOutageSyncEventDI '" + dataID[1] + "','" + dataID[2] + "','" + dataID[3] + "'," +
                                "'" + dataID[4] + "','" + dataID[5] + "','" + dataID[6] + "','" + dataID[7] + "','" + dataID[8] + "','" + dataID[9] + "','LateD','" + dataID[11] + "','" + dataID[12] + @"' ,'" + dataID[12] + @"' ");

                                Returnvls = "Partial Compliance";

                            }

                            var Status = dataID[16];
                            var recordId = dataID[15];

                            if (dataID[17] == "Sync" || dataID[17] == "Desync")
                            {

                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                             GC_COMP_BY='" + dataID[18] + @"',
                                             GC_COMP_DESIGNATION ='" + dataID[19] + @"',
                                             GC_COMP_REMARKS ='" + dataID[20] + @"' ,                              
                                      
                                             GC_COMP_TIME='" + serverDate + @"' 
                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");


                                var mltRcd = HttpContext.Request.Form["nxt"];
                                //new edition code..
                                if (mltRcd != "")
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_DETAIL] SET  GC_COMP_DESYNC_SYNC_DATE_TIME='" + item.Split('½')[1] + @"', STATUS='" + Returnvls + "' WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "' and BLOCK_ID ='" + item.Split('½')[0] + @"'");
                                        }
                                    }
                                }


                            }
                            if (dataID[17] == "Increase/Decrease Load")
                            {

                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                             GC_COMP_BY='" + dataID[18] + @"',
                                             GC_COMP_DESIGNATION ='" + dataID[19] + @"',
                                             GC_COMP_REMARKS ='" + dataID[20] + @"' ,                                                               
                                             GC_COMP_TIME='" + serverDate + @"',
                                                GC_COMP_TARGET_ACHIEVED='" + dataID[4] + @"',
                                              GC_COMP_ACHIEVE_DATE_TIME='" + dataID[5] + @"'
                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");

                            }


                        }
                        break;
                    case 125:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())

                        {
                            DateTime target_achievement_time = Convert.ToDateTime(dataID[12]);
                            DateTime actual_demand_meet_time = Convert.ToDateTime(dataID[13]);

                            int actual_demand_meet = Convert.ToInt16(dataID[14]);
                            int perLessTarget_load = Convert.ToInt16(dataID[15]);
                            int perGreaterTarget_load = Convert.ToInt16(dataID[16]);


                            if ((target_achievement_time >= actual_demand_meet_time) && ((actual_demand_meet >= perLessTarget_load) && (actual_demand_meet <= perGreaterTarget_load)))
                            {
                                Fn.Exec(@"UPDATE CDXP.WP_NPCC_DESPATCH_HEADER SET STATUS='Full Compliance' where  WP_NPCC_DESPATCH_HEADER_ID='" + dataID[11] + "'");

                                Returnvls = "Full Compliance";
                            }
                            else
                            {
                                Fn.Exec(@"UPDATE CDXP.WP_NPCC_DESPATCH_HEADER SET STATUS='Partial Compliance' where  WP_NPCC_DESPATCH_HEADER_ID='" + dataID[11] + "'");

                                Returnvls = "Partial Compliance";

                            }

                            var Status = dataID[23];
                            var recordId = dataID[22];

                            if (dataID[24] == "Increase/Decrease Load")
                            {

                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                             GC_COMP_BY='" + dataID[25] + @"',
                                             GC_COMP_DESIGNATION ='" + dataID[26] + @"',
                                             GC_COMP_REMARKS ='" + dataID[27] + @"' ,                                                               
                                             GC_COMP_TIME='" + serverDate + @"',
                                                GC_COMP_TARGET_ACHIEVED='" + dataID[14] + @"',
                                              GC_COMP_ACHIEVE_DATE_TIME='" + dataID[13] + @"'
                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");
                            }



                        }
                        break;
                    case 126:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            //DateTime actual_sync_time = Convert.ToDateTime(dataID[12]);
                            //Fn.Exec(@"EXEC SyncForcedOutageSyncEventDI '" + dataID[1] + "','" + dataID[2] + "','" + dataID[3] + "'," +
                            //    "'" + dataID[4] + "','" + dataID[5] + "','" + dataID[6] + "','" + dataID[7] + "','" + dataID[8] + "','" + dataID[9] + "'," +
                            //    "'" + dataID[10] + "','" + dataID[11] + "','" + dataID[12] + @"' ,'" + dataID[12] + @"' ");
                            //Returnvls = "Full Compliance";

                            //var recordId = dataID[11];

                            //if (dataID[10] == "Sync" || dataID[10] == "Desync")
                            //{

                            //    Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                            //                 GC_COMP_BY='" + dataID[17] + @"',
                            //                 GC_COMP_DESIGNATION ='" + dataID[18] + @"',
                            //                 GC_COMP_REMARKS ='" + dataID[19] + @"' ,                              

                            //                 GC_COMP_TIME='" + serverDate + @"' 
                            //                 WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");


                            //    var mltRcd = HttpContext.Request.Form["nxt"];
                            //    //new edition code..
                            //    if (mltRcd != "")
                            //    {
                            //        string[] mdata = new string[500];
                            //        mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                            //        foreach (var item in mdata)
                            //        {
                            //            if (item.Contains("½"))
                            //            {
                            //                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_DETAIL] SET  GC_COMP_DESYNC_SYNC_DATE_TIME='" + item.Split('½')[1] + @"' WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "' and BLOCK_ID ='" + item.Split('½')[0] + @"'");
                            //            }
                            //        }
                            //    }


                            //}
                            if (dataID[10] == "Increase/Decrease Load")
                            {
                                DateTime actual_demand_meet_time = Convert.ToDateTime(dataID[12]);

                                int actual_demand_meet = Convert.ToInt16(dataID[11]);
                                int perLessTarget_load = Convert.ToInt16(dataID[5]);
                                int perGreaterTarget_load = Convert.ToInt16(dataID[6]);

                                if ((actual_demand_meet >= perLessTarget_load) && (actual_demand_meet <= perGreaterTarget_load))
                                {
                                    Fn.Exec(@"UPDATE CDXP.WP_NPCC_DESPATCH_HEADER SET STATUS='Full Compliance' where  WP_NPCC_DESPATCH_HEADER_ID='" + dataID[8] + "'");

                                    Returnvls = "Full Compliance";
                                }
                                else
                                {
                                    Fn.Exec(@"UPDATE CDXP.WP_NPCC_DESPATCH_HEADER SET STATUS='Partial Compliance' where  WP_NPCC_DESPATCH_HEADER_ID='" + dataID[8] + "'");

                                    Returnvls = "Partial Compliance";

                                }
                                Fn.Exec(@" UPDATE [CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                             GC_COMP_BY='" + dataID[13] + @"',
                                             GC_COMP_DESIGNATION ='" + dataID[14] + @"',
                                             GC_COMP_REMARKS ='" + dataID[15] + @"' ,                                                               
                                             GC_COMP_TIME='" + serverDate + @"',
                                                GC_COMP_TARGET_ACHIEVED='" + dataID[11] + @"',
                                              GC_COMP_ACHIEVE_DATE_TIME='" + dataID[12] + @"'
                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + dataID[8] + "'");

                            }


                            //Returnvls = Fn.Exec(@"UPDATE CDXP.WP_NPCC_DESPATCH_HEADER SET STATUS='" + dataID[1] + @"' where  WP_NPCC_DESPATCH_HEADER_ID='" + dataID[2] + @"'");
                        }
                        break;
                    case 127:
                        {
                            Fn.Exec(@" UPDATE CDXP.NOTIFICATIONS SET Is_View='True' where Table_PK_ID='" + dataID[1] + @"' ");

                        }

                        break;


                    default:
                        Returnvls = " - 1";
                        break;
                }
            }
            catch (Exception ex)
            {

                return Convert.ToString(ex.Message);
            }


            return Returnvls;
        }

        [HttpPost]
        public string AjaxCallNext()
        {
            sessionCheck(); Load_Notifications();
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");

            var frmdata = HttpContext.Request.Form["vls"];
            string[] dataID = new string[500];
            dataID = Fn.CleanSQL(HttpUtility.UrlDecode(frmdata)).Split('½');
            string Returnvls = "";
            try
            {
                switch (Convert.ToInt32(dataID[0]))
                {
                    case 1001:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT [CDXP].[WP_NPCC_PLANT_EVENTS].[WP_NPCC_PLANT_EVENTS_ID_PK], '' [Sr#]
                                                ,[INTIMATION_TIME] as [Intimation Time]
                                                ,[EVENT_TIME] as [Event Time]
                                                ,[BLOCK] as [Block/Complex]
                                                ,[EVENT] as [Event]
                                                ,[TYPE_OF_OUTAGE] as [Type of Outage]
                                                ,[REASON] as [Reason]
                                                ,[REMARKS] as [Remarks]
                                                ,[NPCC_ACK] as [NPCC Acknowledgement]
                                                ,[NPCC_TYPE_OF_OUTAGE] as [NPCC Agreed Type of Outage]
                                                ,[NPCC_REASON] as [NPCC Agreed Reason]
                        FROM [CDXP].[WP_NPCC_PLANT_EVENTS] WHERE VENDOR_ID = '" + dataID[1] + @"' AND SETUP_SITE_ID_FK = '" + dataID[2] + @"' 
                        AND PLT_BLK_FUEL_ID = '" + dataID[3] + @"' AND STATUS = '" + dataID[4] + @"' AND EVENT_TIME BETWEEN '" + dataID[5] + @"' AND '" + dataID[6] + @"'
                        AND EVENT like '%" + dataID[7] + @"%' AND TYPE_OF_OUTAGE like '%" + dataID[8] + @"%'", "tblJ1");

                        break;

                    case 1002:
                        int demandType = Convert.ToInt32(dataID[6]);

                        switch (demandType)
                        {
                            case 1:
                                //Sync
                                string despatchInsSetSyncDateFrom = dataID[3].ToString();
                                string despatchInsSetSyncDateTo = dataID[4].ToString();

                                if (despatchInsSetSyncDateFrom == "" && despatchInsSetSyncDateTo == "")
                                {
                                    Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT [CDXP].[WP_NPCC_DESPATCH_HEADER].WP_NPCC_DESPATCH_HEADER_ID,'' [Sr#]
                                                , FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') as [Date]
												, FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') as [Notification Time]
                                                , [CDXP].[WP_NPCC_DESPATCH_HEADER].[TARGET_DEMAND] as [Target Demand]
												, [CDXP].[WP_NPCC_DESPATCH_HEADER].[TARGET_DATE_TIME] as [Target Time]
                                                , [CDXP].[WP_NPCC_DESPATCH_DETAIL].[LOAD_ACHIEVE_TIME] as [Achievement Time]
												, [CDXP].[WP_NPCC_DESPATCH_DETAIL].[REMARKS] as [Remarks]

                                        FROM [CDXP].[WP_NPCC_DESPATCH_HEADER]
                                        LEFT JOIN [CDXP].[WP_NPCC_DESPATCH_DETAIL]
                        ON [CDXP].[WP_NPCC_DESPATCH_DETAIL].[WP_NPCC_DESPATCH_HEADER_ID] = [CDXP].[WP_NPCC_DESPATCH_HEADER].[WP_NPCC_DESPATCH_HEADER_ID]

                        WHERE [CDXP].[WP_NPCC_DESPATCH_HEADER].[VENDOR_ID] = '" + dataID[1] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[VENDOR_SITE_ID] = '" + dataID[2] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[DEMAND_TYPE] = '" + dataID[5] + @"'", "tblJ1");
                                }
                                else
                                {
                                    Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT [CDXP].[WP_NPCC_DESPATCH_HEADER].WP_NPCC_DESPATCH_HEADER_ID,'' [Sr#]
                                                , FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') as [Date]
												, FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') as [Notification Time]
                                                , [CDXP].[WP_NPCC_DESPATCH_HEADER].[TARGET_DEMAND] as [Target Demand]
												, [CDXP].[WP_NPCC_DESPATCH_HEADER].[TARGET_DATE_TIME] as [Target Time]
                                                , [CDXP].[WP_NPCC_DESPATCH_DETAIL].[LOAD_ACHIEVE_TIME] as [Achievement Time]
												, [CDXP].[WP_NPCC_DESPATCH_DETAIL].[REMARKS] as [Remarks]

                                        FROM [CDXP].[WP_NPCC_DESPATCH_HEADER]
                                        LEFT JOIN [CDXP].[WP_NPCC_DESPATCH_DETAIL]
                        ON [CDXP].[WP_NPCC_DESPATCH_DETAIL].[WP_NPCC_DESPATCH_HEADER_ID] = [CDXP].[WP_NPCC_DESPATCH_HEADER].[WP_NPCC_DESPATCH_HEADER_ID]

                        WHERE [CDXP].[WP_NPCC_DESPATCH_HEADER].[VENDOR_ID] = '" + dataID[1] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[VENDOR_SITE_ID] = '" + dataID[2] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[DEMAND_TYPE] = '" + dataID[5] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[INTIMATION_TIME] BETWEEN '" + dataID[3] + @"' AND '" + dataID[4] + @"'", "tblJ1");
                                }

                                break;

                            case 2:
                                //Desync
                                string despatchInsSetDecDateFrom = dataID[3].ToString();
                                string despatchInsSetDecDateTo = dataID[4].ToString();

                                if (despatchInsSetDecDateFrom == "" && despatchInsSetDecDateTo == "")
                                {
                                    Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT [CDXP].[WP_NPCC_DESPATCH_HEADER].WP_NPCC_DESPATCH_HEADER_ID,'' [Sr#]
                                                , FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') as [Date]
												, FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') as [Notification Time]
                                                , [CDXP].[WP_NPCC_DESPATCH_HEADER].[TARGET_DEMAND] as [Target Demand]
												, [CDXP].[WP_NPCC_DESPATCH_HEADER].[TARGET_DATE_TIME] as [Target Time]
                                                , [CDXP].[WP_NPCC_DESPATCH_DETAIL].[LOAD_ACHIEVE_TIME] as [Achievement Time]
												, [CDXP].[WP_NPCC_DESPATCH_DETAIL].[REMARKS] as [Remarks]

                                        FROM [CDXP].[WP_NPCC_DESPATCH_HEADER]
                                        LEFT JOIN [CDXP].[WP_NPCC_DESPATCH_DETAIL]
                        ON [CDXP].[WP_NPCC_DESPATCH_DETAIL].[WP_NPCC_DESPATCH_HEADER_ID] = [CDXP].[WP_NPCC_DESPATCH_HEADER].[WP_NPCC_DESPATCH_HEADER_ID]

                        WHERE [CDXP].[WP_NPCC_DESPATCH_HEADER].[VENDOR_ID] = '" + dataID[1] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[VENDOR_SITE_ID] = '" + dataID[2] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[DEMAND_TYPE] = '" + dataID[5] + @"'", "tblJ1");
                                }
                                else
                                {
                                    Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT [CDXP].[WP_NPCC_DESPATCH_HEADER].WP_NPCC_DESPATCH_HEADER_ID,'' [Sr#]
                                                , FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') as [Date]
												, FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') as [Notification Time]
                                                , [CDXP].[WP_NPCC_DESPATCH_HEADER].[TARGET_DEMAND] as [Target Demand]
												, [CDXP].[WP_NPCC_DESPATCH_HEADER].[TARGET_DATE_TIME] as [Target Time]
                                                , [CDXP].[WP_NPCC_DESPATCH_DETAIL].[LOAD_ACHIEVE_TIME] as [Achievement Time]
												, [CDXP].[WP_NPCC_DESPATCH_DETAIL].[REMARKS] as [Remarks]

                                        FROM [CDXP].[WP_NPCC_DESPATCH_HEADER]
                                        LEFT JOIN [CDXP].[WP_NPCC_DESPATCH_DETAIL]
                        ON [CDXP].[WP_NPCC_DESPATCH_DETAIL].[WP_NPCC_DESPATCH_HEADER_ID] = [CDXP].[WP_NPCC_DESPATCH_HEADER].[WP_NPCC_DESPATCH_HEADER_ID]

                        WHERE [CDXP].[WP_NPCC_DESPATCH_HEADER].[VENDOR_ID] = '" + dataID[1] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[VENDOR_SITE_ID] = '" + dataID[2] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[DEMAND_TYPE] = '" + dataID[5] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[INTIMATION_TIME] BETWEEN '" + dataID[3] + @"' AND '" + dataID[4] + @"'", "tblJ1");
                                }

                                break;

                            case 3:
                                //Increase/Decrease Load

                                string despatchInsSetIncDecDateFrom = dataID[3].ToString();
                                string despatchInsSetIncDecDateTo = dataID[4].ToString();

                                if (despatchInsSetIncDecDateFrom == "" && despatchInsSetIncDecDateTo == "")
                                {
                                    Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT [CDXP].[WP_NPCC_DESPATCH_HEADER].WP_NPCC_DESPATCH_HEADER_ID,'' [Sr#]
                                                , FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') as [Date]
												, FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') as [Notification Time]
                                                , [CDXP].[WP_NPCC_DESPATCH_HEADER].[TARGET_DEMAND] as [Target Demand]
												, [CDXP].[WP_NPCC_DESPATCH_HEADER].[TARGET_DATE_TIME] as [Target Time]
                                                , [CDXP].[WP_NPCC_DESPATCH_DETAIL].[LOAD_ACHIEVE_TIME] as [Achievement Time]
												, [CDXP].[WP_NPCC_DESPATCH_DETAIL].[REMARKS] as [Remarks]

                                        FROM [CDXP].[WP_NPCC_DESPATCH_HEADER]
                                        LEFT JOIN [CDXP].[WP_NPCC_DESPATCH_DETAIL]
                        ON [CDXP].[WP_NPCC_DESPATCH_DETAIL].[WP_NPCC_DESPATCH_HEADER_ID] = [CDXP].[WP_NPCC_DESPATCH_HEADER].[WP_NPCC_DESPATCH_HEADER_ID]

                        WHERE [CDXP].[WP_NPCC_DESPATCH_HEADER].[VENDOR_ID] = '" + dataID[1] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[VENDOR_SITE_ID] = '" + dataID[2] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[DEMAND_TYPE] = '" + dataID[5] + @"'", "tblJ1");
                                }
                                else
                                {
                                    Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT [CDXP].[WP_NPCC_DESPATCH_HEADER].WP_NPCC_DESPATCH_HEADER_ID,'' [Sr#]
                                                , FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') as [Date]
												, FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') as [Notification Time]
                                                , [CDXP].[WP_NPCC_DESPATCH_HEADER].[TARGET_DEMAND] as [Target Demand]
												, [CDXP].[WP_NPCC_DESPATCH_HEADER].[TARGET_DATE_TIME] as [Target Time]
                                                , [CDXP].[WP_NPCC_DESPATCH_DETAIL].[LOAD_ACHIEVE_TIME] as [Achievement Time]
												, [CDXP].[WP_NPCC_DESPATCH_DETAIL].[REMARKS] as [Remarks]

                                        FROM [CDXP].[WP_NPCC_DESPATCH_HEADER]
                                        LEFT JOIN [CDXP].[WP_NPCC_DESPATCH_DETAIL]
                        ON [CDXP].[WP_NPCC_DESPATCH_DETAIL].[WP_NPCC_DESPATCH_HEADER_ID] = [CDXP].[WP_NPCC_DESPATCH_HEADER].[WP_NPCC_DESPATCH_HEADER_ID]

                        WHERE [CDXP].[WP_NPCC_DESPATCH_HEADER].[VENDOR_ID] = '" + dataID[1] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[VENDOR_SITE_ID] = '" + dataID[2] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[DEMAND_TYPE] = '" + dataID[5] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[INTIMATION_TIME] BETWEEN '" + dataID[3] + @"' AND '" + dataID[4] + @"'", "tblJ1");
                                }

                                break;

                            case 4:
                                //Emergency


                                string despatchInsSetEmergencyDateFrom = dataID[3].ToString();
                                string despatchInsSetEmergencyDateTo = dataID[4].ToString();

                                if (despatchInsSetEmergencyDateFrom == "" && despatchInsSetEmergencyDateTo == "")
                                {
                                    Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT [CDXP].[WP_NPCC_DESPATCH_HEADER].WP_NPCC_DESPATCH_HEADER_ID,'' [Sr#]
                                                , FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') as [Date]
												, FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') as [Notification Time]
                                                , [CDXP].[WP_NPCC_DESPATCH_HEADER].[TARGET_DEMAND] as [Target Demand]
												, [CDXP].[WP_NPCC_DESPATCH_HEADER].[TARGET_DATE_TIME] as [Target Time]
                                                , [CDXP].[WP_NPCC_DESPATCH_DETAIL].[LOAD_ACHIEVE_TIME] as [Achievement Time]
												, [CDXP].[WP_NPCC_DESPATCH_DETAIL].[REMARKS] as [Remarks]

                                        FROM [CDXP].[WP_NPCC_DESPATCH_HEADER]
                                        LEFT JOIN [CDXP].[WP_NPCC_DESPATCH_DETAIL]
                        ON [CDXP].[WP_NPCC_DESPATCH_DETAIL].[WP_NPCC_DESPATCH_HEADER_ID] = [CDXP].[WP_NPCC_DESPATCH_HEADER].[WP_NPCC_DESPATCH_HEADER_ID]

                        WHERE [CDXP].[WP_NPCC_DESPATCH_HEADER].[VENDOR_ID] = '" + dataID[1] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[VENDOR_SITE_ID] = '" + dataID[2] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[DEMAND_TYPE] = '" + dataID[5] + @"'", "tblJ1");
                                }
                                else
                                {
                                    Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT [CDXP].[WP_NPCC_DESPATCH_HEADER].WP_NPCC_DESPATCH_HEADER_ID,'' [Sr#]
                                                , FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') as [Date]
												, FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') as [Notification Time]
                                                , [CDXP].[WP_NPCC_DESPATCH_HEADER].[TARGET_DEMAND] as [Target Demand]
												, [CDXP].[WP_NPCC_DESPATCH_HEADER].[TARGET_DATE_TIME] as [Target Time]
                                                , [CDXP].[WP_NPCC_DESPATCH_DETAIL].[LOAD_ACHIEVE_TIME] as [Achievement Time]
												, [CDXP].[WP_NPCC_DESPATCH_DETAIL].[REMARKS] as [Remarks]

                                        FROM [CDXP].[WP_NPCC_DESPATCH_HEADER]
                                        LEFT JOIN [CDXP].[WP_NPCC_DESPATCH_DETAIL]
                        ON [CDXP].[WP_NPCC_DESPATCH_DETAIL].[WP_NPCC_DESPATCH_HEADER_ID] = [CDXP].[WP_NPCC_DESPATCH_HEADER].[WP_NPCC_DESPATCH_HEADER_ID]

                        WHERE [CDXP].[WP_NPCC_DESPATCH_HEADER].[VENDOR_ID] = '" + dataID[1] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[VENDOR_SITE_ID] = '" + dataID[2] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[DEMAND_TYPE] = '" + dataID[5] + @"'
                        AND [CDXP].[WP_NPCC_DESPATCH_HEADER].[INTIMATION_TIME] BETWEEN '" + dataID[3] + @"' AND '" + dataID[4] + @"'", "tblJ1");
                                }

                                break;

                            default:
                                break;
                        }

                        break;

                    case 0:

                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            var VENDOR_ID0 = Convert.ToInt32(dataID[1]);
                            Returnvls = Fn.Data2Dropdown(db.AP_SUPPLIER_SITE_ALL.Where(w => w.VENDOR_ID == VENDOR_ID0).Select(s => new { VENDOR_SITE_ID = s.VENDOR_SITE_ID, ADDRESS_LINE1 = s.ADDRESS_LINE1 + "-" + s.VENDOR_SITE_CODE }).OrderBy(ord => ord.ADDRESS_LINE1).ToList());
                        }

                        break;

                    case 1:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            var producerSiteId = Convert.ToInt32(dataID[2]);
                            //var ppaHeaderId = Convert.ToDecimal(db.PPA_HEADER.Where(pid => pid.VENDOR_SITE_ID_FK == producerSiteId).Select(s => s.HEADER_ID_PK).FirstOrDefault());
                            //Returnvls = Fn.Data2Dropdown(db.CPPA_PPA_PLT_BLK_FUEL.Where(w => w.HEADER_ID_FK == ppaHeaderId).Select(s => new { PLT_BLK_FUEL_ID = s.PLT_BLK_FUEL_ID + "½" + s.FUEL_LOOKUP_CODE + "½" + s.CAPACITY.ToString() + "½" + s.BLOCK_LOOKUP_CODE, BLOCK_UNIT_TITLE = s.BLOCK_UNIT_TITLE }).OrderBy(ord => ord.BLOCK_UNIT_TITLE).ToList());
                            //                     Returnvls = Fn.Data2DropdownSQL(@"SELECT 
                            //                     id = CAST(CDXP.CPPA_PPA_PLT_EMO.PLT_BLK_FUEL_ID AS VARCHAR(100)) + '½' + CAST(CDXP.CPPA_PPA_PLT_EMO.ATTRIBUTE1 AS  VARCHAR(100)) + '½' + CAST(CDXP.PPA_HEADER.CONTRACTED_CAPACITY AS VARCHAR(100)) 
                            //+ '½' + CAST(CDXP.PPA_HEADER.POWER_POLICY AS VARCHAR(100))
                            //                     , CDXP.CPPA_PPA_PLT_EMO.BLK_TITLE_FOR_EMO + ' ( ' + ISNULL(CDXP.CPPA_PPA_PLT_BLK_FUEL.ATTRIBUTE2 ,'--')+ ' ) ' + CDXP.CPPA_PPA_PLT_EMO.ATTRIBUTE1
                            //                     FROM CDXP.CPPA_PPA_PLT_EMO 
                            //                     INNER JOIN CDXP.PPA_HEADER ON  PPA_HEADER.HEADER_ID_PK= CPPA_PPA_PLT_EMO.HEADER_ID_FK
                            //                     INNER JOIN CDXP.CPPA_PPA_PLT_BLK_FUEL ON CDXP.CPPA_PPA_PLT_EMO.PLT_BLK_FUEL_ID = CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID  
                            //                     WHERE PPA_HEADER.APPROVAL_STATUS= 'Approved' AND VENDOR_SITE_ID_FK=" + producerSiteId);



                            //new change 19/10/20
                            //                            Returnvls = Fn.Data2DropdownSQL(@"SELECT        CAST(CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID AS VARCHAR(100)) + '½' + CAST(CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS VARCHAR(100)) 
                            //                         + '½' + CAST(CDXP.PPA_HEADER.CONTRACTED_CAPACITY AS VARCHAR(100)) + '½' + CAST(CDXP.PPA_HEADER.POWER_POLICY AS VARCHAR(100)) 
                            //                         + '½' + CAST(CDXP.PPA_HEADER.DEPENDABLE_CAPACITY AS VARCHAR(100)) AS id, CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE + ' ( ' + ISNULL(CDXP.CPPA_PPA_PLT_BLK_FUEL.ATTRIBUTE2, 
                            //                         '--') + ' ) ' + CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS Expr1
                            //FROM                     CDXP.CPPA_PPA_PLT_BLK_FUEL INNER JOIN 
                            //                         CDXP.PPA_HEADER ON CDXP.CPPA_PPA_PLT_BLK_FUEL.HEADER_ID_FK = CDXP.PPA_HEADER.HEADER_ID_PK   
                            //                            WHERE PPA_HEADER.APPROVAL_STATUS= 'Approved' AND  VENDOR_SITE_ID_FK=" + producerSiteId + @" 
                            //                         order by case when CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE='Complex' then null else  CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE end asc");
                            Returnvls = Fn.Data2DropdownSQL(@"EXEC LOAD_BLK_FUEL " + producerSiteId + ", '" + dataID[3] + "'");
                        }
                        break;

                    case 2:

                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            var VENDOR_ID0 = Convert.ToInt32(dataID[1]);
                            Returnvls = db.PPA_HEADER.Where(w => w.VENDOR_SITE_ID_FK == VENDOR_ID0).Select(s => s.POWER_POLICY).FirstOrDefault();
                        }

                        break;
                    case 3:
                        //for only case 3 
                        if (dataID[1] == "0")
                        {

                            if (dataID[12] == "Sync")
                            {
                                String ID03 = Fn.ExenID(@"INSERT INTO [CDXP].[WP_NPCC_DESPATCH_HEADER] (
[STATUS]
,BLOCK_ID
,VENDOR_ID
,VENDOR_SITE_ID
,FUEL
,[INTIMATION_TIME]
,[EMERGENCY_TYPE]
,[TARGET_DEMAND] 
,[TARGET_DATE_TIME]
,[SENDER_NAME]
,[SENDER_DESIGNATION]
,[NPCC_REMARKS]
,[DEMAND_TYPE]
,[PRESENT_STATE]
,[CURRENT_AVAILABILITY]
,[CREATED_BY]
,[CREATED_ON]
,[SYSTEM_CONSTRAINT]
,[SYSTEM_CONSTRAINT_REASON]
,[IPP_CAT]


)
VALUES(
'" + dataID[2] + @"',
CAST('" + dataID[3] + @"' AS int),
'" + dataID[5] + @"',
'" + dataID[6] + @"',
'" + dataID[7] + @"',
'" + dataID[8] + @"',
'" + dataID[11] + @"',
CAST('" + dataID[30] + @"' AS int),
'" + dataID[35] + @"',
'" + dataID[14] + @"',
'" + dataID[15] + @"',
'" + dataID[16] + @"',
'" + dataID[12] + @"',
'" + dataID[24] + @"',
'" + dataID[9] + @"',
'" + Convert.ToString(Session["UserName"]) + @"',
'" + dataID[8] + @"',
'" + dataID[4] + @"',
'" + dataID[11] + @"',
'" + dataID[34] + @"'

) SELECT SCOPE_IDENTITY();");

                                var mltRcd = HttpContext.Request.Form["nxt"];
                                var catgno = HttpContext.Request.Form["catg"];
                                var comp = HttpContext.Request.Form["comp"];


                                if (mltRcd != "" && catgno == "1" && comp == "")
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_DESPATCH_DETAIL ([WP_NPCC_DESPATCH_HEADER_ID], [BLOCK_ID],[BLOCK_NAME],[HOURS_BW_EVENT_NOTIFICATION],PRESENT_STATE,NTS_HOURS,[SYNC_DESYNC_TIME],TARGET_DEMAND,LOAD_ACHIEVE_MINS,LOAD_ACHIEVE_TIME) 
                                            VALUES (" + ID03 + @", '" + item.Split('½')[0] + @"', '" + item.Split('½')[1] + @"','" + item.Split('½')[2] + @"','" + item.Split('½')[3] + @"','" + item.Split('½')[4] + @"','" + item.Split('½')[5] + @"' ,'" + item.Split('½')[6] + @"','" + item.Split('½')[7] + @"','" + item.Split('½')[8] + @"')");
                                        }
                                    }
                                }
                                if (mltRcd != "" && catgno == "2" && comp == "")
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_DESPATCH_DETAIL ([WP_NPCC_DESPATCH_HEADER_ID], [BLOCK_ID],[BLOCK_NAME],[HOURS_BW_EVENT_NOTIFICATION],PRESENT_STATE,LOAD_ACHIEVE_MINS,LOAD_ACHIEVE_TIME) 
                                            VALUES (" + ID03 + @", '" + item.Split('½')[0] + @"', '" + item.Split('½')[1] + @"','" + item.Split('½')[2] + @"','" + item.Split('½')[3] + @"','" + item.Split('½')[4] + @"','" + item.Split('½')[5] + @"')");
                                        }
                                    }
                                }
                                if (mltRcd != "" && catgno == "2" && comp != "")
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_DESPATCH_DETAIL ([WP_NPCC_DESPATCH_HEADER_ID], [BLOCK_ID],[BLOCK_NAME],[HOURS_BW_EVENT_NOTIFICATION],PRESENT_STATE,LOAD_ACHIEVE_MIN_WST,LOAD_ACHIEVE_TIME_WST,LOAD_ACHIEVE_TIME_WOST) 
                                            VALUES (" + ID03 + @", '" + item.Split('½')[0] + @"', '" + item.Split('½')[1] + @"','" + item.Split('½')[2] + @"','" + item.Split('½')[3] + @"','" + item.Split('½')[4] + @"','" + item.Split('½')[5] + @"','" + item.Split('½')[6] + @"')");
                                        }
                                    }
                                }

                                if (mltRcd != "" && catgno == "3" && comp == "")
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_DESPATCH_DETAIL ([WP_NPCC_DESPATCH_HEADER_ID], [BLOCK_ID],[BLOCK_NAME],[HOURS_BW_EVENT_NOTIFICATION],PRESENT_STATE,NTS_HOURS,[SYNC_DESYNC_TIME],TARGET_DEMAND,LOAD_ACHIEVE_MINS,LOAD_ACHIEVE_TIME) 
                                            VALUES (" + ID03 + @", '" + item.Split('½')[0] + @"', '" + item.Split('½')[1] + @"','" + item.Split('½')[2] + @"','" + item.Split('½')[3] + @"','" + item.Split('½')[4] + @"','" + item.Split('½')[5] + @"','" + item.Split('½')[6] + @"','" + item.Split('½')[7] + @"','" + item.Split('½')[8] + @"')");
                                        }
                                    }
                                }
                                if (mltRcd != "" && catgno == "4" && comp == "")
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_DESPATCH_DETAIL ([WP_NPCC_DESPATCH_HEADER_ID], [BLOCK_ID],[BLOCK_NAME],[HOURS_BW_EVENT_NOTIFICATION]) 
                                            VALUES (" + ID03 + @", '" + item.Split('½')[0] + @"', '" + item.Split('½')[1] + @"', '" + item.Split('½')[2] + @"')");
                                        }
                                    }
                                }
                                Returnvls = ID03;
                            }

                            if (dataID[12] == "Desync")
                            {

                                String ID03 = Fn.ExenID(@"INSERT INTO [CDXP].[WP_NPCC_DESPATCH_HEADER] (
[STATUS]
,BLOCK_ID
,VENDOR_ID
,VENDOR_SITE_ID
,FUEL
,[INTIMATION_TIME]
,[EMERGENCY_TYPE]
,[TARGET_DEMAND]
,[TARGET_DATE_TIME]
,[SENDER_NAME]
,[SENDER_DESIGNATION]
,[NPCC_REMARKS]
,[DEMAND_TYPE]
,[PRESENT_STATE]
,[CURRENT_AVAILABILITY]
,[CREATED_BY]
,[CREATED_ON]
,[SYSTEM_CONSTRAINT]
,[SYSTEM_CONSTRAINT_REASON]
,[IPP_CAT]

)
VALUES(
'" + dataID[2] + @"',
'" + dataID[3] + @"',
'" + dataID[5] + @"',
'" + dataID[6] + @"',
'" + dataID[7] + @"',
'" + dataID[8] + @"' ,
'" + dataID[11] + @"',
CAST('" + dataID[30] + @"' AS int),
'" + dataID[35] + @"',
'" + dataID[14] + @"',
'" + dataID[15] + @"',
'" + dataID[16] + @"',
'" + dataID[12] + @"',
'" + dataID[24] + @"',
'" + dataID[9] + @"',
'" + Convert.ToString(Session["UserName"]) + @"',
'" + dataID[8] + @"',
'" + dataID[4] + @"',
'" + dataID[11] + @"'
,'" + dataID[34] + @"'


) SELECT SCOPE_IDENTITY();");
                                var mltRcd = HttpContext.Request.Form["nxt"];
                                var catgno = HttpContext.Request.Form["catg"];
                                var comp = HttpContext.Request.Form["comp"];

                                if ((mltRcd != "" && catgno == "1" && comp == ""))
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_DESPATCH_DETAIL ([WP_NPCC_DESPATCH_HEADER_ID], [BLOCK_ID],[BLOCK_NAME],
                                                PRESENT_STATE,TARGET_DEMAND,NTDS_HOURS,LOAD_ACHIEVE_TIME) 
                                                VALUES (" + ID03 + @", '" + item.Split('½')[0] + @"','" + item.Split('½')[1] + @"','Hot',
                                                '" + item.Split('½')[2] + @"','" + item.Split('½')[3] + @"','" + item.Split('½')[4] + @"')");
                                        }
                                    }
                                }

                                if ((mltRcd != "" && catgno == "4" && comp == ""))
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        //if (item.Contains("½"))
                                        {
                                            Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_DESPATCH_DETAIL ([WP_NPCC_DESPATCH_HEADER_ID], [BLOCK_ID],[BLOCK_NAME]) VALUES (" + ID03 + @", '" + item.Split('½')[0] + @"', '" + item.Split('½')[1] + @"')");
                                        }
                                    }
                                }
                                Returnvls = ID03;

                            }

                            if (dataID[12] == "Increase/Decrease Load")
                            {
                                String ID03 = Fn.ExenID(@"INSERT INTO [CDXP].[WP_NPCC_DESPATCH_HEADER] (
[STATUS]
,BLOCK_ID
,VENDOR_ID
,VENDOR_SITE_ID
,FUEL
,[INTIMATION_TIME]
,[IDLOAD_ACHIEVE_MIN]
,[TARGET_DEMAND]
,[TARGET_DATE_TIME]
,[SENDER_NAME]
,[SENDER_DESIGNATION]
,[NPCC_REMARKS]
,[DEMAND_TYPE],
[PRESENT_STATE],
[CURRENT_LOAD_IDLOAD]
,[CURRENT_AVAILABILITY]
,[CREATED_BY]
,[CREATED_ON]
,[SYSTEM_CONSTRAINT]
,[SYSTEM_CONSTRAINT_REASON]
,[IPP_CAT]

)
VALUES(
'" + dataID[2] + @"',
'" + dataID[3] + @"', 
'" + dataID[5] + @"',
'" + dataID[6] + @"',
'" + dataID[7] + @"',
'" + dataID[8] + @"',
'" + dataID[31] + @"',
'" + dataID[30] + @"',
'" + dataID[32] + @"',
'" + dataID[14] + @"',
'" + dataID[15] + @"',
'" + dataID[16] + @"',
'" + dataID[12] + @"',
'" + dataID[24] + @"',
'" + dataID[29] + @"',
'" + dataID[9] + @"',
'" + Convert.ToString(Session["UserName"]) + @"',
'" + dataID[8] + @"',
'" + dataID[4] + @"',
'" + dataID[11] + @"'
,'" + dataID[34] + @"'


) SELECT SCOPE_IDENTITY();");
                            }

                            if (dataID[12] == "Miscellaneous")
                            {
                                String ID03 = Fn.ExenID(@"INSERT INTO [CDXP].[WP_NPCC_DESPATCH_HEADER] (
[STATUS]
,BLOCK_ID
,VENDOR_ID
,VENDOR_SITE_ID
,FUEL
,[INTIMATION_TIME]
,[EMERGENCY_TYPE]
,[TARGET_DEMAND]
,[TARGET_DATE_TIME]
,[SENDER_NAME]
,[SENDER_DESIGNATION]
,[NPCC_REMARKS]
,[DEMAND_TYPE],
[PRESENT_STATE]
,[CREATED_BY]
,[CREATED_ON]
,[SYSTEM_CONSTRAINT]
,[SYSTEM_CONSTRAINT_REASON]
,[Miscellaneous]
,[IPP_CAT]

)
VALUES(
'" + dataID[2] + @"',
'" + dataID[3] + @"', 
'" + dataID[5] + @"',
'" + dataID[6] + @"',
'" + dataID[7] + @"',
'" + dataID[8] + @"',
'" + dataID[11] + @"',
CAST('" + dataID[34] + @"' AS int),
'" + dataID[36] + @"',
'" + dataID[12] + @"',
'" + dataID[13] + @"',
'" + dataID[14] + @"',
'" + dataID[27] + @"',
'" + dataID[28] + @"',
'" + Convert.ToString(Session["UserName"]) + @"',
'" + dataID[8] + @"',
'" + dataID[4] + @"',
'" + dataID[10] + @"',
'" + dataID[37] + @"',
'" + dataID[38] + @"'




) SELECT SCOPE_IDENTITY();");
                            }

                        }
                        else
                        {
                            var Status = dataID[2];
                            var recordId = dataID[1];
                            if (Status == "Acknowledged")
                            {
                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                            [STATUS] = '" + Status + @"',
                                             GC_ACK_BY='" + dataID[25] + @"',
                                      GC_ACK_DESIGNATION ='" + dataID[26] + @"',
                                          GC_ACK_REMARKS ='" + dataID[17] + @"'
                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");
                            }
                            if (Status == "Achieved")
                            {
                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                             [STATUS]  = '" + Status + @"',
                                            GC_COMP_BY ='" + dataID[15] + @"',
                                 [GC_COMP_DESIGNATION] ='" + dataID[16] + @"',
                             [GC_COMP_TARGET_ACHIEVED] ='" + dataID[20] + @"',
                           [GC_COMP_ACHIEVE_DATE_TIME] =CAST('" + dataID[21] + @"'AS DATETIME),
                                     [GC_COMP_REMARKS] ='" + dataID[22] + @"',
                                        [GC_COMP_TYPE] ='" + dataID[24] + @"'


                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");
                            }
                            Returnvls = dataID[1];
                            //if status= acknowledged
                        }

                        break;
                    case 300:// DI Update
                        //for only case 30 
                        if (dataID[1] != "0")
                        {
                            var Status = dataID[2];
                            var recordId = dataID[1];


                            if (dataID[27] == "Sync")
                            {
                                String ID03 = Fn.ExenID(@"INSERT INTO [CDXP].[WP_NPCC_DESPATCH_HEADER] (
[STATUS]
,BLOCK_ID
,VENDOR_ID
,VENDOR_SITE_ID
,FUEL
,[INTIMATION_TIME]
,[EMERGENCY_TYPE]
,[TARGET_DEMAND]
,[TARGET_DATE_TIME]
,[SENDER_NAME]
,[SENDER_DESIGNATION]
,[NPCC_REMARKS]
,[DEMAND_TYPE]
,[PRESENT_STATE]
,[CURRENT_AVAILABILITY]
,[CREATED_BY]
,[CREATED_ON]
,[SYSTEM_CONSTRAINT]
,[SYSTEM_CONSTRAINT_REASON]
,[IPP_CAT]

)
VALUES(
'" + dataID[2] + @"',
'" + dataID[3] + @"',
'" + dataID[5] + @"',
'" + dataID[6] + @"',
'" + dataID[7] + @"',
'" + dataID[8] + @"',
'" + dataID[11] + @"',
CAST('" + dataID[24] + @"' AS int),
'" + dataID[25] + @"',
'" + dataID[12] + @"',
'" + dataID[13] + @"',
'" + dataID[14] + @"',
'" + dataID[27] + @"',
'" + dataID[28] + @"',
'" + dataID[9] + @"',
'" + Convert.ToString(Session["UserName"]) + @"',
'" + dataID[8] + @"',
'" + dataID[4] + @"',
'" + dataID[10] + @"',
'" + dataID[37] + @"'


) SELECT SCOPE_IDENTITY();");

                                var mltRcd = HttpContext.Request.Form["nxt"];
                                var catgno = HttpContext.Request.Form["catg"];
                                var comp = HttpContext.Request.Form["comp"];


                                if (mltRcd != "" && catgno == "1" && comp == "")
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_DESPATCH_DETAIL ([WP_NPCC_DESPATCH_HEADER_ID], [BLOCK_ID],[HOURS_BW_EVENT_NOTIFICATION],PRESENT_STATE,NTS_HOURS,[SYNC_DESYNC_TIME],TARGET_DEMAND,LOAD_ACHIEVE_MINS,LOAD_ACHIEVE_TIME) 
                                            VALUES (" + ID03 + @", '" + item.Split('½')[0] + @"', '" + item.Split('½')[1] + @"','" + item.Split('½')[2] + @"','" + item.Split('½')[3] + @"','" + item.Split('½')[4] + @"','" + item.Split('½')[5] + @"','" + item.Split('½')[6] + @"','" + item.Split('½')[7] + @"')");
                                        }
                                    }
                                }
                                if (mltRcd != "" && catgno == "2" && comp == "")
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_DESPATCH_DETAIL ([WP_NPCC_DESPATCH_HEADER_ID], [BLOCK_ID],[HOURS_BW_EVENT_NOTIFICATION],PRESENT_STATE,LOAD_ACHIEVE_MINS,LOAD_ACHIEVE_TIME) 
                                            VALUES (" + ID03 + @", '" + item.Split('½')[0] + @"', '" + item.Split('½')[1] + @"','" + item.Split('½')[2] + @"','" + item.Split('½')[3] + @"','" + item.Split('½')[4] + @"')");
                                        }
                                    }
                                }
                                if (mltRcd != "" && catgno == "2" && comp != "")
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_DESPATCH_DETAIL ([WP_NPCC_DESPATCH_HEADER_ID], [BLOCK_ID],[HOURS_BW_EVENT_NOTIFICATION],PRESENT_STATE,LOAD_ACHIEVE_MIN_WST,LOAD_ACHIEVE_TIME_WST,LOAD_ACHIEVE_TIME_WOST) 
                                            VALUES (" + ID03 + @", '" + item.Split('½')[0] + @"', '" + item.Split('½')[1] + @"','" + item.Split('½')[2] + @"','" + item.Split('½')[3] + @"','" + item.Split('½')[4] + @"','" + item.Split('½')[5] + @"')");
                                        }
                                    }
                                }

                                if (mltRcd != "" && catgno == "3" && comp == "")
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_DESPATCH_DETAIL ([WP_NPCC_DESPATCH_HEADER_ID], [BLOCK_ID],[HOURS_BW_EVENT_NOTIFICATION],PRESENT_STATE,NTS_HOURS,[SYNC_DESYNC_TIME],TARGET_DEMAND,LOAD_ACHIEVE_MINS,LOAD_ACHIEVE_TIME) 
                                            VALUES (" + ID03 + @", '" + item.Split('½')[0] + @"', '" + item.Split('½')[1] + @"','" + item.Split('½')[2] + @"','" + item.Split('½')[3] + @"','" + item.Split('½')[4] + @"','" + item.Split('½')[5] + @"','" + item.Split('½')[6] + @"','" + item.Split('½')[7] + @"')");
                                        }
                                    }
                                }
                                if (mltRcd != "" && catgno == "4" && comp == "")
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_DESPATCH_DETAIL ([WP_NPCC_DESPATCH_HEADER_ID], [BLOCK_ID],[HOURS_BW_EVENT_NOTIFICATION]) 
                                            VALUES (" + ID03 + @", '" + item.Split('½')[0] + @"', '" + item.Split('½')[1] + @"')");
                                        }
                                    }
                                }
                                Returnvls = ID03;
                            }

                            if (dataID[27] == "Desync")
                            {

                                String ID03 = Fn.ExenID(@"INSERT INTO [CDXP].[WP_NPCC_DESPATCH_HEADER] (
[STATUS]
,BLOCK_ID
,VENDOR_ID
,VENDOR_SITE_ID
,FUEL
,[INTIMATION_TIME]
,[EMERGENCY_TYPE]
,[TARGET_DEMAND]
,[TARGET_DATE_TIME]
,[SENDER_NAME]
,[SENDER_DESIGNATION]
,[NPCC_REMARKS]
,[DEMAND_TYPE]
,[PRESENT_STATE]
,[CURRENT_AVAILABILITY]
,[CREATED_BY]
,[CREATED_ON]
,[SYSTEM_CONSTRAINT]
,[SYSTEM_CONSTRAINT_REASON]
,[IPP_CAT]

)
VALUES(
'" + dataID[2] + @"',
'" + dataID[3] + @"',
'" + dataID[5] + @"',
'" + dataID[6] + @"',
'" + dataID[7] + @"',
'" + dataID[8] + @"' ,
'" + dataID[11] + @"',
CAST('" + dataID[30] + @"' AS int),
'" + dataID[32] + @"',
'" + dataID[12] + @"',
'" + dataID[13] + @"',
'" + dataID[14] + @"',
'" + dataID[27] + @"',
'" + dataID[28] + @"',
'" + dataID[9] + @"',
'" + Convert.ToString(Session["UserName"]) + @"',
'" + dataID[8] + @"',
'" + dataID[4] + @"',
'" + dataID[10] + @"'
,'" + dataID[37] + @"'


) SELECT SCOPE_IDENTITY();");
                                var mltRcd = HttpContext.Request.Form["nxt"];
                                var catgno = HttpContext.Request.Form["catg"];
                                var comp = HttpContext.Request.Form["comp"];

                                if ((mltRcd != "" && catgno == "1" && comp == ""))
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        if (item.Contains("½"))
                                        {
                                            Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_DESPATCH_DETAIL ([WP_NPCC_DESPATCH_HEADER_ID], [BLOCK_ID],
                                                PRESENT_STATE,TARGET_DEMAND,NTDS_HOURS,LOAD_ACHIEVE_TIME) 
                                                VALUES (" + ID03 + @", '" + item.Split('½')[0] + @"','Hot','" + item.Split('½')[1] + @"',
                                                '" + item.Split('½')[2] + @"','" + item.Split('½')[3] + @"')");
                                        }
                                    }
                                }

                                if ((mltRcd != "" && catgno == "4" && comp == ""))
                                {
                                    string[] mdata = new string[500];
                                    mdata = Fn.CleanSQL(HttpUtility.UrlDecode(mltRcd)).Split('¼');
                                    foreach (var item in mdata)
                                    {
                                        //if (item.Contains("½"))
                                        {
                                            Fn.Exec(@"INSERT INTO CDXP.WP_NPCC_DESPATCH_DETAIL ([WP_NPCC_DESPATCH_HEADER_ID], [BLOCK_ID]) VALUES (" + ID03 + @", '" + item.Split('½')[0] + @"')");
                                        }
                                    }
                                }
                                Returnvls = ID03;

                            }

                            if (dataID[27] == "Increase/Decrease Load")
                            {
                                String ID03 = Fn.ExenID(@"INSERT INTO [CDXP].[WP_NPCC_DESPATCH_HEADER] (
[STATUS]
,BLOCK_ID
,VENDOR_ID
,VENDOR_SITE_ID
,FUEL
,[INTIMATION_TIME]
,[IDLOAD_ACHIEVE_MIN]
,[TARGET_DEMAND]
,[TARGET_DATE_TIME]
,[SENDER_NAME]
,[SENDER_DESIGNATION]
,[NPCC_REMARKS]
,[DEMAND_TYPE],
[PRESENT_STATE],
[CURRENT_LOAD_IDLOAD]
,[CURRENT_AVAILABILITY]
,[CREATED_BY]
,[CREATED_ON]
,[SYSTEM_CONSTRAINT]
,[SYSTEM_CONSTRAINT_REASON]
,[IPP_CAT]

)
VALUES(
'" + dataID[2] + @"',
'" + dataID[3] + @"', 
'" + dataID[5] + @"',
'" + dataID[6] + @"',
'" + dataID[7] + @"',
'" + dataID[8] + @"',
'" + dataID[35] + @"',
'" + dataID[34] + @"',
'" + dataID[36] + @"',
'" + dataID[12] + @"',
'" + dataID[13] + @"',
'" + dataID[14] + @"',
'" + dataID[27] + @"',
'" + dataID[28] + @"',
'" + dataID[23] + @"',
'" + dataID[9] + @"',
'" + Convert.ToString(Session["UserName"]) + @"',
'" + dataID[8] + @"',
'" + dataID[4] + @"',
'" + dataID[10] + @"'
,'" + dataID[37] + @"'


) SELECT SCOPE_IDENTITY();");
                            }

                            if (dataID[27] == "Miscellaneous")
                            {
                                Fn.Exec(@"UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET  
[STATUS] ='" + dataID[2] + @"'
,VENDOR_ID='" + dataID[5] + @"'
,VENDOR_SITE_ID = '" + dataID[6] + @"'
,FUEL='" + dataID[7] + @"'
,[INTIMATION_TIME]='" + dataID[8] + @"'
,[EMERGENCY_TYPE]='" + dataID[11] + @"'
,[TARGET_DATE_TIME]='" + dataID[36] + @"'
,[SENDER_NAME]='" + dataID[12] + @"'
,[SENDER_DESIGNATION]='" + dataID[13] + @"'
,[NPCC_REMARKS]='" + dataID[14] + @"'
,[DEMAND_TYPE]='" + dataID[27] + @"'
,[PRESENT_STATE]='" + dataID[28] + @"'
,[CREATED_BY]='" + Convert.ToString(Session["UserName"]) + @"'
,[CREATED_ON]='" + dataID[8] + @"'
,[SYSTEM_CONSTRAINT]='" + dataID[4] + @"'
,[SYSTEM_CONSTRAINT_REASON]='" + dataID[10] + @"'
,[Miscellaneous]='" + dataID[37] + @"'
,[IPP_CAT]='" + dataID[38] + @"'

WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + dataID[1] + "'");
                                Returnvls = dataID[1];

                            }

                        }
                        else
                        {
                            var Status = dataID[2];
                            var recordId = dataID[1];
                            if (Status == "Acknowledged")
                            {
                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                            [STATUS] = '" + Status + @"',
                                             GC_ACK_BY='" + dataID[25] + @"',
                                      GC_ACK_DESIGNATION ='" + dataID[26] + @"',
                                          GC_ACK_REMARKS ='" + dataID[17] + @"'
                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");
                            }
                            if (Status == "Achieved")
                            {
                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_DESPATCH_HEADER] SET 
                                             [STATUS]  = '" + Status + @"',
                                            GC_COMP_BY ='" + dataID[15] + @"',
                                 [GC_COMP_DESIGNATION] ='" + dataID[16] + @"',
                             [GC_COMP_TARGET_ACHIEVED] ='" + dataID[20] + @"',
                           [GC_COMP_ACHIEVE_DATE_TIME] =CAST('" + dataID[21] + @"'AS DATETIME),
                                     [GC_COMP_REMARKS] ='" + dataID[22] + @"',
                                        [GC_COMP_TYPE] ='" + dataID[24] + @"'


                                             WHERE WP_NPCC_DESPATCH_HEADER_ID = '" + recordId + "'");
                            }
                            Returnvls = dataID[1];
                            //if status= acknowledged
                        }

                        break;

                    case 4:
                        //                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT CDXP.WP_NPCC_DESPATCH_HEADER.WP_NPCC_DESPATCH_HEADER_ID,'' [Sr#],FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') [Notification Date & Time] , [CDXP].[AP_SUPPLIERS].VENDOR_NAME [Power Producer], 
                        //[CDXP].[AP_SUPPLIER_SITE_ALL].ADDRESS_LINE1 [Producer Site],
                        //[Block / Complex / Unit] = CASE WHEN ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].ATTRIBUTE4,'') = '' THEN ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].BLOCK_UNIT_TITLE,'') ELSE
                        //ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].BLOCK_UNIT_TITLE,'') + ' (' + ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].ATTRIBUTE4,'') + ')' END,
                        //CDXP.WP_NPCC_DESPATCH_HEADER.DEMAND_TYPE [Demand Type],
                        //CDXP.WP_NPCC_DESPATCH_HEADER.EMERGENCY_TYPE as [Type of Ramp Up/Down],
                        //[dbo].[WP_NPCC_DESPATCH_DETAIL].SYNC_DESYNC_TIME [Sync/Desync Time],
                        // TARGET_DEMAND [Target Demand (MW)],
                        //FORMAT(TARGET_DATE_TIME,'dd-MMM-yyyy HH:mm') [Target Date & Time] ,
                        //FORMAT([GC_COMP_ACHIEVE_DATE_TIME],'dd-MMM-yyyy HH:mm') [Achievement Date & Time] ,
                        //STATUS
                        //FROM CDXP.WP_NPCC_DESPATCH_HEADER
                        //INNER JOIN [CDXP].[AP_SUPPLIER_SITE_ALL] on CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_SITE_ID= [CDXP].[AP_SUPPLIER_SITE_ALL] .[VENDOR_SITE_ID]
                        //INNER JOIN [CDXP].[AP_SUPPLIERS] on [CDXP].[AP_SUPPLIER_SITE_ALL].vendor_id = [CDXP].[AP_SUPPLIERS].VENDOR_ID
                        //INNER JOIN [CDXP].[CPPA_PPA_PLT_BLK_FUEL] ON  [CDXP].[CPPA_PPA_PLT_BLK_FUEL].PLT_BLK_FUEL_ID =CDXP.WP_NPCC_DESPATCH_HEADER.BLOCK_ID
                        //LEFT OUTER JOIN [dbo].[WP_NPCC_DESPATCH_DETAIL] ON CDXP.WP_NPCC_DESPATCH_HEADER.WP_NPCC_DESPATCH_HEADER_ID = [dbo].[WP_NPCC_DESPATCH_DETAIL].WP_NPCC_DESPATCH_HEADER_ID", "tblJ1");

                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"  
                          SELECT CDXP.WP_NPCC_DESPATCH_HEADER.WP_NPCC_DESPATCH_HEADER_ID,'' [Sr#],
                        FORMAT(CDXP.WP_NPCC_DESPATCH_HEADER.INTIMATION_TIME,'dd-MMM-yyyy HH:mm') [Notification Date & Time] ,  
                        [CDXP].[AP_SUPPLIERS].VENDOR_NAME [Power Producer],
                        [Unit / Complex] = ISNULL([CDXP].[WP_NPCC_DESPATCH_DETAIL].BLOCK_NAME,'Complex'),
                        CDXP.WP_NPCC_DESPATCH_HEADER.DEMAND_TYPE [Demand Type],
                        FORMAT([CDXP].[WP_NPCC_DESPATCH_DETAIL].SYNC_DESYNC_TIME,'dd-MMM-yyyy HH:mm') [Sync/Desync Demand Time],   
                        [CDXP].[WP_NPCC_DESPATCH_DETAIL].TARGET_DEMAND [Sync Target Demand],
                        CDXP.WP_NPCC_DESPATCH_HEADER.TARGET_DEMAND [Target Demand (MW)],
                        CDXP.WP_NPCC_DESPATCH_HEADER.CURRENT_LOAD_IDLOAD [Current Load (MW)],
                        CDXP.WP_NPCC_DESPATCH_HEADER.TARGET_DATE_TIME [Target Load Achievement Time] ,

                        FORMAT([CDXP].[WP_NPCC_DESPATCH_DETAIL].GC_COMP_DESYNC_SYNC_DATE_TIME,'dd-MMM-yyyy HH:mm') [Actual Sync/Desync Time] ,  
                        ISNULL([CDXP].[WP_NPCC_DESPATCH_DETAIL].GC_COMP_TARGET_ACHIEVED ,CDXP.WP_NPCC_DESPATCH_HEADER.GC_COMP_TARGET_ACHIEVED)[Actual Load Achieved (MW)],
                        ISNULL([CDXP].[WP_NPCC_DESPATCH_DETAIL].GC_COMP_ACHIEVE_DATE_TIME ,CDXP.WP_NPCC_DESPATCH_HEADER.GC_COMP_ACHIEVE_DATE_TIME)[Actual Load Achieved Time],

                        ISNULL([CDXP].[WP_NPCC_DESPATCH_DETAIL].STATUS ,CDXP.WP_NPCC_DESPATCH_HEADER.STATUS)[STATUS],
                        FORMAT(CDXP.WP_NPCC_DESPATCH_HEADER.GC_ACK_TIME,'dd-MMM-yyyy HH:mm') [Ack Time]   
                        FROM CDXP.WP_NPCC_DESPATCH_HEADER
                        INNER JOIN [CDXP].[AP_SUPPLIER_SITE_ALL] on CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_SITE_ID= [CDXP].[AP_SUPPLIER_SITE_ALL] .[VENDOR_SITE_ID]
                        INNER JOIN [CDXP].[AP_SUPPLIERS] on [CDXP].[AP_SUPPLIER_SITE_ALL].vendor_id = [CDXP].[AP_SUPPLIERS].VENDOR_ID
                        INNER JOIN [CDXP].[CPPA_PPA_PLT_BLK_FUEL] ON  [CDXP].[CPPA_PPA_PLT_BLK_FUEL].PLT_BLK_FUEL_ID =CDXP.WP_NPCC_DESPATCH_HEADER.BLOCK_ID
                        LEFT OUTER JOIN [CDXP].[WP_NPCC_DESPATCH_DETAIL] ON CDXP.WP_NPCC_DESPATCH_HEADER.WP_NPCC_DESPATCH_HEADER_ID = [CDXP].[WP_NPCC_DESPATCH_DETAIL].WP_NPCC_DESPATCH_HEADER_ID

                          INNER JOIN
                                               CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
                                             CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_SITE_ID  = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID    
                    WHERE  (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')", "tblJ1");

                        break;
                    case 5:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            var SETUP_SITE_ID = Convert.ToDecimal(dataID[1]);

                            Returnvls = Fn.DespatchData2Dropdown((from dh in db.WP_NPCC_DESPATCH_HEADER
                                                                  where dh.VENDOR_SITE_ID == SETUP_SITE_ID
                                                                  select new
                                                                  { dh.SETUP_SITE_ID_FK, dh.TARGET_DEMAND, dh.INTIMATION_TIME, dh.WP_NPCC_DESPATCH_HEADER_ID }).AsEnumerable()
                                        .Select(
                                            s => new
                                            {
                                                Demand_Data = s.SETUP_SITE_ID_FK.ToString() + "½" + s.TARGET_DEMAND.ToString() + "½" + (s.INTIMATION_TIME.HasValue ? s.INTIMATION_TIME.Value.ToString("dd-MMM-yyyy HH:mm") : string.Empty) + "½" + s.WP_NPCC_DESPATCH_HEADER_ID.ToString(),
                                                Demand_No = s.WP_NPCC_DESPATCH_HEADER_ID
                                            }).ToList());
                        }
                        break;
                    case 6:
                        Returnvls = Fn.ExenID(@"INSERT INTO CDXP.WP_NPCC_FAILURE_NOTIFICATION
                         (IPP_CAT,SETUP_SITE_ID_FK, GENERATION_COMPANY, SENDER_NAME, SENDER_DESIGNATION, CREATED_BY, CREATED_ON, NPCC_REMARKS, TARGET_DEMAND,INTIMATION_TIME,STATUS,SITE,SUPPLY,NOTIFICATION_COUNT , COMPLIANCED_ACHIEVEMENT_TIME )
            VALUES('" + dataID[12] + @"','" + dataID[1] + @"','" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + Convert.ToString(Session["UserName"]) + @"','" + dataID[7] + @"','" + dataID[5] + @"','" + dataID[6] + @"','" + serverDate + @"','" + dataID[8] + @"','" + dataID[9] + @"','" + Convert.ToDecimal(dataID[10]) + @"','" + dataID[11] + @"' , NULL);  select SCOPE_IDENTITY();");
                        //                        Fn.ExenID(@"INSERT INTO CDXP.WP_NPCC_DESPATCH_DETAIL
                        //                         (WP_NPCC_DESPATCH_HEADER_ID_FK, TARGET_DATE, TARGET_TIME, REMARKS, SENDER_NAME)
                        //VALUES        ('" + Returnvls + @"','" + dataID[7].Split(' ')[0] + @"','" + dataID[7].Split(' ')[1] + @"','" + dataID[8] + @"');");
                        Fn.Exec(@" CDXP.SP_Notification_Submission_Record '" + Returnvls + "','3','" + dataID[1] + @"','" + dataID[1] + @"','2','" + dataID[2] + @"','" + serverDate + "' ");


                        break;
                    case 60:
                        Returnvls = Fn.Exec(@"UPDATE CDXP.WP_NPCC_FAILURE_NOTIFICATION SET 
SETUP_SITE_ID_FK='" + dataID[1] + @"', 
GENERATION_COMPANY='" + dataID[2] + @"',
SENDER_NAME='" + dataID[3] + @"',
SENDER_DESIGNATION='" + dataID[4] + @"',
CREATED_BY='" + Convert.ToString(Session["UserName"]) + @"',
CREATED_ON='" + dataID[7] + @"',
NPCC_REMARKS='" + dataID[5] + @"',
TARGET_DEMAND='" + dataID[6] + @"',
INTIMATION_TIME='" + serverDate + @"',
STATUS='" + dataID[8] + @"',
SITE='" + dataID[9] + @"',
SUPPLY='" + Convert.ToDecimal(dataID[10]) + @"',
NOTIFICATION_COUNT='" + dataID[11] + @"',
IPP_CAT='" + dataID[13] + @"' 
WHERE WP_NPCC_FAILURE_NOTIFICATION_ID_PK='" + dataID[12] + @"'");
                        //                        Fn.ExenID(@"INSERT INTO CDXP.WP_NPCC_DESPATCH_DETAIL
                        //                         (WP_NPCC_DESPATCH_HEADER_ID_FK, TARGET_DATE, TARGET_TIME, REMARKS, SENDER_NAME)
                        //VALUES        ('" + Returnvls + @"','" + dataID[7].Split(' ')[0] + @"','" + dataID[7].Split(' ')[1] + @"','" + dataID[8] + @"');");
                        break;

                    case 61:
                        Returnvls = Fn.Exec(@"UPDATE CDXP.WP_NPCC_FAILURE_NOTIFICATION SET STATUS='Acknowledged' where WP_NPCC_FAILURE_NOTIFICATION_ID_PK='" + dataID[1] + @"'");

                        break;
                    case 62:
                        var achievementTime = dataID[6];
                        if (achievementTime == "")
                        {
                            Returnvls = Fn.Exec(@"UPDATE CDXP.WP_NPCC_FAILURE_NOTIFICATION SET STATUS='Acknowledged',COMPLIANCE_TYPE='" + dataID[4] + @"',COMPLIANCED_MW='" + dataID[5] + @"',COMPLIANCED_ACHIEVEMENT_TIME=NULL ,GC_ACKNOWLEDGEMENT_TIME ='" + serverDate + "',GC_AckBy = '" + dataID[7] + @"' where WP_NPCC_FAILURE_NOTIFICATION_ID_PK='" + dataID[1] + @"'");
                        }
                        else
                        {
                            Returnvls = Fn.Exec(@"UPDATE CDXP.WP_NPCC_FAILURE_NOTIFICATION SET STATUS='Acknowledged',COMPLIANCE_TYPE='" + dataID[4] + @"',COMPLIANCED_MW='" + dataID[5] + @"',COMPLIANCED_ACHIEVEMENT_TIME='" + dataID[6] + @"',GC_ACKNOWLEDGEMENT_TIME ='" + serverDate + "',GC_AckBy = '" + dataID[7] + @"' where WP_NPCC_FAILURE_NOTIFICATION_ID_PK='" + dataID[1] + @"'");
                        }



                        break;
                    case 7:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"
                                            Select CDXP.WP_NPCC_FAILURE_NOTIFICATION.WP_NPCC_FAILURE_NOTIFICATION_ID_PK,''[Sr#], 
                                            FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') [Notification Date & Time],
                                            GENERATION_COMPANY [Power Producer], TARGET_DEMAND [Target Demand (MW)], 
                                            SUPPLY [Supplied (MW)], STATUS,
                                            GC_ACKNOWLEDGEMENT_TIME [Acknowledgement Time],
                                            NPCC_REMARKS [Remarks],COMPLIANCE_TYPE [Compliance Type],COMPLIANCED_MW [Compliance MW],COMPLIANCED_ACHIEVEMENT_TIME [Achievement Time]

                                            from CDXP.WP_NPCC_FAILURE_NOTIFICATION

                              INNER JOIN [CDXP].[AP_SUPPLIER_SITE_ALL] on CDXP.WP_NPCC_FAILURE_NOTIFICATION.SETUP_SITE_ID_FK= [CDXP].[AP_SUPPLIER_SITE_ALL] .[VENDOR_SITE_ID]
                                INNER JOIN [CDXP].[AP_SUPPLIERS] on [CDXP].[AP_SUPPLIER_SITE_ALL].vendor_id = [CDXP].[AP_SUPPLIERS].VENDOR_ID
                                INNER JOIN
                                   CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
                                 CDXP.WP_NPCC_FAILURE_NOTIFICATION.SETUP_SITE_ID_FK  = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID    
                                WHERE  (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')", "tblJ2");
                        break;
                    case 8:
                        var VENDOR_SITE_ID0 = Convert.ToInt32(dataID[1]);
                        Returnvls = Fn.Data2DropdownSQL(@"   SELECT DISTINCT PPA_HEADER.HEADER_ID_PK AS ID,  CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE FROM CDXP.CPPA_PPA_PLT_BLK_FUEL INNER JOIN CDXP.PPA_HEADER 
                      ON CPPA_PPA_PLT_BLK_FUEL.HEADER_ID_FK = PPA_HEADER.HEADER_ID_PK WHERE VENDOR_SITE_ID_FK='" + dataID[1] + @"'");
                        break;
                    case 9:
                        if (dataID[1] == "0")
                        {
                            //if (dataID[29]=="HRSG")
                            //{
                            //    Returnvls=Fn.ExenID(@"INSERT INTO EVENT_AVAILABILITY_CHECK ([PLT_BLK_FUEL_ID_FK],[VENDOR_ID],[VENDOR_SITE_ID]
                            //            ,[UNIT_BLOCK_TITLE],[FUEL_TYPE],[EVENT_TYPE],[REASON],[TYPE_OF_OUTAGE],[STATUS_BEFORE_SYNC],[STATUS_OF_MACHINE]
                            //            ,[PREV_AVAILABILITY],[AVAILABILITY],[COMPLEX_PREV_AVAILABILITY],[COMPLEX_AVAILABILITY],[EVENT_TIME])

                            //VALUES('" + dataID[3] + @"', '" + dataID[8] + @"', '" + dataID[9] + @"', '" + dataID[6] + @"', '" + dataID[7] + @"', '" + dataID[9] + @"',
                            //'" + dataID[15] + @"', '" + dataID[17] + @"', '" + dataID[18] + @"', '" + dataID[19] + @"', '" + dataID[20] + @"', '" + dataID[21] + @"', '" + dataID[22] + @"',
                            //'" + dataID[23] + @"', '" + dataID[24] + @"', '" + dataID[18] + @"', CAST('" + dataID[16] + @"'AS DATETIME),)");
                            //}

                            //if (dataID[29] == "HRSG" || dataID[29] != "HRSG")
                            //{


                            //Returnvls = Fn.Exec(@"EXEC INSERT_HRSG_EVENT '" + dataID[3] + @"', '" + dataID[8] + @"', '" + dataID[9] + @"', '" + dataID[6] + @"', '" + dataID[7] + @"',
                            //'" + dataID[15] + @"', '" + dataID[17] + @"', '" + dataID[18] + @"', '" + dataID[19] + @"', '" + dataID[20] + @"', '" + dataID[21] + @"', '" + dataID[22] + @"',
                            //'" + dataID[23] + @"', '" + dataID[24] + @"','" + dataID[16] + @"'");

                            var plantEventReason = "";

                            if(dataID[22] == "null")
                            {
                                plantEventReason = " ";
                            }
                            else
                            {
                                plantEventReason = dataID[22];
                            }



                            String ID03 = Fn.ExenID(@"INSERT INTO [CDXP].[WP_NPCC_PLANT_EVENTS] (
                                   [STATUS]
                                   ,[PLT_BLK_FUEL_ID]
                                   ,[GENERATION_COMPANY]
                                   ,[SITE]
                                   ,[BLOCK]
                                   ,[FUEL]
                                   ,[IPP_CATEGORY]
                                   ,[VENDOR_ID]
                                   ,[SETUP_SITE_ID_FK]
                                   ,[POWER_POLICY]
                                   ,[INSTALLED_CAPACITY]
                                   ,[DEPENDABLE_CAPACITY]

                                   ,[EVENT]
                                   ,[TYPE_OF_OUTAGE]
                                   ,[REASON]
                                   ,[REASON_DETAIL]
                                   ,[STATUS_OF_MACHINE]
                                   ,[PREV_AVAILABILITY]
                                   ,[AVAILABILITY]
                                   ,[COMPLEX_PREV_AVAILABILITY]
                                   ,[COMPLEX_AVAILABILITY]
                                   ,[derated_capacity]
                                   ,[last_status_event]
                                   ,[EVENT_TIME]
                                   ,[INTIMATION_TIME]
                                   ,[Miscellaneous_Message]

                                   ,[SENDER_NAME]
                                   ,[SENDER_DESIGNATION]
                                   ,[REMARKS])
                            VALUES(
                            '" + dataID[2] + @"',
                            '" + dataID[3] + @"',
                            '" + dataID[4] + @"',
                            '" + dataID[5] + @"',
                            '" + dataID[6] + @"',
                            '" + dataID[7] + @"' ,
                            '" + dataID[8] + @"',
                           
                            '" + dataID[9] + @"',
                            '" + dataID[10] + @"',

                            '" + dataID[17] + @"',
                            '" + dataID[18] + @"',
                            '" + dataID[19] + @"',

                            '" + dataID[20] + @"',
                            '" + dataID[21] + @"',
                            '" + plantEventReason + @"',
                            '" + dataID[23] + @"',
                            '" + dataID[24] + @"',
                            '" + dataID[25] + @"',
                            '" + dataID[26] + @"',
                            '" + dataID[27] + @"',
                            '" + dataID[28] + @"',
                            '" + dataID[29] + @"',
                           '" + dataID[30] + @"',

                            CAST('" + dataID[31] + @"'AS DATETIME),
                            CAST('" + serverDate + @"'AS DATETIME),
                            '" + dataID[33] + @"' ,
                            '" + dataID[34] + @"',
                            '" + dataID[35] + @"',
                            '" + dataID[36] + @"'


                            ) SELECT SCOPE_IDENTITY();");

                            Returnvls = ID03;
                            Fn.Exec(@" CDXP.SP_Notification_Submission_Record '" + ID03 + "','1','" + dataID[9] + @"','" + dataID[10] + @"','1','" + dataID[4] + @"','" + serverDate + "' ");

                        }

                        else
                        {
                            var Status = dataID[2];
                            var recordId = dataID[1];
                            if (Status == "Submitted")
                            {
                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_PLANT_EVENTS] SET 
                                            [STATUS] = '" + Status + @"'
                                             WHERE [WP_NPCC_PLANT_EVENTS_ID_PK] = '" + recordId + "'");
                                Returnvls = dataID[1];
                            }
                            if (Status == "Draft")
                            {
                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_PLANT_EVENTS] SET 
                                            [STATUS] = '" + Status + @"'
                                             WHERE [WP_NPCC_PLANT_EVENTS_ID_PK] = '" + recordId + "'");
                                Returnvls = dataID[1];
                            }
                            if (Status == "Acknowledge")
                            {
                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_PLANT_EVENTS] SET 
                                            [STATUS] = 'Acknowledged',[NPCC_REMARKS]='" + dataID[21] + @"'
                                             WHERE [WP_NPCC_PLANT_EVENTS_ID_PK] = '" + recordId + "'");
                                Returnvls = dataID[1];
                            }

                            if (Status == "Acknowledged")
                            {
                                //Fn.Exec((@"EXEC NPCC_PLANT_EVENT_UPDATE " + Status + @",CAST('" + dataID[4] + @"'AS DATETIME),'" + dataID[7] + @"','" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"','" + dataID[11] + @"','" + dataID[12] + @"','" + dataID[13] + @"','" + dataID[14] + @"','" + dataID[15] + @"','" + dataID[18] + @"',CAST('" + dataID[16] + @"'AS DATETIME),'" + dataID[19] + @"','" + dataID[3] + @"','" + recordId + "' "));
                                Fn.Exec(@" UPDATE [CDXP].[WP_NPCC_PLANT_EVENTS] SET 
                                              [STATUS] = '" + Status + @"'
                                             ,[NPCC_ACK]= 'true'
                                             ,[NPCC_ACK_DATE_TIME] = CAST('" + serverDate + @"'AS DATETIME)
                                             ,[NPCC_RECEIVER_NAME]='" + dataID[7] + @"'
                                             ,[NPCC_RECEIVER_DESIGNATION]='" + dataID[8] + @"'
                                             ,[NPCC_EVENT]='" + dataID[9] + @"'
                                             ,[NPCC_EVENT_TIME]='" + dataID[10] + @"'
                                             ,[NPCC_TYPE_OF_OUTAGE]='" + dataID[11] + @"'
                                             ,[NPCC_REASON]='" + dataID[12] + @"'
                                             ,[NPCC_STATUS_OF_MACHINE]='" + dataID[13] + @"'
                                             ,[derated_capacity]='" + dataID[14] + @"'
                                             ,[last_status_event]='" + dataID[15] + @"'
                                             ,[Miscellaneous_Message]='" + dataID[18] + @"'
                                             ,[NPCC_INTIMATION_TIME] = CAST('" + dataID[16] + @"'AS DATETIME)
                                             ,[NPCC_REMARKS]='" + dataID[19] + @"'                                            
                                             ,[EVENT_VERIFIED]='" + dataID[3] + @"'
                                             WHERE [WP_NPCC_PLANT_EVENTS_ID_PK] = '" + recordId + "'");

                            }

                            if (Status == "Rejected")
                            {
                                //Fn.Exec((@"EXEC NPCC_PLANT_EVENT_UPDATE " + Status + @",CAST('" + dataID[4] + @"'AS DATETIME),'" + dataID[7] + @"','" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"','" + dataID[11] + @"','" + dataID[12] + @"','" + dataID[13] + @"','" + dataID[14] + @"','" + dataID[15] + @"','" + dataID[18] + @"',CAST('" + dataID[16] + @"'AS DATETIME),'" + dataID[19] + @"','" + dataID[3] + @"','" + recordId + "' "));
                                Fn.Exec(@" UPDATE [CDXP].[WP_NPCC_PLANT_EVENTS] SET 
                                              [STATUS] = '" + Status + @"'
                                             ,[NPCC_ACK]= 'true'
                                             ,[NPCC_ACK_DATE_TIME] = CAST('" + serverDate + @"'AS DATETIME)
                                             ,[NPCC_RECEIVER_NAME]='" + dataID[7] + @"'
                                             ,[NPCC_RECEIVER_DESIGNATION]='" + dataID[8] + @"'
                                             ,[NPCC_EVENT]='" + dataID[9] + @"'
                                             ,[NPCC_EVENT_TIME]='" + dataID[10] + @"'
                                             ,[NPCC_TYPE_OF_OUTAGE]='" + dataID[11] + @"'
                                             ,[NPCC_REASON]='" + dataID[12] + @"'
                                             ,[NPCC_STATUS_OF_MACHINE]='" + dataID[13] + @"'
                                             ,[derated_capacity]='" + dataID[14] + @"'
                                             ,[last_status_event]='" + dataID[15] + @"'
                                             ,[Miscellaneous_Message]='" + dataID[18] + @"'
                                             ,[NPCC_INTIMATION_TIME] = CAST('" + dataID[16] + @"'AS DATETIME)
                                             ,[NPCC_REMARKS]='" + dataID[19] + @"'                                            
                                             ,[EVENT_VERIFIED]='" + dataID[3] + @"'
                                             WHERE [WP_NPCC_PLANT_EVENTS_ID_PK] = '" + recordId + "'");

                            }
                        }

                break;


                    case 109:

                        var recordsId = dataID[1];
                        Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_PLANT_EVENTS] SET 
                                            [STATUS] = 'Acknowledged',[NPCC_REMARKS]='" + dataID[19] + @"',NPCC_ACK_DATE_TIME=CAST('" + dataID[4] + @"'AS DATETIME)
                                             WHERE [WP_NPCC_PLANT_EVENTS_ID_PK] = '" + recordsId + "'");
                        Returnvls = dataID[1];


                        break;
                    case 110:

                        var recId = dataID[1];
                        Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_PLANT_EVENTS] SET 
                                            [STATUS] = 'Acknowledged',NPCC_ACK_DATE_TIME=CAST('" + serverDate + @"'AS DATETIME)
                                             WHERE [WP_NPCC_PLANT_EVENTS_ID_PK] = '" + recId + "'");
                        Returnvls = dataID[1];


                        break;
                    case 10:
                        //Fn.Exec(@"UPDATE [CDXP].[WP_NPCC_PLANT_EVENTS] SET [STATUS]=CASE WHEN DATEDIFF(MINUTE, FORMAT([EVENT_TIME],'dd-MMM-yyyy HH:mm'),
                        // GETDATE()) > 15 and [CDXP].[WP_NPCC_PLANT_EVENTS].[STATUS]!='Draft' THEN 'Acknowledged' ELSE [CDXP].[WP_NPCC_PLANT_EVENTS].[STATUS]
                        // END where [CDXP].[WP_NPCC_PLANT_EVENTS].[STATUS]='Submitted'"); // for NPCC auto acknowledges after 15 mins





                        //Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT[CDXP].[WP_NPCC_PLANT_EVENTS].[WP_NPCC_PLANT_EVENTS_ID_PK],'' [Sr#]
                        // ,[INTIMATION_TIME] [Intimation Date & Time],[GENERATION_COMPANY] [Power Producer],[BLOCK],[FUEL],FORMAT([EVENT_TIME],'dd-MMM-yyyy HH:mm') AS[Event Time]
                        // ,[EVENT], [TYPE_OF_OUTAGE] as [Type of Outage] ,[REASON],[STATUS_OF_MACHINE],
                        // [STATUS], [REMARKS] ,[SENDER_NAME]
                        // ,[EVENT_VERIFIED] AS[VERIFIED EVENT],[NPCC_TYPE_OF_OUTAGE] as [NPCC Agreed Type of Outage], [NPCC_REASON] AS[NPCC REASON],[NPCC_STATUS_OF_MACHINE] AS[NPCC STATUS OF MACHINE],[NPCC_STATUS_BEFORE_SYNC] AS[NPCC STATUS BEFORE SYNC],
                        // [NPCC_INTIMATION_TIME] AS [NPCC INTIMATION TIME]
                        // FROM [CDXP].[WP_NPCC_PLANT_EVENTS] ", "tblJ1");



                        var userName = Convert.ToString(Session["UserName"]);



                        var userType = Convert.ToInt32(Fn.ExenID("SELECT distinct WPU.WP_SETUP_USER_TYPES_ID FROM CDXP.WP_PORTAL_USERS wpu WHERE wpu.USER_NAME = '" + userName + "'"));
                        if (userType == 10)
                        {




                            Returnvls = Fn.HTMLTableWithID_TR_Tag(@"
                                        SELECT [CDXP].[WP_NPCC_PLANT_EVENTS].[WP_NPCC_PLANT_EVENTS_ID_PK],'' [Sr#]
                                        ,FORMAT([EVENT_TIME],'dd-MMM-yyyy HH:mm') AS[Event Time],[GENERATION_COMPANY] [Power Producer],BLOCK [Unit / Complex],[FUEL],FORMAT([INTIMATION_TIME],'dd-MMM-yyyy HH:mm') [Intimation Date &Time]
                                        ,CASE WHEN STATUS='Submitted' THEN [EVENT] ELSE [NPCC_EVENT] END AS [EVENT] , CASE WHEN STATUS='Submitted' THEN [TYPE_OF_OUTAGE] ELSE [NPCC_TYPE_OF_OUTAGE] END AS [Type of Outage] , CASE WHEN STATUS='Submitted' THEN [REASON] ELSE [NPCC_REASON] END AS [REASON],
                                        REASON_DETAIL [Detail],[STATUS],FORMAT([NPCC_ACK_DATE_TIME],'dd-MMM-yyyy HH:mm') [Ack Time], [SENDER_NAME]
                                        ,[REMARKS] AS[IPP REMARKS]
                                        FROM [CDXP].[WP_NPCC_PLANT_EVENTS]
                                        INNER JOIN [CDXP].[AP_SUPPLIER_SITE_ALL] on [CDXP].[WP_NPCC_PLANT_EVENTS].SETUP_SITE_ID_FK= [CDXP].[AP_SUPPLIER_SITE_ALL] .[VENDOR_SITE_ID]
                                        INNER JOIN [CDXP].[AP_SUPPLIERS] on [CDXP].[AP_SUPPLIER_SITE_ALL].vendor_id = [CDXP].[AP_SUPPLIERS].VENDOR_ID
                                        INNER JOIN
                                        CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
                                        [CDXP].[WP_NPCC_PLANT_EVENTS].SETUP_SITE_ID_FK = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID
                                        WHERE (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"') AND (Convert(varchar(20), (DATEADD(DAY, -90, CDXP.GETDATEpk())), 101) <= Convert(varchar(20), EVENT_TIME, 101) )
                                        ORDER BY [CDXP].[WP_NPCC_PLANT_EVENTS].[EVENT_TIME] DESC ", "tblJ1");



                        }



                        else
                        {
                            Returnvls = Fn.HTMLTableWithID_TR_Tag(@"
                                SELECT [CDXP].[WP_NPCC_PLANT_EVENTS].[WP_NPCC_PLANT_EVENTS_ID_PK],'' [Sr#]
                                ,FORMAT([EVENT_TIME],'dd-MMM-yyyy HH:mm') AS[Event Time],[GENERATION_COMPANY] [Power Producer],BLOCK [Unit / Complex],[FUEL],FORMAT([INTIMATION_TIME],'dd-MMM-yyyy HH:mm') [Intimation Date &Time]
                                ,CASE WHEN STATUS='Submitted' THEN [EVENT] ELSE [NPCC_EVENT] END AS [EVENT] , CASE WHEN STATUS='Submitted' THEN [TYPE_OF_OUTAGE] ELSE [NPCC_TYPE_OF_OUTAGE] END AS [Type of Outage] , CASE WHEN STATUS='Submitted' THEN [REASON] ELSE [NPCC_REASON] END AS [REASON],
                                REASON_DETAIL [Detail],[STATUS],FORMAT([NPCC_ACK_DATE_TIME],'dd-MMM-yyyy HH:mm') [Ack Time], [SENDER_NAME]
                                ,[REMARKS] AS[IPP REMARKS]
                                FROM [CDXP].[WP_NPCC_PLANT_EVENTS]
                                INNER JOIN [CDXP].[AP_SUPPLIER_SITE_ALL] on [CDXP].[WP_NPCC_PLANT_EVENTS].SETUP_SITE_ID_FK= [CDXP].[AP_SUPPLIER_SITE_ALL] .[VENDOR_SITE_ID]
                                INNER JOIN [CDXP].[AP_SUPPLIERS] on [CDXP].[AP_SUPPLIER_SITE_ALL].vendor_id = [CDXP].[AP_SUPPLIERS].VENDOR_ID
                                INNER JOIN
                                CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND
                                [CDXP].[WP_NPCC_PLANT_EVENTS].SETUP_SITE_ID_FK = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID
                                WHERE (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"') AND (Convert(varchar(20), (DATEADD(DAY, -10, CDXP.GETDATEpk())), 101) <= Convert(varchar(20), EVENT_TIME, 101) )
                                ORDER BY [CDXP].[WP_NPCC_PLANT_EVENTS].[EVENT_TIME] DESC ", "tblJ1");
                        }
                        break;
                    case 11:
                        try
                        {
                            //string sstr1 = @"INSERT INTO CDXP.WP_NPCC_LOAD_CURTAILMENT
                            //              (VENDOR_ID, GENERATION_COMPANY, TOTAL_WTG, AVAILABLE_WTG, UNAVAILABLE_DUE_TO_FO, UNAVAILABLE_DUE_TO_SO, INSTRUCTION_TYPE, WTG_CURTAILMENT, LOAD_CURTAILMENT, CURTAILMENT_DETAIL, NOTIFICATION_TIME,SYNC_DYSYNC_TIME,ACTUAL_ACHIEVEMENT_TIME,PRIOR_TO_CURTAILMENT,AFTER_CURTAILMENT,WTG_PRIOR_TO_CURTAILMENT,WTG_AFTER_CURTAILMENT,SENDER_NAME,SENDER_DESIGNATION, NPCC_REMARKS, SITE, CREATED_BY, CREATED_ON, STATUS, VENDOR_SITE_ID,WndRad_BEFORE, WndRad_After)
                            //              VALUES        ('" + dataID[1] + @"','" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"','" + dataID[7] + @"','" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"','" + dataID[11] + @"','" + dataID[12] + @"','" + dataID[13] + @"','" + dataID[14] + @"','" + dataID[15] + @"','" + dataID[16] + @"','" + dataID[17] + @"','" + dataID[18] + @"','" + dataID[19] + @"','" + dataID[20] + @"', '" + dataID[21] + @"', '" + Convert.ToString(Session["UserName"]) + @"','" + serverDate + @"','" + dataID[22] + @"','" + dataID[23] + @"','" + dataID[24] + @"','" + dataID[25] + @"');  select SCOPE_IDENTITY();";

                            DateTime NotifyTime = DateTime.Parse(dataID[9]);
                            NotifyTime = NotifyTime.Date;

                            if (dataID[4] != "Change in Curtailment level")
                            {
                                int isLCExists;
                                isLCExists = Convert.ToInt32(Fn.ExenID("SELECT COUNT(WP_NPCC_LOAD_CURTAILMENT_ID_PK) FROM CDXP.WP_NPCC_LOAD_CURTAILMENT    WHERE VENDOR_ID = '" + dataID[1] + @"' AND CAST(NOTIFICATION_TIME AS DATE) = '" + NotifyTime.Date + "' AND INSTRUCTION_TYPE = '" + dataID[4] + "'"));

                                if (isLCExists >= 1)
                                {
                                    Returnvls = "ALREADY_PRESENT";
                                    break;
                                }
                            }

                            string sstr = @"INSERT INTO CDXP.WP_NPCC_LOAD_CURTAILMENT                                (VENDOR_ID,GENERATION_COMPANY,TOTAL_WTG,INSTRUCTION_TYPE,WTG_CURTAILMENT,LOAD_CURTAILMENT,TO_CURTAILMENT,CURTAILMENT_DETAIL,NOTIFICATION_TIME, SENDER_NAME, SENDER_DESIGNATION,NPCC_REMARKS, SITE, CREATED_BY, CREATED_ON, STATUS, VENDOR_SITE_ID, IPPTYPE)
                              VALUES        ('" + dataID[1] + @"','" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"','" + dataID[7] + @"','" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"', '" + dataID[11] + @"','" + dataID[12] + @"','" + dataID[13] + @"','" + Convert.ToString(Session["UserName"]) + @"','" + serverDate + @"','" + dataID[14] + @"','" + dataID[15] + @"','" + dataID[16] + @"');  select SCOPE_IDENTITY();";
                            Returnvls = Fn.ExenID(sstr);
                            Fn.Exec(@" CDXP.SP_Notification_Submission_Record '" + Returnvls + "','6','" + dataID[1] + @"','" + dataID[15] + @"','2','" + dataID[2] + @"','" + serverDate + "' ");

                        }
                        catch (Exception ex)
                        {

                            string st = ex.Message;
                        }
                        break;
                    case 12:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"Select 1000 ID,'' [Sr#], VENDOR_ID, GENERATION_COMPANY, TOTAL_WTG, AVAILABLE_WTG, UNAVAILABLE_DUE_TO_FO, UNAVAILABLE_DUE_TO_SO, INSTRUCTION_TYPE, WTG_CURTAILMENT, LOAD_CURTAILMENT, CURTAILMENT_DETAIL, NOTIFICATION_TIME, SYNC_DYSYNC_TIME, ACTUAL_ACHIEVEMENT_TIME, PRIOR_TO_CURTAILMENT, AFTER_CURTAILMENT, WTG_PRIOR_TO_CURTAILMENT, WTG_AFTER_CURTAILMENT, SENDER_NAME, SENDER_DESIGNATION, NPCC_REMARKS, SITE, CREATED_BY, CREATED_ON
                        FROM CDXP.WP_NPCC_LOAD_CURTAILMENT", "tblJ1");
                        break;

                    case 13:

                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"Select 1000 ID,'' [Sr#], SETUP_SITE_ID_FK [Site ID], GENERATION_COMPANY, SITE, BLOCK, FUEL, 
                                                                EVENT,REMARKS, AVAILABILITY_BEFORE_EVENT
                                                                from CDXP.WP_NPCC_PLANT_EVENTS", "tblJ1");
                        break;

                    case 14:// recent events data 
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT TOP(10) [WP_NPCC_PLANT_EVENTS_ID_PK],'' [Sr#],(SELECT TRIM('Complex' FROM block)) Unit, FORMAT([EVENT_TIME],'dd-MMM-yyyy HH:mm') as [Event Time],
                                            CONCAT (ISNULL([EVENT],''),' - ',ISNULL([TYPE_OF_OUTAGE],'')) 
                                            as [Type Of Event] FROM [CDXP].[WP_NPCC_PLANT_EVENTS] WHERE
											SETUP_SITE_ID_FK='" + dataID[1] + "' ORDER BY [INTIMATION_TIME] DESC", "tblEvents");
                        break;
                    case 15: //recent despatch data
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT  TOP(10) 0 ID,'' [Sr#], FORMAT([INTIMATION_TIME],'dd-MMM-yyyy HH:mm') as [Notification Time],[DEMAND_TYPE] as 
[Demand Type],[TARGET_DEMAND] as [Target Demand]  FROM 
[CDXP].[WP_NPCC_DESPATCH_HEADER]
INNER JOIN [CDXP].[CPPA_PPA_PLT_BLK_FUEL] AS tblBlk on tblBlk.PLT_BLK_FUEL_ID=[CDXP].[WP_NPCC_DESPATCH_HEADER].BLOCK_ID
WHERE [VENDOR_SITE_ID] = '" + dataID[1] + "' ORDER BY [INTIMATION_TIME] DESC", "tblDespatch");
                        break;


                    case 16:// loading recent DIS by id 

                        var rcordId = dataID[1];

                        Returnvls = Fn.Data2Json(@"Select WP_NPCC_DESPATCH_HEADER_ID,SETUP_SITE_ID_FK,GENERATION_COMPANY,
[CDXP].[WP_NPCC_DESPATCH_HEADER].VENDOR_SITE_ID,
[CDXP].[WP_NPCC_DESPATCH_HEADER].VENDOR_ID, SITE,BLOCK_ID,BLOCK,FUEL_ID,FUEL,  
FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') AS INTIMATION_TIME
,INTIMATION_SOURCE_TELEPHONE,INTIMATION_SOURCE_TELEPHONE_DATE_TIME,INTIMATION_SOURCE_FAX,INTIMATION_SOURCE_FAX_DATE_TIME,
INTIMATION_SOURCE_PORTAL,SENDER_NAME,SENDER_DESIGNATION,GC_ACK_BY,GC_ACK_TIME,GC_ACK_DESIGNATION,GC_ACK_REMARKS,
[CDXP].[WP_NPCC_DESPATCH_HEADER].CREATED_BY,CREATED_ON,MODIFIED_BY,GC_COMP_BY,MODIFIED_ON, PRESENT_STATE,
GC_COMP_DESIGNATION,GC_COMP_TIME,FORMAT(GC_COMP_ACHIEVE_DATE_TIME,'dd-MMM-yyyy HH:mm') AS GC_COMP_ACHIEVE_DATE_TIME,
GC_COMP_REMARKS,GC_COMP_TARGET_ACHIEVED,STATUS,NPCC_REMARKS,DEMAND_TYPE,SYNC_DESYNC_DATE_TIME,EMERGENCY_TYPE,
TARGET_DATE_TIME AS TARGET_DATE_TIME,TARGET_DEMAND,
TARGET_DATE_TIME_2,TARGET_DEMAND_2,DEMAND_NO ,IPP_CAT,Miscellaneous,CURRENT_AVAILABILITY,CURRENT_LOAD_IDLOAD,IDLOAD_ACHIEVE_MIN,
DTL = (
SELECT BLOCK_ID,BLOCK_NAME,FORMAT(SYNC_DESYNC_TIME, 'dd-MMM-yyyy HH:mm') AS [SYNC_DESYNC_TIME],HOURS_BW_EVENT_NOTIFICATION,PRESENT_STATE,NTS_HOURS,TARGET_DEMAND,LOAD_ACHIEVE_MINS,LOAD_ACHIEVE_TIME,LOAD_ACHIEVE_TIME_WST,
  LOAD_ACHIEVE_TIME_WOST,LOAD_ACHIEVE_MIN_WST,NTDS_HOURS,FORMAT(GC_COMP_ACHIEVE_DATE_TIME, 'dd-MMM-yyyy HH:mm') AS [GC_COMP_ACHIEVE_DATE_TIME],GC_COMP_TARGET_ACHIEVED, FORMAT(GC_COMP_DESYNC_SYNC_DATE_TIME, 'dd-MMM-yyyy HH:mm') AS [GC_COMP_DESYNC_SYNC_DATE_TIME] FROM 
[CDXP].[WP_NPCC_DESPATCH_DETAIL]
WHERE [WP_NPCC_DESPATCH_HEADER_ID] = '" + rcordId + "' FOR JSON AUTO), POWERPRODUCER = '<option value='''+CAST([CDXP].[AP_SUPPLIERS].VENDOR_ID AS VARCHAR(100))+'''>'+[CDXP].[AP_SUPPLIERS].VENDOR_NAME+'</option>',VENDORSITE =  '<option value='''+CAST([CDXP].[AP_SUPPLIER_SITE_ALL].VENDOR_SITE_ID AS VARCHAR(100))+'''>'+[CDXP].[AP_SUPPLIER_SITE_ALL].ADDRESS_LINE1+'</option>',BLOCKDDL= '<option value='''+CAST([CDXP].[WP_NPCC_DESPATCH_HEADER].BLOCK_ID AS VARCHAR(100))+'''>'+[CDXP].[CPPA_PPA_PLT_BLK_FUEL].BLOCK_UNIT_TITLE+'</option>',[CDXP].[PPA_HEADER].[POWER_POLICY] from [CDXP].[WP_NPCC_DESPATCH_HEADER] INNER JOIN [CDXP].[AP_SUPPLIER_SITE_ALL] ON [CDXP].[AP_SUPPLIER_SITE_ALL].VENDOR_SITE_ID = [CDXP].[WP_NPCC_DESPATCH_HEADER].VENDOR_SITE_ID INNER JOIN [CDXP].[AP_SUPPLIERS] ON [CDXP].[AP_SUPPLIERS].VENDOR_ID = [CDXP].[AP_SUPPLIER_SITE_ALL].VENDOR_ID INNER JOIN [CDXP].[CPPA_PPA_PLT_BLK_FUEL] ON [CDXP].[CPPA_PPA_PLT_BLK_FUEL].PLT_BLK_FUEL_ID = [CDXP].[WP_NPCC_DESPATCH_HEADER].BLOCK_ID  INNER JOIN [CDXP].[PPA_HEADER] ON [CDXP].[PPA_HEADER].VENDOR_SITE_ID_FK= [CDXP].[WP_NPCC_DESPATCH_HEADER].VENDOR_SITE_ID WHERE [WP_NPCC_DESPATCH_HEADER_ID]= '" + rcordId + "'");
                        break;

                    case 17:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            double VENDOR_ID_FK0 = Convert.ToDouble(dataID[2]);
                            double VENDOR_SITE_ID_FK0 = Convert.ToDouble(dataID[1]);
                            Returnvls = Fn.Data2DropdownSQL(@"SELECT id = CAST(ISNULL(CPPA_PPA_PLT_EMO.PLT_BLK_FUEL_ID,'') AS VARCHAR(100)) + '½' + 
                                CAST(ISNULL(CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE,'') AS  VARCHAR(100)) + '½' + 
                                CAST(ISNULL(CDXP.PPA_HEADER.CONTRACTED_CAPACITY,'') AS VARCHAR(100)) + '½' +
                                CAST(ISNULL(CDXP.PPA_HEADER.POWER_POLICY,'') AS VARCHAR(100)) + '½' +
                                CAST(ISNULL(CDXP.CPPA_PPA_PLT_BLK_FUEL.ATTRIBUTE1,'') AS VARCHAR(100))
                                , CDXP.CPPA_PPA_PLT_EMO.BLK_TITLE_FOR_EMO + ' ( ' + ISNULL(CDXP.CPPA_PPA_PLT_BLK_FUEL.ATTRIBUTE2 ,'--')+ ' ) ' + CDXP.CPPA_PPA_PLT_EMO.ATTRIBUTE1
                                FROM CDXP.CPPA_PPA_PLT_EMO
                                INNER JOIN CDXP.PPA_HEADER ON  PPA_HEADER.HEADER_ID_PK= CPPA_PPA_PLT_EMO.HEADER_ID_FK
                                INNER JOIN CDXP.CPPA_PPA_PLT_BLK_FUEL ON CDXP.CPPA_PPA_PLT_EMO.PLT_BLK_FUEL_ID = CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID 
                                WHERE PPA_HEADER.APPROVAL_STATUS= 'Approved' AND PPA_HEADER.VENDOR_ID_FK=" + Convert.ToString(VENDOR_ID_FK0) + @" AND VENDOR_SITE_ID_FK=" + Convert.ToString(VENDOR_SITE_ID_FK0) + @" 
                         order by case when CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE='Complex' then null else  CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE end asc");
                        }
                        break;

                    case 18://FOR REPUSH CODE FOR PLANT EVENT
                        Returnvls = Fn.Data2Json(@"
                            SELECT [CDXP].[WP_NPCC_PLANT_EVENTS].[WP_NPCC_PLANT_EVENTS_ID_PK],
									[STATUS]
                                   ,[PLT_BLK_FUEL_ID] 
                                   ,[IPP_CATEGORY]
                                   ,[GENERATION_COMPANY] 
                                   ,[SITE] 
                                   ,[BLOCK] 
                                   ,[FUEL] 
                                   ,[VENDOR_ID] 
                                   ,[SETUP_SITE_ID_FK] 
                                   ,[EVENT] 
                                   ,FORMAT([EVENT_TIME] ,'dd-MMM-yyyy HH:mm') AS  [EVENT_TIME]
                                   ,[REASON]  
                                   ,[TYPE_OF_OUTAGE]  
                                   ,[STATUS_OF_MACHINE]  
                                  ,[derated_capacity] 
                                  ,[last_status_event]
                                  ,[Miscellaneous_Message] 
                                  ,[POWER_POLICY] 
                                  ,[INSTALLED_CAPACITY] 
                                  ,[DEPENDABLE_CAPACITY] 
                                    
                                   ,FORMAT([INTIMATION_TIME] ,'dd-MMM-yyyy HH:mm') AS [INTIMATION_TIME]
                                   ,[SENDER_NAME]   
                                   ,[SENDER_DESIGNATION] 
                                   ,[REMARKS]
                                  ,[AVAILABILITY]
                                  ,[PREV_AVAILABILITY]
                                  ,[REASON_DETAIL]
,                                   POWERPRODUCER = '<option value='''+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].VENDOR_ID AS VARCHAR(100))+'''>'+[CDXP].[WP_NPCC_PLANT_EVENTS].GENERATION_COMPANY+'</option>'
,
VENDORSITE =  '<option value='''+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[SETUP_SITE_ID_FK]  AS VARCHAR(100))+'''>'+[CDXP].[WP_NPCC_PLANT_EVENTS].[SITE]+'</option>'
,
BLOCKDDL= '<option value='''+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].PLT_BLK_FUEL_ID as VARCHAR(100))+'''>'+[CDXP].[WP_NPCC_PLANT_EVENTS].[BLOCK]+'</option>'
,
EVENTDDL= '<option value='''+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[EVENT]  as varchar(200)) +'''>'+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[EVENT]  as varchar(200)) +'</option>'
,
REASONDDL= '<option value='''+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[REASON]  as varchar(200)) +'''>'+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[REASON]  as varchar(200)) +'</option>'
,
TYPEOFOUTAGE= '<option value='''+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[TYPE_OF_OUTAGE]  as varchar(200)) +'''>'+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[TYPE_OF_OUTAGE]  as varchar(200)) +'</option>'
,
STATUSOFMACHINE= '<option value='''+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[STATUS_OF_MACHINE]  as varchar(200)) +'''>'+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[STATUS_OF_MACHINE]  as varchar(200)) +'</option>'
 ,FORMAT([NPCC_ACK_DATE_TIME],'dd-MMM-yyyy HH:mm') AS  [NPCC_ACK_DATE_TIME] 
                                             ,[NPCC_RECEIVER_NAME]
 ,FORMAT([NPCC_EVENT_TIME],'dd-MMM-yyyy HH:mm') AS  [NPCC_EVENT_TIME] 

                                             ,[NPCC_RECEIVER_DESIGNATION]
                                             ,[NPCC_REMARKS] as NPCC_REMARKS_FOR_IPP
                                             ,[NPCC_REMARKS_ONLY_NPCC],
NPCC_TYPE_OF_OUTAGEDDL= '<option value='''+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[NPCC_TYPE_OF_OUTAGE]  as varchar(200)) +'''>'+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[NPCC_TYPE_OF_OUTAGE]  as varchar(200)) +'</option>'
,NPCC_REASONDDL= '<option value='''+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[NPCC_REASON]  as varchar(200)) +'''>'+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[NPCC_REASON]  as varchar(200)) +'</option>'
,NPCC_STATUS_OF_MACHINE= '<option value='''+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[NPCC_STATUS_OF_MACHINE]  as varchar(200)) +'''>'+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[NPCC_STATUS_OF_MACHINE]  as varchar(200)) +'</option>'
,NPCC_EVENT= '<option value='''+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[NPCC_EVENT]  as varchar(200)) +'''>'+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[NPCC_EVENT]  as varchar(200)) +'</option>'
                                           
                                             
                                             ,[EVENT_VERIFIED]
,FUELDDL= '<option value='''+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[FUEL]  as varchar(200)) +'''>'+CAST([CDXP].[WP_NPCC_PLANT_EVENTS].[FUEL]  as varchar(200)) +'</option>'

  FROM [CDXP].[WP_NPCC_PLANT_EVENTS] WHERE [WP_NPCC_PLANT_EVENTS_ID_PK]='" + dataID[1] + @"'");



                        break;

                    case 180:

                        Returnvls = Fn.Data2Json(@"EXEC NPCC_Complex_Unit " + dataID[1] + "");

                        break;

                    case 181:

                        Returnvls = Fn.Data2Json(@"EXEC NPCC_Complex_Unit_FORInsert " + dataID[1] + "");

                        break;
                    case 19: // Current Available Capacity for 1994 Policy

                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            decimal VENDOR_SITE_ID_FK0 = Convert.ToDecimal(dataID[1]);

                            Returnvls =
                           db.WP_GC_HOURLY_DATA_HEADER
                           .Where(hdr => /*hdr.STATUS == "Acknowledged" &&*/ hdr.VENDOR_SITE_ID_FK == VENDOR_SITE_ID_FK0)
                           .OrderByDescending(x => x.CREATION_DATE)
                                .Select(x => x.CURRENT_AVAILABILITY).FirstOrDefault().ToString();
                        }

                        break;
                    case 190: // Current Available Capacity for DI new procedure

                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {

                            Returnvls = Fn.Data2Json(@"EXEC dbo.Current_Availability_DI " + dataID[1] + ",'" + dataID[2] + "'");
                        }

                        break;
                    case 191: // Complexes for DI new procedure

                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {

                            Returnvls = Fn.Data2DropdownSQL(@"SELECT
  CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID AS id
 ,CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE+' - '+CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS Complex_Title
FROM CDXP.CPPA_PPA_PLT_BLK_FUEL
INNER JOIN CDXP.PPA_HEADER
  ON CDXP.CPPA_PPA_PLT_BLK_FUEL.HEADER_ID_FK = CDXP.PPA_HEADER.HEADER_ID_PK
WHERE CDXP.PPA_HEADER.VENDOR_SITE_ID_FK = '" + dataID[1] + "' AND CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE = 'Complex'");
                        }

                        break;
                    case 20: // Current Available Capacity For 2002 + 2006 etc..

                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            decimal VENDOR_SITE_ID_FK0 = Convert.ToDecimal(dataID[1]);


                            var data = db.WP_GC_HOURLY_DATA_HEADER
                           .Where(hdr => /*hdr.STATUS == "Acknowledged" &&*/ hdr.VENDOR_SITE_ID_FK == VENDOR_SITE_ID_FK0)
                           .OrderByDescending(x => x.CREATION_DATE).FirstOrDefault();
                            var getHour = data.INTIMATION_DATE_TIME.Value.Hour + ":00";
                            var dataId = data.WP_GC_HOURLY_DATA_HEADER_ID_PK;

                            var getAvailability = db.WP_GC_HOURLY_DATA_DETAIL
                                .Where(x => x.WP_GC_HOURLY_DATA_HEADER_ID_FK == dataId && x.TARGET_HOUR == getHour)
                                .Select(x => x.AVAILABILITY).FirstOrDefault();
                            Returnvls = getAvailability.ToString();
                        }

                        break;
                    case 21:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT CDXP.WP_NPCC_DESPATCH_HEADER.WP_NPCC_DESPATCH_HEADER_ID,'' [Sr#],FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') [Notification Date & Time] , [CDXP].[AP_SUPPLIERS].VENDOR_NAME [Power Producer], 
                        [CDXP].[AP_SUPPLIER_SITE_ALL].ADDRESS_LINE1 [Producer Site],
                        [Block / Complex / Unit] = CASE WHEN ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].ATTRIBUTE4,'') = '' THEN ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].BLOCK_UNIT_TITLE,'') ELSE
                        ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].BLOCK_UNIT_TITLE,'') + ' (' + ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].ATTRIBUTE4,'') + ')' END, 
                         TARGET_DEMAND [Target Demand (MW)],
                        FORMAT(TARGET_DATE_TIME,'dd-MMM-yyyy HH:mm') [Target Date & Time] ,
                        FORMAT([GC_COMP_ACHIEVE_DATE_TIME],'dd-MMM-yyyy HH:mm') [Achievement Date & Time] ,
                        STATUS
                        FROM CDXP.WP_NPCC_DESPATCH_HEADER
						inner join [CDXP].[AP_SUPPLIER_SITE_ALL] on CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_SITE_ID= [CDXP].[AP_SUPPLIER_SITE_ALL] .[VENDOR_SITE_ID]
						inner join [CDXP].[AP_SUPPLIERS] on [CDXP].[AP_SUPPLIER_SITE_ALL].vendor_id = [CDXP].[AP_SUPPLIERS].VENDOR_ID
						INNER JOIN [CDXP].[CPPA_PPA_PLT_BLK_FUEL] ON  [CDXP].[CPPA_PPA_PLT_BLK_FUEL].PLT_BLK_FUEL_ID =CDXP.WP_NPCC_DESPATCH_HEADER.BLOCK_ID 

                        WHERE CDXP.WP_NPCC_DESPATCH_HEADER.STATUS = 'Acknowledged' ", "tblJ1");

                        break;
                    case 22:    //LOAD CURTAILMENT
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"Select  CDXP.WP_NPCC_LOAD_CURTAILMENT.WP_NPCC_LOAD_CURTAILMENT_ID_PK,
                                    WP_NPCC_LOAD_CURTAILMENT_ID_PK [Curtailment No],
                                    GENERATION_COMPANY [Power Producer],
                                    FORMAT(NOTIFICATION_TIME, 'dd-MMM-yyyy HH:mm') AS [Notification Date & Time],
                                    INSTRUCTION_TYPE [Instruction Type],
                                    FORMAT(ACTUAL_ACHIEVEMENT_TIME, 'dd-MMM-yyyy HH:mm') AS [Achievement Date & Time],
                                    UNAVAILABLE_DUE_TO_FO [Unavilable Due to FO (MW)],
                                    UNAVAILABLE_DUE_TO_SO [Unavilable Due to SO (MW)],
                                    PRIOR_TO_CURTAILMENT [Prior to Curtailment],
                                    AFTER_CURTAILMENT [After Curtailment],
                                    WTG_PRIOR_TO_CURTAILMENT [WTG/Radiation Prior to Curtailment],
                                    WTG_AFTER_CURTAILMENT [WTG/Radiation After Curtailment],
                                    STATUS
                                    
                        FROM CDXP.WP_NPCC_LOAD_CURTAILMENT JOIN CDXP.AP_SUPPLIERS 
                        ON 
                          CDXP.WP_NPCC_LOAD_CURTAILMENT.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID
                        JOIN
                          CDXP.WP_GC_USER_ACCESS 
                        ON
                          CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID
                        
                          WHERE (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')
                                        
                        ORDER BY [WP_NPCC_LOAD_CURTAILMENT_ID_PK] DESC", "tblJ1");

                        break;
                    case 23:
                        decimal Id = Convert.ToDecimal(dataID[1]);
                        Returnvls = Fn.Data2Json(@"Select CDXP.WP_NPCC_FAILURE_NOTIFICATION.WP_NPCC_FAILURE_NOTIFICATION_ID_PK,''[Sr#],IPP_CAT, 
                        FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') [Notification Date & Time], COMPLAINCEMET,
                        GENERATION_COMPANY [POWERPRODUCER], SITE [SITE], BLOCK [BLOCK], TARGET_DEMAND [TARGET_DEMAND], FUEL, NOTIFICATION_COUNT,
                        SENDER_NAME, SENDER_DESIGNATION, INSTALLED_CAPACITY,FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') AS INTIMATION_TIME,
                        SUPPLY, DEMAND_NO [DEMAND_NO], STATUS, NPCC_REMARKS, GC_AckBy, GC_DESIGNATION, GC_REMARKS
                        ,FORMAT(CREATED_ON,'dd-MMM-yyyy HH:mm') CREATED_ON ,COMPLIANCE_TYPE,COMPLIANCED_MW,FORMAT(COMPLIANCED_ACHIEVEMENT_TIME,'dd-MMM-yyyy HH:mm')  [COMPLIANCED_ACHIEVEMENT_TIME]

                        FROM CDXP.WP_NPCC_FAILURE_NOTIFICATION
                        WHERE CDXP.WP_NPCC_FAILURE_NOTIFICATION.WP_NPCC_FAILURE_NOTIFICATION_ID_PK = '" + Id + @"'");
                        break;

                    case 24:

                        if (dataID[1] == "0")
                        {


                        }
                        else
                        {
                            var Status = dataID[2];
                            var recordId = dataID[1];
                            if (Status == "Acknowledged")
                            {
                                Fn.Exec(@" UPDATE[CDXP].[WP_NPCC_FAILURE_NOTIFICATION] SET 
                                            [STATUS] = '" + Status + @"',
                                             GC_AckBy='" + dataID[17] + @"',
                                      GC_DESIGNATION ='" + dataID[18] + @"',
                                          GC_REMARKS ='" + dataID[19] + @"',
                                       COMPLAINCEMET = '" + dataID[22] + @"'
                                             WHERE WP_NPCC_FAILURE_NOTIFICATION_ID_PK = '" + recordId + "'");
                            }
                            Returnvls = dataID[1];

                        }

                        break;

                    case 25: //DASHBOARD FETCH VALUE
                        Returnvls = Fn.Exec("");
                        break;





                    case 26: // Current Available Capacity for all policies

                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            var POWER_POLICIY = dataID[1];

                            var lst =
                           db.WP_GC_HOURLY_DATA_HEADER
                           .Where(hdr => /*hdr.STATUS == "Acknowledged" &&*/ hdr.POWER_POLICIY == POWER_POLICIY)
                           .Sum(x => x.CURRENT_AVAILABILITY);

                            //.OrderByDescending(x => x.CREATION_DATE)
                            //     .Select(x => x.CURRENT_AVAILABILITY)
                            //     .ToList();

                            if (lst == null)
                            {
                                lst = 000;
                            }

                            decimal? sum = 0.00M;

                            //for (int i = 0; i <= lst.Count; i++)
                            //{
                            //    sum =+ lst[i];
                            //}
                            sum = lst;

                            Returnvls = sum.ToString();
                        }

                        break;

                    case 27: // Current Available Capacity for all Fuels
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            var Fuel = dataID[1];

                            var lst1 =
                           db.CPPA_PPA_PLT_BLK_FUEL
                           .Where(hdr => hdr.FUEL_LOOKUP_CODE == Fuel)
                           .Select(x => x.PLT_BLK_FUEL_ID).ToList();

                            decimal? lst = 0.00M;
                            foreach (var item in lst1)
                            {
                                lst =
                                db.WP_GC_HOURLY_DATA_HEADER
                                .Where(hdr => hdr.PLT_BLK_FUEL_ID == item)
                                .Sum(x => x.CURRENT_AVAILABILITY);
                            }
                            if (lst == null)
                            {
                                lst = 000;
                            }

                            Returnvls = lst.ToString();
                        }

                        break;
                    case 28:
                        decimal case28Id = Convert.ToDecimal(dataID[1]);
                        Returnvls = Fn.Data2Json(@"Select CDXP.WP_NPCC_FAILURE_NOTIFICATION.WP_NPCC_FAILURE_NOTIFICATION_ID_PK,''[Sr#], 
                        FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') [Notification Date & Time], 
                        GENERATION_COMPANY [POWERPRODUCER], SITE [SITE], BLOCK [BLOCK], TARGET_DEMAND [TARGET_DEMAND], FUEL, NOTIFICATION_COUNT,
                        SENDER_NAME, SENDER_DESIGNATION, INSTALLED_CAPACITY,FORMAT(INTIMATION_TIME,'dd-MMM-yyyy HH:mm') AS INTIMATION_TIME,
                        SUPPLY, DEMAND_NO [DEMAND_NO], STATUS, NPCC_REMARKS, GC_AckBy, GC_DESIGNATION, GC_REMARKS 
                        FROM CDXP.WP_NPCC_FAILURE_NOTIFICATION
                        WHERE CDXP.WP_NPCC_FAILURE_NOTIFICATION.WP_NPCC_FAILURE_NOTIFICATION_ID_PK = '" + case28Id + @"'");

                        break;
                    case 29:
                        decimal case29Id = Convert.ToDecimal(dataID[1]);
                        Returnvls = Fn.Data2Json(@"SELECT [WP_NPCC_LOAD_CURTAILMENT_ID_PK],[VENDOR_ID],[GENERATION_COMPANY],
                        [TOTAL_WTG],[AVAILABLE_WTG],[UNAVAILABLE_DUE_TO_FO],[UNAVAILABLE_DUE_TO_SO],[INSTRUCTION_TYPE],[WTG_CURTAILMENT],[LOAD_CURTAILMENT],[TO_CURTAILMENT], [CURTAILMENT_DETAIL],FORMAT(NOTIFICATION_TIME,'dd-MMM-yyyy HH:mm') [NOTIFICATION_TIME],FORMAT(SYNC_DYSYNC_TIME,'dd-MMM-yyyy HH:mm') [SYNC/DESYNC TIME],[IPPTYPE],
                        FORMAT(ACTUAL_ACHIEVEMENT_TIME,'dd-MMM-yyyy HH:mm') [ACTUAL_ACHIEVEMENT_TIME],[PRIOR_TO_CURTAILMENT],[AFTER_CURTAILMENT],[WTG_PRIOR_TO_CURTAILMENT],
                        [WTG_AFTER_CURTAILMENT],[SENDER_NAME],[SENDER_DESIGNATION],[NPCC_REMARKS],[RECEIVER_NAME],[RECEIVER_DESIGNATION],[GC_REMARKS],[CREATED_BY],
                        [CREATED_ON],[MODIFIED_BY],[MODIFIED_ON],[SITE],[STATUS],[ACH_REMARKS],WndRad_BEFORE,WndRad_AFTER  FROM [CDXP].[WP_NPCC_LOAD_CURTAILMENT]  
                        WHERE [CDXP].[WP_NPCC_LOAD_CURTAILMENT].[WP_NPCC_LOAD_CURTAILMENT_ID_PK] =" + case29Id + ";");
                        break;
                    case 30:
                        try
                        {
                            string updateLordCurtailment = @"UPDATE [CDXP].[WP_NPCC_LOAD_CURTAILMENT]" +
                                               @"SET [RECEIVER_NAME] = '" + dataID[1] +
                                                  @"',[RECEIVER_DESIGNATION] = '" + dataID[2] +
                                                  @"',[GC_REMARKS] = '" + dataID[3] +
                                                  @"',[MODIFIED_BY] = '" + dataID[1] +
                                                  @"',[MODIFIED_ON] = '" + serverDate +
                                                  @"',[STATUS] = '" + dataID[4] +
                                                  //@"',[TOTAL_WTG] = " + dataID[6] +
                                                  @"',[AVAILABLE_WTG] = " + dataID[7] +
                                                  @",[UNAVAILABLE_DUE_TO_FO] = " + dataID[8] +
                                                  @",[UNAVAILABLE_DUE_TO_SO] = " + dataID[9] +
                                                  // @",[SYNC_DYSYNC_TIME] = '" + dataID[10] +
                                                  @",[ACTUAL_ACHIEVEMENT_TIME] = '" + dataID[10] +
                                                  @"',[PRIOR_TO_CURTAILMENT] = " + dataID[11] +
                                                  @",[AFTER_CURTAILMENT] = " + dataID[12] +
                                                  @",[WTG_PRIOR_TO_CURTAILMENT] = " + dataID[13] +
                                                  @",[WTG_AFTER_CURTAILMENT] = " + dataID[14] +
                                                  @",[WndRad_BEFORE] = " + dataID[15] +
                                                  @",[WndRad_AFTER] = " + dataID[16] +

                                            @" WHERE[CDXP].[WP_NPCC_LOAD_CURTAILMENT].[WP_NPCC_LOAD_CURTAILMENT_ID_PK] = " + dataID[5];

                            //string sstr = @"INSERT INTO CDXP.WP_NPCC_LOAD_CURTAILMENT
                            //              (VENDOR_ID, GENERATION_COMPANY, TOTAL_WTG, AVAILABLE_WTG, UNAVAILABLE_DUE_TO_FO, UNAVAILABLE_DUE_TO_SO, INSTRUCTION_TYPE, WTG_CURTAILMENT, LOAD_CURTAILMENT, CURTAILMENT_DETAIL, NOTIFICATION_TIME,SYNC_DYSYNC_TIME,ACTUAL_ACHIEVEMENT_TIME,PRIOR_TO_CURTAILMENT,AFTER_CURTAILMENT,WTG_PRIOR_TO_CURTAILMENT,WTG_AFTER_CURTAILMENT,SENDER_NAME,SENDER_DESIGNATION, NPCC_REMARKS, SITE, CREATED_BY, CREATED_ON, STATUS)
                            //              VALUES        ('" + dataID[1] + @"','" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"','" + dataID[7] + @"','" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"','" + dataID[11] + @"','" + dataID[12] + @"','" + dataID[13] + @"','" + dataID[14] + @"','" + dataID[15] + @"','" + dataID[16] + @"','" + dataID[17] + @"','" + dataID[18] + @"','" + dataID[19] + @"','" + dataID[20] + @"', '" + dataID[21] + @"', '" + Convert.ToString(Session["UserName"]) + @"','" + serverDate + @"','" + dataID[22] + @"');  select SCOPE_IDENTITY();";
                            Returnvls = Fn.Exec(updateLordCurtailment);
                        }
                        catch (Exception ex)
                        {

                            string st = ex.Message;
                        }
                        break;
                    case 31:
                        try
                        {
                            string updateLordCurtailment = @"UPDATE [CDXP].[WP_NPCC_LOAD_CURTAILMENT] SET" +
                                                  @"[MODIFIED_ON] = '" + serverDate +
                                                  @"',[STATUS] = '" + dataID[1] +
                                                  @"',[ACH_REMARKS] = '" + dataID[2] +
                                                  @"' WHERE[CDXP].[WP_NPCC_LOAD_CURTAILMENT].[WP_NPCC_LOAD_CURTAILMENT_ID_PK] = '" + dataID[3] + "'";

                            Returnvls = Fn.Exec(updateLordCurtailment);
                        }
                        catch (Exception ex)
                        {

                            string st = ex.Message;
                        }
                        break;
                    case 32:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT CDXP.WP_NPCC_DESPATCH_HEADER.WP_NPCC_DESPATCH_HEADER_ID,'' [Sr#],CDXP.WP_NPCC_PLANT_EVENTS.CREATED_ON [Date], CDXP.WP_NPCC_PLANT_EVENTS.CREATED_ON  [Hours], CDXP.WP_NPCC_PLANT_EVENTS.AVAILABILITY as [Availability], FORMAT(CDXP.WP_NPCC_PLANT_EVENTS.INTIMATION_TIME ,'dd-MMM-yyyy HH:mm') [Intimation Time], FORMAT(CDXP.WP_NPCC_PLANT_EVENTS.EVENT_TIME,'dd-MMM-yyyy HH:mm') [Event Time],CDXP.WP_NPCC_PLANT_EVENTS.EVENT [Event Type],CDXP.WP_NPCC_PLANT_EVENTS.TYPE_OF_OUTAGE [Type of Outage], CDXP.WP_NPCC_PLANT_EVENTS.REASON [Reason], FORMAT(CDXP.WP_NPCC_DESPATCH_HEADER.INTIMATION_TIME,'dd-MMM-yyyy HH:mm') [Request Time], FORMAT(CDXP.WP_NPCC_DESPATCH_HEADER.TARGET_DATE_TIME,'dd-MMM-yyyy HH:mm') [Target Time], NULL AS [Bar], NULL [StandBy], CDXP.WP_NPCC_PLANT_EVENTS.TYPE_OF_OUTAGE [Forced Outage], CDXP.WP_NPCC_PLANT_EVENTS.TYPE_OF_OUTAGE [Scheduled Outage], CDXP.WP_NPCC_DESPATCH_HEADER.TARGET_DEMAND [Demand], CDXP.WP_NPCC_DESPATCH_HEADER.GC_COMP_TARGET_ACHIEVED [Achieved] FROM CDXP.WP_NPCC_PLANT_EVENTS INNER JOIN CDXP.WP_NPCC_DESPATCH_HEADER ON CDXP.WP_NPCC_PLANT_EVENTS.PLT_BLK_FUEL_ID = CDXP.WP_NPCC_DESPATCH_HEADER.BLOCK_ID WHERE  (" + dataID[1] + "= -1 OR CDXP.WP_NPCC_PLANT_EVENTS.VENDOR_ID=" + dataID[1] + ") AND (" +
                             dataID[2] + "= -1 OR CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_SITE_ID=" +
                             dataID[2] + ") AND (" + dataID[3] + "=-1 OR CDXP.WP_NPCC_DESPATCH_HEADER.BLOCK_ID=" +
                             dataID[3] + ") AND ('" + dataID[4] + "'='-1' OR CAST(CDXP.WP_NPCC_PLANT_EVENTS.EVENT_TIME as date)= '" +
                             dataID[4] + "')", "tblJ1");
                        break;
                    case 33:
                        Fn.Exec("EXEC [CDXP].[SP_DEL_DRAFT_DAC_OLD]");
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT  TOP(5)      CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.WP_GC_HOURLY_DATA_HEADER_ID_PK, 0) AS VARCHAR(10)) + '½' + CAST(ISNULL(CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS, '') AS VARCHAR(100)) 
                         + '½' + CASE WHEN CDXP.PPA_HEADER.POWER_POLICY = '2002' OR
                         CDXP.PPA_HEADER.POWER_POLICY = '2015' THEN 'HourlyYes' ELSE 'No' END + '½' + CAST(CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE AS VARCHAR(100)) AS ID, '' AS Sr#, 
                         CDXP.AP_SUPPLIERS.VENDOR_NAME AS [Vendor Name], ISNULL(CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_CODE, '') + ' - ' + ISNULL(CDXP.AP_SUPPLIER_SITE_ALL.ADDRESS_LINE1, '') AS [Vendor Site], 
                         CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE AS Block, CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS Fuel, CDXP.WP_GC_HOURLY_DATA_HEADER.POWER_POLICIY AS Policy, 
                         CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE AS Category, CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS AS Status, FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.TARGET_DATE, 
                         'dd-MMM-yyyy') AS [Target Date], FORMAT(CDXP.WP_GC_HOURLY_DATA_HEADER.INTIMATION_DATE_TIME, 'dd-MMM-yyyy HH:mm') AS [Intimation Time]
FROM            CDXP.AP_SUPPLIER_SITE_ALL INNER JOIN
                         CDXP.PPA_HEADER ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CDXP.PPA_HEADER.VENDOR_SITE_ID_FK AND 
                         CDXP.PPA_HEADER.VENDOR_ID_FK = CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_ID INNER JOIN
                         CDXP.AP_SUPPLIERS ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_ID = CDXP.AP_SUPPLIERS.VENDOR_ID INNER JOIN
                         CDXP.WP_GC_HOURLY_DATA_HEADER ON CDXP.AP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK INNER JOIN
                         CDXP.CPPA_PPA_PLT_BLK_FUEL ON CDXP.WP_GC_HOURLY_DATA_HEADER.PLT_BLK_FUEL_ID = CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID INNER JOIN
                         CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND 
                         CDXP.WP_GC_HOURLY_DATA_HEADER.VENDOR_SITE_ID_FK = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID
WHERE        (CDXP.WP_GC_HOURLY_DATA_HEADER.HOURLY_DATA_TYPE IN ('DAC', 'RDAC', 'ADAC')) AND (CDXP.WP_GC_HOURLY_DATA_HEADER.STATUS <> '" + dataID[1] + @"') AND 
                         (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')
ORDER BY CDXP.WP_GC_HOURLY_DATA_HEADER.LAST_UPDATE_DATE DESC", "tblDB1");
                        break;

                    case 34:

                        //if (dataID[0] == "")
                        //{

                        //}
                        //else
                        //{
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT TOP (5) CDXP.WP_NPCC_DESPATCH_HEADER.WP_NPCC_DESPATCH_HEADER_ID,'' [Sr#],INTIMATION_TIME [Notification Date & Time] , [CDXP].[AP_SUPPLIERS].VENDOR_NAME [Power Producer],
                        [CDXP].[AP_SUPPLIER_SITE_ALL].ADDRESS_LINE1 [Producer Site],
                        [Block / Complex / Unit] = CASE WHEN ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].ATTRIBUTE4,'') = '' THEN ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].BLOCK_UNIT_TITLE,'') ELSE
                        ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].BLOCK_UNIT_TITLE,'') + ' (' + ISNULL([CDXP].[CPPA_PPA_PLT_BLK_FUEL].ATTRIBUTE4,'') + ')' END,
                         TARGET_DEMAND [Target Demand (MW)],
                        TARGET_DATE_TIME [Target Date & Time] ,
                        FORMAT([GC_COMP_ACHIEVE_DATE_TIME],'dd-MMM-yyyy HH:mm') [Achievement Date & Time] ,
                        STATUS
                        FROM CDXP.WP_NPCC_DESPATCH_HEADER
                                    inner join [CDXP].[AP_SUPPLIER_SITE_ALL] on CDXP.WP_NPCC_DESPATCH_HEADER.VENDOR_SITE_ID= [CDXP].[AP_SUPPLIER_SITE_ALL] .[VENDOR_SITE_ID]
                                    inner join [CDXP].[AP_SUPPLIERS] on [CDXP].[AP_SUPPLIER_SITE_ALL].vendor_id = [CDXP].[AP_SUPPLIERS].VENDOR_ID
                                    INNER JOIN [CDXP].[CPPA_PPA_PLT_BLK_FUEL] ON  [CDXP].[CPPA_PPA_PLT_BLK_FUEL].PLT_BLK_FUEL_ID =CDXP.WP_NPCC_DESPATCH_HEADER.BLOCK_ID ", "tblDB2");
                        //}
                        break;
                    case 35:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT TOP(5) [CDXP].[WP_NPCC_PLANT_EVENTS].[WP_NPCC_PLANT_EVENTS_ID_PK],'' [Sr#] 
                            ,[GENERATION_COMPANY],[BLOCK],[FUEL],FORMAT([EVENT_TIME],'dd-MMM-yyyy HH:mm') AS[Event Time]
      ,[EVENT] ,[REASON],[STATUS],[EVENT_VERIFIED] AS[VERIFIED EVENT],[NPCC_REASON] AS[NPCC REASON]
        FROM[CDXP].[WP_NPCC_PLANT_EVENTS]", "tblDB3");

                        break;
                    case 36:    //LOAD CURTAILMENT
                        string s369 = "";
                        if (dataID[9].Trim() != "")
                        {
                            s369 = " AND (CDXP.WP_NPCC_LOAD_CURTAILMENT.WTG_PRIOR_TO_CURTAILMENT = '" + dataID[9] + @"') ";
                        }
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"Select  CDXP.WP_NPCC_LOAD_CURTAILMENT.WP_NPCC_LOAD_CURTAILMENT_ID_PK,
                                    WP_NPCC_LOAD_CURTAILMENT_ID_PK [Curtailment No],
                                    GENERATION_COMPANY [Power Producer],
                                    NOTIFICATION_TIME [Notification Date & Time],
                                    INSTRUCTION_TYPE [Instruction Type],
                                    ACTUAL_ACHIEVEMENT_TIME [Achievement Date & Time],
                                    UNAVAILABLE_DUE_TO_FO [Unavilable Due to FO],
                                    UNAVAILABLE_DUE_TO_SO [Unavilable Due to SO],
                                    PRIOR_TO_CURTAILMENT [Prior to Curtailment],
                                    AFTER_CURTAILMENT [After Curtailment],
                                    WTG_PRIOR_TO_CURTAILMENT [WTG Prior to Curtailment],
                                    WTG_AFTER_CURTAILMENT [WTG After Curtailment],
                                    STATUS
                        FROM CDXP.WP_NPCC_LOAD_CURTAILMENT 
						 WHERE (CDXP.WP_NPCC_LOAD_CURTAILMENT.VENDOR_ID = '" + dataID[1] + @"') AND (CDXP.WP_NPCC_LOAD_CURTAILMENT.VENDOR_SITE_ID = '" + dataID[2] + @"')  AND  (CDXP.WP_NPCC_LOAD_CURTAILMENT.INSTRUCTION_TYPE like '%" + dataID[8] + @"%') 
						 " + s369 + @"
						 AND (CDXP.WP_NPCC_LOAD_CURTAILMENT.NOTIFICATION_TIME between '" + dataID[10] + @"' and '" + dataID[11] + @"') 
						ORDER BY [WP_NPCC_LOAD_CURTAILMENT_ID_PK] DESC", "tblJ1");
                        break;
                    case 37:
                        DataTable dt37 = Fn.FillDSet(
                            @"SELECT        CDXP.CPPA_PPA_DEF_SCH_H.def_sch_hid_pk,  CDXP.CPPA_PPA_DEF_SCH_LINES.column_seq column_seq
FROM            CDXP.CPPA_PPA_DEF_SCH_H INNER JOIN
                         CDXP.PPA_HEADER ON CDXP.CPPA_PPA_DEF_SCH_H.header_id_fk = CDXP.PPA_HEADER.HEADER_ID_PK
						 INNER JOIN CDXP.CPPA_PPA_DEF_SCH_LINES ON CDXP.CPPA_PPA_DEF_SCH_LINES.def_sch_hid_fk=CDXP.CPPA_PPA_DEF_SCH_H.def_sch_hid_pk
WHERE        (CDXP.CPPA_PPA_DEF_SCH_H.sch_lookup_code = '110')  AND CDXP.PPA_HEADER.APPROVAL_STATUS= 'Approved' AND CDXP.PPA_HEADER.VENDOR_ID_FK = 1003 AND CDXP.PPA_HEADER.vendor_site_id_fk=1003
AND (CDXP.CPPA_PPA_DEF_SCH_LINES.column_name = 520 OR CDXP.CPPA_PPA_DEF_SCH_LINES.column_name = 550)").Tables[0];
                        string whr37 = "";
                        string cols37 = "";
                        for (int i = 0; i < dt37.Rows.Count; i++)
                        {
                            whr37 = Convert.ToString(dt37.Rows[i][0]);
                            cols37 += cols37 == "" ? "column" + Convert.ToString(dt37.Rows[i][1]) : ", column" + Convert.ToString(dt37.Rows[i][1]);
                        }

                        break;

                    case 38:
                        //[CDXP].[SP_SCHEDULE10]
                        Returnvls = Fn.ExenID(@"EXEC [CDXP].[SP_SCHEDULE10] " + dataID[1] + ", " + dataID[2] + ", '" + dataID[3] + "', '" + dataID[4] + "'");
                        break;

                    case 39:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            double VENDOR_ID_FK0 = Convert.ToDouble(dataID[1]);
                            double VENDOR_SITE_ID_FK0 = Convert.ToDouble(dataID[2]);
                            double Block = Convert.ToDouble(dataID[3]);

                            // Returnvls = Fn.Data2Dropdown(
                            //db.PPA_HEADER.Where(hdr => hdr.APPROVAL_STATUS == "Approved" && hdr.VENDOR_SITE_ID_FK == VENDOR_SITE_ID_FK0 && hdr.VENDOR_ID_FK == VENDOR_ID_FK0)
                            //     .Join(db.CPPA_PPA_PLT_BLK_FUEL,
                            //     hdr => hdr.HEADER_ID_PK,
                            //     fuel => fuel.HEADER_ID_FK,
                            //     (Header, Fuel) => new { id = Fuel.PLT_BLK_FUEL_ID + "½" + Fuel.FUEL_LOOKUP_CODE + "½" + Header.CONTRACTED_CAPACITY + "½" + Header.POWER_POLICY + "½" + Fuel.ATTRIBUTE5, name = Fuel.BLOCK_UNIT_TITLE + " (" + Fuel.ATTRIBUTE4 + ")" }).ToList());

                            Returnvls = Fn.Data2Json(@"	SELECT  CAST(CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID AS VARCHAR(100)) as id, CAST(CDXP.PPA_HEADER.POWER_POLICY AS VARCHAR(100)) 
                            as power_policy, 
	                        CAST(CDXP.PPA_HEADER.DEPENDABLE_CAPACITY AS VARCHAR(100)) AS complex_dependable_capacity,
	                        CAST(CDXP.CPPA_PPA_PLT_BLK_FUEL.CAPACITY AS VARCHAR(100)) as installed_capacity, CAST(CDXP.CPPA_PPA_PLT_BLK_FUEL.CAPACITY AS VARCHAR(100))   
	                        as dependable_capacity,CAST(CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS VARCHAR(100)) as fuel 
                       
	                        FROM  CDXP.CPPA_PPA_PLT_BLK_FUEL INNER JOIN
                            CDXP.PPA_HEADER ON CDXP.CPPA_PPA_PLT_BLK_FUEL.HEADER_ID_FK = CDXP.PPA_HEADER.HEADER_ID_PK   
                            WHERE PPA_HEADER.APPROVAL_STATUS= 'Approved' AND  VENDOR_SITE_ID_FK=" + VENDOR_SITE_ID_FK0 + @" and PLT_BLK_FUEL_ID=" + Block + @"");

                            break;
                        }
                    case 40:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            double VENDOR_ID_FK0 = Convert.ToDouble(dataID[1]);
                            double VENDOR_SITE_ID_FK0 = Convert.ToDouble(dataID[2]);
                            //double Block = Convert.ToDouble(dataID[3]);

                            // Returnvls = Fn.Data2Dropdown(
                            //db.PPA_HEADER.Where(hdr => hdr.APPROVAL_STATUS == "Approved" && hdr.VENDOR_SITE_ID_FK == VENDOR_SITE_ID_FK0 && hdr.VENDOR_ID_FK == VENDOR_ID_FK0)
                            //     .Join(db.CPPA_PPA_PLT_BLK_FUEL,
                            //     hdr => hdr.HEADER_ID_PK,
                            //     fuel => fuel.HEADER_ID_FK,
                            //     (Header, Fuel) => new { id = Fuel.PLT_BLK_FUEL_ID + "½" + Fuel.FUEL_LOOKUP_CODE + "½" + Header.CONTRACTED_CAPACITY + "½" + Header.POWER_POLICY + "½" + Fuel.ATTRIBUTE5, name = Fuel.BLOCK_UNIT_TITLE + " (" + Fuel.ATTRIBUTE4 + ")" }).ToList());

                            Returnvls = Fn.Data2Json(@"EXEC CHECK_HRSG " + dataID[2] + "");

                            //                                @" DECLARE @a INT;
                            // DECLARE @b VARCHAR(MAX);
                            //SET @b = 'HRSG';
                            // SET  @a= (Select  CDXP.PPA_HEADER.HEADER_ID_PK AS header
                            //      FROM  CDXP.CPPA_PPA_PLT_BLK_FUEL INNER JOIN
                            //      CDXP.PPA_HEADER ON CDXP.CPPA_PPA_PLT_BLK_FUEL.HEADER_ID_FK = CDXP.PPA_HEADER.HEADER_ID_PK 
                            //      WHERE PPA_HEADER.APPROVAL_STATUS= 'Approved' AND  VENDOR_SITE_ID_FK=" + VENDOR_SITE_ID_FK0 + @" AND CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE='HRSG');
                            //  IF @a!= '' 
                            //    BEGIN
                            //    SELECT @b AS abc;
                            //    END");
                            break;
                        }



                    case 41:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            double VENDOR_ID_FK0 = Convert.ToDouble(dataID[1]);
                            double VENDOR_SITE_ID_FK0 = Convert.ToDouble(dataID[2]);
                            double Block = Convert.ToDouble(dataID[3]);

                            // Returnvls = Fn.Data2Dropdown(
                            //db.PPA_HEADER.Where(hdr => hdr.APPROVAL_STATUS == "Approved" && hdr.VENDOR_SITE_ID_FK == VENDOR_SITE_ID_FK0 && hdr.VENDOR_ID_FK == VENDOR_ID_FK0)
                            //     .Join(db.CPPA_PPA_PLT_BLK_FUEL,
                            //     hdr => hdr.HEADER_ID_PK,
                            //     fuel => fuel.HEADER_ID_FK,
                            //     (Header, Fuel) => new { id = Fuel.PLT_BLK_FUEL_ID + "½" + Fuel.FUEL_LOOKUP_CODE + "½" + Header.CONTRACTED_CAPACITY + "½" + Header.POWER_POLICY + "½" + Fuel.ATTRIBUTE5, name = Fuel.BLOCK_UNIT_TITLE + " (" + Fuel.ATTRIBUTE4 + ")" }).ToList());

                            Returnvls = Fn.Data2Json(@"EXEC HRSG_Calculated_Availability " + dataID[2] + "," + dataID[3] + "," + dataID[4] + "");


                            //                                @" DECLARE @a INT;
                            // DECLARE @b VARCHAR(MAX);
                            //SET @b = 'HRSG';
                            // SET  @a= (Select  CDXP.PPA_HEADER.HEADER_ID_PK AS header
                            //      FROM  CDXP.CPPA_PPA_PLT_BLK_FUEL INNER JOIN
                            //      CDXP.PPA_HEADER ON CDXP.CPPA_PPA_PLT_BLK_FUEL.HEADER_ID_FK = CDXP.PPA_HEADER.HEADER_ID_PK 
                            //      WHERE PPA_HEADER.APPROVAL_STATUS= 'Approved' AND  VENDOR_SITE_ID_FK=" + VENDOR_SITE_ID_FK0 + @" AND CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE='HRSG');
                            //  IF @a!= '' 
                            //    BEGIN
                            //    SELECT @b AS abc;
                            //    END");
                        }
                        break;

                    case 42:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            double VENDOR_ID_FK0 = Convert.ToDouble(dataID[1]);
                            double VENDOR_SITE_ID_FK0 = Convert.ToDouble(dataID[2]);
                            double Block = Convert.ToDouble(dataID[3]);

                            Returnvls = Fn.Data2Json(@"EXEC Calculated_Availability " + dataID[2] + "," + dataID[3] + "," + dataID[4] + "");

                        }

                        break;

                    case 43:
                        try
                        {
                            Returnvls = Fn.Data2DropdownWithAttrSQL(@"SELECT 
	                                                            [as].VENDOR_ID,
                                                                 CAST([as].VENDOR_NAME AS VARCHAR(240)) +' -- '+ CAST(ISNULL(ph.Tariff,'') as VARCHAR)  AS                             VENDOR_NAME, 
	                                                            CASE WHEN flv.FND_LOOKUP_VALUES_ID_PK = 1169 
                                                                THEN
	                                                            CAST(ph.WTGS AS VARCHAR(13)) +'½'+ CAST(ph.Cluster AS                                                            VARCHAR(30))+'½'+ CAST(ph.Tariff AS VARCHAR(30)) 
	                                                            ELSE '' 
                                                                END AS DETAILS 

                                                            FROM 
	                                                            CDXP.AP_SUPPLIERS [as] 
	                                                            JOIN 
	                                                            CDXP.PPA_HEADER ph 
                                                                ON 
	                                                            [as].VENDOR_ID = ph.VENDOR_ID_FK
	                                                            JOIN
	                                                            CDXP.FND_LOOKUP_VALUES flv
                                                                ON
	                                                            ph.IPP_CATEGORY = flv.LOOKUP_CODE
                                                            WHERE
	                                                            flv.FND_LOOKUP_VALUES_ID_PK = " + dataID[1] + " ORDER BY ph.Tariff DESC");

                        }
                        catch (Exception ex)
                        {

                            string st = ex.Message;
                        }
                        break;
                    case 44:
                        try
                        {
                            int[] cols = dataID[1].Split(',').Select(y => int.Parse(y)).ToArray();
                            DataSet ds2 = new DataSet();
                            DataTable dt2 = new DataTable();
                            foreach (int id in cols)
                            {
                                ds2 = Fn.FillDSet(@"SELECT distinct  ISNULL(ph.WTGS, 0 ) WTGS , assa.VENDOR_SITE_ID,assa.ADDRESS_LINE1,[as].VENDOR_NAME FROM CDXP.PPA_HEADER ph JOIN CDXP.AP_SUPPLIER_SITE_ALL assa ON ph.VENDOR_SITE_ID_FK = assa.VENDOR_SITE_ID JOIN CDXP.AP_SUPPLIERS [as] ON ph.VENDOR_ID_FK = [as].VENDOR_ID where ph.VENDOR_ID_FK = " + id);

                                dt2 = ds2.Tables[0];
                                string wtgs = Convert.ToString(dt2.Rows[0]["WTGS"]);
                                string site = Convert.ToString(dt2.Rows[0]["ADDRESS_LINE1"]);
                                string vendor_name = Convert.ToString(dt2.Rows[0]["VENDOR_NAME"]);
                                string vendor_site_id = Convert.ToString(dt2.Rows[0]["VENDOR_SITE_ID"]);


                                string sstr = @"INSERT INTO CDXP.WP_NPCC_LOAD_CURTAILMENT                                (VENDOR_ID,GENERATION_COMPANY,TOTAL_WTG,INSTRUCTION_TYPE,WTG_CURTAILMENT,LOAD_CURTAILMENT,CURTAILMENT_DETAIL,NOTIFICATION_TIME, SENDER_NAME, SENDER_DESIGNATION,NPCC_REMARKS, SITE, CREATED_BY, CREATED_ON, STATUS, VENDOR_SITE_ID, IPPTYPE)
                              VALUES        ('" + id + @"','" + vendor_name + @"','" + wtgs + @"','" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"','" + dataID[7] + @"','" + dataID[8] + @"', '" + dataID[9] + @"', '" + site + @"','" + Convert.ToString(Session["UserName"]) + @"','" + serverDate + @"','" + dataID[10] + @"','" + vendor_site_id + @"','" + dataID[11] + @"');  select SCOPE_IDENTITY()";

                                Returnvls = Fn.ExenID(sstr);
                                Fn.Exec(@" CDXP.SP_Notification_Submission_Record '" + Returnvls + "','6','" + id + @"','" + vendor_site_id + @"','2','" + vendor_name + @"','" + serverDate + "' ");

                            }
                        }
                        catch (Exception ex)
                        {

                            return Convert.ToString(ex.Message);
                        }
                        break;
                    case 45:
                        //[CDXP].[SP_SCHEDULE10]
                        Returnvls = Fn.ExenID(@"SELECT ISNULL(A.NetPlant_Output, 0) NetPlant_Output FROM [CDXP].[CPPA_PPA_SCH_10_DTL] A join CDXP.CPPA_PPA_SCH_10 B ON A.SCH_10_ID = B.SCH_10_ID WHERE A.SCH_10_ID =" + dataID[1] + " AND A.Ambient_Temperature =" + dataID[2]);
                        break;
                    case 707:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            var producerSiteId = Convert.ToInt32(dataID[2]);

                            Returnvls = Fn.Data2DropdownSQL(@"SELECT CAST(CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID AS VARCHAR(100)) AS id, CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE + ' ( ' + ISNULL(CDXP.CPPA_PPA_PLT_BLK_FUEL.ATTRIBUTE2, 
                            '--') +' ) ' + CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS Expr1
                            FROM CDXP.CPPA_PPA_PLT_BLK_FUEL INNER JOIN
                                                     CDXP.PPA_HEADER ON CDXP.CPPA_PPA_PLT_BLK_FUEL.HEADER_ID_FK = CDXP.PPA_HEADER.HEADER_ID_PK
                                                        WHERE PPA_HEADER.APPROVAL_STATUS = 'Approved' AND VENDOR_SITE_ID_FK = " + producerSiteId + "  order by case when CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE = 'Complex' then null else CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE end asc");
                        }
                        break;

                    case 712:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            var producerId = Convert.ToInt32(dataID[1]);

                            Returnvls = Fn.Data2DropdownSQL(@"SELECT CAST(CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID AS VARCHAR(100)) AS id, CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE + ' ( ' + ISNULL(CDXP.CPPA_PPA_PLT_BLK_FUEL.ATTRIBUTE2, 
                            '--') +' ) ' + CDXP.CPPA_PPA_PLT_BLK_FUEL.FUEL_LOOKUP_CODE AS Expr1
                            FROM CDXP.CPPA_PPA_PLT_BLK_FUEL INNER JOIN
                                                     CDXP.PPA_HEADER ON CDXP.CPPA_PPA_PLT_BLK_FUEL.HEADER_ID_FK = CDXP.PPA_HEADER.HEADER_ID_PK
                                                        WHERE PPA_HEADER.APPROVAL_STATUS = 'Approved' AND VENDOR_SITE_ID_FK = " + producerId + "  order by case when CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE = 'Complex' then null else CDXP.CPPA_PPA_PLT_BLK_FUEL.BLOCK_UNIT_TITLE end asc");
                        }
                        break;

                    case 708:
                        Returnvls = Fn.Data2DropdownSQL(@"SELECT DI_CLASSIFICATION_STATE_ID as id, STATE_NAME FROM DI_CLASSIFICATION_STATE");
                        break;

                    case 709:
                        if (dataID[1] == "0")
                        {
                            String ID709 = Fn.Exec(@"INSERT INTO [dbo].[DI_CLASSIFICATION_STARTS] (VENDOR_ID_FK, CLASSIFICATION_FORMULA, DI_CLASSIFICATION_STATE_ID_FK, VENDOR_SITE_ID_FK, PLT_BLK_FUEL_ID_FK, NTS, FULL_LOAD_TIME, GREATER_THAN_AND_EQUAL_TO_HOUR_PRESENT_STATE, LESS_THAN_HOUR_PRESENT_STATE, GENERATION_COMPANY, SITE, BLOCK, PRESENT_STATE, RAMP_RATE, LOAD_HELD, IS_HEADER, MINIMUM_LOAD_LIMIT, GREATER_THAN_AND_EQUAL_TO_RAMP_RATE_RANGE, GREATER_THAN_AND_EQUAL_TO_LOAD_HELD_RANGE, LESS_THAN_RAMP_RATE, LESS_THAN_LOAD_HELD_RANGE, FULL_LOAD_TIME_WITH_ST) VALUES('" + dataID[2] + @"', '" + dataID[3] + @"', '" + dataID[4] + @"', '" + dataID[5] + @"', '" + dataID[6] + @"', '" + dataID[7] + @"', '" + dataID[8] + @"', '" + dataID[9] + @"', '" + dataID[10] + @"', '" + dataID[11] + @"', '" + dataID[12] + @"', '" + dataID[13] + @"', '" + dataID[14] + @"', '" + dataID[15] + @"', '" + dataID[16] + @"', '" + dataID[17] + @"', '" + dataID[18] + @"', '" + dataID[19] + @"', '" + dataID[20] + @"', '" + dataID[21] + @"', '" + dataID[22] + @"', '" + dataID[23] + @"') SET ansi_warnings OFF");
                            Returnvls = ID709;
                        }
                        else
                        {
                            String ID709 = Fn.Exec(@"UPDATE [dbo].[DI_CLASSIFICATION_STARTS] SET VENDOR_ID_FK = '" + dataID[2] + @"', CLASSIFICATION_FORMULA = '" + dataID[3] + @"', DI_CLASSIFICATION_STATE_ID_FK = '" + dataID[4] + @"', VENDOR_SITE_ID_FK = '" + dataID[5] + @"', PLT_BLK_FUEL_ID_FK = '" + dataID[6] + @"', NTS = '" + dataID[7] + @"', FULL_LOAD_TIME = '" + dataID[8] + @"', GREATER_THAN_AND_EQUAL_TO_HOUR_PRESENT_STATE = '" + dataID[9] + @"', LESS_THAN_HOUR_PRESENT_STATE = '" + dataID[10] + @"', GENERATION_COMPANY = '" + dataID[11] + @"', SITE = '" + dataID[12] + @"', BLOCK = '" + dataID[13] + @"', PRESENT_STATE = '" + dataID[14] + @"', RAMP_RATE = '" + dataID[15] + @"', LOAD_HELD = '" + dataID[16] + @"', IS_HEADER = '" + dataID[17] + @"', MINIMUM_LOAD_LIMIT = '" + dataID[18] + @"', GREATER_THAN_AND_EQUAL_TO_RAMP_RATE_RANGE = '" + dataID[19] + @"', GREATER_THAN_AND_EQUAL_TO_LOAD_HELD_RANGE = '" + dataID[20] + @"', LESS_THAN_RAMP_RATE = '" + dataID[21] + @"', LESS_THAN_LOAD_HELD_RANGE = '" + dataID[22] + @"', FULL_LOAD_TIME_WITH_ST = '" + dataID[23] + @"' where DI_Classification_Starts_ID = '" + dataID[1] + @"' SET ansi_warnings OFF");
                            Returnvls = ID709;
                        }

                        break;

                    case 710:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT Top 100 [dbo].[DI_CLASSIFICATION_STARTS].[DI_Classification_Starts_ID],'' [ID#],[VENDOR_NAME] [Power Producer],
                    [CLASSIFICATION_FORMULA] [Classification Formula],[STATE_NAME] [Classification State],
                    [ADDRESS_LINE1] [Site Name],[BLOCK_UNIT_TITLE] [Unit], [NTS] [NTS Minutes],[FULL_LOAD_TIME] [Load Time],
                    [GREATER_THAN_AND_EQUAL_TO_HOUR_PRESENT_STATE][Greater/Equal To Hour State],
                    [LESS_THAN_HOUR_PRESENT_STATE] [Less Than Hour State]
                    FROM [dbo].[DI_CLASSIFICATION_STARTS]

                    INNER JOIN CDXP.AP_SUPPLIERS V ON V.VENDOR_ID = [dbo].[DI_CLASSIFICATION_STARTS].VENDOR_ID_FK
                    INNER JOIN CDXP.AP_SUPPLIER_SITE_ALL S ON S.VENDOR_SITE_ID = [dbo].[DI_CLASSIFICATION_STARTS].VENDOR_SITE_ID_FK
                    INNER JOIN CDXP.CPPA_PPA_PLT_BLK_FUEL F ON F.PLT_BLK_FUEL_ID = [dbo].[DI_CLASSIFICATION_STARTS].PLT_BLK_FUEL_ID_FK
                    INNER JOIN dbo.DI_CLASSIFICATION_STATE C ON C.DI_CLASSIFICATION_STATE_ID = [dbo].[DI_CLASSIFICATION_STARTS].DI_CLASSIFICATION_STATE_ID_FK", "tblJ1");
                        break;
                    case 711:
                        Returnvls = Fn.Data2Json(@"
                            SELECT [dbo].[DI_CLASSIFICATION_STARTS].[DI_Classification_Starts_ID],
									[VENDOR_ID_FK]
                                   ,[CLASSIFICATION_FORMULA]
                                   ,[DI_CLASSIFICATION_STATE_ID_FK]
                                   ,[VENDOR_SITE_ID_FK]
                                   ,[PLT_BLK_FUEL_ID_FK]
                                   ,[NTS]
                                   ,[FULL_LOAD_TIME]
                                   ,[GREATER_THAN_AND_EQUAL_TO_HOUR_PRESENT_STATE]
                                   ,[LESS_THAN_HOUR_PRESENT_STATE]
                                   ,[GENERATION_COMPANY]
                                   ,[SITE]
                                   ,[BLOCK]
                                   ,[PRESENT_STATE]
                                   ,[RAMP_RATE]
                                   ,[LOAD_HELD]
                                   ,[IS_HEADER]
                                   ,[MINIMUM_LOAD_LIMIT]
                                   ,[GREATER_THAN_AND_EQUAL_TO_RAMP_RATE_RANGE]
                                   ,[GREATER_THAN_AND_EQUAL_TO_LOAD_HELD_RANGE]
                                   ,[LESS_THAN_RAMP_RATE]
                                   ,[LESS_THAN_LOAD_HELD_RANGE]
                                   ,[FULL_LOAD_TIME_WITH_ST]
  FROM [dbo].[DI_CLASSIFICATION_STARTS] WHERE [DI_Classification_Starts_ID]='" + dataID[1] + @"'");
                        break;

                    case 1010:

                        string plantEventDateFrom = dataID[7].ToString();

                        string plantEventDateTo = dataID[8].ToString();

                        if (plantEventDateFrom == "" && plantEventDateTo == "")

                        {

                            Returnvls = Fn.HTMLTableWithID_TR_Tag(@"

SELECT[CDXP].[WP_NPCC_PLANT_EVENTS].[WP_NPCC_PLANT_EVENTS_ID_PK],'' [Sr#]

,FORMAT([EVENT_TIME],'dd-MMM-yyyy HH:mm') AS[Event Time],[GENERATION_COMPANY] [Power Producer],BLOCK [Unit / Complex],[FUEL],FORMAT([INTIMATION_TIME],'dd-MMM-yyyy HH:mm') [Intimation Date &Time]

,[EVENT], [TYPE_OF_OUTAGE] as [Type of Outage] ,[REASON],

REASON_DETAIL [Detail],[STATUS],FORMAT([NPCC_ACK_DATE_TIME],'dd-MMM-yyyy HH:mm') [Ack Time], [SENDER_NAME]

,[EVENT_VERIFIED] AS[VERIFIED EVENT]

FROM [CDXP].[WP_NPCC_PLANT_EVENTS]

INNER JOIN [CDXP].[AP_SUPPLIER_SITE_ALL] on [CDXP].[WP_NPCC_PLANT_EVENTS].SETUP_SITE_ID_FK= [CDXP].[AP_SUPPLIER_SITE_ALL] .[VENDOR_SITE_ID]

INNER JOIN [CDXP].[AP_SUPPLIERS] on [CDXP].[AP_SUPPLIER_SITE_ALL].vendor_id = [CDXP].[AP_SUPPLIERS].VENDOR_ID

INNER JOIN

CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND

[CDXP].[WP_NPCC_PLANT_EVENTS].SETUP_SITE_ID_FK = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID

WHERE (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')

AND [CDXP].[WP_NPCC_PLANT_EVENTS].VENDOR_ID = '" + dataID[1] + @"' AND [CDXP].[WP_NPCC_PLANT_EVENTS].SETUP_SITE_ID_FK = '" + dataID[2] + @"'

AND [CDXP].[WP_NPCC_PLANT_EVENTS].PLT_BLK_FUEL_ID = '" + dataID[3] + @"' AND [CDXP].[WP_NPCC_PLANT_EVENTS].STATUS like '%" + dataID[4] + @"%'

AND [CDXP].[WP_NPCC_PLANT_EVENTS].EVENT like '%" + dataID[5] + @"%' AND [CDXP].[WP_NPCC_PLANT_EVENTS].TYPE_OF_OUTAGE like '%" + dataID[6] + @"%' ORDER BY [CDXP].[WP_NPCC_PLANT_EVENTS].[EVENT_TIME] DESC ", "tblJ1");

                            break;

                        }

                        else

                        {

                            Returnvls = Fn.HTMLTableWithID_TR_Tag(@"

SELECT[CDXP].[WP_NPCC_PLANT_EVENTS].[WP_NPCC_PLANT_EVENTS_ID_PK],'' [Sr#]

,FORMAT([EVENT_TIME],'dd-MMM-yyyy HH:mm') AS[Event Time],[GENERATION_COMPANY] [Power Producer],BLOCK [Unit / Complex],[FUEL],FORMAT([INTIMATION_TIME],'dd-MMM-yyyy HH:mm') [Intimation Date &Time]

,[EVENT], [TYPE_OF_OUTAGE] as [Type of Outage] ,[REASON],

REASON_DETAIL [Detail],[STATUS],FORMAT([NPCC_ACK_DATE_TIME],'dd-MMM-yyyy HH:mm') [Ack Time], [SENDER_NAME]

,[EVENT_VERIFIED] AS[VERIFIED EVENT]

FROM [CDXP].[WP_NPCC_PLANT_EVENTS]

INNER JOIN [CDXP].[AP_SUPPLIER_SITE_ALL] on [CDXP].[WP_NPCC_PLANT_EVENTS].SETUP_SITE_ID_FK= [CDXP].[AP_SUPPLIER_SITE_ALL] .[VENDOR_SITE_ID]

INNER JOIN [CDXP].[AP_SUPPLIERS] on [CDXP].[AP_SUPPLIER_SITE_ALL].vendor_id = [CDXP].[AP_SUPPLIERS].VENDOR_ID

INNER JOIN

CDXP.WP_GC_USER_ACCESS ON CDXP.AP_SUPPLIERS.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID AND

[CDXP].[WP_NPCC_PLANT_EVENTS].SETUP_SITE_ID_FK = CDXP.WP_GC_USER_ACCESS.ENTITY_SITE_ID

WHERE (CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"')

AND [CDXP].[WP_NPCC_PLANT_EVENTS].VENDOR_ID = '" + dataID[1] + @"' AND [CDXP].[WP_NPCC_PLANT_EVENTS].SETUP_SITE_ID_FK = '" + dataID[2] + @"'

AND [CDXP].[WP_NPCC_PLANT_EVENTS].PLT_BLK_FUEL_ID = '" + dataID[3] + @"' AND [CDXP].[WP_NPCC_PLANT_EVENTS].STATUS like '%" + dataID[4] + @"%' AND [CDXP].[WP_NPCC_PLANT_EVENTS].INTIMATION_TIME BETWEEN '" + dataID[7] + @"' AND '" + dataID[8] + @"'

AND [CDXP].[WP_NPCC_PLANT_EVENTS].EVENT like '%" + dataID[5] + @"%' AND [CDXP].[WP_NPCC_PLANT_EVENTS].TYPE_OF_OUTAGE like '%" + dataID[6] + @"%' ORDER BY [CDXP].[WP_NPCC_PLANT_EVENTS].[EVENT_TIME] DESC ", "tblJ1");

                            break;

                        }

                    default:
                        Returnvls = " - 1";
                        break;
                }
            }
            catch (Exception ex)
            {

                return Convert.ToString(ex.Message);
            }


            return Returnvls;
        }


        [HttpPost]
        public ActionResult UploadDocument()
        {
            sessionCheck(); Load_Notifications();
            return View();
        }


        // Despatch Instruction Code - Start

        public ActionResult DespatchInstructionSpecificationList()
        {
            sessionCheck(); Load_Notifications();

            string vls = HttpUtility.UrlEncode(Fn.Data2Json("EXEC [CDXP].[SP_MENU_BY_USER_ID] '" + Session["UserName"] + "'"));
            ViewBag.LayoutStr = vls;

            var userName = Convert.ToString(Session["UserName"]);

            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == userName).FirstOrDefault();
            }

            return View();
        }


        public ActionResult NewDISpecification()
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
            }
            if (id != null)
            {
                ViewBag.tg = id;
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                }
            }
            else
            {
                ViewBag.tg = "0";
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    ViewBag.IPP = Fn.Data2Dropdown(db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME).Select(x => new { id = x.VENDOR_ID, value = x.VENDOR_NAME }).ToList());
                }
            }
            return View();
        }

        // Dispatch Instruction Code - End


        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase postedFile, System.Web.Mvc.FormCollection frm)
        {
            sessionCheck(); Load_Notifications();
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                //Create a DataTable.
                DataTable dt = new DataTable();
                if (frm["FormFor"] == "OUTAGES")
                {
                    dt.Columns.AddRange(new DataColumn[8] {
                        new DataColumn("OUTAGE_TYPE", typeof(string)),
                        new DataColumn("OUTAGE_FROMTIME",typeof(string)),
                        new DataColumn("OUTAGE_TOTIME",typeof(string)),
                        new DataColumn("AVAILABLE_CAPACITY",typeof(string)),
                        new DataColumn("REMARKS",typeof(string)),
                        new DataColumn("VENDOR_ID",typeof(Int64)),
                        new DataColumn("SITE_ID",typeof(Int64)),
                        new DataColumn("UNIT_NAME",typeof(string))
                    });
                }
                else if (frm["FormFor"] == "METERING")
                {
                    dt.Columns.AddRange(new DataColumn[3] {
                        new DataColumn("PLT_MTR_FROM_TIME", typeof(string)),
                        new DataColumn("PLT_MTR_TO_TIME",typeof(string)),
                        new DataColumn("NEO_DELIVERED",typeof(string))
                    });
                }
                else if (frm["FormFor"] == "AVAILABLITY")
                {
                    dt.Columns.AddRange(new DataColumn[2] {
                        new DataColumn("TARGET_DATE", typeof(string)),
                        new DataColumn("REVISED_AVAILABILITY",typeof(string))
                    });
                }
                else if (frm["FormFor"] == "DISPATCH")
                {
                    dt.Columns.AddRange(new DataColumn[2] {
                        new DataColumn("INTIMATION_TIME", typeof(string)),
                        new DataColumn("TARGET_DEMAND",typeof(string))
                    });
                }

                //Read the contents of CSV file.
                string csvData = System.IO.File.ReadAllText(filePath);

                //Execute a loop over the rows.
                foreach (string row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        dt.Rows.Add();
                        int i = 0;

                        //Execute a loop over the columns.
                        foreach (string cell in row.Split(','))
                        {
                            dt.Rows[dt.Rows.Count - 1][i] = cell;
                            i++;
                        }
                    }
                }

                string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {


                        if (frm["FormFor"] == "OUTAGES")
                        {
                            //Set the database table name.
                            sqlBulkCopy.DestinationTableName = "CDXP.WP_NPCC_OUTAGE_EVENTS";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table
                            sqlBulkCopy.ColumnMappings.Add("OUTAGE_TYPE", "OUTAGE_TYPE");
                            sqlBulkCopy.ColumnMappings.Add("OUTAGE_FROMTIME", "OUTAGE_FROMTIME");
                            sqlBulkCopy.ColumnMappings.Add("OUTAGE_TOTIME", "OUTAGE_TOTIME");
                            sqlBulkCopy.ColumnMappings.Add("AVAILABLE_CAPACITY", "AVAILABLE_CAPACITY");
                            sqlBulkCopy.ColumnMappings.Add("REMARKS", "REMARKS");
                            sqlBulkCopy.ColumnMappings.Add("VENDOR_ID", "VENDOR_ID");
                            sqlBulkCopy.ColumnMappings.Add("SITE_ID", "SITE_ID");
                            sqlBulkCopy.ColumnMappings.Add("UNIT_NAME", "UNIT_NAME");
                        }
                        else if (frm["FormFor"] == "METERING")
                        {
                            //Set the database table name.
                            sqlBulkCopy.DestinationTableName = "CDXP.WP_NPCC_METERING";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table
                            sqlBulkCopy.ColumnMappings.Add("PLT_MTR_FROM_TIME", "PLT_MTR_FROM_TIME");
                            sqlBulkCopy.ColumnMappings.Add("PLT_MTR_TO_TIME", "PLT_MTR_TO_TIME");
                            sqlBulkCopy.ColumnMappings.Add("NEO_DELIVERED", "NEO_DELIVERED");
                        }
                        else if (frm["FormFor"] == "AVAILABILITY")
                        {
                            //Set the database table name.
                            sqlBulkCopy.DestinationTableName = "CDXP.WP_GC_HOURLY_DATA_HEADER";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table
                            sqlBulkCopy.ColumnMappings.Add("TARGET_DATE", "TARGET_DATE");
                            sqlBulkCopy.ColumnMappings.Add("REVISED_AVAILABILITY", "REVISED_AVAILABILITY");
                        }
                        else if (frm["FormFor"] == "DISPATCH")
                        {
                            //Set the database table name.
                            sqlBulkCopy.DestinationTableName = "CDXP.WP_NPCC_DESPATCH_HEADER";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table
                            sqlBulkCopy.ColumnMappings.Add("INTIMATION_TIME", "INTIMATION_TIME");
                            sqlBulkCopy.ColumnMappings.Add("TARGET_DEMAND", "TARGET_DEMAND");
                        }

                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }
            }

            return View("UploadDocument");
        }

    }
}