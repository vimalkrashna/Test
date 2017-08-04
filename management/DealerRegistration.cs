using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace management
{
  
    public class DealerRegistration
    {
        public string Email { get; set; }
        public string Error { get; set; }
        public string DealersId { get; set; }
        public string Name { get; set; }
        public string NameOfCompany { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
        public string Phone { get; set; }
        public string OldPass { get; set; }
        public string  PassWord { get; set; }


        public bool Save(string emailid,string password)
        {
            SqlDataReader dr = null;
            var ms = new MethodStore();
            dr= ms.sp_ExcecuteReader("RegisterDealer", "@Email", emailid, "@Password", password, "@Name",Name,"@NameOfCompany",NameOfCompany,"@Address",Address ,"@City",City, "@State",State, "@Pincode",Pincode,"@PhoneNo",Phone);

            if (!CheckResult(ms, dr))
                return false;

           
            if (dr.Read())
            {
               DealersId = dr["DealerId"].ToString();
               ms.ConnectionClose(dr);
               if(DealersId=="0")
                { Error = "Account already exists with " +emailid  ;return false; }
                
            }

            ms.ConnectionClose(dr);
            return true;

        }

        public string Login(string emailid,string password)
        {
            SqlDataReader dr= null;
            var ms = new MethodStore();
         
            dr = ms.sp_ExcecuteReader("SelectDealer", "@Email", emailid, "@Password", password);

            if (!CheckResult(ms, dr))
                return Error = ms.Error;

            if (dr.Read())
            {
                DealersId = dr["DealersId"].ToString();
                   ms.ConnectionClose(dr);
                   return DealersId;
            }
            ms.ConnectionClose(dr);
            return Error = "Your Email or Password is not correct";
        }
        public bool EmailVerification(string Id)
        {
       
            var ms = new MethodStore();
            var result = ms.sp_ExecuteQuery("VerifyEmail", "@DealerId",Id);
            return (CheckResult(ms,result));
        }
        public string EncryptDealerId(string DealerId)
        {
            var plaintextBytes = Encoding.UTF8.GetBytes(DealerId);
            var v2 = MachineKey.Encode(plaintextBytes, MachineKeyProtection.All);
            return v2;
        }
    
        public string DecryptDealerId(string DealerId)
        {
            var decryptedBytes = MachineKey.Decode(DealerId, MachineKeyProtection.All);
            var DecryptedId=Encoding.UTF8.GetString(decryptedBytes);
            return DecryptedId;
        }

        public bool Logout(HttpRequest request)
        {
            request.Cookies.Clear();
            return true;
        }

        public bool UpdateDealerDetails()
        {
            var ms = new MethodStore();
            var result = ms.sp_ExecuteQuery("UpdateDealerDetails", "@DealerId", DealersId,
                                         "@Name", Name, "@NameOfCompany", NameOfCompany, "@Address", Address, "@City", City, "@State", State, "@Pincode", Pincode, "@PhoneNo", Phone);
            return (CheckResult(ms, result));
        }

       
        public bool Select()
        {
           
            SqlDataReader dr = null;
            var ms = new MethodStore();

            dr = ms.sp_ExcecuteReader("SelectDealerDetails", "@DealerId", DealersId);

            if (!CheckResult(ms, dr))
                return false;

            if (dr.Read())
            {
                Email = dr["DealersEmail"].ToString();
                OldPass = dr["DealersPassword"].ToString();
                Name = dr["Name"].ToString();
                NameOfCompany = dr["NameOfCompany"].ToString();
                Address = dr["Address"].ToString();
                City = dr["City"].ToString();
                State = dr["State"].ToString();
                Pincode = dr["Pincode"].ToString();
                Phone = dr["PhoneNo"].ToString();

            }



            ms.ConnectionClose(dr);
            return true;
        }

      
        public string ForgotPassword(string emailid)
        {
            var ms = new MethodStore();
            SqlDataReader dr = null;
           
            dr = ms.sp_ExcecuteReader("ForgotPassword", "@EmailId", emailid);

             if (!CheckResult(ms, dr))
             return Error=ms.Error;

            if (dr.Read())
            {
                PassWord = dr["DealersPassword"].ToString();
                Name = dr["Name"].ToString();
                return "true";

            }


             ms.ConnectionClose(dr);
            return Error="Email-id is not registered with us Register now." +emailid;
           
        }
        public bool ChangePassword(string Oldpass, string NewPass)
        {
            var ms = new MethodStore();

            var result = ms.sp_ExecuteQuery("ChangePassword", "@DealerId", DealersId, "@OldPassword", Oldpass, "@NewPassword ", NewPass);
            return (CheckResult(ms, result));


        }
        private bool CheckResult(MethodStore ms, SqlDataReader dr)
        {
            if (dr != null) return true;

            Error = ms.Error;
            ms.ConnectionClose(null);
            return false;
        }
        private bool CheckResult(MethodStore ms, bool result)
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

      
    }
}
