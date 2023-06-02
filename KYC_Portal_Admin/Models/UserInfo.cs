using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KYC_Portal_Admin.Models
{
    public class UserInfo
    {
        public int id;
        public string username;
        public string emailid;
        public string userpwd;
        public string firstname;
        public string lastname;
        public string contactno;
        public string address;
        public int userrole;
        public string regdate;
        public int firstlogin;
        public int isactive;
        public string createdat;
        public string updatedat;
        public string recoverytoken;
        public int qualid;
        public int companyid;
    }
}