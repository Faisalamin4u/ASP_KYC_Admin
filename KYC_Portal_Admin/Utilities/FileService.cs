using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KYC_Portal_Admin.Utilities
{
    public static class FileService
    {
        public static string UploadImage_Base64(HttpPostedFileBase postedFileBase)
        {
            if (postedFileBase != null && postedFileBase.ContentLength > 0)
            {
                // Convert the image file to Base64
                string base64String;
                using (var memoryStream = new MemoryStream())
                {
                    postedFileBase.InputStream.CopyTo(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();
                    base64String = Convert.ToBase64String(imageBytes);
                }
                return base64String; ;
            }

            // Handle the case when no file was selected
            return string.Empty;
        }
    }
}