using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDXPWeb.Models;
using ExcelDataReader;
using System.Transactions;
using System.Data.SqlClient;

namespace CDXPWeb.Controllers
{
    public class AdminstrationController : Controller
    {
        private clsSQLCore Fn = new clsSQLCore();
        // GET: Adminstration

        [HttpPost]
        public ActionResult UserSettings()
        {
            // Fn.sessionCheck();


            ViewBag.UserEmails = Fn.ExenID(@"select ISNULL(Email,'') Email from [CPPA_CA].[ApiUsers] where UserID= [CDXP].[FN_WP_PORTAL_USERS_ID_BY_USER_NAME]('" + Convert.ToString(Session["UserName"]) + @"')");


            return View();
        }

        public ActionResult UserAccess()
        {
            // Fn.sessionCheck();
            ViewBag.Useraccess = Fn.Data2Json(@"select UserId, Name
                 from [CPPA_CA].[ApiUsers]
                 where UserType = 2 or UserType = 3 order by 2");

            return View();
        }

        public ActionResult FileMigration()
        {
            return View();
        }

        public ActionResult UploadGraphs()
        {
            ViewBag.GraphTypes = Fn.Data2Json(@"select LuGraphTypes_Id, LuGraphTypes_ChartType from [MO].[LuGraphTypes]");
            return View();
        }

        [HttpPost]
        public string InsertGraphData()
        {

            Int32 GraphType = 0;
            String GraphDate = "";
            String AdditionalRemarks = "";
            String BarColor = null;
            String LineColor = null;

            String result = "";
            try
            {
                if (Request.Files.Count > 0)
                {
                    foreach (string key in Request.Form.AllKeys)
                    {
                        if (key.StartsWith("GraphType"))
                        {
                            GraphType = Convert.ToInt32(Request.Form[key]);
                        }
                        if (key.StartsWith("GraphDate"))
                        {
                            GraphDate = Convert.ToString(Request.Form[key]);
                        }
                        if (key.StartsWith("AdditionalRemarks"))
                        {
                            AdditionalRemarks = Convert.ToString(Request.Form[key]);
                        }
                        if (key.StartsWith("BarColor"))
                        {
                            BarColor = Convert.ToString(Request.Form[key]);
                        }
                        if (key.StartsWith("LineColor"))
                        {
                            LineColor = Convert.ToString(Request.Form[key]);
                        }
                    }
                    HttpPostedFileBase file = Request.Files[0];
                    System.IO.Stream fs = file.InputStream;
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                    Byte[] bytes = br.ReadBytes((Int32)fs.Length);

                    String fileName = file.FileName;
                    String graphFileName = fileName.Split('.')[0] + "_" + GraphType + "_" + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss").Replace(":", "_");
                    String extension = fileName.Split('.')[fileName.Split('.').Length - 1];
                    String path = HttpContext.Server.MapPath("~/Content/_MOIntegrationFiles"); //Path
                    String dbFilePAth = "~/Content/_MOIntegrationFiles/" + graphFileName + "." + extension;
                    string FilePath = Path.Combine(path, Convert.ToString(graphFileName) + "." + extension);
                    if (!System.IO.File.Exists(FilePath))
                    {
                        System.IO.File.WriteAllBytes(FilePath, bytes);
                    }

                    Int32 IsAlreadyExists = Convert.ToInt32(Fn.ExenID(@"select count(1)  from [MO].[MtGraphHeader] where MtGraphHeader_GraphId=" + GraphType + " and MtGraphHeader_GraphDate='" + GraphDate + "'"));
                    if (IsAlreadyExists > 0)
                    {
                        return "Record for uploaded graph have already been uploaded. Please select some other date";
                    }
                    Int32 HeaderId = Convert.ToInt32(Fn.ExenID(@"select isnull(max(MtGraphHeader_Id)+1,1)  from [MO].[MtGraphHeader]"));

                    String query = null;
                    using (TransactionScope tranScope = new TransactionScope())
                    {
                        try
                        {

                            if (GraphType == 4 || GraphType == 1)
                            {
                                query = @"INSERT INTO [MO].[MtGraphHeader]
                           ([MtGraphHeader_ID]
                           ,[MtGraphHeader_GraphId]
                           ,[MtGraphHeader_GraphDate]
                           ,[MtGraphHeader_AdditionalRemarks]
                           ,[MtGraphHeader_FileName]
                           ,[MtGraphHeader_CreatedBy]
                           ,[MtGraphHeader_CreatedOn]
                           ,[MtGraphHeader_ExcelFilePath]
                           ,[MtGraphHeader_BarColor]
                           ,[MtGraphHeader_LineColor]
)
     VALUES
           (" + HeaderId + "," + GraphType + ",'" + GraphDate + "','" + AdditionalRemarks + "','" + fileName + "',1, dateadd(hour,5,getDate()),'" + dbFilePAth + "','#" + BarColor + "','#" + LineColor + "') ";
                            }
                            else
                            {
                                query = @"INSERT INTO [MO].[MtGraphHeader]
           ([MtGraphHeader_ID]
           ,[MtGraphHeader_GraphId]
           ,[MtGraphHeader_GraphDate]
           ,[MtGraphHeader_AdditionalRemarks]
           ,[MtGraphHeader_FileName]
           ,[MtGraphHeader_CreatedBy]
           ,[MtGraphHeader_CreatedOn]
           ,[MtGraphHeader_ExcelFilePath]
)
     VALUES
           (" + HeaderId + "," + GraphType + ",'" + GraphDate + "','" + AdditionalRemarks + "','" + fileName + "',1, dateadd(hour,5,getDate()),'" + dbFilePAth + "') ";
                            }

                            result = Fn.Exec(query);

                            if (result != "1")
                            {
                                return "Error inserting Header data";
                            }
                            DataTable graphData = ReadFromExcel1(file, GraphType);
                            if (graphData.Rows.Count < 1)
                            {
                                return "Uploaded Excel File does not contains data in all columns. Please use provided template.";
                            }
                            List<string> ColumnsContained = GetListOfColumns(GraphType);
                            for (int i = graphData.Columns.Count - 1; i >= 0; i--)
                            {
                                DataColumn dc = graphData.Columns[i];
                                if (!ColumnsContained.Contains(dc.ColumnName))
                                {
                                    graphData.Columns.Remove(dc);
                                }
                            }
                            Fn.Parameters = new List<SqlParameter>();
                            Fn.AddParameter("@GraphTypeId", GraphType);
                            Fn.AddParameter("@GraphHeaderId", HeaderId);
                            Fn.AddParameter("@tblGraphData", graphData);
                            switch (GraphType)
                            {
                                case 1:
                                    {
                                        result = Fn.ExecuteStoredProcedure("[MO].[InsertMarginalCostDemand]");
                                        break;
                                    }
                                case 2:
                                    {
                                        result = Fn.ExecuteStoredProcedure("[MO].[InsertFuelMix]");
                                        break;
                                    }
                                case 3:
                                    {
                                        result = Fn.ExecuteStoredProcedure("[MO].[InsertHourlyGenerationProfile]");
                                        break;
                                    }
                                case 4:
                                    {
                                        result = Fn.ExecuteStoredProcedure("[MO].[InsertDiscoConsumption]");
                                        break;
                                    }
                                case 5:
                                    {
                                        String DiscoId = Fn.ExenID(@"select max(MtGraphHeader_Id) from [MO].[MtGraphHeader] where MtGraphHeader_GraphDate = '" + GraphDate + "' and MtGraphHeader_GraphId = 4");
                                        if (String.IsNullOrEmpty(DiscoId))
                                        {
                                            return "Please upload Disco summary data for selected date.";
                                        }
                                        Int32 DiscoSummaryId = Convert.ToInt32(DiscoId);
                                        Fn.AddParameter("@DiscoSummaryId", DiscoSummaryId);

                                        result = Fn.ExecuteStoredProcedure("[MO].[InsertKEDemandHourly]");
                                        break;
                                    }
                                case 6:
                                    {
                                        String DiscoId = Fn.ExenID(@"select max(MtGraphHeader_Id) from [MO].[MtGraphHeader] where MtGraphHeader_GraphDate = '" + GraphDate + "' and MtGraphHeader_GraphId = 4");
                                        if (String.IsNullOrEmpty(DiscoId))
                                        {
                                            return "Please upload Disco summary data for selected date.";
                                        }
                                        Int32 DiscoSummaryId = Convert.ToInt32(DiscoId);
                                        Fn.AddParameter("@DiscoSummaryId", DiscoSummaryId);
                                        result = Fn.ExecuteStoredProcedure("[MO].[InsertDiscoDemandHourly]");
                                        break;
                                    }
                                default:
                                    Console.WriteLine("Default case");
                                    break;


                            }
                            tranScope.Complete();
                            return result;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            tranScope.Dispose();
                            return ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return result;
        }
        private List<String> GetListOfColumns(int GraphTtypeId)
        {
            List<string> ColumnNames = new List<string>();
            switch (GraphTtypeId)
            {
                case 1:
                    ColumnNames.AddRange(new List<String>() { "Hours", "Demand", "Marginal_Cost" });
                    break;
                case 2:
                    ColumnNames.AddRange(new List<String>() { "Generation_Type", "Grand Total (GWh)", "Percentage_Share" });
                    break;
                case 3:
                    ColumnNames.AddRange(new List<String>() { "Generation_Type", "Grand Total (GWh)", "Percentage_Share", "Hour 0", "Hour 1", "Hour 2", "Hour 3", "Hour 4", "Hour 5", "Hour 6", "Hour 7", "Hour 8", "Hour 9", "Hour 10", "Hour 11", "Hour 12", "Hour 13", "Hour 14", "Hour 15", "Hour 16", "Hour 17", "Hour 18", "Hour 19", "Hour 20", "Hour 21", "Hour 22", "Hour 23" });
                    break;
                case 4:
                    ColumnNames.AddRange(new List<String>() { "DISCO", "Total Energy (GWh)", "Consumption %" });
                    break;
                case 5:
                    ColumnNames.AddRange(new List<String>() { "Hours", "K-Electric Demand (MWh)" });
                    break;
                case 6:
                    ColumnNames.AddRange(new List<String>() { "DISCO_Demand", "Hour 0", "Hour 1", "Hour 2", "Hour 3", "Hour 4", "Hour 5", "Hour 6", "Hour 7", "Hour 8", "Hour 9", "Hour 10", "Hour 11", "Hour 12", "Hour 13", "Hour 14", "Hour 15", "Hour 16", "Hour 17", "Hour 18", "Hour 19", "Hour 20", "Hour 21", "Hour 22", "Hour 23", "SMSData" });
                    break;

            }
            return ColumnNames;

        }

        private DataTable ReadFromExcel1(HttpPostedFileBase upload, Int32 GraphType)
        {
            try
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = upload.InputStream;

                    // We return the interface, so that
                    IExcelDataReader reader = null;


                    if (upload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }

                    //                  reader.IsFirstRowAsColumnNames = true;

                    //                  DataSet result = reader.AsDataSet();
                    DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    DataTable dataTable = new DataTable();
                    List<String> columnNamesList = new List<string>();
                    columnNamesList = GetListOfColumns(GraphType);
                    switch (GraphType)
                    {
                        case 1:
                            dataTable = result.Tables[0];
                            break;
                        case 2:
                            dataTable = result.Tables[1];
                            break;
                        case 3:
                            dataTable = result.Tables[2];
                            break;
                        case 4:
                            dataTable = result.Tables[3];
                            break;
                        case 5:
                            dataTable = result.Tables[4];
                            break;
                        case 6:
                            dataTable = result.Tables[5];
                            break;
                    }

                    columnNamesList = columnNamesList.Select(x => x.Trim()).ToList();

                    foreach (DataColumn column in dataTable.Columns)
                    {
                        columnNamesList.Remove(column.ColumnName.Trim());
                    }
                    if (columnNamesList.Count != 0)
                    {
                        return new DataTable();
                    }
                    reader.Close();
                    return dataTable;
                    //                    return View(result.Tables[0]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return null;
        }
        // [HttpPost]
        //public void GetIPPAndUsers()
        //{
        //    // Fn.sessionCheck();


        //    //ViewBag.CppaUsers = Fn.Data2DropdownSQL(@"SELECT a,b from (
        //    //Select 0 a, 'Select' b, 0 c union
        //    //SELECT UserID, Name, 1 FROM [CPPA_CA].[ApiUsers] where UserType=2 or UserType = 3 ) d
        //    //order by d.c, d.b");

        //    ViewBag.Useraccess = Fn.Data2Json(@"select UserId, Name
        //         from [CPPA_CA].[ApiUsers]
        //         where UserType = 2 or UserType = 3 order by 2");
        //    //ViewBag.FninanceUsers = Fn.Data2DropdownSQL(@"SELECT a,b from (
        //    //Select 0 a, 'Select' b, 0 c union
        //    //SELECT UserID, Name, 1 FROM [CPPA_CA].[ApiUsers] where UserType=3  ) d
        //    //order by d.c, d.b");
        //    //ViewBag.IPPUsers = Fn.Data2DropdownSQL(@"SELECT a,b from (
        //    //Select 0 a, 'Select All' b, 0 c union
        //    //SELECT UserID, Name, 1 FROM [CPPA_CA].[ApiUsers] where UserType=4  ) d
        //    //order by d.c, d.b");

        //    //ViewBag.Vendors = Fn.Data2Json(@"SELECT APS.VENDOR_ID , APS.VENDOR_NAME,
        //    //DTL = ISNULL((Select VENDOR_SITE_ID, ADDRESS_LINE1 from [CDXP].[AP_SUPPLIER_SITE_ALL]  WHERE VENDOR_ID=APS.VENDOR_ID ORDER BY 2 FOR JSON AUTO),'[]')
        //    //FROM [CDXP].[AP_SUPPLIERS] APS ORDER BY 2");


        //}

        [HttpGet]
        public string GetIPPAndSites()
        {
            return Fn.Data2Json("select [CPPA_CA].[AP_SUPPLIERS].vendor_id, vendor_site_id, vendor_new_name, vendor_site_code, address_line1 " +
                "from [CPPA_CA].[APP_SUPPLIER_SITE_ALL] join [CPPA_CA].[AP_SUPPLIERS]" +
                " on [CPPA_CA].[AP_SUPPLIERS].vendor_id = [CPPA_CA].[APP_SUPPLIER_SITE_ALL].vendor_id order by 3");

            //return View("UserAccess");

        }

        [HttpGet]
        public string GetIPPAndUserAccess(int UserId, int VendorSiteId)
        {

            var result = "";
            if ((Convert.ToString(UserId) != "") && (Convert.ToString(VendorSiteId) != ""))
            // if (Request.Headers.Contains("VendorSiteId") && Request.Headers.Contains("UserId"))
            {
                //var vendor_site_id = Request.Headers.GetValues("VendorSiteId").FirstOrDefault();
                //var user_id = Request.Headers.GetValues("UserId").FirstOrDefault();
                result = Fn.Data2Json(@"select vendor_site_id, InquiryRights, CreateRights, EditRights, DeleteRights, HistoricalDataRights" +
                     " from [dbo].[WP_GC_USER_ACCESS] " +
                     "where vendor_site_id = '" + VendorSiteId + "' and userId = '" + UserId + "'");
            }

            return result;
        }

        [HttpPost]
        public string PostAddUserAccess(int UserId, int VendorSiteId, int VendorId, string InquiryRights, string CreateRights, string EditRights, string DeleteRights, string HistoricDataRights)
        {
            var result = "";
            if ((Convert.ToString(UserId) != "") && (Convert.ToString(VendorSiteId) != "") && (Convert.ToString(VendorId) != ""))
            {

                result = Fn.Exec("insert into [dbo].[WP_GC_USER_ACCESS] ([UserId],[VENDOR_ID],[VENDOR_SITE_ID],[EffectiveDateFrom],[EffectiveDateTo],[InquiryRights],[CreateRights],[EditRights],[DeleteRights],[HistoricalDataRights])" +
                   "values ('" + UserId + "', '" + VendorId + "', '" + VendorSiteId + "', getdate(), null, '" + InquiryRights + "', '" + CreateRights + "', '" + EditRights + "', '" + DeleteRights + "', '" + HistoricDataRights + "')");
            }

            return result;
        }

        [HttpPut]
        public string PutUpdateUserAccess(int UserId, int VendorSiteId, int VendorId, string InquiryRights, string CreateRights, string EditRights, string DeleteRights, string HistoricDataRights)
        {
            var result = "";
            if ((Convert.ToString(UserId) != "") && (Convert.ToString(VendorSiteId) != "") && (Convert.ToString(VendorId) != ""))
            {
                result = Fn.Exec("update [dbo].[WP_GC_USER_ACCESS] set inquiryRights = '" + InquiryRights + "', CreateRights = '" + CreateRights + "', EditRights = '" + EditRights + "', DeleteRights = '" + DeleteRights + "', HistoricalDataRights = '" + HistoricDataRights + "', effectiveDateFrom = getdate() " +
                   "where VENDOR_SITE_ID = '" + VendorSiteId + "' and vendor_id = '" + VendorId + "' and userid = '" + UserId + "'");
            }

            return result;
        }


        [HttpDelete]

        public string DeleteUserAccess(int UserId, int VendorSiteId, int vendorId)
        {
            var result = "";
            if ((Convert.ToString(UserId) != "") && (Convert.ToString(VendorSiteId) != "") && (Convert.ToString(vendorId) != ""))
            {
                result = Fn.Exec("delete from [dbo].[WP_GC_USER_ACCESS] where UserId = '" + UserId + "' and vendor_id = '" + vendorId + "' and vendor_site_id = '" + VendorSiteId + "'");
            }

            return result;
        }

        [HttpPost]
        public ActionResult PendingInvoceList()
        {
            // Fn.sessionCheck();
            ViewBag.PendingInvoice = Fn.Data2Json(@"EXEC GetPendingInvoiceList");
            return View();
        }
        [HttpPost]
        public ActionResult Dashboard()
        {
            // Fn.sessionCheck();
            ViewBag.ViewTitle = "Dashboard";

            ViewBag.TechnicalUsers = Fn.Data2DropdownSQL(@"SELECT a,b from (
                Select 0 a, 'Select' b, 0 c union
                SELECT UserID, Name, 1 FROM [CPPA_CA].[ApiUsers] where UserType=2  ) d
                order by d.c, d.b");
            ViewBag.FninanceUsers = Fn.Data2DropdownSQL(@"SELECT a,b from (
                Select 0 a, 'Select' b, 0 c union
                SELECT UserID, Name, 1 FROM [CPPA_CA].[ApiUsers] where UserType=3  ) d
                order by d.c, d.b");
            ViewBag.IPPUsers = Fn.Data2DropdownSQL(@"SELECT a,b from (
                Select 0 a, 'Select All' b, 0 c union
                SELECT UserID, Name, 1 FROM [CPPA_CA].[ApiUsers] where UserType=4  ) d
                order by d.c, d.b");
            return View();
        }
        public ActionResult MenuRegistration()
        {
            ViewBag.ViewTitle = "Menu Registration";
            return View();
        }

        public ActionResult MenuViewRegistration()
        {
            ViewBag.ViewTitle = "Menu View Registration";
            return View();
        }
        public ActionResult UserRegistration()
        {
            ViewBag.ViewTitle = "User Registration";
            return View();
        }
        public ActionResult UserTypeMenuAccess()
        {
            ViewBag.ViewTitle = "UserType Menu Access";
            return View();
        }
        public ActionResult UserEntityAccess()
        {
            ViewBag.ViewTitle = "User Entity Access";
            return View();
        }


        public string GetAllDiariesbyUser4Admin(string id)
        {
            //Fn.sessionCheck();

            string Returnvls = "";
            if (Convert.ToString(id.Split('½')[3]) == "4")
            {
                Returnvls = Fn.Data2Json("EXEC GetDHIAllbyUserID_BetweenSubmissionDate " + Convert.ToString(id.Split('½')[0]) + " , '" + Convert.ToString(id.Split('½')[1]) + "', '" + Convert.ToString(id.Split('½')[2]) + "'");
            }
            else if (Convert.ToString(id.Split('½')[3]) == "2" || Convert.ToString(id.Split('½')[3]) == "3")
            {
                Returnvls = Fn.Data2Json("EXEC GetDHIbyStatus_BetweenSubmissionDate " + Convert.ToString(id.Split('½')[0]) + ", '" + Convert.ToString(id.Split('½')[1]) + "' ,'" + Convert.ToString(id.Split('½')[2]) + "'");
            }

            return Returnvls;
        }

        [HttpPost]
        public string Meter_Reading_Proforma()
        {

            var frmdata = HttpContext.Request.Form["vls"];
            string[] dataID = new string[500];
            dataID = Fn.CleanSQL(HttpUtility.UrlDecode(frmdata)).Split('½');
            string Returnvls = "";
            try
            {
                switch (Convert.ToInt32(dataID[0]))
                {
                    case 0:
                        Returnvls = Fn.Exec(@"INSERT INTO tblHospital
                                      (HospitalName, HospitalAbbreviation, HospitalAddress, tblTehsilID, strPhoneNo, strFaxNo, strLogo)
                    VALUES ('" + dataID[1] + "','" + dataID[2] + "','" + dataID[3] + "','" + dataID[4] + "','" + dataID[5] + "','" + dataID[6] + "','" + dataID[7] + "')");
                        break;
                    case 1:
                        Returnvls = Fn.HTMLTableWithID_TR_Tag(@"SELECT        WP_SETUP_MENU_ID,'' AS Sr#,  MENU_NAME Title, Herf_LINK [href Link], CLASS_NAME [Class], ORDER_BY [Order Number], DESCRIPTION [Description]
                                                                FROM            CDXP.WP_SETUP_MENU
                                                                WHERE        (PARENT_ID IS NULL)
                                                                order by [Order Number]", "tblJ1");
                        break;

                    case 2:
                        Returnvls = Fn.Exec(@"DELETE tblHospital Where tblHospitalID = " + dataID[1]);
                        break;

                    case 3:
                        Returnvls = Fn.Exec(@"UPDATE tblHospital
                    SET          HospitalName ='" + dataID[1] + @"', HospitalAbbreviation ='" + dataID[2] + @"', HospitalAddress ='" + dataID[3] + @"', strCity ='" + dataID[4] + @"', strPhoneNo ='" + dataID[5] + @"', strFaxNo ='" + dataID[6] + @"', strLogo ='" + dataID[7] + @"'
                    where tblHospitalID='" + dataID[8] + @"'");
                        break;

                    case 4:
                        Returnvls = Fn.Data2Json(@"select tblHospitalID,HospitalName from tblHospital order by HospitalName");
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
    }
}