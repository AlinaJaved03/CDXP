using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CDXPWeb.Models;
using Microsoft.SharePoint.Client;

namespace CDXPWeb.Controllers
{
    public class EMOController : Controller
    {
        private clsSQLCore Fn = new clsSQLCore();
        [SharePointContextFilter]
        // GET: EMO
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FuelRatesFromGC()
        {


            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {


                var thisuser = Convert.ToString(Session["UserName"]);

                var ipps = db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME)
                    .Join(db.WP_GC_USER_ACCESS,
                    s => s.VENDOR_ID,
                    a => a.ENTITY_ID,
                    (sup, acc) => new { sup.VENDOR_ID, sup.VENDOR_NAME, acc.WP_PORTAL_USERS_ID })
                    .Join(db.WP_PORTAL_USERS.Where(w => w.USER_NAME == thisuser),
                    x => x.WP_PORTAL_USERS_ID,
                    y => y.WP_PORTAL_USERS_ID,
                    (vend, user) => new { id = vend.VENDOR_ID, vend.VENDOR_NAME }).Distinct().ToList();

                ViewBag.IPP = Fn.Data2Dropdown(ipps);
                ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == thisuser).FirstOrDefault();


                var vid = ipps.FirstOrDefault().id;
                ViewBag.VendorSite = Fn.Data2Dropdown(db.AP_SUPPLIER_SITE_ALL.Where(ww => ww.VENDOR_ID == vid).Join(db.WP_GC_USER_ACCESS,
        s => s.VENDOR_SITE_ID,
        a => a.ENTITY_SITE_ID,
        (sit, access) => new
        {
            WP_PORTAL_USERS_ID = access.WP_PORTAL_USERS_ID,
            IDx = sit.VENDOR_SITE_ID + "½" + sit.VENDOR_ID,
            Namex = sit.VENDOR_SITE_CODE + " - " + sit.ADDRESS_LINE1
        }

        ).Join(db.WP_PORTAL_USERS.Where(x => x.USER_NAME == thisuser),
        ss => ss.WP_PORTAL_USERS_ID,
        u => u.WP_PORTAL_USERS_ID,
        (site, usr) => new
        {
            ID = site.IDx,
            Name = site.Namex
        }
        ).ToList());

            }
            return View();
        }

        [HttpPost]
        public ActionResult FuelRatesFromRFO()
        {

            var frmdata = HttpContext.Request.Form["vls"];
            if (frmdata != null)
            {
                ViewBag.tag = frmdata;
                ViewBag.FRM = Fn.Data2Json(@"EXEC [CDXP].[SP_LOAD_FUEL_RATES_BY_FORECAST_HEADER_ID] " + frmdata.Split('½')[0]);
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    var thisuser = Convert.ToString(Session["UserName"]);


                    var ipps = db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME)
                    .Join(db.WP_GC_USER_ACCESS,
                    s => s.VENDOR_ID,
                    a => a.ENTITY_ID,
                    (sup, acc) => new { sup.VENDOR_ID, sup.VENDOR_NAME, acc.WP_PORTAL_USERS_ID })
                    .Join(db.WP_PORTAL_USERS.Where(w => w.USER_NAME == thisuser),
                    x => x.WP_PORTAL_USERS_ID,
                    y => y.WP_PORTAL_USERS_ID,
                    (vend, user) => new { id = vend.VENDOR_ID, vend.VENDOR_NAME }).Distinct().ToList();

                    ViewBag.IPP = Fn.Data2Dropdown(ipps);
                    var vid = ipps.FirstOrDefault().id;
                    ViewBag.VendorSite = Fn.Data2Dropdown(db.AP_SUPPLIER_SITE_ALL.Where(ww => ww.VENDOR_ID == vid).Join(db.WP_GC_USER_ACCESS,
                        s => s.VENDOR_ID,
                        a => a.ENTITY_ID,
                        (sit, access) => new
                        {
                            WP_PORTAL_USERS_ID = access.WP_PORTAL_USERS_ID,
                            IDx = sit.VENDOR_SITE_ID + "½" + sit.VENDOR_ID,
                            Namex = sit.VENDOR_SITE_CODE + " - " + sit.ADDRESS_LINE1
                        }

                        ).Join(db.WP_PORTAL_USERS.Where(x => x.USER_NAME == thisuser),
                        ss => ss.WP_PORTAL_USERS_ID,
                        u => u.WP_PORTAL_USERS_ID,
                        (site, usr) => new
                        {
                            ID = site.IDx,
                            Name = site.Namex
                        }
                        ).ToList());


                    ViewBag.ddlFuelSupplier = Fn.Data2Dropdown(db.FND_LOOKUP_VALUES.Where(v => v.LOOKUP_TYPE == "CPPA_EMO_FUEL_SOURCE_NAMES" && v.ENABLED_FLAG == "Y").Select(s => new { id = s.LOOKUP_CODE, val = s.MEANING }).ToList());
                    ViewBag.dllPeriod = Fn.Data2Dropdown(db.CPPA_EMO_PERIODS.OrderBy(ord => ord.PERIOD_NUM).Select(s => new { id = s.EMO_PERIOD_ID, name = s.PERIOD_NAME }).ToList());
                    //ViewBag.dllPeriod = Fn.Data2DropdownSQL(@"Select TOP(1) EMO_PERIOD_ID, PERIOD_NAME from [CDXP].[CPPA_EMO_PERIODS] ORDER BY EMO_PERIOD_ID DESC");

                    ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == thisuser).FirstOrDefault();



                }
            }
            else
            {
                ViewBag.tag = "0";
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {

                    var thisuser = Convert.ToString(Session["UserName"]);

                    var ipps = db.AP_SUPPLIERS.OrderBy(ord => ord.VENDOR_NAME)
.Join(db.WP_GC_USER_ACCESS,
s => s.VENDOR_ID,
a => a.ENTITY_ID,
(sup, acc) => new { sup.VENDOR_ID, sup.VENDOR_NAME, acc.WP_PORTAL_USERS_ID })
.Join(db.WP_PORTAL_USERS.Where(w => w.USER_NAME == thisuser),
x => x.WP_PORTAL_USERS_ID,
y => y.WP_PORTAL_USERS_ID,
(vend, user) => new { id = vend.VENDOR_ID, vend.VENDOR_NAME }).Distinct().ToList();

                    ViewBag.IPP = Fn.Data2Dropdown(ipps);


                    ViewBag.ddlFuelSupplier = Fn.Data2Dropdown(db.FND_LOOKUP_VALUES.Where(v => v.LOOKUP_TYPE == "CPPA_EMO_FUEL_SOURCE_NAMES" && v.ENABLED_FLAG == "Y").Select(s => new { id = s.LOOKUP_CODE, val = s.MEANING }).ToList());
                    //ViewBag.dllPeriod = Fn.Data2Dropdown(db.CPPA_EMO_PERIODS.OrderBy(ord => ord.PERIOD_NUM).Select(s => new { id = s.EMO_PERIOD_ID, name = s.PERIOD_NAME }).ToList());
                    ViewBag.dllPeriod = Fn.Data2DropdownSQL(@"Select TOP(1) EMO_PERIOD_ID, PERIOD_NAME from [CDXP].[CPPA_EMO_PERIODS] ORDER BY EMO_PERIOD_ID DESC");
                    ViewBag.userinfo = db.WP_PORTAL_USERS.Where(w => w.USER_NAME == thisuser).FirstOrDefault();

                    var vid = ipps.FirstOrDefault().id;
                    ViewBag.VendorSite = Fn.Data2Dropdown(db.AP_SUPPLIER_SITE_ALL.Where(ww => ww.VENDOR_ID == vid).Join(db.WP_GC_USER_ACCESS,
                        s => s.VENDOR_ID,
                        a => a.ENTITY_ID,
                        (sit, access) => new
                        {
                            WP_PORTAL_USERS_ID = access.WP_PORTAL_USERS_ID,
                            IDx = sit.VENDOR_SITE_ID + "½" + sit.VENDOR_ID,
                            Namex = sit.VENDOR_SITE_CODE + " - " + sit.ADDRESS_LINE1
                        }

                        ).Join(db.WP_PORTAL_USERS.Where(x => x.USER_NAME == thisuser),
                        ss => ss.WP_PORTAL_USERS_ID,
                        u => u.WP_PORTAL_USERS_ID,
                        (site, usr) => new
                        {
                            ID = site.IDx,
                            Name = site.Namex
                        }
                        ).ToList());
                }
            }
            return View();
        }
        public ActionResult EcnomicMeritOrder()
        {
            return View();
        }
        //[HttpPost]


        public void sessionCheck()
        {

            if (Convert.ToString(Session["UserName"]) == "")
            {
                Session.Add("UserName", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["UserName"]).Value)));
            }
            if (Convert.ToString(Session["usr"]) == "")
            {
                Session.Add("usr", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["usr"]).Value)));
            }
            if (Convert.ToString(Session["contextTokenString"]) == "")
            {
                Session.Add("contextTokenString", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["contextTokenString"]).Value)));
            }
            if (Convert.ToString(Session["SPHostUrl"]) == "")
            {
                Session.Add("SPHostUrl", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["SPHostUrl"]).Value)));
            }

        }

        [HttpPost]
        public string EMOjaxCall()
        {
            sessionCheck();

            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");
            var frmdata = HttpContext.Request.Form["vls"];
            var filekData = HttpUtility.UrlDecode(HttpContext.Request.Form["fls"]);
            string[] dataID = new string[500];
            //string[] fdataID = new string[500];
            dataID = Fn.CleanSQL(HttpUtility.UrlDecode(frmdata)).Split('½');

            string Returnvls = "";
            try
            {
                //String PARENT_ID = "0";
                switch (Convert.ToInt32(dataID[0]))
                {

                    case 5:

                        string submitdate = "'" + serverDate + "'";
                        string FN = "0";
                        if (dataID[2] == "Draft")
                        {
                            submitdate = "NULL";
                        }
                        else
                        {
                            FN = Fn.ExenID(@"SELECT MAX(ISNULL([FORECAST_NO], 0)) +1 FN FROM[CDXP].[CPPA_EMO_FORECAST_HEADER]");
                        }
                        string ERP_TRANSFER = dataID[2] == "Draft" ? "0" : "1";

                        string pp = Fn.ExenID(
                            @"SELECT ISNULL(POWER_POLICY,'') FROM CDXP.PPA_HEADER WHERE CDXP.PPA_HEADER.[VENDOR_ID_FK]='" +
                            dataID[6] + @"' AND CDXP.PPA_HEADER.[VENDOR_SITE_ID_FK]='" + dataID[5] +
                            @"' AND CDXP.PPA_HEADER.[APPROVAL_STATUS]='Approved'");
                        if (dataID[1] == "0")
                        {



                            string FORECAST_HEADER_ID = Fn.ExenID(@"INSERT INTO CDXP.CPPA_EMO_FORECAST_HEADER
                         (APPROVAL_STATUS, FORECAST_NO, EMO_PERIOD_ID, VENDOR_SITE_ID, VENDOR_ID, 
PPA_PLANT_EMO_ID, BLOCK_TITLE, 
PPA_PLT_BLK_FUEL_ID,FUEL_NAME, REMARKS,
CREATION_DATE, LAST_UPDATE_DATE, SUBMIT_DATE , CREATED_BY, LAST_UPDATE_LOGIN,ERP_TRANSFER, POWER_POLICY)
VALUES        ('" + dataID[2] + @"','" + FN + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"','" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"','" + dataID[11] + @"','" + dataID[12] + @"',
'" + serverDate + @"','" + serverDate + @"', " + submitdate + @", [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')
, '" + ERP_TRANSFER + "','" + pp + @"');  select SCOPE_IDENTITY();");


                            try
                            {
                                var frmdata52 = HttpContext.Request.Form["vls2"];
                                foreach (var item in frmdata52.Split('│'))
                                {
                                    string[] D52 = new string[500];
                                    D52 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                    Fn.Exec(@"INSERT INTO CDXP.CPPA_EMO_FC_OPN_STK_LINES
                         (STOCK_DESCRIPTION, QUANTITY, RATE, AMOUNT, FORECAST_HEADER_ID)
                            VALUES        ('" + D52[0] + @"','" + D52[1] + @"','" + D52[2] + @"','" + D52[3] + @"','" + FORECAST_HEADER_ID + @"')");
                                }
                            }
                            catch (Exception e)
                            {
                                Fn.Exec("INSERT INTO [CDXP].[Testing] (CheckVAL) VALUES ('EMO 1 = " + e.Message + "')");
                            }

                            try
                            {
                                var frmdata53 = HttpContext.Request.Form["vls3"];
                                foreach (var item in frmdata53.Split('│'))
                                {
                                    string[] D52 = new string[500];
                                    D52 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                    Fn.Exec(@" INSERT INTO CDXP.CPPA_EMO_FORECAST_LINES
                         (FORECAST_HEADER_ID, FUEL_TEMPLATE_LINE_ID, CLAIMED_VALUE)
                            VALUES        ('" + FORECAST_HEADER_ID + @"','" + D52[0] + @"','" + D52[1] + @"')");
                                }
                            }
                            catch (Exception e)
                            {
                                Fn.Exec("INSERT INTO [CDXP].[Testing] (CheckVAL) VALUES ('EMO 2 = " + e.Message + "')");
                            }

                            try
                            {
                                var frmdata54 = HttpContext.Request.Form["vls4"];
                                foreach (var item in frmdata54.Split('│'))
                                {
                                    string[] D54 = new string[500];
                                    D54 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                    Fn.Exec(@"INSERT INTO CDXP.CPPA_EMO_FUEL_RATE_LINES
                         (FUEL_SUPPLIER, QUANTITY, RATE, OTHER_CHARGES, PRODUCT_COST, FORECAST_HEADER_ID , FINANCE_VERIFIED)
                            VALUES        ('" + D54[0] + @"','" + D54[1] + @"','" + D54[2] + @"','" + D54[3] + @"','" + D54[4] + @"','" + FORECAST_HEADER_ID + @"',NULL)");
                                }
                            }
                            catch (Exception e)
                            {
                                Fn.Exec("INSERT INTO [CDXP].[Testing] (CheckVAL) VALUES ('EMO 3 = " + e.Message + "')");
                            }

                            Returnvls = FORECAST_HEADER_ID;
                        }
                        else
                        {

                            Fn.ExenID(@"UPDATE CDXP.CPPA_EMO_FORECAST_HEADER
                         SET  ERP_TRANSFER='" + ERP_TRANSFER + @"',APPROVAL_STATUS = '" + dataID[2] + @"', FORECAST_NO='" + FN + @"', EMO_PERIOD_ID='" + dataID[4] + @"', VENDOR_SITE_ID='" + dataID[5] + @"', VENDOR_ID='" + dataID[6] + @"', PPA_PLANT_EMO_ID='" + dataID[8] + @"', BLOCK_TITLE='" + dataID[9] + @"', PPA_PLT_BLK_FUEL_ID='" + dataID[10] + @"',FUEL_NAME='" + dataID[11] + @"', REMARKS='" + dataID[12] + @"',
 LAST_UPDATE_DATE='" + serverDate + @"', SUBMIT_DATE=" + submitdate + @" ,  LAST_UPDATE_LOGIN=[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), 
POWER_POLICY = '" + pp + @"' 
 WHERE FORECAST_HEADER_ID = " + dataID[1]);
                            string FORECAST_HEADER_ID = dataID[1];

                            try
                            {
                                var frmdata52 = HttpContext.Request.Form["vls2"];
                                foreach (var item in frmdata52.Split('│'))
                                {
                                    if (item.Contains("½"))
                                    {
                                        string[] D52 = new string[500];
                                        D52 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                        Fn.Exec(@"INSERT INTO CDXP.CPPA_EMO_FC_OPN_STK_LINES
                         (STOCK_DESCRIPTION, QUANTITY, RATE, AMOUNT, FORECAST_HEADER_ID)
                            VALUES        ('" + D52[0] + @"','" + D52[1] + @"','" + D52[2] + @"','" + D52[3] + @"','" + FORECAST_HEADER_ID + @"')");
                                    }

                                }
                            }
                            catch (Exception e)
                            {
                                Fn.Exec("INSERT INTO [CDXP].[Testing] (CheckVAL) VALUES ('EMO 4 = " + e.Message + "')");
                            }


                            try
                            {
                                var frmdata53 = HttpContext.Request.Form["vls3"];
                                foreach (var item in frmdata53.Split('│'))
                                {
                                    if (item.Contains("½"))
                                    {
                                        string[] D52 = new string[500];
                                        D52 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                        Fn.Exec(@"UPDATE CDXP.CPPA_EMO_FORECAST_LINES SET CLAIMED_VALUE= '" + D52[1] + @"' WHERE FORECAST_LINE_ID='" + D52[0] + @"' ");
                                    }

                                }
                            }
                            catch (Exception e)
                            {
                                Fn.Exec("INSERT INTO [CDXP].[Testing] (CheckVAL) VALUES ('EMO 5 = " + e.Message + "')");
                            }


                            try
                            {
                                var frmdata54 = HttpContext.Request.Form["vls4"];
                                foreach (var item in frmdata54.Split('│'))
                                {
                                    if (item.Contains("½"))
                                    {
                                        string[] D54 = new string[500];
                                        D54 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                        Fn.Exec(@"INSERT INTO CDXP.CPPA_EMO_FUEL_RATE_LINES
                         (FUEL_SUPPLIER, QUANTITY, PRODUCT_COST, OTHER_CHARGES, RATE, FORECAST_HEADER_ID , FINANCE_VERIFIED)
                            VALUES        ('" + D54[0] + @"','" + D54[1] + @"','" + D54[2] + @"','" + D54[3] + @"','" + D54[4] + @"','" + FORECAST_HEADER_ID + @"',NULL)");
                                    }

                                }
                            }
                            catch (Exception e)
                            {
                                Fn.Exec("INSERT INTO [CDXP].[Testing] (CheckVAL) VALUES ('EMO 6 = " + e.Message + "')");
                            }


                            try
                            {
                                var frmdata55 = HttpContext.Request.Form["vls5"];
                                var frmdata56 = HttpContext.Request.Form["vls6"];
                                if (frmdata55.Length > 0)
                                {
                                    var delvals = frmdata55.Replace('½', ',');
                                    Fn.Exec("DELETE CDXP.CPPA_EMO_FC_OPN_STK_LINES WHERE OPN_STOCK_LINE_ID IN (" + delvals + ")");
                                }
                                if (frmdata56.Length > 0)
                                {
                                    var delvals = frmdata56.Replace('½', ',');
                                    Fn.Exec("DELETE CDXP.CPPA_EMO_FUEL_RATE_LINES WHERE FUEL_RATE_LINE_ID IN (" + delvals + ")");
                                }
                            }
                            catch (Exception e)
                            {
                                Fn.Exec("INSERT INTO [CDXP].[Testing] (CheckVAL) VALUES ('EMO 7 = " + e.Message + "')");
                            }

                            Returnvls = FORECAST_HEADER_ID;


                        }

                        //=======================================================

                        foreach (string itemxx in filekData.Split('¼'))
                        {

                            if (itemxx.Contains("½"))
                            {

                                string fileName = itemxx.Split('½')[2];
                                string ext = fileName.Split('.')[fileName.Split('.').Length - 1];
                                byte[] FileBytes = Convert.FromBase64String(itemxx.Split('½')[5]);
                                //var Filecontents = new StreamContent(new MemoryStream(FileBytes)); ;
                                MemoryStream filestream = new MemoryStream(FileBytes);
                                //==========================


                                var UserNameAndPaswordForID = Fn.UserNameAndPaswordForID("1");
                                var userName = UserNameAndPaswordForID.Split('½')[0];
                                string passWd = UserNameAndPaswordForID.Split('½')[1];
                                SecureString securePassWd = new SecureString();
                                foreach (var c in passWd.ToCharArray())
                                {
                                    securePassWd.AppendChar(c);
                                }
                                //string destinationSiteUrl = Convert.ToString(Session["SPHostUrl"]);
                                string destinationSiteUrl = Convert.ToString("https://cppapk.sharepoint.com/departments/finance");
                                ClientContext clientContext = new ClientContext(destinationSiteUrl);
                                clientContext.Credentials = new SharePointOnlineCredentials(userName, securePassWd);


                                //mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm
                                var list = clientContext.Web.Lists.GetByTitle("Documents");
                                clientContext.Load(list.RootFolder);
                                clientContext.ExecuteQuery();

                                //string folder = Convert.ToString(Session["UserName"]).Replace("_dc@cppapk.onmicrosoft.com", "").ToUpper();

                                var fileUrl = String.Format("{0}/{1}", "/departments/finance/Shared%20Documents/Merit%20Order", fileName); //list.RootFolder.ServerRelativeUrl, fileName);

                                Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, fileUrl, filestream, true);
                                string ONLINE_URL = "https://cppapk.sharepoint.com/departments/finance/Shared%20Documents/Merit%20Order/" + fileName;

                                var web = clientContext.Web;
                                User newUser = web.EnsureUser(Convert.ToString(Session["usr"]));
                                clientContext.Load(newUser);
                                clientContext.ExecuteQuery();

                                FieldUserValue userValue = new FieldUserValue();
                                userValue.LookupId = newUser.Id;
                                var usr = newUser.Id + ";#" + Session["usr"].ToString();
                                Session.Add("newUsr", usr);
                                var f = web.GetFileByServerRelativeUrl(fileUrl);
                                var item = f.ListItemAllFields;
                                item["Title"] = itemxx.Split('½')[3];
                                item["Author"] = usr;
                                item["Editor"] = usr;
                                item.SystemUpdate();
                                clientContext.Load(item, i => i["_dlc_DocId"], i => i.Id);

                                //context.Load(item, i => i.Id);
                                clientContext.ExecuteQuery();
                                var site = clientContext.Web;
                                clientContext.Load(site);
                                clientContext.ExecuteQuery();
                                var rid = Convert.ToString(item.Id);
                                var webid = site.Id.ToString();
                                var docid = Convert.ToString(item["_dlc_DocId"]);
                                string objurl = "https://prod-112.westeurope.logic.azure.com:443/workflows/f1f1af110b1047fdbe4a5db60f1149cc/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=C3EeWBBP-2cgSgo1Rm6EPacISujylURbxrJsiYLv9d4";
                                Uri u = new Uri(objurl);
                                var payload = "{'DocId': '" + rid + @"','LibraryName': 'Documents' ,'SiteUrl': '" + destinationSiteUrl + @"' ,'SiteName': 'Finance Department' ,'currentUserItemId': '0' ,'WebId': '" + webid + @"','authorUser': '" + Convert.ToString(Session["UserName"]) + @"','uploadFrom': 'EMO'}";

                                HttpContent cc = new StringContent(payload, Encoding.UTF8, "application/json");
                                var t = Task.Run(() => PostURI(u, cc));
                                t.Wait();
                                //Fn.Exec(@"INSERT INTO [CDXP].[tblFiles] (strFileName,strSPUrl,strDescription) VALUES ('" + file.FileName + @"','http://172.16.10.54/ecm/folder/download/" + result["_dlc_DocId"].ToString() + @"','" + d[0] + @"')");
                                //Fn.Exec(@"INSERT INTO [CDXP].[WP_FILES] (FILE_URL,PARENT_ID,UPLOADED_FOR,FILE_TITLE,FILE_DESCRIPTION,SHAREPOINT_FILE_ID,SHAREPOINT_FILE_GUID, FILE_NAME, FILE_EXT,FILE_TYPE) VALUES ('http://172.16.10.54/ecm/folder/download/" + rid + @"','" + Returnvls + @"','EMO','" + itemxx.Split('½')[3] + @"','" + itemxx.Split('½')[4] + @"','" + rid + @"','" + webid + @"','','','')");
                                try
                                {
                                    string abc = Fn.Exec(@"INSERT INTO [CDXP].[WP_FILES] (FILE_URL,PARENT_ID,UPLOADED_FOR,FILE_TITLE,FILE_DESCRIPTION,SHAREPOINT_FILE_ID,SHAREPOINT_FILE_GUID,FILES_NAME,FILE_EXT,FILE_TYPE,ONLINE_URL) VALUES ('http://172.16.10.54/ecm/folder/download/" + docid + @"','" + Returnvls + @"','EMO','" + itemxx.Split('½')[3] + @"','" + itemxx.Split('½')[4] + @"','" + rid + @"','" + webid + @"','" + itemxx.Split('½')[2] + @"','" + itemxx.Split('½')[0] + @"','" + itemxx.Split('½')[1] + @"','" + ONLINE_URL + "')");
                                    Fn.Exec("INSERT INTO [CDXP].[Testing] (CheckVAL) VALUES ('" + abc + "')");
                                }
                                catch (Exception ex)
                                {

                                    Fn.Exec("INSERT INTO [CDXP].[Testing] (CheckVAL) VALUES ('" + ex.Message + "')");
                                }
                                //mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm

                                //===========================

                            }

                        }
                        //===========================================================




                        break;

                    default:
                        Returnvls = " - 1";
                        break;
                }

            }
            catch (Exception ex)
            {
                Fn.Exec(@"INSERT INTO  CDXP.Testing (CheckVAL) VALUES ('" + ex.Message + "')");
                return Convert.ToString(ex.Message);
            }


            return Returnvls;
        }
        static async Task<string> PostURI(Uri u, HttpContent c)
        {
            var response = string.Empty;
            using (var client = new HttpClient())
            {
                HttpResponseMessage result = await client.PostAsync(u, c);
                if (result.IsSuccessStatusCode)
                {
                    response = result.StatusCode.ToString();
                }
            }
            return response;
        }

        [HttpPost]
        public string AjaxCall()
        {

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
                    case 0:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            double VENDOR_ID_FK0 = Convert.ToDouble(dataID[2]);
                            double VENDOR_SITE_ID_FK0 = Convert.ToDouble(dataID[1]);

                            //Returnvls = Fn.Data2Dropdown(
                            //    db.CPPA_PPA_PLT_EMO.Join(db.PPA_HEADER.Where(hdr => hdr.APPROVAL_STATUS == "Approved" && hdr.VENDOR_SITE_ID_FK == VENDOR_SITE_ID_FK0 && hdr.VENDOR_ID_FK == VENDOR_ID_FK0),
                            //    emo => emo.HEADER_ID_FK,
                            //    ppahdr => ppahdr.HEADER_ID_PK,
                            //    (pltemo, ppaheader) => new
                            //    {
                            //        id = pltemo.PLT_EMO_ID + "½" + pltemo.BLK_TITLE_FOR_EMO + "½" + pltemo.PLT_BLK_FUEL_ID + "½",
                            //        val = pltemo.BLK_TITLE_FOR_EMO,
                            //        PLT_BLK_FUEL_ID = pltemo.PLT_BLK_FUEL_ID
                            //    }
                            //    ).Join(db.CPPA_PPA_PLT_BLK_FUEL, 
                            //    inner => inner.PLT_BLK_FUEL_ID, 
                            //    typ => typ.PLT_BLK_FUEL_ID, 
                            //    (pltemoa, fuel) => new
                            //    { id = pltemoa.id + fuel.FUEL_LOOKUP_CODE, vals = pltemoa.val }
                            //    ).ToList()
                            //    );
                            Returnvls = Fn.Data2DropdownSQL(@"SELECT 
                            id = CAST(CPPA_PPA_PLT_EMO.PLT_EMO_ID AS VARCHAR(100)) + '½' + CAST(CPPA_PPA_PLT_EMO.BLK_TITLE_FOR_EMO AS  VARCHAR(100)) + '½' + CAST(CPPA_PPA_PLT_EMO.PLT_BLK_FUEL_ID AS VARCHAR(100)) + '½' + CDXP.CPPA_PPA_PLT_EMO.ATTRIBUTE1
                            , CDXP.CPPA_PPA_PLT_EMO.BLK_TITLE_FOR_EMO + ' ( ' + ISNULL(CDXP.CPPA_PPA_PLT_BLK_FUEL.ATTRIBUTE2 ,'--')+ ' ) ' + CDXP.CPPA_PPA_PLT_EMO.ATTRIBUTE1
                            FROM CDXP.CPPA_PPA_PLT_EMO 
                            INNER JOIN CDXP.PPA_HEADER ON  PPA_HEADER.HEADER_ID_PK= CPPA_PPA_PLT_EMO.HEADER_ID_FK
                            INNER JOIN CDXP.CPPA_PPA_PLT_BLK_FUEL ON CDXP.CPPA_PPA_PLT_EMO.PLT_BLK_FUEL_ID = CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID  
                            WHERE PPA_HEADER.APPROVAL_STATUS= 'Approved' AND PPA_HEADER.VENDOR_ID_FK=" + Convert.ToString(VENDOR_ID_FK0) + @" AND VENDOR_SITE_ID_FK=" + Convert.ToString(VENDOR_SITE_ID_FK0));

                            //                            Returnvls = Fn.Data2DropdownSQL(@"SELECT
                            //id = CAST(CPPA_PPA_PLT_EMO.PLT_BLK_FUEL_ID AS VARCHAR(100)) + '½' + CAST(CPPA_PPA_PLT_EMO.BLK_TITLE_FOR_EMO AS  VARCHAR(100)) + '½' + CAST(CPPA_PPA_PLT_EMO.PLT_EMO_ID AS VARCHAR(100)) + '½' + CDXP.CPPA_PPA_PLT_EMO.ATTRIBUTE1
                            //, CDXP.CPPA_PPA_PLT_EMO.BLK_TITLE_FOR_EMO + ' ( ' + ISNULL(CDXP.CPPA_PPA_PLT_BLK_FUEL.ATTRIBUTE2 ,'--')+ ' ) ' + CDXP.CPPA_PPA_PLT_EMO.ATTRIBUTE1
                            //FROM CDXP.CPPA_PPA_PLT_EMO
                            //INNER JOIN CDXP.PPA_HEADER ON  PPA_HEADER.HEADER_ID_PK= CPPA_PPA_PLT_EMO.HEADER_ID_FK
                            //INNER JOIN CDXP.CPPA_PPA_PLT_BLK_FUEL ON CDXP.CPPA_PPA_PLT_EMO.PLT_BLK_FUEL_ID = CDXP.CPPA_PPA_PLT_BLK_FUEL.PLT_BLK_FUEL_ID 
                            //WHERE PPA_HEADER.APPROVAL_STATUS= 'Approved' AND PPA_HEADER.VENDOR_ID_FK=" + Convert.ToString(VENDOR_ID_FK0) + @" AND VENDOR_SITE_ID_FK=" + Convert.ToString(VENDOR_SITE_ID_FK0));
                        }

                        break;

                    case 1:

                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            // Returnvls = Fn.Data2Dropdown(db.CPPA_PPA_PLT_BLK_FUEL.Where(w => w.PLT_BLK_FUEL_ID == Convert.ToDecimal(dataID[2])).Select(s => new { id = s.PLT_BLK_FUEL_ID, n = s.FUEL_LOOKUP_CODE }).ToList());
                            Returnvls = Fn.Data2DropdownSQL(@"SELECT PLT_BLK_FUEL_ID, FUEL_LOOKUP_CODE  FROM 
CDXP.CPPA_PPA_PLT_BLK_FUEL WHERE PLT_BLK_FUEL_ID =" + dataID[2]);

                        }
                        break;
                    case 2:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"EXEC CDXP.CrossTab 'SELECT        CDXP.CPPA_EMO_FORECAST_HEADER.FORECAST_HEADER_ID, [CDXP].[CPPA_EMO_PERIODS].PERIOD_NAME [Period], ISNULL(CAST(CDXP.CPPA_EMO_FORECAST_HEADER.FORECAST_NO AS VARCHAR(100)),'''') [FNo.], CDXP.CPPA_EMO_FORECAST_HEADER.APPROVAL_STATUS [Status], FORMAT(CDXP.CPPA_EMO_FORECAST_HEADER.SUBMIT_DATE, ''dd-MMM-yyyy'') AS [Submission Date], 
                         CDXP.CPPA_EMO_FORECAST_HEADER.BLOCK_TITLE AS Block, CDXP.CPPA_EMO_FORECAST_HEADER.FUEL_NAME AS Fuel, CDXP.CPPA_EMO_FUEL_TEMP_LINES.COMP_NAME, 
                         CDXP.CPPA_EMO_FORECAST_LINES.CLAIMED_VALUE
FROM            CDXP.CPPA_EMO_FORECAST_HEADER INNER JOIN
                         CDXP.CPPA_EMO_FORECAST_LINES ON CDXP.CPPA_EMO_FORECAST_LINES.FORECAST_HEADER_ID = CDXP.CPPA_EMO_FORECAST_HEADER.FORECAST_HEADER_ID INNER JOIN
                         CDXP.CPPA_EMO_FUEL_TEMP_LINES ON CDXP.CPPA_EMO_FUEL_TEMP_LINES.FUEL_TEMPLATE_LINE_ID = CDXP.CPPA_EMO_FORECAST_LINES.FUEL_TEMPLATE_LINE_ID INNER JOIN
                         CDXP.PPA_HEADER ON CDXP.CPPA_EMO_FORECAST_HEADER.VENDOR_ID = CDXP.PPA_HEADER.VENDOR_ID_FK AND 
                         CDXP.CPPA_EMO_FORECAST_HEADER.VENDOR_SITE_ID = CDXP.PPA_HEADER.VENDOR_SITE_ID_FK INNER JOIN
                         CDXP.CPPA_PPA_PLT_EMO ON CDXP.PPA_HEADER.HEADER_ID_PK = CDXP.CPPA_PPA_PLT_EMO.HEADER_ID_FK
                         INNER JOIN [CDXP].[CPPA_EMO_PERIODS] ON [CDXP].[CPPA_EMO_PERIODS].EMO_PERIOD_ID=[CDXP].CPPA_EMO_FORECAST_HEADER.EMO_PERIOD_ID 
WHERE        (CDXP.CPPA_EMO_FORECAST_HEADER.VENDOR_SITE_ID = " + dataID[1] + @") AND (CDXP.CPPA_EMO_FORECAST_HEADER.VENDOR_ID = " + dataID[2] + @") 
and  CDXP.CPPA_PPA_PLT_EMO.PLT_EMO_ID = " + dataID[3] + @" and  CDXP.CPPA_PPA_PLT_EMO.PLT_BLK_FUEL_ID = " + dataID[5] + @" 
AND (CDXP.CPPA_EMO_FORECAST_HEADER.FORECAST_TO_DATE BETWEEN 
                         ''" + dataID[9] + @"'' AND ''" + dataID[10] + @"'') AND (CDXP.CPPA_EMO_FORECAST_HEADER.SUBMIT_DATE BETWEEN ''" + dataID[9] + @"'' AND ''" + dataID[10] + @"'') AND 
                         (CDXP.CPPA_EMO_FUEL_TEMP_LINES.COM_REF_CODE IS NOT NULL)'
,'COMP_NAME','max(CLAIMED_VALUE ELSE 0 )[]',N'FORECAST_HEADER_ID,[Period],[FNo.],[Status],[Submission Date],[Block],[Fuel]',NULL,',1,1'", "tblJ1");
                        break;


                    case 3:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT [CDXP].[CPPA_EMO_FUEL_TEMP_LINES].FUEL_TEMPLATE_LINE_ID, '' [Sr#],[CDXP].[CPPA_EMO_FUEL_TEMP_LINES].COMP_NAME [Component Name],'' [Value], [CDXP].[CPPA_EMO_FUEL_TEMP_LINES].UNIT   FROM [CDXP].[CPPA_PPA_PLT_EMO]
INNER JOIN [CDXP].[CPPA_EMO_FUEL_TEMP_LINES] ON [CDXP].[CPPA_EMO_FUEL_TEMP_LINES].FUEL_TEMPLATE_ID = [CDXP].[CPPA_PPA_PLT_EMO].FUEL_TEMPLATE_ID
WHERE [CDXP].[CPPA_PPA_PLT_EMO].PLT_EMO_ID = " + dataID[1] + @" AND PLT_BLK_FUEL_ID=" + dataID[2] + @" 
ORDER BY [CDXP].[CPPA_EMO_FUEL_TEMP_LINES].COMP_SEQUENCE", "tblJ1");
                        break;

                    case 4:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"EXEC CDXP.CrossTab 'SELECT        CDXP.CPPA_EMO_FORECAST_HEADER.FORECAST_HEADER_ID, [CDXP].[CPPA_EMO_PERIODS].PERIOD_NAME [Period], ISNULL(CAST(CDXP.CPPA_EMO_FORECAST_HEADER.FORECAST_NO AS VARCHAR(100)),'''') [FNo.], CDXP.CPPA_EMO_FORECAST_HEADER.APPROVAL_STATUS [Status], FORMAT(CDXP.CPPA_EMO_FORECAST_HEADER.SUBMIT_DATE, ''dd-MMM-yyyy'') AS [Submission Date], 
                         CDXP.CPPA_EMO_FORECAST_HEADER.BLOCK_TITLE AS Block, CDXP.CPPA_EMO_FORECAST_HEADER.FUEL_NAME AS Fuel, CDXP.CPPA_EMO_FUEL_TEMP_LINES.COMP_NAME, 
                         CDXP.CPPA_EMO_FORECAST_LINES.CLAIMED_VALUE
FROM            CDXP.WP_PORTAL_USERS INNER JOIN
                         CDXP.WP_GC_USER_ACCESS ON CDXP.WP_PORTAL_USERS.WP_PORTAL_USERS_ID = CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID INNER JOIN
                         CDXP.CPPA_EMO_FORECAST_HEADER INNER JOIN
                         CDXP.CPPA_EMO_FORECAST_LINES ON CDXP.CPPA_EMO_FORECAST_LINES.FORECAST_HEADER_ID = CDXP.CPPA_EMO_FORECAST_HEADER.FORECAST_HEADER_ID INNER JOIN
                         CDXP.CPPA_EMO_FUEL_TEMP_LINES ON CDXP.CPPA_EMO_FUEL_TEMP_LINES.FUEL_TEMPLATE_LINE_ID = CDXP.CPPA_EMO_FORECAST_LINES.FUEL_TEMPLATE_LINE_ID INNER JOIN
                         CDXP.PPA_HEADER ON CDXP.CPPA_EMO_FORECAST_HEADER.VENDOR_ID = CDXP.PPA_HEADER.VENDOR_ID_FK AND 
                         CDXP.CPPA_EMO_FORECAST_HEADER.VENDOR_SITE_ID = CDXP.PPA_HEADER.VENDOR_SITE_ID_FK INNER JOIN
                         CDXP.CPPA_PPA_PLT_EMO ON CDXP.PPA_HEADER.HEADER_ID_PK = CDXP.CPPA_PPA_PLT_EMO.HEADER_ID_FK ON 
                         CDXP.WP_GC_USER_ACCESS.ENTITY_ID = CDXP.PPA_HEADER.VENDOR_ID_FK
                         INNER JOIN [CDXP].[CPPA_EMO_PERIODS] ON [CDXP].[CPPA_EMO_PERIODS].EMO_PERIOD_ID=[CDXP].CPPA_EMO_FORECAST_HEADER.EMO_PERIOD_ID 
WHERE        (CDXP.CPPA_EMO_FUEL_TEMP_LINES.COM_REF_CODE IS NOT NULL) AND (CDXP.WP_PORTAL_USERS.USER_NAME = ''" + Convert.ToString(Session["UserName"]) + @"'')'
, 'COMP_NAME', 'max(CLAIMED_VALUE ELSE 0 )[]', N'FORECAST_HEADER_ID,[Period],[FNo.],[Status],[Submission Date],[Block],[Fuel]', NULL, ',1,1'", "tblJ1");
                        break;

                    case 5:
                        string submitdate = "'" + serverDate + "'";
                        string FN = "0";
                        if (dataID[2] == "Draft")
                        {
                            submitdate = "NULL";
                        }
                        else
                        {
                            FN = Fn.ExenID(@"SELECT MAX(ISNULL([FORECAST_NO], 0)) +1 FN FROM[CDXP].[CPPA_EMO_FORECAST_HEADER]");
                        }
                        if (dataID[1] == "0")
                        {
                            string FORECAST_HEADER_ID = Fn.ExenID(@"INSERT INTO CDXP.CPPA_EMO_FORECAST_HEADER
                         (APPROVAL_STATUS, FORECAST_NO, EMO_PERIOD_ID, VENDOR_SITE_ID, VENDOR_ID, 
PPA_PLANT_EMO_ID, BLOCK_TITLE, 
PPA_PLT_BLK_FUEL_ID,FUEL_NAME, REMARKS,
CREATION_DATE, LAST_UPDATE_DATE, SUBMIT_DATE , CREATED_BY, LAST_UPDATE_LOGIN)
VALUES        ('" + dataID[2] + @"','" + FN + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"','" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"','" + dataID[11] + @"','" + dataID[12] + @"',
'" + serverDate + @"','" + serverDate + @"', " + submitdate + @", [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')
);  select SCOPE_IDENTITY();");

                            var frmdata52 = HttpContext.Request.Form["vls2"];
                            foreach (var item in frmdata52.Split('│'))
                            {
                                string[] D52 = new string[500];
                                D52 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                Fn.Exec(@"INSERT INTO CDXP.CPPA_EMO_FC_OPN_STK_LINES
                         (STOCK_DESCRIPTION, QUANTITY, RATE, AMOUNT, FORECAST_HEADER_ID)
                            VALUES        ('" + D52[0] + @"','" + D52[1] + @"','" + D52[2] + @"','" + D52[3] + @"','" + FORECAST_HEADER_ID + @"')");
                            }

                            var frmdata53 = HttpContext.Request.Form["vls3"];
                            foreach (var item in frmdata53.Split('│'))
                            {
                                string[] D52 = new string[500];
                                D52 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                Fn.Exec(@" INSERT INTO CDXP.CPPA_EMO_FORECAST_LINES
                         (FORECAST_HEADER_ID, FUEL_TEMPLATE_LINE_ID, CLAIMED_VALUE)
                            VALUES        ('" + FORECAST_HEADER_ID + @"','" + D52[0] + @"','" + D52[1] + @"')");
                            }

                            var frmdata54 = HttpContext.Request.Form["vls4"];
                            foreach (var item in frmdata54.Split('│'))
                            {
                                string[] D54 = new string[500];
                                D54 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                Fn.Exec(@"INSERT INTO CDXP.CPPA_EMO_FUEL_RATE_LINES
                         (FUEL_SUPPLIER, QUANTITY, PRODUCT_COST, OTHER_CHARGES, RATE, FORECAST_HEADER_ID , FINANCE_VERIFIED)
                            VALUES        ('" + D54[0] + @"','" + D54[1] + @"','" + D54[2] + @"','" + D54[3] + @"','" + D54[4] + @"','" + FORECAST_HEADER_ID + @"',NULL)");
                            }
                            Returnvls = FORECAST_HEADER_ID;
                        }
                        else
                        {
                            Fn.ExenID(@"UPDATE CDXP.CPPA_EMO_FORECAST_HEADER
                         SET APPROVAL_STATUS = '" + dataID[2] + @"', FORECAST_NO='" + FN + @"', EMO_PERIOD_ID='" + dataID[4] + @"', VENDOR_SITE_ID='" + dataID[5] + @"', VENDOR_ID='" + dataID[6] + @"', PPA_PLANT_EMO_ID='" + dataID[8] + @"', BLOCK_TITLE='" + dataID[9] + @"', PPA_PLT_BLK_FUEL_ID='" + dataID[10] + @"',FUEL_NAME='" + dataID[11] + @"', REMARKS='" + dataID[12] + @"',
 LAST_UPDATE_DATE='" + serverDate + @"', SUBMIT_DATE=" + submitdate + @" ,  LAST_UPDATE_LOGIN=[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"') WHERE FORECAST_HEADER_ID = " + dataID[1]);
                            string FORECAST_HEADER_ID = dataID[1];
                            var frmdata52 = HttpContext.Request.Form["vls2"];
                            foreach (var item in frmdata52.Split('│'))
                            {
                                string[] D52 = new string[500];
                                D52 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                Fn.Exec(@"INSERT INTO CDXP.CPPA_EMO_FC_OPN_STK_LINES
                         (STOCK_DESCRIPTION, QUANTITY, RATE, AMOUNT, FORECAST_HEADER_ID)
                            VALUES        ('" + D52[0] + @"','" + D52[1] + @"','" + D52[2] + @"','" + D52[3] + @"','" + FORECAST_HEADER_ID + @"')");
                            }

                            var frmdata53 = HttpContext.Request.Form["vls3"];
                            foreach (var item in frmdata53.Split('│'))
                            {
                                string[] D52 = new string[500];
                                D52 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                Fn.Exec(@"UPDATE CDXP.CPPA_EMO_FORECAST_LINES SET CLAIMED_VALUE= '" + D52[1] + @"' WHERE FORECAST_LINE_ID='" + D52[0] + @"' ");

                            }

                            var frmdata54 = HttpContext.Request.Form["vls4"];
                            foreach (var item in frmdata54.Split('│'))
                            {
                                string[] D54 = new string[500];
                                D54 = Fn.CleanSQL(HttpUtility.UrlDecode(item)).Split('½');
                                Fn.Exec(@"INSERT INTO CDXP.CPPA_EMO_FUEL_RATE_LINES
                         (FUEL_SUPPLIER, QUANTITY, PRODUCT_COST, OTHER_CHARGES, RATE, FORECAST_HEADER_ID , FINANCE_VERIFIED)
                            VALUES        ('" + D54[0] + @"','" + D54[1] + @"','" + D54[2] + @"','" + D54[3] + @"','" + D54[4] + @"','" + FORECAST_HEADER_ID + @"',NULL)");
                            }

                            var frmdata55 = HttpContext.Request.Form["vls5"];
                            var frmdata56 = HttpContext.Request.Form["vls6"];
                            if (frmdata55.Length > 0)
                            {
                                var delvals = frmdata55.Replace('½', ',');
                                Fn.Exec("DELETE CDXP.CPPA_EMO_FC_OPN_STK_LINES WHERE OPN_STOCK_LINE_ID IN (" + delvals + ")");
                            }
                            if (frmdata56.Length > 0)
                            {
                                var delvals = frmdata56.Replace('½', ',');
                                Fn.Exec("DELETE CDXP.CPPA_EMO_FUEL_RATE_LINES WHERE FUEL_RATE_LINE_ID IN (" + delvals + ")");
                            }
                            Returnvls = FORECAST_HEADER_ID;

                        }
                        break;
                    case 6:
                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            Returnvls = Fn.Data2Dropdown(db.FND_LOOKUP_VALUES.Where(v => v.LOOKUP_TYPE == "CPPA_EMO_FUEL_SOURCE_NAMES" && v.ENABLED_FLAG == "Y").Select(s => new { id = s.LOOKUP_CODE, val = s.MEANING }).ToList());
                        }
                        break;

                    case 7:

                        using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                        {
                            var VENDOR_ID0 = Convert.ToInt32(dataID[1]);
                            Returnvls = Fn.Data2Dropdown(db.AP_SUPPLIER_SITE_ALL.Where(w => w.VENDOR_ID == VENDOR_ID0).Select(s => new { VENDOR_SITE_ID = s.VENDOR_SITE_ID + "½" + s.VENDOR_ID, ADDRESS_LINE1 = s.VENDOR_SITE_CODE + " - " + s.ADDRESS_LINE1 }).OrderBy(ord => ord.ADDRESS_LINE1).ToList());
                        }
                        break;
                    default:
                        Returnvls = "-1";
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
        public ActionResult UploadFile()
        {
            return View();
        }


        //[HttpPost]
        //public ActionResult UploadFile(HttpPostedFileBase file)
        //{
        //    try
        //    {

        //        if (file.ContentLength > 0)
        //        {
        //            
        //var UserNameAndPaswordForID = Fn.UserNameAndPaswordForID("1");
        //var userName = UserNameAndPaswordForID.Split('½')[0];
        //string passWd = UserNameAndPaswordForID.Split('½')[1];
        //            var userName = "cstest2@cppapk.onmicrosoft.com";
        //            string passWd = "assistant@1";
        //            SecureString securePassWd = new SecureString();
        //            foreach (var c in passWd.ToCharArray())
        //            {
        //                securePassWd.AppendChar(c);
        //            }
        //            //string _FileName = Path.GetFileName(file.FileName);
        //            ClientContext clientContext = new ClientContext("https://cppapk.sharepoint.com/departments/it");
        //            clientContext.Credentials = new SharePointOnlineCredentials(userName, securePassWd);
        //            ListItem result = UploadFile(clientContext, "Documents", file.FileName, file.InputStream);
        //            //string _path = Path.Combine(Server.MapPath("~/UploadingFiles"), _FileName);
        //            //file.SaveAs(_path);
        //            ViewBag.Message = "File Uploaded Successfully!! URL is http://172.16.10.54/ecm/folder/download/" + result["_dlc_DocId"].ToString() + " and Item Id" + result.Id.ToString();
        //            //JavaScript("triggerFlow()");
        //            ViewBag.DocId = result.Id;
        //            ViewBag.DynamicScripts = "triggerFlow()";
        //        }
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = "Error Message :" + ex.Message + ex.Source + ex.StackTrace;
        //        return View();
        //    }
        //}

        //[HttpPost]
        //private ListItem UploadFile(ClientContext context, string listTitle, string fileName, Stream fs)
        //{
        //    //using (var fs = new FileStream(fileName, FileMode.Open))
        //    //{
        //    //var fi = new FileInfo(fileName);
        //    try
        //    {
        //        var list = context.Web.Lists.GetByTitle(listTitle);
        //        context.Load(list.RootFolder);
        //        context.ExecuteQuery();
        //        var fileUrl = String.Format("{0}/{1}", "/departments/it/Shared%20Documents/ERP/Correspondance/Letters", fileName); //list.RootFolder.ServerRelativeUrl, fileName);

        //        Microsoft.SharePoint.Client.File.SaveBinaryDirect(context, fileUrl, fs, true);

        //        var web = context.Web;
        //        var f = web.GetFileByServerRelativeUrl(fileUrl);
        //        var item = f.ListItemAllFields;
        //        item["Title"] = "Good work";
        //        item.Update();
        //        context.Load(item, i => i["_dlc_DocId"], i => i.Id);

        //        //context.Load(item, i => i.Id);
        //        context.ExecuteQuery();
        //        var site = context.Web;
        //        context.Load(site);
        //        context.ExecuteQuery();
        //        ViewBag.webId = site.Id.ToString();
        //        return item;

        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //    //}
        //}




    }
}