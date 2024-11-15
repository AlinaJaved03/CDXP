using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using CDXPWeb.Models;

namespace CDXPWeb.Controllers
{
    public class SchedularController : Controller
    {
        public static clsSQLCore FnS = new clsSQLCore();

        // GET: Schedular
        public ActionResult Index()
        {
            try
            {
                DateTime utc = DateTime.UtcNow;
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);

                DateTime dtDACStart = new DateTime(pkdatetime.Year, pkdatetime.Month, pkdatetime.Day, 12, 0, 0);
                DateTime dtDACEnd = new DateTime(pkdatetime.Year, pkdatetime.Month, pkdatetime.Day, 13, 0, 0);
                DateTime dtDAHStart = new DateTime(pkdatetime.Year, pkdatetime.Month, pkdatetime.Day, 17, 0, 0);
                DateTime dtDAHEnd = new DateTime(pkdatetime.Year, pkdatetime.Month, pkdatetime.Day, 18, 0, 0);

                if (pkdatetime >= dtDACStart && pkdatetime <= dtDACEnd)
                {
                    this.AUTO_DECLARATION_OF_AVAIL_CAPACITY(pkdatetime);
                    ViewBag.Result = "Auto Declaration of Availablity Run Successfully";
                }

                if (pkdatetime >= dtDAHStart && pkdatetime <= dtDAHEnd)
                {
                    this.AUTO_DAY_AHEAD_DEMAND(pkdatetime);
                    ViewBag.Result = "Auto Day Ahead Demand Run Successfully";
                }

            }
            catch (Exception e)
            {
                ViewBag.Result = e.Message.ToString();
                return View();
            }
            return View();
        }

