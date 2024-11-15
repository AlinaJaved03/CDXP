using Antlr.Runtime;
using CDXPWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CDXPWeb.Controllers
{
    public class MeritOrderController : Controller
    {
        private clsSQLCore Fn = new clsSQLCore();

        public void sessionCheck()
        {

            //Session["UserName"] = "hsre_ipp_op@cppapk.onmicrosoft.com";

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

        public ActionResult MeritOrderReport()
        {
            ViewBag.Ipplist = Fn.Data2DropdownSQL(@"select  DISTINCT VENDOR_ID, RuVendorDetail_Name  from meritorder.RuVendorDetail order by 2 ");

            ViewBag.FuelType = Fn.Data2DropdownSQL(@" EXEC [MeritOrder].[MeritOrder_GetALLFuelTypes]");

            return View();
        }

        public ActionResult MeritOrderMonthlyReport()
        {
            return View();
        }
        // GET: MeritOrder
        public ActionResult MoDashboard()
        {

            //string a = Fn.ExenID(@"select ISNULL(IS_STOCK_MENU_ACCESS,0) [IS_STOCK_MENU_ACCESS] from [CPPA_CA].[ApiUsers] where usertype = 13 AND [USER_NAME] ='" + Convert.ToString(Session["UserName"]) + "'");

            //if (Convert.ToInt32(a) > 0)
            //{   
            //Session["UserName"] = "hsre_ipp_op@cppapk.onmicrosoft.com";
            //string temp = Fn.ExenID(@"SELECT convert(VARCHAR, C.VENDOR_SITE_ID ) + '½' + CONVERT(VARCHAR, C.VENDOR_ID) + '½' + CONVERT(VARCHAR, A.UserId)
            //                                        FROM [dbo].[WP_GC_USER_ACCESS] A JOIN[CPPA_CA].[AP_SUPPLIERS] B ON A.VENDOR_ID = B.VENDOR_ID JOIN[dbo].[APP_SUPPLIER_SITE_ALL_STOCKS] C ON C.VENDOR_ID = B.VENDOR_ID
            //                                        WHERE A.USERID = [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME] ('" + Convert.ToString(Session["UserName"]) + @"')");
            //ViewBag.Ipplist = Fn.Data2DropdownSQL(@"select DISTINCT VENDOR_ID, RuVendorDetail_Name  from meritorder.RuVendorDetail order by 2 ");


            //string temp = Fn.ExenID(@"SELECT convert(VARCHAR, wgua.VENDOR_SITE_ID ) + '½' + CONVERT(VARCHAR, wgua.VENDOR_ID) + '½' + CONVERT(VARCHAR, wgua.UserId)
            //        FROM WP_GC_USER_ACCESS wgua
            //        INNER JOIN CPPA_CA.AP_SUPPLIERS [as]
            //        ON [as].VENDOR_ID=wgua.VENDOR_ID
            //        WHERE wgua.USERID = [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]  ('" + Convert.ToString(Session["UserName"]) + @"')");

            ViewBag.Ipplist = Fn.Data2DropdownSQL(@"select  DISTINCT VENDOR_ID, RuVendorDetail_Name  from meritorder.RuVendorDetail order by 2 ");

            string temp = Fn.ExenID(@"SELECT convert(VARCHAR, wgua.VENDOR_SITE_ID ) + '½' + CONVERT(VARCHAR, wgua.VENDOR_ID) + '½' + CONVERT(VARCHAR, wgua.UserId)
                    FROM WP_GC_USER_ACCESS wgua
                    INNER JOIN meritorder.ruvendordetail [as]
                    ON [as].VENDOR_ID=wgua.VENDOR_ID
                    WHERE wgua.USERID = [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')");


            ViewBag.VendorSiteId = temp.Split('½')[0];
            ViewBag.VendorId = temp.Split('½')[1];
            ViewBag.UserId = temp.Split('½')[2];

            ViewBag.FuelType = Fn.Data2DropdownSQL(@" EXEC [MeritOrder].MeritOrder_GetFuelTypes " + temp.Split('½')[1] + " ");

            ViewBag.StockDate = "2022-12-12";


            DataSet a = new DataSet();
            DataTable dt2 = new DataTable();
            dt2 = Fn.FillDSet("EXEC [MeritOrder].[MeritOrder_GetMeritOrderPortalLockValidation] ").Tables[0];

            ViewBag.PortalLock = Convert.ToString(dt2.Rows[0]["lock_portal"]);
            ViewBag.Portal_lockStart = Convert.ToString(dt2.Rows[0]["Portal_lockStart"]);
            ViewBag.Portal_lockEnd = Convert.ToString(dt2.Rows[0]["Portal_lockEnd"]);

            return View();
        }

        public ActionResult MeritOrderCppa()
        {
            return View();
        }


        public ActionResult MeritOrderNpcc()
        {
            return View();
        }
        public JsonResult getStockDate(int vendorId, string fuelType)
        {
            try
            {
                string actualReceiptDate = null;
                string estimatedDate = null;
                string errorDescription = null;

                string result = null;
                DataTable dt = new DataTable();

                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_type", fuelType);
                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_GetStockDate]").Tables[0];

                actualReceiptDate = Convert.ToDateTime(dt.Rows[0][0]).ToString("yyyy-MM-dd");
                estimatedDate = Convert.ToDateTime(dt.Rows[0][1]).ToString("yyyy-MM-dd");
                errorDescription = dt.Rows[0][2].ToString();
                result = actualReceiptDate + "½" + estimatedDate + "½" + errorDescription;

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public string populateFuelIndex(int vendorId, string fuelType, int type)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetFuelindex] " + vendorId + ", '" + fuelType + "', " + type + " ");

        }

        public string populateFuelIndexID(string vendorId, string fuelType, int type)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetFuelindex_importedcoal] " + vendorId + ", '" + fuelType + "', " + type + " ");

        }

        public string populateComponents(int vendorId, int fuelIndex, int type, int fuelIndexHiddenId = 0)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetFuelindexComponents] " + vendorId + ", " + fuelIndex + ", " + type + ", " + fuelIndexHiddenId);


        }

        public string populateSubType(int vendorId, int fuelIndex)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetFuelindexSubtype] " + vendorId + ", " + fuelIndex + " ");

        }

        public string populateMODataComponents(int vendorId, string fuelType, string date)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetMODataComponents] " + vendorId + ", '" + fuelType + "', '" + date + "' ");

        }

        public string populatePreviousMoData(int vendorId, string fuelType, string date)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetMODataComponentsAutoPopulateValues] " + vendorId + ", '" + fuelType + "', '" + date + "' ");

        }

        public string populateFuelIndexActualConsumptionComponents(int vendorId, int fuelIndex, string date)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetActualConsumptionComponents] " + vendorId + ", '" + fuelIndex + "' ,'" + date + "' ");

        }

        public string editStockRecord(int ReceiptheaderId, int fuelIndexId, int type)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetFuelindexComponentsForUpdate] " + ReceiptheaderId + ", " + fuelIndexId + ", " + type + " ");

        }

        public string editActualStockRecord(int vendorId, int fuelIndexId, string date)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetActualConsumptionComponentsForUpdate] " + vendorId + ", '" + fuelIndexId + "' ,'" + date + "' ");

        }

        public string editEstRecord(int vendorId, int FuelIndex, string date)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetEstimatedConsumptionComponentsFORUPDATE] " + vendorId + ", " + FuelIndex + ", '" + date + "' ");

        }

        public JsonResult updateEstConsumption()
        {

            try
            {
                string responseFlag = null;
                string message = null;

                string result = null;
                DataTable dt = new DataTable();

                var mtConsumptionHeader_Id = HttpContext.Request.Form["mtConsumptionHeader_Id"];
                var fuelType = HttpContext.Request.Form["fuelType"];
                var valArr = HttpContext.Request.Form["valArr"];
                var vendorId = HttpContext.Request.Form["vendorId"];
                var date = HttpContext.Request.Form["date"];
                var fuelIndex = HttpContext.Request.Form["fuelIndex"];
                var userId = HttpContext.Request.Form["userId"];
                var data_tab = HttpContext.Request.Form["data_tab"];
                var filesData = HttpContext.Request.Form["filesData"];

                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@mtConsumptionHeader_Id", mtConsumptionHeader_Id);
                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_Type", fuelType);
                Fn.AddParameter("@date", date);
                Fn.AddParameter("@RuFuelIndex_Id", fuelIndex);
                Fn.AddParameter("@component_data", valArr);
                Fn.AddParameter("@user_id", userId);
                Fn.AddParameter("@data_tab", data_tab);
                Fn.AddParameter("@attachment_data", filesData);

                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_SetConsumption_Components]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public JsonResult deleteEstConsumption(int mtConsumptionHeader_Id)
        {
            try
            {
                string responseFlag = null;
                string message = null;

                string result = null;
                DataTable dt = new DataTable();

                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@mtConsumptionHeader_Id", mtConsumptionHeader_Id);
                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_RemoveEstimatedConsumption]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public string populateFuelIndexConsumptionComponents(int vendorId, int FuelIndex, string date)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetEstimatedConsumptionComponents] " + vendorId + ", '" + FuelIndex + "' ,'" + date + "' ");

        }
        public JsonResult addReceipt()
        {

            try
            {
                string responseFlag = null;
                string message = null;
                string result = null;
                DataTable dt = new DataTable();

                var mtReceiptsHeader_Id = HttpContext.Request.Form["mtReceiptsHeader_Id"];
                var fuelType = HttpContext.Request.Form["fuelType"];
                var valArr = HttpContext.Request.Form["valArr"];
                var vendorId = HttpContext.Request.Form["vendorId"];
                var date = HttpContext.Request.Form["date"];
                var fuelIndex = HttpContext.Request.Form["fuelIndex"];
                var userId = HttpContext.Request.Form["userId"];
                var filesData = HttpContext.Request.Form["filesData"];
                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@mtReceiptsHeader_Id", mtReceiptsHeader_Id);
                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_Type", fuelType);
                Fn.AddParameter("@date", date);
                Fn.AddParameter("@RuFuelIndex_Id", fuelIndex);
                Fn.AddParameter("@component_data", valArr);
                Fn.AddParameter("@user_id", userId);
                Fn.AddParameter("@attachment_data", filesData);

                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_SetReciepts]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }


        }


        public JsonResult submitReceipts()
        {
            try
            {
                string responseFlag = null;
                string message = null;

                string result = null;
                DataTable dt = new DataTable();


                var fuelType = HttpContext.Request.Form["fuelType"];
                var vendorId = HttpContext.Request.Form["vendorId"];
                var date = HttpContext.Request.Form["date"];
                var userId = HttpContext.Request.Form["userId"];

                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_Type", fuelType);
                Fn.AddParameter("@date", date);
                Fn.AddParameter("@user_id", userId);
                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_SubmitReciepts]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;
                //result = "1½Successful";

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }



        }


        public JsonResult updateReceipt()
        {
            try
            {
                string responseFlag = null;
                string message = null;
                string result = null;
                DataTable dt = new DataTable();

                var mtReceiptsHeader_Id = HttpContext.Request.Form["mtReceiptsHeader_Id"];
                var fuelType = HttpContext.Request.Form["fuelType"];
                var valArr = HttpContext.Request.Form["valArr"];
                var vendorId = HttpContext.Request.Form["vendorId"];
                var date = HttpContext.Request.Form["date"];
                var fuelIndex = HttpContext.Request.Form["fuelIndex"];
                var userId = HttpContext.Request.Form["userId"];
                var filesData = HttpContext.Request.Form["filesData"];
                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@mtReceiptsHeader_Id", mtReceiptsHeader_Id);
                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_Type", fuelType);
                Fn.AddParameter("@date", date);
                Fn.AddParameter("@RuFuelIndex_Id", fuelIndex);
                Fn.AddParameter("@component_data", valArr);
                Fn.AddParameter("@user_id", userId);
                Fn.AddParameter("@attachment_data", filesData);
                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_SetReciepts]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string MoDataNotes()
        {
            return Fn.Data2Json(@"EXEC [MeritOrder].[MeritOrder_GetMeritOrderInstructionNotes]");

        }


        public string getSavedReceipt(int vendorId, string fuelType, string date)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetReciepts] " + vendorId + ", '" + fuelType + "', '" + date + "' ");

        }

        public string getOpeningStockWidget(int vendorId, string fuelType, string date)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetOpeningStock] " + vendorId + ", '" + fuelType + "', '" + date + "' ");

        }

        public string getReceiptStockWidget(int vendorId, string fuelType, string date)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetStockReciepts] " + vendorId + ", '" + fuelType + "', '" + date + "' ");

        }
        public string getClosingStockWidget(int vendorId, string fuelType, string date)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetStockclosing] " + vendorId + ", '" + fuelType + "', '" + date + "' ");

        }

        public string getUnits(int vendorId, string fuelType)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetStockUnits] " + vendorId + ", '" + fuelType + "' ");

        }


        public JsonResult generateMeritOrder()
        {
            try
            {
                string responseFlag = null;
                string date = null;

                string result = null;
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                Fn.Parameters = new List<SqlParameter>();

                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_GetMeritOrderSchedulardata]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                date = dt.Rows[0][1].ToString();
                var tempdate = Convert.ToDateTime(date).ToString("yyyy-MM-dd");

                //Fn.AddParameter("@date", date);
                //dt1 = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_SchedulerForMeritOrder]").Tables[0];

                Fn.Data2Json(@"EXEC [MeritOrder].[MeritOrder_SchedulerForMeritOrder] '" + tempdate + "' ");
                result = "1½Successful";

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public JsonResult deleteStock(int mtReceiptsHeader_Id)
        {
            //return Fn.Exec(@" [MeritOrder].[MeritOrder_RemoveReciepts] " + mtReceiptsHeader_Id + "");

            try
            {
                string responseFlag = null;
                string message = null;

                string result = null;
                DataTable dt = new DataTable();

                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@mtReceiptsHeader_Id", mtReceiptsHeader_Id);
                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_RemoveReciepts]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;

                //result = "1½Successful";

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public string getDetailedListReport(int vendorId, string fuelType, string fromDate, string toDate)
        {
            return Fn.Data2Json(@"EXEC [MeritOrder].[MeritOrder_GetStockDetail_Report] " + vendorId + ", '" + fuelType + "', '" + fromDate + "', '" + toDate + "' ");

        }
        public string getDetailedList(int vendorId, string fuelType, string fromDate, string toDate)
        {
            return Fn.Data2Json(@"EXEC [MeritOrder].[MeritOrder_GetStockDetail] " + vendorId + ", '" + fuelType + "', '" + fromDate + "', '" + toDate + "' ");

        }

        public string getDetailedListForRlngAndGas(int vendorId, string fuelType, int fuelIndex, string fromDate, string toDate)
        {



            Fn.Parameters = new List<SqlParameter>();

            Fn.AddParameter("@vendor_id", vendorId);
            Fn.AddParameter("@fuel_type", fuelType);
            Fn.AddParameter("@RuFuelIndex_Id", fuelIndex);
            Fn.AddParameter("@date_from", fromDate);
            Fn.AddParameter("@date_to", toDate);
            var ds = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_GetStockConsumptionDetailFORRLNGGAS]");
            return JsonConvert.SerializeObject(ds, Formatting.Indented);

            // return Json(ds, JsonRequestBehavior.AllowGet);



        }


        #region Consumption

        public string getSavedActualConsumption(int vendorId, string fuelType, string date, string type)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetStockConsumption] " + vendorId + ", '" + fuelType + "', '" + date + "', " + type + " ");

        }

        public JsonResult getSavedActualConsumption_ForRlngAndGas(int vendorId, string fuelType, int fuelIndex, string date, string type)
        {

            try
            {
                string headerId = null;
                string FuelIndexId = null;
                string Components = null;
                string Date_time = null;
                string Attachment_data = null;

                string result = null;
                string Is_Submitted = null;
                DataTable dt = new DataTable();

                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_Type", fuelType);
                Fn.AddParameter("@RuFuelIndex_Id", fuelIndex);
                Fn.AddParameter("@date", date);
                Fn.AddParameter("@data_tab", type);
                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_GetStockConsumptionFORRLNGGAS]").Tables[0];

                headerId = dt.Rows[0][0].ToString();
                FuelIndexId = dt.Rows[0][1].ToString();
                Components = dt.Rows[0][2].ToString();
                Date_time = dt.Rows[0][3].ToString();
                Is_Submitted = dt.Rows[0][4].ToString();
                Attachment_data = dt.Rows[0][5].ToString();
                result = headerId + "½" + FuelIndexId + "½" + Components + "½" + Date_time + "½" + Is_Submitted + "½" + Attachment_data;

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }


        }


        public string ActualConsumptionValidation(int vendorId, string fuelType, string date, int type)
        {
            DataSet a = new DataSet();
            DataTable dt2 = new DataTable();
            dt2 = Fn.FillDSet("EXEC [MeritOrder].[MeritOrder_ConsumptionValidation] " + vendorId + ", '" + fuelType + "', '" + date + "', " + type + "").Tables[0];

            return Convert.ToString(dt2.Rows[0]["error_code"]) + '½' + Convert.ToString(dt2.Rows[0]["error_description"]);

        }


        public string LockPortal()
        {
            DataSet a = new DataSet();
            DataTable dt2 = new DataTable();
            dt2 = Fn.FillDSet("EXEC [MeritOrder].[MeritOrder_GetMeritOrderPortalLockValidation] ").Tables[0];

            return Convert.ToString(dt2.Rows[0]["lock_portal"]) + '½' + Convert.ToString(dt2.Rows[0]["Portal_lockStart"]) + '½' + Convert.ToString(dt2.Rows[0]["Portal_lockEnd"]);


        }


        public JsonResult MoDataValidation(int vendorId, string fuelType, string date)
        {

            try
            {
                string responseFlag = null;
                string message = null;
                string result = null;
                DataTable dt = new DataTable();
                DataSet a = new DataSet();
                dt = Fn.FillDSet("EXEC [MeritOrder].[MeritOrder_MODetailValidation]  " + vendorId + ", '" + fuelType + "', '" + date + "' ").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw;
            }


        }


        public string RecieptsValidation(int vendorId, string fuelType, string date)
        {
            DataSet a = new DataSet();
            DataTable dt2 = new DataTable();
            dt2 = Fn.FillDSet("EXEC [MeritOrder].[MeritOrder_RecieptsValidation] " + vendorId + ", '" + fuelType + "', '" + date + "' ").Tables[0];

            return Convert.ToString(dt2.Rows[0]["disable_reciepts"]);

        }

        public JsonResult addEstConsumption()
        {

            try
            {
                string responseFlag = null;
                string message = null;

                string result = null;
                DataTable dt = new DataTable();

                var mtConsumptionHeader_Id = HttpContext.Request.Form["mtConsumptionHeader_Id"];
                var fuelType = HttpContext.Request.Form["fuelType"];
                var valArr = HttpContext.Request.Form["valArr"];
                var vendorId = HttpContext.Request.Form["vendorId"];
                var date = HttpContext.Request.Form["date"];
                var fuelIndex = HttpContext.Request.Form["fuelIndex"];
                var userId = HttpContext.Request.Form["userId"];
                var data_tab = HttpContext.Request.Form["data_tab"];
                var filesData = HttpContext.Request.Form["filesData"];


                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@mtConsumptionHeader_Id", mtConsumptionHeader_Id);
                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_Type", fuelType);
                Fn.AddParameter("@date", date);
                Fn.AddParameter("@RuFuelIndex_Id", fuelIndex);
                Fn.AddParameter("@component_data", valArr);
                Fn.AddParameter("@user_id", userId);
                Fn.AddParameter("@data_tab", data_tab);
                Fn.AddParameter("@attachment_data", filesData);
                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_SetConsumption_Components]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;
                //HttpContext.Request.Form["valArr"] = "";
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }


        }

        public JsonResult submitEstConsumption()
        {

            try
            {
                string responseFlag = null;
                string message = null;

                string result = null;
                DataTable dt = new DataTable();


                var fuelType = HttpContext.Request.Form["fuelType"];
                var vendorId = HttpContext.Request.Form["vendorId"];
                var date = HttpContext.Request.Form["date"];
                var userId = HttpContext.Request.Form["userId"];
                var type = HttpContext.Request.Form["type"];

                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_Type", fuelType);
                Fn.AddParameter("@date", date);
                Fn.AddParameter("@user_id", userId);
                Fn.AddParameter("@data_tab", type);
                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_SubmitConsumption]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;
                //result = "1½Successful";

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public JsonResult getSavedEstConsumption_ForRlngAndGas(int vendorId, string fuelType, int fuelIndex, string date, string type)
        {

            try
            {
                string Amount = null;
                string Wavg = null;
                string Quantity = null;
                string Date_time = null;
                string Attachment_data = null;
                string headerid = null;
                string result = null;
                string Rate = null;
                string is_Submitted = null;
                DataTable dt = new DataTable();

                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_Type", fuelType);
                // Fn.AddParameter("@RuFuelIndex_Id", fuelIndex);
                Fn.AddParameter("@date", date);
                Fn.AddParameter("@data_tab", type);
                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_GetStockEstimatedConsumption]").Tables[0];


                if (dt != null && dt.Rows.Count > 0)
                {
                    headerid = dt.Rows[0][0].ToString();
                    Date_time = dt.Rows[0][1].ToString();
                    Quantity = dt.Rows[0][2].ToString();
                    Rate = dt.Rows[0][3].ToString();
                    Amount = dt.Rows[0][4].ToString();
                    Wavg = dt.Rows[0][5].ToString();
                    is_Submitted = dt.Rows[0][6].ToString();
                    Attachment_data = dt.Rows[0][7].ToString();
                    result = headerid + "½" + Date_time + "½" + Quantity + "½" + Rate + "½" + Amount + "½" + Wavg + "½" + is_Submitted + "½" + Attachment_data;
                }





                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }


        }

        public string getSavedEstConsumption(int vendorId, string fuelType, string date, string type)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetStockEstimatedConsumption] " + vendorId + ", '" + fuelType + "', '" + date + "', " + type + " ");

        }
        public JsonResult addActualConsumption()
        {

            try
            {
                string responseFlag = null;
                string message = null;

                string result = null;
                DataTable dt = new DataTable();

                var mtConsumptionHeader_Id = HttpContext.Request.Form["mtConsumptionHeader_Id"];
                var fuelType = HttpContext.Request.Form["fuelType"];
                var valArr = HttpContext.Request.Form["valArr"];
                var vendorId = HttpContext.Request.Form["vendorId"];
                var date = HttpContext.Request.Form["date"];
                var fuelIndex = HttpContext.Request.Form["fuelIndex"];
                var userId = HttpContext.Request.Form["userId"];
                var data_tab = HttpContext.Request.Form["data_tab"];
                var filesData = HttpContext.Request.Form["filesData"];


                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@mtConsumptionHeader_Id", mtConsumptionHeader_Id);
                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_Type", fuelType);
                Fn.AddParameter("@date", date);
                Fn.AddParameter("@RuFuelIndex_Id", fuelIndex);
                Fn.AddParameter("@component_data", valArr);
                Fn.AddParameter("@user_id", userId);
                Fn.AddParameter("@data_tab", data_tab);
                Fn.AddParameter("@attachment_data", filesData);
                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_SetConsumption_Components]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }


        }


        public JsonResult submitActualConsumption()
        {

            try
            {
                string responseFlag = null;
                string message = null;

                string result = null;
                DataTable dt = new DataTable();


                var fuelType = HttpContext.Request.Form["fuelType"];
                var vendorId = HttpContext.Request.Form["vendorId"];
                var date = HttpContext.Request.Form["date"];
                var userId = HttpContext.Request.Form["userId"];
                var type = HttpContext.Request.Form["type"];

                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_Type", fuelType);
                Fn.AddParameter("@date", date);
                Fn.AddParameter("@user_id", userId);
                Fn.AddParameter("@data_tab", type);
                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_SubmitConsumption]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;
                //result = "1½Successful";

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }

        }



        public JsonResult updateActualConsumption()
        {

            try
            {
                string responseFlag = null;
                string message = null;

                string result = null;
                DataTable dt = new DataTable();

                var mtConsumptionHeader_Id = HttpContext.Request.Form["mtConsumptionHeader_Id"];
                var fuelType = HttpContext.Request.Form["fuelType"];
                var valArr = HttpContext.Request.Form["valArr"];
                var vendorId = HttpContext.Request.Form["vendorId"];
                var date = HttpContext.Request.Form["date"];
                var fuelIndex = HttpContext.Request.Form["fuelIndex"];
                var userId = HttpContext.Request.Form["userId"];
                var data_tab = HttpContext.Request.Form["data_tab"];
                var filesData = HttpContext.Request.Form["filesData"];

                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@mtConsumptionHeader_Id", mtConsumptionHeader_Id);
                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_Type", fuelType);
                Fn.AddParameter("@date", date);
                Fn.AddParameter("@RuFuelIndex_Id", fuelIndex);
                Fn.AddParameter("@component_data", valArr);
                Fn.AddParameter("@user_id", userId);
                Fn.AddParameter("@data_tab", data_tab);
                Fn.AddParameter("@attachment_data", filesData);

                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_SetConsumption_Components]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public JsonResult deleteActualConsumption(int mtConsumptionHeader_Id)
        {
            try
            {
                string responseFlag = null;
                string message = null;

                string result = null;
                DataTable dt = new DataTable();

                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@mtConsumptionHeader_Id", mtConsumptionHeader_Id);
                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_RemoveConsumption]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        #endregion

        public JsonResult addMoData()
        {
            try
            {
                string responseFlag = null;
                string message = null;
                string result = null;
                DataTable dt = new DataTable();

                var mtMODataHeader_Id = HttpContext.Request.Form["mtMODataHeader_Id"];
                var fuelType = HttpContext.Request.Form["fuelType"];
                var valArr = HttpContext.Request.Form["valArr"];
                var vendorId = HttpContext.Request.Form["vendorId"];
                var date = HttpContext.Request.Form["date"];
                var userId = HttpContext.Request.Form["userId"];
                var filesData = HttpContext.Request.Form["filesData"];
                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@mtMODataHeader_Id", mtMODataHeader_Id);
                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_Type", fuelType);
                Fn.AddParameter("@date", date); ;
                Fn.AddParameter("@component_data", valArr);
                Fn.AddParameter("@user_id", userId);
                Fn.AddParameter("@attachment_data", filesData);

                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_SetMOData]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        public JsonResult GetRefValues(int vendorId, string fueltype)
        {
            try
            {
                DataTable dt = new DataTable();
                Fn.Parameters = new List<SqlParameter>();
                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_type", fueltype);

                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_GetBackendConfigValuesOfAnIPP]").Tables[0];

                string result = dt.Rows[0][0].ToString();

                return Json(result, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string GetMonthlyReport()
        {
            try
            {
                DataTable dt = new DataTable();

                string Returnvls = Fn.Data2Json("EXEC [MeritOrder].[MeritOrder_GetIppReceiptsSubmissionSummary]");
                return Returnvls;

                //dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_GetIppReceiptsSubmissionSummary]").Tables[0];

                //string result = dt.Rows[0][0].ToString();

                // return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public JsonResult getSavedMoData(int vendorId, string fuelType, string date)
        {

            try
            {
                string headerId = null;
                string attachmentData = null;
                string Components = null;
                string Date_time = null;

                string result = null;
                string Is_Submitted = null;
                DataTable dt = new DataTable();

                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_Type", fuelType);
                Fn.AddParameter("@date", date);
                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_GetMODetail]").Tables[0];

                headerId = dt.Rows[0][0].ToString();
                Components = dt.Rows[0][1].ToString();
                Date_time = dt.Rows[0][2].ToString();
                Is_Submitted = dt.Rows[0][3].ToString();
                attachmentData = dt.Rows[0][4].ToString();
                result = headerId + "½" + Components + "½" + Date_time + "½" + Is_Submitted + "½" + attachmentData;

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }


        }


        public string getMoDataSummary(int vendorId, string fuelType, string date)
        {

            try
            {

                return Fn.Data2Json(@" [MeritOrder].[MeritOrder_FCCCalculationViewForModetail] " + vendorId + ", '" + fuelType + "', '" + date + "' ");


            }
            catch (Exception ex)
            {

                throw;
            }


        }


        public string editMoData(int MoDataheaderId, int vendorId, string fuelType)
        {
            return Fn.Data2Json(@" [MeritOrder].[MeritOrder_GetMODataComponentsForUpdate] " + MoDataheaderId + ", " + vendorId + ", '" + fuelType + "' ");

        }

        public JsonResult updateMoData()
        {
            try
            {
                string responseFlag = null;
                string message = null;
                string result = null;
                DataTable dt = new DataTable();

                var mtMODataHeader_Id = HttpContext.Request.Form["mtMODataHeader_Id"];
                var fuelType = HttpContext.Request.Form["fuelType"];
                var valArr = HttpContext.Request.Form["valArr"];
                var vendorId = HttpContext.Request.Form["vendorId"];
                var date = HttpContext.Request.Form["date"];
                var userId = HttpContext.Request.Form["userId"];
                var filesData = HttpContext.Request.Form["filesData"];
                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@mtMODataHeader_Id", mtMODataHeader_Id);
                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_Type", fuelType);
                Fn.AddParameter("@date", date); ;
                Fn.AddParameter("@component_data", valArr);
                Fn.AddParameter("@user_id", userId);
                Fn.AddParameter("@attachment_data", filesData);

                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_SetMOData]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        public JsonResult submitMoData()
        {

            try
            {
                string responseFlag = null;
                string message = null;

                string result = null;
                DataTable dt = new DataTable();


                var fuelType = HttpContext.Request.Form["fuelType"];
                var vendorId = HttpContext.Request.Form["vendorId"];
                var date = HttpContext.Request.Form["date"];
                var userId = HttpContext.Request.Form["userId"];

                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@vendor_id", vendorId);
                Fn.AddParameter("@fuel_Type", fuelType);
                Fn.AddParameter("@date", date);
                Fn.AddParameter("@user_id", userId);
                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_SubmitMOData]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public JsonResult deleteMoData(int mtMODataHeader_Id)
        {

            try
            {
                string responseFlag = null;
                string message = null;

                string result = null;
                DataTable dt = new DataTable();

                Fn.Parameters = new List<SqlParameter>();

                Fn.AddParameter("@mtMODataHeader_Id", mtMODataHeader_Id);
                dt = Fn.ExecuteDataSet("[MeritOrder].[MeritOrder_RemoveMOData]").Tables[0];

                responseFlag = dt.Rows[0][0].ToString();
                message = dt.Rows[0][1].ToString();
                result = responseFlag + "½" + message;
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public string MOCppa(string date)
        {
            return Fn.Data2Json(@"EXEC [MeritOrder].[MeritOrder_GetMeritOrderDetail] '" + date + "' ");

        }

        public string downloadAttachment(int attachmentId)
        {
            string Returnvls = "";
            try
            {

                DataSet ds2 = new DataSet();
                DataTable dt2 = new DataTable();
                //by babar
                ds2 = Fn.FillDSet(@"Exec [MeritOrder].[MeritOrder_GetAttachmentData]" + attachmentId);
                dt2 = ds2.Tables[0];
                if (dt2.Rows.Count > 0)
                {
                    string nm = Convert.ToString(dt2.Rows[0][0]);
                    string ftyp = Convert.ToString(dt2.Rows[0][1]);
                    string file = Convert.ToString(dt2.Rows[0][3]);
                    string ext = nm.Split('.')[nm.Split('.').Length - 1];
                    String path = HttpContext.Server.MapPath("/Content/_FilesNew"); //Path
                    string FilePath = Path.Combine(path, attachmentId + "." + ext);

                    if (!System.IO.File.Exists(FilePath))
                    {
                        byte[] byteArray = Convert.FromBase64String(file);
                        string HDR2 = Convert.ToBase64String(byteArray);
                        byte[] FileBytes = Convert.FromBase64String(HDR2);
                        System.IO.File.WriteAllBytes(FilePath, FileBytes);
                    }
                    var obj2 = new { fdata = "", name = nm, typ = ftyp, ext = ext };
                    return Returnvls = new JavaScriptSerializer().Serialize(obj2);
                }
                else
                {
                    return Returnvls = "No data found for this id";
                }

            }
            catch (Exception ex)
            {

                //Returnvls = ex.Message;
                throw;
            }
        }

        public string populateFuelTypes(int VendorId)
        {
            var result = "";
            if (Convert.ToString(VendorId) != "")
            {
                result = Fn.Data2Json(@" EXEC [MeritOrder].MeritOrder_GetFuelTypes " + VendorId + " ");
            }

            return result;
        }

    }
}