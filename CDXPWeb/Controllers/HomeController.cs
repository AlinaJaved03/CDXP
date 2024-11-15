using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDXPWeb.Models;
using Microsoft.IdentityModel.Web;
using System.Web.Security;
using System.Net.Http;
using System.IO;
using System.Security;
using System.Net;
using System.Threading.Tasks;
using System.Text;

namespace CDXPWeb.Controllers
{
    //public class bodyContent
    //{
    //    public string DocId { get; set; }
    //    public string LibraryName { get; set; }
    //    public string SiteUrl { get; set; }
    //    public string SiteName { get; set; }
    //    public string currentUserItemId { get; set; }
    //    public string WebId { get; set; }


    //}
    public class HomeController : Controller
    {
        private clsSQLCore Fn = new clsSQLCore(); string email = "";
        [SharePointContextFilter]


        //public void sessionCheck()
        //{

        //    if (Convert.ToString(Session["UserName"]) == "")
        //    {
        //        Session.Add("UserName", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["UserName"]).Value)));
        //    }
        //    if (Convert.ToString(Session["usr"]) == "")
        //    {
        //        Session.Add("usr", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["usr"]).Value)));
        //    }
        //    if (Convert.ToString(Session["contextTokenString"]) == "")
        //    {
        //        Session.Add("contextTokenString", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["contextTokenString"]).Value)));
        //    }
        //    if (Convert.ToString(Session["SPHostUrl"]) == "")
        //    {
        //        Session.Add("SPHostUrl", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["SPHostUrl"]).Value)));
        //    }

        //}
        public void sessionCheck()
        {

            //if (Convert.ToString(Session["UserName"]) == "")
            //{
            //    try
            //    {
            //        Session.Add("UserName", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["UserName"]).Value)));
            //    }
            //    catch (Exception e)
            //    {

            //    }

            //}
            //if (Convert.ToString(Session["usr"]) == "")
            //{
            //    try
            //    {
            //        Session.Add("usr", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["usr"]).Value)));
            //    }
            //    catch (Exception e)
            //    {

            //    }
            //}
            //if (Convert.ToString(Session["contextTokenString"]) == "")
            //{
            //    try
            //    {
            //        Session.Add("contextTokenString", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["contextTokenString"]).Value)));
            //    }
            //    catch (Exception e)
            //    {

            //    }
            //}
            //if (Convert.ToString(Session["SPHostUrl"]) == "")
            //{
            //    try
            //    {
            //        Session.Add("SPHostUrl", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["SPHostUrl"]).Value)));
            //    }
            //    catch (Exception e)
            //    {

            //    }
            //}

        }
        public ActionResult Index()
        {
            try
            {
                string contextTokenString = TokenHelper.GetContextTokenFromRequest(HttpContext.Request);
                Session.Add("contextTokenString", contextTokenString);
                Response.Cookies["contextTokenString"].Value = Convert.ToString(Session["contextTokenString"]);
                Response.Cookies["contextTokenString"].Expires = DateTime.Now.AddDays(1);

                Session.Add("SPHostUrl", HttpContext.Request.QueryString["SPHostUrl"]);
                Response.Cookies["SPHostUrl"].Value = Convert.ToString(Session["SPHostUrl"]);
                Response.Cookies["SPHostUrl"].Expires = DateTime.Now.AddDays(1);

                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);


                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    clientContext.AuthenticationMode = ClientAuthenticationMode.Default;
                    if (clientContext != null)
                    {

                        clientContext.Load(clientContext.Web);
                        clientContext.Load(clientContext.Web.CurrentUser);
                        clientContext.ExecuteQuery();
                        foreach (var item in clientContext.Web.CurrentUser.LoginName.Split('|'))
                        {
                            if (item.Contains("@"))
                            {
                                Session.Add("UserName", item);
                                Response.Cookies["UserName"].Value = Convert.ToString(item);
                                Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(1);
                                email = item;
                            }
                        }
                        Session.Add("usr", clientContext.Web.CurrentUser.LoginName);

                        Response.Cookies["usr"].Value = Convert.ToString(Session["usr"]);
                        Response.Cookies["usr"].Expires = DateTime.Now.AddDays(1);

                        ViewBag.lgnm = "LoginName:" + clientContext.Web.CurrentUser.LoginName;
                        ViewBag.ttl = "Title:" + Convert.ToString(clientContext.Web.CurrentUser.Title);
                        ViewBag.eml = "Email:" + Convert.ToString(clientContext.Web.CurrentUser.Email);
                    }
                }

