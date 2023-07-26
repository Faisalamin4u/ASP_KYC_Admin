using KYC_Portal_Admin.Models;
using KYC_Portal_Admin.Utilities;
using KYC_Portal_Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KYC_Portal_Admin.Controllers
{
    public class CompaniesController : Controller
    {
        public List<CompanyInfo> listCompany = new List<CompanyInfo>();

        // GET: Companies
        public ActionResult Index()
        {
            SetCompanyList();
            foreach (var company in listCompany)
            {
                var fileName = company.logofile.Split('|');
                if (fileName != null && fileName.Count() > 1)
                    company.logofile = fileName[0];

                var fileName1 = company.signaturefile.Split('|');
                if (fileName1 != null && fileName1.Count() > 1)
                    company.signaturefile = fileName1[0];
            }
            return View(listCompany);
        }
        public String errorMessage = "";
        public String successMessage = "";
        public CompanyInfoVM companyInfoVM = new CompanyInfoVM();
        public ActionResult Create()
        {
            return View(companyInfoVM);
        }
        [HttpPost]
        public ActionResult Create(CompanyInfo companyInfo)
        {
            CompanyCreate(companyInfo);
            companyInfoVM.companyInfo = companyInfo;
            companyInfoVM.successMessage = successMessage;
            companyInfoVM.errorMessage = errorMessage;
            return View(companyInfoVM);
        }
        public CompanyInfo editCompanyInfo = new CompanyInfo();
        public ActionResult Edit(int id)
        {
            SetCompany(id);
            companyInfoVM.companyInfo = editCompanyInfo;
            return View(companyInfoVM);
        }
        [HttpPost]
        public ActionResult Edit(CompanyInfo companyInfo)
        {
            companyInfo.id = editCompanyInfo.id;
            EditCompany(companyInfo);
            companyInfoVM.companyInfo = companyInfo;
            companyInfoVM.successMessage = successMessage;
            companyInfoVM.errorMessage = errorMessage;
            return View(companyInfoVM);
        }
        public ActionResult Delete(int id)
        {
            DeleteCompany(id);
            return RedirectToAction(nameof(Index));
        }
        public void DeleteCompany(int id)
        {


            try
            {
                String connectionString = Constants.CONNECTION_STRING;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "Delete from company where id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }
        public void EditCompany(CompanyInfo companyInfo)
        {
            var files = Request.Files;
            if (files == null || (files != null && files.Count < 2))
            {
                errorMessage = "Please Select Logo or signature file."; return;
            }
            var logoFile = files[0];
            var stampFile = files[1];
            int.TryParse(Request.Form["id"], out int id);
            companyInfo.id = id;
            companyInfo.companyname = Request.Form["companyname"];
            companyInfo.compaddress = Request.Form["compaddress"];
            companyInfo.logofile = logoFile.FileName + "|" + FileService.UploadImage_Base64(logoFile);
            companyInfo.signaturefile = stampFile.FileName + "|" + FileService.UploadImage_Base64(stampFile); // Request.Form["signaturefile"];         
            companyInfo.isactive = Request.Form["isactive"];
            companyInfo.createdat = Request.Form["createdat"];
            companyInfo.createdby = 1; // Convert.ToInt32(Request.Form["createdby"]); // corrected syntax
            companyInfo.updatedat = Request.Form["updatedat"];
            companyInfo.updatedby = 1; // Convert.ToInt32(Request.Form["updatedby"]);


            if (companyInfo.id == 0 || companyInfo.companyname.Length == 0 ||
               companyInfo.compaddress.Length == 0 || companyInfo.logofile.Length == 0)

            //companyInfo.signature_file.Length ==0 || companyInfo.signature_file ==0 || companyInfo.is_active ==0
            //|| companyInfo.created_at ==0 || companyInfo.created_by ==0 || companyInfo.updated_at ==0 || companyInfo.updated_by==0)
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
                    String sql = "UPDATE Company " +
                      "SET companyname=@company_name, signaturefile=@signaturefile, logofile=@logofile,isactive=@isactive , compaddress=@address" +

                    " WHERE id=@id";


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", companyInfo.id);
                        command.Parameters.AddWithValue("@company_name", companyInfo.companyname);
                        command.Parameters.AddWithValue("@address", companyInfo.compaddress);
                        command.Parameters.AddWithValue("@logofile", companyInfo.logofile);
                        command.Parameters.AddWithValue("@signaturefile", companyInfo.signaturefile);
                        command.Parameters.AddWithValue("@isactive", 1);
                        command.ExecuteNonQuery();

                    }


                }
            }



            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Companies/Index");

        }

        public void SetCompany(int id)
        {
            try
            {
                String connectionString = Constants.CONNECTION_STRING;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select * from company where id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                editCompanyInfo.id = reader.GetInt32(0);
                                editCompanyInfo.companyname = reader.GetString(1);
                                editCompanyInfo.compaddress = reader.GetString(2);
                                editCompanyInfo.logofile = reader.GetString(3); // corrected syntax
                                editCompanyInfo.signaturefile = reader.GetString(4); // corrected syntax

                                try
                                {
                                    editCompanyInfo.isactive = reader.GetString(5);
                                }
                                catch (Exception ex)
                                { }

                                try
                                {
                                    editCompanyInfo.createdat = reader.GetString(6);
                                }
                                catch (Exception ex)
                                { }

                                try
                                {
                                    editCompanyInfo.updatedby = reader.GetInt32(9);
                                }
                                catch (Exception ex)
                                { }
                                try
                                {
                                    editCompanyInfo.createdby = reader.GetInt32(7);
                                }
                                catch (Exception ex)
                                { }

                                try
                                {
                                    editCompanyInfo.updatedat = reader.GetString(8);
                                }
                                catch (Exception ex)
                                { }



                                //  companyInfo.isactive = reader.GetString(4);
                                ///  companyInfo.createdat = reader.GetString(5);
                                //  companyInfo.createdby = reader.GetInt32(6);
                                //  companyInfo.updatedat = reader.GetString(7);
                                //   companyInfo.updatedby = reader.GetInt32(8);
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

        public void CompanyCreate(CompanyInfo companyInfo)
        {
            var files = Request.Files;
            if (files == null || (files != null && files.Count < 2))
            {
                errorMessage = "Please Select Logo or signature file."; return;
            }
            var logoFile = files[0];
            var stampFile = files[1];
            int.TryParse(Request.Form["id"], out int id);
            companyInfo.id = id;
            companyInfo.companyname = Request.Form["companyname"];
            companyInfo.compaddress = Request.Form["compaddress"];
            companyInfo.logofile = logoFile.FileName + "|" + FileService.UploadImage_Base64(logoFile);
            companyInfo.signaturefile = stampFile.FileName + "|" + FileService.UploadImage_Base64(stampFile); // Request.Form["signaturefile"];
            companyInfo.isactive = "1"; // Request.Form["is_active"];
            companyInfo.createdat = DateTime.Now.ToString(); // Request.Form["created_at"];
                                                             //companyInfo.created_by = Request.Form["created_by"];

            companyInfo.updatedat = DateTime.Now.ToString(); //Request.Form["updated_at"];
            //companyInfo.updated_by = Request.Form["updated_by"];

            if (companyInfo.companyname.Length == 0 || companyInfo.compaddress.Length == 0
    )

            {
                errorMessage = "All the fields are required";
                return;
            }

            //save the new user into the database

            try
            {
                String connectionString = Constants.CONNECTION_STRING;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "Insert into  company" +
                          "(companyname, compaddress, logofile, signaturefile,isactive,createdat,  createdby,updatedat,updatedby,regdate ) values " +
                          "(@companyname,@compaddress,@logofile,@signaturefile,@isactive,@createdat,@createdby,@updatedat,@updatedby,@regdate);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@companyname", companyInfo.companyname);
                        command.Parameters.AddWithValue("@compaddress", companyInfo.compaddress);
                        command.Parameters.AddWithValue("@logofile", companyInfo.logofile);
                        command.Parameters.AddWithValue("@signaturefile", companyInfo.signaturefile);
                        command.Parameters.AddWithValue("@isactive", 1);
                        command.Parameters.AddWithValue("@createdat", companyInfo.createdat);
                        command.Parameters.AddWithValue("@createdby", companyInfo.createdby);
                        command.Parameters.AddWithValue("@updatedat", companyInfo.updatedat);
                        command.Parameters.AddWithValue("@updatedby", companyInfo.updatedby);
                        command.Parameters.AddWithValue("@regdate", companyInfo.createdat);


                        command.ExecuteNonQuery();

                    }


                }


            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            companyInfo.companyname = ""; companyInfo.compaddress = ""; companyInfo.logofile = ""; companyInfo.signaturefile = "";
            companyInfo.isactive = ""; companyInfo.createdat = ""; companyInfo.updatedat = "";
            /*ompanyInfo.updated_by = "";*/
            successMessage = "New Company Added Correctly";

            Response.Redirect("/Companies/Index");
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





                                listCompany.Add(CompanyInfo);
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

    }
}