using CDXPWeb.Models;
using System;
//using System.Web.Http;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Web.UI;


namespace CDXPWeb.Controllers
{
    public class StockController : Controller
    {
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


        [HttpPost]
        public ActionResult Index()
        {
            sessionCheck();
            //string a = Fn.ExenID(
            //    @"SELECT STOCK_FUEL_ID FROM APP_SUPPLIER_SITE_ALL_STOCKS
            //           INNER JOIN CPPA_CA.ApiUsers ON PPA_HEADER_ID_FK = VENDOR_ID
            //           WHERE [USER_NAME] ='" +
            //                        Convert.ToString(Session["UserName"]) + "'");

            string a = Fn.ExenID(@"select ISNULL(IS_STOCK_MENU_ACCESS,0) [IS_STOCK_MENU_ACCESS] from [CPPA_CA].[ApiUsers] where  [USER_NAME] ='" + Convert.ToString(Session["UserName"]) + "'");



            if (Convert.ToInt32(a) > 0)
            {

                string temp = Fn.ExenID(@"SELECT convert(VARCHAR, C.VENDOR_SITE_ID ) + '½' + CONVERT(VARCHAR, C.VENDOR_ID)
                                                        FROM[dbo].[WP_GC_USER_ACCESS] A JOIN[CPPA_CA].[AP_SUPPLIERS] B ON A.VENDOR_ID = B.VENDOR_ID JOIN[dbo].[APP_SUPPLIER_SITE_ALL_STOCKS] C ON C.VENDOR_ID = B.VENDOR_ID
                                                        WHERE A.USERID = [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME] ('" + Convert.ToString(Session["UserName"]) + @"')");

                // Split the temp string using '½' as the separator
                string[] splitValues = temp.Split('½');

                // The first part contains VENDOR_SITE_ID
                string vendorSiteID = splitValues[0];

                // The second part contains VENDOR_ID
                string vendorID = splitValues[1];

                ViewBag.VendorSiteId = temp.Split('½')[0];
                temp = temp.Split('½')[1];
                var vendorsiteid = temp.Split('½')[0]; 

                //ViewBag.StockAccess = a;

                ViewBag.dll = Fn.Data2DropdownSQL(@" SELECT CAST(B.VENDOR_ID AS VARCHAR(100))+'½' + CAST(C.VENDOR_SITE_ID AS VARCHAR(100)) ID ,
                C.ADDRESS_LINE1
                FROM[dbo].[WP_GC_USER_ACCESS] A
                INNER JOIN[CPPA_CA].[AP_SUPPLIERS] B ON A.VENDOR_ID = B.VENDOR_ID INNER JOIN CPPA_CA.APP_SUPPLIER_SITE_ALL C ON C.VENDOR_ID = B.VENDOR_ID

                WHERE
                A.USERID = [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')");



                ViewBag.FuelType = Fn.Data2DropdownSQL(@" select DISTINCT STOCK_FUEL_ID,STOCK_FUEL_NAME from [dbo].[APP_SUPPLIER_SITE_ALL_STOCKS] WHERE VENDOR_ID = '" + vendorID + "' order by STOCK_FUEL_ID ");


                ViewBag.StockAccess = Fn.ExenID(@" select TOP 1 STOCK_FUEL_ID from [dbo].[APP_SUPPLIER_SITE_ALL_STOCKS] WHERE VENDOR_ID = '" + vendorID + "' AND VENDOR_SITE_ID = '" + vendorSiteID + "' order by STOCK_FUEL_ID ");


                //ViewBag.FuelType = Fn.Data2DropdownSQL(
                //    @"  select  DISTINCT STOCK_FUEL_ID,STOCK_FUEL_NAME from [dbo].[APP_SUPPLIER_SITE_ALL_STOCKS]
                //        inner join CPPA_CA.ApiUsers on PPA_HEADER_ID_FK = VENDOR_ID
                //             where USER_NAME in ('" + Convert.ToString(Session["UserName"]) + @"')");

                //[CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('ap_ipp@cppapk.onmicrosoft.com') 
                //order by 2");
                return View();
            }
            else
            {
                string User = Fn.ExenID(@"SELECT CAST(au.UserId AS VARCHAR(100))+'½'+CAST(au.UserType AS VARCHAR(100))+'½'+CAST(ISNULL(au.IS_STOCK_MENU_ACCESS,0) AS VARCHAR(100))  FROM cppa_ca.ApiUsers au WHERE au.UserId = [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')");
                ViewBag.UserType = User.Split('½')[1];
                ViewBag.StockAccess = User.Split('½')[2];

                if (User.Split('½')[1] == "4")
                {
                    ViewBag.response = Fn.Data2Json("EXEC GetDHIbyStatusbyUserID " + Convert.ToString(User.Split('½')[0]));
                }
                else if (User.Split('½')[1] == "2" || User.Split('½')[1] == "3")
                {
                    ViewBag.response = Fn.Data2Json("EXEC GetDHIbyStatus  " + Convert.ToString(User.Split('½')[0]));
                }
                return View("../Invoice/IppDashboard");
            }


        }

        public ActionResult ViewStock()
        {
            sessionCheck();
            string UserType = Fn.ExenID(@"SELECT CAST(au.UserType AS VARCHAR(100))+'½'+ CAST(au.UserId AS VARCHAR(100)) ID FROM cppa_ca.ApiUsers au WHERE au.UserId = [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"') ");
            ViewBag.UserType = UserType.Split('½')[0];
            ViewBag.UserId = UserType.Split('½')[1];

            //string ddls = Fn.Data2Json(@"SELECT DISTINCT B.VENDOR_ID, B.VENDOR_NAME ,
            //                                SiteJson=(
            //                                select C.VENDOR_SITE_ID VENDOR_SITE_ID, C.ADDRESS_LINE1 from [dbo].[APP_SUPPLIER_SITE_ALL_STOCKS] C where C.VENDOR_ID=B.VENDOR_ID
            //                                ORDER BY ADDRESS_LINE1
            //                                for json auto
            //                                )
            //                                FROM [dbo].[WP_GC_USER_ACCESS] A JOIN[CPPA_CA].[AP_SUPPLIERS] B ON A.VENDOR_ID = B.VENDOR_ID  JOIN [dbo].[APP_SUPPLIER_SITE_ALL_STOCKS] C ON C.VENDOR_ID = B.VENDOR_ID    
            //                                WHERE A.USERID = [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"') 
            //                                order by 2");

            //ViewBag.FuelType = Fn.Data2DropdownSQL(
            //       @"  select  DISTINCT STOCK_FUEL_ID,STOCK_FUEL_NAME from [dbo].[APP_SUPPLIER_SITE_ALL_STOCKS]
            //            inner join CPPA_CA.ApiUsers on PPA_HEADER_ID_FK = VENDOR_ID
            //                 where USER_NAME in ('" + Convert.ToString(Session["UserName"]) + @"')");



            string temp = Fn.ExenID(@"SELECT convert(VARCHAR, C.VENDOR_SITE_ID ) + '½' + CONVERT(VARCHAR, C.VENDOR_ID)
                                                        FROM[dbo].[WP_GC_USER_ACCESS] A JOIN[CPPA_CA].[AP_SUPPLIERS] B ON A.VENDOR_ID = B.VENDOR_ID JOIN[dbo].[APP_SUPPLIER_SITE_ALL_STOCKS] C ON C.VENDOR_ID = B.VENDOR_ID
                                                        WHERE A.USERID = [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME] ('" + Convert.ToString(Session["UserName"]) + @"')");

            ViewBag.VendorSiteId = temp.Split('½')[0];
            temp = temp.Split('½')[1];

            ViewBag.FuelType = Fn.Data2DropdownSQL(@" select DISTINCT stock_fuel_ID, STOCK_FUEL_NAME from APP_SUPPLIER_SITE_ALL_STOCKS ORDER BY STOCK_FUEL_ID");
            ViewBag.StockAccess = Fn.ExenID(@" select TOP 1 STOCK_FUEL_ID from [dbo].[APP_SUPPLIER_SITE_ALL_STOCKS] WHERE VENDOR_ID = '" + temp + "' order by STOCK_FUEL_ID ");
            //ViewBag.dll = Fn.Data2DropdownSQL(@" SELECT DISTINCT  CAST(B.VENDOR_ID AS VARCHAR(100))+'½' + CAST(C.VENDOR_SITE_ID AS VARCHAR(100)) ID ,
            //    C.ADDRESS_LINE1
            //    FROM[dbo].[WP_GC_USER_ACCESS] A
            //    INNER JOIN[CPPA_CA].[AP_SUPPLIERS] B ON A.VENDOR_ID = B.VENDOR_ID INNER JOIN CPPA_CA.APP_SUPPLIER_SITE_ALL C ON C.VENDOR_ID = B.VENDOR_ID

            //    WHERE
            //    A.USERID = [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')");

            //ViewBag.ddls = ddls;
            return View();
        }


        [System.Web.Mvc.HttpGet]
        public string GetStockDataBySiteId(string vendorSiteId, string fromDate, string toDate, string fuelType)
        {
            return Fn.Data2Json(@"SELECT A.VENDOR_NAME, A.VENDOR_ID, B.ADDRESS_LINE1, B.VENDOR_SITE_ID, C.StockId,  FORMAT(C.TransactionDate,'dd-MMM-yyyy') TransactionDate, C.OpeningStock, C.Purchased, C.Consumed, C.StockPosition, C.MidCost, C.[Status], C.Created_By, C.Creation_Date, C.Last_Update_Date, C.Last_Update_Login, IsNull(C.DeadStock, 0.0000) DeadStock, IsNull(C.UsableStock, 0.0000) UsableStock, IsNull(C.FirmOrder, 0.0000) FirmOrder, IsNull(Rate, 0.0000) Rate, IsNull(C.Remarks, '') Remarks , FuelType  FROM StockRecords C JOIN[CPPA_CA].[AP_SUPPLIERS] A ON C.VendorId = A.VENDOR_ID JOIN[CPPA_CA].[APP_SUPPLIER_SITE_ALL] B ON C.VendorSiteId = B.VENDOR_SITE_ID WHERE   (C.TransactionDate BETWEEN '" + fromDate + "' AND '" + toDate + "') AND C.VendorSiteId = '" + vendorSiteId + "' AND C.FuelType='" + fuelType + "' order by C.TransactionDate DESC");
        }



        [System.Web.Mvc.HttpGet]
        public string GetOpeningStockBySiteId(string vendorSiteId, string fuelType)
        {

            var stockPosition = Fn.Data2Json(@"SELECT  StockPosition, StockId FROM StockRecords  WHERE VendorSiteId ='" + vendorSiteId + "' AND FuelType='" + fuelType + "' ORDER BY 2 DESC");

            return stockPosition;

        }

        public string GetStockIPPBYFuel(string fid)
        {
           
            decimal fuelID1 = Convert.ToDecimal(fid);
            return Fn.Data2Json(@"SELECT DISTINCT 
            [as].VENDOR_ID, 
            [as].VENDOR_NAME ,
        SiteJson=(
        select C.VENDOR_SITE_ID VENDOR_SITE_ID, C.ADDRESS_LINE1 from [dbo].[APP_SUPPLIER_SITE_ALL_STOCKS] C where C.VENDOR_ID=[as].VENDOR_ID
        ORDER BY ADDRESS_LINE1
        for json auto
        ) 
		FROM 
		CPPA_CA.AP_SUPPLIERS [as] 
		JOIN WP_GC_USER_ACCESS wgua 
		ON [as].VENDOR_ID = wgua.VENDOR_ID 

		JOIN CPPA_CA.ApiUsers au 
		ON au.UserId = wgua.UserId
		JOIN APP_SUPPLIER_SITE_ALL_STOCKS assas
		ON assas.VENDOR_ID = [as].VENDOR_ID
		JOIN
		CPPA_CA.APP_SUPPLIER_SITE_ALL assa 
		ON
		assa.VENDOR_SITE_ID = wgua.VENDOR_SITE_ID
		WHERE assas.STOCK_FUEL_ID = " + fuelID1 + " AND au.UserId = [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + "')");

        }

        private string GetDefaultDate(string FirstOrLast)
        {
            var todayDate = DateTime.Now;
            DateTime returndate = DateTime.Now;

            if (FirstOrLast == "First")
            {
                returndate = todayDate.AddMonths(-3);
                returndate = new DateTime(returndate.Year, returndate.Month, 1);
            }
            else if (FirstOrLast == "Last")
            {
                returndate = new DateTime(todayDate.Year, todayDate.Month, 1);
                returndate = returndate.AddMonths(1).AddDays(-1);
            }
            return returndate.ToString("dd-MMM-yyyy");
        }

        [System.Web.Mvc.HttpGet]
        public string GetStockData(string vendorId)
        {
            //return Fn.Data2Json(@"SELECT StockId, TransactionDate as Transaction_Date,FORMAT(TransactionDate,'dd-MMM-yyyy') TransactionDate, OpeningStock, Purchased, Consumed, StockPosition, MidCost,VendorId, VendorSiteId, Status, Created_By, Creation_Date, Last_Update_Date, Last_Update_Login, IsNull(DeadStock, 0.0000) DeadStock, IsNull(UsableStock, 0.0000) UsableStock, IsNull(FirmOrder, 0.0000) FirmOrder, C.Remarks from StockRecords where VendorId='" + vendorId.Split('½')[0] + "' AND VendorSiteId='" + vendorId.Split('½')[1] + "' AND MONTH(TransactionDate) = MONTH(CONVERT(DATE, GETDATE())) AND YEAR(TransactionDate) = YEAR(CONVERT(DATE, GETDATE())) order by Transaction_Date DESC");

            //return Fn.Data2Json(@"SELECT TOP(30) StockId, TransactionDate as Transaction_Date,FORMAT(TransactionDate,'dd-MMM-yyyy') TransactionDate, OpeningStock, Purchased, Consumed, StockPosition, MidCost,VendorId, VendorSiteId, Status, Created_By, Creation_Date, Last_Update_Date, Last_Update_Login, IsNull(DeadStock, 0.0000) DeadStock, IsNull(UsableStock, 0.0000) UsableStock, IsNull(FirmOrder, 0.0000) FirmOrder, IsNull(Rate, 0.0000) Rate, IsNull(Remarks, '') Remarks from StockRecords where VendorId='" + vendorId.Split('½')[0] + "' AND VendorSiteId='" + vendorId.Split('½')[1] + "' and TransactionDate >= '" + GetDefaultDate("First") + "'  order by Transaction_Date DESC, StockId desc ");


            return Fn.Data2Json(@"SELECT TOP(30) 
                                StockId, 
                                TransactionDate AS Transaction_Date, 
                                FORMAT(TransactionDate, 'dd-MMM-yyyy') TransactionDate, 
                                OpeningStock, 
                                Purchased, 
                                Consumed, 
                                StockPosition, 
                                MidCost, 
                                VendorId, 
                                VendorSiteId, 
                                Status, 
                                Created_By, 
                                Creation_Date, 
                                Last_Update_Date, 
                                Last_Update_Login, 
                                ISNULL(DeadStock, 0.0000) DeadStock, 
                                ISNULL(UsableStock, 0.0000) UsableStock, 
                                ISNULL(FirmOrder, 0.0000) FirmOrder, 
                                ISNULL(Rate, 0.0000) Rate, 
                                ISNULL(Remarks, '') Remarks 
                            FROM StockRecords 
                            WHERE 
                                 VendorId='" + vendorId.Split('½')[0] + "' " +
                                 "AND VendorSiteId='" + vendorId.Split('½')[1] + "' " +
                                 "AND TransactionDate >= DATEADD(MONTH, -3, (SELECT MAX(TransactionDate) " +
                                 "FROM StockRecords where VendorId='" + vendorId.Split('½')[0] + "' " +
                                 "AND VendorSiteId='" + vendorId.Split('½')[1] + "'))" +
                                 " ORDER BY Transaction_Date DESC, StockId DESC");
        }



        [HttpPost]
        public string SaveStockData()
        {
            var frmdata = HttpContext.Request.Form["vls"];
            string[] dataID = new string[500];
            string sql = "";
            dataID = Fn.CleanSQL(HttpUtility.UrlDecode(frmdata)).Split('½');

            string vendor = dataID[0];

            //dataID[0]  => Vendor Id
            //dataID[1]  => Vendor Site Id
            //dataID[2]  => Consumed Stock
            //dataID[3]  => Purchased Stock
            //dataID[4]  => Opening Stock
            //dataID[5]  => Stock Id
            //dataID[6]  => Status
            //dataID[7]  => Mid Cost
            //dataID[8]  => Stock Date , Transaction Date
            //dataID[9]  => Dead Stock
            //dataID[10] => Usable Stock
            //dataID[11] => Firm Order
            //dataID[12] => remarks
            //dataID[13] => stockMenuAccess (FuelType)
            //dataID[14] => Rate 
            //dataID[15] => FuelType 


            //Checking Whether the previous stock is drafted or submitted
            string resultOut = Fn.ExenID("select COUNT(*) FROM StockRecords WHERE Status= 'Draft' and vendorID = " + dataID[0] + " and vendorsiteid = " + dataID[1] + " AND STOCKID <> " + dataID[5]);
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            if (Convert.ToInt32(resultOut) != 0)
            {
                return "ERR-STOCK-01";
            }

            string resultOut1 = Fn.ExenID("select COUNT(*) FROM StockRecords WHERE TransactionDate > FORMAT(CAST('" + dataID[8] + "' AS date), 'yyyy-MM-dd') and vendorID = " + dataID[0] + " and vendorsiteid = " + dataID[1] + " and FuelType =" + dataID[15] + " ORDER BY 1 desc ");
            if (Convert.ToInt32(resultOut1) != 0)
            {
                return "ERR-STOCK-03";
            }

            string lsSQL = "";
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);
            string pkdatetime1 = pkdatetime.ToString("yyyy-MM-dd h:mm:ss");
            double stockPosition = (Convert.ToDouble(dataID[4]) + Convert.ToDouble(dataID[3])) - Convert.ToDouble(dataID[2]);

            //Whether Insert Or Update
            if (Convert.ToDecimal(dataID[5]) != 0)
            {
                lsSQL = " UPDATE StockRecords SET Purchased= " + dataID[3] + ", ";
                lsSQL += " Consumed= " + dataID[2] + ", ";
                lsSQL += " StockPosition= " + stockPosition + ", ";
                lsSQL += " MidCost= " + dataID[7] + ", ";
                lsSQL += " Status = '" + dataID[6] + "', ";
                lsSQL += " Last_Update_Date = '" + pkdatetime1 + "' , ";
                lsSQL += " Last_Update_Login = [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"'), ";
                lsSQL += " DeadStock = " + dataID[9] + ", ";
                lsSQL += " UsableStock = " + dataID[10] + ", ";
                lsSQL += " FirmOrder = " + dataID[11] + " , ";
                lsSQL += " Rate = " + dataID[14] + " , ";
                lsSQL += " Remarks = '" + dataID[12] + "' ";
                lsSQL += " WHERE StockID = '" + dataID[5] + "' ";

                string result = Fn.Exec(lsSQL);
                if (result == "1")
                {
                    return "CPPA-STOCK-02";
                }
                else
                {
                    return result;
                }
            }
            else
            {

                var x = "SELECT * FROM StockRecords WHERE CREATED_BY = " + " SELECT [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"') ";


                // Get API User Id e.g 20
                string y = Fn.ExenID(" SELECT [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')");


                lsSQL = "INSERT INTO StockRecords (TransactionDate, OpeningStock, Purchased, Consumed, StockPosition, MidCost, VendorId, VendorSiteId, Status, Created_By, Creation_date, DeadStock, UsableStock, FirmOrder, Remarks, Rate, FuelType) VALUES ('" + dataID[8] + "', " + dataID[4] + ", " + dataID[3] + ", " + dataID[2] + ", " + stockPosition + ", " + dataID[7] + ", " + dataID[0] + ", " + dataID[1] + ", '" + dataID[6] + "'," + y + ",'" + pkdatetime1 + "', " + dataID[9] + ", " + dataID[10] + "," + dataID[11] + ", '" + dataID[12] + "', '" + dataID[14] + "', '" + dataID[15] + "') ";


                string result = Fn.Exec(lsSQL);

                if (result != "1")
                {
                    return result;
                }
                else
                {
                    return "CPPA-STOCK-01";
                }
            }
        }

        public ActionResult ExportToExcel(string vendorsiteid)
        {
            DataSet a = new DataSet();

            a = Fn.FillDSet(@"SELECT TOP 120
	                                TableB.TransactionDate, TableB.OpeningStock, TableB.Consumed, TableB.Purchased, TableB.ClosingStock, TableB.DeadStock, TableB.UseableStock, TableB.FirmOrder, TableB.Remarks
                                FROM (
		                                SELECT 
		                                StockId,
		                                TableA.RowNum,
			                                TransactionDate,
			                                (	SELECT 
					                                SR.OpeningStock 
				                                FROM 	
					                                StockRecords sr
				                                WHERE 
					                                sr.StockId = TableA.FirstStockEntry
			                                ) AS OpeningStock,
			                                Purchased,
			                                Consumed,
			                                MidCost,
			                                DeadStock,
			                                FirmOrder,
			                                Remarks,
			                                (	SELECT 
					                                SR.StockPosition 
				                                FROM 	
					                                StockRecords sr
				                                WHERE 
					                                sr.StockId = TableA.LastStockEntry
			                                ) AS ClosingStock,
			                                (	SELECT 
					                                SR.UsableStock
				                                FROM 	
					                                StockRecords sr
				                                WHERE 
					                                sr.StockId = TableA.LastStockEntry
			                                ) AS UseableStock
	
		                                FROM ( 
				                                SELECT 
						                                sr.StockId, 
						                                --OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId ORDER BY sr.Creation_Date) AS STOCKID, 
						                                ROW_NUMBER() OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId ORDER BY sr.Creation_Date) AS RowNum, 
						                                sr.TransactionDate AS TransactionDate, 
						                                min(sr.StockId) OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId) AS FirstStockEntry, 
						                                MAX(sr.StockId) OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId) AS LastStockEntry, 
						                                SUM(sr.Purchased) OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId) AS Purchased, 
						                                SUM(sr.Consumed) OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId) AS Consumed, 
						                                SUM(ISNULL(sr.MidCost, 0)) OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId) AS MidCost, 
						                                SUM(ISNULL(sr.DeadStock, 0)) OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId) AS DeadStock, 
						                                SUM(sr.FirmOrder) OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId) AS FirmOrder,
						                                sr.Creation_Date,
						                                sr.Remarks
					                                FROM 
						                                StockRecords sr 
					                                where 
						                                VendorSiteId=" + vendorsiteid + @" 
		                                ) TableA
					
                                 ) TableB WHERE TableB.RowNum = 1
                                    ORDER BY TableB.TransactionDate DESc");


            var gv = new GridView();
            //gv.DataSource = this.Session["Data"];
            //gv.DataBind();

            DataTable dt = new DataTable();
            dt.Columns.Add("TransactionDate");
            dt.Columns.Add("OpeningStock");
            dt.Columns.Add("Consumed");
            dt.Columns.Add("Purchased");
            dt.Columns.Add("StockPosition");
            dt.Columns.Add("DeadStock");
            dt.Columns.Add("UsableStock");
            dt.Columns.Add("FirmOrder");

            dt.Columns.Add("Remarks");
            gv.DataSource = a;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=stockdata.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("GetStockData");
        }


        public ActionResult ExportToExcel_Users(string vendorSiteIdForDownload)
        {
            DataSet a = new DataSet();

            a = Fn.FillDSet(@"SELECT TOP 120
	                                TableB.TransactionDate, TableB.OpeningStock, TableB.Consumed, TableB.Purchased, TableB.ClosingStock, TableB.DeadStock, TableB.UseableStock, TableB.FirmOrder, TableB.Remarks
                                FROM (
		                                SELECT 
		                                StockId,
		                                TableA.RowNum,
			                                TransactionDate,
			                                (	SELECT 
					                                SR.OpeningStock 
				                                FROM 	
					                                StockRecords sr
				                                WHERE 
					                                sr.StockId = TableA.FirstStockEntry
			                                ) AS OpeningStock,
			                                Purchased,
			                                Consumed,
			                                MidCost,
			                                DeadStock,
			                                FirmOrder,
			                                Remarks,
			                                (	SELECT 
					                                SR.StockPosition 
				                                FROM 	
					                                StockRecords sr
				                                WHERE 
					                                sr.StockId = TableA.LastStockEntry
			                                ) AS ClosingStock,
			                                (	SELECT 
					                                SR.UsableStock
				                                FROM 	
					                                StockRecords sr
				                                WHERE 
					                                sr.StockId = TableA.LastStockEntry
			                                ) AS UseableStock
	
		                                FROM ( 
				                                SELECT 
						                                sr.StockId, 
						                                --OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId ORDER BY sr.Creation_Date) AS STOCKID, 
						                                ROW_NUMBER() OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId ORDER BY sr.Creation_Date) AS RowNum, 
						                                sr.TransactionDate AS TransactionDate, 
						                                min(sr.StockId) OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId) AS FirstStockEntry, 
						                                MAX(sr.StockId) OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId) AS LastStockEntry, 
						                                SUM(sr.Purchased) OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId) AS Purchased, 
						                                SUM(sr.Consumed) OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId) AS Consumed, 
						                                SUM(ISNULL(sr.MidCost, 0)) OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId) AS MidCost, 
						                                SUM(ISNULL(sr.DeadStock, 0)) OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId) AS DeadStock, 
						                                SUM(sr.FirmOrder) OVER (PARTITION BY sr.TransactionDate, sr.VendorId, sr.VendorSiteId) AS FirmOrder,
						                                sr.Creation_Date,
						                                sr.Remarks
					                                FROM 
						                                StockRecords sr 
					                                where 
						                                VendorSiteId=" + vendorSiteIdForDownload + @" 
		                                ) TableA
					
                                 ) TableB WHERE TableB.RowNum = 1
                                    ORDER BY TableB.TransactionDate DESc");


            var gv = new GridView();
            //gv.DataSource = this.Session["Data"];
            //gv.DataBind();

            DataTable dt = new DataTable();
            dt.Columns.Add("TransactionDate");
            dt.Columns.Add("OpeningStock");
            dt.Columns.Add("Consumed");
            dt.Columns.Add("Purchased");
            dt.Columns.Add("StockPosition");
            dt.Columns.Add("DeadStock");
            dt.Columns.Add("UsableStock");
            dt.Columns.Add("FirmOrder");

            dt.Columns.Add("Remarks");


            gv.DataSource = a;
            gv.DataBind();


            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=stockdata.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("GetStockData");
        }



        public ActionResult PerDaySummary()
        {
            return View();
        }



        [System.Web.Mvc.HttpGet]
        public string GetPerDaySummary(string SelectedDate)
        {
            return Fn.Data2Json(@"
                                   
                                   SELECT 
	                                    Sup.VENDOR_NAME,
	                                    rvsc.VendorId, 
	                                    rvsc.VendorSiteId, 
	                                    StockRec.TransactionDate,
	                                    StockRec.FuelType, 
	                                    ISNULL(StockRec.PreviousDayConsumption,0) PreviousDayConsumption,
	                                    ISNULL(StockRec.PreviousDayReceipt,0) PreviousDayReceipt,
	                                    [dbo].[STOCKREPORT_GetUseAbleStocck](rvsc.VendorId, rvsc.VendorSiteId, rvsc.FuelType, StockRec.TransactionDate) AS UsableStock,
	                                    [dbo].[STOCKREPORT_GetDeadStock](rvsc.VendorId, rvsc.VendorSiteId, rvsc.FuelType, StockRec.TransactionDate) AS DeadStock,
	                                    [dbo].[STOCKREPORT_GetClosingStock](rvsc.VendorId, rvsc.VendorSiteId, rvsc.FuelType, StockRec.TransactionDate) AS ClosingStock,
	                                    [dbo].[STOCKREPORT_GetOpeningStock](rvsc.VendorId, rvsc.VendorSiteId, rvsc.FuelType, StockRec.TransactionDate) AS OpeningStock,
	                                    CAST( ROUND ([dbo].[STOCKREPORT_GetUseAbleStocck](rvsc.VendorId, rvsc.VendorSiteId, rvsc.FuelType, StockRec.TransactionDate) /rvsc.RequirementPerDay, 0) AS int) AS Days,
	                                    rvsc.Capacity,
	                                    rvsc.RequirementPerDay, 
	                                    rvsc.StockMaintainedDays, 
	                                    rvsc.TotalStocksRequired, 
	                                    rvsc.FirmOrderPlacedfortheMonth, 
	                                    rvsc.RecieptAgainstFirmOrderTillDate, 
	                                    rvsc.Remarks, 
	                                    rvsc.OilType, 
	                                    rvsc.ParentOilType, 
	                                    rvsc.FuelType
                                    FROM RU_VendorStockConstant rvsc
                                    LEFT JOIN (
                                        SELECT 
	                                        sr.TransactionDate AS TransactionDate,
		                                    sr.VendorId,
	                                        sr.VendorSiteId,
		                                    sr.FuelType,

		                                    (	SELECT 
				                                    SUM(sr1.Consumed)
			                                    FROM
				                                    StockRecords sr1
			                                    WHERE	
				                                    sr1.Status = 'Submitted'
			                                    AND
				                                    sr1.TransactionDate = DATEADD(day,-1, sr.TransactionDate)
			                                    AND
				                                    sr1.VendorId = sr.VendorId
			                                    AND
				                                    sr1.VendorSiteId = sr.VendorSiteId
			                                    AND
				                                    sr1.FuelType = sr.FuelType
		                                    ) PreviousDayConsumption,
		                                    (	SELECT 
				                                    SUM(sr1.Purchased)
			                                    FROM
				                                    StockRecords sr1
			                                    WHERE	
				                                    sr1.Status = 'Submitted'
			                                    AND
				                                    sr1.TransactionDate = DATEADD(day,-1, sr.TransactionDate)
			                                    AND
				                                    sr1.VendorId = sr.VendorId
			                                    AND
				                                    sr1.VendorSiteId = sr.VendorSiteId
			                                    AND
				                                    sr1.FuelType = sr.FuelType
		                                    ) PreviousDayReceipt,
		                                    (
			                                    SELECT 
				                                    sr1.OpeningStock
			                                    FROM
				                                    StockRecords sr1
			                                    WHERE
				                                    sr1.Status = 'Submitted'
			                                    AND
				                                    sr1.StockId = MAX(sr.StockId)

		                                    ) OpeningStock,
	                                        (
			                                    SELECT 
				                                    sr1.StockPosition 
			                                    FROM 
				                                    dbo.StockRecords sr1 
			                                    WHERE 
				                                    sr1.StockId = MAX(sr.StockId)
		                                    ) AS ClosingStock,
		                                    (
			                                    SELECT 
				                                    sr1.UsableStock 
			                                    FROM 
				                                    dbo.StockRecords sr1 
			                                    WHERE 
				                                    sr1.StockId = MAX(sr.StockId)
		                                    ) AS UsableStock,
		                                    (
		                                        SELECT 
			                                        sr1.DeadStock 
		                                        FROM 
			                                        dbo.StockRecords sr1 
		                                        WHERE 
			                                        sr1.StockId = MAX(sr.StockId)
	                                        ) AS DeadStock
                                        FROM dbo.StockRecords sr 
                                        WHERE sr.TransactionDate= CAST('" + SelectedDate + @"' AS DATE) AND sr.Status='Submitted'
                                        group by  
	                                        sr.TransactionDate, sr.VendorId, sr.VendorSiteId, SR.FuelType
	                                    ) StockRec ON rvsc.VendorId = StockRec.VendorId AND rvsc.VendorSiteId = StockRec.VendorSiteId AND rvsc.FuelType = StockRec.FuelType
	                                    INNER JOIN 
                                    CPPA_CA.AP_SUPPLIERS Sup ON rvsc.VendorId= sup.VENDOR_ID
                                    WHERE rvsc.ParentOilType <> 'COAL'
                                    ORDER BY  rvsc.OilType ASC
                                    ");

        }






        [HttpPost]

        public ActionResult ExportToExcel_DailySummary(string SelectedDate)
        {
            DataSet a = new DataSet();
            a = Fn.FillDSet(@"
                                SELECT 
	                                    Sup.VENDOR_NAME,
	                                    rvsc.VendorId, 
	                                    rvsc.VendorSiteId, 
	                                    StockRec.TransactionDate,
	                                    StockRec.FuelType, 
	                                    ISNULL(StockRec.PreviousDayConsumption,0) PreviousDayConsumption,
	                                    ISNULL(StockRec.PreviousDayReceipt,0) PreviousDayReceipt,
	                                    [dbo].[STOCKREPORT_GetUseAbleStocck](rvsc.VendorId, rvsc.VendorSiteId, rvsc.FuelType, StockRec.TransactionDate) AS UsableStock,
	                                    [dbo].[STOCKREPORT_GetDeadStock](rvsc.VendorId, rvsc.VendorSiteId, rvsc.FuelType, StockRec.TransactionDate) AS DeadStock,
	                                    [dbo].[STOCKREPORT_GetClosingStock](rvsc.VendorId, rvsc.VendorSiteId, rvsc.FuelType, StockRec.TransactionDate) AS ClosingStock,
	                                    [dbo].[STOCKREPORT_GetOpeningStock](rvsc.VendorId, rvsc.VendorSiteId, rvsc.FuelType, StockRec.TransactionDate) AS OpeningStock,
	                                     CAST( ROUND ([dbo].[STOCKREPORT_GetUseAbleStocck](rvsc.VendorId, rvsc.VendorSiteId, rvsc.FuelType, StockRec.TransactionDate) /rvsc.RequirementPerDay, 0) AS int) AS Days,
	                                    rvsc.Capacity,
	                                    rvsc.RequirementPerDay, 
	                                    rvsc.StockMaintainedDays, 
	                                    rvsc.TotalStocksRequired, 
	                                    rvsc.FirmOrderPlacedfortheMonth, 
	                                    rvsc.RecieptAgainstFirmOrderTillDate, 
	                                    rvsc.Remarks, 
	                                    rvsc.OilType, 
	                                    rvsc.ParentOilType, 
	                                    rvsc.FuelType
                                    FROM RU_VendorStockConstant rvsc
                                    LEFT JOIN (
                                        SELECT 
	                                        sr.TransactionDate AS TransactionDate,
		                                    sr.VendorId,
	                                        sr.VendorSiteId,
		                                    sr.FuelType,

		                                    (	SELECT 
				                                    SUM(sr1.Consumed)
			                                    FROM
				                                    StockRecords sr1
			                                    WHERE	
				                                    sr1.Status = 'Submitted'
			                                    AND
				                                    sr1.TransactionDate = DATEADD(day,-1, sr.TransactionDate)
			                                    AND
				                                    sr1.VendorId = sr.VendorId
			                                    AND
				                                    sr1.VendorSiteId = sr.VendorSiteId
			                                    AND
				                                    sr1.FuelType = sr.FuelType
		                                    ) PreviousDayConsumption,
		                                    (	SELECT 
				                                    SUM(sr1.Purchased)
			                                    FROM
				                                    StockRecords sr1
			                                    WHERE	
				                                    sr1.Status = 'Submitted'
			                                    AND
				                                    sr1.TransactionDate = DATEADD(day,-1, sr.TransactionDate)
			                                    AND
				                                    sr1.VendorId = sr.VendorId
			                                    AND
				                                    sr1.VendorSiteId = sr.VendorSiteId
			                                    AND
				                                    sr1.FuelType = sr.FuelType
		                                    ) PreviousDayReceipt,
		                                    (
			                                    SELECT 
				                                    sr1.OpeningStock
			                                    FROM
				                                    StockRecords sr1
			                                    WHERE
				                                    sr1.Status = 'Submitted'
			                                    AND
				                                    sr1.StockId = MAX(sr.StockId)

		                                    ) OpeningStock,
	                                        (
			                                    SELECT 
				                                    sr1.StockPosition 
			                                    FROM 
				                                    dbo.StockRecords sr1 
			                                    WHERE 
				                                    sr1.StockId = MAX(sr.StockId)
		                                    ) AS ClosingStock,
		                                    (
			                                    SELECT 
				                                    sr1.UsableStock 
			                                    FROM 
				                                    dbo.StockRecords sr1 
			                                    WHERE 
				                                    sr1.StockId = MAX(sr.StockId)
		                                    ) AS UsableStock,
		                                    (
		                                        SELECT 
			                                        sr1.DeadStock 
		                                        FROM 
			                                        dbo.StockRecords sr1 
		                                        WHERE 
			                                        sr1.StockId = MAX(sr.StockId)
	                                        ) AS DeadStock
                                        FROM dbo.StockRecords sr 
                                        WHERE sr.TransactionDate= CAST('" + SelectedDate + @"' AS DATE) AND sr.Status='Submitted'
                                        group by  
	                                        sr.TransactionDate, sr.VendorId, sr.VendorSiteId, SR.FuelType
	                                    ) StockRec ON rvsc.VendorId = StockRec.VendorId AND rvsc.VendorSiteId = StockRec.VendorSiteId AND rvsc.FuelType = StockRec.FuelType
	                                    INNER JOIN 
                                    CPPA_CA.AP_SUPPLIERS Sup ON rvsc.VendorId= sup.VENDOR_ID
                                    WHERE rvsc.ParentOilType <> 'COAL'
                                    ORDER BY  rvsc.OilType ASC
                            ");

            var gv = new GridView();
            //gv.DataSource = this.Session["Data"];
            //gv.DataBind();

            DataTable dt = new DataTable();
            dt.Columns.Add("Power Station");
            dt.Columns.Add("Capacity");
            dt.Columns.Add("Requirement Per Day(A)");
            dt.Columns.Add("Stock to be Maintained");
            dt.Columns.Add("Total Stocks required as per PPA");
            dt.Columns.Add("Previous Day Consumption");
            dt.Columns.Add("Previous Day Receipt");
            dt.Columns.Add("Opening Stock");
            dt.Columns.Add("Closing Stock");
            dt.Columns.Add("Days");
            dt.Columns.Add("Days/Hours");
            dt.Columns.Add("Firm Order Placed");
            dt.Columns.Add("Reciept against firm order till date");
            dt.Columns.Add("Remarks");

            gv.DataSource = a;
            gv.DataBind();


            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=stockdata.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            return View("GetPerDaySummary");
        }
    }
}