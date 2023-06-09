using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KYC_Portal_Admin.ViewModels
{
    public class AttachFilesVm
    {
        public Dictionary<int, string> PANFilePath { get; set; } = new Dictionary<int, string>();
        public Dictionary<int,string> BankFilePath { get; set; } = new Dictionary<int, string>();
        public Dictionary<int,string> AgreementsFilePath { get; set; } = new Dictionary<int, string>();
        public Dictionary<int,string> ReimbursementFilesPath { get; set; } = new Dictionary<int, string>();
    }
}