                string vls = HttpUtility.UrlEncode(Fn.Data2Json("EXEC [CDXP].[SP_MENU_BY_USER_ID] '" + Session["UserName"] + "'"));
                ViewBag.LayoutStr = vls;
            }
            catch (Exception ex)
            {

                Fn.Exec("INSERT INTO [CDXP].[Testing] (CheckVAL) VALUES ('" + ex.Message + "')");
            }
           
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        public ActionResult BillingMonth()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult DISCO_Dashboard()
        {
            Fn.sessionCheck();
            if (Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["UserName"]).Value) == "ntdc_dc@cppapk.onmicrosoft.com")
            {
                ViewBag.Title = "NTDC Dashboard";
            }
            else
            {
                ViewBag.Title = "DISCO Dashboard";
            }
            return View();
        }

        [HttpPost]
        public string Meter_Reading_Proforma()
        {
            Fn.sessionCheck();
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");
            var frmdata = HttpContext.Request.Form["vls"];
            string[] dataID = new string[500];
            string sql = "";

            dataID = Fn.CleanSQL(HttpUtility.UrlDecode(frmdata)).Split('½');
            string Returnvls = "";
            try
            {
                switch (Convert.ToInt32(dataID[0]))
                {
                    case 0:
                        Returnvls = Fn.Data2Json(@"EXEC [CDXP].[SP_CPPA_CDP_HEADER_DETAIL_BY_CDP_ID] " + dataID[1]);
                        break;

                    case 1:
                        Returnvls = Fn.Data2Json(@"EXEC [CDXP].[SP_METER_READING_PERFORMA_PREVIOUS_DATA] " + dataID[1] + ", " + dataID[2]);
                        break;
                    case 2:
                        if (dataID[37] == "0")
                        {
                            Returnvls = Fn.ExenID(@"INSERT INTO CDXP.WP_DISCO_CDP_MONTHLY_DATA (
CDP_ID, READING_DATE_TIME, [status], ACCOUNTING_MONTH_ID, PRIMARY_IMPORT_ACTIVE_READING, PRIMARY_IMPORT_REACTIVE_READING, PRIMARY_EXPORT_ACTIVE_READING, PRIMARY_EXPORT_REACTIVE_READING, 
                         BACKUP_IMPORT_ACTIVE_READING, BACKUP_IMPORT_REACTIVE_READING, BACKUP_EXPORT_ACTIVE_READING, BACKUP_EXPORT_REACTIVE_READING, PRIMARY_IMPORT_ACTIVE_AJD, 
                         PRIMARY_IMPORT_REACTIVE_ADJ, PRIMARY_EXPORT_ACTIVE_AJD, PRIMARY_EXPORT_REACTIVE_ADJ, BACKUP_IMPORT_ACTIVE_AJD, BACKUP_IMPORT_REACTIVE_ADJ, BACKUP_EXPORT_ACTIVE_AJD, 
                         BACKUP_EXPORT_REACTIVE_ADJ, PRIMARY_IMPORT_ACTIVE_MDI, PRIMARY_IMPORT_REACTIVE_MDI, PRIMARY_EXPORT_ACTIVE_MDI, PRIMARY_EXPORT_REACTIVE_MDI, BACKUP_IMPORT_ACTIVE_MDI, 
                         BACKUP_IMPORT_REACTIVE_MDI, BACKUP_EXPORT_ACTIVE_MDI, BACKUP_EXPORT_REACTIVE_MDI, IS_PRIMARY_METER, PRIMARY_CDP_DTL_ID, BACKUP_CDP_DTL_ID, REMARKS, 
                         PREVIOUS_WP_DISCO_CDP_MONTHLY_DATA_ID, ENG_ACTIVE_IMPORT, ENG_ACTIVE_EXPORT, MDI_ACTIVE_IMPORT, MDI_ACTIVE_EXPORT, CREATION_DATE,CREATED_BY_WP_PORTAL_USERS_ID) VALUES (
'" + dataID[1] + @"','" + dataID[2] + @"',0,'" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"','" + dataID[7] + @"','" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"','" + dataID[11] + @"','" + dataID[12] + @"',
'" + dataID[13] + @"','" + dataID[14] + @"','" + dataID[15] + @"','" + dataID[16] + @"','" + dataID[17] + @"','" + dataID[18] + @"','" + dataID[19] + @"','" + dataID[20] + @"','" + dataID[21] + @"','" + dataID[22] + @"','" + dataID[23] + @"',
'" + dataID[24] + @"','" + dataID[25] + @"','" + dataID[26] + @"','" + dataID[27] + @"','" + dataID[28] + @"','" + dataID[29] + @"','" + dataID[30] + @"','" + dataID[31] + @"','" + dataID[32] + @"','" + dataID[33] + @"','" + dataID[34] + @"','" + dataID[35] + @"','" + dataID[36] + @"','" + serverDate + @"',
[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')
);  select '1'");

                        }
                        else
                        {
                            Returnvls = Fn.ExenID(@"UPDATE       CDXP.WP_DISCO_CDP_MONTHLY_DATA
                         SET CDP_ID = '" + dataID[1] + @"', READING_DATE_TIME = '" + dataID[2] + @"', ACCOUNTING_MONTH_ID = '" + dataID[3] + @"', PRIMARY_IMPORT_ACTIVE_READING = '" + dataID[4] + @"', 
                         PRIMARY_IMPORT_REACTIVE_READING = '" + dataID[5] + @"', PRIMARY_EXPORT_ACTIVE_READING = '" + dataID[6] + @"', PRIMARY_EXPORT_REACTIVE_READING = '" + dataID[7] + @"', 
                         BACKUP_IMPORT_ACTIVE_READING = '" + dataID[8] + @"', BACKUP_IMPORT_REACTIVE_READING = '" + dataID[9] + @"', BACKUP_EXPORT_ACTIVE_READING = '" + dataID[10] + @"', 
                         BACKUP_EXPORT_REACTIVE_READING = '" + dataID[11] + @"', PRIMARY_IMPORT_ACTIVE_AJD = '" + dataID[12] + @"', PRIMARY_IMPORT_REACTIVE_ADJ = '" + dataID[13] + @"', 
                         PRIMARY_EXPORT_ACTIVE_AJD = '" + dataID[14] + @"', PRIMARY_EXPORT_REACTIVE_ADJ = '" + dataID[15] + @"', BACKUP_IMPORT_ACTIVE_AJD = '" + dataID[16] + @"', 
                         BACKUP_IMPORT_REACTIVE_ADJ = '" + dataID[17] + @"', BACKUP_EXPORT_ACTIVE_AJD = '" + dataID[18] + @"', BACKUP_EXPORT_REACTIVE_ADJ = '" + dataID[19] + @"', 
                         PRIMARY_IMPORT_ACTIVE_MDI = '" + dataID[20] + @"', PRIMARY_IMPORT_REACTIVE_MDI = '" + dataID[21] + @"', PRIMARY_EXPORT_ACTIVE_MDI = '" + dataID[22] + @"', 
                         PRIMARY_EXPORT_REACTIVE_MDI = '" + dataID[23] + @"', BACKUP_IMPORT_ACTIVE_MDI = '" + dataID[24] + @"', BACKUP_IMPORT_REACTIVE_MDI = '" + dataID[25] + @"', 
                         BACKUP_EXPORT_ACTIVE_MDI = '" + dataID[26] + @"', BACKUP_EXPORT_REACTIVE_MDI = '" + dataID[27] + @"', IS_PRIMARY_METER = '" + dataID[28] + @"', PRIMARY_CDP_DTL_ID = '" + dataID[29] + @"', 
                         BACKUP_CDP_DTL_ID = '" + dataID[30] + @"', REMARKS = '" + dataID[31] + @"', PREVIOUS_WP_DISCO_CDP_MONTHLY_DATA_ID = '" + dataID[32] + @"', ENG_ACTIVE_IMPORT = '" + dataID[33] + @"', 
                         ENG_ACTIVE_EXPORT = '" + dataID[34] + @"', MDI_ACTIVE_IMPORT = '" + dataID[35] + @"', MDI_ACTIVE_EXPORT = '" + dataID[36] + @"', LAST_UPDATE_DATE = '" + serverDate + @"', 
                         LAST_UPDATE_BY_WP_PORTAL_USERS_ID = CDXP.FN_WP_PORTAL_USERS_ID_BY_USER_NAME('" + Convert.ToString(Session["UserName"]) + @"')
                         WHERE WP_DISCO_CDP_MONTHLY_DATA_ID='" + dataID[37] + @"'; select '1'");
                        }
                        break;

                    case 3:

                        Returnvls = Fn.HTMLTableWithID_TR_Tag1(@"EXEC [CDXP].[SP_DISCO_MONTHLY_REPORT] " + dataID[1] + @", " + dataID[2] + @", " + dataID[3], "tblJ1");

                        break;


                    //					case 4:
                    //						Returnvls = Fn.HTMLTableWithID_TR_TagNext(@"SELECT        AM.ACCOUNTING_MONTH_ID , [CDXP].[DisableEditButtonDashBoard](AM.ACCOUNTING_MONTH_ID, '" + Convert.ToString(Session["UserName"]) + @"') as DisableEditButton, '' AS Sr#, UPPER(DATENAME(MONTH, CAST(AM.MONTH_NUMBER AS VARCHAR(2)) + '-01-' + CAST(AM.YEAR_NUMBER AS VARCHAR(4)))) + ' - ' + CAST(AM.YEAR_NUMBER AS VARCHAR(4)) 
                    //						 AS DISPLAY, [CDXP].[GetDashBoardSummary](AM.ACCOUNTING_MONTH_ID, '" + Convert.ToString(Session["UserName"]) + @"') as Summary, CDXP.GetDiscoWisePercentage(AM.ACCOUNTING_MONTH_ID, customer_id) AS [Completed %], 
                    //							  case when [CDXP].[DisableEditButtonDashBoard](AM.ACCOUNTING_MONTH_ID, '" + Convert.ToString(Session["UserName"]) + @"') = 0 then 'Draft'
                    //									when [CDXP].[DisableEditButtonDashBoard](AM.ACCOUNTING_MONTH_ID, '" + Convert.ToString(Session["UserName"]) + @"') = 1 then 'Submitted'
                    //									when [CDXP].[DisableEditButtonDashBoard](AM.ACCOUNTING_MONTH_ID, '" + Convert.ToString(Session["UserName"]) + @"') = 2 then 'Received'
                    //									when [CDXP].[DisableEditButtonDashBoard](AM.ACCOUNTING_MONTH_ID, '" + Convert.ToString(Session["UserName"]) + @"') = 3 then 'Returned'
                    //									else 'INVALID'
                    //								end AS Status,                                  
                    //						 ISNULL(CAST(ABSTBL.[ENG DELIVERED] AS VARCHAR(100)),'') [ENG DELIVERED],  ISNULL(CAST(ABSTBL.MDI AS VARCHAR(100)),'') [MDI]
                    //						FROM            CDXP.ACCOUNTING_MONTH AS AM LEFT OUTER JOIN
                    //							 (

                    //       SELECT    CASE WHEN WP_DISCO_CDP_MONTHLY_DATA_1.IS_PRIMARY_METER='Y' THEN SUM(ABS(WP_DISCO_CDP_MONTHLY_DATA_1.P_ENG_ACTIVE_IMPORT - WP_DISCO_CDP_MONTHLY_DATA_1.P_ENG_ACTIVE_EXPORT)) ELSE   SUM(ABS(WP_DISCO_CDP_MONTHLY_DATA_1.B_ENG_ACTIVE_IMPORT - WP_DISCO_CDP_MONTHLY_DATA_1.B_ENG_ACTIVE_EXPORT)) END AS [ENG DELIVERED], 
                    //										CASE WHEN WP_DISCO_CDP_MONTHLY_DATA_1.IS_PRIMARY_METER='Y' THEN SUM(ABS(WP_DISCO_CDP_MONTHLY_DATA_1.P_MDI_ACTIVE_IMPORT - WP_DISCO_CDP_MONTHLY_DATA_1.P_MDI_ACTIVE_EXPORT)) ELSE SUM(ABS(WP_DISCO_CDP_MONTHLY_DATA_1.B_MDI_ACTIVE_IMPORT - WP_DISCO_CDP_MONTHLY_DATA_1.B_MDI_ACTIVE_EXPORT)) END AS MDI, CDXP.CPPA_CDP_HEADER.CUSTOMER_ID, 
                    //														 WP_DISCO_CDP_MONTHLY_DATA_1.ACCOUNTING_MONTH_ID
                    //							   FROM            CDXP.WP_DISCO_CDP_MONTHLY_DATA AS WP_DISCO_CDP_MONTHLY_DATA_1 INNER JOIN
                    //														 CDXP.CPPA_CDP_HEADER ON CDXP.CPPA_CDP_HEADER.CDP_ID = WP_DISCO_CDP_MONTHLY_DATA_1.CDP_ID
                    //							   GROUP BY CDXP.CPPA_CDP_HEADER.CUSTOMER_ID, WP_DISCO_CDP_MONTHLY_DATA_1.ACCOUNTING_MONTH_ID, WP_DISCO_CDP_MONTHLY_DATA_1.IS_PRIMARY_METER
                    //							   HAVING         (CDXP.CPPA_CDP_HEADER.CUSTOMER_ID IN
                    //															 (SELECT        WP_GC_USER_ACCESS_2.ENTITY_ID
                    //															   FROM            CDXP.WP_GC_USER_ACCESS AS WP_GC_USER_ACCESS_2 INNER JOIN
                    //																						 CDXP.WP_PORTAL_USERS AS WP_PORTAL_USERS_2 ON WP_PORTAL_USERS_2.WP_PORTAL_USERS_ID = WP_GC_USER_ACCESS_2.WP_PORTAL_USERS_ID
                    //															   WHERE        (WP_PORTAL_USERS_2.USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"')))) AS ABSTBL ON AM.ACCOUNTING_MONTH_ID = ABSTBL.ACCOUNTING_MONTH_ID order by AM.ACCOUNTING_MONTH_ID desc
                    //", "tblJ1");
                    //						break;


                    case 4:
                        Returnvls = Fn.HTMLTableWithID_TR_TagNext(@"SELECT        AM.ACCOUNTING_MONTH_ID , [CDXP].[DisableEditButtonDashBoard](AM.ACCOUNTING_MONTH_ID, '" + Convert.ToString(Session["UserName"]) + @"') as DisableEditButton, '' AS Sr#, UPPER(DATENAME(MONTH, CAST(AM.MONTH_NUMBER AS VARCHAR(2)) + '-01-' + CAST(AM.YEAR_NUMBER AS VARCHAR(4)))) + ' - ' + CAST(AM.YEAR_NUMBER AS VARCHAR(4)) 
                         AS DISPLAY, [CDXP].[GetDashBoardSummary](AM.ACCOUNTING_MONTH_ID, '" + Convert.ToString(Session["UserName"]) + @"') as Summary, CDXP.GetDiscoWisePercentage(AM.ACCOUNTING_MONTH_ID, customer_id) AS [Completed %], 
                              case when [CDXP].[DisableEditButtonDashBoard](AM.ACCOUNTING_MONTH_ID, '" + Convert.ToString(Session["UserName"]) + @"') = 0 then 'Draft'
                                    when [CDXP].[DisableEditButtonDashBoard](AM.ACCOUNTING_MONTH_ID, '" + Convert.ToString(Session["UserName"]) + @"') = 1 then 'Submitted'
                                    when [CDXP].[DisableEditButtonDashBoard](AM.ACCOUNTING_MONTH_ID, '" + Convert.ToString(Session["UserName"]) + @"') = 2 then 'Received'
                                    when [CDXP].[DisableEditButtonDashBoard](AM.ACCOUNTING_MONTH_ID, '" + Convert.ToString(Session["UserName"]) + @"') = 3 then 'Returned'
                                    else 'INVALID'
                                end AS Status,                                  
                         ISNULL(CAST(ABSTBL.[ENG DELIVERED] AS VARCHAR(100)),'') [ENG DELIVERED],  ISNULL(CAST(ABSTBL.MDI AS VARCHAR(100)),'') [MDI]
                        FROM            CDXP.ACCOUNTING_MONTH AS AM LEFT OUTER JOIN
                             (

                             SELECT SUM(innerqry.[ENG DELIVERED]) [ENG DELIVERED], SUM(innerqry.MDI) MDI, innerqry.CUSTOMER_ID, innerqry.ACCOUNTING_MONTH_ID FROM (
       SELECT    CASE WHEN WP_DISCO_CDP_MONTHLY_DATA_1.IS_PRIMARY_METER='Y' THEN SUM(ABS(ISNULL(WP_DISCO_CDP_MONTHLY_DATA_1.P_ENG_ACTIVE_IMPORT,0) - ISNULL(WP_DISCO_CDP_MONTHLY_DATA_1.P_ENG_ACTIVE_EXPORT,0))) ELSE   SUM(ABS(ISNULL(WP_DISCO_CDP_MONTHLY_DATA_1.B_ENG_ACTIVE_IMPORT,0) - ISNULL(WP_DISCO_CDP_MONTHLY_DATA_1.B_ENG_ACTIVE_EXPORT,0))) END AS [ENG DELIVERED], 
                                        CASE WHEN WP_DISCO_CDP_MONTHLY_DATA_1.IS_PRIMARY_METER='Y' THEN SUM(ABS(ISNULL(WP_DISCO_CDP_MONTHLY_DATA_1.P_MDI_ACTIVE_IMPORT,0) - ISNULL(WP_DISCO_CDP_MONTHLY_DATA_1.P_MDI_ACTIVE_EXPORT,0))) ELSE SUM(ABS(ISNULL(WP_DISCO_CDP_MONTHLY_DATA_1.B_MDI_ACTIVE_IMPORT,0) - ISNULL(WP_DISCO_CDP_MONTHLY_DATA_1.B_MDI_ACTIVE_EXPORT,0))) END AS MDI, CDXP.CPPA_CDP_HEADER.CUSTOMER_ID, 
                                                         WP_DISCO_CDP_MONTHLY_DATA_1.ACCOUNTING_MONTH_ID
                               FROM            CDXP.WP_DISCO_CDP_MONTHLY_DATA AS WP_DISCO_CDP_MONTHLY_DATA_1 INNER JOIN
                                                         CDXP.CPPA_CDP_HEADER ON CDXP.CPPA_CDP_HEADER.CDP_ID = WP_DISCO_CDP_MONTHLY_DATA_1.CDP_ID
                               GROUP BY CDXP.CPPA_CDP_HEADER.CUSTOMER_ID, WP_DISCO_CDP_MONTHLY_DATA_1.ACCOUNTING_MONTH_ID, WP_DISCO_CDP_MONTHLY_DATA_1.IS_PRIMARY_METER
                               HAVING         (CDXP.CPPA_CDP_HEADER.CUSTOMER_ID IN
                                                             (SELECT        WP_GC_USER_ACCESS_2.ENTITY_ID
                                                               FROM            CDXP.WP_GC_USER_ACCESS AS WP_GC_USER_ACCESS_2 INNER JOIN
                                                                                         CDXP.WP_PORTAL_USERS AS WP_PORTAL_USERS_2 ON WP_PORTAL_USERS_2.WP_PORTAL_USERS_ID = WP_GC_USER_ACCESS_2.WP_PORTAL_USERS_ID
                                                               WHERE        (WP_PORTAL_USERS_2.USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"')))
                                                               
                                                             )  AS innerqry GROUP BY innerqry.CUSTOMER_ID, innerqry.ACCOUNTING_MONTH_ID
                                                               
                                                               
                                                               ) AS ABSTBL ON AM.ACCOUNTING_MONTH_ID = ABSTBL.ACCOUNTING_MONTH_ID order by AM.ACCOUNTING_MONTH_ID desc
", "tblJ1");
                        break;

                    case 5:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT CUSTOMER_ID, CASE WHEN isnull([Energy (kWh)],'') = '' and isnull([MDI (kW)],'') = '' then 1 else  0 end as DisableEditButton, 
Sr#, [Distribution Company], [Received %], [Energy (kWh)], [MDI (kW)] FROM (
SELECT        CDXP.WP_SETUP_DISCO.CUSTOMER_ID, 0 AS DISABLEEDITBUTTON,  '' [Sr#], CDXP.WP_SETUP_DISCO.ABBREVIATION [Distribution Company], [CDXP].[GetDiscoWisePercentage](" + dataID[1] + @", CDXP.WP_SETUP_DISCO.CUSTOMER_ID) [Received %], ISNULL(DCTOTAL.[ENG DELIVERED],'') [Energy (kWh)], ISNULL(DCTOTAL.MDI,'') [MDI (kW)]
FROM            CDXP.WP_SETUP_DISCO INNER JOIN
                         CDXP.WP_GC_USER_ACCESS ON CDXP.WP_GC_USER_ACCESS.ENTITY_ID = CDXP.WP_SETUP_DISCO.CUSTOMER_ID INNER JOIN
                         CDXP.WP_PORTAL_USERS ON CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = CDXP.WP_PORTAL_USERS.WP_PORTAL_USERS_ID LEFT OUTER JOIN
                             (SELECT        AM.ACCOUNTING_MONTH_ID, '' AS Sr#, UPPER(DATENAME(MONTH, CAST(AM.MONTH_NUMBER AS VARCHAR(2)) + '-01-' + CAST(AM.YEAR_NUMBER AS VARCHAR(4)))) 
                                                         + ' - ' + CAST(AM.YEAR_NUMBER AS VARCHAR(4)) AS DISPLAY,  'Draft' AS Status, 
                                                         ISNULL(CAST(ABSTBL.[ENG DELIVERED] AS VARCHAR(100)), '') AS [ENG DELIVERED], ISNULL(CAST(ABSTBL.MDI AS VARCHAR(100)), '') AS MDI, ABSTBL.CUSTOMER_ID
                               FROM            CDXP.ACCOUNTING_MONTH AS AM INNER JOIN
                                                             (SELECT        SUM(ABS(CASE WHEN WP_DISCO_CDP_MONTHLY_DATA_1.IS_PRIMARY_METER = 'Y' THEN  (WP_DISCO_CDP_MONTHLY_DATA_1.P_ENG_ACTIVE_IMPORT - WP_DISCO_CDP_MONTHLY_DATA_1.P_ENG_ACTIVE_EXPORT) ELSE (WP_DISCO_CDP_MONTHLY_DATA_1.B_ENG_ACTIVE_IMPORT - WP_DISCO_CDP_MONTHLY_DATA_1.B_ENG_ACTIVE_EXPORT) END )) AS [ENG DELIVERED], 
                                                                                         SUM(ABS(
                                                                                         CASE WHEN WP_DISCO_CDP_MONTHLY_DATA_1.IS_PRIMARY_METER = 'Y' THEN  (WP_DISCO_CDP_MONTHLY_DATA_1.P_MDI_ACTIVE_IMPORT - WP_DISCO_CDP_MONTHLY_DATA_1.P_MDI_ACTIVE_EXPORT) ELSE
                                                                                         (WP_DISCO_CDP_MONTHLY_DATA_1.B_MDI_ACTIVE_IMPORT - WP_DISCO_CDP_MONTHLY_DATA_1.B_MDI_ACTIVE_EXPORT) END
                                                                                         )) AS MDI, CDXP.CPPA_CDP_HEADER.CUSTOMER_ID, 
                                                                                         WP_DISCO_CDP_MONTHLY_DATA_1.ACCOUNTING_MONTH_ID
                                                               FROM            CDXP.WP_DISCO_CDP_MONTHLY_DATA AS WP_DISCO_CDP_MONTHLY_DATA_1 INNER JOIN
                                                                                         CDXP.CPPA_CDP_HEADER ON CDXP.CPPA_CDP_HEADER.CDP_ID = WP_DISCO_CDP_MONTHLY_DATA_1.CDP_ID
                                                               GROUP BY CDXP.CPPA_CDP_HEADER.CUSTOMER_ID, WP_DISCO_CDP_MONTHLY_DATA_1.ACCOUNTING_MONTH_ID
                                                               HAVING         (CDXP.CPPA_CDP_HEADER.CUSTOMER_ID IN
                                                                                             (SELECT        WP_GC_USER_ACCESS_2.ENTITY_ID
                                                                                               FROM            CDXP.WP_GC_USER_ACCESS AS WP_GC_USER_ACCESS_2 INNER JOIN
                                                                                                                         CDXP.WP_PORTAL_USERS AS WP_PORTAL_USERS_2 ON WP_PORTAL_USERS_2.WP_PORTAL_USERS_ID = WP_GC_USER_ACCESS_2.WP_PORTAL_USERS_ID
                                                                                               WHERE        (WP_PORTAL_USERS_2.USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"')) AND WP_DISCO_CDP_MONTHLY_DATA_1.ACCOUNTING_MONTH_ID=" + dataID[1] + @")) AS ABSTBL ON AM.ACCOUNTING_MONTH_ID = ABSTBL.ACCOUNTING_MONTH_ID
                               WHERE        (AM.ACCOUNTING_MONTH_ID = " + dataID[1] + @")) AS DCTOTAL ON CDXP.WP_SETUP_DISCO.CUSTOMER_ID = DCTOTAL.CUSTOMER_ID
WHERE        (CDXP.WP_PORTAL_USERS.USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"')) TABLEA", "tblJ1");
                        break;
                    case 6:
                        sql = " UPDATE [CDXP].[WP_DISCO_CDP_MONTHLY_DATA] SET [Status] = 1, ";
                        sql += " LAST_UPDATE_BY_WP_PORTAL_USERS_ID = CDXP.FN_WP_PORTAL_USERS_ID_BY_USER_NAME('" + Convert.ToString(Session["UserName"]) + @"'), ";
                        sql += " LAST_UPDATE_DATE = '" + serverDate + @"'";
                        sql += " WHERE Accounting_Month_ID = " + dataID[1] + "; select '1'; ";

                        Returnvls = Fn.ExenID(sql);

                        break;

                    case 7:
                        sql = " UPDATE [CDXP].[WP_DISCO_CDP_MONTHLY_DATA] SET [Status] = 1, ";
                        sql += " LAST_UPDATE_BY_WP_PORTAL_USERS_ID = CDXP.FN_WP_PORTAL_USERS_ID_BY_USER_NAME('" + Convert.ToString(Session["UserName"]) + @"'), ";
                        sql += " LAST_UPDATE_DATE = '" + serverDate + @"'";
                        sql += " WHERE WP_DISCO_CDP_MONTHLY_DATA_ID in ( " + dataID[4] + "); select '1'; ";


                        Returnvls = Fn.ExenID(sql);

                        break;

                    case 8:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT    CDXP.CPPA_CDP_HEADER.CDP_ID ID, '' [Sr #],    CDXP.CPPA_CDP_HEADER.CDP_ID AS [CDP #], CDXP.CPPA_CDP_HEADER.GRID_STATION_NAME AS [NAME OF G/S], CDXP.CPPA_CDP_HEADER.TF_LINE_VOLTAGE AS Voltage, 
                         CDXP.CPPA_CDP_HEADER.TF_TL AS [TF/TL], CDXP.CPPA_CDP_HEADER.DISCO_AREA_CODE AS Area, CDXP.CPPA_CDP_HEADER.ENGY_IMP_EXP_TO_CODE AS [Eng Import From], 
                         CDXP.CPPA_CDP_HEADER.ENGY_IMP_EXP_FROM_CODE AS [Eng Export To], ISNULL(CAST(MD.[Primary Meter No] AS VARCHAR(100)), '') AS [Primary Meter No], ISNULL(CAST(MD.[Backup Meter No] AS VARCHAR(100)), '') 
                         AS [Backup Meter No], CASE WHEN ISNULL(MD.IS_PRIMARY_METER, '') = 'Y' THEN 'PRIMARY METER' WHEN ISNULL(MD.IS_PRIMARY_METER, '') = 'N' THEN 'BACKUP METER' ELSE '' END AS [Billing Meter], 
                         ISNULL(CAST(MD.ENG_ACTIVE_IMPORT AS VARCHAR(100)),'') [Eng Import], 
                         ISNULL(CAST(MD.ENG_ACTIVE_EXPORT AS VARCHAR(100)),'') [Eng Export], 
                         ISNULL(CAST(MD.MDI_ACTIVE_IMPORT AS VARCHAR(100)),'') [MDI Import], 
                         ISNULL(CAST(MD.MDI_ACTIVE_EXPORT AS VARCHAR(100)),'') [MDI EXPORT]
                         FROM CDXP.CPPA_CDP_HEADER LEFT OUTER JOIN
                             (SELECT        CDXP.WP_DISCO_CDP_MONTHLY_DATA.CDP_ID, CDXP.WP_DISCO_CDP_MONTHLY_DATA.IS_PRIMARY_METER, CPPA_CDP_DETAIL_1.METER_NO AS [Primary Meter No], 
                                                         CDXP.CPPA_CDP_DETAIL.METER_NO AS [Backup Meter No], CDXP.WP_DISCO_CDP_MONTHLY_DATA.ENG_ACTIVE_IMPORT, CDXP.WP_DISCO_CDP_MONTHLY_DATA.ENG_ACTIVE_EXPORT, 
                                                         CDXP.WP_DISCO_CDP_MONTHLY_DATA.MDI_ACTIVE_IMPORT, CDXP.WP_DISCO_CDP_MONTHLY_DATA.MDI_ACTIVE_EXPORT
                               FROM            CDXP.WP_DISCO_CDP_MONTHLY_DATA INNER JOIN
                                                         CDXP.CPPA_CDP_DETAIL AS CPPA_CDP_DETAIL_1 ON CDXP.WP_DISCO_CDP_MONTHLY_DATA.PRIMARY_CDP_DTL_ID = CPPA_CDP_DETAIL_1.CDP_DTL_ID INNER JOIN
                                                         CDXP.CPPA_CDP_DETAIL ON CDXP.WP_DISCO_CDP_MONTHLY_DATA.BACKUP_CDP_DTL_ID = CDXP.CPPA_CDP_DETAIL.CDP_DTL_ID
                               WHERE        (CDXP.WP_DISCO_CDP_MONTHLY_DATA.ACCOUNTING_MONTH_ID = " + dataID[2] + @") and ISNULL(status,0) != 0) AS MD ON CDXP.CPPA_CDP_HEADER.CDP_ID = MD.CDP_ID
                         WHERE        (CDXP.CPPA_CDP_HEADER.CUSTOMER_ID = " + dataID[1] + ")", "tblJ1");
                        break;


                    case 9:

                        Returnvls = Fn.HTMLTableWithID_TR_Tag2(@"EXEC [CDXP].[SP_DISCO_MONTHLY_REPORT_CPPA_VERIFICATION] " + dataID[1] + ", " + dataID[2] + ", " + dataID[3] + "," + "NULL" + ", '" + dataID[4] + "'", "tblJ1", dataID[3]);
                        //Returnvls = Fn.HTMLTableWithID_TR_Tag(@"EXEC [CDXP].[SP_DISCO_MONTHLY_REPORT_CPPA_VERIFICATION] " + dataID[1] + @", " + dataID[2] + ", " + dataID[3] + ",'" + dataID[4] + "'", "tblJ1");
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
        public ActionResult MRP()
        {
            Fn.sessionCheck();
            var frmdata = HttpContext.Request.Form["vls"];
            string id = frmdata;
            if (id != null)
            {
                ViewBag.WP_DISCO_CDP_MONTHLY_DATA_ID = "0";
                ViewBag.Condition = "N";
                if (id.Split('½')[5] != "N")
                {
                    ViewBag.WP_DISCO_CDP_MONTHLY_DATA_ID = id.Split('½')[4];
                    ViewBag.Condition = id.Split('½')[5];
                }
                ViewBag.closeCondition = Convert.ToString(id.Split('½')[6]);
                ViewBag.inputvals = id;
                ViewBag.ddlDISCOs = "<option value='" + id.Split('½')[0] + @"'>" + id.Split('½')[1] + @"</option>";
                ViewBag.ddlMonths = "<option value='" + id.Split('½')[2] + @"'>" + id.Split('½')[3] + @"</option>";
                ViewBag.ddlCDPNumber_tg = id.Split('½')[4] + "½" + id.Split('½')[5];
                ViewBag.FrmJson = Fn.Data2Json("");

            }
            return View();
        }


        [HttpPost]
        public ActionResult MeterReadingProforma()
        {
            Fn.sessionCheck();
            ViewBag.Title = "Meter Reading Proforma";
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                var userName = Convert.ToString(Session["UserName"]);
                ViewBag.ddlCDPNumber = Fn.Data2Dropdown(db.SP_ddlCDPNumber(userName).ToList<SP_ddlCDPNumber_Result>());
            }

            //DateTime utc = DateTime.UtcNow;
            //TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            //DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);

            //ViewBag.dtToday = pkdatetime.ToString("dd-MMM-yyyy HH:mm");
            return View();
        }

        [HttpPost]
        public string AjaxCall()
        {
            Fn.sessionCheck();
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string serverDate = pkdatetime.ToString("dd-MMM-yyyy HH:mm");
            var frmdata = HttpContext.Request.Form["vls"];
            string[] dataID = new string[500];
            string sql = "";

            dataID = Fn.CleanSQL(HttpUtility.UrlDecode(frmdata)).Split('½');
            string Returnvls = "", Returnvls1 = "";
            try
            {
                switch (Convert.ToInt32(dataID[0]))
                {
                    case 0:
                        Returnvls = Fn.Data2Json(@"EXEC [CDXP].[SP_CPPA_CDP_HEADER_DETAIL_BY_CDP_ID] " + dataID[1]);
                        break;

                    case 1:
                        //Returnvls = Fn.Data2Json(@"EXEC [CDXP].[SP_LAST_METER_Reading_By_CDP_DTL_ID] " + dataID[1] + ", '" + dataID[2]+"'");
                        Returnvls = Fn.Data2Json(@"EXEC [CDXP].[SP_LAST_METER_Reading_By_CDP_DTL_ID] " + dataID[1] + ", '" + dataID[2] + "', '" + dataID[3] + @"'");
                        break;

                    case 2:
                        if (dataID[1] == "0")
                        {
                            Returnvls = Fn.Exec(@"INSERT INTO CDXP.WP_DISCO_CDP_MONTHLY_DATA (
CUSTOMER_ID, CDP_ID,TRANSACTION_TYPE, PRIMARY_CDP_DTL_ID, BACKUP_CDP_DTL_ID, ACCOUNTING_MONTH_ID, PM_READING_DATE_TIME, BM_READING_DATE_TIME, FROM_DATE, TO_DATE,  
                         PRIMARY_IMPORT_ACTIVE_READING, PRIMARY_IMPORT_REACTIVE_READING, PRIMARY_EXPORT_ACTIVE_READING, PRIMARY_EXPORT_REACTIVE_READING, BACKUP_IMPORT_ACTIVE_READING, 
                         BACKUP_IMPORT_REACTIVE_READING, BACKUP_EXPORT_ACTIVE_READING, BACKUP_EXPORT_REACTIVE_READING, PRIMARY_IMPORT_ACTIVE_AJD, PRIMARY_IMPORT_REACTIVE_ADJ, PRIMARY_EXPORT_ACTIVE_AJD, 
                         PRIMARY_EXPORT_REACTIVE_ADJ, BACKUP_IMPORT_ACTIVE_AJD, BACKUP_IMPORT_REACTIVE_ADJ, BACKUP_EXPORT_ACTIVE_AJD, BACKUP_EXPORT_REACTIVE_ADJ, PRIMARY_IMPORT_ACTIVE_MDI, 
                         PRIMARY_IMPORT_REACTIVE_MDI, PRIMARY_EXPORT_ACTIVE_MDI, PRIMARY_EXPORT_REACTIVE_MDI, BACKUP_IMPORT_ACTIVE_MDI, BACKUP_IMPORT_REACTIVE_MDI, BACKUP_EXPORT_ACTIVE_MDI, 
                         BACKUP_EXPORT_REACTIVE_MDI, IS_PRIMARY_METER, REMARKS, P_ENG_ACTIVE_IMPORT, P_ENG_ACTIVE_EXPORT, P_MDI_ACTIVE_IMPORT, P_MDI_ACTIVE_EXPORT, B_ENG_ACTIVE_IMPORT, B_ENG_ACTIVE_EXPORT, 
                         B_MDI_ACTIVE_IMPORT, B_MDI_ACTIVE_EXPORT,PM_PREVIOUS_READING_DATE_TIME,BM_PREVIOUS_READING_DATE_TIME,PREVIOUS_READING_PRIM_IMP_ACT,PREVIOUS_READING_PRIM_IMP_REACT,PREVIOUS_READING_PRIM_EXP_ACT,PREVIOUS_READING_PRIM_EXP_REACT,PREVIOUS_READING_BACK_IMP_ACT,PREVIOUS_READING_BACK_IMP_REACT,PREVIOUS_READING_BACK_EXP_ACT,PREVIOUS_READING_BACK_EXP_REACT, CREATION_DATE,CREATED_BY_WP_PORTAL_USERS_ID) VALUES (
'" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"','" + dataID[7] + @"','" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"'
,'" + dataID[11] + @"','" + dataID[12] + @"','" + dataID[13] + @"','" + dataID[14] + @"','" + dataID[15] + @"','" + dataID[16] + @"','" + dataID[17] + @"','" + dataID[18] + @"','" + dataID[19] + @"','" + dataID[20] + @"'
,'" + dataID[21] + @"','" + dataID[22] + @"','" + dataID[23] + @"','" + dataID[24] + @"','" + dataID[25] + @"','" + dataID[26] + @"','" + dataID[27] + @"','" + dataID[28] + @"','" + dataID[29] + @"','" + dataID[30] + @"'
,'" + dataID[31] + @"','" + dataID[32] + @"','" + dataID[33] + @"','" + dataID[34] + @"','" + dataID[35] + @"','" + dataID[36] + @"','" + dataID[37] + @"','" + dataID[38] + @"','" + dataID[39] + @"','" + dataID[40] + @"'
,'" + dataID[41] + @"','" + dataID[42] + @"','" + dataID[43] + @"','" + dataID[44] + @"','" + dataID[45] + @"'
,'" + dataID[46] + @"'
,'" + dataID[47] + @"'
,'" + dataID[48] + @"'
,'" + dataID[49] + @"'
,'" + dataID[50] + @"'
,'" + dataID[51] + @"'
,'" + dataID[52] + @"'
,'" + dataID[53] + @"'
,'" + dataID[54] + @"'
,'" + dataID[55] + @"','" + serverDate + @"', [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')
);");

                        }
                        else
                        {
                            Returnvls = Fn.Exec(@"UPDATE       CDXP.WP_DISCO_CDP_MONTHLY_DATA
SET                CUSTOMER_ID = '" + dataID[2] + @"', CDP_ID = '" + dataID[3] + @"', TRANSACTION_TYPE = '" + dataID[4] + @"', PRIMARY_CDP_DTL_ID = '" + dataID[5] + @"', BACKUP_CDP_DTL_ID = '" + dataID[6] + @"', ACCOUNTING_MONTH_ID = '" + dataID[7] + @"', 
                         PM_READING_DATE_TIME = '" + dataID[8] + @"', BM_READING_DATE_TIME = '" + dataID[9] + @"', FROM_DATE = '" + dataID[10] + @"', TO_DATE = '" + dataID[11] + @"', 
                         PRIMARY_IMPORT_ACTIVE_READING = '" + dataID[12] + @"', PRIMARY_IMPORT_REACTIVE_READING = '" + dataID[13] + @"', PRIMARY_EXPORT_ACTIVE_READING = '" + dataID[14] + @"', 
                         PRIMARY_EXPORT_REACTIVE_READING = '" + dataID[15] + @"', BACKUP_IMPORT_ACTIVE_READING = '" + dataID[16] + @"', BACKUP_IMPORT_REACTIVE_READING = '" + dataID[17] + @"', 
                         BACKUP_EXPORT_ACTIVE_READING = '" + dataID[18] + @"', BACKUP_EXPORT_REACTIVE_READING = '" + dataID[19] + @"', PRIMARY_IMPORT_ACTIVE_AJD = '" + dataID[20] + @"', 
                         PRIMARY_IMPORT_REACTIVE_ADJ = '" + dataID[21] + @"', PRIMARY_EXPORT_ACTIVE_AJD = '" + dataID[22] + @"', PRIMARY_EXPORT_REACTIVE_ADJ = '" + dataID[23] + @"', 
                         BACKUP_IMPORT_ACTIVE_AJD = '" + dataID[24] + @"', BACKUP_IMPORT_REACTIVE_ADJ = '" + dataID[25] + @"', BACKUP_EXPORT_ACTIVE_AJD = '" + dataID[26] + @"', BACKUP_EXPORT_REACTIVE_ADJ = '" + dataID[27] + @"',
                          PRIMARY_IMPORT_ACTIVE_MDI = '" + dataID[28] + @"', PRIMARY_IMPORT_REACTIVE_MDI = '" + dataID[29] + @"', PRIMARY_EXPORT_ACTIVE_MDI = '" + dataID[30] + @"', 
                         PRIMARY_EXPORT_REACTIVE_MDI = '" + dataID[31] + @"', BACKUP_IMPORT_ACTIVE_MDI = '" + dataID[32] + @"', BACKUP_IMPORT_REACTIVE_MDI = '" + dataID[33] + @"', 
                         BACKUP_EXPORT_ACTIVE_MDI = '" + dataID[34] + @"', BACKUP_EXPORT_REACTIVE_MDI = '" + dataID[35] + @"', IS_PRIMARY_METER = '" + dataID[36] + @"', REMARKS = '" + dataID[37] + @"', 
                         P_ENG_ACTIVE_IMPORT = '" + dataID[38] + @"', P_ENG_ACTIVE_EXPORT = '" + dataID[39] + @"', P_MDI_ACTIVE_IMPORT = '" + dataID[40] + @"', P_MDI_ACTIVE_EXPORT = '" + dataID[41] + @"', 
                         B_ENG_ACTIVE_IMPORT = '" + dataID[42] + @"', B_ENG_ACTIVE_EXPORT = '" + dataID[43] + @"', B_MDI_ACTIVE_IMPORT = '" + dataID[44] + @"', B_MDI_ACTIVE_EXPORT = '" + dataID[45] + @"', 
                         PM_PREVIOUS_READING_DATE_TIME = '" + dataID[46] + @"', BM_PREVIOUS_READING_DATE_TIME = '" + dataID[47] + @"', PREVIOUS_READING_PRIM_IMP_ACT = '" + dataID[48] + @"', 
                         PREVIOUS_READING_PRIM_IMP_REACT = '" + dataID[49] + @"', PREVIOUS_READING_PRIM_EXP_ACT = '" + dataID[50] + @"', PREVIOUS_READING_PRIM_EXP_REACT = '" + dataID[51] + @"', 
                         PREVIOUS_READING_BACK_IMP_ACT = '" + dataID[52] + @"', PREVIOUS_READING_BACK_IMP_REACT = '" + dataID[53] + @"', PREVIOUS_READING_BACK_EXP_ACT = '" + dataID[54] + @"', 
                         PREVIOUS_READING_BACK_EXP_REACT = '" + dataID[55] + @"' WHERE        (WP_DISCO_CDP_MONTHLY_DATA_ID = " + dataID[1] + ") ");
                        }
                        break;

                    case 4:
                        //                 Returnvls = Fn.HTMLTableWithID_TR_TagNext(@"SELECT        AM.ACCOUNTING_MONTH_ID , 0 as DisableEditButton, '' AS Sr#, UPPER(DATENAME(MONTH, CAST(AM.MONTH_NUMBER AS VARCHAR(2)) + '-01-' + CAST(AM.YEAR_NUMBER AS VARCHAR(4)))) + ' - ' + CAST(AM.YEAR_NUMBER AS VARCHAR(4)) 
                        //                  AS [Billing Month], [CDXP].[GetDashBoardSummary](AM.ACCOUNTING_MONTH_ID, '" + Convert.ToString(Session["UserName"]) + @"') as Summary, CDXP.GetDiscoWisePercentageCDP(AM.ACCOUNTING_MONTH_ID, customer_id) AS [Submission %], 

                        //                  ISNULL(CAST(ABSTBL.[ENG DELIVERED] AS VARCHAR(100)),'') [Energy Delivered (kWh)],  ISNULL(CAST(ABSTBL.MDI AS VARCHAR(100)),'') [MDI (kW)]
                        //                 FROM            CDXP.ACCOUNTING_MONTH AS AM LEFT OUTER JOIN
                        //                      (

                        //                      SELECT SUM(innerqry.[ENG DELIVERED]) [ENG DELIVERED], SUM(innerqry.MDI) MDI, innerqry.CUSTOMER_ID, innerqry.ACCOUNTING_MONTH_ID FROM (
                        //SELECT    CASE WHEN WP_DISCO_CDP_MONTHLY_DATA_1.IS_PRIMARY_METER='Y' THEN SUM(ISNULL(WP_DISCO_CDP_MONTHLY_DATA_1.P_ENG_ACTIVE_IMPORT,0) - ISNULL(WP_DISCO_CDP_MONTHLY_DATA_1.P_ENG_ACTIVE_EXPORT,0)) ELSE   SUM(ISNULL(WP_DISCO_CDP_MONTHLY_DATA_1.B_ENG_ACTIVE_IMPORT,0) - ISNULL(WP_DISCO_CDP_MONTHLY_DATA_1.B_ENG_ACTIVE_EXPORT,0)) END AS [ENG DELIVERED], 
                        //                                 CASE WHEN WP_DISCO_CDP_MONTHLY_DATA_1.IS_PRIMARY_METER='Y' THEN SUM(ISNULL(WP_DISCO_CDP_MONTHLY_DATA_1.P_MDI_ACTIVE_IMPORT,0) - ISNULL(WP_DISCO_CDP_MONTHLY_DATA_1.P_MDI_ACTIVE_EXPORT,0)) ELSE SUM(ISNULL(WP_DISCO_CDP_MONTHLY_DATA_1.B_MDI_ACTIVE_IMPORT,0) - ISNULL(WP_DISCO_CDP_MONTHLY_DATA_1.B_MDI_ACTIVE_EXPORT,0)) END AS MDI, WP_DISCO_CDP_MONTHLY_DATA_1.CUSTOMER_ID, 
                        //                                                  WP_DISCO_CDP_MONTHLY_DATA_1.ACCOUNTING_MONTH_ID
                        //                        FROM            CDXP.WP_DISCO_CDP_MONTHLY_DATA AS WP_DISCO_CDP_MONTHLY_DATA_1 WHERE WP_DISCO_CDP_MONTHLY_DATA_1.STATUS NOT IN (0,3)
                        //                        GROUP BY WP_DISCO_CDP_MONTHLY_DATA_1.CUSTOMER_ID, WP_DISCO_CDP_MONTHLY_DATA_1.ACCOUNTING_MONTH_ID, WP_DISCO_CDP_MONTHLY_DATA_1.IS_PRIMARY_METER
                        //                        HAVING         (WP_DISCO_CDP_MONTHLY_DATA_1.CUSTOMER_ID IN
                        //                                                      (SELECT        WP_GC_USER_ACCESS_2.ENTITY_ID
                        //                                                        FROM            CDXP.WP_GC_USER_ACCESS AS WP_GC_USER_ACCESS_2 INNER JOIN
                        //                                                                                  CDXP.WP_PORTAL_USERS AS WP_PORTAL_USERS_2 ON WP_PORTAL_USERS_2.WP_PORTAL_USERS_ID = WP_GC_USER_ACCESS_2.WP_PORTAL_USERS_ID
                        //                                                        WHERE        (WP_PORTAL_USERS_2.USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"')))

                        //                                                      )  AS innerqry GROUP BY innerqry.CUSTOMER_ID, innerqry.ACCOUNTING_MONTH_ID


                        //                                                        ) AS ABSTBL ON AM.ACCOUNTING_MONTH_ID = ABSTBL.ACCOUNTING_MONTH_ID  WHERE AM.IS_ACTIVE=1 order by AM.ACCOUNTING_MONTH_ID desc ", "tblJ1");

                        Returnvls = Fn.HTMLTableWithID_TR_TagNext(@"SELECT        AM.ACCOUNTING_MONTH_ID, 0 AS DisableEditButton, '' AS Sr#, UPPER(DATENAME(MONTH, CAST(AM.MONTH_NUMBER AS VARCHAR(2)) + '-01-' + CAST(AM.YEAR_NUMBER AS VARCHAR(4)))) 
                         + ' - ' + CAST(AM.YEAR_NUMBER AS VARCHAR(4)) AS [Billing Month], CDXP.GetDashBoardSummary(AM.ACCOUNTING_MONTH_ID, '" + Convert.ToString(Session["UserName"]) + @"') AS Summary, 
                         CDXP.GetDiscoWisePercentageCDP(AM.ACCOUNTING_MONTH_ID, ABSTBL.CUSTOMER_ID) AS [Submission %], ISNULL(CAST(ABSTBL.[ENG DELIVERED] AS VARCHAR(100)), '') AS [Energy Delivered (kWh)], 
                         ISNULL(CAST(ABSTBL.MDI AS VARCHAR(100)), '') AS [MDI (kW)]
FROM            CDXP.ACCOUNTING_MONTH AS AM LEFT OUTER JOIN
                             (SELECT        SUM([ENG DELIVERED]) AS [ENG DELIVERED], SUM(MDI) AS MDI, CUSTOMER_ID, ACCOUNTING_MONTH_ID
                               FROM            (SELECT        MD.CDP_ID, 
							   
CAST((CASE WHEN ISNULL(CAST(CDXP.CPPA_CDP_HEADER.Auxilary AS VARCHAR(100)), '0') != '1' AND 
                                                                                   CDXP.CPPA_CDP_HEADER.CUSTOMER_ID != CDXP.CPPA_CDP_HEADER.FROM_CUSTOMER_ID THEN ISNULL(CAST(MD.ENG_ACTIVE_EXPORT AS VARCHAR(100)), '') 
                                                                                   WHEN (ISNULL(CAST(CDXP.CPPA_CDP_HEADER.Auxilary AS VARCHAR(100)), '0') = '1' AND CDXP.CPPA_CDP_HEADER.CUSTOMER_ID = CDXP.CPPA_CDP_HEADER.TO_CUSTOMER_ID) 
                                                                                   THEN ISNULL(CAST(MD.ENG_ACTIVE_EXPORT AS VARCHAR(100)), '') ELSE ISNULL(CAST(MD.ENG_ACTIVE_IMPORT AS VARCHAR(100)), '') END
																				   ) AS NUMERIC(18,6))
                                                                                   - 
																				   CAST((CASE WHEN ISNULL(CAST(CDXP.CPPA_CDP_HEADER.Auxilary AS VARCHAR(100)), '0') != '1' AND 
                                                                                   CDXP.CPPA_CDP_HEADER.CUSTOMER_ID != CDXP.CPPA_CDP_HEADER.FROM_CUSTOMER_ID THEN ISNULL(CAST(MD.ENG_ACTIVE_IMPORT AS VARCHAR(100)), '') 
                                                                                   WHEN (ISNULL(CAST(CDXP.CPPA_CDP_HEADER.Auxilary AS VARCHAR(100)), '0') = '1' AND CDXP.CPPA_CDP_HEADER.CUSTOMER_ID = CDXP.CPPA_CDP_HEADER.TO_CUSTOMER_ID) 
                                                                                   THEN ISNULL(CAST(MD.ENG_ACTIVE_IMPORT AS VARCHAR(100)), '') ELSE ISNULL(CAST(MD.ENG_ACTIVE_EXPORT AS VARCHAR(100)), '') END) AS NUMERIC(18,6))
																				   
																				   AS [ENG DELIVERED], 
                                                                                  CAST( (CASE WHEN ISNULL(CAST(CDXP.CPPA_CDP_HEADER.Auxilary AS VARCHAR(100)), '0') != '1' AND 
                                                                                   CDXP.CPPA_CDP_HEADER.CUSTOMER_ID != CDXP.CPPA_CDP_HEADER.FROM_CUSTOMER_ID THEN ISNULL(CAST(MD.MDI_ACTIVE_EXPORT AS VARCHAR(100)), '') 
                                                                                   WHEN (ISNULL(CAST(CDXP.CPPA_CDP_HEADER.Auxilary AS VARCHAR(100)), '0') = '1' AND CDXP.CPPA_CDP_HEADER.CUSTOMER_ID = CDXP.CPPA_CDP_HEADER.TO_CUSTOMER_ID) 
                                                                                   THEN ISNULL(CAST(MD.MDI_ACTIVE_EXPORT AS VARCHAR(100)), '') ELSE ISNULL(CAST(MD.MDI_ACTIVE_IMPORT AS VARCHAR(100)), '') END) 
																				   AS NUMERIC(18,6))
                                                                                   - CAST((CASE WHEN ISNULL(CAST(CDXP.CPPA_CDP_HEADER.Auxilary AS VARCHAR(100)), '0') != '1' AND 
                                                                                   CDXP.CPPA_CDP_HEADER.CUSTOMER_ID != CDXP.CPPA_CDP_HEADER.FROM_CUSTOMER_ID THEN ISNULL(CAST(MD.MDI_ACTIVE_IMPORT AS VARCHAR(100)), '') 
                                                                                   WHEN (ISNULL(CAST(CDXP.CPPA_CDP_HEADER.Auxilary AS VARCHAR(100)), '0') = '1' AND CDXP.CPPA_CDP_HEADER.CUSTOMER_ID = CDXP.CPPA_CDP_HEADER.TO_CUSTOMER_ID) 
                                                                                   THEN ISNULL(CAST(MD.MDI_ACTIVE_IMPORT AS VARCHAR(100)), '') ELSE ISNULL(CAST(MD.MDI_ACTIVE_EXPORT AS VARCHAR(100)), '') END) AS NUMERIC(18,6)) AS MDI, MD.CUSTOMER_ID, 
                                                                                   MD.ACCOUNTING_MONTH_ID
                                                         FROM            CDXP.CPPA_CDP_HEADER INNER JOIN
                                                                                       (SELECT        CDP_ID, SUM(CASE WHEN MD1.IS_PRIMARY_METER = 'Y' THEN MD1.P_ENG_ACTIVE_IMPORT ELSE MD1.B_ENG_ACTIVE_IMPORT END) AS ENG_ACTIVE_IMPORT, 
                                                                                                                   SUM(CASE WHEN MD1.IS_PRIMARY_METER = 'Y' THEN MD1.P_ENG_ACTIVE_EXPORT ELSE MD1.B_ENG_ACTIVE_EXPORT END) AS ENG_ACTIVE_EXPORT, 
                                                                                                                   SUM(CASE WHEN MD1.IS_PRIMARY_METER = 'Y' THEN MD1.P_MDI_ACTIVE_IMPORT ELSE MD1.B_MDI_ACTIVE_IMPORT END) AS MDI_ACTIVE_IMPORT, 
                                                                                                                   SUM(CASE WHEN MD1.IS_PRIMARY_METER = 'Y' THEN MD1.P_MDI_ACTIVE_EXPORT ELSE MD1.B_MDI_ACTIVE_EXPORT END) AS MDI_ACTIVE_EXPORT, CUSTOMER_ID, 
                                                                                                                   ACCOUNTING_MONTH_ID
                                                                                         FROM            CDXP.WP_DISCO_CDP_MONTHLY_DATA AS MD1
                                                                                         WHERE        (Status NOT IN (0, 3))
                                                                                         GROUP BY CUSTOMER_ID, ACCOUNTING_MONTH_ID, IS_PRIMARY_METER,CDP_ID
                                                                                         HAVING         (CUSTOMER_ID IN
                                                                                                                       (SELECT        WP_GC_USER_ACCESS_2.ENTITY_ID
                                                                                                                         FROM            CDXP.WP_GC_USER_ACCESS AS WP_GC_USER_ACCESS_2 INNER JOIN
                                                                                                                                                   CDXP.WP_PORTAL_USERS AS WP_PORTAL_USERS_2 ON 
                                                                                                                                                   WP_PORTAL_USERS_2.WP_PORTAL_USERS_ID = WP_GC_USER_ACCESS_2.WP_PORTAL_USERS_ID
                                                                                                                         WHERE        (WP_PORTAL_USERS_2.USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"')))) AS MD ON CDXP.CPPA_CDP_HEADER.CDP_ID = MD.CDP_ID AND 
                                                                                   CDXP.CPPA_CDP_HEADER.CUSTOMER_ID = MD.CUSTOMER_ID) AS innerqry
                               GROUP BY CUSTOMER_ID, ACCOUNTING_MONTH_ID) AS ABSTBL ON AM.ACCOUNTING_MONTH_ID = ABSTBL.ACCOUNTING_MONTH_ID
WHERE        (AM.IS_ACTIVE = 1)
ORDER BY AM.ACCOUNTING_MONTH_ID DESC ", "tblJ1");

                        break;

                    case 7:



                        Returnvls = Fn.Exec(@"UPDATE CDXP.WP_DISCO_CDP_MONTHLY_DATA SET Status = 1, SUBMITT_DATE_TIME='" + serverDate + "' WHERE CUSTOMER_ID='" + dataID[1] + "' AND ACCOUNTING_MONTH_ID='" + dataID[2] + "' AND CDP_ID IN (" + dataID[4] + ");");
                        string dtls = Fn.ExenID(@"SELECT ISNULL((SELECT CUSTOMER_NAME FROM [CDXP].[WP_SETUP_DISCO] WHERE CUSTOMER_ID = '" + dataID[1] + @"') ,'')+'½'+ISNULL(
(SELECT STRING_AGG(ISNULL(CDP_NUMBER,''), ', ') FROM [CDXP].[CPPA_CDP_HEADERS]  WHERE CDP_ID IN (" + dataID[4] + @")),'') +'½'+
ISNULL((
SELECT  UPPER(DATENAME(MONTH,CAST([MONTH_NUMBER] AS VARCHAR(2))+'-01-'+ CAST([YEAR_NUMBER] AS VARCHAR(4)))) + '-' + CAST([YEAR_NUMBER] AS VARCHAR(4)) AS DISPLAY FROM [CDXP].[ACCOUNTING_MONTH] WHERE  [ACCOUNTING_MONTH_ID]='" + dataID[2] + @"'
),'') + '½' + 
ISNULL((SELECT ISNULL(EMAIL_ADDRESSES,'') FROM [CDXP].[WP_PORTAL_USERS] WHERE WP_PORTAL_USERS_ID = (SELECT [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'))),'')
+ '½' + 
ISNULL((SELECT ISNULL(EMAIL_ADDRESSES,'') FROM [CDXP].[WP_PORTAL_USERS] WHERE WP_PORTAL_USERS_ID = 175),'') + '½'+
(SELECT FORMAT([CDXP].[GETDATEpk](),'dd-MMM-yyyy HH:mm'))
T");
                        string Mailresult = "";
                        Task.Factory.StartNew<string>(() =>
                        Fn.SendMail(dtls, "CDP Data Submitted", "submitted"))
                .ContinueWith(ant => Mailresult = ant.Result,
                              TaskScheduler.FromCurrentSynchronizationContext());

                        break;


                    case 10:
                        Returnvls = Fn.Data2Json(@"EXEC [CDXP].[SP_MRP_EDIT_DATA_BY_WP_DISCO_CDP_MONTHLY_DATA_ID] " + dataID[1]);
                        break;


                    case 11:

                        string cdpNXT;
                        sql = " UPDATE [CDXP].[WP_DISCO_CDP_MONTHLY_DATA] SET [CPPA_SELECTED]=1, [Status] = 2, ";
                        sql += " LAST_UPDATE_BY_WP_PORTAL_USERS_ID = CDXP.FN_WP_PORTAL_USERS_ID_BY_USER_NAME('" + Convert.ToString(Session["UserName"]) + @"'), ";
                        sql += " LAST_UPDATE_DATE = '" + serverDate + @"'";
                        sql += " WHERE WP_DISCO_CDP_MONTHLY_DATA_ID in ( " + dataID[4] + "); select '1'; ";
                        Returnvls = Fn.ExenID(sql);

                        string dtls11 = Fn.ExenID(@"SELECT ISNULL((SELECT CUSTOMER_NAME FROM [CDXP].[WP_SETUP_DISCO] WHERE CUSTOMER_ID = '" + dataID[1] + @"') ,'')+'½'+ISNULL(
                            (SELECT STRING_AGG(ISNULL(CDP_NUMBER,''), ', ') FROM [CDXP].[CPPA_CDP_HEADERS]  WHERE CDP_ID IN (" + dataID[6] + @")),'') +'½'+
                            ISNULL((
                            SELECT  UPPER(DATENAME(MONTH,CAST([MONTH_NUMBER] AS VARCHAR(2))+'-01-'+ CAST([YEAR_NUMBER] AS VARCHAR(4)))) + '-' + CAST([YEAR_NUMBER] AS VARCHAR(4)) AS DISPLAY FROM [CDXP].[ACCOUNTING_MONTH] WHERE  [ACCOUNTING_MONTH_ID]='" + dataID[2] + @"'
                            ),'') + '½' + 
                            ISNULL((SELECT ISNULL(EMAIL_ADDRESSES,'') FROM [CDXP].[WP_PORTAL_USERS] WHERE WP_PORTAL_USERS_ID = 175),'') + '½'+
                            ISNULL((SELECT ISNULL(EMAIL_ADDRESSES,'') FROM [CDXP].[WP_PORTAL_USERS] WHERE WP_PORTAL_USERS_ID = (SELECT [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'))),'')
                            + '½' + 
                            (SELECT FORMAT([CDXP].[GETDATEpk](),'dd-MMM-yyyy HH:mm'))
                            T");

                        // tr for disco dashboard
                        Returnvls = Fn.HTMLTableWithID_TR(@"[CDXP].[SP_DISCO_MONTHLY_REPORT] " + dataID[1] + @", " + dataID[2] + @"," + 3 + @", '" + dataID[6] + "'");

                        // tr for interdisco dashboard
                        Returnvls1 = Fn.HTMLTableAdmin_TR(@"[CDXP].[SP_DISCO_MONTHLY_REPORT_CPPA_VERIFICATION] " + dataID[1] + @", " + dataID[2] + ", " + 2 + ",'" + dataID[6] + "'," + "NULL" + "");
                        // for json 
                        cdpNXT = Returnvls1.Substring(Returnvls1.IndexOf("[")).Replace("</td></tr>", "");
                        String customer_id = Fn.ExenID(@"
				                            ​SELECT
                                                    cch.CUSTOMER_ID
                                                FROM
                                                    CDXP.CPPA_CDP_HEADER cch
			                                                INNER JOIN[CDXP].[WP_SETUP_DISCO] B ON B.CUSTOMER_ID = CCH.TO_CUSTOMER_ID     
			                                                INNER JOIN[CDXP].[WP_SETUP_DISCO] C ON C.CUSTOMER_ID = CCH.FROM_CUSTOMER_ID     

                                                WHERE
                                                        cch.CDP_ID = " + dataID[6] + @"
                                                    AND
                                                        cch.CUSTOMER_ID <> " + dataID[1] + @" -- Pass here Customer ID of Received Record
                                                    AND
                                                NOT EXISTS (
                                                    SELECT
                                                            'X'
                                                    FROM
                                                            CDXP.WP_DISCO_CDP_MONTHLY_DATA wdcmd
                                                    WHERE
                                                            wdcmd.CDP_ID = CCH.CDP_ID
                                                        AND
                                                            wdcmd.ACCOUNTING_MONTH_ID = " + dataID[2] + @"
                                                        AND
                                                            wdcmd.CUSTOMER_ID = cch.CUSTOMER_ID
				                                        AND     
                                                            (B.CUSTOMER_TYPE = 5 or B.CUSTOMER_TYPE = 6) 
				                                        AND     
                                                            (C.CUSTOMER_TYPE = 5 OR C.CUSTOMER_TYPE = 6) 
				                                        AND     
                                                            CCH.CDP_STATUS_CODE = 'Active'
                                                )
                                          ");

                        if (!String.IsNullOrEmpty(customer_id))
                        {

                            Fn.Exec(@"INSERT INTO CDXP.WP_DISCO_CDP_MONTHLY_DATA (
                                CUSTOMER_ID, CDP_ID,TRANSACTION_TYPE, PRIMARY_CDP_DTL_ID, BACKUP_CDP_DTL_ID, ACCOUNTING_MONTH_ID, PM_READING_DATE_TIME, BM_READING_DATE_TIME, FROM_DATE, TO_DATE,  
                         PRIMARY_IMPORT_ACTIVE_READING, PRIMARY_IMPORT_REACTIVE_READING, PRIMARY_EXPORT_ACTIVE_READING, PRIMARY_EXPORT_REACTIVE_READING, BACKUP_IMPORT_ACTIVE_READING, 
                         BACKUP_IMPORT_REACTIVE_READING, BACKUP_EXPORT_ACTIVE_READING, BACKUP_EXPORT_REACTIVE_READING, PRIMARY_IMPORT_ACTIVE_AJD, PRIMARY_IMPORT_REACTIVE_ADJ, PRIMARY_EXPORT_ACTIVE_AJD, 
                         PRIMARY_EXPORT_REACTIVE_ADJ, BACKUP_IMPORT_ACTIVE_AJD, BACKUP_IMPORT_REACTIVE_ADJ, BACKUP_EXPORT_ACTIVE_AJD, BACKUP_EXPORT_REACTIVE_ADJ, PRIMARY_IMPORT_ACTIVE_MDI, 
                         PRIMARY_IMPORT_REACTIVE_MDI, PRIMARY_EXPORT_ACTIVE_MDI, PRIMARY_EXPORT_REACTIVE_MDI, BACKUP_IMPORT_ACTIVE_MDI, BACKUP_IMPORT_REACTIVE_MDI, BACKUP_EXPORT_ACTIVE_MDI, 
                         BACKUP_EXPORT_REACTIVE_MDI, IS_PRIMARY_METER, REMARKS, P_ENG_ACTIVE_IMPORT, P_ENG_ACTIVE_EXPORT, P_MDI_ACTIVE_IMPORT, P_MDI_ACTIVE_EXPORT, B_ENG_ACTIVE_IMPORT, B_ENG_ACTIVE_EXPORT, 
                         B_MDI_ACTIVE_IMPORT, B_MDI_ACTIVE_EXPORT,PM_PREVIOUS_READING_DATE_TIME,BM_PREVIOUS_READING_DATE_TIME,PREVIOUS_READING_PRIM_IMP_ACT,PREVIOUS_READING_PRIM_IMP_REACT,      PREVIOUS_READING_PRIM_EXP_ACT,PREVIOUS_READING_PRIM_EXP_REACT,PREVIOUS_READING_BACK_IMP_ACT,PREVIOUS_READING_BACK_IMP_REACT,PREVIOUS_READING_BACK_EXP_ACT,PREVIOUS_READING_BACK_EXP_REACT, CREATION_DATE,CREATED_BY_WP_PORTAL_USERS_ID,LAST_UPDATE_BY_WP_PORTAL_USERS_ID,Status,Received_CDP_Record,Admin_Received_Record,CDP_NXT) 
                        VALUES (" + customer_id + "," + dataID[6] + @",'Monthly Reading',
                            ( SELECT ccd1.CDP_DTL_ID FROM CDXP.CPPA_CDP_DETAIL ccd1 WHERE ccd1.CDP_ID = " + dataID[6] + @" AND ccd1.IS_PRIMARY_METER = 'Y')
                        , (SELECT ccd1.CDP_DTL_ID FROM CDXP.CPPA_CDP_DETAIL ccd1 WHERE ccd1.CDP_ID = " + dataID[6] + @" AND ccd1.IS_PRIMARY_METER = 'N')
                        ," + dataID[2] + @",GetDate(),GetDate(),GetDate(),GetDate(),0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,'Y',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,GetDate(),
                        (SELECT[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + "')),(SELECT[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + "')), 4, '" + Returnvls + "', '" + Returnvls1 + "', '" + cdpNXT + "')");

                        }
                        else
                        {
                            // BELOW CODE WILL BE USED TO DECIDE WHETHER WE NEED TO SHOW HISTORY BUTTON OR NOT ( FOR INTER DISCO IT WILL SHOW WHICH RECORD IS RECEIVED)
                            Fn.Exec(@"UPDATE CDXP.WP_DISCO_CDP_MONTHLY_DATA SET Received_CDP_Record='" + Returnvls + "',Admin_Received_Record='" + Returnvls1 + "', Status=4, CDP_NXT='" + cdpNXT + "'  WHERE STATUS <> 2 AND [ACCOUNTING_MONTH_ID]=" + dataID[2] + " AND  CDP_ID=" + dataID[6] + "");
                        }
                        string Mailresult11 = "";
                        Task.Factory.StartNew<string>(() =>
                        Fn.SendMail(dtls11, "CDP Data Received", "received"))
                .ContinueWith(ant => Mailresult11 = ant.Result,
                              TaskScheduler.FromCurrentSynchronizationContext());

                        break;

                    case 12:
                                           

                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"  
SELECT 
	                        CUSTOMER_ID,  
	                        Sr#, 
	                        [Distribution Company], 
	                        [Received %], 
	                        CASE 
	                        WHEN TABLEA.CUSTOMER_ID = 1052 THEN [dbo].[Get_PESCO_Energy](ISNULL([Energy (kWh)],0)," + dataID[1] + @") 
	                        WHEN TABLEA.CUSTOMER_ID = 1058 THEN (SELECT Energy FROM [CDXP].[TESCO_Energy_details] WHERE account_month_id=" + dataID[1] + @") 
	                        ELSE  [Energy (kWh)] END AS  [Energy (kWh)], 
	                        CASE 
	                        WHEN TABLEA.CUSTOMER_ID = 1052 THEN [dbo].[Get_PESCO_MDI](ISNULL([MDI (kW)],0)," + dataID[1] + @")
	                        WHEN TABLEA.CUSTOMER_ID = 1058 THEN (SELECT MDI FROM [CDXP].[TESCO_Energy_details] WHERE account_month_id=" + dataID[1] + @")
	                        ELSE  [MDI (kW)] END AS  [MDI (kW)] 
                    FROM (
                            SELECT        CDXP.WP_SETUP_DISCO.CUSTOMER_ID, 0 AS DISABLEEDITBUTTON,  '' [Sr#], CDXP.WP_SETUP_DISCO.ABBREVIATION [Distribution Company], [CDXP].[GetDiscoWisePercentage](" + dataID[1] + @", CDXP.WP_SETUP_DISCO.CUSTOMER_ID) [Received %], ISNULL(DCTOTAL.[ENG DELIVERED],'0') [Energy (kWh)], ISNULL(DCTOTAL.MDI,'0') [MDI (kW)]
                            FROM            
			                    CDXP.WP_SETUP_DISCO 
		                    INNER JOIN CDXP.WP_GC_USER_ACCESS ON CDXP.WP_GC_USER_ACCESS.ENTITY_ID = CDXP.WP_SETUP_DISCO.CUSTOMER_ID 
		                    INNER JOIN CDXP.WP_PORTAL_USERS ON CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = CDXP.WP_PORTAL_USERS.WP_PORTAL_USERS_ID 
		                    LEFT OUTER JOIN
                       (							 
						SELECT        
							AM.ACCOUNTING_MONTH_ID, '' AS Sr#, UPPER(DATENAME(MONTH, CAST(AM.MONTH_NUMBER AS VARCHAR(2)) + '-01-' + CAST		(AM.YEAR_NUMBER AS VARCHAR(4)))) 
							+ ' - ' + CAST(AM.YEAR_NUMBER AS VARCHAR(4)) AS DISPLAY,  'Draft' AS Status, 
                            ISNULL(CAST(ABSTBL.[ENG DELIVERED] AS VARCHAR(100)), '') AS [ENG DELIVERED], ISNULL(CAST(ABSTBL.MDI AS VARCHAR(100)), '') AS MDI, ABSTBL.CUSTOMER_ID
                         FROM            
							CDXP.ACCOUNTING_MONTH AS AM 
							INNER JOIN (
										SELECT        
											SUM([ENG DELIVERED]) AS [ENG DELIVERED], 
											SUM(MDI) AS MDI, 
											CUSTOMER_ID, 
											ACCOUNTING_MONTH_ID
										FROM            
											(
												SELECT        
													MD.CDP_ID, 
													MD.ENG_ACTIVE_IMPORT-MD.ENG_ACTIVE_EXPORT AS [ENG DELIVERED],
													MD.MDI_ACTIVE_IMPORT-MD.MDI_ACTIVE_EXPORT AS MDI,
													
			                                           MD.CUSTOMER_ID, 
                                                       MD.ACCOUNTING_MONTH_ID
                                                 FROM            
													CDXP.CPPA_CDP_HEADER 
													INNER JOIN (
																SELECT        
				MD1.CDP_ID, 
				CASE 
					WHEN  Isnull(CDXP.CPPA_CDP_HEADERS.TO_customer_id, 0) = MD1.CUSTOMER_ID AND Status = 2 THEN 
						SUM(CASE 
						WHEN MD1.IS_PRIMARY_METER = 'Y' THEN
							MD1.P_ENG_ACTIVE_EXPORT 										
						 ELSE		
							MD1.B_ENG_ACTIVE_EXPORT END)
					WHEN  Isnull(CDXP.CPPA_CDP_HEADERS.FROM_CUSTOMER_ID, 0) = MD1.CUSTOMER_ID AND Status = 2 THEN 
						SUM(CASE 
						WHEN MD1.IS_PRIMARY_METER = 'Y' THEN
							MD1.P_ENG_ACTIVE_IMPORT 										
						 ELSE		
							MD1.B_ENG_ACTIVE_IMPORT END)
					ELSE
						sum(CAST(JSON_VALUE(CDP_NXT, '$[0].BBB[0].CCC[0]._ENG_ACTIVE_IMPORT') AS DECIMAL(18,0))) 
				
				END AS ENG_ACTIVE_IMPORT, 
				
				CASE WHEN Isnull(CDXP.CPPA_CDP_HEADERS.TO_customer_id, 0) = MD1.CUSTOMER_ID AND STATUS = 2 THEN 
					SUM(CASE
					   WHEN MD1.IS_PRIMARY_METER = 'Y' THEN 
					        MD1.P_ENG_ACTIVE_IMPORT 
					   ELSE	
						   MD1.B_ENG_ACTIVE_IMPORT END) 
					WHEN Isnull(CDXP.CPPA_CDP_HEADERS.FROM_CUSTOMER_ID, 0) = MD1.CUSTOMER_ID AND STATUS = 2 THEN 
					SUM(CASE
					   WHEN MD1.IS_PRIMARY_METER = 'Y' THEN 
					        MD1.P_ENG_ACTIVE_EXPORT 
					   ELSE	
						   MD1.B_ENG_ACTIVE_EXPORT END) 
				ELSE
					
					sum(cast(JSON_VALUE(CDP_NXT, '$[0].BBB[0].CCC[0]._ENG_ACTIVE_EXPORT') AS DECIMAL(18,0)))
				END AS ENG_ACTIVE_EXPORT, 

				CASE WHEN
				     Isnull(CDXP.CPPA_CDP_HEADERS.TO_customer_id, 0) = MD1.CUSTOMER_ID AND STATUS = 2 THEN 
					SUM(CASE WHEN
					        MD1.IS_PRIMARY_METER = 'Y' THEN 
					        MD1.P_MDI_ACTIVE_EXPORT 
						ELSE									
							MD1.B_MDI_ACTIVE_EXPORT END)
					WHEN
				     Isnull(CDXP.CPPA_CDP_HEADERS.FROM_CUSTOMER_ID, 0) = MD1.CUSTOMER_ID AND STATUS = 2 THEN 
					SUM(CASE WHEN
					        MD1.IS_PRIMARY_METER = 'Y' THEN 
					        MD1.P_MDI_ACTIVE_IMPORT 
						ELSE									
							MD1.B_MDI_ACTIVE_IMPORT END) 
				ELSE
					sum(cast(JSON_VALUE(CDP_NXT, '$[0].BBB[0].CCC[0]._MDI_ACTIVE_IMPORT') AS DECIMAL(18,0)))
				END AS MDI_ACTIVE_IMPORT, 
				
				CASE WHEN Isnull(CDXP.CPPA_CDP_HEADERS.TO_customer_id, 0) = MD1.CUSTOMER_ID AND STATUS = 2 THEN 
					SUM(CASE 
					        WHEN MD1.IS_PRIMARY_METER = 'Y' THEN
							     MD1.P_MDI_ACTIVE_IMPORT
						    ELSE								
								MD1.B_MDI_ACTIVE_IMPORT END) 
					WHEN Isnull(CDXP.CPPA_CDP_HEADERS.FROM_CUSTOMER_ID, 0) = MD1.CUSTOMER_ID AND STATUS = 2 THEN 
					SUM(CASE 
					        WHEN MD1.IS_PRIMARY_METER = 'Y' THEN
							     MD1.P_MDI_ACTIVE_EXPORT
						    ELSE								
								MD1.B_MDI_ACTIVE_EXPORT END) 
				ELSE
				  	sum(CAST(JSON_VALUE(CDP_NXT, '$[0].BBB[0].CCC[0]._MDI_ACTIVE_EXPORT') AS DECIMAL(18,0)))
				END AS MDI_ACTIVE_EXPORT,  
				CUSTOMER_ID, 
				ACCOUNTING_MONTH_ID
					
				FROM            
				CDXP.WP_DISCO_CDP_MONTHLY_DATA AS MD1 
				INNER JOIN CDXP.CPPA_CDP_HEADERS ON MD1.CDP_ID=CDXP.CPPA_CDP_HEADERS.CDP_ID
				WHERE        
				(Status  IN (2, 4 )) 
				GROUP 
				BY CUSTOMER_ID, ACCOUNTING_MONTH_ID, IS_PRIMARY_METER,MD1.CDP_ID, MD1.Status, FROM_CUSTOMER_ID, TO_CUSTOMER_ID
			HAVING         
				(CUSTOMER_ID IN
							(SELECT        
								WP_GC_USER_ACCESS_2.ENTITY_ID
							FROM            
								CDXP.WP_GC_USER_ACCESS AS WP_GC_USER_ACCESS_2 
								INNER JOIN CDXP.WP_PORTAL_USERS AS WP_PORTAL_USERS_2 ON															WP_PORTAL_USERS_2.WP_PORTAL_USERS_ID =																			WP_GC_USER_ACCESS_2.WP_PORTAL_USERS_ID
							WHERE        
								(WP_PORTAL_USERS_2.USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"')
							)
				)

																) AS MD ON CDXP.CPPA_CDP_HEADER.CDP_ID = MD.CDP_ID
																 AND CDXP.CPPA_CDP_HEADER.CUSTOMER_ID = MD.CUSTOMER_ID) AS innerqry
																GROUP BY CUSTOMER_ID, ACCOUNTING_MONTH_ID															   ) AS ABSTBL ON AM.ACCOUNTING_MONTH_ID = ABSTBL.ACCOUNTING_MONTH_ID
												WHERE        (AM.ACCOUNTING_MONTH_ID = " + dataID[1] + @")							   
							   ) AS DCTOTAL ON CDXP.WP_SETUP_DISCO.CUSTOMER_ID = DCTOTAL.CUSTOMER_ID
WHERE        (CDXP.WP_PORTAL_USERS.USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"') and CDXP.WP_SETUP_DISCO.CUSTOMER_ID <> 8248
) TABLEA", "tblJ1");
                        break;

                    case 13:
                        sql = " UPDATE [CDXP].[WP_DISCO_CDP_MONTHLY_DATA] SET [CPPA_SELECTED]=0, [Status] = 3, ";
                        sql += " LAST_UPDATE_BY_WP_PORTAL_USERS_ID = CDXP.FN_WP_PORTAL_USERS_ID_BY_USER_NAME('" + Convert.ToString(Session["UserName"]) + @"'), ";
                        sql += " LAST_UPDATE_DATE = '" + serverDate + @"'";
                        sql += " WHERE WP_DISCO_CDP_MONTHLY_DATA_ID in ( " + dataID[4] + "); select '1'; ";
                        Returnvls = Fn.ExenID(sql);
                        string dtls13 = Fn.ExenID(@"SELECT ISNULL((SELECT CUSTOMER_NAME FROM [CDXP].[WP_SETUP_DISCO] WHERE CUSTOMER_ID = '" + dataID[1] + @"') ,'')+'½'+ISNULL(
(SELECT STRING_AGG(ISNULL(CDP_NUMBER,''), ', ') FROM [CDXP].[CPPA_CDP_HEADERS] INNER JOIN [CDXP].[WP_DISCO_CDP_MONTHLY_DATA] ON [CDXP].[WP_DISCO_CDP_MONTHLY_DATA].CDP_ID =[CDXP].[CPPA_CDP_HEADERS].CDP_ID  WHERE [WP_DISCO_CDP_MONTHLY_DATA_ID] IN (" + dataID[4] + @")),'') +'½'+
ISNULL((
SELECT  UPPER(DATENAME(MONTH,CAST([MONTH_NUMBER] AS VARCHAR(2))+'-01-'+ CAST([YEAR_NUMBER] AS VARCHAR(4)))) + '-' + CAST([YEAR_NUMBER] AS VARCHAR(4)) AS DISPLAY FROM [CDXP].[ACCOUNTING_MONTH] WHERE  [ACCOUNTING_MONTH_ID]='" + dataID[2] + @"'
),'') + '½' + 
ISNULL((SELECT ISNULL(EMAIL_ADDRESSES,'') FROM [CDXP].[WP_PORTAL_USERS] WHERE WP_PORTAL_USERS_ID = 175),'') + '½'+
ISNULL((SELECT ISNULL(EMAIL_ADDRESSES,'') FROM [CDXP].[WP_PORTAL_USERS] WHERE WP_PORTAL_USERS_ID = (SELECT [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'))),'')
+ '½' + 
(SELECT FORMAT([CDXP].[GETDATEpk](),'dd-MMM-yyyy HH:mm'))
T");
                        string Mailresult13 = "";
                        Task.Factory.StartNew<string>(() =>
                        Fn.SendMail(dtls13, "CDP Data Returned", "returned"))
                .ContinueWith(ant => Mailresult13 = ant.Result,
                              TaskScheduler.FromCurrentSynchronizationContext());

                        break;


                    case 14:
                        Returnvls = Fn.Exec(@"DELETE [CDXP].[WP_DISCO_CDP_MONTHLY_DATA] WHERE CUSTOMER_ID=" + dataID[1] + @" AND ACCOUNTING_MONTH_ID=" + dataID[2] + @" AND CDP_ID=" + dataID[3]);
                        break;

                    case 15:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"Select  CAST(ACCOUNTING_MONTH_ID AS VARCHAR(100)) +'½'+ CAST(MONTH_NUMBER AS VARCHAR(100)) +'½'+ CAST(YEAR_NUMBER AS VARCHAR(100))+'½'+ CAST(CAST(ISNULL(IS_ACTIVE,0) AS INT) AS VARCHAR(100)) AS ID, '' [Sr#], DATENAME(month, CAST( '2019-'+CAST(MONTH_NUMBER AS VARCHAR(100))+'-1' AS DATETIME)) [Month], YEAR_NUMBER [Year], 
CASE WHEN ISNULL(IS_ACTIVE,'0') = 0 THEN 'De active' ELSE 'Active' END AS [Status] from [CDXP].[ACCOUNTING_MONTH]", "tblJ1");
                        break;

                    case 16:
                        try
                        {
                            string hdrid16 = Fn.ExenID(@"INSERT INTO [CDXP].[MDINEO_HEADER] (DOCUMENT_NO,DOC_MONTH, NEO_TOTAL, MDI_TOTAL, APPROVAL_STATUS, ORGANIZATION_ID , CREATION_DATE,   CREATED_BY)
VALUES('" + dataID[1].Split('¼')[0] + @"','" + dataID[1].Split('¼')[1] + @"','" + dataID[1].Split('¼')[2] + @"','" + dataID[1].Split('¼')[3] + @"','" + dataID[1].Split('¼')[4] + @"','" + dataID[1].Split('¼')[5] + @"',CDXP.GETDATEpk()  , [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')); select SCOPE_IDENTITY()");
                            Returnvls = hdrid16;
                            foreach (string item16 in dataID[2].Split('¡'))
                            {
                                if (item16 != "")
                                {
                                    Fn.Exec(@"INSERT INTO [CDXP].[MDINEO_LINE] ( DISCO_ID, DISCO_NAME, NEO_KW, MDI_KW,HEADER_ID_FK ) 
                                                                        VALUES('" + item16.Split('¼')[0] + "','" + item16.Split('¼')[1] + "','" + item16.Split('¼')[2] + "','" + item16.Split('¼')[3] + "'," + hdrid16 + @")");
                                }
                            }
                            try
                            {
                                Fn.Exec(@"UPDATE [CDXP].[ACCOUNTING_MONTH] SET IS_ACTIVE=0 WHERE ACCOUNTING_MONTH_ID = " + dataID[1].Split('¼')[6]);
                            }
                            catch (Exception ex)
                            {

                                Returnvls = ex.Message;
                            }

                        }
                        catch (Exception ex)
                        {
                            Returnvls = ex.Message;
                        }

                        break;
                    case 17:
                        Returnvls = Fn.Exec(@" EXEC [CDXP].[SP_ADD_ACCOUNTING_MONTH] " + dataID[1] + ", " + dataID[2] + ", " + dataID[3]);
                        break;

                    case 18:
                        dataID[1] = dataID[1] == "NULL" ? " IS NULL " : " = " + dataID[1];
                        dataID[2] = dataID[2] == "NULL" ? " IS NULL " : " = " + dataID[2];
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT      CDXP.CPPA_CDP_HEADERS.CDP_ID AS ID, '' AS Sr#, ISNULL(WP_SETUP_DISCO_1.ABBREVIATION,'') AS [From Customer], ISNULL(CDXP.WP_SETUP_DISCO.ABBREVIATION,'') AS [To Customer], 
                         CDXP.CPPA_CDP_HEADERS.CDP_NUMBER AS [CDP Number], CDXP.CPPA_CDP_HEADERS.GRID_STATION_CODE AS [Grid Station Code], CDXP.CPPA_CDP_HEADERS.GRID_STATION_NAME AS [Grid Station Name], 
                         CDXP.CPPA_CDP_HEADERS.LINE_NAME_CODE AS [Line Name Code], CDXP.CPPA_CDP_HEADERS.CONTROLLED_BY_CODE AS [Controlled By Code], 
                         CDXP.CPPA_CDP_HEADERS.DISCO_AREA_CODE AS [DISCO Aerea Code], CDXP.CPPA_CDP_HEADERS.LOCATION_CODE AS [Location Code], CDXP.CPPA_CDP_HEADERS.LOCATION_NAME AS [Location Name], 
                         CDXP.CPPA_CDP_HEADERS.ENGY_IMP_EXP_FROM_CODE AS [Energy Import From], CDXP.CPPA_CDP_HEADERS.ENGY_IMP_EXP_TO_CODE AS [Energy Export To], 
                         CDXP.CPPA_CDP_HEADERS.GRID_VOLTAGE_CODE AS [Grid Voltage Code], CDXP.CPPA_CDP_HEADERS.TF_LINE_VOLTAGE AS [TF Line Voltage], CDXP.CPPA_CDP_HEADERS.TF_TL AS [TF-TL], 
                         CDXP.CPPA_CDP_HEADERS.CDP_UNIT_CODE AS [CDP Unit Code], CDXP.CPPA_CDP_HEADERS.FEEDER_NAME_CODE AS [Feeder Name Code], 
						 CASE WHEN FORMAT(ISNULL(CDXP.CPPA_CDP_HEADERS.EFFECTIVE_FROM,''), 'dd-MMM-yyyy') = '01-Jan-1900' THEN '' ELSE FORMAT(CDXP.CPPA_CDP_HEADERS.EFFECTIVE_FROM, 'dd-MMM-yyyy') END AS [Effective From Date], 
						 CASE WHEN FORMAT(ISNULL(CDXP.CPPA_CDP_HEADERS.EFFECTIVE_TO,''), 'dd-MMM-yyyy') = '01-Jan-1900' THEN '' ELSE FORMAT(CDXP.CPPA_CDP_HEADERS.EFFECTIVE_TO, 'dd-MMM-yyyy') END AS [Effective To Date], CDXP.CPPA_CDP_HEADERS.CDP_STATUS_CODE AS Status, CASE WHEN ISNULL(Auxilary,'0')='0' THEN 'No' Else 'Yes' End as Auxilary 
FROM            CDXP.CPPA_CDP_HEADERS LEFT OUTER JOIN
                         CDXP.WP_SETUP_DISCO ON CDXP.CPPA_CDP_HEADERS.TO_CUSTOMER_ID = CDXP.WP_SETUP_DISCO.CUSTOMER_ID LEFT OUTER JOIN 
                         CDXP.WP_SETUP_DISCO AS WP_SETUP_DISCO_1 ON CDXP.CPPA_CDP_HEADERS.FROM_CUSTOMER_ID = WP_SETUP_DISCO_1.CUSTOMER_ID
WHERE        (CDXP.CPPA_CDP_HEADERS.FROM_CUSTOMER_ID  " + dataID[1] + ") AND (CDXP.CPPA_CDP_HEADERS.TO_CUSTOMER_ID  " + dataID[2] + ")", "tblJ1");
                        break;

                    case 19:
                        dataID[1] = dataID[1] == "" ? "NULL" : dataID[1];
                        dataID[2] = dataID[2] == "" ? "NULL" : dataID[2];
                        if (dataID[22] == "0")
                        {

                            var valExists = Fn.ExenID(@"Select ISNULL(count(*),0) TTL from CDXP.CPPA_CDP_HEADERS WHERE CDP_NUMBER='" + dataID[3] + @"'");
                            if (valExists == "0")
                            {
                                Returnvls = Fn.Exec(@"INSERT INTO CDXP.CPPA_CDP_HEADERS
                         (FROM_CUSTOMER_ID, TO_CUSTOMER_ID, CDP_NUMBER, GRID_STATION_CODE, GRID_STATION_NAME, LINE_NAME_CODE, CONTROLLED_BY_CODE, DISCO_AREA_CODE, LOCATION_CODE, LOCATION_NAME, 
                         ENGY_IMP_EXP_FROM_CODE, ENGY_IMP_EXP_TO_CODE, GRID_VOLTAGE_CODE, TF_LINE_VOLTAGE, TF_TL, CDP_UNIT_CODE, FEEDER_NAME_CODE, EFFECTIVE_FROM, EFFECTIVE_TO, CDP_STATUS_CODE, Auxilary)
VALUES        (" + dataID[1] + @"," + dataID[2] + @",'" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"','" + dataID[7] + @"','" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"','" + dataID[11] + @"','" + dataID[12] + @"','" + dataID[13] + @"','" + dataID[14] + @"','" + dataID[15] + @"','" + dataID[16] + @"','" + dataID[17] + @"','" + dataID[18] + @"','" + dataID[19] + @"','" + dataID[20] + @"','" + dataID[21] + @"')");

                            }
                            else
                            {
                                Returnvls = "-1";
                            }

                        }
                        else
                        {
                            Returnvls = Fn.Exec(@"UPDATE       CDXP.CPPA_CDP_HEADERS
SET                FROM_CUSTOMER_ID = " + dataID[1] + @", TO_CUSTOMER_ID = " + dataID[2] + @", CDP_NUMBER = '" + dataID[3] + @"', GRID_STATION_CODE = '" + dataID[4] + @"', GRID_STATION_NAME = '" + dataID[5] + @"', 
                         LINE_NAME_CODE = '" + dataID[6] + @"', CONTROLLED_BY_CODE = '" + dataID[7] + @"', DISCO_AREA_CODE = '" + dataID[8] + @"', LOCATION_CODE = '" + dataID[9] + @"', LOCATION_NAME = '" + dataID[10] + @"', 
                         ENGY_IMP_EXP_FROM_CODE = '" + dataID[11] + @"', ENGY_IMP_EXP_TO_CODE = '" + dataID[12] + @"', GRID_VOLTAGE_CODE = '" + dataID[13] + @"', TF_LINE_VOLTAGE = '" + dataID[14] + @"', TF_TL = '" + dataID[15] + @"', 
                         CDP_UNIT_CODE = '" + dataID[16] + @"', FEEDER_NAME_CODE = '" + dataID[17] + @"', EFFECTIVE_FROM = '" + dataID[18] + @"', EFFECTIVE_TO = '" + dataID[19] + @"', CDP_STATUS_CODE = '" + dataID[20] + @"', Auxilary = '" + dataID[21] + @"'
WHERE        (CDP_ID = " + dataID[22] + @")");
                        }
                        break;

                    case 20:
                        Returnvls = Fn.Data2Json(@"SELECT        FROM_CUSTOMER_ID, TO_CUSTOMER_ID, CDP_NUMBER, GRID_STATION_CODE, GRID_STATION_NAME, LINE_NAME_CODE, CONTROLLED_BY_CODE, DISCO_AREA_CODE, LOCATION_CODE, LOCATION_NAME, 
                         ENGY_IMP_EXP_FROM_CODE, ENGY_IMP_EXP_TO_CODE, GRID_VOLTAGE_CODE, TF_LINE_VOLTAGE, TF_TL, CDP_UNIT_CODE, FEEDER_NAME_CODE, CASE WHEN FORMAT(ISNULL(CDXP.CPPA_CDP_HEADERS.EFFECTIVE_FROM,''), 'dd-MMM-yyyy') = '01-Jan-1900' THEN '' ELSE FORMAT(CDXP.CPPA_CDP_HEADERS.EFFECTIVE_FROM, 'dd-MMM-yyyy') END AS EFFECTIVE_FROM, 
                         CASE WHEN FORMAT(ISNULL(CDXP.CPPA_CDP_HEADERS.EFFECTIVE_TO,''), 'dd-MMM-yyyy') = '01-Jan-1900' THEN '' ELSE FORMAT(CDXP.CPPA_CDP_HEADERS.EFFECTIVE_TO, 'dd-MMM-yyyy') END  AS EFFECTIVE_TO, ISNULL(CDP_STATUS_CODE,'De-active') CDP_STATUS_CODE, CAST(ISNULL(Auxilary,'0') AS INT) Auxilary, CDP_ID
FROM            CDXP.CPPA_CDP_HEADERS 
where CDP_ID =" + dataID[1]);
                        break;

                    case 21:
                        Returnvls = Fn.Data2DropdownSQL(@"Select CDP_ID , CDP_NUMBER from [CDXP].[CPPA_CDP_HEADER] WHERE CUSTOMER_ID= " + dataID[1]);
                        break;

                    case 22:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT      CDP_DTL_ID, '' AS Sr#, ISNULL(CDXP.CPPA_CDP_HEADERS.CDP_NUMBER,'') [CDP Number] , CASE WHEN IS_PRIMARY_METER = 'Y' THEN 'Primary' Else 'Backup' end [Meter Category], 


ISNULL(METER_MAKE_CODE,'') [Meter Make Code], ISNULL(METER_NO,'') [Meter No],
ISNULL(MLT_FCT_ENERGY,0) [Multiplying Factor Energy],
ISNULL(MLT_FCT_MDI,0) [Multiplying Factor MDI], 
ISNULL(MAX_RANGE,0) [Max Range], ISNULL(CORRECTION_FACTOR,0) [Correction Factor], 
ISNULL(INITIAL_IMPORT_READING,0) [Initial Import Reading], 
ISNULL(INITIAL_EXPORT_READING,0) [Initial Export Reading], 
ISNULL(METER_UNIT_CODE,'') [Meter Unit Code], 
CASE WHEN ISNULL(FORMAT(CDXP.CPPA_CDP_DETAIL.EFFECTIVE_FROM,'dd-MMM-yyyy'),'') = '01-Jan-1900' THEN '' ELSE ISNULL(FORMAT(CDXP.CPPA_CDP_DETAIL.EFFECTIVE_FROM,'dd-MMM-yyyy'),'') END [Effective From]  , 
CASE WHEN ISNULL(FORMAT(CDXP.CPPA_CDP_DETAIL.EFFECTIVE_TO,'dd-MMM-yyyy'),'') =  '01-Jan-1900' THEN '' ELSE  ISNULL(FORMAT(CDXP.CPPA_CDP_DETAIL.EFFECTIVE_TO,'dd-MMM-yyyy'),'') END [Effective To] ,
ISNULL(CDXP.CPPA_CDP_DETAIL.METER_STATUS_CODE,'') Status,
ISNULL(CDXP.CPPA_CDP_DETAIL.REMARKS,'') Remarks
FROM            CDXP.CPPA_CDP_DETAIL INNER JOIN CDXP.CPPA_CDP_HEADERS ON CDXP.CPPA_CDP_HEADERS.CDP_ID = CDXP.CPPA_CDP_DETAIL.CDP_ID
WHERE (CDXP.CPPA_CDP_DETAIL.CDP_ID = " + dataID[1] + ")", "tblJ1");
                        break;


                    case 23:
                        Returnvls = Fn.Data2Json(@"SELECT       CDP_ID,  IS_PRIMARY_METER, 
isnull(METER_MAKE_CODE,'') [Meter Make Code], ISNULL(METER_NO,'') [Meter No],
ISNULL(MLT_FCT_ENERGY,0) [Multiplying Factor Energy],
isnull(MLT_FCT_MDI,0) [Multiplying Factor MDI], 
isnull(MAX_RANGE,0) [Max Range], 

ISNULL(INITIAL_IMPORT_READING,0) [Initial Active Import Reading], 
ISNULL(INITIAL_EXPORT_READING,0) [Initial Active Export Reading], 

ISNULL(INITIAL_IMPORT_READING_REACTIVE,0) [Initial Reactive Import Reading],
ISNULL(INITIAL_EXPORT_READING_REACTIVE,0) [Initial Reactive Export Reading], 

ISNULL(CORRECTION_FACTOR,0) [Correction Factor], 

                         ISNULL(METER_UNIT_CODE,'') [Meter Unit Code], 
						 CASE WHEN ISNULL(FORMAT(CDXP.CPPA_CDP_DETAIL.EFFECTIVE_FROM,'dd-MMM-yyyy'),'') = '01-Jan-1900' THEN '' ELSE ISNULL(FORMAT(CDXP.CPPA_CDP_DETAIL.EFFECTIVE_FROM,'dd-MMM-yyyy'),'') END [Effective From]  , 
						 CASE WHEN ISNULL(FORMAT(CDXP.CPPA_CDP_DETAIL.EFFECTIVE_TO,'dd-MMM-yyyy'),'') =  '01-Jan-1900' THEN '' ELSE  ISNULL(FORMAT(CDXP.CPPA_CDP_DETAIL.EFFECTIVE_TO,'dd-MMM-yyyy'),'') END [Effective To] ,
						 ISNULL(CDXP.CPPA_CDP_DETAIL.METER_STATUS_CODE,'') METER_STATUS_CODE,
ISNULL(CDXP.CPPA_CDP_DETAIL.REMARKS,'') Remarks,
						 CDP_DTL_ID
FROM            CDXP.CPPA_CDP_DETAIL WHERE CDXP.CPPA_CDP_DETAIL.CDP_DTL_ID =" + dataID[1]);
                        break;

                    case 24:
                        if (dataID[18] == "0")
                        {
                            int n;
                            bool isNumeric = int.TryParse(dataID[4], out n);

                            var valExists = Fn.ExenID(@"Select ISNULL(count(*),0) TTL from CDXP.CPPA_CDP_DETAIL WHERE METER_NO='" + dataID[4] + @"'");
                            if (valExists == "0" || !isNumeric)
                            {
                                Returnvls = Fn.Exec(@"INSERT INTO CDXP.CPPA_CDP_DETAIL
                         (CDP_ID, IS_PRIMARY_METER, METER_MAKE_CODE, METER_NO, MLT_FCT_ENERGY, MLT_FCT_MDI, MAX_RANGE, INITIAL_IMPORT_READING, INITIAL_EXPORT_READING,INITIAL_IMPORT_READING_REACTIVE,INITIAL_EXPORT_READING_REACTIVE, CORRECTION_FACTOR, METER_UNIT_CODE, 
                         EFFECTIVE_FROM, EFFECTIVE_TO, METER_STATUS_CODE , REMARKS)
VALUES        ('" + dataID[1] + @"','" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] +
                                                    @"','" + dataID[6] + @"','" + dataID[7] + @"','" + dataID[8] +
                                                    @"','" + dataID[9] + @"','" + dataID[10] + @"','" + dataID[11] +
                                                    @"','" + dataID[12] + @"','" + dataID[13] + @"','" + dataID[14] +
                                                    @"','" + dataID[15] + @"','" + dataID[16] + @"','" + dataID[17] + @"')");
                            }
                            else
                            {
                                Returnvls = "-1";
                            }
                        }
                        else
                        {
                            Returnvls = Fn.Exec(@"UPDATE       CDXP.CPPA_CDP_DETAIL
SET CDP_ID = '" + dataID[1] + @"', IS_PRIMARY_METER = '" + dataID[2] + @"', METER_MAKE_CODE = '" + dataID[3] + @"', METER_NO = '" + dataID[4] + @"', MLT_FCT_ENERGY = '" + dataID[5] + @"', 
                         MLT_FCT_MDI = '" + dataID[6] + @"', MAX_RANGE = '" + dataID[7] + @"',
INITIAL_IMPORT_READING = '" + dataID[8] + @"', INITIAL_EXPORT_READING = '" + dataID[9] + @"' 
 , INITIAL_IMPORT_READING_REACTIVE = '" + dataID[10] + @"', INITIAL_EXPORT_READING_REACTIVE = '" + dataID[11] + @"'                        
 , CORRECTION_FACTOR = '" + dataID[12] + @"', METER_UNIT_CODE = '" + dataID[13] + @"', EFFECTIVE_FROM = '" + dataID[14] + @"', EFFECTIVE_TO = '" + dataID[15] + @"', METER_STATUS_CODE = '" + dataID[16] + @"', REMARKS = '" + dataID[17] + @"'
WHERE        (CDP_DTL_ID = " + dataID[18] + @")"
);
                        }
                        break;

                    case 25:
                        Returnvls = Fn.Exec(@"UPDATE [CPPA_CA].[ApiUsers] SET Email='" + dataID[1] + "', EmailSubscription=1 WHERE UserID=[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')");

                        //Returnvls = Fn.Exec(@"UPDATE [CPPA_CA].[ApiUsers] SET Email='" + dataID[1] + "' WHERE UserID= [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')");

                        var getEmail = Fn.ExenID(@"select Email from [CPPA_CA].[ApiUsers] where UserID=[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')");

                        if (getEmail == "" || getEmail == null)
                        {
                            Returnvls = Fn.Exec(@"UPDATE [CPPA_CA].[ApiUsers] SET EmailSubscription=0 WHERE UserID=[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')");
                        }

                        break;

                    case 26:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"
                        select
                            WP_DISCO_CDP_MONTHLY_DATA_ID,
                            WP_DISCO_CDP_MONTHLY_DATA.CDP_ID,
                            CDP_NUMBER AS [CDP #],
                            GRID_STATION_NAME AS [NAME OF G/S], 
                            GRID_VOLTAGE_CODE AS [Voltage], 
                            TF_LINE_VOLTAGE AS [TF/TL],
                            DISCO_AREA_CODE AS [Area], 
                            ENGY_IMP_EXP_FROM_CODE AS 	[Energy Export From],
                            ENGY_IMP_EXP_TO_CODE AS 	[Energy Export To],
                            ISNULL((SELECT TOP 1 CAST(ISNULL(PMNO.METER_NO,'') AS VARCHAR(100)) FROM [CDXP].[CPPA_CDP_DETAIL] AS PMNO WHERE PMNO.IS_PRIMARY_METER='Y' AND PMNO.CDP_ID = CDXP.CPPA_CDP_HEADERS.CDP_ID),'') AS [Primary Meter No],
                            ISNULL((SELECT TOP 1 CAST(ISNULL(BMNO.METER_NO,'') AS VARCHAR(100)) FROM [CDXP].[CPPA_CDP_DETAIL] AS BMNO WHERE BMNO.IS_PRIMARY_METER='N' AND BMNO.CDP_ID = CDXP.CPPA_CDP_HEADERS.CDP_ID),'')  AS [Backup Meter No],
                            P_ENG_ACTIVE_IMPORT as [Energy Import (kWh)], 
                            P_ENG_ACTIVE_EXPORT as [Energy Export (kWh)], 
                            P_MDI_ACTIVE_IMPORT as [MDI Import (kW)], 
                            P_MDI_ACTIVE_EXPORT as [MDI Export (kW)],
                            case 
								when status = 0 then 'Draft'
								when status = 1 then 'Submitted'
								when status = 2 then 'Received'
								when status = 3 then 'Returned'
								when status = 4 then 'Already Approved ' + CAST(
																				(	SELECT 
																						CDXP.WP_SETUP_DISCO.ABBREVIATION 
																					FROM 
																						CDXP.WP_SETUP_DISCO 
																					WHERE 
																						CUSTOMER_ID = ( 
																										SELECT 
																											CUSTOMER_ID 
																										FROM 
																											CDXP.WP_DISCO_CDP_MONTHLY_DATA X 
																										WHERE 
																											X.ACCOUNTING_MONTH_ID = " + dataID[2] + @"
                                                                                                        AND 
																											X.CDP_ID = CDXP.CPPA_CDP_HEADERS.CDP_ID 
																										AND  
																											X.CUSTOMER_ID <> " + dataID[3] + @" 
																										)
																				 ) AS VARCHAR) + ' Data'
							when isnull(status,0) = 0 then 'Draft'
							else 'INVALID'
						    end AS [Status]
                        FROM 
                            CDXP.WP_DISCO_CDP_MONTHLY_DATA
                            INNER join CDXP.CPPA_CDP_HEADERS on CDXP.CPPA_CDP_HEADERS.CDP_ID=CDXP.WP_DISCO_CDP_MONTHLY_DATA.CDP_ID
                            INNER join CDXP.CPPA_CDP_DETAIL on CDXP.CPPA_CDP_DETAIL.CDP_ID=CDXP.CPPA_CDP_HEADERS.CDP_ID
                        WHERE
                          CUSTOMER_ID=" + dataID[3] + " and WP_DISCO_CDP_MONTHLY_DATA.CDP_ID=" + dataID[1] + " and ACCOUNTING_MONTH_ID=" + dataID[2] + " and Status <> 2 and CPPA_CDP_DETAIL.IS_PRIMARY_METER='Y'", "tblDClaim");
                        break;



                    case 27:

                        int dataCount = Convert.ToInt32(Fn.ExenID(@"SELECT COUNT(id) from CDXP.TESCO_Energy_details WHERE account_month_id=" + dataID[1] + " "));

                        if (dataCount > 0)
                        {
                            Returnvls = Fn.Exec(@"UPDATE [CDXP].[TESCO_Energy_details] SET Energy=" + dataID[2] + ", MDI=" + dataID[3] + " WHERE account_month_id=" + dataID[1] + "");
                        }
                        else
                        {
                            Returnvls = Fn.Exec(@"INSERT INTO [CDXP].[TESCO_Energy_details] (account_month_id,Energy,MDI) VALUES (" + Convert.ToInt32(dataID[1]) + "," + dataID[2] + "," + dataID[3] + ")");
                        }


                        break;


                    case 28:

                        var EnergyData = Fn.ExenID(@"SELECT Energy from CDXP.TESCO_Energy_details WHERE account_month_id=" + dataID[1] + " ");
                        var MDIData = Fn.ExenID(@"SELECT MDI from CDXP.TESCO_Energy_details WHERE account_month_id=" + dataID[1] + " ");

                        var PESCOEnergyVal = Fn.ExenID(@"SELECT PESCO_Energy from [dbo].[Get_PESCO_Energy_MDI](" + dataID[1] + ")");
                        var PESCOMDIVal = Fn.ExenID(@"SELECT PESCO_MDI from [dbo].[Get_PESCO_Energy_MDI](" + dataID[1] + ")");


                        Returnvls = EnergyData + '½' + MDIData + '½' + PESCOEnergyVal + '½' + PESCOMDIVal;

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
        public string MRPAjaxCall()
        {
            Fn.sessionCheck();
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
                String PARENT_ID = "0";
                switch (Convert.ToInt32(dataID[0]))
                {
                    case 0:

                        if (dataID[1] == "0")
                        {
                            try
                            {
                                PARENT_ID = Fn.ExenID(@"INSERT INTO CDXP.WP_DISCO_CDP_MONTHLY_DATA (
CUSTOMER_ID, CDP_ID,TRANSACTION_TYPE, PRIMARY_CDP_DTL_ID, BACKUP_CDP_DTL_ID, ACCOUNTING_MONTH_ID, PM_READING_DATE_TIME, BM_READING_DATE_TIME, FROM_DATE, TO_DATE,  
                         PRIMARY_IMPORT_ACTIVE_READING, PRIMARY_IMPORT_REACTIVE_READING, PRIMARY_EXPORT_ACTIVE_READING, PRIMARY_EXPORT_REACTIVE_READING, BACKUP_IMPORT_ACTIVE_READING, 
                         BACKUP_IMPORT_REACTIVE_READING, BACKUP_EXPORT_ACTIVE_READING, BACKUP_EXPORT_REACTIVE_READING, PRIMARY_IMPORT_ACTIVE_AJD, PRIMARY_IMPORT_REACTIVE_ADJ, PRIMARY_EXPORT_ACTIVE_AJD, 
                         PRIMARY_EXPORT_REACTIVE_ADJ, BACKUP_IMPORT_ACTIVE_AJD, BACKUP_IMPORT_REACTIVE_ADJ, BACKUP_EXPORT_ACTIVE_AJD, BACKUP_EXPORT_REACTIVE_ADJ, PRIMARY_IMPORT_ACTIVE_MDI, 
                         PRIMARY_IMPORT_REACTIVE_MDI, PRIMARY_EXPORT_ACTIVE_MDI, PRIMARY_EXPORT_REACTIVE_MDI, BACKUP_IMPORT_ACTIVE_MDI, BACKUP_IMPORT_REACTIVE_MDI, BACKUP_EXPORT_ACTIVE_MDI, 
                         BACKUP_EXPORT_REACTIVE_MDI, IS_PRIMARY_METER, REMARKS, P_ENG_ACTIVE_IMPORT, P_ENG_ACTIVE_EXPORT, P_MDI_ACTIVE_IMPORT, P_MDI_ACTIVE_EXPORT, B_ENG_ACTIVE_IMPORT, B_ENG_ACTIVE_EXPORT, 
                         B_MDI_ACTIVE_IMPORT, B_MDI_ACTIVE_EXPORT,PM_PREVIOUS_READING_DATE_TIME,BM_PREVIOUS_READING_DATE_TIME,PREVIOUS_READING_PRIM_IMP_ACT,PREVIOUS_READING_PRIM_IMP_REACT,PREVIOUS_READING_PRIM_EXP_ACT,PREVIOUS_READING_PRIM_EXP_REACT,PREVIOUS_READING_BACK_IMP_ACT,PREVIOUS_READING_BACK_IMP_REACT,PREVIOUS_READING_BACK_EXP_ACT,PREVIOUS_READING_BACK_EXP_REACT
,PM_ROUND_COMPLETED_IA
,BM_ROUND_COMPLETED_IA
,PM_ROUND_COMPLETED_IR
,BM_ROUND_COMPLETED_IR
,PM_ROUND_COMPLETED_EA
,BM_ROUND_COMPLETED_EA
,PM_ROUND_COMPLETED_ER
,BM_ROUND_COMPLETED_ER , CREATION_DATE,CREATED_BY_WP_PORTAL_USERS_ID
) VALUES (
'" + dataID[2] + @"','" + dataID[3] + @"','" + dataID[4] + @"','" + dataID[5] + @"','" + dataID[6] + @"','" + dataID[7] + @"','" + dataID[8] + @"','" + dataID[9] + @"','" + dataID[10] + @"'
,'" + dataID[11] + @"','" + dataID[12] + @"','" + dataID[13] + @"','" + dataID[14] + @"','" + dataID[15] + @"','" + dataID[16] + @"','" + dataID[17] + @"','" + dataID[18] + @"','" + dataID[19] + @"','" + dataID[20] + @"'
,'" + dataID[21] + @"','" + dataID[22] + @"','" + dataID[23] + @"','" + dataID[24] + @"','" + dataID[25] + @"','" + dataID[26] + @"','" + dataID[27] + @"','" + dataID[28] + @"','" + dataID[29] + @"','" + dataID[30] + @"'
,'" + dataID[31] + @"','" + dataID[32] + @"','" + dataID[33] + @"','" + dataID[34] + @"','" + dataID[35] + @"','" + dataID[36] + @"','" + dataID[37] + @"','" + dataID[38] + @"','" + dataID[39] + @"','" + dataID[40] + @"'
,'" + dataID[41] + @"','" + dataID[42] + @"','" + dataID[43] + @"','" + dataID[44] + @"','" + dataID[45] + @"'
,'" + dataID[46] + @"'
,'" + dataID[47] + @"'
,'" + dataID[48] + @"'
,'" + dataID[49] + @"'
,'" + dataID[50] + @"'
,'" + dataID[51] + @"'
,'" + dataID[52] + @"'
,'" + dataID[53] + @"'
,'" + dataID[54] + @"'
,'" + dataID[55] + @"'
,'" + dataID[56] + @"'
,'" + dataID[57] + @"'
,'" + dataID[58] + @"'
,'" + dataID[59] + @"'
,'" + dataID[60] + @"'
,'" + dataID[61] + @"'
,'" + dataID[62] + @"'
,'" + dataID[63] + @"','" + serverDate + @"', [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')
); select SCOPE_IDENTITY()");




                                Returnvls = "1";
                            }
                            catch (Exception ex)
                            {

                                Fn.Exec("INSERT INTO [CDXP].[Testing] (CheckVAL) VALUES ('" + ex.Message + "')");
                            }

                        }
                        else
                        {
                            PARENT_ID = dataID[1];
                            Returnvls = Fn.Exec(@"UPDATE       CDXP.WP_DISCO_CDP_MONTHLY_DATA
SET                CUSTOMER_ID = '" + dataID[2] + @"', CDP_ID = '" + dataID[3] + @"', TRANSACTION_TYPE = '" + dataID[4] + @"', PRIMARY_CDP_DTL_ID = '" + dataID[5] + @"', BACKUP_CDP_DTL_ID = '" + dataID[6] + @"', ACCOUNTING_MONTH_ID = '" + dataID[7] + @"', 
                         PM_READING_DATE_TIME = '" + dataID[8] + @"', BM_READING_DATE_TIME = '" + dataID[9] + @"', FROM_DATE = '" + dataID[10] + @"', TO_DATE = '" + dataID[11] + @"', 
                         PRIMARY_IMPORT_ACTIVE_READING = '" + dataID[12] + @"', PRIMARY_IMPORT_REACTIVE_READING = '" + dataID[13] + @"', PRIMARY_EXPORT_ACTIVE_READING = '" + dataID[14] + @"', 
                         PRIMARY_EXPORT_REACTIVE_READING = '" + dataID[15] + @"', BACKUP_IMPORT_ACTIVE_READING = '" + dataID[16] + @"', BACKUP_IMPORT_REACTIVE_READING = '" + dataID[17] + @"', 
                         BACKUP_EXPORT_ACTIVE_READING = '" + dataID[18] + @"', BACKUP_EXPORT_REACTIVE_READING = '" + dataID[19] + @"', PRIMARY_IMPORT_ACTIVE_AJD = '" + dataID[20] + @"', 
                         PRIMARY_IMPORT_REACTIVE_ADJ = '" + dataID[21] + @"', PRIMARY_EXPORT_ACTIVE_AJD = '" + dataID[22] + @"', PRIMARY_EXPORT_REACTIVE_ADJ = '" + dataID[23] + @"', 
                         BACKUP_IMPORT_ACTIVE_AJD = '" + dataID[24] + @"', BACKUP_IMPORT_REACTIVE_ADJ = '" + dataID[25] + @"', BACKUP_EXPORT_ACTIVE_AJD = '" + dataID[26] + @"', BACKUP_EXPORT_REACTIVE_ADJ = '" + dataID[27] + @"',
                          PRIMARY_IMPORT_ACTIVE_MDI = '" + dataID[28] + @"', PRIMARY_IMPORT_REACTIVE_MDI = '" + dataID[29] + @"', PRIMARY_EXPORT_ACTIVE_MDI = '" + dataID[30] + @"', 
                         PRIMARY_EXPORT_REACTIVE_MDI = '" + dataID[31] + @"', BACKUP_IMPORT_ACTIVE_MDI = '" + dataID[32] + @"', BACKUP_IMPORT_REACTIVE_MDI = '" + dataID[33] + @"', 
                         BACKUP_EXPORT_ACTIVE_MDI = '" + dataID[34] + @"', BACKUP_EXPORT_REACTIVE_MDI = '" + dataID[35] + @"', IS_PRIMARY_METER = '" + dataID[36] + @"', REMARKS = '" + dataID[37] + @"', 
                         P_ENG_ACTIVE_IMPORT = '" + dataID[38] + @"', P_ENG_ACTIVE_EXPORT = '" + dataID[39] + @"', P_MDI_ACTIVE_IMPORT = '" + dataID[40] + @"', P_MDI_ACTIVE_EXPORT = '" + dataID[41] + @"', 
                         B_ENG_ACTIVE_IMPORT = '" + dataID[42] + @"', B_ENG_ACTIVE_EXPORT = '" + dataID[43] + @"', B_MDI_ACTIVE_IMPORT = '" + dataID[44] + @"', B_MDI_ACTIVE_EXPORT = '" + dataID[45] + @"', 
                         PM_PREVIOUS_READING_DATE_TIME = '" + dataID[46] + @"', BM_PREVIOUS_READING_DATE_TIME = '" + dataID[47] + @"', PREVIOUS_READING_PRIM_IMP_ACT = '" + dataID[48] + @"', 
                         PREVIOUS_READING_PRIM_IMP_REACT = '" + dataID[49] + @"', PREVIOUS_READING_PRIM_EXP_ACT = '" + dataID[50] + @"', PREVIOUS_READING_PRIM_EXP_REACT = '" + dataID[51] + @"', 
                         PREVIOUS_READING_BACK_IMP_ACT = '" + dataID[52] + @"', PREVIOUS_READING_BACK_IMP_REACT = '" + dataID[53] + @"', PREVIOUS_READING_BACK_EXP_ACT = '" + dataID[54] + @"', 
                         PREVIOUS_READING_BACK_EXP_REACT = '" + dataID[55] + @"',

PM_ROUND_COMPLETED_IA = '" + dataID[56] + @"',
BM_ROUND_COMPLETED_IA = '" + dataID[57] + @"',
PM_ROUND_COMPLETED_IR = '" + dataID[58] + @"',
BM_ROUND_COMPLETED_IR = '" + dataID[59] + @"',
PM_ROUND_COMPLETED_EA = '" + dataID[60] + @"',
BM_ROUND_COMPLETED_EA = '" + dataID[61] + @"',
PM_ROUND_COMPLETED_ER = '" + dataID[62] + @"',
BM_ROUND_COMPLETED_ER = '" + dataID[63] + @"'









WHERE        (WP_DISCO_CDP_MONTHLY_DATA_ID = " + dataID[1] + ") ");
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


                                //var userName = "carbon81@cppapk.onmicrosoft.com";
                                //string passWd = "Cppa@ecm123";
                                var UserNameAndPaswordForID = Fn.UserNameAndPaswordForID("1");
                                var userName = UserNameAndPaswordForID.Split('½')[0];
                                string passWd = UserNameAndPaswordForID.Split('½')[1];
                                SecureString securePassWd = new SecureString();
                                foreach (var c in passWd.ToCharArray())
                                {
                                    securePassWd.AppendChar(c);
                                }
                                //string destinationSiteUrl = Convert.ToString(Session["SPHostUrl"]);
                                string destinationSiteUrl = Convert.ToString("https://cppapk.sharepoint.com/departments/technical");
                                ClientContext clientContext = new ClientContext(destinationSiteUrl);
                                clientContext.Credentials = new SharePointOnlineCredentials(userName, securePassWd);

                                //mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm
                                var list = clientContext.Web.Lists.GetByTitle("Documents");
                                clientContext.Load(list.RootFolder);
                                clientContext.ExecuteQuery();

                                string folder = Convert.ToString(Session["UserName"]).Replace("_dc@cppapk.onmicrosoft.com", "").ToUpper();

                                var fileUrl = String.Format("{0}/{1}", "/departments/technical/Shared%20Documents/Metering%20Data/" + folder, fileName); //list.RootFolder.ServerRelativeUrl, fileName);

                                Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, fileUrl, filestream, true);

                                string ONLINE_URL = "https://cppapk.sharepoint.com/departments/technical/Shared%20Documents/Metering%20Data/" + folder + "/" + fileName;

                                //






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
                                var payload = "{'DocId': '" + rid + @"','LibraryName': 'Documents' ,'SiteUrl': '" + destinationSiteUrl + @"' ,'SiteName': 'Technical Department' ,'currentUserItemId': '0' ,'WebId': '" + webid + @"','authorUser': '" + Convert.ToString(Session["UserName"]) + @"','uploadFrom': 'MRP'}";

                                HttpContent cc = new StringContent(payload, Encoding.UTF8, "application/json");
                                var t = Task.Run(() => PostURI(u, cc));
                                t.Wait();
                                //Fn.Exec(@"INSERT INTO [CDXP].[tblFiles] (strFileName,strSPUrl,strDescription) VALUES ('" + file.FileName + @"','http://172.16.10.54/ecm/folder/download/" + result["_dlc_DocId"].ToString() + @"','" + d[0] + @"')");

                                try
                                {
                                    string abc = Fn.Exec(@"INSERT INTO [CDXP].[WP_FILES] (FILE_URL,PARENT_ID,UPLOADED_FOR,FILE_TITLE,FILE_DESCRIPTION,SHAREPOINT_FILE_ID,SHAREPOINT_FILE_GUID,FILES_NAME,FILE_EXT,FILE_TYPE,ONLINE_URL) VALUES ('http://172.16.10.54/ecm/folder/download/" + Convert.ToString(docid) + @"','" + PARENT_ID + @"','MRP','" + itemxx.Split('½')[3] + @"','" + itemxx.Split('½')[4] + @"','" + rid + @"','" + webid + @"','" + itemxx.Split('½')[2] + @"','" + itemxx.Split('½')[0] + @"','" + itemxx.Split('½')[1] + @"','" + ONLINE_URL + "')");
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

                return Convert.ToString(ex.Message);
            }


            return Returnvls;
        }

        //public string fnFolder() {
        //    string Folder = "";
        //    switch (Convert.ToString(Session["UserName"]).ToUpper())
        //    {


        //        case "LESCO_DC@CPPAPK.ONMICROSOFT.COM":
        //            Folder = "/departments/technical/Shared%20Documents/Metering%20Data/LESCO";
        //            break;
        //        case "FESCO_DC@CPPAPK.ONMICROSOFT.COM":
        //            Folder = "/departments/technical/Shared%20Documents/Metering%20Data/FESCO";
        //            break;
        //        case "MEPCO_DC@CPPAPK.ONMICROSOFT.COM":
        //            Folder = "/departments/technical/Shared%20Documents/Metering%20Data/MEPCO";
        //            break;
        //        case "QESCO_DC@CPPAPK.ONMICROSOFT.COM":
        //            Folder = "/departments/technical/Shared%20Documents/Metering%20Data/QESCO";
        //            break;
        //        case "GEPCO_DC@CPPAPK.ONMICROSOFT.COM":
        //            Folder = "/departments/technical/Shared%20Documents/Metering%20Data/GEPCO";
        //            break;
        //        case "IESCO_DC@CPPAPK.ONMICROSOFT.COM":
        //            Folder = "/departments/technical/Shared%20Documents/Metering%20Data/IESCO";
        //            break;
        //        case "PESCO_DC@CPPAPK.ONMICROSOFT.COM":
        //            Folder = "/departments/technical/Shared%20Documents/Metering%20Data/PESCO";
        //            break;
        //        case "HESCO_DC@CPPAPK.ONMICROSOFT.COM":
        //            Folder = "/departments/technical/Shared%20Documents/Metering%20Data/HESCO";
        //            break;
        //        case "KESCO_DC@CPPAPK.ONMICROSOFT.COM":
        //            Folder = "/departments/technical/Shared%20Documents/Metering%20Data/KESCO";
        //            break;
        //        case "TAESCO_DC@CPPAPK.ONMICROSOFT.COM":
        //            Folder = "/departments/technical/Shared%20Documents/Metering%20Data/TAESCO";
        //            break;
        //        case "SEPCO_DC@CPPAPK.ONMICROSOFT.COM":
        //            Folder = "/departments/technical/Shared%20Documents/Metering%20Data/SEPCO";
        //            break;
        //        case "NTDC_DC@CPPAPK.ONMICROSOFT.COM":
        //            Folder = "/departments/technical/Shared%20Documents/Metering%20Data/NTDC";
        //            break;
        //        case "DISCO_DC@CPPAPK.ONMICROSOFT.COM":
        //            Folder = "/departments/technical/Shared%20Documents/Metering%20Data/DISCO";
        //            break;
        //        default:
        //            Folder = "/departments/it/Shared%20Documents/ERP/Correspondance/Letters";
        //            break;
        //    }
        //    return Folder;
        //}
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
        public ActionResult MeterReadingProformaDisplayOnly()
        {
            Fn.sessionCheck();

            ViewBag.Title = "Meter Reading Proforma";


            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                var userName = Convert.ToString(Session["UserName"]);
                ViewBag.ddlCDPNumber = Fn.Data2Dropdown(db.SP_ddlCDPNumber(userName).ToList<SP_ddlCDPNumber_Result>());
                //ViewBag.ddlMonths = Fn.Data2Dropdown(db.SP_ddlMonths().ToList<SP_ddlMonths_Result>());
            }

            //DateTime utc = DateTime.UtcNow;
            //TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            //DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);

            //ViewBag.dtToday = pkdatetime.ToString("dd-MMM-yyyy HH:mm");
            return View();
        }

        [HttpPost]
        public ActionResult DISCO_Monthly_Data()
        {
            Fn.sessionCheck();
            ViewBag.Title = "DISCO Monthly Data";

            var frmdata = HttpContext.Request.Form["vls"];
            string id = frmdata;
            if (id != null)
            {
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    ViewBag.ddlMonths = Fn.Data2Dropdown(db.SP_ddlMonths().ToList<SP_ddlMonths_Result>());
                    var UserName = Convert.ToString(Session["UserName"]);
                    ViewBag.ddlDISCOs = Fn.Data2Dropdown(db.SP_ddlDISCOs(UserName).ToList<SP_ddlDISCOs_Result>());
                }
            }
            else
            {
                using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
                {
                    ViewBag.ddlMonths = Fn.Data2Dropdown(db.SP_ddlMonths().ToList<SP_ddlMonths_Result>());
                    var UserName = Convert.ToString(Session["UserName"]);
                    ViewBag.ddlDISCOs = Fn.Data2Dropdown(db.SP_ddlDISCOs(UserName).ToList<SP_ddlDISCOs_Result>());
                }
            }


            return View();
        }

        [HttpPost]
        public ActionResult DISCO_Monthly_DataCPPAG()
        {
            Fn.sessionCheck();
            ViewBag.Title = "DISCO Monthly Data";

            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.ddlMonths = Fn.Data2Dropdown(db.SP_ddlMonths().ToList<SP_ddlMonths_Result>());
                var UserName = Convert.ToString(Session["UserName"]);
                ViewBag.ddlDISCOs = Fn.Data2Dropdown(db.SP_ddlDISCOs(UserName).ToList<SP_ddlDISCOs_Result>());
            }
            return View();
        }

        [HttpPost]
        public ActionResult Energy_Delivered()
        {
            Fn.sessionCheck();
            ViewBag.Title = "Energy Delivered";
            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.ddlMonths = Fn.Data2DropdownSQL(@" EXEC [CDXP].[SP_ddlMonths_TotalEnergy_Delivered] "); // Fn.Data2Dropdown(db.SP_ddlMonths_TotalEnergy_Delivered().ToList<SP_ddlMonths_TotalEnergy_Delivered_Result>());
            }
            return View();
        }

        [HttpPost]
        public ActionResult CPPAG_CDP_PROCESS()
        {
            Fn.sessionCheck();
            ViewBag.Title = "DISCO Monthly Data";

            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.ddlMonths = Fn.Data2Dropdown(db.SP_ddlMonths().ToList<SP_ddlMonths_Result>());
                var UserName = Convert.ToString(Session["UserName"]);
                ViewBag.ddlDISCOs = Fn.Data2Dropdown(db.SP_ddlDISCOs(UserName).ToList<SP_ddlDISCOs_Result>());
            }
            return View();
        }
        [HttpPost]
        public ActionResult NTDC_CDP_PROCESS()
        {
            Fn.sessionCheck();
            ViewBag.Title = "DISCO Monthly Data";

            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.ddlMonths = Fn.Data2Dropdown(db.SP_ddlMonths().ToList<SP_ddlMonths_Result>());
                var UserName = Convert.ToString(Session["UserName"]);
                ViewBag.ddlDISCOs = Fn.Data2DropdownSQL(@"EXEC [CDXP].[SP_ddlDISCOs_All] 'ntdc_dc@cppapk.onmicrosoft.com'");
            }
            return View();
        }

        [HttpPost]
        public ActionResult TESCO_Energy_MDI()
        {
            sessionCheck();
            ViewBag.Title = "CDP Registration";

            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                ViewBag.ddlMonths = Fn.Data2Dropdown(db.SP_ddlMonths().ToList<SP_ddlMonths_Result>());
                var UserName = Convert.ToString(Session["UserName"]);
                ViewBag.ddlDISCOs = Fn.Data2Dropdown(db.SP_ddlDISCOs(UserName).ToList<SP_ddlDISCOs_Result>());

            }
            return View();
        }


        [HttpPost]
        public ActionResult MeterRegistration()
        {
            Fn.sessionCheck();
            ViewBag.Title = "CDP Registration";

            using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            {
                var UserName = Convert.ToString(Session["UserName"]);
                ViewBag.ddlDISCOs = Fn.Data2Dropdown(db.SP_ddlDISCOs(UserName).ToList<SP_ddlDISCOs_Result>());

            }
            return View();
        }

        [HttpPost]
        public ActionResult CDPRegistration()
        {
            ViewBag.Title = "CDP Registration";

            //using (WP_CPPAG_PrototypeEntities db = new WP_CPPAG_PrototypeEntities())
            //{
            var UserName = Convert.ToString(Session["UserName"]);
            ViewBag.ddlDISCOs = Fn.Data2DropdownSQL(@"SELECT 'NULL' , 'IPP' ABBREVIATION
union
SELECT CAST(CUSTOMER_ID AS VARCHAR(100)), ABBREVIATION FROM [CDXP].[WP_SETUP_DISCO] 
INNER JOIN [CDXP].[WP_GC_USER_ACCESS] ON [CDXP].[WP_GC_USER_ACCESS].ENTITY_ID = [CDXP].[WP_SETUP_DISCO].CUSTOMER_ID
INNER JOIN [CDXP].[WP_PORTAL_USERS] ON [CDXP].[WP_PORTAL_USERS].WP_PORTAL_USERS_ID = [CDXP].[WP_GC_USER_ACCESS].WP_PORTAL_USERS_ID
WHERE [CDXP].[WP_PORTAL_USERS].USER_NAME = '" + UserName + @"'
ORDER BY ABBREVIATION");// Fn.Data2Dropdown(db.SP_ddlDISCOs(UserName).ToList<SP_ddlDISCOs_Result>());
            //}
            return View();
        }

        [HttpPost]
        public ActionResult logout()
        {
            //if (Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["UserName"]).Value)) != "")
            //{
            try
            {
                var c = new HttpCookie("UserName");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);

            }
            catch (Exception e)
            {

            }
            try
            {
                var c = new HttpCookie("usr");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);

            }
            catch (Exception e)
            {

            }
            try
            {
                var c = new HttpCookie("contextTokenString");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);

            }
            catch (Exception e)
            {

            }
            try
            {
                var c = new HttpCookie("SPHostUrl");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);

            }
            catch (Exception e)
            {

            }




            //}

            Session.Clear();
            Session.Abandon();
            return View();
        }

    }

}



