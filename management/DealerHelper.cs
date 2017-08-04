using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Web;

namespace management
{
   public  class DealerHelper
    {
        public  string OrderId { get; set; }
        public  string StartDate { get; set; }
        public  string TillDate { get; set; }

        public static  string Error { get; set; }
        public static bool DownloadInvoice()
        {
            return true;
        }
        public static string GetDealersIdCookies(HttpRequest request)
        {
            return request.Cookies["DealersId"] == null ? "" : request.Cookies["DealersId"].Value;
        }
        public static string GetDealersEmailCookies(HttpRequest request)
        {
            return request.Cookies["Email"] == null ? "" : request.Cookies["Email"].Value;
        }
        public static string GetDealersNameCookies(HttpRequest request)
        {
            return request.Cookies["DealersName"] == null ? "" : request.Cookies["DealersName"].Value;
        }
        public static string GetOrderIdCookies(HttpRequest request)
        {
            return request.Cookies["OrderId"] == null ? "" : request.Cookies["OrderId"].Value;
        }
        public static bool GenerateInvoice()
        {
            return true;
        }

        public static bool SendInvoice(string emailId,string path)
        {
            try
            {

                MailMessage message = new MailMessage();
                message.To.Add(emailId);
                message.From = new MailAddress("vimalkrashna17@gmail.com");
                message.Subject = "Clearification";
                message.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = true;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                credentials.UserName = "vimalkrashna17@gmail.com";
                credentials.Password = "17@Vimal";
                smtp.Credentials = credentials;
                smtp.Port = 587;
                smtp.Send(message);
                return true;
                
            }
            catch (Exception ex)
            {
                Error = "Could not send the e-mail ";
                return false;
            }
        }
        public static string ConvertDateFormat(string date, string format)
        {
            string newDate = null;

            if (format.ToLower() == "yyyymmdd")
            {
                newDate = Convert.ToDateTime(date).ToString("u");
            }

            return newDate;
        }
        public static string CheckingOrderTillDate()
        {
            string abc=null;
            return abc;
        }

        private static bool CheckResult(MethodStore ms, bool result)
        {
            if (!result)
            {
                Error = ms.Error;
                ms.ConnectionClose();
                return false;
            }

            ms.ConnectionClose();
            return true;
        }

        private static bool CheckResult(MethodStore ms, SqlDataReader dr)
        {
            if (dr != null) return true;

            Error = ms.Error;
            ms.ConnectionClose(null);
            return false;
        }
        
    }
}
