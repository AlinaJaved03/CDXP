using CDXPWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDXPWeb.Controllers
{
    public class ScheduleTenController : Controller
    {
        private clsSQLCore Fn = new clsSQLCore();
        private clsSQLCoreOld FnO = new clsSQLCoreOld();





        [SharePointContextFilter]
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
        }

        public ActionResult ScheduleTenList()
        {
            sessionCheck();
            return View();
        }
        [HttpPost]
        public ActionResult ScheduleTenForm(String Id)
        {
            sessionCheck();
            DBSEntities entities = new DBSEntities();
            var sch10_id = 0;
            var caseid = Id.Split('½')[1];
            if (caseid == "e")
            {
                sch10_id = Convert.ToInt16(Id.Split('½')[0]);
            }
            else
            {
                sch10_id = 0;
            }
            ViewBag.SCH_ID = Convert.ToInt16(sch10_id);

            return View(entities.CPPA_PPA_SCH_10_DTL.Where(s => s.SCH_10_ID == sch10_id).ToList());
            //sessionCheck();
            //return View();
        }

        [HttpGet]
        public string GetIPP()
        {
            return Fn.Data2DropdownSQL(@"
                                SELECT v.VENDOR_ID, v.VENDOR_NAME FROM [CDXP].[AP_SUPPLIERS] as v join[CDXP].[PPA_HEADER] as h
                                on v.VENDOR_ID = h.VENDOR_ID_FK where h.IPP_CATEGORY = 20 Order by v.VENDOR_NAME");
        }

        [HttpPost]
        public string GetFule(string venderid)
        {
            return Fn.Data2DropdownSQL(@"
                                select DISTINCT  FUEL_LOOKUP_CODE Id, FUEL_LOOKUP_CODE Fule  from [CDXP].[CPPA_PPA_PLT_BLK_FUEL] as f
                                join [CDXP].[PPA_HEADER] as h on f.HEADER_ID_FK  = h.HEADER_ID_PK where h.VENDOR_ID_FK =" + venderid);
        }

        [HttpPost]
        public string GetList()
        {
            sessionCheck();
            string Returnvls = "";
            Returnvls = Fn.HTMLTableWithID_TR_Tag(@"
                                select s.[SCH_10_ID],'' [Sr#],v.VENDOR_NAME [Power Producer], s.[FUEL_LOOKUP_CODE] [Fuel], s.[SCH_10_TYPE] [Schedule Type],
                                s.[REMARKS] [Remarks]   from [CDXP].[CPPA_PPA_SCH_10] as s
                                join  [CDXP].[AP_SUPPLIERS] as v on s.VENDOR_ID = v.VENDOR_ID
                                JOIN   CDXP.WP_GC_USER_ACCESS ON v.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID     
                                WHERE  CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"'
                                ORDER BY s.[CREATION_DATE] DESC ", "tblJ1");
            return Returnvls;
        }

        [HttpPost]
        public string GetVendorName(int Id)
        {
            sessionCheck();
            string Returnvls = "";
            Returnvls = Fn.ExenID(@" select v.VENDOR_NAME [Power Producer]  from [CDXP].[CPPA_PPA_SCH_10] as s
                                join  [CDXP].[AP_SUPPLIERS] as v on s.VENDOR_ID = v.VENDOR_ID
                                JOIN   CDXP.WP_GC_USER_ACCESS ON v.VENDOR_ID = CDXP.WP_GC_USER_ACCESS.ENTITY_ID     
                                WHERE  CDXP.WP_GC_USER_ACCESS.WP_PORTAL_USERS_ID = '" + Convert.ToString(Session["UserID"]) + @"' AND s.SCH_10_ID= " + Id);
            return Returnvls;
        }

        [HttpPost]
        public string GetHeaderVendorValues(int Id)
        {
            sessionCheck();
            string Returnvls = "";
            var HeaderfuelCode = Fn.ExenID(@"SELECT FUEL_LOOKUP_CODE FROM [CDXP].[CPPA_PPA_SCH_10] WHERE SCH_10_ID=" + Id);
            var HeaderSch10Type = Fn.ExenID(@"SELECT SCH_10_TYPE FROM [CDXP].[CPPA_PPA_SCH_10] WHERE SCH_10_ID=" + Id);
            var HeaderRemarks = Fn.ExenID(@"SELECT REMARKS FROM [CDXP].[CPPA_PPA_SCH_10] WHERE SCH_10_ID=" + Id);

            Returnvls = HeaderfuelCode + '½' + HeaderSch10Type + '½' + HeaderRemarks;
            return Returnvls;
        }

        [HttpPost]
        public string UpdateSch10Data()
        {
            var frmdata = HttpContext.Request.Form["vls"];
            string[] dataID = new string[500];
            dataID = Fn.CleanSQL(HttpUtility.UrlDecode(frmdata)).Split('¡');

            string Returnvls = "";
            try
            {

                var count = dataID[1].Split('¼').Length;
            }

            catch (Exception ex)
            {
                return Convert.ToString(ex.Message);
            }

            return Returnvls;
        }


        public JsonResult InsertRecord(List<ScheduleTen> lstSch)
        {

            using (DBSEntities DB = new DBSEntities())
            {
                //Check for NULL.
                if (lstSch == null)
                {
                    lstSch = new List<ScheduleTen>();
                }
                bool flage = true;
                int scheduleId = 0;
                //Loop and insert records.
                foreach (ScheduleTen obj in lstSch)
                {
                    if (flage)
                    {
                        scheduleId = SaveScheduleHeader(obj);
                        flage = false;
                    }
                    else
                    {

                        CPPA_PPA_SCH_10_DTL objdetail = new CPPA_PPA_SCH_10_DTL();
                        objdetail.Ambient_Temperature = Convert.ToDecimal(obj.Ambient_Temperature);
                        objdetail.Correction_Factor = Convert.ToDecimal(obj.Correction_Factor);
                        objdetail.NetPlant_Output = Convert.ToDecimal(obj.NetPlant_Output);
                        objdetail.SCH_10_ID = scheduleId;
                        DB.CPPA_PPA_SCH_10_DTL.Add(objdetail);


                    }
                }
                if (scheduleId != -1)
                {
                    int insertedRecords = DB.SaveChanges();
                    return Json(insertedRecords);

                }
                else
                {
                    return Json(0);
                }
            }
        }

        private int SaveScheduleHeader(ScheduleTen obj)
        {

            var userID = Convert.ToInt16(Session["UserID"]);


            using (DBSEntities DB = new DBSEntities())
            {

                CPPA_PPA_SCH_10 objH = new CPPA_PPA_SCH_10();
                objH.CREATED_BY = 177;
                objH.CREATION_DATE = DateTime.Now;
                objH.VENDOR_ID = Convert.ToInt32(obj.VendorIDvalue);
                //Convert.ToInt16(Fn.ExenID(@"SELECT VENDOR_ID FROM [CDXP].[CPPA_PPA_SCH_10] WHERE SCH_10_ID=" + Convert.ToInt16(obj.Sch_10_id_value)));
                objH.HEADER_ID_PK = Convert.ToDecimal(Fn.ExenID(@"SELECT ph.HEADER_ID_PK FROM CDXP.PPA_HEADER ph WHERE ph.VENDOR_ID_FK=" + objH.VENDOR_ID));
                objH.FUEL_LOOKUP_CODE = obj.Fuel_Lookup_Code;
                objH.REMARKS = obj.Remaks;
                objH.SCH_10_TYPE = obj.SCH_10_TYPE;
                objH = DB.CPPA_PPA_SCH_10.Add(objH);

                int isAvailExists = Convert.ToInt32(Fn.ExenID("SELECT COUNT(cps.SCH_10_ID) FROM CDXP.CPPA_PPA_SCH_10 cps WHERE cps.FUEL_LOOKUP_CODE = '" + objH.FUEL_LOOKUP_CODE + "' AND cps.VENDOR_ID = " + objH.VENDOR_ID + " "));

                if (isAvailExists >= 1)
                {
                    ViewBag.RecordCount = 1;
                    return -1;
                }
                else
                {
                    DB.SaveChanges();
                    return objH.SCH_10_ID;

                }



            }
        }


        [HttpPost]
        public string AjaxCall()
        {
            sessionCheck();
            var frmdata = HttpContext.Request.Form["vls"];
            string[] dataID = new string[500];
            dataID = Fn.CleanSQL(HttpUtility.UrlDecode(frmdata)).Split('¡');

            string Returnvls = "";
            try
            {
                switch (Convert.ToInt32(dataID[0]))
                {
                    case 1:
                        var count = (dataID[2].Split('¼').Length);

                        decimal ambient_temp;
                        decimal netOutput;
                        var corrFactor = "";

                        int i = 0;


                        while (i < count)
                        {
                            var reqRow = dataID[2].Split('¼')[i];

                            var reqDataId = reqRow.Split('½')[0];

                            ambient_temp = Convert.ToDecimal(reqRow.Split('½')[1]);
                            netOutput = Convert.ToDecimal(reqRow.Split('½')[2]);
                            corrFactor = reqRow.Split('½')[3];

                            if (reqDataId != "0")
                            {
                                Returnvls = Fn.Exec(@"UPDATE CDXP.CPPA_PPA_SCH_10_DTL SET Ambient_Temperature=" + ambient_temp + ", NetPlant_Output= " + netOutput + ", Correction_Factor = " + corrFactor + " WHERE SCH_10_DTL_ID = " + reqDataId);
                            }
                            else
                            {
                                var sch10_id = dataID[1];
                                Returnvls = Fn.Exec(@"INSERT INTO CDXP.CPPA_PPA_SCH_10_DTL (SCH_10_ID,Ambient_Temperature, NetPlant_Output, Correction_Factor) VALUES ( " + sch10_id + "," + ambient_temp + "," + netOutput + " ," + corrFactor + ")");

                            }

                            i++;

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
    }
}