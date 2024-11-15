using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace CDXPWeb.Models
{
    public class clsSQLCoreOld
    {
        public SqlConnection CN = new SqlConnection();
        public string SQL_Str = Convert.ToString(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnectionOld"]);
        public clsSQLCoreOld()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public string CleanSQL(string Txt)
        {
            string str = Txt.Replace("'", "''");
            return str;
        }

        public void CnStr()
        {
            if (CN.State == ConnectionState.Open)

                CN.Close();
            CN.ConnectionString = SQL_Str;
        }


        public string Exec(string str)
        {
            string Out;
            try
            {
                CnStr();
                CN.Open();
                SqlCommand cmd = new SqlCommand(str, CN);
                Out = cmd.ExecuteNonQuery().ToString();

                CN.Close();
            }
            catch (System.Exception ex)
            {
                Out = ex.Message;
            }
            return Out;
        }
        public string ExenID(string str)
        {
            string ID;
            try
            {
                CnStr();
                CN.Open();
                SqlCommand cmd = new SqlCommand(str, CN);
                ID = cmd.ExecuteScalar().ToString();
                CN.Close();
            }

            catch (System.Exception ex)
            {
                ID = ex.Message;
            }
            return ID;
        }



        public string Data2Dropdown<T>(IList<T> dt)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            string ret = "";
            foreach (T item in dt)
            {
                ret += "<option value='" + Convert.ToString(properties[0].GetValue(item)) + "'>" + Convert.ToString(properties[1].GetValue(item)) + "</option>";
            }
            return ret;
        }
        public string DespatchData2Dropdown<T>(IList<T> dt2)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            string ret = "";
            foreach (T item in dt2)
            {
                ret += "<option value='" + Convert.ToString(properties[0].GetValue(item)) + "'>" + Convert.ToString(properties[1].GetValue(item)) + "</option>";
            }
            return ret;
        }

        public string Data2Json(string str)
        {
            try
            {
                DataTable dt = FillDSet(str).Tables[0];
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;
                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }
                return serializer.Serialize(rows);
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        public DataSet FillDSet(string cmd)
        {
            DataSet MyDataSet = new DataSet();
            try
            {
                System.Data.SqlClient.SqlDataAdapter MyDataAdapter = new System.Data.SqlClient.SqlDataAdapter(cmd, SQL_Str);
                MyDataAdapter.Fill(MyDataSet);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return MyDataSet;
        }
        public string Data2DropdownSQL(string SQL)
        {
            DataSet rlst = FillDSet(SQL);
            string outstring = "";

            for (int i = 0; i < rlst.Tables[0].Rows.Count; i++)
            {
                outstring = outstring + "<option value='" + rlst.Tables[0].Rows[i][0] + "'>" + rlst.Tables[0].Rows[i][1] + "</option>";
            }

            return outstring;
        }
        public string HTMLTableWithID_TR_Tag(string SQL, string tblId)
        {
            DataSet rlst = FillDSet(SQL);
            string outstring = "<table id='" + tblId + "' class='table table-striped table-bordered table-hover'><thead><tr>";
            int colCount = rlst.Tables[0].Columns.Count;
            for (int i = 1; i < colCount; i++)
            {
                outstring = outstring + "<th>" + rlst.Tables[0].Columns[i].ColumnName + "</th>";
            }
            outstring = outstring + "</tr></thead><tbody>";
            for (int i = 0; i < rlst.Tables[0].Rows.Count; i++)
            {
                outstring = outstring + "<tr tag='" + rlst.Tables[0].Rows[i][0] + "'>";
                for (int j = 1; j < colCount; j++)
                {
                    outstring = outstring + "<td>" + rlst.Tables[0].Rows[i][j] + "</td>";
                }
                outstring = outstring + "</tr>";
            }

            string ssssssssssssss = outstring + "</tbody>";
            return outstring + "</tbody></table>";
        }


        public string HTMLTableWithID_TR_TagNext(string SQL, string tblId)
        {
            DataSet rlst = FillDSet(SQL);
            string outstring = "<table id='" + tblId + "' class='table table-striped table-bordered table-hover'><thead><tr>";
            int colCount = rlst.Tables[0].Columns.Count;
            decimal output = 0;
            bool isNumeric = false;

            for (int i = 2; i < colCount; i++)
            {


                if (rlst.Tables[0].Columns[i].ColumnName == "ENG DELIVERED" ||
                    rlst.Tables[0].Columns[i].ColumnName == "MDI" ||
                    rlst.Tables[0].Columns[i].ColumnName == "Energy (kWh)" ||
                    rlst.Tables[0].Columns[i].ColumnName == "MDI (kW)" ||
                    rlst.Tables[0].Columns[i].ColumnName == "Eng Import" ||
                    rlst.Tables[0].Columns[i].ColumnName == "Eng Export" ||
                    rlst.Tables[0].Columns[i].ColumnName == "MDI Import" ||
                    rlst.Tables[0].Columns[i].ColumnName == "MDI EXPORT")
                {
                    outstring = outstring + "<th class='text-right'>" + rlst.Tables[0].Columns[i].ColumnName + "</th>";
                }
                else
                {
                    outstring = outstring + "<th>" + rlst.Tables[0].Columns[i].ColumnName + "</th>";
                }

            }

            outstring = outstring + "</tr></thead><tbody>";
            for (int i = 0; i < rlst.Tables[0].Rows.Count; i++)
            {
                outstring = outstring + "<tr tag='" + rlst.Tables[0].Rows[i][0] + "' DisableEdit='" + rlst.Tables[0].Rows[i][1] + "'>";
                for (int j = 2; j < colCount; j++)
                {
                    isNumeric = decimal.TryParse(rlst.Tables[0].Rows[i][j].ToString(), out output);

                    if (isNumeric)
                    {
                        outstring = outstring + "<td class='text-right'>" + Convert.ToDecimal(rlst.Tables[0].Rows[i][j]).ToString("N0") + "</td>";
                    }
                    else
                    {
                        outstring = outstring + "<td>" + rlst.Tables[0].Rows[i][j].ToString().Replace(Environment.NewLine, "<br />") + "</td>";
                    }
                }
                outstring = outstring + "</tr>";
            }

            string ssssssssssssss = outstring + "</tbody>";
            return outstring + "</tbody></table>";
        }



        public string SendMail(string UserID, string Subject, string TransactionNo)
        {
            //DateTime utc = DateTime.UtcNow;
            //TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            //DateTime pkdatetime = TimeZoneInfo.ConvertTimeFromUtc(utc, tzi);

            //List<sp_DHI_Email_InfoNEWResult> amail = new List<sp_DHI_Email_InfoNEWResult>();
            string ret = "1";
            string Body = "";
            //////using (DBDataContext db = new DBDataContext())
            //////{
            //////    amail = db.sp_DHI_Email_InfoNEW(Convert.ToInt64(TransactionNo), UserID, pkdatetime, "Submitted", "").ToList<sp_DHI_Email_InfoNEWResult>();
            //////}

            if (UserID.Split('½')[3].Trim() != "")//(amail.Count > 0 && amail[0].Email.Length > 20)
            {
                string byto = TransactionNo == "submitted" ? "to" : "by";
                string MailTo = UserID.Split('½')[3];// amail[0].Email;
                Body = "<br>It is informed that the Metering data of following CDPs from <strong>" + UserID.Split('½')[0] + "</strong>  for month of <strong>" + UserID.Split('½')[2] + @"</strong> has been " + TransactionNo + @" at <strong>" + UserID.Split('½')[5] + @"</strong>  " + byto + @" CPPA-G.<br><br> <strong>CDPs Details</strong><br>" + UserID.Split('½')[1] + @"<br><br>";
                string tmplt = @"<!DOCTYPE html PUBLIC ' -//W3C//DTD XHTML 1.0 Strict//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd'> <html xmlns='http://www.w3.org/1999/xhtml'> <head> <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /> <meta name='viewport' content='width=device-width, initial-scale=1.0'> <meta http-equiv='X-UA-Compatible' content='IE=edge,chrome=1'> <title>CPPA-G EMAIL</title> <style type='text/css'> /* reset */ article,aside,details,figcaption,figure,footer,header,hgroup,nav,section,summary{display:block}audio,canvas,video{display:inline-block;*display:inline;*zoom:1}audio:not([controls]){display:none;height:0}[hidden]{display:none}html{font-size:100%;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%}html,button,input,select,textarea{font-family:sans-serif}body{margin:0}a:focus{outline:thin dotted}a:active,a:hover{outline:0}h1{font-size:2em;margin:0 0 0}h2{font-size:1.5em;margin:0 0 .83em 0}h3{font-size:1.17em;margin:1em 0}h4{font-size:1em;margin:1.33em 0}h5{font-size:.83em;margin:1.67em 0}h6{font-size:.75em;margin:2.33em 0}abbr[title]{border-bottom:1px dotted}b,strong{font-weight:bold}blockquote{margin:1em 40px}dfn{font-style:italic}mark{background:#ff0;color:#000}p,pre{margin:1em 0}code,kbd,pre,samp{font-family:monospace,serif;_font-family:'courier new',monospace;font-size:1em}pre{white-space:pre;white-space:pre-wrap;word-wrap:break-word}q{quotes:none}q:before,q:after{content:'';content:none}small{font-size:75%}sub,sup{font-size:75%;line-height:0;position:relative;vertical-align:baseline}sup{top:-0.5em}sub{bottom:-0.25em}dl,menu,ol,ul{margin:1em 0}dd{margin:0 0 0 40px}menu,ol,ul{padding:0 0 0 40px}nav ul,nav ol{list-style:none;list-style-image:none}img{border:0;-ms-interpolation-mode:bicubic}svg:not(:root){overflow:hidden}figure{margin:0}form{margin:0}fieldset{border:1px solid #c0c0c0;margin:0 2px;padding:.35em .625em .75em}legend{border:0;padding:0;white-space:normal;*margin-left:-7px}button,input,select,textarea{font-size:100%;margin:0;vertical-align:baseline;*vertical-align:middle}button,input{line-height:normal}button,html input[type='button'],input[type='reset'],input[type='submit']{-webkit-appearance:button;cursor:pointer;*overflow:visible}button[disabled],input[disabled]{cursor:default}input[type='checkbox'],input[type='radio']{box-sizing:border-box;padding:0;*height:13px;*width:13px}input[type='search']{-webkit-appearance:textfield;-moz-box-sizing:content-box;-webkit-box-sizing:content-box;box-sizing:content-box}input[type='search']::-webkit-search-cancel-button,input[type='search']::-webkit-search-decoration{-webkit-appearance:none}button::-moz-focus-inner,input::-moz-focus-inner{border:0;padding:0}textarea{overflow:auto;vertical-align:top}table{border-collapse:collapse;border-spacing:0} /* custom client-specific styles including styles for different online clients */ .ReadMsgBody{width:100%;} .ExternalClass{width:100%;} /* hotmail / outlook.com */ .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div{line-height:100%;} /* hotmail / outlook.com */ table, td{mso-table-lspace:0pt; mso-table-rspace:0pt;} /* Outlook */ #outlook a{padding:0;} /* Outlook */ img{-ms-interpolation-mode: bicubic;display:block;outline:none; text-decoration:none;} /* IExplorer */ body, table, td, p, a, li, blockquote{-ms-text-size-adjust:100%; -webkit-text-size-adjust:100%; font-weight:normal!important;} .ExternalClass td[class='ecxflexibleContainerBox'] h3 {padding-top: 10px !important;} /* hotmail */ /* email template styles */ h1{display:block;font-size:26px;font-style:normal;font-weight:normal;line-height:100%;} h2{display:block;font-size:20px;font-style:normal;font-weight:normal;line-height:120%;} h3{display:block;font-size:17px;font-style:normal;font-weight:normal;line-height:110%;} h4{display:block;font-size:18px;font-style:italic;font-weight:normal;line-height:100%;} .flexibleImage{height:auto;} br{line-height:0%;} table[class=flexibleContainerCellDivider] {padding-bottom:0 !important;padding-top:0 !important;} body, #bodyTbl{background-color:#E1E1E1;} #emailHeader{background-color:#E1E1E1;} #emailBody{background-color:#FFFFFF;} #emailFooter{background-color:#E1E1E1;} .textContent {color:#8B8B8B; font-family:Helvetica; font-size:16px; line-height:125%; text-align:Left;} .textContent a{color:#205478; text-decoration:underline;} .emailButton{background-color:#205478; border-collapse:separate;} .buttonContent{color:#FFFFFF; font-family:Helvetica; font-size:18px; font-weight:bold; line-height:100%; padding:15px; text-align:center;} .buttonContent a{color:#FFFFFF; display:block; text-decoration:none!important; border:0!important;} #invisibleIntroduction {display:none;display:none !important;} /* hide the introduction text */ /* other framework hacks and overrides */ span[class=ios-color-hack] a {color:#275100!important;text-decoration:none!important;} /* Remove all link colors in IOS (below are duplicates based on the color preference) */ span[class=ios-color-hack2] a {color:#205478!important;text-decoration:none!important;} span[class=ios-color-hack3] a {color:#8B8B8B!important;text-decoration:none!important;} /* phones and sms */ .a[href^='tel'], a[href^='sms'] {text-decoration:none!important;color:#606060!important;pointer-events:none!important;cursor:default!important;} .mobile_link a[href^='tel'], .mobile_link a[href^='sms'] {text-decoration:none!important;color:#606060!important;pointer-events:auto!important;cursor:default!important;} /* responsive styles */ @media only screen and (max-width: 480px){ body{width:100% !important; min-width:100% !important;} table[id='emailHeader'], table[id='emailBody'], table[id='emailFooter'], table[class='flexibleContainer'] {width:100% !important;} td[class='flexibleContainerBox'], td[class='flexibleContainerBox'] table {display: block;width: 100%;text-align: left;} td[class='imageContent'] img {height:auto !important; width:100% !important; max-width:100% !important;} img[class='flexibleImage']{height:auto !important; width:100% !important;max-width:100% !important;} img[class='flexibleImageSmall']{height:auto !important; width:auto !important;} table[class='flexibleContainerBoxNext']{padding-top: 10px !important;} table[class='emailButton']{width:100% !important;} td[class='buttonContent']{padding:0 !important;} td[class='buttonContent'] a{padding:15px !important;} br{line-height:0%;} } </style> <!-- MS Outlook custom styles --> <!--[if mso 12]> <style type='text/css'> .flexibleContainer{display:block !important; width:100% !important;} </style> <![endif]--> <!--[if mso 14]> <style type='text/css'> .flexibleContainer{display:block !important; width:100% !important;} </style> <![endif]--> </head> <body bgcolor='#E1E1E1' leftmargin='0' marginwidth='0' topmargin='0' marginheight='0' offset='0'> <center style='background-color:#E1E1E1;'> <table border='0' cellpadding='0' cellspacing='0' height='100%' width='100%' id='bodyTbl' style='table-layout: fixed;max-width:100% !important;width: 100% !important;min-width: 100% !important;'> <tr> <td align='center' valign='top' id='bodyCell'> <div style='font-family:Helvetica,Arial,sans-serif;font-size:13px;color:#828282;text-align:center;line-height:120%;'> If you have any troubles, you can see this email <a href='#' target='_blank' style='text-decoration:none;border-bottom:1px solid #828282;color:#828282;'><span style='color:#828282;'>in your browser</span></a>. </div> <table style='border: none;border-spacing: 0px;'> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='80%' style='color:#FFFFFF;' bgcolor='#3498db'> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='100%' class='flexibleContainer'> <tr> <td align='center' valign='top' width='100%' class='flexibleContainerCell'> <table border='0' cellpadding='10' cellspacing='0' width='100%'> <tr> <td align='center' valign='top' class='textContent'> <h1 style='color:#FFFFFF;line-height:100%;font-family:Helvetica,Arial,sans-serif;font-size:35px;font-weight:normal;text-align:left;'>CPPA-G</h1> <h2 style='text-align:left;font-weight:normal;font-family:Helvetica,Arial,sans-serif;font-size:23px;margin-bottom:5px;color:#fff;line-height:135%;background:#3498db;padding: 0;'>Central Power Purchasing Agency(G) Ltd.</h2> <div style='text-align:left;font-family:Helvetica,Arial,sans-serif;font-size:15px;margin-bottom:0;color:#000;line-height:135%;'>'To become a world-class power market operator by providing the optimum environment for trading electricity in the Pakistani power market'</div> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> </td> </tr> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='80%' bgcolor='#FFFFFF'> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='100%' class='flexibleContainer'> <tr> <td align='center' valign='top' width='100%' class='flexibleContainerCell'> <table border='0' cellpadding='10' cellspacing='0' width='100%'> <tr> <td align='center' valign='top'> <table border='0' cellpadding='0' cellspacing='0' width='100%' style='margin-bottom:20px;'> <tr> <td valign='top' class='textContent'> <h3 style='color:#5F5F5F;line-height:125%;font-family:Helvetica,Arial,sans-serif;font-size:20px;font-weight:normal;margin-top:0;margin-bottom:3px;text-align:left;text-decoration: underline;'>" + Subject + @"</h3> <div style='text-align:left;font-family:Helvetica,Arial,sans-serif;font-size:15px;margin-bottom:0;color:#5F5F5F;line-height:135%;'>" + Body + @"</div> </td> </tr> </table> <table border='0' cellpadding='0' cellspacing='0' width='50%' class='emailButton' style='background-color: #3498DB;'> <tr> <td align='center' valign='middle' class='buttonContent' style='padding-top:15px;padding-bottom:15px;padding-right:15px;padding-left:15px;'> <a style='color:#FFFFFF;text-decoration:none;font-family:Helvetica,Arial,sans-serif;font-size:20px;line-height:135%;' href='https://cppapk.sharepoint.com' target='_blank'>Go to CPPA-G Web Portal</a> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> </td> </tr> </table> <table bgcolor='#E1E1E1' border='0' cellpadding='0' cellspacing='0' width='80%' id='emailFooter'> <tr> <td valign='top' bgcolor='#E1E1E1'> <div style='font-family:Helvetica,Arial,sans-serif;font-size:13px;color:#828282;text-align:center;line-height:120%;'> <div>Copyright &#169; 2019. All rights reserved.</div> <div>If you don't want to receive these emails from us in the future, please <a href='#' target='_blank' style='text-decoration:none;color:#828282;'><span style='color:#828282;'>unsubscribe</span></a></div> </div> </td> </tr> </table> </td> </tr> </table> </center> </body> </html>";

                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.To.Add(MailTo);
                mail.CC.Add(UserID.Split('½')[4]);
                mail.Bcc.Add("muhammad.usman@cppa.gov.pk");
                mail.From = new MailAddress("erpautoemail@cppa.gov.pk", "CDXP Notification");
                mail.Subject = Subject;
                mail.Body = tmplt;
                mail.IsBodyHtml = true;

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

    }
}