using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KYC_Portal_Admin.Models
{
    public class CompanyInfo
    {
        public int id{get;set;}
        public string companyname{get;set;}
        public string compaddress{get;set;}
        public string logofile{get;set;}
        public string signaturefile{get;set;}
        public string isactive { get; set; } = "1";
        public string regdate{get;set;}
        public string createdat { get; set; }

        public int createdby { get; set; }
        public string updatedat { get; set; }
        public int updatedby { get; set; }
    }
}