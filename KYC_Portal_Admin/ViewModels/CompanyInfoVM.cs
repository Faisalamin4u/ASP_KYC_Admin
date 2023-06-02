using KYC_Portal_Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KYC_Portal_Admin.ViewModels
{
    public class CompanyInfoVM
    {

        public CompanyInfo companyInfo = new CompanyInfo();
        public String errorMessage = "";
        public String successMessage = "";
    }
}