        private Boolean AUTO_DAY_AHEAD_DEMAND(DateTime pkdatetime)
        {
            try
            {
                DateTime Nextday = pkdatetime.AddDays(1);
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                string sql = "";
                sql = "select VENDOR_SITE_ID from CDXP.AP_SUPPLIER_SITE_ALL a inner join CDXP.PPA_HEADER  C ON a.VENDOR_SITE_ID=c.VENDOR_SITE_ID_FK where not exists(select VENDOR_SITE_ID from[CDXP].[WP_GC_HOURLY_DATA_HEADER] b where b.VENDOR_SITE_ID_FK = a.VENDOR_SITE_ID and HOURLY_DATA_TYPE = 'DAD' AND TARGET_DATE = '" + Nextday.Date + "') and c.IPP_CATEGORY IN (20,30)"; //Thermal & Nuclear only
                //clsSQLCore fns = new clsSQLCore();
                var data = FnS.FillDSet(sql);
                List<string> values = new List<string>();
                if (data != null)
                {
                    var dt_ = data.Tables[0];
                    foreach (DataRow row in dt_.Rows)
                    {
                        values.Add(Convert.ToString(row[0]));
                    }

                }

                foreach (string item in values) // Loop through List with foreach
                {
                    //ds = FnS.FillDSet("SELECT TOP(1)* FROM [CDXP].[WP_GC_HOURLY_DATA_HEADER] WHERE HOURLY_DATA_TYPE='DAD' AND VENDOR_SITE_ID_FK = " + item + " ORDER BY TARGET_DATE DESC");

                    ds = FnS.FillDSet("select TOP(1)* FROM [CDXP].[WP_GC_HOURLY_DATA_HEADER] a inner join CDXP.AP_SUPPLIER_SITE_ALL b on a.VENDOR_SITE_ID_FK=b.VENDOR_SITE_ID INNER JOIN CDXP.AP_SUPPLIERS c on b.VENDOR_ID=c.VENDOR_ID where a.HOURLY_DATA_TYPE='DAD' and a.VENDOR_SITE_ID_FK=" + item + " order by a.TARGET_DATE DESC");

                    dt = ds.Tables[0];
                    if (dt.Rows.Count != 0)
                    {
                        int number, number2;
                        var prevHeaderId = FnS.ExenID(@"select TOP 1 WP_GC_HOURLY_DATA_HEADER_ID_PK from [CDXP].[WP_GC_HOURLY_DATA_HEADER] where HOURLY_DATA_TYPE='DAD' and VENDOR_SITE_ID_FK=" + item + " order by TARGET_DATE DESC");

                        string vendorname = Convert.ToString(dt.Rows[0]["vendor_name"]);
                        var newHeaderId = FnS.ExenID(@"INSERT INTO [CDXP].[WP_GC_HOURLY_DATA_HEADER] OUTPUT INSERTED.WP_GC_HOURLY_DATA_HEADER_ID_PK
                select TOP 1
                VENDOR_SITE_ID_FK,
                HOURLY_DATA_TYPE,
                GENERATION_COMPANY,
                COMPLEX_WISE,
                SITE,
                BLOCK,
                FUEL,
                '" + pkdatetime + @"',
                '" + Nextday.Date + @"',
                INTIMATION_SOURCE_TELEPHONE,
                INTIMATION_SOURCE_TELEPHONE_DATE_TIME,
                INTIMATION_SOURCE_FAX,
                INTIMATION_SOURCE_FAX_DATE_TIME,
                INTIMATION_SOURCE_PORTAL,
                STATUS,
                ACK_TIME,
                 '" + vendorname + @"',
                null,
                'System Generated Entry',            
                RECEIVER_NAME,
                RECEIVER_DESIGNATION,
                RECEIVER_REMARKS,
                ATTRIBUTE1,
                ATTRIBUTE2,
                ATTRIBUTE3,
                ATTRIBUTE4,
                ATTRIBUTE5,
                POWER_POLICIY,
                PLT_BLK_FUEL_ID,
                IS_REVISED,
                PARENT_ID,
                CURRENT_AVAILABILITY,
                REVISED_AVAILABILITY,
               '" + pkdatetime + @"',
                157,
               '" + pkdatetime + @"',
                LAST_UPDATED_BY,
                '" + pkdatetime + @"',
                REVISION_TYPE,
                FLAG,
                '" + pkdatetime + @"',
                WITH_AMBIENT_TEMPERATURE,
				IPP_CAT
                from [CDXP].[WP_GC_HOURLY_DATA_HEADER] where HOURLY_DATA_TYPE='DAD' and VENDOR_SITE_ID_FK=" + item + " order by TARGET_DATE DESC");

                        if (int.TryParse(newHeaderId, out number))
                        {
                            var newAvailability = 0.0000;
                            var parentDAC = FnS.ExenID(@"SELECT PARENT_ID FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK = " + prevHeaderId);
                            //Get the Availability of DAC on which DAD is made
                            var parentDACAvail = FnS.ExenID(@"SELECT AVAILABILITY FROM CDXP.WP_GC_HOURLY_DATA_DETAIL WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK = " + parentDAC);

                            var prevHeaderIdAvail = FnS.ExenID(@"SELECT AVAILABILITY FROM CDXP.WP_GC_HOURLY_DATA_DETAIL WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK = " + prevHeaderId);

                            if (Convert.ToDouble(prevHeaderIdAvail) > Convert.ToDouble(parentDACAvail))
                            {
                                newAvailability = Convert.ToDouble(parentDACAvail);
                            }
                            else
                            {
                                newAvailability = Convert.ToDouble(prevHeaderIdAvail);
                            }


                            var ret = FnS.Exec(@"INSERT INTO [CDXP].[WP_GC_HOURLY_DATA_DETAIL]
                            select 
                            " + newHeaderId + @",
                            BLOCK_OR_FUEL_ID_FK,
                            TARGET_HOUR,
                            AMBIENT_TEMPERATURE,
                            AMBIENT_AVAILABILITY,'" +
                            newAvailability + @"',
                            '',
                            AVAILABILITY_AS_PER_SCH_10,
                            DACLARED_AVAILABILITY,
                            ATTRIBUTE1,
                            ATTRIBUTE2,
                            ATTRIBUTE3,
                            ATTRIBUTE4,
                            ATTRIBUTE_5,
                            GETDATE(),
                            SCH_10_ID,
                            FUEL_TYPE,
                            REVISED_TEMPERATURE,
                            HOURLY_CAP,
				            HEAD,
				            INFLOW
                            FROM 
                            [CDXP].[WP_GC_HOURLY_DATA_DETAIL]
                            WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK =" + prevHeaderId + "");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }


        private bool AUTO_DECLARATION_OF_AVAIL_CAPACITY(DateTime pkdatetime)
        {
            try
            {
                string sql0 = "";
                sql0 = "select VENDOR_SITE_ID from CDXP.AP_SUPPLIER_SITE_ALL a inner join CDXP.PPA_HEADER  C ON a.VENDOR_SITE_ID=c.VENDOR_SITE_ID_FK where c.IPP_CATEGORY IN (10,20,40)"; //Hydel, Thermal, Baggase
                var data0 = FnS.FillDSet(sql0);
                List<string> values0 = new List<string>();
                if (data0 != null)
                {
                    var dt_0 = data0.Tables[0];
                    foreach (DataRow row0 in dt_0.Rows)
                    {
                        values0.Add(Convert.ToString(row0[0]));
                    }
                }

                foreach (string item in values0) // Loop through List with foreach
                {
                    DateTime Nextday;
                    int number, number2;
                    DataSet ds = new DataSet();
                    DataTable dt = new DataTable();
                    var DAC_Before_Sub_Hour = Convert.ToInt32(FnS.ExenID(@"SELECT DAC_Before_Sub_Hour FROM [CDXP].[PPA_HEADER] WHERE VENDOR_SITE_ID_FK = " + item));


                    if (DAC_Before_Sub_Hour < 24)
                    {
                        Nextday = pkdatetime.AddDays(1);
                    }
                    else
                    {
                        Nextday = pkdatetime.AddDays(2);
                    }


                    var x = FnS.ExenID(@" SELECT COUNT(*) FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE VENDOR_SITE_ID_FK =" + item + " AND TARGET_DATE = '" + Nextday.Date + "'");

                    if (x == "0")
                    {
                        ds = FnS.FillDSet("SELECT TOP(1)* FROM [CDXP].[WP_GC_HOURLY_DATA_HEADER] a inner join CDXP.AP_SUPPLIER_SITE_ALL b on a.VENDOR_SITE_ID_FK=b.VENDOR_SITE_ID INNER JOIN CDXP.AP_SUPPLIERS c on b.VENDOR_ID=c.VENDOR_ID WHERE a.HOURLY_DATA_TYPE='DAC' AND (a.STATUS ='Submitted' OR a.STATUS='Acknowledged') AND a.VENDOR_SITE_ID_FK = " + item + " ORDER BY a.TARGET_DATE DESC");

                        dt = ds.Tables[0];
                        if (dt.Rows.Count != 0)
                        {
                            var prevHeaderId = FnS.ExenID(@"select TOP 1 WP_GC_HOURLY_DATA_HEADER_ID_PK from [CDXP].[WP_GC_HOURLY_DATA_HEADER] where HOURLY_DATA_TYPE='DAC' and VENDOR_SITE_ID_FK=" + item + " AND (STATUS ='Submitted' OR STATUS='Acknowledged') AND TARGET_DATE != '" + Nextday.Date + "' order by TARGET_DATE DESC");

                            string vendorname = Convert.ToString(dt.Rows[0]["vendor_name"]);
                            var newHeaderId = FnS.ExenID(@"INSERT INTO [CDXP].[WP_GC_HOURLY_DATA_HEADER] OUTPUT INSERTED.WP_GC_HOURLY_DATA_HEADER_ID_PK
                select TOP 1
                VENDOR_SITE_ID_FK,
                HOURLY_DATA_TYPE,
                GENERATION_COMPANY,
                COMPLEX_WISE,
                SITE,
                BLOCK,
                FUEL,
                '" + pkdatetime + @"',
                '" + Nextday.Date + @"',
                INTIMATION_SOURCE_TELEPHONE,
                INTIMATION_SOURCE_TELEPHONE_DATE_TIME,
                INTIMATION_SOURCE_FAX,
                INTIMATION_SOURCE_FAX_DATE_TIME,
                INTIMATION_SOURCE_PORTAL,
                STATUS,
                ACK_TIME,
                '" + vendorname + @"',
                '',
                'System Generated Entry',            
                RECEIVER_NAME,
                RECEIVER_DESIGNATION,
                RECEIVER_REMARKS,
                ATTRIBUTE1,
                ATTRIBUTE2,
                ATTRIBUTE3,
                ATTRIBUTE4,
                ATTRIBUTE5,
                POWER_POLICIY,
                PLT_BLK_FUEL_ID,
                IS_REVISED,
                PARENT_ID,
                CURRENT_AVAILABILITY,
                REVISED_AVAILABILITY,
                '" + pkdatetime + @"',
                157,
               '" + pkdatetime + @"',
                LAST_UPDATED_BY,
                '" + pkdatetime + @"',
                REVISION_TYPE,
                FLAG,
                '" + pkdatetime + @"',
                WITH_AMBIENT_TEMPERATURE,
				IPP_CAT
                from [CDXP].[WP_GC_HOURLY_DATA_HEADER] where HOURLY_DATA_TYPE='DAC'  AND (STATUS ='Submitted' OR STATUS='Acknowledged') AND VENDOR_SITE_ID_FK=" + item + " order by TARGET_DATE DESC");

                            if (int.TryParse(newHeaderId, out number))
                            {

                                var ret = FnS.Exec(@"INSERT INTO [CDXP].[WP_GC_HOURLY_DATA_DETAIL]
                select 
                " + newHeaderId + @",
                BLOCK_OR_FUEL_ID_FK,
                TARGET_HOUR,
                AMBIENT_TEMPERATURE,
                AMBIENT_AVAILABILITY,
                [CDXP].[FN_LAST_REVISION_BY_DACID](" + prevHeaderId + @", CDXP.WP_GC_HOURLY_DATA_DETAIL.TARGET_HOUR) AS AVAILABILITY,
                null,
                AVAILABILITY_AS_PER_SCH_10,
                DACLARED_AVAILABILITY,
                ATTRIBUTE1,
                ATTRIBUTE2,
                ATTRIBUTE3,
                ATTRIBUTE4,
                ATTRIBUTE_5, 
                GETDATE(),
				SCH_10_ID,
                FUEL_TYPE,
                REVISED_TEMPERATURE,
                HOURLY_CAP,
                HEAD,
                INFLOW
                FROM 
                [CDXP].[WP_GC_HOURLY_DATA_DETAIL]
                WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK =" + prevHeaderId + "");

                                if ((int.TryParse(ret, out number2)) == false)
                                {
                                    FnS.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_HEADER WHERE WP_GC_HOURLY_DATA_HEADER_ID_PK='" + newHeaderId + "'");
                                    FnS.Exec("DELETE FROM CDXP.WP_GC_HOURLY_DATA_DETAIL WHERE WP_GC_HOURLY_DATA_HEADER_ID_FK='" + newHeaderId + "'");
                                    return false;// "Error occured while inserting Detail for HOURLY_HEADER_ID" + newHeaderId;
                                }
                            }
                            else
                            {
                                return false;// "Error occured while inserting Header against HOURLY_HEADER_ID" + prevHeaderId; ;
                            }
                        }


                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;

        }
    }
}