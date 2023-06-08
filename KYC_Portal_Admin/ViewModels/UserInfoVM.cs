using KYC_Portal_Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KYC_Portal_Admin.ViewModels
{
    public class UserInfoVM
    {
        public UserInfo docInfo = new UserInfo();
        public List<CompanyInfo> Companies { get; set; }=new List<CompanyInfo>();
        public String errorMessage = "";
        public String successMessage = "";
    }
}