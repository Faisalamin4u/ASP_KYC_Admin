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
    public class UsersController : Controller
    {
        public List<UserInfo> listUsers = new List<UserInfo>();

        // GET: Companies
        public ActionResult Index()
        {
            SetUserList();
            return View(listUsers);
        }
        public String errorMessage = "";
        public String successMessage = "";
        public UserInfoVM userInfoVM = new UserInfoVM();
        public ActionResult Create()
        {
            return View(userInfoVM);
        }
        [HttpPost]
        public ActionResult Create(UserInfo userInfo)
        {
            UserCreate(userInfo);
            userInfoVM.docInfo = userInfo;
            userInfoVM.successMessage = successMessage;
            userInfoVM.errorMessage = errorMessage;
            return View(userInfoVM);
        }
        public UserInfo editUserInfo = new UserInfo();
        public ActionResult Edit(int id)
        {
            editUserInfo.id = id;
            SetUser();
            userInfoVM.docInfo = editUserInfo;
            return View(userInfoVM);
        }
        [HttpPost]
        public ActionResult Edit(UserInfo userInfo)
        {
            userInfo.id = editUserInfo.id;
            EditUser(userInfo);
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
            DocInfo.emailid = Request.Form["emailid"];
            DocInfo.contactno = Request.Form["contactno"];
            DocInfo.address = Request.Form["address"];

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
                      "SET firstname=@firstname, lastname=@lastname, emailid=@emailid, contactno=@contactno, username=@username, userpwd=@userpwd, address=@address" +
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

        public void UserCreate(UserInfo docInfo)
        {

            docInfo.firstname = Request.Form["firstname"];
            docInfo.lastname = Request.Form["lastname"];
            docInfo.emailid = Request.Form["email"];
            docInfo.contactno = Request.Form["phone"];
            docInfo.address = Request.Form["address"];
            docInfo.userpwd = Request.Form["userpwd"];
            docInfo.username = Request.Form["firstname"];


            if (docInfo.firstname.Length == 0 || docInfo.emailid.Length == 0 ||
                docInfo.contactno.Length == 0)
            {
                errorMessage = "All the fields are required";
                return;
            }

            //save the new user into the database

            docInfo.qualid = 0;
            docInfo.isactive = 1;
            docInfo.userrole = 1;
            docInfo.companyid = 1;
            docInfo.firstlogin = 0;
            docInfo.regdate = DateTime.Now.ToString();
            docInfo.createdat = DateTime.Now.ToString();

            try
            {
                String connectionString = Constants.CONNECTION_STRING;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "Insert into Users" +
                          "(username, emailid, userpwd, firstname, lastname, contactno, address, userrole, regdate, firstlogin, isactive, qualid, companyid ) values " +
                          "(@username,@emailid,@userpwd, @firstname, @lastname, @contactno, @address, @userrole,   @regdate,@firstlogin, @isactive, @qualid,@companyid);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", docInfo.firstname);
                        command.Parameters.AddWithValue("@emailid", docInfo.emailid);
                        command.Parameters.AddWithValue("@userpwd", docInfo.userpwd);
                        command.Parameters.AddWithValue("@firstname", docInfo.firstname);
                        command.Parameters.AddWithValue("@lastname", docInfo.lastname);
                        command.Parameters.AddWithValue("@contactno", docInfo.contactno);
                        command.Parameters.AddWithValue("@address", docInfo.address);
                        command.Parameters.AddWithValue("@userrole", docInfo.userrole);
                        command.Parameters.AddWithValue("@isactive", docInfo.isactive);
                        command.Parameters.AddWithValue("@qualid", docInfo.qualid);

                        command.Parameters.AddWithValue("@regdate", docInfo.regdate);
                        command.Parameters.AddWithValue("@companyid", docInfo.companyid);
                        command.Parameters.AddWithValue("@firstlogin", docInfo.firstlogin);

                        command.ExecuteNonQuery();

                    }


                }


            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            docInfo.firstname = ""; docInfo.emailid = ""; docInfo.contactno = ""; docInfo.address = "";
            successMessage = "New User Added Correctly";

            Response.Redirect("/Users/Index");
        }
        public void SetUserList()
        {
            try
            {
                String connectionString = Constants.CONNECTION_STRING;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = " SELECT * FROM Users";
                    using (SqlCommand command = new SqlCommand(sql, connection))

                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserInfo docInfo = new UserInfo();
                                docInfo.id = Convert.ToInt32(reader.GetInt32(0));
                                docInfo.firstname = reader.GetString(4);
                                docInfo.lastname = reader.GetString(5);
                                docInfo.emailid = reader.GetString(2);
                                docInfo.contactno = reader.GetString(6);
                                docInfo.address = reader.GetString(7);

                                try
                                {
                                    docInfo.createdat = reader.GetDateTime(12).ToString();
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

    }
}