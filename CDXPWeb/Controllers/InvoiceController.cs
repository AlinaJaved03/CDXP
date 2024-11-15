using CDXPWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Transactions;
using System.Globalization;
using System.IO.Compression;
using System.Security;
using Microsoft.SharePoint.Client;
using System.Web.Routing;

namespace CDXPWeb.Controllers
{
    public class InvoiceController : Controller
    {
        string[] monthNames = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Juy", "Aug", "Sep", "Oct", "Nov", "Dec" };

        ////////public DBDataContext() :
        ////////        base(global::System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString, mappingSource)
        ////////{
        ////////    OnCreated();
        ////////}
        private clsSQLCore Fn = new clsSQLCore(); string email = "";
        public void sessionCheck()
        {

            if (Convert.ToString(Session["UserName"]) == "")
            {
                try
                {
                    Session.Add("UserName", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["UserName"]).Value)));
                }
                catch (Exception e)
                {

                }

            }
            if (Convert.ToString(Session["usr"]) == "")
            {
                try
                {
                    Session.Add("usr", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["usr"]).Value)));
                }
                catch (Exception e)
                {

                }
            }
            if (Convert.ToString(Session["contextTokenString"]) == "")
            {
                try
                {
                    Session.Add("contextTokenString", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["contextTokenString"]).Value)));
                }
                catch (Exception e)
                {

                }
            }
            if (Convert.ToString(Session["SPHostUrl"]) == "")
            {
                try
                {
                    Session.Add("SPHostUrl", Convert.ToString(Convert.ToString(((HttpCookie)HttpContext.Request.Cookies["SPHostUrl"]).Value)));
                }
                catch (Exception e)
                {

                }
            }

        }

        // GET: Invoice

      
        [HttpPost]
        public ActionResult SearchInvoices()
        {
            sessionCheck();
            ViewBag.invTypDrp = Fn.Data2DropdownSQL(@"SELECT
  Invoice_id_parent, INVOICE_TYPES
FROM dbo.INVOICE_DISTINCT_TYPES");

            ViewBag.statusDrp = Fn.Data2DropdownSQL(@"SELECT DISTINCT
            APPROVED_STATUS ID,APPROVED_STATUS AS Status
            FROM[CPPA_CA].[DIARY_HEADER_INTERFACE]
            WHERE APPROVED_STATUS IN('Submitted', 'Returned', 'Received')
            UNION
                SELECT DISTINCT
                PAYMENT_STATUS, PAYMENT_STATUS
            FROM[CPPA_CA].[WP_GC_ERP_INVOICES]");

            ViewBag.ippDrp = Fn.Data2DropdownSQL(
                @"select distinct [dbo].[WP_GC_USER_ACCESS].vendor_id, vendor_new_name from [dbo].[WP_GC_USER_ACCESS] join [CPPA_CA].[AP_SUPPLIERS] on [CPPA_CA].[AP_SUPPLIERS].vendor_id = [dbo].[WP_GC_USER_ACCESS].VENDOR_ID where UserId = [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')");
            //var UserDTL = Fn.ExenID(@"SELECT UserID=WP_PORTAL_USERS_ID, UserTypeID=WP_SETUP_USER_TYPES_ID, Name=DISPLAY_NAME FROM [CDXP].[WP_PORTAL_USERS] WHERE USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"' FOR  JSON AUTO, INCLUDE_NULL_VALUES ");
            //var header_id_pk = Fn.Data2Json(@"[dbo].[sp_GetDiary] '" + Convert.ToString(Session["UserName"]) + @"'");
            //var obj = new { header_id_pk = header_id_pk, UserDTL = UserDTL };
            //ViewBag.invHeader = new JavaScriptSerializer().Serialize(obj);
            string UserType = Fn.ExenID(@"SELECT au.UserType FROM cppa_ca.ApiUsers au WHERE au.UserId = [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"') ");
            ViewBag.UserType = UserType;
            return View();
        }
        [HttpPost]
        public ActionResult SearchForm()
        {
            sessionCheck();
            var UserDTL = Fn.ExenID(@"SELECT UserID=WP_PORTAL_USERS_ID, UserTypeID=WP_SETUP_USER_TYPES_ID, Name=DISPLAY_NAME FROM [CDXP].[WP_PORTAL_USERS] WHERE USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"' FOR  JSON AUTO, INCLUDE_NULL_VALUES ");
            var header_id_pk = Fn.Data2Json(@"[dbo].[sp_GetDiary] '" + Convert.ToString(Session["UserName"]) + @"'");
            var obj = new { header_id_pk = header_id_pk, UserDTL = UserDTL };
            ViewBag.invHeader = new JavaScriptSerializer().Serialize(obj);
            return View();
        }

        [HttpPost]
        public string getWHours()
        {
            string wHours = Fn.ExenID(@"SELECT 
		                            CAST(MEANING AS VARCHAR) +'½'+
	                                CAST(ATTRIBUTE1  AS VARCHAR)+'½'+
		                            CAST(ATTRIBUTE2 AS VARCHAR) AS WorkingDayTimes 
	                              FROM 
		                            CPPA_CA.FND_LOOKUP_VALUES 
	                              WHERE 
		                            LOOKUP_TYPE = 'CPPA_INVOICE_SUBMIT_TIME_LK' 
		                            AND 
		                            MEANING = DATENAME(weekday,GETUTCDATE())");
            return wHours;
        }

        [HttpPost]
        public string getWHoursDetails()
        {
            string wHours = Fn.HTMLTableWithID_TR_Tag(@"SELECT 
	                                            FND_LOOKUP_VALUES_ID_PK,
	                                            MEANING As Weekdays,
	                                            Case WHEN ATTRIBUTE1 = '' OR ATTRIBUTE1 IS NULL OR LEN (TRIM (ATTRIBUTE1))  = 0 
                                                    THEN 'Off Day' 
                                                    ELSE ATTRIBUTE1 END as [Opening Time], 
	                                            Case WHEN ATTRIBUTE2 = '' OR ATTRIBUTE2 IS NULL OR LEN (TRIM (ATTRIBUTE2))  = 0 
                                                    THEN 'Off Day' 
                                                    ELSE ATTRIBUTE2 END as [Closing Time]
                                            FROM 
	                                            CPPA_CA.FND_LOOKUP_VALUES 
                                            WHERE 
	                                            LOOKUP_TYPE = 'CPPA_INVOICE_SUBMIT_TIME_LK'
                                                ORDER BY 
                                                    CASE
                                                        WHEN MEANING = 'Monday' THEN 2
                                                        WHEN MEANING = 'Tuesday' THEN 3
                                                        WHEN MEANING = 'Wednesday' THEN 4
                                                        WHEN MEANING = 'Thursday' THEN 5
                                                        WHEN MEANING = 'Friday' THEN 6
                                                        WHEN MEANING = 'Saturday' THEN 7
		                                                WHEN MEANING = 'Sunday' THEN 8

                                                    END ASC", "tblJ1");
            return wHours;
        }
        [HttpPost]
        public ActionResult MonthlyInvoice()
        {
            //Fn.WriteToFile1("Case started");
            sessionCheck();
            //var xxx = Fn.Data2Json(@"SELECT UserID=WP_PORTAL_USERS_ID, UserTypeID=WP_SETUP_USER_TYPES_ID, Name=DISPLAY_NAME FROM [CDXP].[WP_PORTAL_USERS] WHERE USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"'");
            var UserDTL = Fn.ExenID(@"SELECT UserID=WP_PORTAL_USERS_ID, UserTypeID=WP_SETUP_USER_TYPES_ID, Name=DISPLAY_NAME FROM [CDXP].[WP_PORTAL_USERS] WHERE USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"' FOR  JSON AUTO, INCLUDE_NULL_VALUES ");
            //Fn.WriteToFile1("Query has been executed" + UserDTL);
            var header_id_pk = Fn.Data2Json(@"[dbo].[sp_GetDiary] '" + Convert.ToString(Session["UserName"]) + @"'");
            header_id_pk = header_id_pk == "Cannot find table 0." ? "[]" : header_id_pk;
            var obj = new { header_id_pk = header_id_pk, UserDTL = UserDTL };
           // Fn.WriteToFile1("SP has been executed" + obj);
            ViewBag.invHeader = new JavaScriptSerializer().Serialize(obj);
            //ViewBag.invHeader = @"""header_id_pk"":" + header_id_pk+@""",""UserDTL"":"+ UserDTL ;
            //Fn.WriteToFile1("About to Return View");
            return View();
           
        }
        [HttpPost]
        public ActionResult DifferentialInvoice()
        {
            sessionCheck();
            //var xxx = Fn.Data2Json(@"SELECT UserID=WP_PORTAL_USERS_ID, UserTypeID=WP_SETUP_USER_TYPES_ID, Name=DISPLAY_NAME FROM [CDXP].[WP_PORTAL_USERS] WHERE USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"'");
            var UserDTL = Fn.ExenID(@"SELECT UserID=WP_PORTAL_USERS_ID, UserTypeID=WP_SETUP_USER_TYPES_ID, Name=DISPLAY_NAME, PostUser = IS_POST_USER  FROM [CDXP].[WP_PORTAL_USERS] WHERE USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"' FOR  JSON AUTO, INCLUDE_NULL_VALUES ");
            var header_id_pk = Fn.Data2Json(@"[dbo].[sp_GetDiary] '" + Convert.ToString(Session["UserName"]) + @"'");
            header_id_pk = header_id_pk == "Cannot find table 0." ? "[]" : header_id_pk;
            var obj = new { header_id_pk = header_id_pk, UserDTL = UserDTL };
            ViewBag.invHeader = new JavaScriptSerializer().Serialize(obj);
            //ViewBag.invHeader = @"""header_id_pk"":" + header_id_pk+@""",""UserDTL"":"+ UserDTL ;
            return View();
        }

        [HttpPost]
        public ActionResult InterestInvoice()
        {
            sessionCheck();
            //var xxx = Fn.Data2Json(@"SELECT UserID=WP_PORTAL_USERS_ID, UserTypeID=WP_SETUP_USER_TYPES_ID, Name=DISPLAY_NAME FROM [CDXP].[WP_PORTAL_USERS] WHERE USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"'");
            var UserDTL = Fn.ExenID(@"SELECT UserID=WP_PORTAL_USERS_ID, UserTypeID=WP_SETUP_USER_TYPES_ID, Name=DISPLAY_NAME FROM [CDXP].[WP_PORTAL_USERS] WHERE USER_NAME = '" + Convert.ToString(Session["UserName"]) + @"' FOR  JSON AUTO, INCLUDE_NULL_VALUES ");
            var header_id_pk = Fn.Data2Json(@"[dbo].[sp_GetDiary] '" + Convert.ToString(Session["UserName"]) + @"'");
            header_id_pk = header_id_pk == "Cannot find table 0." ? "[]" : header_id_pk;
            var invoiceTypesInterest = Fn.Data2Json(@"SELECT    CPPA_CA.PPA_APPLICABLE_INVOICES.APP_INVOICES_ID_PK,     LookUpQry.MEANING INVOICE_TYPES, CPPA_CA.PPA_APPLICABLE_INVOICES.IS_HOURLY
FROM            CPPA_CA.APP_SUPPLIER_SITE_ALL INNER JOIN
                         CPPA_CA.PPA_HEADER ON CPPA_CA.APP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID = CPPA_CA.PPA_HEADER.VENDOR_SITE_ID_FK INNER JOIN
                         CPPA_CA.PPA_APPLICABLE_INVOICES ON CPPA_CA.PPA_HEADER.HEADER_ID_PK = CPPA_CA.PPA_APPLICABLE_INVOICES.HEADER_ID_FK INNER JOIN
                             (SELECT        LOOKUP_CODE, MEANING
                               FROM            CPPA_CA.FND_LOOKUP_VALUES
                               WHERE        (LOOKUP_TYPE = 'CPPA_INVOICE_TYPES_LK')) AS LookUpQry ON CPPA_CA.PPA_APPLICABLE_INVOICES.INVOICE_TYPES = LookUpQry.LOOKUP_CODE
WHERE  (CPPA_CA.PPA_HEADER.APPROVAL_STATUS='Approved') And (CPPA_CA.APP_SUPPLIER_SITE_ALL.VENDOR_SITE_ID IN (
SELECT ISNULL((SELECT VENDOR_SITE_ID FROM dbo.WP_GC_USER_ACCESS
INNER JOIN [CPPA_CA].[ApiUsers] ON [CPPA_CA].[ApiUsers].UserId = dbo.WP_GC_USER_ACCESS.UserId 
WHERE [CPPA_CA].[ApiUsers].[USER_NAME] = '" + Convert.ToString(Session["UserName"]) + @"' AND [CPPA_CA].[ApiUsers].[UserType]=4
),0)
))  and LookUpQry.MEANING LIKE '%Interest%'");
            var obj = new { header_id_pk = header_id_pk, UserDTL = UserDTL, Interestinvoice_id_pk = invoiceTypesInterest };

            ViewBag.invHeader = new JavaScriptSerializer().Serialize(obj);
            //ViewBag.invHeader = @"""header_id_pk"":" + header_id_pk+@""",""UserDTL"":"+ UserDTL ;
            return View();
        }
        [HttpPost]
        public ActionResult HourlyInvoice()
        {
            return View();
        }


        [HttpPost]
        public ActionResult IppDashboard()
        {
            sessionCheck();
            string User = Fn.ExenID(@"SELECT CAST(au.UserId AS VARCHAR(100))+'½'+CAST(au.UserType AS VARCHAR(100))+'½'+CAST(ISNULL(au.IS_STOCK_MENU_ACCESS,0) AS VARCHAR(100))  FROM cppa_ca.ApiUsers au WHERE au.UserId = [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')");
            ViewBag.UserType = User.Split('½')[1];
            ViewBag.StockAccess = User.Split('½')[2];

            ViewBag.response = "[]";
            if (User.Split('½')[1] == "4")
            {
                ViewBag.response = Fn.Data2Json("EXEC GetDHIbyStatusbyUserID " + Convert.ToString(User.Split('½')[0]));
                
            }
            else if (User.Split('½')[1] == "2" || User.Split('½')[1] == "3")
            {
                ViewBag.response = Fn.Data2Json("EXEC GetDHIbyStatus  " + Convert.ToString(User.Split('½')[0]));
               
            }

            return View();
            
        }

        [HttpPost]
        public string AjaxCall()
        {
            sessionCheck();
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
                        DataTable dt0 = Fn.FillDSet("EXEC GetAllBlockFuelsByPPAHeaderIDandAppInvTypeIDJSON '" + Convert.ToString(dataID[1]) + @"', '" + Convert.ToString(dataID[3]) + @"'").Tables[0];
                        JavaScriptSerializer serializer = new JavaScriptSerializer();

                        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                        Dictionary<string, object> row;
                        foreach (DataRow dr in dt0.Rows)
                        {
                            row = new Dictionary<string, object>();
                            foreach (DataColumn col in dt0.Columns)
                            {
                                row.Add(col.ColumnName, dr[col]);
                            }
                            rows.Add(row);
                        }
                        string ddlItems = Fn.Data2DropdownSQL(" SELECT DISTINCT VENDOR_INV_NO V, VENDOR_INV_NO FROM [CPPA_CA].[WP_GC_ERP_INVOICES] WHERE VENDOR_SITE_ID_FK=" + Convert.ToString(dataID[2]) + " AND INV_TYPE=(SELECT TOP(1) INVOICE_TYPES FROM [CPPA_CA].[PPA_APPLICABLE_INVOICES] WHERE APP_INVOICES_ID_PK=" + dataID[3] + ") ");

                        var obj0 = new { blockData = serializer.Serialize(rows), adjInv = ddlItems };
                        Returnvls = new JavaScriptSerializer().Serialize(obj0);


                        break;

                    case 1:
                        //Returnvls = Fn.Data2Json(@"Select TOP 100 HEADER_ID_PK = [CPPA_CA].[DIARY_HEADER_INTERFACE].DIARY_HEADER_ID, DOCUMENT_NO = [CPPA_CA].[DIARY_HEADER_INTERFACE].VEN_INV_LETTER_NO, DOCUMENT_DATE = [CPPA_CA].[DIARY_HEADER_INTERFACE].VEN_INV_LETTER_DATE, DOC_REC_ID_FK = 0 
                        //from [CPPA_CA].[DIARY_HEADER_INTERFACE]
                        //inner join [CPPA_CA].[PPA_APPLICABLE_INVOICES] on  [CPPA_CA].[DIARY_HEADER_INTERFACE].INVOICE_TYPE_ID = [CPPA_CA].[PPA_APPLICABLE_INVOICES].APP_INVOICES_ID_PK                       
                        //where [CPPA_CA].[PPA_APPLICABLE_INVOICES].INVOICE_TYPES in (
                        //'EPP', 'EPP_V&OM', 'UNDELIVERED ENERGY INVOICE', 'PRE_COD_ENERGY_PAYMENT','STARTUP', 'PRE_COD_TEST_EN_PAY'
                        //) and [CPPA_CA].[DIARY_HEADER_INTERFACE].ORGANIZATION_ID= " + dataID[2] + @"                       
                        //and [CPPA_CA].[DIARY_HEADER_INTERFACE].FORM_NAME = 'MonthlyForm.aspx' ORDER BY [CPPA_CA].[DIARY_HEADER_INTERFACE].VEN_INV_LETTER_DATE DESC");
                        //break;

                        Returnvls = Fn.Data2Json(@"SELECT TOP 100
	                HEADER_ID_PK = [CPPA_CA].[DIARY_HEADER_INTERFACE].DIARY_HEADER_ID
                   ,DOCUMENT_NO = [CPPA_CA].[DIARY_HEADER_INTERFACE].VEN_INV_LETTER_NO
                   ,DOCUMENT_DATE = [CPPA_CA].[DIARY_HEADER_INTERFACE].VEN_INV_LETTER_DATE
                   ,DOC_REC_ID_FK = 0
                   ,APPROVED_STATUS
                    FROM [CPPA_CA].[DIARY_HEADER_INTERFACE]
                    INNER JOIN [CPPA_CA].[PPA_APPLICABLE_INVOICES]
	                    ON [CPPA_CA].[DIARY_HEADER_INTERFACE].INVOICE_TYPE_ID = [CPPA_CA].[PPA_APPLICABLE_INVOICES].APP_INVOICES_ID_PK
                    left JOIN CPPA_CA.WP_GC_ERP_INVOICES
	                    ON CPPA_CA.WP_GC_ERP_INVOICES.TRANSACTION_NO = CPPA_CA.DIARY_HEADER_INTERFACE.TransactionNumber
						AND PAYMENT_STATUS <> 'claim returned'
                    WHERE [CPPA_CA].[PPA_APPLICABLE_INVOICES].INVOICE_TYPES IN (
                    'EPP', 'EPP_V&OM', 'UNDELIVERED ENERGY INVOICE', 'PRE_COD_ENERGY_PAYMENT', 'STARTUP', 'PRE_COD_TEST_EN_PAY'
                    )
                    AND [CPPA_CA].[DIARY_HEADER_INTERFACE].ORGANIZATION_ID = " + dataID[2] + @"
                    AND [CPPA_CA].[DIARY_HEADER_INTERFACE].FORM_NAME = 'MonthlyForm.aspx'
                    ORDER BY [CPPA_CA].[DIARY_HEADER_INTERFACE].VEN_INV_LETTER_DATE DESC");
                        break;

                    //case 2:
                    //    try
                    //    {
                    //        DataSet ds2 = new DataSet();
                    //        DataTable dt2 = new DataTable();
                    //        ds2 = Fn.FillDSet(@"SELECT ATTACH_HEADER_ID,DIARY_HEADER_ID,ATTACHMENT_TYPE,ATTACHMENT_TITLE,ATTACHMENT_DESC,ATTACHMENT_CATEGORY,ATTACHMENT_SHORT_TEXT,FILE_NAME,FILE_TYPE,CREATION_DATE FROM [CPPA_CA].[ATTACHMENT_HEADER] WHERE ATTACH_HEADER_ID =" + dataID[1]);
                    //        dt2 = ds2.Tables[0];
                    //        if (dt2.Rows.Count > 0)
                    //        {
                    //            string nm = Convert.ToString(dt2.Rows[0]["FILE_NAME"]);
                    //            string ftyp = Convert.ToString(dt2.Rows[0]["ATTACHMENT_TYPE"]);

                    //            string ext = nm.Split('.')[nm.Split('.').Length - 1];
                    //            String path = HttpContext.Server.MapPath("/Content/_FilesNew"); //Maintain Path for record purpose
                    //            string FilePath = Path.Combine(path, dataID[1] + "." + ext);
                    //            if (!System.IO.File.Exists(FilePath))
                    //            {
                    //                DataSet ds21 = new DataSet();
                    //                DataTable dt21 = new DataTable();
                    //                ds21 = Fn.FillDSet("exec sp_GetFilesByID " + dataID[1]);
                    //                dt21 = ds21.Tables[0];
                    //                byte[] byteArray = Convert.FromBase64String(Convert.ToString(dt21.Rows[0]["FILE_DATA"]));
                    //                string HDR2 = Convert.ToBase64String(byteArray);
                    //                byte[] FileBytes = Convert.FromBase64String(HDR2);
                    //                System.IO.File.WriteAllBytes(FilePath, FileBytes);
                    //            }
                    //            var obj2 = new { fdata = "", name = nm, typ = ftyp, ext = ext };
                    //            Returnvls = new JavaScriptSerializer().Serialize(obj2);
                    //        }
                    //        else
                    //        {
                    //            Returnvls = "No data found for this id";
                    //        }

                    //    }
                    //    catch (Exception ex)
                    //    {

                    //        Returnvls = ex.Message;
                    //    }
                    //    break;


                    case 2:
                        try
                        {


                            // Retrieve the file data from the stored procedure
                            DataSet dsFileData = Fn.FillDSet("exec sp_GetFilesByID " + dataID[1]);
                            DataTable dtFileData = dsFileData.Tables[0];

                            if (dtFileData.Rows.Count > 0)
                            {
                                // Get the binary data from the database and convert it to base64
                                byte[] fileBytes = Convert.FromBase64String(Convert.ToString(dtFileData.Rows[0]["FILE_DATA"]));
                                string base64FileData = Convert.ToBase64String(fileBytes);
                                string fileName = Convert.ToString(dtFileData.Rows[0]["FILE_NAME"]);
                                string fileType = Convert.ToString(dtFileData.Rows[0]["ATTACHMENT_TYPE"]);
                                string extension = fileName.Split('.')[fileName.Split('.').Length - 1]; // Get the file extension


                                // Create an object with the file data and metadata to return to the client
                                var fileObject = new
                                {
                                    fdata = base64FileData, // Base64 string of file data
                                    name = fileName,
                                    typ = fileType,
                                    ext = extension
                                };

                                // Serialize the object to JSON for the client to use
                                Returnvls = new JavaScriptSerializer().Serialize(fileObject);
                            }
                            else
                            {
                                Returnvls = "File data not found in the database.";
                            }

                        }
                        catch (Exception ex)
                        {
                            // Handle or log the exception
                            Returnvls = "An error occurred: " + ex.Message;
                        }
                        break;
                    case 3:
                        try
                        {
                            //Fn.WriteToFile1("Case started");
                            ApiUser userobj = new ApiUser();
                            sp_DHI_BY_IDNEWResult HDR3 = new sp_DHI_BY_IDNEWResult();// = new sp_DHI_BY_IDNEWResult();
                            List<sp_DDTL_BY_DHIIDResult> DTLs;
                            List<object> flist = new List<object>();
                            // List<object> dislist = new List<object>();
                            List<object> adjList = new List<object>();
                            List<GetAllBlockFuelsByPPAHeaderIDandAppInvTypeIDJSONResult> blklist = new List<GetAllBlockFuelsByPPAHeaderIDandAppInvTypeIDJSONResult>();
                           // Fn.WriteToFile1("");

                            using (DBDataContext db = new DBDataContext())
                            {
                                db.CommandTimeout = 5000;
                                //HDR = db.sp_DHI_BY_ID(Convert.ToDecimal(id)).FirstOrDefault();
                               
                                decimal diaryhid = Convert.ToDecimal(dataID[1]);
                               
                                decimal vendorSiteId = Convert.ToDecimal(dataID[2]);
                              
                                string userName = Convert.ToString(Session["UserName"]);
                              

                                userobj = db.ApiUsers.Where(v => v.USER_NAME == userName).FirstOrDefault();
                               

                                HDR3 = db.sp_DHI_BY_IDNEW(diaryhid, userobj.UserId, pkdatetime, "VIEWED", Convert.ToInt32(vendorSiteId), "").FirstOrDefault();
                               

                                DTLs = db.sp_DDTL_BY_DHIID(Convert.ToDecimal(dataID[1])).ToList<sp_DDTL_BY_DHIIDResult>();
                                //UserName = db.ApiUsers.Where(x => x.UserId == UserID).FirstOrDefault().Name;
                                

                                //flist = db.ATTACHMENT_HEADERs.Where(v => v.DIARY_HEADER_ID == Convert.ToDecimal(dataID[1]) && (v.bisDeleted == false || v.bisDeleted == null)).Select(s => new { s.FILE_NAME, s.ATTACH_HEADER_ID, s.FILE_TYPE, s.ATTACHMENT_TITLE, s.ATTACHMENT_DESC }).ToList<object>();
                                //Fn.WriteToFile1("flist  " + flist.ToString());

                                flist = db.ATTACHMENT_HEADERs.Where(v => v.DIARY_HEADER_ID == Convert.ToDecimal(dataID[1]) && (v.bisDeleted == false || v.bisDeleted == null)).Select(s => new { s.FILE_NAME, s.ATTACH_HEADER_ID, s.FILE_TYPE, s.ATTACHMENT_TITLE, s.ATTACHMENT_DESC, s.FILE_LINK }).ToList<object>();
                              

                                //dislist = db.WP_GC_ERP_INVOICEs.Where(v => v.SHOW_DISP_ON_PORTAL == "Y" &&  v.TRANSACTION_NO == Convert.ToString(dataID[1])).Select(se => new {se.DISPUTE_ATTACH_ID, se.DISPUTE_REMARKS, se.SHOW_DISP_ON_PORTAL, se.SEND_DISP_TO_PORTAL, se.FILE_NAME_1, se.FILE_DATA_1 }).ToList<object>();


                                adjList = db.WP_GC_ADJUSTMENTs.Where(v => v.DIARY_HEADER_ID == Convert.ToDecimal(dataID[1])).Select(s => new { s.ADJ_HEADER_ID, s.REFERENCE_INVOICE_NUMBER, s.ADJ_TYPES, s.ADJ_AMOUNT, s.REMARKS }).ToList<object>();

                               

                                blklist = db.GetAllBlockFuelsByPPAHeaderIDandAppInvTypeIDJSON(Convert.ToInt32(HDR3.PPA_HEADER_ID), Convert.ToInt32(HDR3.INVOICE_TYPE_ID)).ToList<GetAllBlockFuelsByPPAHeaderIDandAppInvTypeIDJSONResult>();
                            }

                            string disputeAttach = Fn.Data2Json(@"select * from [dbo].[DISPUTE_ATTACHMENTS] a
                                                            inner join [CPPA_CA].[WP_GC_ERP_INVOICES] b 
                                                            on a.AP_INVOICE_ID=b.AP_INVOICE_ID
                                                            inner join [CPPA_CA].[DIARY_HEADER_INTERFACE] c
                                                            on b.TRANSACTION_NO=c.TransactionNumber
                                                            where b.PAYMENT_STATUS in ('Verified', 'Fully Paid', 'Partially Paid','Claim Returned') and c.DIARY_HEADER_ID = " + dataID[1]);

                            string fuelSummary = Fn.Data2Json(@"SELECT ppa.FUEL_TYPE,   C.COMP_NAME, D.UNIT as UNIT, SUM(C.COMP_VALUE) COMP_VALUE FROM [CPPA_CA].[BLOCKS_HEADER_INTERFACE] B 
                            INNER JOIN[CPPA_CA].[COMP_HEADER_INTERFACE] C ON B.[BLOCK_HEADER_ID] = C.[BLOCK_HEADER_ID] inner join[CPPA_CA].[PPA_BLOCKS_FUELS] ppa on B.[PPA_BLOCK_HEADER_ID] = ppa.BLK_FUEL_ID_PK 
                            INNER JOIN CPPA_CA.PPA_COMP_DEFS D ON C.COM_DEF_ID_FK = D.COMP_DEF_ID_PK
                            WHERE[DIARY_HEADER_ID] = " + dataID[1] + @"
                            GROUP BY ppa.FUEL_TYPE, C.COMP_NAME, D.UNIT
                            ORDER BY FUEL_TYPE, COMP_NAME");
                          
                            if (HDR3 != null)
                            {
                                var obj3 = new { DispAttach = disputeAttach, FuelSummary = fuelSummary, DHIHDR = HDR3, DTL = DTLs, FList = flist, AdjList = adjList, UserDtl = new { uid = userobj.UserId, utyp = userobj.UserType, Name = userobj.USER_NAME }, blklist = blklist };
                                JavaScriptSerializer sr = new JavaScriptSerializer();
                                sr.MaxJsonLength = int.MaxValue;
                                var data = sr.Serialize(obj3);
                                

                                Returnvls = data;
                            }
                            else
                            {
                                //return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data found for this id");
                                Returnvls = "No data found for this id";
                            }
                           
                        }
                        catch (Exception ex) {
                            Fn.WriteToFile1("exception ------------- "+ex.Message);
                        }
                        break;

                    case 4:

                        Returnvls = Fn.Data2DropdownSQL(" SELECT DISTINCT VENDOR_INV_NO V, VENDOR_INV_NO FROM [CPPA_CA].[WP_GC_ERP_INVOICES] WHERE VENDOR_SITE_ID_FK=" + Convert.ToString(dataID[1]) + " AND INV_TYPE=(SELECT TOP(1) INVOICE_TYPES FROM [CPPA_CA].[PPA_APPLICABLE_INVOICES] WHERE APP_INVOICES_ID_PK=" + dataID[2] + ") ");
                        break;


                    case 5:
                        Returnvls = Fn.Data2Json(@"SELECT
                         AP_INVOICE_ID HEADER_ID_PK, 
                         VENDOR_INV_NO,
                         INV_TYPE,
                         FORMAT(INV_PERIOD_FRM,'dd-MMM-yyyy','en-us') AS INV_PERIOD_FRM, 
                         FORMAT(INV_PERIOD_TO,'dd-MMM-yyyy','en-us') AS INV_PERIOD_TO ,
                         0 PPA_ID_FK,TOTAL_CLAIMED_AMOUNT,  
                         TOTAL_VERIFIED_AMOUNT TOT_INC_GST_CL 
                         FROM
                           [CPPA_CA].[WP_GC_ERP_INVOICES]
                         INNER JOIN [CPPA_CA].[PPA_APPLICABLE_INVOICES] ON [CPPA_CA].[PPA_APPLICABLE_INVOICES].INVOICE_TYPES= [CPPA_CA].[WP_GC_ERP_INVOICES].INV_TYPE
                         WHERE  
                          (VENDOR_SITE_ID_FK = " + dataID[1] + @") 
                          AND AP_INVOICE_ID IS NOT NULL
                          AND PAYMENT_STATUS <> 'Claim Returned'
                          AND (CPPA_CA.PPA_APPLICABLE_INVOICES.APP_INVOICES_ID_PK=" + dataID[2] + @") 
                          ORDER BY  VENDOR_INV_NO");
                        break;

                    case 6:
                        DataTable dt = Fn.FillDSet("EXEC GetAllBlockFuelsByPPAHeaderIDandAppInvTypeIDJSON '" + Convert.ToString(dataID[1]) + @"', '" + Convert.ToString(dataID[3]) + @"'").Tables[0];
                        JavaScriptSerializer serializer6 = new JavaScriptSerializer();

                        List<Dictionary<string, object>> rows6 = new List<Dictionary<string, object>>();
                        Dictionary<string, object> row6;
                        foreach (DataRow dr in dt.Rows)
                        {
                            row = new Dictionary<string, object>();
                            foreach (DataColumn col in dt.Columns)
                            {
                                row.Add(col.ColumnName, dr[col]);
                            }
                            rows6.Add(row);
                        }
                        string ddlItems6 = Fn.Data2DropdownSQL(" SELECT DISTINCT VENDOR_INV_NO V, VENDOR_INV_NO FROM [CPPA_CA].[WP_GC_ERP_INVOICES] WHERE VENDOR_SITE_ID_FK=" + Convert.ToString(dataID[2]) + " AND INV_TYPE=(SELECT TOP(1) INVOICE_TYPES FROM [CPPA_CA].[PPA_APPLICABLE_INVOICES] WHERE APP_INVOICES_ID_PK=" + dataID[3] + ") ");
                        //return Request.CreateResponse(HttpStatusCode.OK, new { blockData = serializer6.Serialize(rows), adjInv = ddlItems });
                        Returnvls = new JavaScriptSerializer().Serialize(new { blockData = serializer6.Serialize(rows6), adjInv = ddlItems6 });
                        break;

                    case 7:
                        Returnvls = Fn.Data2DropdownSQL(" SELECT DISTINCT VENDOR_INV_NO V, VENDOR_INV_NO FROM [CPPA_CA].[WP_GC_ERP_INVOICES] WHERE VENDOR_SITE_ID_FK=" + dataID[1] + " AND INV_TYPE=(SELECT TOP(1) INVOICE_TYPES FROM [CPPA_CA].[PPA_APPLICABLE_INVOICES] WHERE APP_INVOICES_ID_PK=" + dataID[2] + ") ");
                        break;
                    case 8:
                        try
                        {
                            Returnvls = Fn.Data2Json(@"

                        SELECT       
                            CPPA_CA.PPA_BLOCKS_FUELS.BLOCK_NO, 
                            CPPA_CA.PPA_BLOCKS_FUELS.FUEL_TYPE,
                            CPPA_CA.PPA_COMP_DEFS.COMP_NAME, 
                            CPPA_CA.PPA_COMP_DEFS.UNIT, 
                            CPPA_CA.PPA_BLOCKS_FUELS.BLK_FUEL_ID_PK, 
                            CPPA_CA.PPA_COMP_DEFS.COMP_DEF_ID_PK,
                            CPPA_CA.PPA_COMP_DEFS.INC_IN_TOT
                       FROM         
                            CPPA_CA.PPA_HEADER INNER JOIN
                            CPPA_CA.WP_GC_ERP_INVOICES ON CPPA_CA.PPA_HEADER.VENDOR_SITE_ID_FK = CPPA_CA.WP_GC_ERP_INVOICES.VENDOR_SITE_ID_FK INNER JOIN
                            CPPA_CA.PPA_APPLICABLE_INVOICES INNER JOIN
                            CPPA_CA.PPA_BLOCKS_FUELS ON CPPA_CA.PPA_APPLICABLE_INVOICES.APP_INVOICES_ID_PK = CPPA_CA.PPA_BLOCKS_FUELS.APP_INVOICES_ID_FK
                            INNER JOIN  CPPA_CA.PPA_COMP_DEFS ON CPPA_CA.PPA_BLOCKS_FUELS.BLK_FUEL_ID_PK = CPPA_CA.PPA_COMP_DEFS.BLK_FUEL_ID_FK ON 
                            CPPA_CA.PPA_HEADER.HEADER_ID_PK = CPPA_CA.PPA_APPLICABLE_INVOICES.HEADER_ID_FK AND CPPA_CA.WP_GC_ERP_INVOICES.INV_TYPE = CPPA_CA.PPA_APPLICABLE_INVOICES.INVOICE_TYPES
                      WHERE  
                           (CPPA_CA.PPA_COMP_DEFS.IS_DISABLE = 'N'AND CPPA_CA.PPA_COMP_DEFS.SHOW_ON_DIARY='YES')
                      AND
                           (CPPA_CA.WP_GC_ERP_INVOICES.AP_INVOICE_ID = " + dataID[1] + @") 
                      AND
                           (CPPA_CA.PPA_HEADER.APPROVAL_STATUS = 'Approved')
                      ORDER BY 
                        CPPA_CA.PPA_BLOCKS_FUELS.BLOCK_NO, 
                        CPPA_CA.PPA_COMP_DEFS.COMP_NAME");
                            //return Request.CreateResponse(HttpStatusCode.OK, invList);
                        }
                        catch (Exception ex)
                        {

                            return ex.Message;// Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
                        }
                        break;
                    case 9:

                        ApiUser userobj9 = new ApiUser();

                        sp_DHI_BY_IDNEWResult HDR9;// = new sp_DHI_BY_IDNEWResult();
                        List<sp_DifferentialDTL_BY_DHIIDResult> DTLs9 = new List<sp_DifferentialDTL_BY_DHIIDResult>();

                        List<object> flist9 = new List<object>();
                        List<object> adjList9 = new List<object>();
                        //List<object> dislistDiff = new List<object>();
                        using (DBDataContext db = new DBDataContext())
                        {
                            db.CommandTimeout = 5000;
                            //HDR = db.sp_DHI_BY_ID(Convert.ToDecimal(id)).FirstOrDefault();
                            userobj9 = db.ApiUsers.Where(v => v.USER_NAME == Convert.ToString(Session["UserName"])).FirstOrDefault();
                            HDR9 = db.sp_DHI_BY_IDNEW(Convert.ToDecimal(dataID[1]), userobj9.UserId, pkdatetime, "VIEWED", Convert.ToInt32(dataID[2]), "").FirstOrDefault();
                            DTLs9 = db.sp_DifferentialDTL_BY_DHIID(Convert.ToDecimal(dataID[1])).ToList<sp_DifferentialDTL_BY_DHIIDResult>();
                            //UserName = db.ApiUsers.Where(x => x.UserId == userobj9.UserId).FirstOrDefault().Name;
                            flist9 = db.ATTACHMENT_HEADERs.Where(v => v.DIARY_HEADER_ID == Convert.ToDecimal(dataID[1]) && (v.bisDeleted == false || v.bisDeleted == null)).Select(s => new { s.FILE_NAME, s.ATTACH_HEADER_ID, s.FILE_TYPE, s.ATTACHMENT_TITLE, s.ATTACHMENT_DESC, s.FILE_LINK }).ToList<object>();
                            adjList9 = db.WP_GC_ADJUSTMENTs.Where(v => v.DIARY_HEADER_ID == Convert.ToDecimal(dataID[1])).Select(s => new { s.ADJ_HEADER_ID, s.REFERENCE_INVOICE_NUMBER, s.ADJ_TYPES, s.ADJ_AMOUNT, s.REMARKS }).ToList<object>();
                        }

                        string disputeAttachDiff = Fn.Data2Json(@"select * from [dbo].[DISPUTE_ATTACHMENTS] a
                                                            inner join [CPPA_CA].[WP_GC_ERP_INVOICES] b 
                                                            on a.AP_INVOICE_ID=b.AP_INVOICE_ID
                                                            inner join [CPPA_CA].[DIARY_HEADER_INTERFACE] c
                                                            on b.TRANSACTION_NO=c.TransactionNumber
                                                            where b.PAYMENT_STATUS in ('Verified', 'Fully Paid', 'Partially Paid','Claim Returned') and c.DIARY_HEADER_ID =" + dataID[1]);


                        if (HDR9 != null)
                        {
                            Returnvls = new JavaScriptSerializer().Serialize(new
                            {
                                DispAttach = disputeAttachDiff,
                                DHIHDR = HDR9,
                                DTL = DTLs9,
                                FList = flist9,
                                AdjList = adjList9,
                                UserDtl = new { uid = userobj9.UserId, utyp = userobj9.UserType, Name = userobj9.Name }
                            });
                            // return Request.CreateResponse(HttpStatusCode.OK, new { DHIHDR = HDR, DTL = DTLs9, FList = flist9, AdjList = adjList9, UserDtl = new { uid = userobj9.UserId, utyp = userobj9.UserType, Name = userobj9.Name } });
                        }
                        else
                        {
                            Returnvls = "No data found for this id";
                        }
                        break;

                    case 10:
                        ApiUser userobj10 = new ApiUser();


                        sp_DHI_BY_IDNEWResult HDR10;// = new sp_DHI_BY_IDNEWResult();
                        List<sp_InterestDTL_BY_DHIIDResult> DTLs10;
                        List<object> flist10 = new List<object>();
                        List<object> adjList10 = new List<object>();
                        using (DBDataContext db = new DBDataContext())
                        {
                            db.CommandTimeout = 5000;
                            userobj10 = db.ApiUsers.Where(v => v.USER_NAME == Convert.ToString(Session["UserName"])).FirstOrDefault();

                            HDR10 = db.sp_DHI_BY_IDNEW(Convert.ToDecimal(dataID[1]), userobj10.UserId, pkdatetime, "VIEWED", Convert.ToInt32(dataID[2]), "").FirstOrDefault();

                            DTLs10 = db.sp_InterestDTL_BY_DHIID(Convert.ToDecimal(dataID[1])).ToList<sp_InterestDTL_BY_DHIIDResult>();
                            //UserName = db.ApiUsers.Where(x => x.UserId == UserID).FirstOrDefault().Name;

                            flist10 = db.ATTACHMENT_HEADERs.Where(v => v.DIARY_HEADER_ID == Convert.ToDecimal(dataID[1]) && (v.bisDeleted == false || v.bisDeleted == null)).Select(s => new { s.FILE_NAME, s.ATTACH_HEADER_ID, s.FILE_TYPE, s.ATTACHMENT_TITLE, s.ATTACHMENT_DESC, s.FILE_LINK }).ToList<object>();

                            adjList10 = db.WP_GC_ADJUSTMENTs.Where(v => v.DIARY_HEADER_ID == Convert.ToDecimal(dataID[1])).Select(s => new { s.ADJ_HEADER_ID, s.REFERENCE_INVOICE_NUMBER, s.ADJ_TYPES, s.ADJ_AMOUNT, s.REMARKS }).ToList<object>();
                        }

                        string disputeAttachInterest = Fn.Data2Json(@"select * from [dbo].[DISPUTE_ATTACHMENTS] a
                                                            inner join [CPPA_CA].[WP_GC_ERP_INVOICES] b 
                                                            on a.AP_INVOICE_ID=b.AP_INVOICE_ID
                                                            inner join [CPPA_CA].[DIARY_HEADER_INTERFACE] c
                                                            on b.TRANSACTION_NO=c.TransactionNumber
                                                            where b.PAYMENT_STATUS in ('Verified', 'Fully Paid', 'Partially Paid','Claim Returned') and c.DIARY_HEADER_ID = " + dataID[1]);


                        if (HDR10 != null)
                        {
                            Returnvls = new JavaScriptSerializer().Serialize(new { DispAttach = disputeAttachInterest, DHIHDR = HDR10, DTL = DTLs10, FList = flist10, AdjList = adjList10, UserDtl = new { uid = userobj10.UserId, utyp = userobj10.UserType, Name = userobj10.Name } });
                            //return Request.CreateResponse(HttpStatusCode.OK, new { DHIHDR = HDR, DTL = DTLs, FList = flist, AdjList = adjList, UserDtl = new { uid = UserID, utyp = UserTypeID, Name = UserName } });
                        }
                        else
                        {
                            //return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data found for this id");
                            Returnvls = "No data found for this id";
                        }
                        break;
                    case 11:

                        var list11 = new List<sp_INVOICE_TYPES4Interest_by_VENDOR_SITE_IDResult>();
                        using (DBDataContext db = new DBDataContext())
                        {
                            db.CommandTimeout = 200; // 200 seconds
                            list11 = db.sp_INVOICE_TYPES4Interest_by_VENDOR_SITE_ID(Convert.ToInt32(dataID[1])).ToList<sp_INVOICE_TYPES4Interest_by_VENDOR_SITE_IDResult>();
                        }

                        if (list11 != null)
                        {
                            Returnvls = new JavaScriptSerializer().Serialize(list11);
                            // return Request.CreateResponse(HttpStatusCode.OK, list);
                        }
                        else
                        {
                            //return Request.CreateErrorResponse(HttpStatusCode.OK, "No Vendor Sites found");
                            Returnvls = "No Vendor Sites found";
                        }

                        break;
                    case 12:
                        Returnvls = Fn.Data2Json(@"                   
                  SELECT    PAYMENT_STATUS,
                            AP_INVOICE_ID HEADER_ID_PK,
                            CASE WHEN ISNULL(TRANSACTION_NO,'')='' THEN
                            VENDOR_INV_NO ELSE
                            VENDOR_INV_NO END AS VENDOR_INV_NO,
                            INV_TYPE,
                            FORMAT(INV_PERIOD_FRM,'dd-MMM-yyyy','en-us') AS INV_PERIOD_FRM,
                            FORMAT(INV_PERIOD_TO,'dd-MMM-yyyy','en-us') AS INV_PERIOD_TO ,
                            0 PPA_ID_FK,TOTAL_CLAIMED_AMOUNT,
                            TOTAL_VERIFIED_AMOUNT TOT_INC_GST_CL
                FROM
                            [CPPA_CA].[WP_GC_ERP_INVOICES]
                WHERE
                            (PAYMENT_STATUS = 'Fully Paid' or PAYMENT_STATUS ='DN Adjusted') 
			    AND 
					        INV_TYPE NOT IN ('L.P','PRE LIVE INTEREST')
 
                AND
                            (VENDOR_SITE_ID_FK =  " + dataID[1] + @")
                            AND
                            [CPPA_CA].[WP_GC_ERP_INVOICES].AP_INVOICE_ID IS NOT NULL
                            AND
                            Vendor_INV_no NOT in ( SELECT IPP_INV_NO
                            FROM CPPA_CA.WP_GC_INV_DIFF_PARENT DIFF
                            INNER JOIN CPPA_CA.DIARY_HEADER_INTERFACE dhi ON DHI.DIARY_HEADER_ID = diff.DIARY_HEADER_ID_FK
                            WHERE dhi.IS_DIFFERENTIAL = 'I'
                            AND dhi.ORGANIZATION_ID =  " + dataID[1] + @" and (dhi.APPROVED_STATUS not in  ('Withdraw','Deleted'))
                            AND DIFF.PARENT_INV_ID_FK = AP_INVOICE_ID 
                                        )
                AND Vendor_INV_no not in (
									SELECT  v.VEN_INV_LETTER_NO
									FROM CPPA_CA.DIARY_HEADER_INTERFACE v
									WHERE ORGANIZATION_ID = 284070
									AND INVOICE_TYPES = 'CPP'
									AND CAST(SUBMIT_DATE AS DATE) <= '2023-12-28'
								)
                           
                UNION

                SELECT       PAYMENT_STATUS,
                            AP_INVOICE_ID HEADER_ID_PK,
                            CASE WHEN ISNULL(TRANSACTION_NO,'')='' THEN
                            CONCAT_WS(' Amount/',VENDOR_INV_NO,TOTAL_VERIFIED_AMOUNT) ELSE
                            VENDOR_INV_NO END AS VENDOR_INV_NO,
                            INV_TYPE,
                            FORMAT(INV_PERIOD_FRM,'dd-MMM-yyyy','en-us') AS INV_PERIOD_FRM,
                            FORMAT(INV_PERIOD_TO,'dd-MMM-yyyy','en-us') AS INV_PERIOD_TO ,
                            0 PPA_ID_FK,TOTAL_CLAIMED_AMOUNT,
                            TOTAL_VERIFIED_AMOUNT TOT_INC_GST_CL
                FROM
                              [CPPA_CA].[WP_GC_ERP_INVOICES]
                WHERE

                            (VENDOR_SITE_ID_FK =  " + dataID[1] + @")
                            AND
                            Vendor_INV_no in ( SELECT IPP_INV_NO
                            FROM CPPA_CA.WP_GC_INV_DIFF_PARENT DIFF
                            INNER JOIN CPPA_CA.DIARY_HEADER_INTERFACE dhi ON DHI.DIARY_HEADER_ID = diff.DIARY_HEADER_ID_FK
                            INNER JOIN CPPA_CA.WP_GC_ERP_INVOICES ERP ON ERP.TRANSACTION_NO=DHI.TransactionNumber
                            WHERE  (ERP.PAYMENT_STATUS = 'Claim Returned' AND dhi.IS_DIFFERENTIAL = 'I') 
                            AND dhi.ORGANIZATION_ID =  " + dataID[1] + @"
                            )
                AND Vendor_INV_no not in (
									SELECT  v.VEN_INV_LETTER_NO
									FROM CPPA_CA.DIARY_HEADER_INTERFACE v
									WHERE ORGANIZATION_ID = 284070
									AND INVOICE_TYPES = 'CPP'
									AND CAST(SUBMIT_DATE AS DATE) <= '2023-12-28'
								)
                            ORDER BY VENDOR_INV_NO");
                        break;

                    case 13:
                        string UserID13 = Fn.ExenID(@"SELECT [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')");
                        Returnvls = Fn.Data2Json(@"EXEC GetDHIAllbyUserID " + UserID13);
                        break;

                    case 14:
                        try
                        {
                            Fn.Exec(
                                @"UPDATE CPPA_CA.DIARY_HEADER_INTERFACE SET APPROVED_STATUS='Deleted', LAST_UPDATE_DATE = CPPA_CA.GETDATEpk(), LAST_UPDATE_LOGIN= CDXP.FN_WP_PORTAL_USERS_ID_BY_USER_NAME('" +
                                Convert.ToString(Session["UserName"]) + @"') WHERE DIARY_HEADER_ID = " + dataID[1]);
                            Returnvls = dataID[1];
                        }
                        catch (Exception e)
                        {
                            Returnvls = "-7";
                        }

                        break;

                    case 15:
                        int UserID15 = Convert.ToInt32(Fn.ExenID(@"SELECT [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')"));
                        Returnvls = Fn.Data2Json("EXEC GetDHIbyStatusbyUserID " + Convert.ToString(UserID15));
                        break;
                    case 16:
                        Returnvls = Fn.Data2Json(@"
                                               SELECT        
	                                                CPPA_CA.tblInvoiceLog.tblInvoiceLogID, 
	                                                CASE WHEN CPPA_CA.tblInvoiceLog.UserId = 0 AND Action = 'Submitted' THEN 
		                                                [dbo].[GetIPPName](CPPA_CA.tblInvoiceLog.DIARY_HEADER_ID)
	                                                ELSE
		                                                CPPA_CA.ApiUsers.Name 
	                                                END AS Name,
	                                                ISNULL(CPPA_CA.ApiUsers.Designation,'') Designation,
	                                                CASE WHEN CPPA_CA.tblInvoiceLog.UserId = 0 AND Action = 'Submitted' THEN 
		                                                'IPP User'
	                                                ELSE	
		                                                [CPPA_CA_ApiUserType].Name 
	                                                END AS [User Type], 
	                                                FORMAT(CPPA_CA.tblInvoiceLog.dtDateTime, 'dd-MMM-yyyy', 'en-us') + ' ' + LEFT(CONVERT(VARCHAR(50), CPPA_CA.tblInvoiceLog.dtDateTime, 108), 5) AS [Date Time], 
	                                                CPPA_CA.tblInvoiceLog.Action
                                                FROM            
	                                                CPPA_CA.tblInvoiceLog 
	                                                LEFT OUTER JOIN CPPA_CA.ApiUsers ON CPPA_CA.tblInvoiceLog.UserId = CPPA_CA.ApiUsers.UserId 
	                                                LEFT OUTER JOIN [CPPA_CA_ApiUserType] ON CPPA_CA.ApiUsers.UserType = [CPPA_CA_ApiUserType].ApiUserTypeId
                                                WHERE        (CPPA_CA.tblInvoiceLog.DIARY_HEADER_ID = " + dataID[1] + @")
                                                ORDER BY dtDateTime DESC");
                        break;
                    case 17:
                        int UserID17 = Convert.ToInt32(Fn.ExenID(@"SELECT [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')"));
                        Returnvls = Fn.Data2Json(@"EXEC dbo.GetDHIbyCriteria " + UserID17 + ", '" + dataID[1] + "', '" + dataID[2] + "', '" + dataID[3] + "', '" + dataID[4] + "', '" + dataID[5] + "', '" + dataID[6] + "', '" + dataID[7] + "'");
                        break;
                    case 18:
                        try
                        {
                            DataSet ds2 = new DataSet();
                            DataTable dt2 = new DataTable();
                            ds2 = Fn.FillDSet(@"
                            select * from [dbo].[DISPUTE_ATTACHMENTS] where DISP_ATT_PK = " + dataID[1]);
                            dt2 = ds2.Tables[0];
                            if (dt2.Rows.Count > 0)
                            {
                                string DISP_ATT_PK = Convert.ToString(dt2.Rows[0]["DISP_ATT_PK"]);
                                string disp_nm = Convert.ToString(dt2.Rows[0]["File_Name"]);
                                // string disp_ftyp = Convert.ToString(dt2.Rows[0]["FILE_NAME_1"]);
                                string disp_remarks = Convert.ToString(dt2.Rows[0]["DISPUTE_REMARKS"]);

                                string ext = disp_nm.Split('.')[disp_nm.Split('.').Length - 1];
                                String path = HttpContext.Server.MapPath("/Content/_FilesNew"); //Path
                                string FilePath = Path.Combine(path, dataID[1] + "." + ext);
                                if (!System.IO.File.Exists(FilePath))
                                {
                                    DataSet ds21 = new DataSet();
                                    DataTable dt21 = new DataTable();
                                    ds21 = Fn.FillDSet("exec sp_GetDisputeFilesByID " + dataID[1]);
                                    dt21 = ds21.Tables[0];

                                    //if (dataID[2] == "1")
                                    //{
                                    byte[] byteArray = Convert.FromBase64String(Convert.ToString(dt21.Rows[0]["File_Data"]));
                                    string HDR2 = Convert.ToBase64String(byteArray);
                                    byte[] FileBytes = Convert.FromBase64String(HDR2);
                                    System.IO.File.WriteAllBytes(FilePath, FileBytes);
                                    //}

                                    //byte[] byteArray = Convert.FromBase64String(Convert.ToString(dt21.Rows[0]["FILE_DATA_1"]));
                                    //string HDR2 = Convert.ToBase64String(byteArray);
                                    //byte[] FileBytes = Convert.FromBase64String(HDR2);
                                    //System.IO.File.WriteAllBytes(FilePath, FileBytes);
                                }
                                //var obj2 = new { fdata = "", name = disp_nm };
                                var obj2 = new { fdata = "", name = disp_nm, ext = ext };
                                //Returnvls = new JavaScriptSerializer().Serialize(new { DHIHDR = HDR10, DTL = DTLs10, FList = flist10, AdjList = adjList10, UserDtl = new { uid = userobj10.UserId, utyp = userobj10.UserType, Name = userobj10.Name } });
                                Returnvls = new JavaScriptSerializer().Serialize(obj2);
                            }
                            else
                            {
                                Returnvls = "No data found for this id";
                            }

                        }
                        catch (Exception ex)
                        {

                            Returnvls = ex.Message;
                        }
                        break;

                    case 19:
                        Returnvls = Fn.Data2Json(@"SELECT 
	                HEADER_ID_PK = [CPPA_CA].[DIARY_HEADER_INTERFACE].DIARY_HEADER_ID
                   ,DOCUMENT_NO = [CPPA_CA].[DIARY_HEADER_INTERFACE].VEN_INV_LETTER_NO
                   ,DOCUMENT_DATE = [CPPA_CA].[DIARY_HEADER_INTERFACE].VEN_INV_LETTER_DATE
                   ,DOC_REC_ID_FK = 0
                         FROM [CPPA_CA].[DIARY_HEADER_INTERFACE]
                         INNER JOIN [CPPA_CA].[PPA_APPLICABLE_INVOICES]
	                      ON [CPPA_CA].[DIARY_HEADER_INTERFACE].INVOICE_TYPE_ID = [CPPA_CA].[PPA_APPLICABLE_INVOICES].APP_INVOICES_ID_PK
                          WHERE [CPPA_CA].[PPA_APPLICABLE_INVOICES].INVOICE_TYPES IN ('Commodity Charge Invoice')
                    AND [CPPA_CA].[DIARY_HEADER_INTERFACE].ORGANIZATION_ID = " + dataID[2] + @"
                    AND APPROVED_STATUS <> 'Deleted'
                    ORDER BY [CPPA_CA].[DIARY_HEADER_INTERFACE].VEN_INV_LETTER_DATE DESC");
                        break;
                    default:
                        Returnvls = "-1";
                        break;
                    
                }
            }
            catch (Exception ex)
            {

                return Convert.ToString(ex.Message);
                //return Request.CreateResponse(HttpStatusCode.OK, new { blockData = serializer.Serialize(rows), adjInv = ddlItems });
            }


            return Returnvls;
        }
        [HttpPost]
        public string IInvoice()
        {
            string strFormData = "0";
            string strBlockData = "0";
            string TransactionNumber = "";
            int UserID = Convert.ToInt32(Fn.ExenID(@"SELECT [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')"));
            //int UserID = 0;
            //if (Request.Headers.Contains("UserID"))
            //{
            //    var dssd = Request.Headers.GetValues("UserID").FirstOrDefault();
            //    UserID = dssd != null ? Convert.ToInt32(dssd) : 0;
            //}
            DateTime utc = DateTime.UtcNow;
            DateTime utc_nextday = DateTime.Parse(DateTime.UtcNow.Date.AddDays(1).ToShortDateString() + " 9:01AM");
            // bool nextDayDateRequired = false;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            DateTime pkdatetime_NextDay = DateTime.Parse(pkdatetime.AddDays(1).ToShortDateString() + " 9:00AM");

            try
            {
                //0   807½				            PPA HEADER ID
                //1   Sapphire Wind Power Company Limited½  
                //2   1035½
                //3   ½---Draft½
                //4   1353½				            Invoice Type
                //5   05-Sep-2018½  			    Invoice Due Date****
                //6   1111 Inv No½      		    Invoice / Letter No *****
                //7   0½				            Parent Invoice*****
                //8   02-Sep-2018½  			    Invoice / Letter Date ******
                //9   03-Sep-2018½          		Invoice Period From *******
                //10  00:00½                        Invoice Period From Time
                //11  04-Sep-2018           		Invoice Period To********
                //12  00:00½                        Invoice Period To Time
                //13  PKR½				            Currency********
                //14  0.0000½				        Total Claim********
                //15  66666 Remarks½			    Remarks
                //16  Draft½                        Button Value
                //17  77777 Notes½                  Lower Notes
                //18  0                             Create or Edit

                var FormData = HttpContext.Request.Form["strFormData"];
                //var FormData = "92½Jamshoro Power Company Limited-(Genco-1)½1019½Draft½1404½18-Jan-2019½123test½0½18-Jan-2019½18-Jan-2019½00:00½18-Jan-2019½00:00½PKR½13,463.0000½½Draft½½0";
                var BlockData = HttpContext.Request.Form["strBlockData"];
                //var BlockData = "7308½Post½JPCL/CPP/0118½CPP½01-Jan-2018½31-Jan-2018½331,204,858.0000½331,204,858.0000½13,463.0000½½0¡0½18-Jan-2019½12½1,555.0000½12.0000½13,463.0000½½»";
                var filekData = HttpContext.Request.Form["strfilekData"];
                var fileDelData = HttpContext.Request.Form["strfileDelData"];
                var dellblock = HttpContext.Request.Form["strdellblock"];
                var fileupdate = HttpContext.Request.Form["strfileupdate"];
                var adjData = HttpContext.Request.Form["stradjData"];
                var adjDelData = HttpContext.Request.Form["stradjDelData"];
                var delDetail = HttpContext.Request.Form["strdelDetail"];
                var diary = FormData.Split('½');
                decimal DIARY_HEADER_ID = Convert.ToDecimal(diary[18]);
                // INSERT CASE START
                if (DIARY_HEADER_ID == 0)
                {
                    using (TransactionScope tranScope = new TransactionScope())
                    {
                        using (DBDataContext db = new DBDataContext())
                        {
                            if (fileupdate.Length > 0)
                            {
                                foreach (var item in fileupdate.Split('¡'))
                                {
                                    var fl = db.ATTACHMENT_HEADERs.Where(v => v.ATTACH_HEADER_ID == Convert.ToDecimal(item.Split('½')[0])).FirstOrDefault();
                                    if (fl != null)
                                    {
                                        fl.ATTACHMENT_TITLE = item.Split('½')[1];
                                        fl.ATTACHMENT_DESC = item.Split('½')[2];
                                        db.SubmitChanges();
                                    }
                                }
                            }
                            var c = db.DIARY_HEADER_INTERFACEs.Where(vvv => vvv.VEN_INV_LETTER_NO == diary[6]).FirstOrDefault();
                            if (c != null)
                            {
                                return "-1";
                            }

                            //DHI Insert Starts

                            DIARY_HEADER_INTERFACE obj = new DIARY_HEADER_INTERFACE();
                            obj.CREATION_DATE = pkdatetime;
                            obj.PPA_HEADER_ID = Convert.ToInt32(diary[0]);
                            obj.ORGANIZATION_ID = Convert.ToInt64(diary[2]);
                            obj.INVOICE_TYPE_ID = Convert.ToDouble(diary[4]);
                            obj.INV_DUE_DATE = ToDateTime(diary[5]);
                            obj.VEN_INV_LETTER_NO = diary[6];
                            obj.PARENT_INVOICE_NO = 0;
                            obj.VEN_INV_LETTER_DATE = ToDateTime(diary[8]);
                            obj.INVOICE_PERIOD_FROM = ToDateTime(diary[9] + " " + diary[10]);
                            obj.INVOICE_PERIOD_TO = ToDateTime(diary[11] + " " + diary[12]);
                            obj.CURRENCY = diary[13];
                            obj.TOTAL_CLAIM = Convert.ToDecimal(diary[14]);
                            obj.REMARKS = diary[15];
                            obj.APPROVED_STATUS = diary[16];
                            obj.LAST_UPDATE_DATE = pkdatetime;
                            obj.LAST_UPDATE_LOGIN = Convert.ToString(UserID);
                            obj.IS_DIFFERENTIAL = "I";
                            obj.FORM_NAME = "InterestInvoice.aspx";
                            obj.IS_APPROVED_BY_TECHNICAL = "1";
                            obj.INTERFACE_STATUS = "Temp";
                            if (diary[16] == "Submitted")
                            {
                              

                                var fndlpH = db.FND_LOOKUP_VALUEs.Where(v => v.MEANING.ToUpper() == Convert.ToString(TowDigit(pkdatetime.Day)) + "-" + monthNames[pkdatetime.Month - 1].ToUpper() && v.LOOKUP_TYPE == "CPPA_GAZETTED_HOLIDAYS_LK" && v.ENABLED_FLAG == "Y").FirstOrDefault();
                                

                                if (fndlpH != null)
                                {
                                    return "-2";
                                }//end of if
                                else
                                {
                                   
                                    var fndlpT = db.FND_LOOKUP_VALUEs.Where(v => v.LOOKUP_CODE.ToUpper() == pkdatetime.ToString("ddd").ToUpper() && v.LOOKUP_TYPE == "CPPA_INVOICE_SUBMIT_TIME_LK").FirstOrDefault();
                                  
                                    if (fndlpT != null)
                                    {
                                        try
                                        {
                                            
                                            DateTime T1 = new DateTime(pkdatetime.Year, pkdatetime.Month, pkdatetime.Day, Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[0]), Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[1]), Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[2]));
                                            DateTime T2 = new DateTime(pkdatetime.Year, pkdatetime.Month, pkdatetime.Day, Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[0]), Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[1]), Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[2]));
                                            
                                            bool flg = (pkdatetime >= T1 && pkdatetime <= T2) ? true : false;
                                            if (!flg)
                                            {
                                                return "Invoice can only be submitted between " + fndlpT.ATTRIBUTE1.Split(':')[0] + ":" + fndlpT.ATTRIBUTE1.Split(':')[1] + " to " + fndlpT.ATTRIBUTE2.Split(':')[0] + ":" + fndlpT.ATTRIBUTE2.Split(':')[1];
                                            }
                                        }
                                        catch (Exception)
                                        {
                                          
                                            tranScope.Dispose();
                                            return "-3";
                                        }

                                    }
                                    else
                                    {
                                        tranScope.Dispose();
                                        return "-3";

                                    }
                                }//end of else  

                                obj.SUBMIT_DATE = pkdatetime;

                                var sno = obj.TransactionNumber;
                                if (sno == null)
                                {

                                    //if (mx == null || mx == 0)
                                    //{
                                    //    obj.TransactionNumber = 1;
                                    //    strFormData = "1";
                                    //}
                                    //else
                                    //{
                                    //    obj.TransactionNumber = mx + 1;
                                    //    strFormData = Convert.ToString(mx + 1);
                                    //}
                                    if (obj.IS_APPROVED_BY_TECHNICAL == "1" && obj.IS_APPROVED_BY_FINANCE == null)
                                    {
                                        long? mx = db.DIARY_HEADER_INTERFACEs.Max(z => z.TransactionNumber);
                                        if (mx == null || mx == 0)
                                        {
                                            obj.TransactionNumber = 1;
                                            strFormData = "1";
                                            TransactionNumber = "1";
                                        }
                                        else
                                        {
                                            obj.TransactionNumber = mx + 1;
                                            strFormData = Convert.ToString(mx + 1);
                                            TransactionNumber = strFormData;
                                        }
                                    }
                                    else
                                    {
                                        strFormData = Convert.ToString(obj.TransactionNumber);
                                        TransactionNumber = strFormData;
                                        obj.IS_APPROVED_BY_TECHNICAL = "1";
                                        obj.IS_APPROVED_BY_FINANCE = null;
                                    }
                                }
                                else
                                {
                                    strFormData = Convert.ToString(sno);
                                    TransactionNumber = strFormData;
                                }
                                obj.APPROVED_STATUS = diary[16];



                            }

                            db.DIARY_HEADER_INTERFACEs.InsertOnSubmit(obj);
                            db.SubmitChanges();

                            //DHI Insert Ends
                            DIARY_HEADER_ID = obj.DIARY_HEADER_ID;
                            foreach (string blk in BlockData.Split('»'))
                            {
                                if (blk.Length > 0)
                                {
                                    decimal DIFF_PAR_ID_PK = 0;
                                    for (int i = 0; i < blk.Split('¡').Length; i++)
                                    {
                                        if (i == 0)
                                        {
                                            try
                                            {
                                                
                                                WP_GC_INV_DIFF_PARENT parent = new WP_GC_INV_DIFF_PARENT();
                                                parent.PARENT_INV_ID_FK = Convert.ToDecimal(blk.Split('¡')[i].Split('½')[0]);
                                                parent.PRE_POST = blk.Split('¡')[i].Split('½')[1];
                                                parent.IPP_INV_NO = blk.Split('¡')[i].Split('½')[2];
                                                parent.INVOICE_TYPE = blk.Split('¡')[i].Split('½')[3];
                                                if (Convert.ToString(blk.Split('¡')[i].Split('½')[4]).Trim() == "")
                                                {
                                                    parent.PAR_INV_PER_FRM = null;
                                                }
                                                else
                                                {
                                                    parent.PAR_INV_PER_FRM = ToDateTime(blk.Split('¡')[i].Split('½')[4]);
                                                }
                                                if (Convert.ToString(blk.Split('¡')[i].Split('½')[5]).Trim() == "")
                                                {
                                                    parent.PAR_INV_PER_TO = null;
                                                }
                                                else
                                                {
                                                    parent.PAR_INV_PER_TO = ToDateTime(blk.Split('¡')[i].Split('½')[5]);
                                                }

                                                if (!string.IsNullOrEmpty(blk.Split('¡')[i].Split('½')[6]) && blk.Split('¡')[i].Split('½')[6] != "undefined")
                                                {

                                                }

                                                //parent.PAR_INV_PER_FRM = ToDateTime(blk.Split('¡')[i].Split('½')[4]);
                                                //parent.PAR_INV_PER_TO = ToDateTime(blk.Split('¡')[i].Split('½')[5]);
                                                parent.CLAIM_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[6]);
                                                parent.PAR_INV_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[7]);
                                                parent.CURRENT_CLAIM = Convert.ToDouble(blk.Split('¡')[i].Split('½')[8]);
                                                parent.REMARKS = blk.Split('¡')[i].Split('½')[9];
                                                parent.DIARY_HEADER_ID_FK = DIARY_HEADER_ID;
                                                db.WP_GC_INV_DIFF_PARENTs.InsertOnSubmit(parent);
                                                
                                                db.SubmitChanges();
                                                
                                                DIFF_PAR_ID_PK = parent.DIFF_PAR_ID_PK;
                                            }
                                            catch (Exception ex)
                                            {
                                               
                                                tranScope.Dispose();
                                                strFormData = ex.Message;
                                               
                                            }

                                        }
                                        else
                                        {
                                           
                                            WP_GC_INTEREST_DETAIL dtl = new WP_GC_INTEREST_DETAIL();
                                            dtl.PAYMENT_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[1]);
                                            dtl.NO_OF_DAYS = Convert.ToInt32(blk.Split('¡')[i].Split('½')[2]);
                                            dtl.AMOUNT_PAID = Convert.ToDouble(blk.Split('¡')[i].Split('½')[3]);
                                            dtl.INTEREST_RATE = Convert.ToDouble(blk.Split('¡')[i].Split('½')[4]);
                                            dtl.INTEREST_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[5]);
                                            if (!string.IsNullOrEmpty(blk.Split('¡')[i].Split('½')[6]) && blk.Split('¡')[i].Split('½')[6] != "undefined")
                                            {
                                                dtl.FROM_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[6]);
                                            }
                                            else
                                            {
                                                dtl.FROM_DATE = null;
                                            }

                                            if (!string.IsNullOrEmpty(blk.Split('¡')[i].Split('½')[7]))
                                            {
                                                dtl.End_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[7]);
                                            }
                                            else
                                            {
                                                dtl.End_DATE = null;
                                            }

                                            dtl.HEADER_ID_FK = DIARY_HEADER_ID;
                                            dtl.DIFF_PAR_ID_FK = DIFF_PAR_ID_PK;
                                            if (dtl.DIFF_PAR_ID_FK != 0 && dtl.DIFF_PAR_ID_FK != null) // Parent (Block) with id = 0 shall not pass  OSAMa
                                            {
                                                db.WP_GC_INTEREST_DETAILs.InsertOnSubmit(dtl);
                                               
                                                db.SubmitChanges();
                                                
                                            }

                                        }
                                    }//END OF FOR LOOP
                                }
                            }

                            foreach (string item in filekData.Split('¡'))
                            {
                                if (item.Contains("½"))
                                {
                                    decimal? AttachId = 0;
                                  
                                    db.sp_Upload_FileNew(DIARY_HEADER_ID, item.Split('½')[1], item.Split('½')[3], item.Split('½')[4], item.Split('½')[2], item.Split('½')[1],
                                   item.Split('½')[5], pkdatetime, Convert.ToString(UserID), ref AttachId);
                                    //try
                                    //{
                                    //    string nm = item.Split('½')[2];
                                    //    string ext = nm.Split('.')[nm.Split('.').Length - 1];
                                    //    String path = HttpContext.Server.MapPath("~/Content/_FilesNew"); //Maintain Path for record purpose
                                    //    string FilePath = Path.Combine(path, Convert.ToString(AttachId) + "." + ext);
                                    //    if (!System.IO.File.Exists(FilePath))
                                    //    {
                                            
                                    //        byte[] FileBytes = Convert.FromBase64String(item.Split('½')[5]);
                                    //        System.IO.File.WriteAllBytes(FilePath, FileBytes);
                                           
                                    //    }
                                    //}
                                    //catch(Exception ex)
                                    //{
                                       
                                    //    tranScope.Dispose();
                                       
                                    //}
                                }

                            }

                            if (dellblock != "")
                            {
                                List<WP_GC_INV_DIFF_PARENT> oList = new List<WP_GC_INV_DIFF_PARENT>();
                                List<WP_GC_INTEREST_DETAIL> dtList = new List<WP_GC_INTEREST_DETAIL>();
                                foreach (string item in dellblock.Split('½'))
                                {
                                   
                                    WP_GC_INV_DIFF_PARENT gc = db.WP_GC_INV_DIFF_PARENTs.Where(v => v.DIFF_PAR_ID_PK == Convert.ToDecimal(item)).FirstOrDefault();
                                    if (gc != null)
                                    {
                                        oList.Add(gc);
                                    }
                                    List<WP_GC_INTEREST_DETAIL> dtl = db.WP_GC_INTEREST_DETAILs.Where(v => v.DIFF_PAR_ID_FK == gc.DIFF_PAR_ID_PK).ToList<WP_GC_INTEREST_DETAIL>();
                                    foreach (var itms in dtl)
                                    {
                                        dtList.Add(itms);
                                    }
                                }
                                
                                db.WP_GC_INV_DIFF_PARENTs.DeleteAllOnSubmit(oList);
                                db.WP_GC_INTEREST_DETAILs.DeleteAllOnSubmit(dtList);
                                db.SubmitChanges();
                                
                            }
                            if (adjDelData.Length > 0)
                            {
                                List<WP_GC_ADJUSTMENT> adl = new List<WP_GC_ADJUSTMENT>();
                                foreach (string item in adjDelData.Split('½'))
                                {
                                    var a = db.WP_GC_ADJUSTMENTs.Where(v => v.ADJ_HEADER_ID == Convert.ToDecimal(item)).FirstOrDefault();
                                    if (a != null)
                                    {
                                        adl.Add(a);
                                    }
                                }
                                if (adl.Count > 0)
                                {
                                   
                                    db.WP_GC_ADJUSTMENTs.DeleteAllOnSubmit(adl);
                                    db.SubmitChanges();
                                    
                                }
                            }
                            if (adjData.Length > 0)
                            {
                                foreach (string item in adjData.Split('¼'))
                                {
                                    if (item.Split('½')[0] == "0")
                                    {
                                        WP_GC_ADJUSTMENT oadj = new WP_GC_ADJUSTMENT();
                                        oadj.DIARY_HEADER_ID = DIARY_HEADER_ID;
                                        oadj.REFERENCE_INVOICE_NUMBER = item.Split('½')[1];
                                        oadj.ADJ_TYPES = item.Split('½')[2];
                                        oadj.ADJ_AMOUNT = Convert.ToDouble(item.Split('½')[3]);
                                        oadj.REMARKS = item.Split('½')[4];
                                        db.WP_GC_ADJUSTMENTs.InsertOnSubmit(oadj);
                                        db.SubmitChanges();
                                    }
                                    else
                                    {
                                        var objdup = db.WP_GC_ADJUSTMENTs.Where(w => w.ADJ_HEADER_ID == Convert.ToDecimal(item.Split('½')[0])).FirstOrDefault();
                                        if (objdup != null)
                                        {
                                            objdup.REFERENCE_INVOICE_NUMBER = item.Split('½')[1];
                                            objdup.ADJ_TYPES = item.Split('½')[2];
                                            objdup.ADJ_AMOUNT = Convert.ToDouble(item.Split('½')[3]);
                                            objdup.REMARKS = item.Split('½')[4];
                                            db.SubmitChanges();
                                        }
                                    }
                                }
                            }
                           
                            tranScope.Complete();
                            

                            if (TransactionNumber != "")
                            {
                                string Mailresult = "";
                                Task.Factory.StartNew<string>(() =>
                                SendMail(UserID, "Invoice Submitted", TransactionNumber))
                        .ContinueWith(ant => Mailresult = ant.Result,
                                      TaskScheduler.FromCurrentSynchronizationContext());
                            }

                        }//END OF USING
                    }//END OF TRANSACTION
                }
                // INSERT CASE ENDS


                // UPDATE CASE START
                else
                {
                    using (DBDataContext db = new DBDataContext())
                    {
                        var cdx = db.DIARY_HEADER_INTERFACEs.Where(vvv => vvv.VEN_INV_LETTER_NO == diary[6] && vvv.DIARY_HEADER_ID != DIARY_HEADER_ID).FirstOrDefault();
                        if (cdx != null)
                        {
                            return "-1";
                            // return "Invoice number " + diary[6] + " already exists..!";
                        }
                        if (delDetail.Length > 0)
                        {
                            foreach (var item in delDetail.Split('½'))
                            {
                                if (item != "")
                                {
                                    var com = db.WP_GC_INTEREST_DETAILs.Where(x => x.INT_DET_ID_PK == Convert.ToDecimal(item)).FirstOrDefault();
                                    if (com != null)
                                    {
                                        db.WP_GC_INTEREST_DETAILs.DeleteOnSubmit(com);
                                        db.SubmitChanges();
                                    }
                                }
                            }
                        }

                        if (fileupdate.Length > 0)
                        {
                            foreach (var item in fileupdate.Split('¡'))
                            {
                                var fl = db.ATTACHMENT_HEADERs.Where(v => v.ATTACH_HEADER_ID == Convert.ToDecimal(item.Split('½')[0])).FirstOrDefault();
                                if (fl != null)
                                {
                                    fl.ATTACHMENT_TITLE = item.Split('½')[1];
                                    fl.ATTACHMENT_DESC = item.Split('½')[2];
                                    db.SubmitChanges();
                                }
                            }
                        }
                        // DIARY_HEADER_INTERFACE obj = new DIARY_HEADER_INTERFACE();
                        DIARY_HEADER_INTERFACE obj = db.DIARY_HEADER_INTERFACEs.Where(vd => vd.DIARY_HEADER_ID == Convert.ToDecimal(DIARY_HEADER_ID)).FirstOrDefault();
                        if (obj != null)
                        {
                            //obj.CREATION_DATE = pkdatetime;
                            obj.PPA_HEADER_ID = Convert.ToInt32(diary[0]);
                            obj.ORGANIZATION_ID = Convert.ToInt64(diary[2]);
                            obj.INVOICE_TYPE_ID = Convert.ToDouble(diary[4]);
                            obj.INV_DUE_DATE = ToDateTime(diary[5]);
                            obj.VEN_INV_LETTER_NO = diary[6];
                            obj.PARENT_INVOICE_NO = 0;
                            obj.VEN_INV_LETTER_DATE = ToDateTime(diary[8]);
                            obj.INVOICE_PERIOD_FROM = ToDateTime(diary[9] + " " + diary[10]);
                            obj.INVOICE_PERIOD_TO = ToDateTime(diary[11] + " " + diary[12]);
                            obj.CURRENCY = diary[13];
                            obj.TOTAL_CLAIM = Convert.ToDecimal(diary[14]);
                            obj.REMARKS = diary[15];
                            obj.APPROVED_STATUS = diary[16];
                            obj.LAST_UPDATE_DATE = pkdatetime;
                            obj.LAST_UPDATE_LOGIN = Convert.ToString(UserID);
                            obj.IS_DIFFERENTIAL = "I";
                            obj.FORM_NAME = "InterestInvoice.aspx";
                            obj.IS_APPROVED_BY_TECHNICAL = "1";
                            obj.INTERFACE_STATUS = "Temp";
                            if (diary[16] == "Submitted")
                            {
                                var fndlpH = db.FND_LOOKUP_VALUEs.Where(v => v.MEANING.ToUpper() == Convert.ToString(TowDigit(pkdatetime.Day)) + "-" + monthNames[pkdatetime.Month - 1].ToUpper() && v.LOOKUP_TYPE == "CPPA_GAZETTED_HOLIDAYS_LK" && v.ENABLED_FLAG == "Y").FirstOrDefault();
                                if (fndlpH != null)
                                {
                                    return "-2";
                                }//end of if
                                else
                                {
                                    var fndlpT = db.FND_LOOKUP_VALUEs.Where(v => v.LOOKUP_CODE.ToUpper() == pkdatetime.ToString("ddd").ToUpper() && v.LOOKUP_TYPE == "CPPA_INVOICE_SUBMIT_TIME_LK").FirstOrDefault();
                                    if (fndlpT != null)
                                    {
                                        try
                                        {
                                            DateTime T1 = new DateTime(pkdatetime.Year, pkdatetime.Month, pkdatetime.Day, Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[0]), Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[1]), Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[2]));
                                            DateTime T2 = new DateTime(pkdatetime.Year, pkdatetime.Month, pkdatetime.Day, Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[0]), Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[1]), Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[2]));

                                            bool flg = (pkdatetime >= T1 && pkdatetime <= T2) ? true : false;
                                            if (!flg)
                                            {
                                                return "Invoice can only be submitted between " + fndlpT.ATTRIBUTE1.Split(':')[0] + ":" + fndlpT.ATTRIBUTE1.Split(':')[1] + " to " + fndlpT.ATTRIBUTE2.Split(':')[0] + ":" + fndlpT.ATTRIBUTE2.Split(':')[1];
                                            }
                                        }
                                        catch (Exception)
                                        {

                                            return "-3";
                                           
                                        }

                                    }
                                    else
                                    {
                                        return "-3";
                                    }
                                }//end of else  
                                 //obj.SUBMIT_DATE = pkdatetime;
                                 //long? mx = db.DIARY_HEADER_INTERFACEs.Max(z => z.TransactionNumber);

                                //if (obj.IS_APPROVED_BY_TECHNICAL == null && obj.IS_APPROVED_BY_FINANCE == null)
                                //{
                                //    if (mx == null || mx == 0)
                                //    {
                                //        obj.TransactionNumber = 1;
                                //        strFormData = "1";
                                //    }
                                //    else
                                //    {
                                //        obj.TransactionNumber = mx + 1;
                                //        strFormData = Convert.ToString(mx + 1);
                                //    }
                                //}
                                //else
                                //{
                                //    strFormData = Convert.ToString(obj.TransactionNumber);
                                //}
                                obj.SUBMIT_DATE = pkdatetime;
                                var sno = obj.TransactionNumber;
                                if (sno == null)
                                {

                                    //if (mx == null || mx == 0)
                                    //{
                                    //    obj.TransactionNumber = 1;
                                    //    strFormData = "1";
                                    //}
                                    //else
                                    //{
                                    //    obj.TransactionNumber = mx + 1;
                                    //    strFormData = Convert.ToString(mx + 1);
                                    //}
                                    if (obj.IS_APPROVED_BY_TECHNICAL == "1" && obj.IS_APPROVED_BY_FINANCE == null)
                                    {
                                        long? mx = db.DIARY_HEADER_INTERFACEs.Max(z => z.TransactionNumber);
                                        if (mx == null || mx == 0)
                                        {
                                            obj.TransactionNumber = 1;
                                            strFormData = "1";
                                            TransactionNumber = "1";
                                        }
                                        else
                                        {
                                            obj.TransactionNumber = mx + 1;
                                            strFormData = Convert.ToString(mx + 1);
                                            TransactionNumber = strFormData;
                                        }
                                    }
                                    else
                                    {
                                        strFormData = Convert.ToString(obj.TransactionNumber);
                                        TransactionNumber = strFormData;
                                        obj.IS_APPROVED_BY_TECHNICAL = "1";
                                        obj.IS_APPROVED_BY_FINANCE = null;
                                    }
                                }
                                else
                                {
                                    strFormData = Convert.ToString(sno);
                                    TransactionNumber = strFormData;
                                }
                                obj.APPROVED_STATUS = diary[16];
                            }

                            //db.DIARY_HEADER_INTERFACEs.InsertOnSubmit(obj);
                            db.SubmitChanges();
                            //DIARY_HEADER_ID = obj.DIARY_HEADER_ID;

                        }

                        foreach (string blk in BlockData.Split('»'))
                        {
                            if (blk.Length > 0)
                            {
                                decimal DIFF_PAR_ID_PK = 0;
                                //INSERT
                                if (blk.Split('¡')[0].Split('½')[10] == "0")
                                {
                                    for (int i = 0; i < blk.Split('¡').Length; i++)
                                    {
                                        if (i == 0)
                                        {
                                            try
                                            {
                                                WP_GC_INV_DIFF_PARENT parent = new WP_GC_INV_DIFF_PARENT();
                                                parent.PARENT_INV_ID_FK = Convert.ToDecimal(blk.Split('¡')[i].Split('½')[0]);
                                                parent.PRE_POST = blk.Split('¡')[i].Split('½')[1];
                                                parent.IPP_INV_NO = blk.Split('¡')[i].Split('½')[2];
                                                parent.INVOICE_TYPE = blk.Split('¡')[i].Split('½')[3];
                                                if (Convert.ToString(blk.Split('¡')[i].Split('½')[4]).Trim() == "")
                                                {
                                                    parent.PAR_INV_PER_FRM = null;
                                                }
                                                else
                                                {
                                                    parent.PAR_INV_PER_FRM = ToDateTime(blk.Split('¡')[i].Split('½')[4]);
                                                }
                                                if (Convert.ToString(blk.Split('¡')[i].Split('½')[5]).Trim() == "")
                                                {
                                                    parent.PAR_INV_PER_TO = null;
                                                }
                                                else
                                                {
                                                    parent.PAR_INV_PER_TO = ToDateTime(blk.Split('¡')[i].Split('½')[5]);
                                                }
                                                //parent.PAR_INV_PER_FRM = ToDateTime(blk.Split('¡')[i].Split('½')[4]);
                                                //parent.PAR_INV_PER_TO = ToDateTime(blk.Split('¡')[i].Split('½')[5]);
                                                parent.CLAIM_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[6]);
                                                parent.PAR_INV_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[7]);
                                                parent.CURRENT_CLAIM = Convert.ToDouble(blk.Split('¡')[i].Split('½')[8]);
                                                parent.REMARKS = blk.Split('¡')[i].Split('½')[9];
                                                parent.DIARY_HEADER_ID_FK = DIARY_HEADER_ID;
                                                db.WP_GC_INV_DIFF_PARENTs.InsertOnSubmit(parent);
                                                db.SubmitChanges();
                                                DIFF_PAR_ID_PK = parent.DIFF_PAR_ID_PK;
                                            }
                                            catch (Exception ex)
                                            {

                                                strFormData = ex.Message;
                                            }

                                        }
                                        else
                                        {
                                            WP_GC_INTEREST_DETAIL dtl = new WP_GC_INTEREST_DETAIL();
                                            dtl.PAYMENT_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[1]);
                                            dtl.NO_OF_DAYS = Convert.ToInt32(blk.Split('¡')[i].Split('½')[2]);
                                            dtl.AMOUNT_PAID = Convert.ToDouble(blk.Split('¡')[i].Split('½')[3]);
                                            dtl.INTEREST_RATE = Convert.ToDouble(blk.Split('¡')[i].Split('½')[4]);
                                            dtl.INTEREST_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[5]);
                                            if (!string.IsNullOrEmpty(blk.Split('¡')[i].Split('½')[6]) && blk.Split('¡')[i].Split('½')[6] != "undefined")
                                            {
                                                dtl.FROM_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[6]);
                                            }
                                            else
                                            {
                                                dtl.FROM_DATE = null;
                                            }

                                            if (!string.IsNullOrEmpty(blk.Split('¡')[i].Split('½')[7]))
                                            {
                                                dtl.End_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[7]);
                                            }
                                            else
                                            {
                                                dtl.End_DATE = null;
                                            }
                                            dtl.HEADER_ID_FK = DIARY_HEADER_ID;
                                            dtl.DIFF_PAR_ID_FK = DIFF_PAR_ID_PK;
                                            if (dtl.DIFF_PAR_ID_FK != 0 && dtl.DIFF_PAR_ID_FK != null) // Parent (Block) with id = 0 shall not pass Osama
                                            {
                                                db.WP_GC_INTEREST_DETAILs.InsertOnSubmit(dtl);
                                                db.SubmitChanges();
                                            }

                                        }
                                    }//END OF FOR LOOP
                                }
                                //INSERT Blocks and Components in UPDATE CASE Ends 

                                //UPDATE Blocks and Components in UPDATE CASE Starts
                                else
                                {
                                    for (int i = 0; i < blk.Split('¡').Length; i++)
                                    {

                                        if (i == 0)
                                        {
                                            try
                                            {

                                                WP_GC_INV_DIFF_PARENT parent = db.WP_GC_INV_DIFF_PARENTs.Where(p => p.DIFF_PAR_ID_PK == Convert.ToDecimal(blk.Split('¡')[i].Split('½')[10])).FirstOrDefault();
                                                if (parent != null)
                                                {
                                                    parent.PARENT_INV_ID_FK = Convert.ToDecimal(blk.Split('¡')[i].Split('½')[0]);
                                                    parent.PRE_POST = blk.Split('¡')[i].Split('½')[1];
                                                    parent.IPP_INV_NO = blk.Split('¡')[i].Split('½')[2];
                                                    parent.INVOICE_TYPE = blk.Split('¡')[i].Split('½')[3];
                                                    if (Convert.ToString(blk.Split('¡')[i].Split('½')[4]).Trim() == "")
                                                    {
                                                        parent.PAR_INV_PER_FRM = null;
                                                    }
                                                    else
                                                    {
                                                        parent.PAR_INV_PER_FRM = ToDateTime(blk.Split('¡')[i].Split('½')[4]);
                                                    }
                                                    if (Convert.ToString(blk.Split('¡')[i].Split('½')[5]).Trim() == "")
                                                    {
                                                        parent.PAR_INV_PER_TO = null;
                                                    }
                                                    else
                                                    {
                                                        parent.PAR_INV_PER_TO = ToDateTime(blk.Split('¡')[i].Split('½')[5]);
                                                    }
                                                    //parent.PAR_INV_PER_FRM = ToDateTime(blk.Split('¡')[i].Split('½')[4]);
                                                    //parent.PAR_INV_PER_TO = ToDateTime(blk.Split('¡')[i].Split('½')[5]);
                                                    parent.CLAIM_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[6]);
                                                    parent.PAR_INV_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[7]);
                                                    parent.CURRENT_CLAIM = Convert.ToDouble(blk.Split('¡')[i].Split('½')[8]);
                                                    parent.REMARKS = blk.Split('¡')[i].Split('½')[9];
                                                    parent.DIARY_HEADER_ID_FK = DIARY_HEADER_ID;
                                                    //db.WP_GC_INV_DIFF_PARENTs.InsertOnSubmit(parent);
                                                    db.SubmitChanges();
                                                    DIFF_PAR_ID_PK = parent.DIFF_PAR_ID_PK;
                                                }

                                            }
                                            catch (Exception ex)
                                            {

                                                return ex.Message;
                                            }

                                        }
                                        else
                                        {

                                            //WP_GC_INTEREST_DETAIL dtl = new WP_GC_INTEREST_DETAIL();
                                            WP_GC_INTEREST_DETAIL dtl = db.WP_GC_INTEREST_DETAILs.Where(d => d.INT_DET_ID_PK == Convert.ToDecimal(blk.Split('¡')[i].Split('½')[0])).FirstOrDefault();
                                            if (dtl != null)
                                            {
                                                dtl.PAYMENT_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[1]);
                                                dtl.NO_OF_DAYS = Convert.ToInt32(blk.Split('¡')[i].Split('½')[2]);
                                                dtl.AMOUNT_PAID = Convert.ToDouble(blk.Split('¡')[i].Split('½')[3]);
                                                dtl.INTEREST_RATE = Convert.ToDouble(blk.Split('¡')[i].Split('½')[4]);
                                                dtl.INTEREST_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[5]);

                                                if (!string.IsNullOrEmpty(blk.Split('¡')[i].Split('½')[6]) && blk.Split('¡')[i].Split('½')[6] != "undefined")
                                                {
                                                    dtl.FROM_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[6]);
                                                }
                                                else
                                                {
                                                    dtl.FROM_DATE = null;
                                                }

                                                if (!string.IsNullOrEmpty(blk.Split('¡')[i].Split('½')[7]))
                                                {
                                                    dtl.End_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[7]);
                                                }
                                                else
                                                {
                                                    dtl.End_DATE = null;
                                                }
                                                dtl.HEADER_ID_FK = DIARY_HEADER_ID;
                                                //dtl.DIFF_PAR_ID_FK = DIFF_PAR_ID_PK;
                                                //db.WP_GC_INTEREST_DETAILs.InsertOnSubmit(dtl);
                                                db.SubmitChanges();
                                            }
                                            else
                                            {
                                                WP_GC_INTEREST_DETAIL dtlNew = new WP_GC_INTEREST_DETAIL();

                                                var lCompData = blk.Split('¡')[i].Split('½');


                                                dtlNew.PAYMENT_DATE = ToDateTime(lCompData[1]); // PAYMENT DATE 
                                                dtlNew.NO_OF_DAYS = Convert.ToInt32(lCompData[2]); // No of Days Delay  
                                                dtlNew.AMOUNT_PAID = Convert.ToDouble(lCompData[3]); //Amount Paid 
                                                dtlNew.INTEREST_RATE = Convert.ToDouble(lCompData[4]); // Interest Rate % 
                                                dtlNew.INTEREST_AMOUNT = Convert.ToDouble(lCompData[5]); //Interest Amount 
                                                if (!string.IsNullOrEmpty(lCompData[6]))
                                                {
                                                    dtlNew.FROM_DATE = ToDateTime(lCompData[6]); // Interest Rate From
                                                }
                                                else
                                                {
                                                    dtlNew.FROM_DATE = null;
                                                }

                                                if (!string.IsNullOrEmpty(lCompData[7]))
                                                {
                                                    dtlNew.End_DATE = ToDateTime(lCompData[7]); // Interest Rate To
                                                }
                                                else
                                                {
                                                    dtlNew.End_DATE = null;
                                                }

                                                dtlNew.HEADER_ID_FK = DIARY_HEADER_ID;
                                                dtlNew.DIFF_PAR_ID_FK = DIFF_PAR_ID_PK;
                                                if (dtlNew.DIFF_PAR_ID_FK != 0 && dtlNew.DIFF_PAR_ID_FK != null) // Parent (Block) with id = 0 shall not pass Osama
                                                {
                                                    db.WP_GC_INTEREST_DETAILs.InsertOnSubmit(dtlNew);
                                                    db.SubmitChanges();
                                                }
                                            }

                                        }

                                    }//END OF FOR LOOP
                                }

                                //UPDATE Blocks and Components in UPDATE CASE Ends

                            }
                        }


                        if (fileDelData.Trim().Length > 0)
                        {
                            fileDelData = fileDelData.Replace("¼", "");

                            foreach (var item in fileDelData.Split('½'))
                            {


                                try
                                {
                                    var f = db.ATTACHMENT_HEADERs.Where(v => v.ATTACH_HEADER_ID == Convert.ToDecimal(item)).FirstOrDefault();
                                    if (f != null)
                                    {
                                        f.bisDeleted = true;
                                        db.SubmitChanges();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    strFormData = ex.Message;

                                }
                            }
                        }

                        if (dellblock != "")
                        {
                            List<WP_GC_INV_DIFF_PARENT> oList = new List<WP_GC_INV_DIFF_PARENT>();
                            List<WP_GC_INTEREST_DETAIL> dtList = new List<WP_GC_INTEREST_DETAIL>();
                            foreach (string item in dellblock.Split('½'))
                            {
                                WP_GC_INV_DIFF_PARENT gc = db.WP_GC_INV_DIFF_PARENTs.Where(v => v.DIFF_PAR_ID_PK == Convert.ToDecimal(item)).FirstOrDefault();
                                if (gc != null)
                                {
                                    oList.Add(gc);
                                }
                                List<WP_GC_INTEREST_DETAIL> dtl = db.WP_GC_INTEREST_DETAILs.Where(v => v.DIFF_PAR_ID_FK == gc.DIFF_PAR_ID_PK).ToList<WP_GC_INTEREST_DETAIL>();
                                foreach (var itms in dtl)
                                {
                                    dtList.Add(itms);
                                }
                            }
                            db.WP_GC_INV_DIFF_PARENTs.DeleteAllOnSubmit(oList);
                            db.WP_GC_INTEREST_DETAILs.DeleteAllOnSubmit(dtList);
                            db.SubmitChanges();
                        }

                        foreach (string item in filekData.Split('¡'))
                        {
                            if (item.Contains("½"))
                            {
                                decimal? AttachId = 0;
                                db.sp_Upload_FileNew(DIARY_HEADER_ID, item.Split('½')[1], item.Split('½')[3], item.Split('½')[4], item.Split('½')[2], item.Split('½')[1],
                               item.Split('½')[5], pkdatetime, Convert.ToString(UserID), ref AttachId);
                                //try
                                //{
                                //    string nm = item.Split('½')[2];
                                //    string ext = nm.Split('.')[nm.Split('.').Length - 1];
                                //    String path = HttpContext.Server.MapPath("~/Content/_FilesNew"); //Maintain Path for record purpose
                                //    string FilePath = Path.Combine(path, Convert.ToString(AttachId) + "." + ext);
                                //    if (!System.IO.File.Exists(FilePath))
                                //    {
                                //        byte[] FileBytes = Convert.FromBase64String(item.Split('½')[5]);
                                //        System.IO.File.WriteAllBytes(FilePath, FileBytes);
                                //    }
                                //}
                                //catch
                                //{


                                //}
                            }

                        }

                        if (adjDelData.Length > 0)
                        {
                            List<WP_GC_ADJUSTMENT> adl = new List<WP_GC_ADJUSTMENT>();
                            foreach (string item in adjDelData.Split('½'))
                            {
                                var a = db.WP_GC_ADJUSTMENTs.Where(v => v.ADJ_HEADER_ID == Convert.ToDecimal(item)).FirstOrDefault();
                                if (a != null)
                                {
                                    adl.Add(a);
                                }
                            }
                            if (adl.Count > 0)
                            {
                                db.WP_GC_ADJUSTMENTs.DeleteAllOnSubmit(adl);
                                db.SubmitChanges();
                            }
                        }

                        if (adjDelData.Length > 0)
                        {
                            List<WP_GC_ADJUSTMENT> adl = new List<WP_GC_ADJUSTMENT>();
                            foreach (string item in adjDelData.Split('½'))
                            {
                                var a = db.WP_GC_ADJUSTMENTs.Where(v => v.ADJ_HEADER_ID == Convert.ToDecimal(item)).FirstOrDefault();
                                if (a != null)
                                {
                                    adl.Add(a);
                                }
                            }
                            if (adl.Count > 0)
                            {
                                db.WP_GC_ADJUSTMENTs.DeleteAllOnSubmit(adl);
                                db.SubmitChanges();
                            }
                        }
                        if (adjData.Length > 0)
                        {
                            foreach (string item in adjData.Split('¼'))
                            {
                                if (item.Split('½')[0] == "0")
                                {
                                    WP_GC_ADJUSTMENT oadj = new WP_GC_ADJUSTMENT();
                                    oadj.DIARY_HEADER_ID = DIARY_HEADER_ID;
                                    oadj.REFERENCE_INVOICE_NUMBER = item.Split('½')[1];
                                    oadj.ADJ_TYPES = item.Split('½')[2];
                                    oadj.ADJ_AMOUNT = Convert.ToDouble(item.Split('½')[3]);
                                    oadj.REMARKS = item.Split('½')[4];
                                    db.WP_GC_ADJUSTMENTs.InsertOnSubmit(oadj);
                                    db.SubmitChanges();
                                }
                                else
                                {
                                    var objdup = db.WP_GC_ADJUSTMENTs.Where(w => w.ADJ_HEADER_ID == Convert.ToDecimal(item.Split('½')[0])).FirstOrDefault();
                                    if (objdup != null)
                                    {
                                        objdup.REFERENCE_INVOICE_NUMBER = item.Split('½')[1];
                                        objdup.ADJ_TYPES = item.Split('½')[2];
                                        objdup.ADJ_AMOUNT = Convert.ToDouble(item.Split('½')[3]);
                                        objdup.REMARKS = item.Split('½')[4];
                                        db.SubmitChanges();
                                    }
                                }
                            }
                        }
                    }//END OF USING
                }//END OF ELSE

                if (diary[17] != "")
                {
                    try
                    {
                        string Remarks = "½[" + Convert.ToString(TowDigit(pkdatetime.Day)) + "-" + monthNames[pkdatetime.Month - 1] + "-" + Convert.ToString(pkdatetime.Year) + " " + Convert.ToString(TowDigit(pkdatetime.Hour)) + ":" + Convert.ToString(TowDigit(pkdatetime.Minute)) + "]½" + diary[17];
                        using (DBDataContext db = new DBDataContext())
                        {
                            Remarks = Convert.ToString(UserID) + "½" + Convert.ToString(db.ApiUsers.Where(x => x.UserId == UserID).FirstOrDefault().Name) + Remarks;
                            db.uspDiaryUpdateRemakOnly(DIARY_HEADER_ID, Remarks);



                        } ///END OF USING STATEMENT
                    }
                    catch (Exception ex)
                    {

                        return ex.Message;
                    }
                }//END OF IF STATEMENT

                if (TransactionNumber != "")
                {
                    string Mailresult = "";
                    Task.Factory.StartNew<string>(() =>
                    SendMail(UserID, "Invoice Submitted", TransactionNumber))
            .ContinueWith(ant => Mailresult = ant.Result,
                          TaskScheduler.FromCurrentSynchronizationContext());
                }
                return strFormData;
            }
            catch (Exception ex)
            {

                if (ex.HResult == -2146232016)
                {
                    return "-4";
                }

                return ex.Message;
            }
        }

        [HttpPost]
        public string DInvoice()
        {
            sessionCheck();
            string strFormData = "0";
            string strBlockData = "0";
            string strCompData = "0";
            string TransactionNumber = "";
            int UserID = Convert.ToInt32(Fn.ExenID(@"SELECT [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')"));
            ////////if (Request.Headers.Contains("UserID"))
            ////////{
            ////////    var dssd = Request.Headers.GetValues("UserID").FirstOrDefault();
            ////////    UserID = dssd != null ? Convert.ToInt32(dssd) : 0;
            ////////}
            DateTime utc = DateTime.UtcNow;
            DateTime utc_nextday = DateTime.Parse(DateTime.UtcNow.Date.AddDays(1).ToShortDateString() + " 9:01AM");
            //  bool nextDayDateRequired = false;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            DateTime pkdatetime_NextDay = DateTime.Parse(pkdatetime.AddDays(1).ToShortDateString() + " 9:00AM");


            try
            {
                //0   807½				            PPA HEADER ID
                //1   Sapphire Wind Power Company Limited½  
                //2   1035½
                //3   ½---Draft½
                //4   1353½				            Invoice Type
                //5   05-Sep-2018½  			    Invoice Due Date****
                //6   1111 Inv No½      		    Invoice / Letter No *****
                //7   0½				            Parent Invoice*****
                //8   02-Sep-2018½  			    Invoice / Letter Date ******
                //9   03-Sep-2018½          		Invoice Period From *******
                //10  00:00½                        Invoice Period From Time
                //11  04-Sep-2018           		Invoice Period To********
                //12  00:00½                        Invoice Period To Time
                //13  PKR½				            Currency********
                //14  0.0000½				        Total Claim********
                //15  66666 Remarks½			    Remarks
                //16  Draft½                        Button Value
                //17  77777 Notes½                  Lower Notes
                //18  0                             Create or Edit
                var FormData = HttpContext.Request.Form["strFormData"];
                var BlockData = HttpContext.Request.Form["strBlockData"];
                var filekData = HttpContext.Request.Form["strfilekData"];
                var fileDelData = HttpContext.Request.Form["strfileDelData"];
                var dellblock = HttpContext.Request.Form["strdellblock"];
                var fileupdate = HttpContext.Request.Form["strfileupdate"];
                var adjData = HttpContext.Request.Form["stradjData"];
                var adjDelData = HttpContext.Request.Form["stradjDelData"];
                var compData = HttpContext.Request.Form["strCompData"];
                /* var FormData = "737½Kot Addu Power Company Ltd.½1044½Draft½887½21-Jan-2020½test_908½0½21-Jan-2020½21-Jan-2020½00:00½21-Jan-2020½00:00½PKR½121.0000½½Draft½½7569";
                 var BlockData = "8628½21620½Post½LP 0288 EPP 0260 JAN-18½L.P½31-Jan-2018½31-Dec-2017½308,599,110.0000½308,599,110.0000½121.0000½½8628¡0½0½21-Jan-2020½77½77.0000½67.0000½77.0000½21-Jan-2020½21-Jan-2020¡0½0½21-Jan-2020½4½44.0000½4.0000½44.0000½21-Jan-2020½21-Jan-2020»0½21619½Post½LP 0288 GST 0259 DEC-17½L.P½31-Dec-2017½30-Nov-2017½54,755,817.0000½54,755,817.0000½0.0000½½0¡0½0½21-Jan-2020½8½7.0000½7.0000½0.0000½21-Jan-2020½21-Jan-2020»";
                 var fileDelData = "";
                 var filekData = "";
                     var dellblock= "";
                 var fileupdate= "";
                 var adjData= "";
                 var adjDelData= "";
                 var compData = "";*/
                var diary = FormData.Split('½');
                decimal DIARY_HEADER_ID = Convert.ToDecimal(diary[18]);
                if (DIARY_HEADER_ID == 0)
                {
                    using (TransactionScope tranScope = new TransactionScope())
                    {
                        using (DBDataContext db = new DBDataContext())
                        {
                            if (fileupdate.Length > 0)
                            {
                                foreach (var item in fileupdate.Split('¡'))
                                {
                                    var fl = db.ATTACHMENT_HEADERs.Where(v => v.ATTACH_HEADER_ID == Convert.ToDecimal(item.Split('½')[0])).FirstOrDefault();
                                    if (fl != null)
                                    {
                                        fl.ATTACHMENT_TITLE = item.Split('½')[1];
                                        fl.ATTACHMENT_DESC = item.Split('½')[2];
                                        db.SubmitChanges();
                                    }
                                }
                            }
                            var c = db.DIARY_HEADER_INTERFACEs.Where(vvv => vvv.VEN_INV_LETTER_NO == diary[6]).FirstOrDefault();
                            if (c != null)
                            {
                                return "-1";
                            }
                            DIARY_HEADER_INTERFACE obj = new DIARY_HEADER_INTERFACE();
                            obj.CREATION_DATE = pkdatetime;
                            obj.PPA_HEADER_ID = Convert.ToInt32(diary[0]);
                            obj.ORGANIZATION_ID = Convert.ToInt64(diary[2]);
                            obj.INVOICE_TYPE_ID = Convert.ToDouble(diary[4]);
                            obj.INV_DUE_DATE = ToDateTime(diary[5]);
                            obj.VEN_INV_LETTER_NO = diary[6];
                            obj.PARENT_INVOICE_NO = 0;
                            obj.VEN_INV_LETTER_DATE = ToDateTime(diary[8]);
                            obj.INVOICE_PERIOD_FROM = ToDateTime(diary[9] + " " + diary[10]);
                            obj.INVOICE_PERIOD_TO = ToDateTime(diary[11] + " " + diary[12]);
                            obj.CURRENCY = diary[13];
                            obj.TOTAL_CLAIM = Convert.ToDecimal(diary[14]);
                            obj.REMARKS = diary[15];
                            obj.APPROVED_STATUS = diary[16];
                            obj.LAST_UPDATE_DATE = pkdatetime;
                            obj.LAST_UPDATE_LOGIN = Convert.ToString(UserID);
                            obj.IS_DIFFERENTIAL = "Yes";
                            obj.FORM_NAME = "DifferentialInvoice.aspx";
                            obj.INTERFACE_STATUS = "Temp";
                            if (diary[16] == "Submitted")
                            {
                                var fndlpH = db.FND_LOOKUP_VALUEs.Where(v => v.MEANING.ToUpper() == Convert.ToString(TowDigit(pkdatetime.Day)) + "-" + monthNames[pkdatetime.Month - 1].ToUpper() && v.LOOKUP_TYPE == "CPPA_GAZETTED_HOLIDAYS_LK" && v.ENABLED_FLAG == "Y").FirstOrDefault();
                                if (fndlpH != null)
                                {
                                    return "-2";
                                }//end of if
                                else
                                {
                                    var fndlpT = db.FND_LOOKUP_VALUEs.Where(v => v.LOOKUP_CODE.ToUpper() == pkdatetime.ToString("ddd").ToUpper() && v.LOOKUP_TYPE == "CPPA_INVOICE_SUBMIT_TIME_LK").FirstOrDefault();
                                    if (fndlpT != null)
                                    {
                                        try
                                        {
                                            DateTime T1 = new DateTime(pkdatetime.Year, pkdatetime.Month, pkdatetime.Day, Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[0]), Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[1]), Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[2]));
                                            DateTime T2 = new DateTime(pkdatetime.Year, pkdatetime.Month, pkdatetime.Day, Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[0]), Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[1]), Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[2]));

                                            bool flg = (pkdatetime >= T1 && pkdatetime <= T2) ? true : false;
                                            if (!flg)
                                            {
                                                return "Invoice can only be submitted between " + fndlpT.ATTRIBUTE1.Split(':')[0] + ":" + fndlpT.ATTRIBUTE1.Split(':')[1] + " to " + fndlpT.ATTRIBUTE2.Split(':')[0] + ":" + fndlpT.ATTRIBUTE2.Split(':')[1];
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            tranScope.Dispose();
                                            return "-3";
                                        }

                                    }
                                    else
                                    {
                                        tranScope.Dispose();
                                        return "-3";
                                    }

                                }//end of else  

                                obj.SUBMIT_DATE = pkdatetime;

                                long? mx = db.DIARY_HEADER_INTERFACEs.Max(z => z.TransactionNumber);
                                if (mx == null || mx == 0)
                                {
                                    obj.TransactionNumber = 1;
                                    strFormData = "1";
                                    TransactionNumber = "1";
                                }
                                else
                                {
                                    obj.TransactionNumber = mx + 1;
                                    strFormData = Convert.ToString(mx + 1);
                                    TransactionNumber = strFormData;
                                }


                            }

                            db.DIARY_HEADER_INTERFACEs.InsertOnSubmit(obj);
                            db.SubmitChanges();
                            DIARY_HEADER_ID = obj.DIARY_HEADER_ID;

                            if (adjDelData.Length > 0)
                            {
                                List<WP_GC_ADJUSTMENT> adl = new List<WP_GC_ADJUSTMENT>();
                                foreach (string item in adjDelData.Split('½'))
                                {
                                    var a = db.WP_GC_ADJUSTMENTs.Where(v => v.ADJ_HEADER_ID == Convert.ToDecimal(item)).FirstOrDefault();
                                    if (a != null)
                                    {
                                        adl.Add(a);
                                    }
                                }
                                if (adl.Count > 0)
                                {
                                    db.WP_GC_ADJUSTMENTs.DeleteAllOnSubmit(adl);
                                    db.SubmitChanges();
                                }
                            }
                            if (adjData.Length > 0)
                            {
                                foreach (string item in adjData.Split('¼'))
                                {
                                    if (item.Split('½')[0] == "0")
                                    {
                                        WP_GC_ADJUSTMENT oadj = new WP_GC_ADJUSTMENT();
                                        oadj.DIARY_HEADER_ID = DIARY_HEADER_ID;
                                        oadj.REFERENCE_INVOICE_NUMBER = item.Split('½')[1];
                                        oadj.ADJ_TYPES = item.Split('½')[2];
                                        oadj.ADJ_AMOUNT = Convert.ToDouble(item.Split('½')[3]);
                                        oadj.REMARKS = item.Split('½')[4];
                                        db.WP_GC_ADJUSTMENTs.InsertOnSubmit(oadj);
                                        db.SubmitChanges();
                                    }
                                    else
                                    {
                                        var objdup = db.WP_GC_ADJUSTMENTs.Where(w => w.ADJ_HEADER_ID == Convert.ToDecimal(item.Split('½')[0])).FirstOrDefault();
                                        if (objdup != null)
                                        {
                                            objdup.REFERENCE_INVOICE_NUMBER = item.Split('½')[1];
                                            objdup.ADJ_TYPES = item.Split('½')[2];
                                            objdup.ADJ_AMOUNT = Convert.ToDouble(item.Split('½')[3]);
                                            objdup.REMARKS = item.Split('½')[4];
                                            db.SubmitChanges();
                                        }
                                    }
                                }
                            }

                            //Start Block and Component
                            foreach (string blk in BlockData.Split('»'))
                            {
                                if (blk.Length > 0)
                                {
                                    decimal DIFF_PAR_ID_PK = 0;
                                    string ls_invoiceType = ""; //21-01-2020
                                    for (int i = 0; i < blk.Split('¡').Length; i++)
                                    {
                                        if (i == 0)
                                        {
                                            try
                                            {
                                                WP_GC_INV_DIFF_PARENT parent = new WP_GC_INV_DIFF_PARENT();
                                                parent.PARENT_INV_ID_FK = Convert.ToDecimal(blk.Split('¡')[i].Split('½')[1]);
                                                parent.PRE_POST = blk.Split('¡')[i].Split('½')[2];
                                                parent.IPP_INV_NO = blk.Split('¡')[i].Split('½')[3];
                                                ls_invoiceType = parent.INVOICE_TYPE = blk.Split('¡')[i].Split('½')[4]; //21-01-2020

                                                if (Convert.ToString(blk.Split('¡')[i].Split('½')[5]).Trim() == "")
                                                {
                                                    parent.PAR_INV_PER_FRM = null;
                                                }
                                                else
                                                {
                                                    parent.PAR_INV_PER_FRM = ToDateTime(blk.Split('¡')[i].Split('½')[5]);
                                                }
                                                if (Convert.ToString(blk.Split('¡')[i].Split('½')[6]).Trim() == "")
                                                {
                                                    parent.PAR_INV_PER_TO = null;
                                                }
                                                else
                                                {
                                                    parent.PAR_INV_PER_TO = ToDateTime(blk.Split('¡')[i].Split('½')[6]);
                                                }
                                                parent.CLAIM_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[7]);
                                                parent.PAR_INV_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[8]);
                                                parent.CURRENT_CLAIM = Convert.ToDouble(blk.Split('¡')[i].Split('½')[9]);
                                                parent.REMARKS = blk.Split('¡')[i].Split('½')[10];
                                                parent.DIARY_HEADER_ID_FK = DIARY_HEADER_ID;
                                                db.WP_GC_INV_DIFF_PARENTs.InsertOnSubmit(parent);
                                                db.SubmitChanges();
                                                DIFF_PAR_ID_PK = parent.DIFF_PAR_ID_PK;
                                            }
                                            catch (Exception ex)
                                            {
                                                tranScope.Dispose();
                                                strFormData = ex.Message;
                                            }

                                        }
                                        else
                                        {//INSERT COMPONENT START 
                                            WP_GC_INTEREST_DETAIL dtl = new WP_GC_INTEREST_DETAIL();

                                            if (ls_invoiceType == "Pre Live Interest" || ls_invoiceType == "L.P") //21-01-2020
                                            {
                                                dtl.PAYMENT_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[2]);
                                                dtl.NO_OF_DAYS = Convert.ToInt32(blk.Split('¡')[i].Split('½')[3]);
                                                dtl.AMOUNT_PAID = Convert.ToDouble(blk.Split('¡')[i].Split('½')[4]);
                                                dtl.INTEREST_RATE = Convert.ToDouble(blk.Split('¡')[i].Split('½')[5]);
                                                dtl.INTEREST_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[6]);

                                                if (!string.IsNullOrEmpty(blk.Split('¡')[i].Split('½')[7]) && blk.Split('¡')[i].Split('½')[7] != "undefined")
                                                {
                                                    dtl.FROM_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[7]);
                                                }
                                                else
                                                {
                                                    dtl.FROM_DATE = null;
                                                }

                                                if (!string.IsNullOrEmpty(blk.Split('¡')[i].Split('½')[8]))
                                                {
                                                    dtl.End_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[8]);
                                                }
                                                else
                                                {
                                                    dtl.End_DATE = null;
                                                }
                                                dtl.HEADER_ID_FK = DIARY_HEADER_ID;
                                                dtl.DIFF_PAR_ID_FK = DIFF_PAR_ID_PK;

                                                if (dtl.DIFF_PAR_ID_FK != 0 && dtl.DIFF_PAR_ID_FK != null) // Parent (Block) with id = 0 shall not pass  OSAMa
                                                {
                                                    db.WP_GC_INTEREST_DETAILs.InsertOnSubmit(dtl);
                                                    db.SubmitChanges();
                                                }

                                            }
                                            else
                                            {
                                                dtl.PPA_BLOCK_HEADER_ID = Convert.ToDecimal(blk.Split('¡')[i].Split('½')[1]);
                                                dtl.COM_DEF_ID_FK = Convert.ToDecimal(blk.Split('¡')[i].Split('½')[2]);
                                                dtl.Block_Complex = Convert.ToString(blk.Split('¡')[i].Split('½')[3]);
                                                dtl.FULE_TYPE = Convert.ToString(blk.Split('¡')[i].Split('½')[4]);
                                                dtl.COMPONENT = Convert.ToString(blk.Split('¡')[i].Split('½')[5]);
                                                dtl.UNITS = Convert.ToString(blk.Split('¡')[i].Split('½')[6]);
                                                dtl.INTEREST_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[7]);


                                                if (Convert.ToString(blk.Split('¡')[i].Split('½')[8]) == "YES")
                                                {
                                                    dtl.IS_INCLUDE_IN_TOTAL = "Y";
                                                }
                                                else if (Convert.ToString(blk.Split('¡')[i].Split('½')[8]) == "NO")
                                                {
                                                    dtl.IS_INCLUDE_IN_TOTAL = "N";
                                                }

                                                dtl.REMARKS = Convert.ToString(blk.Split('¡')[i].Split('½')[9]);
                                                dtl.HEADER_ID_FK = DIARY_HEADER_ID;
                                                dtl.DIFF_PAR_ID_FK = DIFF_PAR_ID_PK;

                                                if (dtl.DIFF_PAR_ID_FK != 0 && dtl.DIFF_PAR_ID_FK != null) // Parent (Block) with id = 0 shall not pass
                                                {
                                                    db.WP_GC_INTEREST_DETAILs.InsertOnSubmit(dtl);
                                                    db.SubmitChanges();
                                                }
                                            }


                                        }//INSERT COMPONENT end 
                                    }//END OF FOR LOOP
                                }
                            }

                            if (fileDelData.Trim().Length > 0)
                            {
                                foreach (var item in fileDelData.Split('½'))
                                {
                                    try
                                    {
                                        var f = db.ATTACHMENT_HEADERs.Where(v => v.ATTACH_HEADER_ID == Convert.ToDecimal(item)).FirstOrDefault();
                                        if (f != null)
                                        {
                                            f.bisDeleted = true;
                                            db.SubmitChanges();
                                        }
                                    }
                                    catch
                                    {
                                        tranScope.Dispose();

                                    }
                                }
                            }

                            if (dellblock != "")
                            {
                                List<WP_GC_INV_DIFF_PARENT> oList = new List<WP_GC_INV_DIFF_PARENT>();
                                List<WP_GC_INTEREST_DETAIL> dtList = new List<WP_GC_INTEREST_DETAIL>();
                                foreach (string item in dellblock.Split('½'))
                                {
                                    WP_GC_INV_DIFF_PARENT gc = db.WP_GC_INV_DIFF_PARENTs.Where(v => v.DIFF_PAR_ID_PK == Convert.ToDecimal(item)).FirstOrDefault();
                                    if (gc != null)
                                    {
                                        oList.Add(gc);
                                    }
                                    List<WP_GC_INTEREST_DETAIL> dtl = db.WP_GC_INTEREST_DETAILs.Where(v => v.DIFF_PAR_ID_FK == gc.DIFF_PAR_ID_PK).ToList<WP_GC_INTEREST_DETAIL>();
                                    foreach (var itms in dtl)
                                    {
                                        dtList.Add(itms);
                                    }
                                }
                                db.WP_GC_INV_DIFF_PARENTs.DeleteAllOnSubmit(oList);
                                db.WP_GC_INTEREST_DETAILs.DeleteAllOnSubmit(dtList);
                                db.SubmitChanges();
                            }

                            foreach (string item in filekData.Split('¡'))
                            {
                                if (item.Contains("½"))
                                {
                                    decimal? AttachId = 0;
                                    db.sp_Upload_FileNew(DIARY_HEADER_ID, item.Split('½')[1], item.Split('½')[3], item.Split('½')[4], item.Split('½')[2], item.Split('½')[1],
                                   item.Split('½')[5], pkdatetime, Convert.ToString(UserID), ref AttachId);
                                    //try
                                    //{
                                    //    string nm = item.Split('½')[2];
                                    //    string ext = nm.Split('.')[nm.Split('.').Length - 1];
                                    //    String path = HttpContext.Server.MapPath("~/Content/_FilesNew");//Maintain Path for record purpose
                                    //    string FilePath = Path.Combine(path, Convert.ToString(AttachId) + "." + ext);
                                    //    if (!System.IO.File.Exists(FilePath))
                                    //    {
                                    //        byte[] FileBytes = Convert.FromBase64String(item.Split('½')[5]);
                                    //        System.IO.File.WriteAllBytes(FilePath, FileBytes);
                                    //    }
                                    //}
                                    //catch
                                    //{

                                    //    // tranScope.Dispose();
                                    //}
                                }

                            }
                            tranScope.Complete();


                        }//END OF USING
                    }// END OF TRANSACTION

                }
                //Insert Case Ends

                //Update Case Starts
                else
                {
                    using (DBDataContext db = new DBDataContext())
                    {
                        var cxd = db.DIARY_HEADER_INTERFACEs.Where(vvv => vvv.VEN_INV_LETTER_NO == diary[6] && vvv.DIARY_HEADER_ID != DIARY_HEADER_ID).FirstOrDefault();
                        if (cxd != null)
                        {
                            return "-1";
                        }
                        if (fileupdate.Length > 0)
                        {
                            foreach (var item in fileupdate.Split('¡'))
                            {
                                var fl = db.ATTACHMENT_HEADERs.Where(v => v.ATTACH_HEADER_ID == Convert.ToDecimal(item.Split('½')[0])).FirstOrDefault();
                                if (fl != null)
                                {
                                    fl.ATTACHMENT_TITLE = item.Split('½')[1];
                                    fl.ATTACHMENT_DESC = item.Split('½')[2];
                                    db.SubmitChanges();
                                }
                            }
                        }

                        if (fileDelData.Trim().Length > 0)
                        {
                            foreach (var item in fileDelData.Split('½'))
                            {


                                try
                                {
                                    var f = db.ATTACHMENT_HEADERs.Where(v => v.ATTACH_HEADER_ID == Convert.ToDecimal(item)).FirstOrDefault();
                                    if (f != null)
                                    {
                                        f.bisDeleted = true;
                                        db.SubmitChanges();
                                    }
                                }
                                catch
                                {


                                }
                            }
                        }
                        if (adjDelData.Length > 0)
                        {
                            List<WP_GC_ADJUSTMENT> adl = new List<WP_GC_ADJUSTMENT>();
                            foreach (string item in adjDelData.Split('½'))
                            {
                                var a = db.WP_GC_ADJUSTMENTs.Where(v => v.ADJ_HEADER_ID == Convert.ToDecimal(item)).FirstOrDefault();
                                if (a != null)
                                {
                                    adl.Add(a);
                                }
                            }
                            if (adl.Count > 0)
                            {
                                db.WP_GC_ADJUSTMENTs.DeleteAllOnSubmit(adl);
                                db.SubmitChanges();
                            }
                        }
                        if (adjData.Length > 0)
                        {
                            foreach (string item in adjData.Split('¼'))
                            {
                                if (item.Split('½')[0] == "0")
                                {
                                    WP_GC_ADJUSTMENT oadj = new WP_GC_ADJUSTMENT();
                                    oadj.DIARY_HEADER_ID = DIARY_HEADER_ID;
                                    oadj.REFERENCE_INVOICE_NUMBER = item.Split('½')[1];
                                    oadj.ADJ_TYPES = item.Split('½')[2];
                                    oadj.ADJ_AMOUNT = Convert.ToDouble(item.Split('½')[3]);
                                    oadj.REMARKS = item.Split('½')[4];
                                    db.WP_GC_ADJUSTMENTs.InsertOnSubmit(oadj);
                                    db.SubmitChanges();
                                }
                                else
                                {
                                    var objdup = db.WP_GC_ADJUSTMENTs.Where(w => w.ADJ_HEADER_ID == Convert.ToDecimal(item.Split('½')[0])).FirstOrDefault();
                                    if (objdup != null)
                                    {
                                        objdup.REFERENCE_INVOICE_NUMBER = item.Split('½')[1];
                                        objdup.ADJ_TYPES = item.Split('½')[2];
                                        objdup.ADJ_AMOUNT = Convert.ToDouble(item.Split('½')[3]);
                                        objdup.REMARKS = item.Split('½')[4];
                                        db.SubmitChanges();
                                    }
                                }
                            }
                        }
                        // DIARY_HEADER_INTERFACE obj = new DIARY_HEADER_INTERFACE();
                        DIARY_HEADER_INTERFACE obj = db.DIARY_HEADER_INTERFACEs.Where(vd => vd.DIARY_HEADER_ID == Convert.ToDecimal(DIARY_HEADER_ID)).FirstOrDefault();
                        if (obj != null)
                        {
                            //obj.CREATION_DATE = pkdatetime;
                            obj.PPA_HEADER_ID = Convert.ToInt32(diary[0]);
                            obj.ORGANIZATION_ID = Convert.ToInt64(diary[2]);
                            obj.INVOICE_TYPE_ID = Convert.ToDouble(diary[4]);
                            obj.INV_DUE_DATE = ToDateTime(diary[5]);
                            obj.VEN_INV_LETTER_NO = diary[6];
                            obj.PARENT_INVOICE_NO = 0;
                            obj.VEN_INV_LETTER_DATE = ToDateTime(diary[8]);
                            obj.INVOICE_PERIOD_FROM = ToDateTime(diary[9] + " " + diary[10]);
                            obj.INVOICE_PERIOD_TO = ToDateTime(diary[11] + " " + diary[12]);
                            obj.CURRENCY = diary[13];
                            obj.TOTAL_CLAIM = Convert.ToDecimal(diary[14]);
                            obj.REMARKS = diary[15];
                            obj.APPROVED_STATUS = diary[16];
                            obj.LAST_UPDATE_DATE = pkdatetime;
                            obj.LAST_UPDATE_LOGIN = Convert.ToString(UserID);
                            obj.IS_DIFFERENTIAL = "Yes";
                            obj.FORM_NAME = "DifferentialInvoice.aspx";
                            obj.INTERFACE_STATUS = "Temp";
                            if (diary[16] == "Submitted")
                            {
                                var fndlpH = db.FND_LOOKUP_VALUEs.Where(v => v.MEANING.ToUpper() == Convert.ToString(TowDigit(pkdatetime.Day)) + "-" + monthNames[pkdatetime.Month - 1].ToUpper() && v.LOOKUP_TYPE == "CPPA_GAZETTED_HOLIDAYS_LK" && v.ENABLED_FLAG == "Y").FirstOrDefault();
                                if (fndlpH != null)
                                {
                                    return "-2";
                                }//end of if
                                else
                                {
                                    var fndlpT = db.FND_LOOKUP_VALUEs.Where(v => v.LOOKUP_CODE.ToUpper() == pkdatetime.ToString("ddd").ToUpper() && v.LOOKUP_TYPE == "CPPA_INVOICE_SUBMIT_TIME_LK").FirstOrDefault();
                                    if (fndlpT != null)
                                    {
                                        try
                                        {
                                            DateTime T1 = new DateTime(pkdatetime.Year, pkdatetime.Month, pkdatetime.Day, Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[0]), Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[1]), Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[2]));
                                            DateTime T2 = new DateTime(pkdatetime.Year, pkdatetime.Month, pkdatetime.Day, Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[0]), Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[1]), Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[2]));

                                            bool flg = (pkdatetime >= T1 && pkdatetime <= T2) ? true : false;
                                            if (!flg)
                                            {
                                                return "Invoice can only be submitted between " + fndlpT.ATTRIBUTE1.Split(':')[0] + ":" + fndlpT.ATTRIBUTE1.Split(':')[1] + " to " + fndlpT.ATTRIBUTE2.Split(':')[0] + ":" + fndlpT.ATTRIBUTE2.Split(':')[1];
                                            }
                                        }
                                        catch (Exception)
                                        {

                                            return "-3";
                                        }

                                    }
                                    else
                                    {
                                        return "-3";
                                    }
                                }//end of else  
                                obj.SUBMIT_DATE = pkdatetime;


                                long? mx = db.DIARY_HEADER_INTERFACEs.Max(z => z.TransactionNumber);
                                //if (mx == null || mx == 0)
                                //{
                                //    obj.TransactionNumber = 1;
                                //    strFormData = "1";
                                //}
                                //else
                                //{
                                //    obj.TransactionNumber = mx + 1;
                                //    strFormData = Convert.ToString(mx + 1);
                                //}
                                if (obj.IS_APPROVED_BY_TECHNICAL == null && obj.IS_APPROVED_BY_FINANCE == null)
                                {
                                    if (obj.TransactionNumber == null)
                                    {
                                        if (mx == null || mx == 0)
                                        {
                                            obj.TransactionNumber = 1;
                                            strFormData = "1";
                                            TransactionNumber = "1";
                                        }
                                        else
                                        {
                                            obj.TransactionNumber = mx + 1;
                                            strFormData = Convert.ToString(mx + 1);
                                            TransactionNumber = strFormData;
                                        }
                                    }
                                    else
                                    {
                                        strFormData = Convert.ToString(obj.TransactionNumber);
                                        TransactionNumber = strFormData;
                                    }

                                }
                                else
                                {
                                    if (obj.TransactionNumber == null)
                                    {
                                        if (mx == null || mx == 0)
                                        {
                                            obj.TransactionNumber = 1;
                                            strFormData = "1";
                                            TransactionNumber = "1";
                                        }
                                        else
                                        {
                                            obj.TransactionNumber = mx + 1;
                                            strFormData = Convert.ToString(mx + 1);
                                            TransactionNumber = strFormData;
                                        }
                                    }
                                    strFormData = Convert.ToString(obj.TransactionNumber);
                                    TransactionNumber = strFormData;
                                    obj.IS_APPROVED_BY_TECHNICAL = null;
                                    obj.IS_APPROVED_BY_FINANCE = null;
                                    db.SubmitChanges();
                                }


                            }

                            //db.DIARY_HEADER_INTERFACEs.InsertOnSubmit(obj);
                            db.SubmitChanges();
                            //DIARY_HEADER_ID = obj.DIARY_HEADER_ID;
                        }

                        foreach (string blk in BlockData.Split('»'))
                        {
                            if (blk.Length > 0)
                            {
                                decimal DIFF_PAR_ID_PK = 0;
                                //INSERT of Blocks and Components in UPDATE CASE Starts
                                if (blk.Split('¡')[0].Split('½')[0] == "0") // New block checks here
                                {
                                    string ls_invoiceType = "";            //21-01-2020
                                    for (int i = 0; i < blk.Split('¡').Length; i++)
                                    {
                                        if (i == 0)
                                        {
                                            try
                                            {
                                                WP_GC_INV_DIFF_PARENT parent = new WP_GC_INV_DIFF_PARENT();
                                                parent.PARENT_INV_ID_FK = Convert.ToDecimal(blk.Split('¡')[i].Split('½')[1]);
                                                parent.PRE_POST = blk.Split('¡')[i].Split('½')[2];
                                                parent.IPP_INV_NO = blk.Split('¡')[i].Split('½')[3];
                                                ls_invoiceType = parent.INVOICE_TYPE = blk.Split('¡')[i].Split('½')[4];
                                                if (Convert.ToString(blk.Split('¡')[i].Split('½')[5]).Trim() == "")
                                                {
                                                    parent.PAR_INV_PER_FRM = null;
                                                }
                                                else
                                                {
                                                    parent.PAR_INV_PER_FRM = ToDateTime(blk.Split('¡')[i].Split('½')[5]);
                                                }
                                                if (Convert.ToString(blk.Split('¡')[i].Split('½')[6]).Trim() == "")
                                                {
                                                    parent.PAR_INV_PER_TO = null;
                                                }
                                                else
                                                {
                                                    parent.PAR_INV_PER_TO = ToDateTime(blk.Split('¡')[i].Split('½')[6]);
                                                }
                                                //parent.PAR_INV_PER_FRM = ToDateTime(blk.Split('¡')[i].Split('½')[5]);
                                                //parent.PAR_INV_PER_TO = ToDateTime(blk.Split('¡')[i].Split('½')[6]);
                                                parent.CLAIM_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[7]);
                                                parent.PAR_INV_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[8]);
                                                parent.CURRENT_CLAIM = Convert.ToDouble(blk.Split('¡')[i].Split('½')[9]);
                                                parent.REMARKS = blk.Split('¡')[i].Split('½')[10];
                                                parent.DIARY_HEADER_ID_FK = DIARY_HEADER_ID;
                                                db.WP_GC_INV_DIFF_PARENTs.InsertOnSubmit(parent);
                                                db.SubmitChanges();
                                                DIFF_PAR_ID_PK = parent.DIFF_PAR_ID_PK;
                                            }
                                            catch (Exception ex)
                                            {

                                                strFormData = ex.Message;
                                            }

                                        }
                                        else
                                        {
                                            //WP_GC_INTEREST_DETAIL dtl = new WP_GC_INTEREST_DETAIL();
                                            //dtl.PAYMENT_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[1]);
                                            //dtl.NO_OF_DAYS = Convert.ToInt32(blk.Split('¡')[i].Split('½')[2]);
                                            //dtl.AMOUNT_PAID = Convert.ToDouble(blk.Split('¡')[i].Split('½')[3]);
                                            //dtl.INTEREST_RATE = Convert.ToDouble(blk.Split('¡')[i].Split('½')[4]);
                                            //dtl.INTEREST_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[5]);
                                            //dtl.FROM_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[6]);
                                            //dtl.End_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[7]);
                                            //dtl.HEADER_ID_FK = DIARY_HEADER_ID;
                                            //dtl.DIFF_PAR_ID_FK = DIFF_PAR_ID_PK;
                                            //db.WP_GC_INTEREST_DETAILs.InsertOnSubmit(dtl);
                                            //db.SubmitChanges();

                                            WP_GC_INTEREST_DETAIL dtl = new WP_GC_INTEREST_DETAIL();

                                            if (ls_invoiceType == "Pre Live Interest" || ls_invoiceType == "L.P") //21-01-2020
                                            {
                                                dtl.PAYMENT_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[2]);
                                                dtl.NO_OF_DAYS = Convert.ToInt32(blk.Split('¡')[i].Split('½')[3]);
                                                dtl.AMOUNT_PAID = Convert.ToDouble(blk.Split('¡')[i].Split('½')[4]);
                                                dtl.INTEREST_RATE = Convert.ToDouble(blk.Split('¡')[i].Split('½')[5]);
                                                dtl.INTEREST_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[6]);

                                                if (!string.IsNullOrEmpty(blk.Split('¡')[i].Split('½')[7]) && blk.Split('¡')[i].Split('½')[7] != "undefined")
                                                {
                                                    dtl.FROM_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[7]);
                                                }
                                                else
                                                {
                                                    dtl.FROM_DATE = null;
                                                }

                                                if (!string.IsNullOrEmpty(blk.Split('¡')[i].Split('½')[8]))
                                                {
                                                    dtl.End_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[8]);
                                                }
                                                else
                                                {
                                                    dtl.End_DATE = null;
                                                }
                                                dtl.HEADER_ID_FK = DIARY_HEADER_ID;
                                                dtl.DIFF_PAR_ID_FK = DIFF_PAR_ID_PK;

                                                if (dtl.DIFF_PAR_ID_FK != 0 && dtl.DIFF_PAR_ID_FK != null) // Parent (Block) with id = 0 shall not pass 
                                                {
                                                    db.WP_GC_INTEREST_DETAILs.InsertOnSubmit(dtl);
                                                    db.SubmitChanges();
                                                }

                                            }
                                            else
                                            {
                                                dtl.PPA_BLOCK_HEADER_ID = Convert.ToDecimal(blk.Split('¡')[i].Split('½')[1]);
                                                dtl.COM_DEF_ID_FK = Convert.ToDecimal(blk.Split('¡')[i].Split('½')[2]);
                                                dtl.Block_Complex = Convert.ToString(blk.Split('¡')[i].Split('½')[3]);
                                                dtl.FULE_TYPE = Convert.ToString(blk.Split('¡')[i].Split('½')[4]);
                                                dtl.COMPONENT = Convert.ToString(blk.Split('¡')[i].Split('½')[5]);
                                                dtl.UNITS = Convert.ToString(blk.Split('¡')[i].Split('½')[6]);
                                                dtl.INTEREST_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[7]);

                                                if (Convert.ToString(blk.Split('¡')[i].Split('½')[8]) == "YES")
                                                {
                                                    dtl.IS_INCLUDE_IN_TOTAL = "Y";
                                                }
                                                else if (Convert.ToString(blk.Split('¡')[i].Split('½')[8]) == "NO")
                                                {
                                                    dtl.IS_INCLUDE_IN_TOTAL = "N";
                                                }
                                                dtl.REMARKS = Convert.ToString(blk.Split('¡')[i].Split('½')[9]);
                                                dtl.HEADER_ID_FK = DIARY_HEADER_ID;
                                                dtl.DIFF_PAR_ID_FK = DIFF_PAR_ID_PK;
                                                //Aymen
                                                if (dtl.DIFF_PAR_ID_FK != 0 && dtl.DIFF_PAR_ID_FK != null) // Parent (Block) with id = 0 shall not pass
                                                {
                                                    db.WP_GC_INTEREST_DETAILs.InsertOnSubmit(dtl);
                                                    db.SubmitChanges();
                                                }
                                            }


                                        }
                                    }//END OF FOR LOOP
                                }
                                //INSERT of Blocks and Components in UPDATE CASE Ends

                                //UPDATE of Blocks and Components in UPDATE CASE Starts
                                else
                                {
                                    string ls_invoiceType = "";                     //21-01-2020
                                    for (int i = 0; i < blk.Split('¡').Length; i++) // Old Block update here
                                    {
                                        if (i == 0)
                                        {
                                            try
                                            {
                                                //WP_GC_INV_DIFF_PARENT parent = new WP_GC_INV_DIFF_PARENT();
                                                WP_GC_INV_DIFF_PARENT parent = db.WP_GC_INV_DIFF_PARENTs.Where(p => p.DIFF_PAR_ID_PK == Convert.ToDecimal(blk.Split('¡')[i].Split('½')[0])).FirstOrDefault();
                                                if (parent != null)
                                                {
                                                    parent.PARENT_INV_ID_FK = Convert.ToDecimal(blk.Split('¡')[i].Split('½')[1]);
                                                    parent.PRE_POST = blk.Split('¡')[i].Split('½')[2];
                                                    parent.IPP_INV_NO = blk.Split('¡')[i].Split('½')[3];
                                                    ls_invoiceType = parent.INVOICE_TYPE = blk.Split('¡')[i].Split('½')[4];
                                                    if (Convert.ToString(blk.Split('¡')[i].Split('½')[5]).Trim() == "")
                                                    {
                                                        parent.PAR_INV_PER_FRM = null;
                                                    }
                                                    else
                                                    {
                                                        parent.PAR_INV_PER_FRM = ToDateTime(blk.Split('¡')[i].Split('½')[5]);
                                                    }
                                                    if (Convert.ToString(blk.Split('¡')[i].Split('½')[6]).Trim() == "")
                                                    {
                                                        parent.PAR_INV_PER_TO = null;
                                                    }
                                                    else
                                                    {
                                                        parent.PAR_INV_PER_TO = ToDateTime(blk.Split('¡')[i].Split('½')[6]);
                                                    }
                                                    //parent.PAR_INV_PER_FRM = ToDateTime(blk.Split('¡')[i].Split('½')[5]);
                                                    //parent.PAR_INV_PER_TO = ToDateTime(blk.Split('¡')[i].Split('½')[6]);
                                                    parent.CLAIM_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[7]);
                                                    parent.PAR_INV_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[8]);
                                                    parent.CURRENT_CLAIM = Convert.ToDouble(blk.Split('¡')[i].Split('½')[9]);
                                                    parent.REMARKS = blk.Split('¡')[i].Split('½')[10];
                                                    parent.DIARY_HEADER_ID_FK = DIARY_HEADER_ID;
                                                    //db.WP_GC_INV_DIFF_PARENTs.InsertOnSubmit(parent);
                                                    db.SubmitChanges();
                                                    DIFF_PAR_ID_PK = parent.DIFF_PAR_ID_PK;
                                                }

                                            }
                                            catch (Exception ex)
                                            {

                                                strFormData = ex.Message;
                                            }

                                        }
                                        else
                                        {

                                            //WP_GC_INTEREST_DETAIL dtl = new WP_GC_INTEREST_DETAIL();
                                            WP_GC_INTEREST_DETAIL dtl = db.WP_GC_INTEREST_DETAILs.Where(d => d.INT_DET_ID_PK == Convert.ToDecimal(blk.Split('¡')[i].Split('½')[0])).FirstOrDefault();
                                            if (dtl != null)
                                            {
                                                //dtl.PAYMENT_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[1]);
                                                //dtl.NO_OF_DAYS = Convert.ToInt32(blk.Split('¡')[i].Split('½')[2]);
                                                //dtl.AMOUNT_PAID = Convert.ToDouble(blk.Split('¡')[i].Split('½')[3]);
                                                //dtl.INTEREST_RATE = Convert.ToDouble(blk.Split('¡')[i].Split('½')[4]);
                                                //dtl.INTEREST_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[5]);
                                                //dtl.FROM_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[6]);
                                                //dtl.End_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[7]);
                                                //dtl.HEADER_ID_FK = DIARY_HEADER_ID;
                                                //dtl.DIFF_PAR_ID_FK = DIFF_PAR_ID_PK;
                                                ////db.WP_GC_INTEREST_DETAILs.InsertOnSubmit(dtl);
                                                //db.SubmitChanges();
                                                //WP_GC_INTEREST_DETAIL dtl = new WP_GC_INTEREST_DETAIL();

                                                if (ls_invoiceType == "Pre Live Interest" || ls_invoiceType == "L.P")   //21-01-2020
                                                {
                                                    dtl.PAYMENT_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[2]);
                                                    dtl.NO_OF_DAYS = Convert.ToInt32(blk.Split('¡')[i].Split('½')[3]);
                                                    dtl.AMOUNT_PAID = Convert.ToDouble(blk.Split('¡')[i].Split('½')[4]);
                                                    dtl.INTEREST_RATE = Convert.ToDouble(blk.Split('¡')[i].Split('½')[5]);
                                                    dtl.INTEREST_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[6]);

                                                    if (!string.IsNullOrEmpty(blk.Split('¡')[i].Split('½')[7]) && blk.Split('¡')[i].Split('½')[7] != "undefined")
                                                    {
                                                        dtl.FROM_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[7]);
                                                    }
                                                    else
                                                    {
                                                        dtl.FROM_DATE = null;
                                                    }

                                                    if (!string.IsNullOrEmpty(blk.Split('¡')[i].Split('½')[8]))
                                                    {
                                                        dtl.End_DATE = ToDateTime(blk.Split('¡')[i].Split('½')[8]);
                                                    }
                                                    else
                                                    {
                                                        dtl.End_DATE = null;
                                                    }

                                                    string[] compArr = null;
                                                    if (compData != null || compData != "")
                                                    {
                                                        compArr = compData.Split('½');
                                                        Array.Resize(ref compArr, compArr.Length - 1);
                                                        for (int c = 0; c < compArr.Length; c++)
                                                        {
                                                            var record = db.WP_GC_INTEREST_DETAILs.FirstOrDefault(m => m.INT_DET_ID_PK == Convert.ToDecimal(compArr[c]));
                                                            db.WP_GC_INTEREST_DETAILs.DeleteOnSubmit(record);

                                                        }

                                                    }
                                                    dtl.HEADER_ID_FK = DIARY_HEADER_ID;
                                                    dtl.DIFF_PAR_ID_FK = DIFF_PAR_ID_PK;

                                                    if (dtl.DIFF_PAR_ID_FK != 0 && dtl.DIFF_PAR_ID_FK != null) // Parent (Block) with id = 0 shall not pass  OSAMa
                                                    {
                                                        //db.WP_GC_INTEREST_DETAILs.InsertOnSubmit(dtl);
                                                        db.SubmitChanges();
                                                        compData = "";
                                                    }
                                                    //AYMEN
                                                }
                                                else
                                                {
                                                    dtl.PPA_BLOCK_HEADER_ID = Convert.ToDecimal(blk.Split('¡')[i].Split('½')[1]);
                                                    dtl.COM_DEF_ID_FK = Convert.ToDecimal(blk.Split('¡')[i].Split('½')[2]);
                                                    dtl.Block_Complex = Convert.ToString(blk.Split('¡')[i].Split('½')[3]);
                                                    dtl.FULE_TYPE = Convert.ToString(blk.Split('¡')[i].Split('½')[4]);
                                                    dtl.COMPONENT = Convert.ToString(blk.Split('¡')[i].Split('½')[5]);
                                                    dtl.UNITS = Convert.ToString(blk.Split('¡')[i].Split('½')[6]);
                                                    dtl.INTEREST_AMOUNT = Convert.ToDouble(blk.Split('¡')[i].Split('½')[7]);

                                                    if (Convert.ToString(blk.Split('¡')[i].Split('½')[8]) == "YES")
                                                    {
                                                        dtl.IS_INCLUDE_IN_TOTAL = "Y";
                                                    }
                                                    else if (Convert.ToString(blk.Split('¡')[i].Split('½')[8]) == "NO")
                                                    {
                                                        dtl.IS_INCLUDE_IN_TOTAL = "N";
                                                    }


                                                    string[] compArr = null;
                                                    if (compData != null || compData != "")
                                                    {
                                                        compArr = compData.Split('½');
                                                        Array.Resize(ref compArr, compArr.Length - 1);
                                                        for (int c = 0; c < compArr.Length; c++)
                                                        {
                                                            var record = db.WP_GC_INTEREST_DETAILs.FirstOrDefault(m => m.INT_DET_ID_PK == Convert.ToDecimal(compArr[c]));
                                                            db.WP_GC_INTEREST_DETAILs.DeleteOnSubmit(record);

                                                        }

                                                    }


                                                    dtl.REMARKS = Convert.ToString(blk.Split('¡')[i].Split('½')[9]);
                                                    dtl.HEADER_ID_FK = DIARY_HEADER_ID;
                                                    dtl.DIFF_PAR_ID_FK = DIFF_PAR_ID_PK;
                                                    //Aymen
                                                    if (dtl.DIFF_PAR_ID_FK != 0 && dtl.DIFF_PAR_ID_FK != null) // Parent (Block) with id = 0 shall not pass
                                                    {

                                                        db.SubmitChanges();
                                                        compData = "";
                                                    }
                                                }

                                            }
                                            else    //dtl == null -Insert of component in Update case (old block new components)
                                            {
                                                if (ls_invoiceType == "Pre Live Interest" || ls_invoiceType == "L.P") //21-01-2020
                                                {

                                                    WP_GC_INTEREST_DETAIL dtlNew = new WP_GC_INTEREST_DETAIL();
                                                    var lCompData = blk.Split('¡')[i].Split('½');


                                                    dtlNew.PAYMENT_DATE = ToDateTime(lCompData[2]); // PAYMENT DATE 
                                                    dtlNew.NO_OF_DAYS = Convert.ToInt32(lCompData[3]); // No of Days Delay  
                                                    dtlNew.AMOUNT_PAID = Convert.ToDouble(lCompData[4]); //Amount Paid 
                                                    dtlNew.INTEREST_RATE = Convert.ToDouble(lCompData[5]); // Interest Rate % 
                                                    dtlNew.INTEREST_AMOUNT = Convert.ToDouble(lCompData[6]); //Interest Amount 
                                                    if (!string.IsNullOrEmpty(lCompData[7]))
                                                    {
                                                        dtlNew.FROM_DATE = ToDateTime(lCompData[7]); // Interest Rate From
                                                    }
                                                    else
                                                    {
                                                        dtlNew.FROM_DATE = null;
                                                    }

                                                    if (!string.IsNullOrEmpty(lCompData[8]))
                                                    {
                                                        dtlNew.End_DATE = ToDateTime(lCompData[8]); // Interest Rate To
                                                    }
                                                    else
                                                    {
                                                        dtlNew.End_DATE = null;
                                                    }

                                                    string[] compArr = null;
                                                    if (compData != null || compData != "")
                                                    {
                                                        compArr = compData.Split('½');
                                                        Array.Resize(ref compArr, compArr.Length - 1);
                                                        for (int c = 0; c < compArr.Length; c++)
                                                        {
                                                            var record = db.WP_GC_INTEREST_DETAILs.FirstOrDefault(m => m.INT_DET_ID_PK == Convert.ToDecimal(compArr[c]));
                                                            db.WP_GC_INTEREST_DETAILs.DeleteOnSubmit(record);

                                                        }

                                                    }

                                                    dtlNew.HEADER_ID_FK = DIARY_HEADER_ID;
                                                    dtlNew.DIFF_PAR_ID_FK = DIFF_PAR_ID_PK;
                                                    if (dtlNew.DIFF_PAR_ID_FK != 0 && dtlNew.DIFF_PAR_ID_FK != null) // Parent (Block) with id = 0 shall not pass Osama
                                                    {
                                                        db.WP_GC_INTEREST_DETAILs.InsertOnSubmit(dtlNew);
                                                        db.SubmitChanges();
                                                    }
                                                }
                                            }

                                        }

                                    }//END OF FOR LOOP
                                }

                                //UPDATE of Blocks and Components in UPDATE CASE Ends

                            }
                        }
                        foreach (string item in filekData.Split('¡'))
                        {
                            if (item.Contains("½"))
                            {
                                decimal? AttachId = 0;
                                db.sp_Upload_FileNew(DIARY_HEADER_ID, item.Split('½')[1], item.Split('½')[3], item.Split('½')[4], item.Split('½')[2], item.Split('½')[1],
                               item.Split('½')[5], pkdatetime, Convert.ToString(UserID), ref AttachId);
                                //try
                                //{
                                //    string nm = item.Split('½')[2];
                                //    string ext = nm.Split('.')[nm.Split('.').Length - 1];
                                //    String path = HttpContext.Server.MapPath("~/Content/_FilesNew"); //Maintain Path for record purpose
                                //    string FilePath = Path.Combine(path, Convert.ToString(AttachId) + "." + ext);
                                //    if (!System.IO.File.Exists(FilePath))
                                //    {
                                //        byte[] FileBytes = Convert.FromBase64String(item.Split('½')[5]);
                                //        System.IO.File.WriteAllBytes(FilePath, FileBytes);
                                //    }
                                //}
                                //catch
                                //{


                                //}
                            }

                        }
                        if (dellblock != "")
                        {
                            List<WP_GC_INV_DIFF_PARENT> oList = new List<WP_GC_INV_DIFF_PARENT>();
                            List<WP_GC_INTEREST_DETAIL> dtList = new List<WP_GC_INTEREST_DETAIL>();
                            foreach (string item in dellblock.Split('½'))
                            {
                                WP_GC_INV_DIFF_PARENT gc = db.WP_GC_INV_DIFF_PARENTs.Where(v => v.DIFF_PAR_ID_PK == Convert.ToDecimal(item)).FirstOrDefault();
                                if (gc != null)
                                {
                                    oList.Add(gc);
                                }
                                List<WP_GC_INTEREST_DETAIL> dtl = db.WP_GC_INTEREST_DETAILs.Where(v => v.DIFF_PAR_ID_FK == gc.DIFF_PAR_ID_PK).ToList<WP_GC_INTEREST_DETAIL>();
                                foreach (var itms in dtl)
                                {
                                    dtList.Add(itms);
                                }
                            }
                            db.WP_GC_INV_DIFF_PARENTs.DeleteAllOnSubmit(oList);
                            db.WP_GC_INTEREST_DETAILs.DeleteAllOnSubmit(dtList);
                            db.SubmitChanges();
                        }
                    }//END OF USING
                }//END OF ELSE

                if (diary[17] != "")
                {
                    try
                    {
                        string Remarks = "½[" + Convert.ToString(TowDigit(pkdatetime.Day)) + "-" + monthNames[pkdatetime.Month - 1] + "-" + Convert.ToString(pkdatetime.Year) + " " + Convert.ToString(TowDigit(pkdatetime.Hour)) + ":" + Convert.ToString(TowDigit(pkdatetime.Minute)) + "]½" + diary[17];
                        using (DBDataContext db = new DBDataContext())
                        {
                            Remarks = Convert.ToString(UserID) + "½" + Convert.ToString(db.ApiUsers.Where(x => x.UserId == UserID).FirstOrDefault().Name) + Remarks;
                            db.uspDiaryUpdateRemakOnly(DIARY_HEADER_ID, Remarks);



                        } ///END OF USING STATEMENT
                    }
                    catch (Exception ex)
                    {

                        return ex.Message;
                    }
                }//END OF IF STATEMENT

                if (TransactionNumber != "")
                {
                    string Mailresult = "";
                    Task.Factory.StartNew<string>(() =>
                    SendMail(UserID, "Invoice Submitted", TransactionNumber))
            .ContinueWith(ant => Mailresult = ant.Result,
                          TaskScheduler.FromCurrentSynchronizationContext());
                }
                return strFormData;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }
        [HttpPost]
        public string Withdraw()
        {
            try
            {
                int UserID = 0;
                //if (Request.Headers.Contains("UserID"))
                //{
                //    var dssd = Request.Headers.GetValues("UserID").FirstOrDefault();
                //    UserID = dssd != null ? Convert.ToInt32(dssd) : 0;
                //}
                UserID = Convert.ToInt32(Fn.ExenID(@"SELECT [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')"));
                var FormData = HttpContext.Request.Form["strFormData"];
                DateTime utc = DateTime.UtcNow;
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
                // string Remarks = "½[" + Convert.ToString(TowDigit(pkdatetime.Day)) + "-" + monthNames[pkdatetime.Month-1] + "-" + Convert.ToString(pkdatetime.Year) + " " + Convert.ToString(TowDigit(pkdatetime.Hour)) + ":" + Convert.ToString(TowDigit(pkdatetime.Minute)) + "]½" + FormData.Split('½')[2];
                using (DBDataContext db = new DBDataContext())
                {
                    // Remarks = Convert.ToString(UserID) + "½" + Convert.ToString(db.ApiUsers.Where(x => x.UserId == UserID).FirstOrDefault().Name) + Remarks;
                    // db.uspDiaryUpdateRemak(Convert.ToDecimal(FormData.Split('½')[0]), Remarks, UserID, pkdatetime, FormData.Split('½')[1], FormData.Split('½')[2].Trim().Length);
                    var dhi = db.DIARY_HEADER_INTERFACEs.Where(w => w.DIARY_HEADER_ID == Convert.ToDecimal(FormData)).FirstOrDefault();
                    if (dhi != null)
                    {
                        dhi.APPROVED_STATUS = "Withdraw";
                        dhi.LAST_UPDATE_DATE = pkdatetime;
                        dhi.LAST_UPDATE_LOGIN = Convert.ToString(UserID);
                        db.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
            return "1";
        }

        [HttpPost]
        public string CreateInvoice()
        {
            string strFormData = "0";
            string strBlockData = "0";
            string TransactionNumber = "";
            try
            {
                var FormData = HttpContext.Request.Form["strFormData"];
                var BlockData = HttpContext.Request.Form["strBlockData"];
                var filekData = HttpContext.Request.Form["strfilekData"];
                var adjData = HttpContext.Request.Form["stradjData"];
                var adjDelData = HttpContext.Request.Form["stradjDelData"];

                //var FormData = "867½The Hub Power Company Limited½1037½Draft½84½04-Feb-2019½EPP-20/01/2019-GST½866½21-Jan-2019½20-Jan-2019½00:00½20-Jan-2019½00:00½PKR½11,414,454.0000½½Submitted½";
                //var BlockData = "810½1½Complex½RFO½20-Jan-2019½20-Jan-2019½11,414,454.0000½0.0000½0.0000½0.0000½0.0000½¡fin½1933½6105½GST Amount½Rs.½11,414,454.0000½17.0000½";
                //var filekData = "";
                //var adjData = "";
                //var adjDelData = "";


                var diary = FormData.Split('½');
                //0   737½				            PPA HEADER ID
                //1   Kot Addu Power Company Ltd.½
                //2   kapco_ipp @cppag.onmicrosoft.com½
                //3   Draft½
                //4   884½				            Invoice Type
                //5   31 - Aug - 2018½			    Invoice Due Date****
                //6   Invoice / Letter No½		    Invoice / Letter No *****
                //7   5702½				            Parent Invoice*****
                //8   01 - Aug - 2018½			    Invoice / Letter Date ******
                //9   20 - Aug - 2018½00:00½		Invoice Period From *******
                //11  30 - Aug - 2018½00:00½		Invoice Period To********
                //13  PKR½				            Currency********
                //14  23365½				        Total Claim********
                //15  Remarks½			            Remarks
                //16  Draft                         Button Value

                DateTime utc = DateTime.UtcNow;
                DateTime utc_nextday = DateTime.Parse(DateTime.UtcNow.Date.AddDays(1).ToShortDateString() + " 9:01AM");
                //bool nextDayDateRequired = false;
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
                DateTime pkdatetime_NextDay = DateTime.Parse(pkdatetime.AddDays(1).ToShortDateString() + " 9:00AM");

                decimal? PARENT_INVOICE_NO = null;
                int UserID = Convert.ToInt32(Fn.ExenID(@"SELECT [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')"));
                //if (Request.Headers.Contains("UserID"))
                //{
                //    var dssd = Request.Headers.GetValues("UserID").FirstOrDefault();
                //    UserID = dssd != null ? Convert.ToInt32(dssd) : 0;
                //}
                try
                {
                    PARENT_INVOICE_NO = Convert.ToDecimal(diary[7]);
                }
                catch { PARENT_INVOICE_NO = 0; }

                decimal DIARY_HEADER_ID = 0;
                using (TransactionScope tranScope = new TransactionScope())
                {
                    using (DBDataContext db = new DBDataContext())
                    {
                        var c = db.DIARY_HEADER_INTERFACEs.Where(vvv => vvv.VEN_INV_LETTER_NO == diary[6]).FirstOrDefault();
                        if (c != null)
                        {
                            return "-1";
                        }

                        if (diary[16] == "Submitted")
                        {
                            double appInvoiceID = Convert.ToDouble(diary[4]);

                            // ******************* User should not be able to Submit GST invoice if Parent Invoice is not Submitted **********************

                            var lsInvoiceType = db.PPA_APPLICABLE_INVOICEs.Where(v => v.APP_INVOICES_ID_PK == appInvoiceID).FirstOrDefault();

                            if (lsInvoiceType.INVOICE_TYPES == "GST")
                            {
                                var statuses = new string[] { "Submitted", "Received" };

                                var StatusOfParentInvoiceObj = db.DIARY_HEADER_INTERFACEs.Where(v => statuses.Contains(v.APPROVED_STATUS) && v.DIARY_HEADER_ID == PARENT_INVOICE_NO).FirstOrDefault();

                                if (StatusOfParentInvoiceObj == null)
                                {
                                    return "-101";
                                }

                            }

                        }

                        DIARY_HEADER_INTERFACE obj = new DIARY_HEADER_INTERFACE();
                        obj.CREATION_DATE = pkdatetime;
                        obj.PPA_HEADER_ID = Convert.ToInt32(diary[0]);
                        obj.ORGANIZATION_ID = Convert.ToInt64(diary[2]);
                        obj.INVOICE_TYPE_ID = Convert.ToDouble(diary[4]);
                        obj.INV_DUE_DATE = ToDateTime(diary[5]);
                        obj.VEN_INV_LETTER_NO = diary[6];
                        obj.PARENT_INVOICE_NO = PARENT_INVOICE_NO;
                        obj.VEN_INV_LETTER_DATE = ToDateTime(diary[8]);
                        obj.INVOICE_PERIOD_FROM = ToDateTime(diary[9] + " " + diary[10]);
                        obj.INVOICE_PERIOD_TO = ToDateTime(diary[11] + " " + diary[12]);
                        obj.CURRENCY = diary[13];
                        obj.TOTAL_CLAIM = Convert.ToDecimal(diary[14]);
                        obj.REMARKS = diary[15];
                        obj.APPROVED_STATUS = diary[16];
                        obj.LAST_UPDATE_DATE = pkdatetime;
                        obj.LAST_UPDATE_LOGIN = Convert.ToString(UserID);
                        obj.IS_DIFFERENTIAL = "No";
                        obj.FORM_NAME = "MonthlyForm.aspx";
                        obj.INTERFACE_STATUS = "Temp";
                        if (diary[16] == "Submitted")
                        {
                            var fndlpH = db.FND_LOOKUP_VALUEs.Where(v => v.MEANING.ToUpper() == Convert.ToString(TowDigit(pkdatetime.Day)) + "-" + monthNames[pkdatetime.Month - 1].ToUpper() && v.LOOKUP_TYPE == "CPPA_GAZETTED_HOLIDAYS_LK" && v.ENABLED_FLAG == "Y").FirstOrDefault();
                            if (fndlpH != null)
                            {
                                return "-2";
                            }//end of if
                            else
                            {
                                var fndlpT = db.FND_LOOKUP_VALUEs.Where(v => v.LOOKUP_CODE.ToUpper() == pkdatetime.ToString("ddd").ToUpper() && v.LOOKUP_TYPE == "CPPA_INVOICE_SUBMIT_TIME_LK").FirstOrDefault();
                                if (fndlpT != null)
                                {
                                    try
                                    {
                                        DateTime T1 = new DateTime(pkdatetime.Year, pkdatetime.Month, pkdatetime.Day, Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[0]), Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[1]), Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[2]));
                                        DateTime T2 = new DateTime(pkdatetime.Year, pkdatetime.Month, pkdatetime.Day, Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[0]), Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[1]), Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[2]));

                                        bool flg = (pkdatetime >= T1 && pkdatetime <= T2) ? true : false;
                                        if (!flg)
                                        {
                                            return "Invoice can only be submitted between " + fndlpT.ATTRIBUTE1.Split(':')[0] + ":" + fndlpT.ATTRIBUTE1.Split(':')[1] + " to " + fndlpT.ATTRIBUTE2.Split(':')[0] + ":" + fndlpT.ATTRIBUTE2.Split(':')[1];
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        tranScope.Dispose();
                                        return "-3";
                                    }

                                }
                                else
                                {
                                    tranScope.Dispose();
                                    return "-3";
                                }

                            }//end of else  

                            obj.SUBMIT_DATE = pkdatetime;


                            long? mx = db.DIARY_HEADER_INTERFACEs.Max(z => z.TransactionNumber);
                            if (mx == null || mx == 0)
                            {
                                obj.TransactionNumber = 1;
                                strFormData = "1";
                                TransactionNumber = "1";
                            }
                            else
                            {
                                obj.TransactionNumber = mx + 1;
                                strFormData = Convert.ToString(mx + 1);
                                TransactionNumber = strFormData;
                            }
                        }

                        // ***************************************************************************************
                        var isTransactionNoExists = db.DIARY_HEADER_INTERFACEs.Where(v => v.TransactionNumber == obj.TransactionNumber).FirstOrDefault();

                        if (isTransactionNoExists != null)
                        {
                            tranScope.Dispose();
                            return "Transaction Number has already been used. Please try to submit again.";
                        }

                        // ***************************************************************************************

                        db.DIARY_HEADER_INTERFACEs.InsertOnSubmit(obj);
                        db.SubmitChanges();
                        DIARY_HEADER_ID = obj.DIARY_HEADER_ID;
                        if (DIARY_HEADER_ID != 0)
                        {

                            foreach (string item in BlockData.Split('»'))
                            {
                                decimal BLOCK_HEADER_ID = 0;
                                for (int i = 0; i < item.Split('¡').Length; i++)
                                {

                                    if (i == 0)
                                    {
                                        BLOCKS_HEADER_INTERFACE bobj = new BLOCKS_HEADER_INTERFACE();
                                        bobj.DIARY_HEADER_ID = DIARY_HEADER_ID;
                                        bobj.PPA_BLOCK_HEADER_ID = Convert.ToInt32(item.Split('¡')[i].Split('½')[0]);
                                        bobj.bisRequired = Convert.ToInt32(item.Split('¡')[i].Split('½')[1]);
                                        bobj.RATE_VALID_FROM = ToDateTime(item.Split('¡')[i].Split('½')[4]);
                                        bobj.RATE_VALID_TO = ToDateTime(item.Split('¡')[i].Split('½')[5]);
                                        bobj.CLAIM_TOTAL = Convert.ToDecimal(item.Split('¡')[i].Split('½')[6]);
                                        bobj.ADVANCE_PAID = Convert.ToDecimal(item.Split('¡')[i].Split('½')[7]);
                                        bobj.REMAINING_CURRENT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[8]);
                                        bobj.ADVANCE_NEXT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[9]);
                                        bobj.PAYABLE_CURRENT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[10]);
                                        bobj.REMARKS = item.Split('¡')[i].Split('½')[11];
                                        db.BLOCKS_HEADER_INTERFACEs.InsertOnSubmit(bobj);
                                        db.SubmitChanges();
                                        BLOCK_HEADER_ID = bobj.BLOCK_HEADER_ID;
                                    }
                                    else
                                    {
                                        COMP_HEADER_INTERFACE cobj = new COMP_HEADER_INTERFACE();
                                        if (item.Split('¡')[i].Split('½')[0] == "tech")
                                        {
                                            cobj.IS_INCLUDE_IN_TOTAL = "N";
                                            cobj.BLOCK_HEADER_ID = BLOCK_HEADER_ID;
                                            cobj.COM_DEF_ID_FK = Convert.ToDouble(item.Split('¡')[i].Split('½')[2]);
                                            cobj.COMP_NAME = item.Split('¡')[i].Split('½')[3];
                                            cobj.COMP_VALUE = Convert.ToDecimal(item.Split('¡')[i].Split('½')[5]);
                                            cobj.REMARKS = item.Split('¡')[i].Split('½')[6];
                                            cobj.IS_MANDATORY = item.Split('¡')[i].Split('½')[8];
                                            if (cobj.BLOCK_HEADER_ID != 0 && cobj.BLOCK_HEADER_ID != null) //  (Block) with id = 0 shall not pass Aymen
                                            {
                                                db.COMP_HEADER_INTERFACEs.InsertOnSubmit(cobj);
                                                db.SubmitChanges();
                                            }
                                        }//Technical Components Insert END
                                        else  //Financial Components Insert Start
                                        {
                                            if (item.Split('¡')[i].Split('½')[8] == "YES")
                                            {
                                                cobj.IS_INCLUDE_IN_TOTAL = "Y";
                                            }
                                            else
                                            {
                                                cobj.IS_INCLUDE_IN_TOTAL = "N";
                                            }
                                            //cobj.IS_INCLUDE_IN_TOTAL = "Y";
                                            cobj.BLOCK_HEADER_ID = BLOCK_HEADER_ID;
                                            cobj.COM_DEF_ID_FK = Convert.ToDouble(item.Split('¡')[i].Split('½')[2]);
                                            cobj.COMP_NAME = item.Split('¡')[i].Split('½')[3];
                                            cobj.COMP_VALUE = Convert.ToDecimal(item.Split('¡')[i].Split('½')[5]);
                                            cobj.GST_PER = item.Split('¡')[i].Split('½')[6];
                                            cobj.REMARKS = item.Split('¡')[i].Split('½')[7];
                                            cobj.IS_MANDATORY = item.Split('¡')[i].Split('½')[9];
                                            db.COMP_HEADER_INTERFACEs.InsertOnSubmit(cobj);
                                            db.SubmitChanges();
                                        }

                                    }
                                }
                            }
                            if (adjDelData.Length > 0)
                            {
                                List<WP_GC_ADJUSTMENT> adl = new List<WP_GC_ADJUSTMENT>();
                                foreach (string item in adjDelData.Split('½'))
                                {
                                    var a = db.WP_GC_ADJUSTMENTs.Where(v => v.ADJ_HEADER_ID == Convert.ToDecimal(item)).FirstOrDefault();
                                    if (a != null)
                                    {
                                        adl.Add(a);
                                    }
                                }
                                if (adl.Count > 0)
                                {
                                    db.WP_GC_ADJUSTMENTs.DeleteAllOnSubmit(adl);
                                    db.SubmitChanges();
                                }
                            }

                            if (adjData.Length > 0)
                            {
                                foreach (string item in adjData.Split('¼'))
                                {
                                    if (item.Split('½')[0] == "0")
                                    {
                                        WP_GC_ADJUSTMENT oadj = new WP_GC_ADJUSTMENT();
                                        oadj.DIARY_HEADER_ID = DIARY_HEADER_ID;
                                        oadj.REFERENCE_INVOICE_NUMBER = item.Split('½')[1];
                                        oadj.ADJ_TYPES = item.Split('½')[2];
                                        oadj.ADJ_AMOUNT = Convert.ToDouble(item.Split('½')[3]);
                                        oadj.REMARKS = item.Split('½')[4];
                                        db.WP_GC_ADJUSTMENTs.InsertOnSubmit(oadj);
                                        db.SubmitChanges();
                                    }
                                    else
                                    {
                                        var objdup = db.WP_GC_ADJUSTMENTs.Where(w => w.ADJ_HEADER_ID == Convert.ToDecimal(item.Split('½')[0])).FirstOrDefault();
                                        if (objdup != null)
                                        {
                                            objdup.REFERENCE_INVOICE_NUMBER = item.Split('½')[1];
                                            objdup.ADJ_TYPES = item.Split('½')[2];
                                            objdup.ADJ_AMOUNT = Convert.ToDouble(item.Split('½')[3]);
                                            objdup.REMARKS = item.Split('½')[4];
                                            db.SubmitChanges();
                                        }
                                    }
                                }
                            }
                        }

                        //if (filekData.Contains("¡"))
                        //{
                        foreach (string item in filekData.Split('¡'))
                        {
                            if (item.Contains("½"))
                            {
                                decimal? AttachId = 0;
                                db.sp_Upload_FileNew(DIARY_HEADER_ID, item.Split('½')[1], item.Split('½')[3], item.Split('½')[4], item.Split('½')[2], item.Split('½')[1],
                                                           item.Split('½')[5], pkdatetime, Convert.ToString(UserID), ref AttachId);
                                //try
                                //{
                                //    string nm = item.Split('½')[2];
                                //    string ext = nm.Split('.')[nm.Split('.').Length - 1];
                                //    String path = HttpContext.Server.MapPath("~/Content/_FilesNew"); //Maintain Path for record purpose
                                //    string FilePath = Path.Combine(path, Convert.ToString(AttachId) + "." + ext);
                                //    if (!System.IO.File.Exists(FilePath))
                                //    {
                                //        byte[] FileBytes = Convert.FromBase64String(item.Split('½')[5]);
                                //        System.IO.File.WriteAllBytes(FilePath, FileBytes);
                                //    }
                                //}
                                //catch
                                //{
                                //    tranScope.Dispose();

                                //}


                            }
                            // }
                        }
                        tranScope.Complete();



                    }///END OF USING
                }
                if (diary[17] != "")
                {
                    try
                    {
                        string Remarks = "½[" + Convert.ToString(TowDigit(pkdatetime.Day)) + "-" + monthNames[pkdatetime.Month - 1] + "-" + Convert.ToString(pkdatetime.Year) + " " + Convert.ToString(TowDigit(pkdatetime.Hour)) + ":" + Convert.ToString(TowDigit(pkdatetime.Minute)) + "]½" + diary[17];
                        using (DBDataContext db = new DBDataContext())
                        {
                            Remarks = Convert.ToString(UserID) + "½" + Convert.ToString(db.ApiUsers.Where(x => x.UserId == UserID).FirstOrDefault().Name) + Remarks;
                            db.uspDiaryUpdateRemakOnly(DIARY_HEADER_ID, Remarks);
                        }
                    }
                    catch (Exception ex)
                    {

                        return ex.Message;
                    }
                }//END OF IF STATEMENT


                if (TransactionNumber != "")
                {
                    string Mailresult = "";
                    Task.Factory.StartNew<string>(() =>
                    SendMail(UserID, "Invoice Submitted", TransactionNumber))
            .ContinueWith(ant => Mailresult = ant.Result,
                          TaskScheduler.FromCurrentSynchronizationContext());
                }

                return strFormData;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }



        [HttpPost]
        public string UpdateInvoice()
        {
            sessionCheck();
            string strFormData = "0";
            string strBlockData = "0";
            string TransactionNumber = "";
            try
            {
                var FormData = HttpContext.Request.Form["strFormData"];
                var BlockData = HttpContext.Request.Form["strBlockData"];

                // var FormData = "4496½Liberty Power Tech Limited½1056½Returned½1332½31 - Aug - 2019½LPTL / CPP - M / JUL19 / 0115½0½01 - Aug - 2019½01 - Jul - 2019½00:00½31 - Jul - 2019½00:00½PKR½548,942,030.0000½½Draft½";
                // var BlockData = "3981½1½Complex½RFO½01-Jul-2019½30-Sep-2019½520,748,854.0000½-333,843,987.0000½186,904,867.0000½362,037,163.0000½548,942,030.0000½¡tech½10075½11281½AvailCap½kWh½144,861,810.0000½¡fin½10076½11284½CPP Amount½Rs.½520,748,854.0000½½¡fin½10077½16913½Advance Already Verified½Rs.½-333,843,987.0000½½¡fin½10078½16914½Advance Next Month½Rs.½362,037,163.0000½½";

                var filekData = HttpContext.Request.Form["strfilekData"];
                var fileDelData = HttpContext.Request.Form["strfileDelData"];
                var blockDataNew = HttpContext.Request.Form["strblockDataNew"];
                var dellblock = HttpContext.Request.Form["strdellblock"];
                var fileupdate = HttpContext.Request.Form["strfileupdate"];
                var adjData = HttpContext.Request.Form["stradjData"];
                var adjDelData = HttpContext.Request.Form["stradjDelData"];

                var diary = FormData.Split('½');
                //0   737½				            PPA HEADER ID
                //1   Kot Addu Power Company Ltd.½
                //2   kapco_ipp @cppag.onmicrosoft.com½
                //3   Draft½
                //4   884½				            Invoice Type
                //5   31 - Aug - 2018½			    Invoice Due Date****
                //6   Invoice / Letter No½		    Invoice / Letter No *****
                //7   5702½				            Parent Invoice*****
                //8   01 - Aug - 2018½			    Invoice / Letter Date ******
                //9   20 - Aug - 2018½00:00½		Invoice Period From *******
                //11  30 - Aug - 2018½00:00½		Invoice Period To********
                //13  PKR½				            Currency********
                //14  23365½				        Total Claim********
                //15  Remarks½			            Remarks
                //16  Draft                         Button Value
                //17  Invoice Bottom Remarks
                decimal? PARENT_INVOICE_NO = null;
                try
                {
                    PARENT_INVOICE_NO = Convert.ToDecimal(diary[7]);
                }
                catch { PARENT_INVOICE_NO = 0; }
                DateTime utc = DateTime.UtcNow;
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
                decimal DIARY_HEADER_ID = Convert.ToDecimal(diary[0]);

                int UserID = 0;

                using (DBDataContext db = new DBDataContext())
                {
                    var c = db.DIARY_HEADER_INTERFACEs.Where(vvv => vvv.VEN_INV_LETTER_NO == diary[6] && vvv.DIARY_HEADER_ID != DIARY_HEADER_ID).FirstOrDefault();
                    if (c != null)
                    {
                        return "-1";
                        // return "Invoice number " + diary[6] + " already exists..!";
                    }
                }
                if (diary[17] != "")
                {
                    try
                    {

                        //if (Request.Headers.Contains("UserID"))
                        //{
                        //    var dssd = Request.Headers.GetValues("UserID").FirstOrDefault();
                        //    UserID = dssd != null ? Convert.ToInt32(dssd) : 0;
                        //}

                        UserID = Convert.ToInt32(Fn.ExenID(@"SELECT [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')"));

                        string Remarks = "½[" + Convert.ToString(TowDigit(pkdatetime.Day)) + "-" + monthNames[pkdatetime.Month - 1] + "-" + Convert.ToString(TowDigit(pkdatetime.Year)) + " " + Convert.ToString(TowDigit(pkdatetime.Hour)) + ":" + Convert.ToString(TowDigit(pkdatetime.Minute)) + "]½" + diary[17];
                        using (DBDataContext db = new DBDataContext())
                        {
                            Remarks = Convert.ToString(UserID) + "½" + Convert.ToString(db.ApiUsers.Where(x => x.UserId == UserID).FirstOrDefault().Name) + Remarks;
                            db.uspDiaryUpdateRemakOnly(DIARY_HEADER_ID, Remarks);
                        }
                    }
                    catch (Exception ex)
                    {

                        return ex.Message;
                    }
                }//END OF IF STATEMENT

                var prevAPPROVED_STATUS = "";
                using (DBDataContext db = new DBDataContext())
                {
                    var obj = db.DIARY_HEADER_INTERFACEs.Where(x => x.DIARY_HEADER_ID == Convert.ToDecimal(diary[0])).FirstOrDefault();
                    if (obj != null)
                    {
                        if (diary[16] == "Submitted")
                        {
                            double appInvoiceID = Convert.ToDouble(diary[4]);


                            // ******************* User should not be able to Submit GST invoice if Parent Invoice is not Submitted **********************

                            var lsInvoiceType = db.PPA_APPLICABLE_INVOICEs.Where(v => v.APP_INVOICES_ID_PK == appInvoiceID).FirstOrDefault();

                            if (lsInvoiceType.INVOICE_TYPES == "GST")
                            {
                                var statuses = new string[] { "Submitted", "Received" };

                                var StatusOfParentInvoiceObj = db.DIARY_HEADER_INTERFACEs.Where(v => statuses.Contains(v.APPROVED_STATUS) && v.DIARY_HEADER_ID == PARENT_INVOICE_NO).FirstOrDefault();

                                if (StatusOfParentInvoiceObj == null)
                                {
                                    return "-101";
                                }

                            }

                        }

                        //obj.CREATION_DATE = pkdatetime;
                        //obj.PPA_HEADER_ID = Convert.ToInt32(diary[0]);
                        //obj.IS_APPROVED_BY_FINANCE = null;
                        //obj.IS_APPROVED_BY_TECHNICAL = null;
                        obj.ORGANIZATION_ID = Convert.ToInt64(diary[2]);
                        obj.INVOICE_TYPE_ID = Convert.ToDouble(diary[4]);
                        obj.INV_DUE_DATE = ToDateTime(diary[5]);
                        obj.VEN_INV_LETTER_NO = diary[6];
                        obj.PARENT_INVOICE_NO = PARENT_INVOICE_NO;
                        obj.VEN_INV_LETTER_DATE = ToDateTime(diary[8]);
                        obj.INVOICE_PERIOD_FROM = ToDateTime(diary[9] + " " + diary[10]);
                        obj.INVOICE_PERIOD_TO = ToDateTime(diary[11] + " " + diary[12]);
                        obj.CURRENCY = diary[13];
                        obj.TOTAL_CLAIM = Convert.ToDecimal(diary[14]);
                        obj.REMARKS = diary[15];
                        obj.LAST_UPDATE_LOGIN = Convert.ToString(UserID);
                        obj.LAST_UPDATE_DATE = pkdatetime;
                        obj.IS_DIFFERENTIAL = "No";
                        obj.FORM_NAME = "MonthlyForm.aspx";
                        obj.INTERFACE_STATUS = "Temp";
                        prevAPPROVED_STATUS = obj.APPROVED_STATUS;
                        if (diary[16] == "Submitted")
                        {
                            var fndlpH = db.FND_LOOKUP_VALUEs.Where(v => v.MEANING.ToUpper() == Convert.ToString(TowDigit(pkdatetime.Day)) + "-" + monthNames[pkdatetime.Month - 1].ToUpper() && v.LOOKUP_TYPE == "CPPA_GAZETTED_HOLIDAYS_LK" && v.ENABLED_FLAG == "Y").FirstOrDefault();
                            if (fndlpH != null)
                            {
                                return "-2";
                            }//end of if
                            else
                            {
                                var fndlpT = db.FND_LOOKUP_VALUEs.Where(v => v.LOOKUP_CODE.ToUpper() == pkdatetime.ToString("ddd").ToUpper() && v.LOOKUP_TYPE == "CPPA_INVOICE_SUBMIT_TIME_LK").FirstOrDefault();
                                if (fndlpT != null)
                                {
                                    try
                                    {
                                        DateTime T1 = new DateTime(pkdatetime.Year, pkdatetime.Month, pkdatetime.Day, Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[0]), Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[1]), Convert.ToInt32(fndlpT.ATTRIBUTE1.Split(':')[2]));
                                        DateTime T2 = new DateTime(pkdatetime.Year, pkdatetime.Month, pkdatetime.Day, Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[0]), Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[1]), Convert.ToInt32(fndlpT.ATTRIBUTE2.Split(':')[2]));

                                        bool flg = (pkdatetime >= T1 && pkdatetime <= T2) ? true : false;
                                        if (!flg)
                                        {
                                            return "Invoice can only be submitted between " + fndlpT.ATTRIBUTE1.Split(':')[0] + ":" + fndlpT.ATTRIBUTE1.Split(':')[1] + " to " + fndlpT.ATTRIBUTE2.Split(':')[0] + ":" + fndlpT.ATTRIBUTE2.Split(':')[1];
                                        }
                                    }
                                    catch (Exception)
                                    {

                                        return "-3";
                                    }

                                }
                                else
                                {
                                    return "-3";
                                }
                            }//end of else  
                            obj.SUBMIT_DATE = pkdatetime;
                            var sno = obj.TransactionNumber;
                            if (sno == null)
                            {

                                //if (mx == null || mx == 0)
                                //{
                                //    obj.TransactionNumber = 1;
                                //    strFormData = "1";
                                //}
                                //else
                                //{
                                //    obj.TransactionNumber = mx + 1;
                                //    strFormData = Convert.ToString(mx + 1);
                                //}
                                if (obj.IS_APPROVED_BY_TECHNICAL == null && obj.IS_APPROVED_BY_FINANCE == null)
                                {
                                    long? mx = db.DIARY_HEADER_INTERFACEs.Max(z => z.TransactionNumber);
                                    if (mx == null || mx == 0)
                                    {
                                        obj.TransactionNumber = 1;
                                        strFormData = "1";
                                        TransactionNumber = "1";
                                    }
                                    else
                                    {
                                        obj.TransactionNumber = mx + 1;
                                        strFormData = Convert.ToString(mx + 1);
                                        TransactionNumber = strFormData;
                                    }
                                }
                                else
                                {
                                    strFormData = Convert.ToString(obj.TransactionNumber);
                                    obj.IS_APPROVED_BY_TECHNICAL = null;
                                    obj.IS_APPROVED_BY_FINANCE = null;
                                    db.SubmitChanges();
                                }
                            }
                            else
                            {
                                strFormData = Convert.ToString(sno);
                                TransactionNumber = strFormData;
                            }

                            obj.APPROVED_STATUS = diary[16];


                        }


                        // ***************************************************************************************
                        var isTransactionNoExists = db.DIARY_HEADER_INTERFACEs.Where(v => v.TransactionNumber == obj.TransactionNumber).FirstOrDefault();

                        if (isTransactionNoExists != null && prevAPPROVED_STATUS != "Returned")
                        {
                            return "Transaction Number has already been used. Please try to submit again.";
                        }

                        // ***************************************************************************************

                        db.SubmitChanges();


                        if (DIARY_HEADER_ID != 0)
                        {

                            foreach (string item in BlockData.Split('»'))
                            {
                                decimal BLOCK_HEADER_ID = 0;
                                for (int i = 0; i < item.Split('¡').Length; i++)
                                {

                                    if (i == 0)
                                    {


                                        //BLOCKS_HEADER_INTERFACE bobj = new BLOCKS_HEADER_INTERFACE();
                                        //bobj.DIARY_HEADER_ID = DIARY_HEADER_ID;
                                        //bobj.PPA_BLOCK_HEADER_ID = Convert.ToInt32(item.Split('¡')[i].Split('½')[0]);
                                        //bobj.bisRequired = Convert.ToInt32(item.Split('¡')[i].Split('½')[1]);
                                        //bobj.RATE_VALID_FROM = ToDateTime(item.Split('¡')[i].Split('½')[4]);
                                        //bobj.RATE_VALID_TO = ToDateTime(item.Split('¡')[i].Split('½')[5]);
                                        //bobj.CLAIM_TOTAL = Convert.ToDecimal(item.Split('¡')[i].Split('½')[6]);
                                        //bobj.ADVANCE_PAID = Convert.ToDecimal(item.Split('¡')[i].Split('½')[7]);
                                        //bobj.REMAINING_CURRENT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[8]);
                                        //bobj.ADVANCE_NEXT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[9]);
                                        //bobj.PAYABLE_CURRENT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[10]);
                                        //bobj.REMARKS = item.Split('¡')[i].Split('½')[11];
                                        //db.BLOCKS_HEADER_INTERFACEs.InsertOnSubmit(bobj);
                                        //db.SubmitChanges();
                                        //BLOCK_HEADER_ID = bobj.BLOCK_HEADER_ID;

                                        var mblk = db.BLOCKS_HEADER_INTERFACEs.Where(v => v.BLOCK_HEADER_ID == Convert.ToDecimal(item.Split('¡')[i].Split('½')[0])).FirstOrDefault();
                                        if (mblk != null)
                                        {
                                            BLOCK_HEADER_ID = mblk.BLOCK_HEADER_ID;
                                            mblk.RATE_VALID_FROM = ToDateTime(item.Split('¡')[i].Split('½')[4]);
                                            mblk.RATE_VALID_TO = ToDateTime(item.Split('¡')[i].Split('½')[5]);
                                            mblk.CLAIM_TOTAL = Convert.ToDecimal(item.Split('¡')[i].Split('½')[6]);
                                            mblk.ADVANCE_PAID = Convert.ToDecimal(item.Split('¡')[i].Split('½')[7]);
                                            mblk.REMAINING_CURRENT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[8]);
                                            mblk.ADVANCE_NEXT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[9]);
                                            mblk.PAYABLE_CURRENT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[10]);
                                            mblk.REMARKS = item.Split('¡')[i].Split('½')[11];
                                            db.SubmitChanges();
                                        }
                                    }
                                    else
                                    {
                                        COMP_HEADER_INTERFACE cobj = new COMP_HEADER_INTERFACE();
                                        if (item.Split('¡')[i].Split('½')[0] == "tech")
                                        {
                                            //cobj.IS_INCLUDE_IN_TOTAL = "N";
                                            //cobj.BLOCK_HEADER_ID = BLOCK_HEADER_ID;
                                            //cobj.COM_DEF_ID_FK = Convert.ToDouble(item.Split('¡')[i].Split('½')[2]);
                                            //cobj.COMP_NAME = item.Split('¡')[i].Split('½')[3];
                                            //cobj.COMP_VALUE = Convert.ToDecimal(item.Split('¡')[i].Split('½')[5]);
                                            //cobj.REMARKS = item.Split('¡')[i].Split('½')[6];
                                            //db.COMP_HEADER_INTERFACEs.InsertOnSubmit(cobj);
                                            //db.SubmitChanges();

                                            var mcobj = db.COMP_HEADER_INTERFACEs.Where(v => v.COM_HEADER_ID == Convert.ToDecimal(item.Split('¡')[i].Split('½')[1])).FirstOrDefault();
                                            if (mcobj != null)
                                            {
                                                mcobj.BLOCK_HEADER_ID = BLOCK_HEADER_ID;
                                                mcobj.COM_DEF_ID_FK = Convert.ToDouble(item.Split('¡')[i].Split('½')[2]);
                                                mcobj.COMP_NAME = item.Split('¡')[i].Split('½')[3];
                                                mcobj.COMP_VALUE = Convert.ToDecimal(item.Split('¡')[i].Split('½')[5]);
                                                mcobj.REMARKS = item.Split('¡')[i].Split('½')[6];
                                                if (mcobj.BLOCK_HEADER_ID != 0 && mcobj.BLOCK_HEADER_ID != null)
                                                {
                                                    db.SubmitChanges();
                                                }
                                            }

                                        }
                                        else //UPDATE CASE FINANCIAL UPDATE
                                        {
                                            //cobj.IS_INCLUDE_IN_TOTAL = "Y";
                                            //cobj.BLOCK_HEADER_ID = BLOCK_HEADER_ID;
                                            //cobj.COM_DEF_ID_FK = Convert.ToDouble(item.Split('¡')[i].Split('½')[2]);
                                            //cobj.COMP_NAME = item.Split('¡')[i].Split('½')[3];
                                            //cobj.COMP_VALUE = Convert.ToDecimal(item.Split('¡')[i].Split('½')[5]);
                                            //cobj.GST_PER = item.Split('¡')[i].Split('½')[6];
                                            //cobj.REMARKS = item.Split('¡')[i].Split('½')[7];
                                            //db.COMP_HEADER_INTERFACEs.InsertOnSubmit(cobj);
                                            //db.SubmitChanges();

                                            var mcobj = db.COMP_HEADER_INTERFACEs.Where(v => v.COM_HEADER_ID == Convert.ToDecimal(item.Split('¡')[i].Split('½')[1])).FirstOrDefault();
                                            if (mcobj != null)
                                            {
                                                mcobj.BLOCK_HEADER_ID = BLOCK_HEADER_ID;
                                                mcobj.COM_DEF_ID_FK = Convert.ToDouble(item.Split('¡')[i].Split('½')[2]);
                                                mcobj.COMP_NAME = item.Split('¡')[i].Split('½')[3];
                                                mcobj.COMP_VALUE = Convert.ToDecimal(item.Split('¡')[i].Split('½')[5]);
                                                mcobj.GST_PER = item.Split('¡')[i].Split('½')[6];
                                                mcobj.REMARKS = item.Split('¡')[i].Split('½')[7];

                                                if (mcobj.BLOCK_HEADER_ID != 0 && mcobj.BLOCK_HEADER_ID != null) //  (Block) with id = 0 shall not pass Aymen
                                                {

                                                    db.SubmitChanges();
                                                }
                                            }
                                        }

                                    }
                                }
                            }


                        }
                        if (adjDelData.Length > 0)
                        {
                            List<WP_GC_ADJUSTMENT> adl = new List<WP_GC_ADJUSTMENT>();
                            foreach (string item in adjDelData.Split('½'))
                            {
                                var a = db.WP_GC_ADJUSTMENTs.Where(v => v.ADJ_HEADER_ID == Convert.ToDecimal(item)).FirstOrDefault();
                                if (a != null)
                                {
                                    adl.Add(a);
                                }
                            }
                            if (adl.Count > 0)
                            {
                                db.WP_GC_ADJUSTMENTs.DeleteAllOnSubmit(adl);
                                db.SubmitChanges();
                            }
                        }
                        if (adjData.Length > 0)
                        {
                            foreach (string item in adjData.Split('¼'))
                            {
                                if (item.Split('½')[0] == "0")
                                {
                                    WP_GC_ADJUSTMENT oadj = new WP_GC_ADJUSTMENT();
                                    oadj.DIARY_HEADER_ID = DIARY_HEADER_ID;
                                    oadj.REFERENCE_INVOICE_NUMBER = item.Split('½')[1];
                                    oadj.ADJ_TYPES = item.Split('½')[2];
                                    oadj.ADJ_AMOUNT = Convert.ToDouble(item.Split('½')[3]);
                                    oadj.REMARKS = item.Split('½')[4];
                                    db.WP_GC_ADJUSTMENTs.InsertOnSubmit(oadj);
                                    db.SubmitChanges();
                                }
                                else
                                {
                                    var objdup = db.WP_GC_ADJUSTMENTs.Where(w => w.ADJ_HEADER_ID == Convert.ToDecimal(item.Split('½')[0])).FirstOrDefault();
                                    if (objdup != null)
                                    {
                                        objdup.REFERENCE_INVOICE_NUMBER = item.Split('½')[1];
                                        objdup.ADJ_TYPES = item.Split('½')[2];
                                        objdup.ADJ_AMOUNT = Convert.ToDouble(item.Split('½')[3]);
                                        objdup.REMARKS = item.Split('½')[4];
                                        db.SubmitChanges();
                                    }
                                }
                            }
                        }

                    }


                    foreach (string item in BlockData.Split('»'))
                    {
                        decimal BLOCK_HEADER_ID = 0;
                        for (int i = 0; i < item.Split('¡').Length; i++)
                        {

                            if (i == 0)
                            {
                                //BLOCKS_HEADER_INTERFACE bobj = new BLOCKS_HEADER_INTERFACE();
                                var bobj = db.BLOCKS_HEADER_INTERFACEs.Where(x => x.BLOCK_HEADER_ID == Convert.ToDecimal(item.Split('¡')[i].Split('½')[0])).FirstOrDefault();
                                if (bobj != null)
                                {
                                    bobj.DIARY_HEADER_ID = DIARY_HEADER_ID;
                                    //bobj.PPA_BLOCK_HEADER_ID = Convert.ToInt32(item.Split('¡')[i].Split('½')[0]);
                                    bobj.bisRequired = Convert.ToInt32(item.Split('¡')[i].Split('½')[1]);
                                    bobj.RATE_VALID_FROM = ToDateTime(item.Split('¡')[i].Split('½')[4]);
                                    bobj.RATE_VALID_TO = ToDateTime(item.Split('¡')[i].Split('½')[5]);
                                    bobj.CLAIM_TOTAL = Convert.ToDecimal(item.Split('¡')[i].Split('½')[6]);
                                    bobj.ADVANCE_PAID = Convert.ToDecimal(item.Split('¡')[i].Split('½')[7]);
                                    bobj.REMAINING_CURRENT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[8]);
                                    bobj.ADVANCE_NEXT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[9]);
                                    bobj.PAYABLE_CURRENT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[10]);
                                    bobj.REMARKS = item.Split('¡')[i].Split('½')[11];
                                    //db.BLOCKS_HEADER_INTERFACEs.InsertOnSubmit(bobj);
                                    db.SubmitChanges();
                                    BLOCK_HEADER_ID = bobj.BLOCK_HEADER_ID;
                                }

                            }
                            else
                            {
                                //COMP_HEADER_INTERFACE cobj = new COMP_HEADER_INTERFACE();
                                var cobj = db.COMP_HEADER_INTERFACEs.Where(x => x.COM_HEADER_ID == Convert.ToDecimal(item.Split('¡')[i].Split('½')[1])).FirstOrDefault();
                                if (cobj != null)
                                {
                                    if (item.Split('¡')[i].Split('½')[0] == "tech")
                                    {
                                        cobj.IS_INCLUDE_IN_TOTAL = "N";
                                        cobj.BLOCK_HEADER_ID = BLOCK_HEADER_ID;
                                        cobj.COM_DEF_ID_FK = Convert.ToDouble(item.Split('¡')[i].Split('½')[2]);
                                        cobj.COMP_NAME = item.Split('¡')[i].Split('½')[3];
                                        cobj.COMP_VALUE = Convert.ToDecimal(item.Split('¡')[i].Split('½')[5]);
                                        cobj.REMARKS = item.Split('¡')[i].Split('½')[6];
                                        cobj.IS_MANDATORY= item.Split('¡')[i].Split('½')[8];
                                        //db.COMP_HEADER_INTERFACEs.InsertOnSubmit(cobj);
                                        if (cobj.BLOCK_HEADER_ID != 0 && cobj.BLOCK_HEADER_ID != null) //  (Block) with id = 0 shall not pass Aymen
                                        {
                                            db.SubmitChanges();
                                        }
                                    }
                                    else
                                    {
                                        if (item.Split('¡')[i].Split('½')[8] == "Y")
                                        {
                                            cobj.IS_INCLUDE_IN_TOTAL = "Y";
                                        }
                                        else
                                        {
                                            cobj.IS_INCLUDE_IN_TOTAL = "N";
                                        }
                                        //cobj.IS_INCLUDE_IN_TOTAL = "Y";
                                        cobj.BLOCK_HEADER_ID = BLOCK_HEADER_ID;
                                        cobj.COM_DEF_ID_FK = Convert.ToDouble(item.Split('¡')[i].Split('½')[2]);
                                        cobj.COMP_NAME = item.Split('¡')[i].Split('½')[3];
                                        cobj.COMP_VALUE = Convert.ToDecimal(item.Split('¡')[i].Split('½')[5]);
                                        cobj.GST_PER = item.Split('¡')[i].Split('½')[6];
                                        cobj.REMARKS = item.Split('¡')[i].Split('½')[7];
                                        cobj.IS_MANDATORY = item.Split('¡')[i].Split('½')[9];

                                        //db.COMP_HEADER_INTERFACEs.InsertOnSubmit(cobj);
                                        if (cobj.BLOCK_HEADER_ID != 0 && cobj.BLOCK_HEADER_ID != null) //  (Block) with id = 0 shall not pass Aymen
                                        {
                                            db.SubmitChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //DIARY_HEADER_ID = obj.DIARY_HEADER_ID;
                    if (DIARY_HEADER_ID != 0 && blockDataNew.Length > 0)
                    {
                        //UPDATE BLOCKS INSERT CASE 


                        ///////////Insert if new block appear
                        foreach (string item in blockDataNew.Split('»'))
                        {
                            decimal BLOCK_HEADER_ID = 0;
                            for (int i = 0; i < item.Split('¡').Length; i++)
                            {

                                if (i == 0)
                                {
                                    BLOCKS_HEADER_INTERFACE bobj = new BLOCKS_HEADER_INTERFACE();
                                    bobj.DIARY_HEADER_ID = DIARY_HEADER_ID;
                                    bobj.PPA_BLOCK_HEADER_ID = Convert.ToInt32(item.Split('¡')[i].Split('½')[0]);
                                    bobj.bisRequired = Convert.ToInt32(item.Split('¡')[i].Split('½')[1]);
                                    bobj.RATE_VALID_FROM = ToDateTime(item.Split('¡')[i].Split('½')[4]);
                                    bobj.RATE_VALID_TO = ToDateTime(item.Split('¡')[i].Split('½')[5]);
                                    bobj.CLAIM_TOTAL = Convert.ToDecimal(item.Split('¡')[i].Split('½')[6]);
                                    bobj.ADVANCE_PAID = Convert.ToDecimal(item.Split('¡')[i].Split('½')[7]);
                                    bobj.REMAINING_CURRENT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[8]);
                                    bobj.ADVANCE_NEXT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[9]);
                                    bobj.PAYABLE_CURRENT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[10]);
                                    bobj.REMARKS = item.Split('¡')[i].Split('½')[11];
                                    db.BLOCKS_HEADER_INTERFACEs.InsertOnSubmit(bobj);
                                    db.SubmitChanges();
                                    BLOCK_HEADER_ID = bobj.BLOCK_HEADER_ID;
                                }
                                else //Tech comp update insert case
                                {
                                    COMP_HEADER_INTERFACE cobj = new COMP_HEADER_INTERFACE();
                                    if (item.Split('¡')[i].Split('½')[0] == "tech")
                                    {
                                        cobj.IS_INCLUDE_IN_TOTAL = "N";
                                        cobj.BLOCK_HEADER_ID = BLOCK_HEADER_ID;
                                        cobj.COM_DEF_ID_FK = Convert.ToDouble(item.Split('¡')[i].Split('½')[2]);
                                        cobj.COMP_NAME = item.Split('¡')[i].Split('½')[3];
                                        cobj.COMP_VALUE = Convert.ToDecimal(item.Split('¡')[i].Split('½')[5]);
                                        cobj.REMARKS = item.Split('¡')[i].Split('½')[6];
                                        cobj.IS_MANDATORY = item.Split('¡')[i].Split('½')[8];

                                        if (cobj.BLOCK_HEADER_ID != 0 && cobj.BLOCK_HEADER_ID != null)
                                        {
                                            db.COMP_HEADER_INTERFACEs.InsertOnSubmit(cobj);
                                            db.SubmitChanges();
                                        }
                                    }
                                    else  //Financial comp update insert case
                                    {
                                        if (item.Split('¡')[i].Split('½')[8] == "YES")
                                        {
                                            cobj.IS_INCLUDE_IN_TOTAL = "Y";
                                        }
                                        else
                                        {
                                            cobj.IS_INCLUDE_IN_TOTAL = "N";
                                        }
                                        //cobj.IS_INCLUDE_IN_TOTAL = "Y";
                                        cobj.BLOCK_HEADER_ID = BLOCK_HEADER_ID;
                                        cobj.COM_DEF_ID_FK = Convert.ToDouble(item.Split('¡')[i].Split('½')[2]);
                                        cobj.COMP_NAME = item.Split('¡')[i].Split('½')[3];
                                        cobj.COMP_VALUE = Convert.ToDecimal(item.Split('¡')[i].Split('½')[5]);
                                        cobj.GST_PER = item.Split('¡')[i].Split('½')[6];
                                        cobj.REMARKS = item.Split('¡')[i].Split('½')[7];
                                        cobj.IS_MANDATORY = item.Split('¡')[i].Split('½')[9];

                                        if (cobj.BLOCK_HEADER_ID != 0 && cobj.BLOCK_HEADER_ID != null) //  (Block) with id = 0 shall not pass Aymen
                                        {
                                            db.COMP_HEADER_INTERFACEs.InsertOnSubmit(cobj);
                                            db.SubmitChanges();
                                        }
                                    }

                                }
                            }
                        }
                        ///////////End of Insert if new block appear

                        foreach (string item in BlockData.Split('»'))
                        {
                            if (item.Length > 2)
                            {


                                decimal BLOCK_HEADER_ID = 0;
                                for (int i = 0; i < item.Split('¡').Length; i++)
                                {

                                    if (i == 0)
                                    {
                                        //BLOCKS_HEADER_INTERFACE bobj = new BLOCKS_HEADER_INTERFACE();
                                        var bobj = db.BLOCKS_HEADER_INTERFACEs.Where(x => x.BLOCK_HEADER_ID == Convert.ToDecimal(item.Split('¡')[i].Split('½')[0])).FirstOrDefault();
                                        if (bobj != null)
                                        {
                                            bobj.DIARY_HEADER_ID = DIARY_HEADER_ID;
                                            //bobj.PPA_BLOCK_HEADER_ID = Convert.ToInt32(item.Split('¡')[i].Split('½')[0]);
                                            bobj.bisRequired = Convert.ToInt32(item.Split('¡')[i].Split('½')[1]);
                                            bobj.RATE_VALID_FROM = ToDateTime(item.Split('¡')[i].Split('½')[4]);
                                            bobj.RATE_VALID_TO = ToDateTime(item.Split('¡')[i].Split('½')[5]);
                                            bobj.CLAIM_TOTAL = Convert.ToDecimal(item.Split('¡')[i].Split('½')[6]);
                                            bobj.ADVANCE_PAID = Convert.ToDecimal(item.Split('¡')[i].Split('½')[7]);
                                            bobj.REMAINING_CURRENT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[8]);
                                            bobj.ADVANCE_NEXT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[9]);
                                            bobj.PAYABLE_CURRENT_MONTH = Convert.ToDecimal(item.Split('¡')[i].Split('½')[10]);
                                            bobj.REMARKS = item.Split('¡')[i].Split('½')[11];
                                            //db.BLOCKS_HEADER_INTERFACEs.InsertOnSubmit(bobj);
                                            db.SubmitChanges();
                                            BLOCK_HEADER_ID = bobj.BLOCK_HEADER_ID;
                                        }

                                    }
                                    else //Update components update case
                                    {
                                        //COMP_HEADER_INTERFACE cobj = new COMP_HEADER_INTERFACE();
                                        var cobj = db.COMP_HEADER_INTERFACEs.Where(x => x.COM_HEADER_ID == Convert.ToDecimal(item.Split('¡')[i].Split('½')[1])).FirstOrDefault();
                                        if (cobj != null)
                                        {
                                            if (item.Split('¡')[i].Split('½')[0] == "tech")
                                            {
                                                cobj.IS_INCLUDE_IN_TOTAL = "N";
                                                cobj.BLOCK_HEADER_ID = BLOCK_HEADER_ID;
                                                cobj.COM_DEF_ID_FK = Convert.ToDouble(item.Split('¡')[i].Split('½')[2]);
                                                cobj.COMP_NAME = item.Split('¡')[i].Split('½')[3];
                                                cobj.COMP_VALUE = Convert.ToDecimal(item.Split('¡')[i].Split('½')[5]);
                                                cobj.REMARKS = item.Split('¡')[i].Split('½')[6];
                                                cobj.IS_MANDATORY = item.Split('¡')[i].Split('½')[8];

                                                //db.COMP_HEADER_INTERFACEs.InsertOnSubmit(cobj);
                                                if (cobj.BLOCK_HEADER_ID != 0 && cobj.BLOCK_HEADER_ID != null) // Parent (Block) with id = 0 shall not pass Aymen
                                                {
                                                    db.SubmitChanges();
                                                }
                                            }
                                            else //Update Fin Components update case
                                            {
                                                if (item.Split('¡')[i].Split('½')[8] == "Y")
                                                {
                                                    cobj.IS_INCLUDE_IN_TOTAL = "Y";
                                                }
                                                else
                                                {
                                                    cobj.IS_INCLUDE_IN_TOTAL = "N";
                                                }
                                                //cobj.IS_INCLUDE_IN_TOTAL = "Y";
                                                cobj.BLOCK_HEADER_ID = BLOCK_HEADER_ID;
                                                cobj.COM_DEF_ID_FK = Convert.ToDouble(item.Split('¡')[i].Split('½')[2]);
                                                cobj.COMP_NAME = item.Split('¡')[i].Split('½')[3];
                                                cobj.COMP_VALUE = Convert.ToDecimal(item.Split('¡')[i].Split('½')[5]);
                                                cobj.GST_PER = item.Split('¡')[i].Split('½')[6];
                                                cobj.REMARKS = item.Split('¡')[i].Split('½')[7];
                                                cobj.IS_MANDATORY = item.Split('¡')[i].Split('½')[9];

                                                //db.COMP_HEADER_INTERFACEs.InsertOnSubmit(cobj);
                                                if (cobj.BLOCK_HEADER_ID != 0 && cobj.BLOCK_HEADER_ID != null) // Parent (Block) with id = 0 shall not pass Aymen
                                                {
                                                    db.SubmitChanges();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (string item in filekData.Split('¡'))
                    {
                        if (item.Contains("½"))
                        {
                            decimal? AttachId = 0;
                            db.sp_Upload_FileNew(DIARY_HEADER_ID, item.Split('½')[1], item.Split('½')[3], item.Split('½')[4], item.Split('½')[2], item.Split('½')[1],
                           item.Split('½')[5], pkdatetime, Convert.ToString(UserID), ref AttachId);
                            //try
                            //{
                            //    string nm = item.Split('½')[2];
                            //    string ext = nm.Split('.')[nm.Split('.').Length - 1];
                            //    String path = HttpContext.Server.MapPath("~/Content/_FilesNew"); //Maintain Path for record purpose
                            //    string FilePath = Path.Combine(path, Convert.ToString(AttachId) + "." + ext);
                            //    if (!System.IO.File.Exists(FilePath))
                            //    {
                            //        byte[] FileBytes = Convert.FromBase64String(item.Split('½')[5]);
                            //        System.IO.File.WriteAllBytes(FilePath, FileBytes);
                            //    }
                            //}
                            //catch
                            //{


                            //}
                        }

                    }


                    if (dellblock.Trim().Length > 0)
                    {
                        foreach (var item in dellblock.Split('½'))
                        {


                            try
                            {
                                var f = db.BLOCKS_HEADER_INTERFACEs.Where(v => v.BLOCK_HEADER_ID == Convert.ToDecimal(item)).FirstOrDefault();
                                if (f != null)
                                {
                                    db.BLOCKS_HEADER_INTERFACEs.DeleteOnSubmit(f);
                                    db.SubmitChanges();
                                }
                            }
                            catch
                            {


                            }
                        }
                    }

                    if (fileDelData.Trim().Length > 0)
                    {
                        fileDelData = fileDelData.Replace("¼", "");

                        foreach (var item in fileDelData.Split('½'))
                        {


                            try
                            {
                                var f = db.ATTACHMENT_HEADERs.Where(v => v.ATTACH_HEADER_ID == Convert.ToDecimal(item)).FirstOrDefault();
                                if (f != null)
                                {
                                    f.bisDeleted = true;
                                    db.SubmitChanges();
                                }
                            }
                            catch
                            {


                            }
                        }
                    }

                    if (fileupdate.Length > 0)
                    {
                        foreach (var item in fileupdate.Split('¡'))
                        {
                            var fl = db.ATTACHMENT_HEADERs.Where(v => v.ATTACH_HEADER_ID == Convert.ToDecimal(item.Split('½')[0])).FirstOrDefault();
                            if (fl != null)
                            {
                                fl.ATTACHMENT_TITLE = item.Split('½')[1];
                                fl.ATTACHMENT_DESC = item.Split('½')[2];
                                db.SubmitChanges();
                            }
                        }
                    }


                }///END OF USING

                if (TransactionNumber != "")
                {
                    string Mailresult = "";
                    Task.Factory.StartNew<string>(() =>
                    SendMail(UserID, "Invoice Submitted", TransactionNumber))
            .ContinueWith(ant => Mailresult = ant.Result,
                          TaskScheduler.FromCurrentSynchronizationContext());
                }

                return strFormData;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }
        [HttpPost]
        public string UpdateRemarks()
        {

            // *****************************************************************************
            // 0    Diary Header ID
            // 1    Status
            // 2    Remarks
            // 3    Hard Copy Submision Date
            // 4    Hard Copy Submission Time
            // 5    Hard Copy Submission Type
            // *****************************************************************************
            string str = "1";
            try
            {
                int UserID = 0;
                string[] Values = null;
                
                UserID = Convert.ToInt32(Fn.ExenID(@"SELECT [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')"));
                var FormData = HttpContext.Request.Form["strFormData"];
                //var FormData = "3529½Engro Powergen Qadirpur Limited½1009½Draft½1085½14-Jun-2019½1111½0½14-Jun-2019½14-Jun-2019½00:00½14-Jun-2019½00:00½PKR½1,000.0000½½Draft½";
                Values = FormData.Split('½');
                // if PPA is not approved then user cannot receive
                string ppa_dtl;
                ppa_dtl = Fn.ExenID(@"select count(*) from CPPA_CA.PPA_HEADER  inner join CPPA_CA.DIARY_HEADER_INTERFACE on CPPA_CA.DIARY_HEADER_INTERFACE.ORGANIZATION_ID=CPPA_CA.PPA_HEADER.vendor_site_id_fk
                            where DIARY_HEADER_ID='" + Values[0] + "' and APPROVAL_STATUS='Approved'");
                if (ppa_dtl == "0")
                {
                    return "PPA is Incomplete! Kindly Change the PPA status.";
                }
                DateTime utc = DateTime.UtcNow;
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
                string Remarks = "½[" + Convert.ToString(TowDigit(pkdatetime.Day)) + "-" + monthNames[pkdatetime.Month - 1] + "-" + Convert.ToString(pkdatetime.Year) + " " + Convert.ToString(TowDigit(pkdatetime.Hour)) + ":" + Convert.ToString(TowDigit(pkdatetime.Minute)) + "]½" + Values[2];
                using (DBDataContext db = new DBDataContext())
                {

                    // ******************* User shoudl not be able to Receive GST invoice if Parent Invoice is not Receieved **********************
                    var invoiceData = db.DIARY_HEADER_INTERFACEs.Where(v => v.DIARY_HEADER_ID == Convert.ToInt32(Values[0])).FirstOrDefault();

                    if (Values[1] == "Received")
                        if (invoiceData.PARENT_INVOICE_NO != 0)
                        {
                            var parentInvoiceData = db.DIARY_HEADER_INTERFACEs.Where(v => v.DIARY_HEADER_ID == invoiceData.PARENT_INVOICE_NO).FirstOrDefault();

                            string FinanceApprovalStatus = "", TechnicalApprovalStatus = "";

                            if (string.IsNullOrEmpty(parentInvoiceData.IS_APPROVED_BY_FINANCE) || parentInvoiceData.IS_APPROVED_BY_FINANCE == "0")
                            {
                                FinanceApprovalStatus = "0";
                            }
                            else
                            {
                                FinanceApprovalStatus = parentInvoiceData.IS_APPROVED_BY_FINANCE;
                            }

                            if (string.IsNullOrEmpty(parentInvoiceData.IS_APPROVED_BY_TECHNICAL) || parentInvoiceData.IS_APPROVED_BY_TECHNICAL == "0")
                            {
                                TechnicalApprovalStatus = "0";
                            }
                            else
                            {
                                TechnicalApprovalStatus = parentInvoiceData.IS_APPROVED_BY_TECHNICAL;
                            }

                            if (FinanceApprovalStatus == "0" && TechnicalApprovalStatus == "1")
                            {
                                return "Parent Invoice is not Received by Finance User." + Environment.NewLine + "Please receive Parent Invoice first .... !";
                            }
                            else if (FinanceApprovalStatus == "1" && TechnicalApprovalStatus == "0")
                            {
                                return "Parent Invoice is not Received by Technical User." + Environment.NewLine + "Please receive Parent Invoice first .... !";
                            }
                            else if (FinanceApprovalStatus == "0" && TechnicalApprovalStatus == "0")
                            {
                                return "Parent Invoice is not Received by both Technical and Finance Users." + Environment.NewLine + "Please receive Parent Invoice first .... !";
                            }
                        }
                    // **************************** END Of Validation **************************************************

                    Remarks = Convert.ToString(UserID) + "½" + Convert.ToString(db.ApiUsers.Where(x => x.UserId == UserID).FirstOrDefault().Name) + Remarks;
                    if (Values[1] == "Returned")
                    {
                        db.uspDiaryUpdateRemak(Convert.ToDecimal(Values[0]), Remarks, UserID, pkdatetime, Values[1], Values[2].Trim().Length, " ", "");
                    }
                    else
                    {
                        db.uspDiaryUpdateRemak(Convert.ToDecimal(Values[0]), Remarks, UserID, pkdatetime, Values[1], Values[2].Trim().Length, Values[3] + " " + Values[4], Values[5]);
                    }

                }
                
                str += "½" + Fn.ExenID(@"SELECT APPROVED_STATUS from [CPPA_CA].[DIARY_HEADER_INTERFACE] where DIARY_HEADER_ID =" + Values[0]);
                str += "½" + Fn.ExenID(@"SELECT count(*) from [CPPA_CA].[ATTACHMENT_HEADER] where DIARY_HEADER_ID =" + Values[0]);
                string Mailresult = "";
                Task.Factory.StartNew<string>(() => SendMailReceiveReturn(UserID, Convert.ToString(FormData.Split('½')[1]), Convert.ToString(FormData.Split('½')[0])))
                .ContinueWith(ant => Mailresult = ant.Result,
                      TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
            return str;
        }
        public static DateTime ToDateTime(String value)
        {
            try
            {
                if (value == null || value == "")
                    return new DateTime(0);
                if (value.Split(' ').Length > 1)
                {
                    DateTime dt = DateTime.Parse(value.Split(' ')[0], CultureInfo.CurrentCulture);
                    DateTime rdt = new DateTime(dt.Year, dt.Month, dt.Day, Convert.ToInt32(value.Split(' ')[1].Split(':')[0]), Convert.ToInt32(value.Split(' ')[1].Split(':')[1]), 0);
                    return rdt;
                }
                else
                {
                    return DateTime.Parse(value, CultureInfo.CurrentCulture);
                }
            }
            catch
            {
                return new DateTime(0);

            }

        }
        private string TowDigit(int hour)
        {
            if (hour < 10)
            {
                return "0" + Convert.ToString(hour);
            }
            else
            {
                return Convert.ToString(hour);
            }
        }


        private string SendMail(int UserID, string Subject, string TransactionNo)
        {
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            //
            List<sp_DHI_Email_InfoNEWResult> amail = new List<sp_DHI_Email_InfoNEWResult>();
            string ret = "1";
            string Body = "";
            using (DBDataContext db = new DBDataContext())
            {
                amail = db.sp_DHI_Email_InfoNEW(Convert.ToInt64(TransactionNo), UserID, pkdatetime, "Submitted", "").ToList<sp_DHI_Email_InfoNEWResult>();
            }

            if (amail.Count > 0 && amail[0].Email.Length > 20)
            {
                string MailTo = amail[0].Email;
                Body = "<br>It is to inform you that an invoice of <strong>" + amail[0].INVOICE_TYPE + "</strong> from <strong>" + amail[0].VENDOR_NAME + @"</strong>, site <strong>" + amail[0].ADDRESS_LINE1 + @"</strong> at <strong>" + amail[0].DATETIME + @"</strong> has been submitted with Transaction <strong>#" + TransactionNo + "</strong><br><br><br>";
                string tmplt = @"<!DOCTYPE html PUBLIC ' -//W3C//DTD XHTML 1.0 Strict//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd'> <html xmlns='http://www.w3.org/1999/xhtml'> <head> <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /> <meta name='viewport' content='width=device-width, initial-scale=1.0'> <meta http-equiv='X-UA-Compatible' content='IE=edge,chrome=1'> <title>CPPA-G EMAIL</title> <style type='text/css'> /* reset */ article,aside,details,figcaption,figure,footer,header,hgroup,nav,section,summary{display:block}audio,canvas,video{display:inline-block;*display:inline;*zoom:1}audio:not([controls]){display:none;height:0}[hidden]{display:none}html{font-size:100%;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%}html,button,input,select,textarea{font-family:sans-serif}body{margin:0}a:focus{outline:thin dotted}a:active,a:hover{outline:0}h1{font-size:2em;margin:0 0 0}h2{font-size:1.5em;margin:0 0 .83em 0}h3{font-size:1.17em;margin:1em 0}h4{font-size:1em;margin:1.33em 0}h5{font-size:.83em;margin:1.67em 0}h6{font-size:.75em;margin:2.33em 0}abbr[title]{border-bottom:1px dotted}b,strong{font-weight:bold}blockquote{margin:1em 40px}dfn{font-style:italic}mark{background:#ff0;color:#000}p,pre{margin:1em 0}code,kbd,pre,samp{font-family:monospace,serif;_font-family:'courier new',monospace;font-size:1em}pre{white-space:pre;white-space:pre-wrap;word-wrap:break-word}q{quotes:none}q:before,q:after{content:'';content:none}small{font-size:75%}sub,sup{font-size:75%;line-height:0;position:relative;vertical-align:baseline}sup{top:-0.5em}sub{bottom:-0.25em}dl,menu,ol,ul{margin:1em 0}dd{margin:0 0 0 40px}menu,ol,ul{padding:0 0 0 40px}nav ul,nav ol{list-style:none;list-style-image:none}img{border:0;-ms-interpolation-mode:bicubic}svg:not(:root){overflow:hidden}figure{margin:0}form{margin:0}fieldset{border:1px solid #c0c0c0;margin:0 2px;padding:.35em .625em .75em}legend{border:0;padding:0;white-space:normal;*margin-left:-7px}button,input,select,textarea{font-size:100%;margin:0;vertical-align:baseline;*vertical-align:middle}button,input{line-height:normal}button,html input[type='button'],input[type='reset'],input[type='submit']{-webkit-appearance:button;cursor:pointer;*overflow:visible}button[disabled],input[disabled]{cursor:default}input[type='checkbox'],input[type='radio']{box-sizing:border-box;padding:0;*height:13px;*width:13px}input[type='search']{-webkit-appearance:textfield;-moz-box-sizing:content-box;-webkit-box-sizing:content-box;box-sizing:content-box}input[type='search']::-webkit-search-cancel-button,input[type='search']::-webkit-search-decoration{-webkit-appearance:none}button::-moz-focus-inner,input::-moz-focus-inner{border:0;padding:0}textarea{overflow:auto;vertical-align:top}table{border-collapse:collapse;border-spacing:0} /* custom client-specific styles including styles for different online clients */ .ReadMsgBody{width:100%;} .ExternalClass{width:100%;} /* hotmail / outlook.com */ .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div{line-height:100%;} /* hotmail / outlook.com */ table, td{mso-table-lspace:0pt; mso-table-rspace:0pt;} /* Outlook */ #outlook a{padding:0;} /* Outlook */ img{-ms-interpolation-mode: bicubic;display:block;outline:none; text-decoration:none;} /* IExplorer */ body, table, td, p, a, li, blockquote{-ms-text-size-adjust:100%; -webkit-text-size-adjust:100%; font-weight:normal!important;} .ExternalClass td[class='ecxflexibleContainerBox'] h3 {padding-top: 10px !important;} /* hotmail */ /* email template styles */ h1{display:block;font-size:26px;font-style:normal;font-weight:normal;line-height:100%;} h2{display:block;font-size:20px;font-style:normal;font-weight:normal;line-height:120%;} h3{display:block;font-size:17px;font-style:normal;font-weight:normal;line-height:110%;} h4{display:block;font-size:18px;font-style:italic;font-weight:normal;line-height:100%;} .flexibleImage{height:auto;} br{line-height:0%;} table[class=flexibleContainerCellDivider] {padding-bottom:0 !important;padding-top:0 !important;} body, #bodyTbl{background-color:#E1E1E1;} #emailHeader{background-color:#E1E1E1;} #emailBody{background-color:#FFFFFF;} #emailFooter{background-color:#E1E1E1;} .textContent {color:#8B8B8B; font-family:Helvetica; font-size:16px; line-height:125%; text-align:Left;} .textContent a{color:#205478; text-decoration:underline;} .emailButton{background-color:#205478; border-collapse:separate;} .buttonContent{color:#FFFFFF; font-family:Helvetica; font-size:18px; font-weight:bold; line-height:100%; padding:15px; text-align:center;} .buttonContent a{color:#FFFFFF; display:block; text-decoration:none!important; border:0!important;} #invisibleIntroduction {display:none;display:none !important;} /* hide the introduction text */ /* other framework hacks and overrides */ span[class=ios-color-hack] a {color:#275100!important;text-decoration:none!important;} /* Remove all link colors in IOS (below are duplicates based on the color preference) */ span[class=ios-color-hack2] a {color:#205478!important;text-decoration:none!important;} span[class=ios-color-hack3] a {color:#8B8B8B!important;text-decoration:none!important;} /* phones and sms */ .a[href^='tel'], a[href^='sms'] {text-decoration:none!important;color:#606060!important;pointer-events:none!important;cursor:default!important;} .mobile_link a[href^='tel'], .mobile_link a[href^='sms'] {text-decoration:none!important;color:#606060!important;pointer-events:auto!important;cursor:default!important;} /* responsive styles */ @media only screen and (max-width: 480px){ body{width:100% !important; min-width:100% !important;} table[id='emailHeader'], table[id='emailBody'], table[id='emailFooter'], table[class='flexibleContainer'] {width:100% !important;} td[class='flexibleContainerBox'], td[class='flexibleContainerBox'] table {display: block;width: 100%;text-align: left;} td[class='imageContent'] img {height:auto !important; width:100% !important; max-width:100% !important;} img[class='flexibleImage']{height:auto !important; width:100% !important;max-width:100% !important;} img[class='flexibleImageSmall']{height:auto !important; width:auto !important;} table[class='flexibleContainerBoxNext']{padding-top: 10px !important;} table[class='emailButton']{width:100% !important;} td[class='buttonContent']{padding:0 !important;} td[class='buttonContent'] a{padding:15px !important;} br{line-height:0%;} } </style> <!-- MS Outlook custom styles --> <!--[if mso 12]> <style type='text/css'> .flexibleContainer{display:block !important; width:100% !important;} </style> <![endif]--> <!--[if mso 14]> <style type='text/css'> .flexibleContainer{display:block !important; width:100% !important;} </style> <![endif]--> </head> <body bgcolor='#E1E1E1' leftmargin='0' marginwidth='0' topmargin='0' marginheight='0' offset='0'> <center style='background-color:#E1E1E1;'> <table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%' id='bodyTbl' style='table-layout: fixed;max-width:100% !important;width: 100% !important;min-width: 100% !important;'> <tr> <td align='center' valign='top' id='bodyCell'> <div style='font-family:Helvetica,Arial,sans-serif;font-size:13px;color:#828282;text-align:center;line-height:120%;'> If you have any troubles, you can see this email <a href='#' target='_blank' style='text-decoration:none;border-bottom:1px solid #828282;color:#828282;'><span style='color:#828282;'>in your browser</span></a>. </div> <table style='border: none;border-spacing: 0px;'> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='80%' style='color:#FFFFFF;' bgcolor='#3498db'> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='100%' class='flexibleContainer'> <tr> <td align='center' valign='top' width='100%' class='flexibleContainerCell'> <table border='0' cellpadding='10' cellspacing='0' width='100%'> <tr> <td align='center' valign='top' class='textContent'> <h1 style='color:#FFFFFF;line-height:100%;font-family:Helvetica,Arial,sans-serif;font-size:35px;font-weight:normal;text-align:left;'>CPPA-G</h1> <h2 style='text-align:left;font-weight:normal;font-family:Helvetica,Arial,sans-serif;font-size:23px;margin-bottom:5px;color:#fff;line-height:135%;background:#3498db;padding: 0;'>Central Power Purchasing Agency(G) Ltd.</h2> <div style='text-align:left;font-family:Helvetica,Arial,sans-serif;font-size:15px;margin-bottom:0;color:#000;line-height:135%;'>'To become a world-class power market operator by providing the optimum environment for trading electricity in the Pakistani power market'</div> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> </td> </tr> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='80%' bgcolor='#FFFFFF'> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='100%' class='flexibleContainer'> <tr> <td align='center' valign='top' width='100%' class='flexibleContainerCell'> <table border='0' cellpadding='10' cellspacing='0' width='100%'> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='100%' style='margin-bottom:20px;'> <tr> <td valign='top' class='textContent'> <h3 style='color:#5F5F5F;line-height:125%;font-family:Helvetica,Arial,sans-serif;font-size:20px;font-weight:normal;margin-top:0;margin-bottom:3px;text-align:left;text-decoration: underline;'>" + Subject + @"</h3> <div style='text-align:left;font-family:Helvetica,Arial,sans-serif;font-size:15px;margin-bottom:0;color:#5F5F5F;line-height:135%;'>" + Body + @"</div> </td> </tr> </table> <table border='0' cellpadding='0' cellspacing='0' width='50%' class='emailButton' style='background-color: #3498DB;'> <tr> <td align='center' valign='middle' class='buttonContent' style='padding-top:15px;padding-bottom:15px;padding-right:15px;padding-left:15px;'> <a style='color:#FFFFFF;text-decoration:none;font-family:Helvetica,Arial,sans-serif;font-size:20px;line-height:135%;' href='https://cppapk.sharepoint.com' target='_blank'>Go to CPPA-G Web Portal</a> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> <table bgcolor='#E1E1E1' border='0' cellpadding='0' cellspacing='0' width='80%' id='emailFooter'> <tr> <td valign='top' bgcolor='#E1E1E1'> <div style='font-family:Helvetica,Arial,sans-serif;font-size:13px;color:#828282;text-align:center;line-height:120%;'> <div>Copyright &#169; 2019. All rights reserved.</div> <div>If you don't want to receive these emails from us in the future, please <a href='#' target='_blank' style='text-decoration:none;color:#828282;'><span style='color:#828282;'>unsubscribe</span></a></div> </div> </td> </tr> </table> </td> </tr> </table> </center> </body> </html>";

                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.To.Add(MailTo);
                //mail.Bcc.Add("muhammad.usman@cppa.gov.pk");
                mail.From = new MailAddress("erpautoemail@cppa.gov.pk", "CPPA-G Invoice Notification");
                mail.Subject = Subject;
                mail.Body = tmplt;
                mail.IsBodyHtml = true;



                //string FileName = Path.GetFileName(FileUploadAttachments.PostedFile.FileName);
                //Attachment attachment = new Attachment(FileUploadAttachments.PostedFile.InputStream, FileName);
                //mail.Attachments.Add(attachment);

                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("erpautoemail@cppa.gov.pk", "Cpp@erp123");
                client.Host = "mail.cppa.gov.pk";
                client.Port = 25;

                try
                {
                    client.Send(mail);
                }
                catch (Exception ex)
                {
                    ret = ex.Message;
                }
            }


            return ret;
        }

        //----- for ECM MS Flow error-----
        [HttpPost]
        public string CreateECMMail()
        {
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string ret = "1";
            string Body = "";
            int dhi = Convert.ToInt32(HttpContext.Request.Form["diary_id"]);
            string vendor_name = HttpContext.Request.Form["Vname"];
            int VSID = Convert.ToInt32(HttpContext.Request.Form["VsiteID"]);
            string MailTo = "alina.javed@cppa.gov.pk, ahmad.ehsan@cppa.gov.pk";
            Body = "<br>It is to inform you that Microsoft Flow for <strong>" + vendor_name + @"</strong> at <strong>" + pkdatetime + @"</strong> has failed to update link having diary_Header_id <strong>#" + dhi + "</strong> and SITE_ID <strong>#" + VSID + "</strong><br><br><br>";
            string tmplt = @"<!DOCTYPE html PUBLIC ' -//W3C//DTD XHTML 1.0 Strict//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd'> <html xmlns='http://www.w3.org/1999/xhtml'> <head> <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /> <meta name='viewport' content='width=device-width, initial-scale=1.0'> <meta http-equiv='X-UA-Compatible' content='IE=edge,chrome=1'> <title>CPPA-G EMAIL</title> <style type='text/css'> /* reset */ article,aside,details,figcaption,figure,footer,header,hgroup,nav,section,summary{display:block}audio,canvas,video{display:inline-block;*display:inline;*zoom:1}audio:not([controls]){display:none;height:0}[hidden]{display:none}html{font-size:100%;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%}html,button,input,select,textarea{font-family:sans-serif}body{margin:0}a:focus{outline:thin dotted}a:active,a:hover{outline:0}h1{font-size:2em;margin:0 0 0}h2{font-size:1.5em;margin:0 0 .83em 0}h3{font-size:1.17em;margin:1em 0}h4{font-size:1em;margin:1.33em 0}h5{font-size:.83em;margin:1.67em 0}h6{font-size:.75em;margin:2.33em 0}abbr[title]{border-bottom:1px dotted}b,strong{font-weight:bold}blockquote{margin:1em 40px}dfn{font-style:italic}mark{background:#ff0;color:#000}p,pre{margin:1em 0}code,kbd,pre,samp{font-family:monospace,serif;_font-family:'courier new',monospace;font-size:1em}pre{white-space:pre;white-space:pre-wrap;word-wrap:break-word}q{quotes:none}q:before,q:after{content:'';content:none}small{font-size:75%}sub,sup{font-size:75%;line-height:0;position:relative;vertical-align:baseline}sup{top:-0.5em}sub{bottom:-0.25em}dl,menu,ol,ul{margin:1em 0}dd{margin:0 0 0 40px}menu,ol,ul{padding:0 0 0 40px}nav ul,nav ol{list-style:none;list-style-image:none}img{border:0;-ms-interpolation-mode:bicubic}svg:not(:root){overflow:hidden}figure{margin:0}form{margin:0}fieldset{border:1px solid #c0c0c0;margin:0 2px;padding:.35em .625em .75em}legend{border:0;padding:0;white-space:normal;*margin-left:-7px}button,input,select,textarea{font-size:100%;margin:0;vertical-align:baseline;*vertical-align:middle}button,input{line-height:normal}button,html input[type='button'],input[type='reset'],input[type='submit']{-webkit-appearance:button;cursor:pointer;*overflow:visible}button[disabled],input[disabled]{cursor:default}input[type='checkbox'],input[type='radio']{box-sizing:border-box;padding:0;*height:13px;*width:13px}input[type='search']{-webkit-appearance:textfield;-moz-box-sizing:content-box;-webkit-box-sizing:content-box;box-sizing:content-box}input[type='search']::-webkit-search-cancel-button,input[type='search']::-webkit-search-decoration{-webkit-appearance:none}button::-moz-focus-inner,input::-moz-focus-inner{border:0;padding:0}textarea{overflow:auto;vertical-align:top}table{border-collapse:collapse;border-spacing:0} /* custom client-specific styles including styles for different online clients */ .ReadMsgBody{width:100%;} .ExternalClass{width:100%;} /* hotmail / outlook.com */ .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div{line-height:100%;} /* hotmail / outlook.com */ table, td{mso-table-lspace:0pt; mso-table-rspace:0pt;} /* Outlook */ #outlook a{padding:0;} /* Outlook */ img{-ms-interpolation-mode: bicubic;display:block;outline:none; text-decoration:none;} /* IExplorer */ body, table, td, p, a, li, blockquote{-ms-text-size-adjust:100%; -webkit-text-size-adjust:100%; font-weight:normal!important;} .ExternalClass td[class='ecxflexibleContainerBox'] h3 {padding-top: 10px !important;} /* hotmail */ /* email template styles */ h1{display:block;font-size:26px;font-style:normal;font-weight:normal;line-height:100%;} h2{display:block;font-size:20px;font-style:normal;font-weight:normal;line-height:120%;} h3{display:block;font-size:17px;font-style:normal;font-weight:normal;line-height:110%;} h4{display:block;font-size:18px;font-style:italic;font-weight:normal;line-height:100%;} .flexibleImage{height:auto;} br{line-height:0%;} table[class=flexibleContainerCellDivider] {padding-bottom:0 !important;padding-top:0 !important;} body, #bodyTbl{background-color:#E1E1E1;} #emailHeader{background-color:#E1E1E1;} #emailBody{background-color:#FFFFFF;} #emailFooter{background-color:#E1E1E1;} .textContent {color:#8B8B8B; font-family:Helvetica; font-size:16px; line-height:125%; text-align:Left;} .textContent a{color:#205478; text-decoration:underline;} .emailButton{background-color:#205478; border-collapse:separate;} .buttonContent{color:#FFFFFF; font-family:Helvetica; font-size:18px; font-weight:bold; line-height:100%; padding:15px; text-align:center;} .buttonContent a{color:#FFFFFF; display:block; text-decoration:none!important; border:0!important;} #invisibleIntroduction {display:none;display:none !important;} /* hide the introduction text */ /* other framework hacks and overrides */ span[class=ios-color-hack] a {color:#275100!important;text-decoration:none!important;} /* Remove all link colors in IOS (below are duplicates based on the color preference) */ span[class=ios-color-hack2] a {color:#205478!important;text-decoration:none!important;} span[class=ios-color-hack3] a {color:#8B8B8B!important;text-decoration:none!important;} /* phones and sms */ .a[href^='tel'], a[href^='sms'] {text-decoration:none!important;color:#606060!important;pointer-events:none!important;cursor:default!important;} .mobile_link a[href^='tel'], .mobile_link a[href^='sms'] {text-decoration:none!important;color:#606060!important;pointer-events:auto!important;cursor:default!important;} /* responsive styles */ @media only screen and (max-width: 480px){ body{width:100% !important; min-width:100% !important;} table[id='emailHeader'], table[id='emailBody'], table[id='emailFooter'], table[class='flexibleContainer'] {width:100% !important;} td[class='flexibleContainerBox'], td[class='flexibleContainerBox'] table {display: block;width: 100%;text-align: left;} td[class='imageContent'] img {height:auto !important; width:100% !important; max-width:100% !important;} img[class='flexibleImage']{height:auto !important; width:100% !important;max-width:100% !important;} img[class='flexibleImageSmall']{height:auto !important; width:auto !important;} table[class='flexibleContainerBoxNext']{padding-top: 10px !important;} table[class='emailButton']{width:100% !important;} td[class='buttonContent']{padding:0 !important;} td[class='buttonContent'] a{padding:15px !important;} br{line-height:0%;} } </style> <!-- MS Outlook custom styles --> <!--[if mso 12]> <style type='text/css'> .flexibleContainer{display:block !important; width:100% !important;} </style> <![endif]--> <!--[if mso 14]> <style type='text/css'> .flexibleContainer{display:block !important; width:100% !important;} </style> <![endif]--> </head> <body bgcolor='#E1E1E1' leftmargin='0' marginwidth='0' topmargin='0' marginheight='0' offset='0'> <center style='background-color:#E1E1E1;'> <table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%' id='bodyTbl' style='table-layout: fixed;max-width:100% !important;width: 100% !important;min-width: 100% !important;'> <tr> <td align='center' valign='top' id='bodyCell'> <div style='font-family:Helvetica,Arial,sans-serif;font-size:13px;color:#828282;text-align:center;line-height:120%;'> If you have any troubles, you can see this email <a href='#' target='_blank' style='text-decoration:none;border-bottom:1px solid #828282;color:#828282;'><span style='color:#828282;'>in your browser</span></a>. </div> <table style='border: none;border-spacing: 0px;'> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='80%' style='color:#FFFFFF;' bgcolor='#3498db'> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='100%' class='flexibleContainer'> <tr> <td align='center' valign='top' width='100%' class='flexibleContainerCell'> <table border='0' cellpadding='10' cellspacing='0' width='100%'> <tr> <td align='center' valign='top' class='textContent'> <h1 style='color:#FFFFFF;line-height:100%;font-family:Helvetica,Arial,sans-serif;font-size:35px;font-weight:normal;text-align:left;'>CPPA-G</h1> <h2 style='text-align:left;font-weight:normal;font-family:Helvetica,Arial,sans-serif;font-size:23px;margin-bottom:5px;color:#fff;line-height:135%;background:#3498db;padding: 0;'>Central Power Purchasing Agency(G) Ltd.</h2> <div style='text-align:left;font-family:Helvetica,Arial,sans-serif;font-size:15px;margin-bottom:0;color:#000;line-height:135%;'>'To become a world-class power market operator by providing the optimum environment for trading electricity in the Pakistani power market'</div> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> </td> </tr> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='80%' bgcolor='#FFFFFF'> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='100%' class='flexibleContainer'> <tr> <td align='center' valign='top' width='100%' class='flexibleContainerCell'> <table border='0' cellpadding='10' cellspacing='0' width='100%'> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='100%' style='margin-bottom:20px;'> <tr> <td valign='top' class='textContent'> <h3 style='color:#5F5F5F;line-height:125%;font-family:Helvetica,Arial,sans-serif;font-size:20px;font-weight:normal;margin-top:0;margin-bottom:3px;text-align:left;text-decoration: underline;'>WorkFlow Error</h3> <div style='text-align:left;font-family:Helvetica,Arial,sans-serif;font-size:15px;margin-bottom:0;color:#5F5F5F;line-height:135%;'>" + Body + @"</div> </td> </tr> </table> <table border='0' cellpadding='0' cellspacing='0' width='50%' class='emailButton' style='background-color: #3498DB;'> <tr> <td align='center' valign='middle' class='buttonContent' style='padding-top:15px;padding-bottom:15px;padding-right:15px;padding-left:15px;'> <a style='color:#FFFFFF;text-decoration:none;font-family:Helvetica,Arial,sans-serif;font-size:20px;line-height:135%;' href='https://cppapk.sharepoint.com' target='_blank'>Go to CPPA-G Web Portal</a> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> <table bgcolor='#E1E1E1' border='0' cellpadding='0' cellspacing='0' width='80%' id='emailFooter'> <tr> <td valign='top' bgcolor='#E1E1E1'> <div style='font-family:Helvetica,Arial,sans-serif;font-size:13px;color:#828282;text-align:center;line-height:120%;'> <div>Copyright &#169; 2019. All rights reserved.</div> <div>If you don't want to receive these emails from us in the future, please <a href='#' target='_blank' style='text-decoration:none;color:#828282;'><span style='color:#828282;'>unsubscribe</span></a></div> </div> </td> </tr> </table> </td> </tr> </table> </center> </body> </html>";

            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.To.Add(MailTo);
            //mail.Bcc.Add("muhammad.usman@cppa.gov.pk");
            mail.From = new MailAddress("erpautoemail@cppa.gov.pk", "CPPA-G Attachment failure Notification");
            mail.Subject = "WorkFlow Error";
            mail.Body = tmplt;
            mail.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("erpautoemail@cppa.gov.pk", "Cpp@erp123");
            client.Host = "mail.cppa.gov.pk";
            client.Port = 25;
            try
            {
                client.Send(mail);
                //Fn.WriteToFile("Error in sending attachment : " + "diary_id =" + dhi + ", Vendor_name=" + vendor_name + " , VsiteID =" + VSID);
            }
            catch (Exception ex)
            {
                ret = ex.Message;
                //Fn.WriteToFile("Error in sending attachment : " + "diary_id =" + dhi + ", Vendor_name=" + vendor_name + " , VsiteID =" + VSID);
            }
            return ret;
        }
        private string SendMailReceiveReturn(int UserID, string Subject, string DIARY_HEADER_ID)
        {
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            //
            List<sp_DHI_Email_InfoReceiveReturnResult> amail = new List<sp_DHI_Email_InfoReceiveReturnResult>();
            string ret = "1";
            string Body = "";
            using (DBDataContext db = new DBDataContext())
            {
                amail = db.sp_DHI_Email_InfoReceiveReturn(Convert.ToDecimal(DIARY_HEADER_ID), UserID, pkdatetime, Subject, "").ToList<sp_DHI_Email_InfoReceiveReturnResult>();
            }

            //if (amail[0].IS_APPROVED_BY_FINANCE == null)
            //{
            //    return ret;
            //}
            //if (amail[0].IS_APPROVED_BY_TECHNICAL == null)
            //{
            //    return ret;
            //}
            //if (Subject == "Received" && (amail[0].IS_APPROVED_BY_FINANCE == "0" || amail[0].IS_APPROVED_BY_TECHNICAL == "0"))
            //{
            //    Subject = "Returned";
            //}


            if (amail[0].APPROVED_STATUS == "Received")
            {
                Subject = "Received";
            }
            else if (amail[0].APPROVED_STATUS == "Returned")
            {

                Subject = "Returned";
            }
            else
            {
                return ret;
            }


            if (amail.Count > 0 && amail[0].Email.Length > 10)
            {
                string MailTo = amail[0].Email;
                Body = "<br>It is to inform you that an invoice of <strong>" + amail[0].INVOICE_TYPE + "</strong> from <strong>" + amail[0].VENDOR_NAME + @"</strong>, site <strong>" + amail[0].ADDRESS_LINE1 + @"</strong> at <strong>" + amail[0].DATETIME + @"</strong> has been " + Subject + @" by CPPA-G with Transaction <strong>#" + Convert.ToString(amail[0].TransactionNumber) + "</strong><br><br><br>";
                
                string tmplt = @"<!DOCTYPE html PUBLIC ' -//W3C//DTD XHTML 1.0 Strict//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd'> <html xmlns='http://www.w3.org/1999/xhtml'> <head> <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /> <meta name='viewport' content='width=device-width, initial-scale=1.0'> <meta http-equiv='X-UA-Compatible' content='IE=edge,chrome=1'> <title>CPPA-G EMAIL</title> <style type='text/css'> /* reset */ article,aside,details,figcaption,figure,footer,header,hgroup,nav,section,summary{display:block}audio,canvas,video{display:inline-block;*display:inline;*zoom:1}audio:not([controls]){display:none;height:0}[hidden]{display:none}html{font-size:100%;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%}html,button,input,select,textarea{font-family:sans-serif}body{margin:0}a:focus{outline:thin dotted}a:active,a:hover{outline:0}h1{font-size:2em;margin:0 0 0}h2{font-size:1.5em;margin:0 0 .83em 0}h3{font-size:1.17em;margin:1em 0}h4{font-size:1em;margin:1.33em 0}h5{font-size:.83em;margin:1.67em 0}h6{font-size:.75em;margin:2.33em 0}abbr[title]{border-bottom:1px dotted}b,strong{font-weight:bold}blockquote{margin:1em 40px}dfn{font-style:italic}mark{background:#ff0;color:#000}p,pre{margin:1em 0}code,kbd,pre,samp{font-family:monospace,serif;_font-family:'courier new',monospace;font-size:1em}pre{white-space:pre;white-space:pre-wrap;word-wrap:break-word}q{quotes:none}q:before,q:after{content:'';content:none}small{font-size:75%}sub,sup{font-size:75%;line-height:0;position:relative;vertical-align:baseline}sup{top:-0.5em}sub{bottom:-0.25em}dl,menu,ol,ul{margin:1em 0}dd{margin:0 0 0 40px}menu,ol,ul{padding:0 0 0 40px}nav ul,nav ol{list-style:none;list-style-image:none}img{border:0;-ms-interpolation-mode:bicubic}svg:not(:root){overflow:hidden}figure{margin:0}form{margin:0}fieldset{border:1px solid #c0c0c0;margin:0 2px;padding:.35em .625em .75em}legend{border:0;padding:0;white-space:normal;*margin-left:-7px}button,input,select,textarea{font-size:100%;margin:0;vertical-align:baseline;*vertical-align:middle}button,input{line-height:normal}button,html input[type='button'],input[type='reset'],input[type='submit']{-webkit-appearance:button;cursor:pointer;*overflow:visible}button[disabled],input[disabled]{cursor:default}input[type='checkbox'],input[type='radio']{box-sizing:border-box;padding:0;*height:13px;*width:13px}input[type='search']{-webkit-appearance:textfield;-moz-box-sizing:content-box;-webkit-box-sizing:content-box;box-sizing:content-box}input[type='search']::-webkit-search-cancel-button,input[type='search']::-webkit-search-decoration{-webkit-appearance:none}button::-moz-focus-inner,input::-moz-focus-inner{border:0;padding:0}textarea{overflow:auto;vertical-align:top}table{border-collapse:collapse;border-spacing:0} /* custom client-specific styles including styles for different online clients */ .ReadMsgBody{width:100%;} .ExternalClass{width:100%;} /* hotmail / outlook.com */ .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div{line-height:100%;} /* hotmail / outlook.com */ table, td{mso-table-lspace:0pt; mso-table-rspace:0pt;} /* Outlook */ #outlook a{padding:0;} /* Outlook */ img{-ms-interpolation-mode: bicubic;display:block;outline:none; text-decoration:none;} /* IExplorer */ body, table, td, p, a, li, blockquote{-ms-text-size-adjust:100%; -webkit-text-size-adjust:100%; font-weight:normal!important;} .ExternalClass td[class='ecxflexibleContainerBox'] h3 {padding-top: 10px !important;} /* hotmail */ /* email template styles */ h1{display:block;font-size:26px;font-style:normal;font-weight:normal;line-height:100%;} h2{display:block;font-size:20px;font-style:normal;font-weight:normal;line-height:120%;} h3{display:block;font-size:17px;font-style:normal;font-weight:normal;line-height:110%;} h4{display:block;font-size:18px;font-style:italic;font-weight:normal;line-height:100%;} .flexibleImage{height:auto;} br{line-height:0%;} table[class=flexibleContainerCellDivider] {padding-bottom:0 !important;padding-top:0 !important;} body, #bodyTbl{background-color:#E1E1E1;} #emailHeader{background-color:#E1E1E1;} #emailBody{background-color:#FFFFFF;} #emailFooter{background-color:#E1E1E1;} .textContent {color:#8B8B8B; font-family:Helvetica; font-size:16px; line-height:125%; text-align:Left;} .textContent a{color:#205478; text-decoration:underline;} .emailButton{background-color:#205478; border-collapse:separate;} .buttonContent{color:#FFFFFF; font-family:Helvetica; font-size:18px; font-weight:bold; line-height:100%; padding:15px; text-align:center;} .buttonContent a{color:#FFFFFF; display:block; text-decoration:none!important; border:0!important;} #invisibleIntroduction {display:none;display:none !important;} /* hide the introduction text */ /* other framework hacks and overrides */ span[class=ios-color-hack] a {color:#275100!important;text-decoration:none!important;} /* Remove all link colors in IOS (below are duplicates based on the color preference) */ span[class=ios-color-hack2] a {color:#205478!important;text-decoration:none!important;} span[class=ios-color-hack3] a {color:#8B8B8B!important;text-decoration:none!important;} /* phones and sms */ .a[href^='tel'], a[href^='sms'] {text-decoration:none!important;color:#606060!important;pointer-events:none!important;cursor:default!important;} .mobile_link a[href^='tel'], .mobile_link a[href^='sms'] {text-decoration:none!important;color:#606060!important;pointer-events:auto!important;cursor:default!important;} /* responsive styles */ @media only screen and (max-width: 480px){ body{width:100% !important; min-width:100% !important;} table[id='emailHeader'], table[id='emailBody'], table[id='emailFooter'], table[class='flexibleContainer'] {width:100% !important;} td[class='flexibleContainerBox'], td[class='flexibleContainerBox'] table {display: block;width: 100%;text-align: left;} td[class='imageContent'] img {height:auto !important; width:100% !important; max-width:100% !important;} img[class='flexibleImage']{height:auto !important; width:100% !important;max-width:100% !important;} img[class='flexibleImageSmall']{height:auto !important; width:auto !important;} table[class='flexibleContainerBoxNext']{padding-top: 10px !important;} table[class='emailButton']{width:100% !important;} td[class='buttonContent']{padding:0 !important;} td[class='buttonContent'] a{padding:15px !important;} br{line-height:0%;} } </style> <!-- MS Outlook custom styles --> <!--[if mso 12]> <style type='text/css'> .flexibleContainer{display:block !important; width:100% !important;} </style> <![endif]--> <!--[if mso 14]> <style type='text/css'> .flexibleContainer{display:block !important; width:100% !important;} </style> <![endif]--> </head> <body bgcolor='#E1E1E1' leftmargin='0' marginwidth='0' topmargin='0' marginheight='0' offset='0'> <center style='background-color:#E1E1E1;'> <table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%' id='bodyTbl' style='table-layout: fixed;max-width:100% !important;width: 100% !important;min-width: 100% !important;'> <tr> <td align='center' valign='top' id='bodyCell'> <div style='font-family:Helvetica,Arial,sans-serif;font-size:13px;color:#828282;text-align:center;line-height:120%;'> If you have any troubles, you can see this email <a href='#' target='_blank' style='text-decoration:none;border-bottom:1px solid #828282;color:#828282;'><span style='color:#828282;'>in your browser</span></a>. </div> <table style='border: none;border-spacing: 0px;'> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='80%' style='color:#FFFFFF;' bgcolor='#3498db'> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='100%' class='flexibleContainer'> <tr> <td align='center' valign='top' width='100%' class='flexibleContainerCell'> <table border='0' cellpadding='10' cellspacing='0' width='100%'> <tr> <td align='center' valign='top' class='textContent'> <h1 style='color:#FFFFFF;line-height:100%;font-family:Helvetica,Arial,sans-serif;font-size:35px;font-weight:normal;text-align:left;'>CPPA-G</h1> <h2 style='text-align:left;font-weight:normal;font-family:Helvetica,Arial,sans-serif;font-size:23px;margin-bottom:5px;color:#fff;line-height:135%;background:#3498db;padding: 0;'>Central Power Purchasing Agency(G) Ltd.</h2> <div style='text-align:left;font-family:Helvetica,Arial,sans-serif;font-size:15px;margin-bottom:0;color:#000;line-height:135%;'>'To become a world-class power market operator by providing the optimum environment for trading electricity in the Pakistani power market'</div> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> </td> </tr> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='80%' bgcolor='#FFFFFF'> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='100%' class='flexibleContainer'> <tr> <td align='center' valign='top' width='100%' class='flexibleContainerCell'> <table border='0' cellpadding='10' cellspacing='0' width='100%'> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='100%' style='margin-bottom:20px;'> <tr> <td valign='top' class='textContent'> <h3 style='color:#5F5F5F;line-height:125%;font-family:Helvetica,Arial,sans-serif;font-size:20px;font-weight:normal;margin-top:0;margin-bottom:3px;text-align:left;text-decoration: underline;'>Invoice " + Subject + @"</h3> <div style='text-align:left;font-family:Helvetica,Arial,sans-serif;font-size:15px;margin-bottom:0;color:#5F5F5F;line-height:135%;'>" + Body + @"</div> </td> </tr> </table> <table border='0' cellpadding='0' cellspacing='0' width='50%' class='emailButton' style='background-color: #3498DB;'> <tr> <td align='center' valign='middle' class='buttonContent' style='padding-top:15px;padding-bottom:15px;padding-right:15px;padding-left:15px;'> <a style='color:#FFFFFF;text-decoration:none;font-family:Helvetica,Arial,sans-serif;font-size:20px;line-height:135%;' href='https://cppapk.sharepoint.com' target='_blank'>Go to CPPA-G Web Portal</a> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> <table bgcolor='#E1E1E1' border='0' cellpadding='0' cellspacing='0' width='80%' id='emailFooter'> <tr> <td valign='top' bgcolor='#E1E1E1'> <div style='font-family:Helvetica,Arial,sans-serif;font-size:13px;color:#828282;text-align:center;line-height:120%;'> <div>Copyright &#169; 2019. All rights reserved.</div> <div>If you don't want to receive these emails from us in the future, please <a href='#' target='_blank' style='text-decoration:none;color:#828282;'><span style='color:#828282;'>unsubscribe</span></a></div> </div> </td> </tr> </table> </td> </tr> </table> </center> </body> </html>";

                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.To.Add(MailTo);
                // mail.Bcc.Add("muhammad.usman@cppa.gov.pk");
                mail.From = new MailAddress("erpautoemail@cppa.gov.pk", "CPPA-G Invoice Notification");
                mail.Subject = "Invoice " + Subject;
                mail.Body = tmplt;
                mail.IsBodyHtml = true;



                //string FileName = Path.GetFileName(FileUploadAttachments.PostedFile.FileName);
                //Attachment attachment = new Attachment(FileUploadAttachments.PostedFile.InputStream, FileName);
                //mail.Attachments.Add(attachment);

                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("erpautoemail@cppa.gov.pk", "Cpp@erp123");
                client.Host = "mail.cppa.gov.pk";
                client.Port = 25;

                try
                {
                    client.Send(mail);
                }
                catch (Exception ex)
                {
                    ret = ex.Message;
                }
            }


            return ret;
        }


        public static byte[] DownloadFileViaRestAPI(string webUrl, ICredentials credentials, string documentLibName, string fileName)
        {
            webUrl = webUrl.EndsWith("/") ? webUrl.Substring(0, webUrl.Length - 1) : webUrl;
            string webRelativeUrl = null;
            if (webUrl.Split('/').Length > 3)
            {
                webRelativeUrl = "/" + webUrl.Split(new char[] { '/' }, 4)[3];
            }
            else
            {
                webRelativeUrl = "";
            }
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                client.Credentials = credentials;
                Uri endpointUri = new Uri("https://cppapk.sharepoint.com" + "/_api/web/GetFileByServerRelativeUrl('" + HttpUtility.UrlEncode(documentLibName) + "')/$value");
                byte[] data = client.DownloadData(endpointUri);
                return data;
               
            }
        }
        public FileResult DownloadMultipleFiles(string dataId)
        {
            try
            {
               
                //string siteURL = "https://cppapk.sharepoint.com/";
                //set credential of SharePoint online
                SecureString secureString = new SecureString();
                foreach (char c in "cdxp@1234".ToCharArray())
                {
                    secureString.AppendChar(c);
                }
                ICredentials credentials = new SharePointOnlineCredentials("cdxpsupport@CPPAPK.onmicrosoft.com", secureString);
                DataSet ds_ = new DataSet();
                DataTable dt_ = new DataTable();
                ds_ = Fn.FillDSet(@"SELECT ATTACH_HEADER_ID,FILE_NAME,File_Link,FILE_DATA FROM [CPPA_CA].[ATTACHMENT_HEADER] 
                  WHERE  DIARY_HEADER_ID =" + dataId + " AND bisDeleted=0");
                dt_ = ds_.Tables[0];
                List<DownloadFile> list_ = new List<DownloadFile>();
                if (dt_.Rows.Count > 0)
                {                  
                    foreach (DataRow dtRow in dt_.Rows)
                    {
                        string flink = dtRow.ItemArray[2].ToString();
                        string fname = dtRow.ItemArray[1].ToString();
                        string fdata = dtRow.ItemArray[3].ToString();
                        Byte[] data = new Byte[0];
                        if (fdata == "")
                        {
                            string siteURL = flink.Split(new string[] { ".com" }, StringSplitOptions.None)[0] + ".com";
                            string fileUrl = flink.Split(new string[] { ".com" }, StringSplitOptions.None)[1].Split(new string[] { "?" }, StringSplitOptions.None)[0];
                            var d = DownloadFileViaRestAPI(siteURL, credentials, fileUrl, fname);
                            DownloadFile Obj1 = new DownloadFile()
                            {
                                FileName = fname,
                                FileBytes = d
                            };
                            list_.Add(Obj1);
                        }
                        else if (dtRow["FILE_DATA"] != DBNull.Value)
                        {
                            data = (Byte[])(dtRow["FILE_DATA"]);
                            DownloadFile Obj = new DownloadFile()
                            {
                                FileName = fname,
                                FileBytes = data
                            };
                            list_.Add(Obj);
                        }
                    }
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                        {
                            foreach (var file in list_)
                            {
                                var entry = archive.CreateEntry(file.FileName, CompressionLevel.Fastest);
                                using (var zipStream = entry.Open())
                                {
                                    zipStream.Write(file.FileBytes, 0, file.FileBytes.Length);
                                }
                            }

                        }
                        ms.Position = 0;
                        return File(ms.ToArray(), "application/zip", "AllFiles.zip");
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        //public FileResult DownloadMultipleFiles(string dataId)
        //{
        //    try
        //    {
        //        string nm = "";
        //        DataSet ds_ = new DataSet();
        //        DataTable dt_ = new DataTable();
        //        ds_ = Fn.FillDSet(@"SELECT ATTACH_HEADER_ID,FILE_NAME,FILE_DATA FROM [CPPA_CA].[ATTACHMENT_HEADER] 
        //          WHERE  DIARY_HEADER_ID =" + dataId + " AND bisDeleted=0");
        //        dt_ = ds_.Tables[0];
        //        List<DownloadFile> list_ = new List<DownloadFile>();
        //        if (dt_.Rows.Count > 0)
        //        {
        //            foreach (DataRow dtRow in dt_.Rows)
        //            {
        //                if (dtRow["FILE_NAME"] != DBNull.Value)
        //                    nm = Convert.ToString(dtRow["FILE_NAME"]);
        //                Byte[] data = new Byte[0];
        //                if (dtRow["FILE_DATA"] != DBNull.Value)
        //                    data = (Byte[])(dtRow["FILE_DATA"]);
        //                DownloadFile Obj = new DownloadFile()
        //                {
        //                    FileName = nm,
        //                    FileBytes = data
        //                };
        //                list_.Add(Obj);
        //            }
        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
        //                {
        //                    foreach (var file in list_)
        //                    {
        //                        var entry = archive.CreateEntry(file.FileName, CompressionLevel.Fastest);
        //                        using (var zipStream = entry.Open())
        //                        {
        //                            zipStream.Write(file.FileBytes, 0, file.FileBytes.Length);
        //                        }
        //                    }

        //                }
        //                ms.Position = 0;
        //                return File(ms.ToArray(), "application/zip", "AllFiles.zip");
        //            }
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
    }
}



public class DownloadFile
{
    public string FileName { get; set; }
    public byte[] FileBytes { get; set; }
}
