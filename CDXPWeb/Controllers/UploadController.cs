using CDXPWeb.Models;
using Microsoft.Office.SharePoint.Tools;
using Microsoft.SharePoint.Client;
using Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CDXPWeb.Controllers
{
    public class UploadController : Controller
    {
        private clsSQLCore Fn = new clsSQLCore();
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
        public ActionResult UploadFile()
        {
            sessionCheck();
            return View();
        }
        [HttpPost]

        public string UploadFileWithFile()
        {

            var frmdata = HttpContext.Request.Form["vls"];
            string[] d = frmdata.Split('½');

            HttpFileCollectionBase SelectedFiles = HttpContext.Request.Files;






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
            string destinationSiteUrl = Convert.ToString(Session["SPHostUrl"]);
            ClientContext clientContext = new ClientContext(destinationSiteUrl);
            clientContext.Credentials = new SharePointOnlineCredentials(userName, securePassWd);

            try
            {
                string rid = "";
                for (int i = 0; i < SelectedFiles.Count; i++)
                {
                    HttpPostedFileBase file = SelectedFiles[i];
                    if (file.ContentLength > 0)
                    {
                        ListItem result = UploadFileList(clientContext, "Documents", file.FileName, file.InputStream, d[0]);
                        ViewBag.Message = "File Uploaded Successfully!! URL is http://172.16.10.54/ecm/folder/download/" + result["_dlc_DocId"].ToString() + " and Item Id" + result.Id.ToString();

                        Fn.Exec(@"INSERT INTO [CDXP].[tblFiles] (strFileName,strSPUrl,strDescription) VALUES ('" + file.FileName + @"','http://172.16.10.54/ecm/folder/download/" + result["_dlc_DocId"].ToString() + @"','" + d[0] + @"')");
                        rid = Convert.ToString(result.Id);

                    }
                }




                string objurl = "https://prod-15.westeurope.logic.azure.com:443/workflows/2d08ebbbf7ed420eb4dbb7e71c173cb3/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=80pOZbDcBdzo148xQb5UC6feDLrG0STQAB5CsFAdYtA";
                Uri u = new Uri(objurl);
                var payload = "{'DocId': '" + rid + @"','LibraryName': 'Documents' ,'SiteUrl': '" + destinationSiteUrl + @"' ,'SiteName': 'Information Technology Department' ,'currentUserItemId': '0' ,'WebId': '" + webid + @"'}";

                HttpContent cc = new StringContent(payload, Encoding.UTF8, "application/json");
                var t = Task.Run(() => PostURI(u, cc));
                t.Wait();




                return "1½" + webid + "½" + rid;
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error Message :" + ex.Message + ex.Source + ex.StackTrace;
                return ex.Message;
            }
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


        public string webid { get; set; }
        private ListItem UploadFileList(ClientContext context, string listTitle, string fileName, Stream fs, string strMetadata)
        {
            try
            {
                var list = context.Web.Lists.GetByTitle(listTitle);
                context.Load(list.RootFolder);
                context.ExecuteQuery();
                var fileUrl = String.Format("{0}/{1}", "/departments/it/Shared%20Documents/ERP/Correspondance/Letters", fileName); //list.RootFolder.ServerRelativeUrl, fileName);

                Microsoft.SharePoint.Client.File.SaveBinaryDirect(context, fileUrl, fs, true);

                var web = context.Web;
                User newUser = web.EnsureUser(Convert.ToString(Session["usr"]));
                context.Load(newUser);
                context.ExecuteQuery();

                FieldUserValue userValue = new FieldUserValue();
                userValue.LookupId = newUser.Id;
                var usr = newUser.Id + ";#" + Session["usr"].ToString();
                Session.Add("newUsr", usr);
                var f = web.GetFileByServerRelativeUrl(fileUrl);
                var item = f.ListItemAllFields;
                item["Title"] = strMetadata;
                item["Author"] = usr;
                item["Editor"] = usr;
                item.SystemUpdate();
                context.Load(item, i => i["_dlc_DocId"], i => i.Id);

                //context.Load(item, i => i.Id);
                context.ExecuteQuery();
                var site = context.Web;
                context.Load(site);
                context.ExecuteQuery();

                webid = site.Id.ToString();
                return item;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}