using KYC_Portal_Admin.Models;
using KYC_Portal_Admin.Utilities;
using KYC_Portal_Admin.ViewModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KYC_Portal_Admin.Controllers
{
    public class UsersController : Controller
    {
        public List<UserInfo> listUsers = new List<UserInfo>();

        // GET: Companies
        public ActionResult Index()
        {
            SetUserList();
            ViewBag.SuccessMessage=TempData["successMessage"];
            return View(listUsers);
        }
        public ActionResult ExportToExcel()
        {
            var userTable = GetUserDataTable();
            // Get your table data here
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Users");

                // Load the table data into the worksheet
                worksheet.Cells["A1"].LoadFromDataTable(userTable, true);

                // Auto-fit columns for better visibility
                worksheet.Cells.AutoFitColumns();

                // Set some additional formatting if needed
                worksheet.Cells.Style.Font.Size = 12;
                worksheet.Cells.Style.Font.Name = "Arial";

                // Set response headers for Excel file download
                string fileName = "Users.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                byte[] excelBytes = package.GetAsByteArray();

                return File(excelBytes, contentType, fileName);
            }
        }
        public DataTable ConvertReaderToDataTable(SqlDataReader reader)
        {
            DataTable dataTable = new DataTable();

            // Create columns in DataTable based on the reader schema
            for (int i = 0; i < reader.FieldCount; i++)
            {
                dataTable.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
            }

            // Populate DataTable with data from the reader
            while (reader.Read())
            {
                DataRow row = dataTable.NewRow();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[i] = reader[i];
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
        AttachFilesVm attachFilesVm = new AttachFilesVm();
        public ActionResult ViewAttachments(int id)
        {
            SetPANInfo(id);
            return View(attachFilesVm);
        }
        public ActionResult ViewFile(int id)
        {
            var FileName = Read(id);
            var file = FileName.Split('|');
            return openFile(file[0], file[1]);
        }
        public ActionResult ReadPanFile(int id)
        {
            var FileName = GETPANInfo(id);
            string[] file = new string[2];
            foreach (var item in FileName)
            {
                file = item.Key.Split('|');

            }
            return openFile(file[0], file[1]);
        }
        public ActionResult ReadBankFile(int id)
        {
            var FileName = GETPANInfo(id);
            string[] file = new string[2];
            foreach (var item in FileName)
            {
                file = item.Value.Split('|');

            }
            return openFile(file[0], file[1]);
        }
        public Dictionary<string, string> GETPANInfo(int id)
        {
            Dictionary<string, string> files = new Dictionary<string, string>();
            String connectionString = Constants.CONNECTION_STRING;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String sql = "select top 1 [pan_file],[bank_file] from pan_detail where id=@ID;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0) && !reader.IsDBNull(1))
                                files.Add(reader.GetString(0), reader.GetString(1));
                            else if (!reader.IsDBNull(0))
                                files.Add(reader.GetString(0), string.Empty);
                            else
                                files.Add(string.Empty, reader.GetString(1));

                        }
                    }
                }
            }
            return files;
        }
        public string Read(int id)
        {
            string fileName = string.Empty;
            try
            {
                using (SqlConnection connection = new SqlConnection(Constants.CONNECTION_STRING))
                {
                    connection.Open();
                    String sql = " SELECT filename FROM reimbursement where id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))

                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                fileName = reader.GetString(0);

                        }


                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());

            }
            return fileName;

        }

        public ActionResult openFile(string base64String, string fileName)
        {
            byte[] fileBytes = Convert.FromBase64String(base64String);

            string uploadsPath = Server.MapPath("~/Uploads");
            string filePath = Path.Combine(uploadsPath, fileName);

            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            System.IO.File.WriteAllBytes(filePath, fileBytes);


            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound();
            }

            string contentType = MimeMapping.GetMimeMapping(fileName);
            if (string.IsNullOrEmpty(contentType))
            {
                contentType = "application/octet-stream";
            }
            return new FileContentResult(fileBytes, contentType);
        }

        public void SetPANInfo(int id)
        {
            String connectionString = Constants.CONNECTION_STRING;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String sql = "select pan_file,bank_file,id from pan_detail where user_id=@userID;";
                sql += "select filename from reimbursement where user_id=@userID;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@userID", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                                attachFilesVm.PANFilePath.Add(reader.GetInt32(2), reader.GetString(0));
                            if (!reader.IsDBNull(1))
                                attachFilesVm.BankFilePath.Add(reader.GetInt32(2), reader.GetString(1));
                        }
                    }
                }
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String sql = "select id, filename,for_month_text,for_year_num from reimbursement where user_id=@userID;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@userID", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                if (!reader.IsDBNull(1))
                                    attachFilesVm.ReimbursementFilesPath.Add(reader.GetInt32(0), reader.GetString(1) + " (" + reader.GetString(2) + "-" + reader.GetInt32(3) + " )");
                            }
                            catch (Exception) { }
                        }
                    }
                }
            }
        }
        public String errorMessage = "";
        public String successMessage = "";
        public UserInfoVM userInfoVM = new UserInfoVM();
        public ActionResult Create()
        {
            SetCompanyList();

            return View(userInfoVM);
        }
        [HttpPost]
        public ActionResult Create(UserInfo userInfo)
        {
            UserCreate(userInfo);
            SetCompanyList();

            userInfoVM.docInfo = userInfo;
            userInfoVM.successMessage = successMessage;
            userInfoVM.errorMessage = errorMessage;
            return View(userInfoVM);
        }
        [HttpPost]
        public ActionResult UploadExcelFile(FormCollection formCollection)
        {
            SetCompanyList();
            // Get the uploaded file
            var file = Request.Files["excelFile"];

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                        // Assuming the data starts from the 2nd row (skip the header row)
                        int startRow = 2;
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = startRow; row <= rowCount; row++)
                        {
                            // Assuming the columns are in order (FirstName, LastName, Phone, Company, Address, Email, Password)
                            string firstName = worksheet.Cells[row, 1].Value?.ToString();
                            string lastName = worksheet.Cells[row, 2].Value?.ToString();
                            string phone = worksheet.Cells[row, 3].Value?.ToString();
                            var companyName= worksheet.Cells[row, 4].Value?.ToString();
                            int companyId = userInfoVM
                                            .Companies
                                            .FirstOrDefault(x=>x.companyname.Equals(companyName,StringComparison.OrdinalIgnoreCase))?
                                            .id??0;
                            //int companyId = Convert.ToInt32(worksheet.Cells[row, 4].Value);
                            string address = worksheet.Cells[row, 5].Value?.ToString();
                            string email = worksheet.Cells[row, 6].Value?.ToString();
                            string password = worksheet.Cells[row, 7].Value?.ToString();

                            SaveUser(new UserInfo
                            {
                                firstname = firstName,
                                lastname = lastName,
                                contactno = phone,
                                companyid = companyId,
                                address = address,
                                emailid = email,
                                userpwd = password
                            });

                            TempData["successMessage"] = "Users Imported Successfully!";

                        }

                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    ViewBag.ErrorMessage = "Error occurred while processing the file: " + ex.Message;
                }
            }

            // Redirect back to the view (you may also use TempData to preserve the message across redirects)
            return RedirectToAction("Index", "Users");
        }
        public UserInfo editUserInfo = new UserInfo();
        public ActionResult Edit(int id)
        {
            editUserInfo.id = id;
            SetUser();
            SetCompanyList();

            userInfoVM.docInfo = editUserInfo;
            return View(userInfoVM);
        }
        [HttpPost]
        public ActionResult Edit(UserInfo userInfo)
        {
            userInfo.id = editUserInfo.id;
            EditUser(userInfo);
            SetCompanyList();

            userInfoVM.docInfo = userInfo;
            userInfoVM.successMessage = successMessage;
            userInfoVM.errorMessage = errorMessage;
            return View(userInfoVM);
        }
        public ActionResult Delete(int id)
        {
            editUserInfo.id = id;
            DeleteUser();
            return RedirectToAction(nameof(Index));
        }
        public void DeleteUser()
        {
            int id = editUserInfo.id;

            try
            {
                String connectionString = Constants.CONNECTION_STRING;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "Delete from users where id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }
        public void EditUser(UserInfo DocInfo)
        {
            DocInfo.id = Convert.ToInt32(Request.Form["id"]);
            DocInfo.firstname = Request.Form["firstname"];
            DocInfo.lastname = Request.Form["lastname"];
            DocInfo.emailid = Request.Form["email"];
            DocInfo.contactno = Request.Form["phone"];
            DocInfo.address = Request.Form["address"];
            int.TryParse(Request.Form["companyid"], out int companyID);
            int.TryParse(Request.Form["qualid"], out int qualid);
            DocInfo.companyid = companyID;
            DocInfo.qualid = qualid;
            DocInfo.username = Request.Form["firstname"];
            DocInfo.userpwd = Request.Form["userpwd"];

            DocInfo.updatedat = DateTime.Now.ToString();

            if (DocInfo.firstname.Length == 0 ||
               DocInfo.emailid.Length == 0 || DocInfo.contactno.Length == 0)
            {
                errorMessage = " All fields are required";
                return;
            }

            try
            {
                String connectionString = Constants.CONNECTION_STRING;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "UPDATE users " +
                      "SET firstname=@firstname, lastname=@lastname, emailid=@emailid, contactno=@contactno, username=@username,companyid=@companyID, qualid=@qualid,userpwd=@userpwd, address=@address,updatedat=@updatedat" +
                    " WHERE id=@id";


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@firstname", DocInfo.firstname); //    .fullname);
                        command.Parameters.AddWithValue("@lastname", DocInfo.lastname);
                        command.Parameters.AddWithValue("@emailid", DocInfo.emailid);
                        command.Parameters.AddWithValue("@contactno", DocInfo.contactno);
                        command.Parameters.AddWithValue("@userpwd", DocInfo.userpwd);
                        command.Parameters.AddWithValue("@username", DocInfo.username);
                        command.Parameters.AddWithValue("@address", DocInfo.address);
                        command.Parameters.AddWithValue("@companyID", DocInfo.companyid);
                        command.Parameters.AddWithValue("@qualid", DocInfo.qualid);
                        command.Parameters.AddWithValue("@updatedat", DateTime.Now.ToString());
                        command.Parameters.AddWithValue("@id", DocInfo.id);


                        command.ExecuteNonQuery();

                    }


                }
            }



            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Users/Index");

        }
        public void SetCompanyList()
        {
            try
            {
                String connectionString = Constants.CONNECTION_STRING;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = " SELECT * FROM company";
                    using (SqlCommand command = new SqlCommand(sql, connection))

                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CompanyInfo CompanyInfo = new CompanyInfo();
                                CompanyInfo.id = reader.GetInt32(0);
                                CompanyInfo.companyname = reader.GetString(1);
                                CompanyInfo.compaddress = reader.GetString(2);
                                CompanyInfo.logofile = reader.GetString(3);
                                CompanyInfo.signaturefile = reader.GetString(4);
                                try
                                {
                                    CompanyInfo.isactive = reader.GetString(5);
                                }
                                catch (Exception ex)
                                { }

                                try
                                {
                                    CompanyInfo.createdat = reader.GetString(6);
                                }
                                catch (Exception ex)
                                { }

                                try
                                {
                                    CompanyInfo.updatedby = reader.GetInt32(9);
                                }
                                catch (Exception ex)
                                { }
                                try
                                {
                                    CompanyInfo.createdby = reader.GetInt32(7);
                                }
                                catch (Exception ex)
                                { }

                                try
                                {
                                    CompanyInfo.updatedat = reader.GetString(8);
                                }
                                catch (Exception ex)
                                { }





                                userInfoVM.Companies.Add(CompanyInfo);
                            }

                        }



                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());

            }

        }

        public void SetUser()
        {
            int id = editUserInfo.id;

            try
            {
                String connectionString = Constants.CONNECTION_STRING;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select * from users where id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                editUserInfo.id = reader.GetInt32(0);
                                editUserInfo.emailid = reader.GetString(2);
                                editUserInfo.userpwd = reader.GetString(3);
                                editUserInfo.firstname = reader.GetString(4);
                                editUserInfo.lastname = reader.GetString(5);
                                editUserInfo.contactno = reader.GetString(6);
                                editUserInfo.address = reader.GetString(7);
                                editUserInfo.qualid = reader.GetInt32(15);
                                editUserInfo.companyid = reader.GetInt32(16);
                            }
                        }

                    }


                }



            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;

            }
        }

        public void UserCreate(UserInfo userinfo)
        {

            userinfo.firstname = Request.Form["firstname"];
            userinfo.lastname = Request.Form["lastname"];
            userinfo.emailid = Request.Form["email"];
            userinfo.contactno = Request.Form["phone"];
            userinfo.address = Request.Form["address"];
            userinfo.userpwd = Request.Form["userpwd"];
            userinfo.username = Request.Form["firstname"];
            int.TryParse(Request.Form["companyid"], out int companyID);
            userinfo.companyid = companyID;
            int.TryParse(Request.Form["qualid"], out int qualid);
            userinfo.qualid = qualid;


            if (userinfo.firstname.Length == 0 || userinfo.emailid.Length == 0 ||
                userinfo.contactno.Length == 0 || userinfo.companyid == 0)
            {
                errorMessage = "All the fields are required";
                return;
            }

            SaveUser(userinfo);
                
            userinfo.firstname = ""; userinfo.emailid = ""; userinfo.contactno = ""; userinfo.address = "";
            successMessage = "New User Added Correctly";

            Response.Redirect("/Users/Index");
        }
        private void SaveUser(UserInfo userInfo)
        {
            //save the new user into the database

            userInfo.qualid = 0;
            userInfo.isactive = 1;
            userInfo.userrole = 1;
            userInfo.firstlogin = 0;
            userInfo.regdate = DateTime.Now.ToString();
            userInfo.createdat = DateTime.Now.ToString();
            try
            {
                String connectionString = Constants.CONNECTION_STRING;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "Insert into Users" +
                          "(username, emailid, userpwd, firstname, lastname, contactno, address, userrole,createdat, regdate, firstlogin, isactive, qualid, companyid ) values " +
                          "(@username,@emailid,@userpwd, @firstname, @lastname, @contactno, @address, @userrole,@createdat,   @regdate,@firstlogin, @isactive, @qualid,@companyid);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", userInfo.firstname);
                        command.Parameters.AddWithValue("@emailid", userInfo.emailid);
                        command.Parameters.AddWithValue("@userpwd", userInfo.userpwd);
                        command.Parameters.AddWithValue("@firstname", userInfo.firstname);
                        command.Parameters.AddWithValue("@lastname", userInfo.lastname);
                        command.Parameters.AddWithValue("@contactno", userInfo.contactno);
                        command.Parameters.AddWithValue("@address", userInfo.address);
                        command.Parameters.AddWithValue("@userrole", userInfo.userrole);
                        command.Parameters.AddWithValue("@isactive", userInfo.isactive);
                        command.Parameters.AddWithValue("@qualid", userInfo.qualid);
                        command.Parameters.AddWithValue("@regdate", userInfo.regdate);
                        command.Parameters.AddWithValue("@createdat", userInfo.createdat);
                        command.Parameters.AddWithValue("@companyid", userInfo.companyid);
                        command.Parameters.AddWithValue("@firstlogin", userInfo.firstlogin);

                        command.ExecuteNonQuery();

                    }


                }


            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
        }
        public void SetUserList()
        {
            try
            {
                String connectionString = Constants.CONNECTION_STRING;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = " SELECT id,firstname,lastname,emailid,contactno,address,CONVERT(nvarchar(50),CONVERT(date, regdate)) as regdat FROM Users";
                    using (SqlCommand command = new SqlCommand(sql, connection))

                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserInfo docInfo = new UserInfo();
                                docInfo.id = Convert.ToInt32(reader.GetInt32(0));
                                docInfo.firstname = reader.GetString(1);
                                docInfo.lastname = reader.GetString(2);
                                docInfo.emailid = reader.GetString(3);
                                docInfo.contactno = reader.GetString(4);
                                docInfo.address = reader.GetString(5);

                                try
                                {

                                    docInfo.createdat = reader.GetString(6);
                                }
                                catch (Exception ex)
                                { }

                                listUsers.Add(docInfo);
                            }

                        }



                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());

            }


        }
        public DataTable GetUserDataTable()
        {
            DataTable userTable = new DataTable();
            try
            {
                String connectionString = Constants.CONNECTION_STRING;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT TOP (1000) ROW_NUMBER() OVER (ORDER BY u.id) AS 'Sr. #'\r\n      ,[username] as 'User Name'\r\n      ,[emailid] as Email\r\n      ,[firstname] as 'First Name'\r\n      ,[lastname] as 'Last Name'\r\n      ,[contactno] as 'Contact Number'\r\n      ,[address] as 'Address'\r\n      ,CASE \r\n        WHEN ur.[roletitle] < 1 THEN ''\r\n        ELSE ur.[roletitle]\r\n    END  as 'Role'\r\n      ,CONVERT(nvarchar(50),CONVERT(date, u.regdate)) AS 'Registerd Date'\r\n      ,CASE \r\n        WHEN u.isactive = 1 THEN 'Active'\r\n        ELSE 'De-Active'\r\n    END AS [Status]\r\n      ,cc.country_name as 'Country'\r\n\t  ,ci.cityname as 'City'\r\n      ,q.title as 'Qualification'\r\n      ,c.companyname as Company\r\n  FROM [KYCportal].[dbo].[users] u\r\n  left join company c on c.id=u.companyid\r\n  left join qualification q on q.id=u.qualid\r\n  left join userrole ur on ur.id=u.userrole\r\n  left join country cc on cc.id=u.userrole\r\n  left join city ci on ci.id=u.userrole";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            userTable = ConvertReaderToDataTable(reader);//for export excel                           
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());

            }
            return userTable;

        }

    }
}