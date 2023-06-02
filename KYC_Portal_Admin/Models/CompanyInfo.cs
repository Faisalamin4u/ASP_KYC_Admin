using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KYC_Portal_Admin.Models
{
    public class CompanyInfo
    {
        public string id;
        public string companyname;
        public string compaddress;
        public string logofile;
        public string signaturefile;
        public string isactive="1";
        public string regdate;
        public string createdat { get; set; }

        public int createdby { get; set; }
        public string updatedat { get; set; }
        public int updatedby { get; set; }
    }
}