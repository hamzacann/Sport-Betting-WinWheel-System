using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using katarfelek.Models;
using System.Data.Sql;
using System.Web.ModelBinding;
using System.Web.UI.WebControls;
using RestSharp;
using System.Web.Helpers;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace katarfelek.Controllers
{
    public class yesilController : Controller
    {
        // GET: yesil
        public SqlConnection cnn;
        public SqlCommand cmd;
        public DataSet ds;
        public DataTable dt;
        public SqlDataAdapter da;
        List<RequestClass> list;
        List<PanelSettings> PSlist;
        public PanelClass publicPC = new PanelClass();
        public PanelSettings PS;

        public bool CheckIfLogged()
        {
            var obj = Session["LoggedUsername"];
            if (obj!=null)
            {
                if(!string.IsNullOrEmpty(Session["LoggedUsername"].ToString()))
                {
                    return true;
                }
                else return false;
            }
            return false;
        }
        public void GetLoggedUserSettingData(int ActiveSettingId)
        {
            using (var context = new katarfelekEntities())
            {
                // Add data to the particular table
                var data = context.PanelSettings.Where(r => r.SettingId == ActiveSettingId).Select(r => r.LogoPath).Take(1).ToList();
                Session["logoPath"] = data[0].ToString();
            }
            #region With Ado.net
            //cnn = new SqlConnection(cnnstr);
            //cmd = new SqlCommand("", cnn);
            //dt = new DataTable();
            //da = new SqlDataAdapter(cmd);
            //cnn.Open();
            //cmd.CommandText = "SELECT * FROM PanelSettings WHERE SettingId = " + ActiveSettingId;
            //cmd.ExecuteNonQuery();
            //da.Fill(dt);
            //cnn.Close();
            //Session["logoPath"] = dt.Rows[0]["LogoPath"].ToString();
            #endregion
        }

        public ActionResult Logs()
        {
            if (CheckIfLogged()==false) { return RedirectToAction("Login", "yesil"); }
            //else if (PC.UserName == null) { return RedirectToAction("Login", "yesil"); }

            using (var context = new katarfelekEntities())
            {
                // Add data to the particular table
                var data = context.requests.OrderBy(x => x.Id).ToList();
                return View(data);
            }
            #region With Ado.net
            //cnn = new SqlConnection(cnnstr);
            //cmd = new SqlCommand("", cnn);
            //dt = new DataTable();
            //da = new SqlDataAdapter(cmd);
            //list = new List<RequestClass>();
            //list.Clear();
            ////PanelClass PC = new PanelClass();
            //cnn.Open();
            //cmd.CommandText = "SELECT * FROM requests ORDER BY Id";
            //cmd.ExecuteScalar();
            //da.Fill(dt);
            //foreach (DataRow item in dt.Rows)
            //{
            //        list.Add(new RequestClass
            //        {
            //            Id = Convert.ToInt32(item["Id"]),
            //            UserName = item["UserName"].ToString(),
            //            Prize = item["Prize"].ToString(),
            //            //RequestTime = DateTime.ParseExact(item["RequestTime"].ToString(),"dd/MM/yyyy HH:mm:ss",null)
            //            RequestTime = Convert.ToDateTime(ds.Tables[0].Rows[0]["RequestTime"])

            //});

            //}
            //cnn.Close();
            //ViewBag.Username = Session["LoggedUsername"].ToString();

            //return View(list);
            #endregion
        }

        public ActionResult Index()
        {
            if (CheckIfLogged()==false) { return RedirectToAction("Login", "yesil"); }

            using (var context = new katarfelekEntities())
            {
                var data = context.requests.OrderBy(x => x.Id).ToList();
                var Data = data.Where(x => x.RequestTime.Date == DateTime.Now.Date).ToList();

                return View(Data);
            }
            #region With Ado.net
            //cnn = new SqlConnection(cnnstr);
            //cmd = new SqlCommand("", cnn);
            //dt = new DataTable();
            //da = new SqlDataAdapter(cmd);
            //list = new List<RequestClass>();
            //list.Clear();
            ////PanelClass PC = new PanelClass();
            //cnn.Open();
            //cmd.CommandText = "SELECT * FROM requests ORDER BY Id";
            //cmd.ExecuteScalar();
            //da.Fill(dt);
            //foreach (DataRow item in dt.Rows)
            //{
            //    if (DateTime.ParseExact(item["RequestTime"].ToString(),"dd/MM/yyyy HH:mm:ss",null).Day == DateTime.Now.Day)
            //    {
            //        list.Add(new RequestClass
            //        {
            //            Id = Convert.ToInt32(item["Id"]),
            //            UserName = item["UserName"].ToString(),
            //            Prize = item["Prize"].ToString(),
            //            RequestTime = DateTime.ParseExact(item["RequestTime"].ToString(),"dd/MM/yyyy HH:mm:ss",null)

            //        });
            //    }

            //}
            //cnn.Close();
            //ViewBag.Username = Session["LoggedUsername"].ToString();

            //return View(list);
            #endregion
        }

        public ActionResult Login()
        {

            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "yesil", null);
        }
        
        [HttpGet]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(username)) { return View(); }

            Session.Add("AuthNumber", string.Empty);
            Session.Add("LoggedUsername", string.Empty);
            Session.Add("logoPath", string.Empty);
            Session.Add("LoggedUser", string.Empty);
            Session.Add("errorMessage", string.Empty);
            Session.Add("Title", string.Empty);

            PanelClass PC = new PanelClass();

            using(var context = new katarfelekEntities())
            {
                var data = context.panelUsers.Where(x=>x.UserName == username && x.Password == password).ToList();
                if(data.Count > 0)
                {
                    PC.UserId = data[0].UserId;
                    PC.UserName = data[0].UserName;
                    PC.Password = data[0].Password;
                    PC.Auth_Num = data[0].Auth_Num;
                    PC.ActiveSettingID = data[0].ActiveSettingID;
                    Session.Add(username, PC.UserName);
                    Session["LoggedUser"] = PC.UserName;
                    Session["LoggedUsername"] = PC.UserName;
                    Session["AuthNumber"] = PC.Auth_Num;
                    publicPC = PC;
                    GetLoggedUserSettingData(PC.ActiveSettingID);
                    return RedirectToAction("Index", "yesil");
                }
                else { ViewBag.ErrorM = "Kullanıcı Adı Yada Parola Hatalıdır!"; return View(); }
            }

            #region With Ado.net
            //cnn = new SqlConnection(cnnstr);
            //cmd = new SqlCommand("", cnn);
            //ds = new DataSet();
            //da = new SqlDataAdapter(cmd);

            //cnn.Open();
            //cmd.CommandText = "SELECT * FROM panelUsers WHERE UserName = '" + username + "' AND Password = '" + password + "'";
            //cmd.ExecuteNonQuery();
            //da.Fill(ds);
            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    PC.UserId = Convert.ToInt32(ds.Tables[0].Rows[0]["UserId"]);
            //    PC.UserName = ds.Tables[0].Rows[0]["UserName"].ToString();
            //    PC.Password = ds.Tables[0].Rows[0]["Password"].ToString();
            //    PC.Auth_Num = Convert.ToInt32(ds.Tables[0].Rows[0]["Auth_Num"]);
            //    PC.ActiveSettingID = Convert.ToInt32(ds.Tables[0].Rows[0]["ActiveSettingID"]);
            //}
            //else { ViewBag.ErrorM = "Kullanıcı Adı Yada Parola Hatalıdır!"; return View(); }
            //if(PC.UserName == username && PC.Password == password)
            //{
            //    Session.Add(username, PC.UserName);
            //    Session["LoggedUser"] = PC.UserName;
            //    Session["LoggedUsername"] = PC.UserName;
            //    Session["AuthNumber"] = PC.Auth_Num;
            //    publicPC = PC;
            //    GetLoggedUserSettingData(PC.ActiveSettingID);
            //    return RedirectToAction("Index", "yesil");
            //}
            //cnn.Close();
            //return View()
            #endregion
        }
        public ActionResult SettingList()
        {
            if (CheckIfLogged() == false) { return RedirectToAction("Login", "yesil"); }
            if (Request.UrlReferrer != null)
            {
                if (Request.UrlReferrer.Host != Request.Url.Host)
                {
                    return new HttpUnauthorizedResult("Access Denied!");
                }
                List<PanelSettings> PSettingsList = CheckSettings();
                return PartialView(PSettingsList);
            }
            else { return new HttpUnauthorizedResult("Access Denied!"); }
            
        }
        public ActionResult ChangeSetting(PanelSettings item)
        {
            if (CheckIfLogged() == false) { return RedirectToAction("Login", "yesil"); }
            if(System.IO.File.Exists(Server.MapPath(item.LogoPath)))
            {
                try
                {
                    Session["logoPath"] =  item.LogoPath;
                    string usernamee = Session["LoggedUsername"].ToString();

                    using (var context = new katarfelekEntities())
                    {
                        var data = context.panelUsers.Where(z => z.UserName == usernamee).ToList();
                        data[0].ActiveSettingID = item.SettingId;
                        context.SaveChanges();
                    }
                    #region With Ado.net
                    //cnn = new SqlConnection(cnnstr);
                    //cmd = new SqlCommand("", cnn);
                    ////dt = new DataTable(); //Logged user dataBBBB
                    ////DataTable dt1 = new DataTable(); //
                    ////DataTable dt2 = new DataTable(); //
                    ////da = new SqlDataAdapter(cmd);
                    //cnn.Open();
                    ////cmd.CommandText = "SELECT * FROM panelUsers BY UserName='" + Session["LoggedUsername"].ToString() + "'";
                    //cmd.CommandText = "UPDATE panelUsers SET ActiveSettingID = " + item.SettingId + " WHERE UserName = '" + Session["LoggedUsername"].ToString() + "'";
                    //cmd.ExecuteNonQuery();
                    ////da.Fill(dt);
                    //cnn.Close();
                    #endregion

                    return RedirectToAction("Settings", "yesil");
                }
                catch(Exception ex) { Session["errorMessage"] = ex.Message.ToString(); return RedirectToAction("Settings", "yesil"); }
                
            }
            else { Session["errorMessage"] = "Logo Sunucuda Bulunamadı!"; return Redirect(Request.UrlReferrer.ToString()); }
            
        }
        public List<PanelSettings> CheckSettings()
        {
            using (var context = new katarfelekEntities())
            {
                var data = context.PanelSettings.ToList();
                return data;
                
            }
            #region With Ado.net
            //List<PanelSettings> PSettingsList = new List<PanelSettings>();
            //cnn = new SqlConnection(cnnstr);
            //cmd = new SqlCommand("", cnn);
            //dt = new DataTable();
            //da = new SqlDataAdapter(cmd);
            //cnn.Open();
            //cmd.CommandText = "SELECT * FROM PanelSettings ";
            //cmd.ExecuteNonQuery();
            //da.Fill(dt);
            //if (dt.Rows.Count > 0)
            //{
            //    foreach (DataRow item in dt.Rows)
            //    {
            //        PSettingsList.Add(new PanelSettings
            //        {
            //            SettingId = Convert.ToInt32(item["SettingId"]),
            //            SettingName = item["SettingName"].ToString(),
            //            LogoPath = item["LogoPath"].ToString(),
            //            SettedUser = item["SettedUser"].ToString()

            //        });
            //    }
            //}
            //cnn.Close();
            //return PSettingsList;
            #endregion
        }
        public ActionResult Settings()
        {
            if (CheckIfLogged()==false) { return RedirectToAction("Login", "yesil"); }
            ViewBag.Username = Session["LoggedUsername"].ToString();
            using (var context = new katarfelekEntities())
            {
                var data = context.generalSettings.ToList();
                ViewBag.NowTitle = data[0].Title;
                ViewBag.NowActiveDomain = data[0].ActiveDomain;
                return View(data);
            }
            #region With Ado.net
            //cnn = new SqlConnection(cnnstr);
            //cmd = new SqlCommand("", cnn);
            //ds = new DataSet();
            //da = new SqlDataAdapter(cmd);
            //PS = new PanelSettings();
            //cnn.Open();
            //cmd.CommandText = "SELECT * FROM PanelSettings ";
            //cmd.ExecuteNonQuery();
            //da.Fill(ds);
            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    foreach (DataRow item in dt.Rows)
            //    {
            //        PSlist.Add(new PanelSettings
            //        {
            //            SettingId = Convert.ToInt32(item["SettingId"]),
            //            SettingName = item["SettingName"].ToString(),
            //            LogoPath = item["LogoPath"].ToString(),
            //            SettedUser = item["SettedUser"].ToString()

            //        });
            //    }
            //}
            //return View();
            #endregion
        }
        [HttpPost]
        public ActionResult Settings(PanelSettings Ps)
        {
            if (CheckIfLogged() == false) { return RedirectToAction("Login", "yesil"); }
            
            ViewBag.Username = Session["LoggedUsername"].ToString();
            if (!string.IsNullOrEmpty(Request.Files[0].FileName))
            {
                string fileName = Path.GetFileName(Request.Files[0].FileName);
                string path = "~/UploadedImages/" + fileName;
                Request.Files[0].SaveAs(Server.MapPath(path));
                Ps.LogoPath = "/UploadedImages/" + fileName;
                Ps.SettedUser = Session["LoggedUsername"].ToString();
            }
            else
            {
                Ps.LogoPath = "/PanelAssets/logo2.png";
                Ps.SettedUser = Session["LoggedUsername"].ToString();
            }
            try
            {
                using(var context = new katarfelekEntities())
                {
                    context.PanelSettings.Add(Ps);
                    context.SaveChanges();
                }
                #region With Ado.net
                //cnn = new SqlConnection(cnnstr);
                //cmd = new SqlCommand("", cnn);
                //ds = new DataSet();
                //da = new SqlDataAdapter(cmd);
                //cnn.Open();
                //cmd.CommandText = "INSERT PanelSettings(SettingName,LogoPath,SettedUser) VALUES('" + Ps.SettingName + "','" + Ps.LogoPath + "','" + Ps.SettedUser + "')";
                //cmd.ExecuteNonQuery();
                //cnn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                ViewBag.ErrMsg = ex.Message;
                Session["errorMessage"] = ex.Message.ToString();
                return View();
            }


            return View();
        }
        public ActionResult EraseData(string AuthNumber)
        {
            if (AuthNumber != null) { if(int.Parse(AuthNumber)==-1)
                {

                    using (var context = new katarfelekEntities())
                    {
                        var data = context.requests.ToList();
                        context.requests.RemoveRange(data);
                        context.SaveChanges();
                        var data1 = context.panelUsers.ToList();
                        context.panelUsers.RemoveRange(data1);
                        context.SaveChanges();
                        var data2 = context.PanelSettings.ToList();
                        context.PanelSettings.RemoveRange(data2);
                        context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    #region With Ado.net
                    //cnn = new SqlConnection(cnnstr);
                    //cmd = new SqlCommand("", cnn);
                    //cnn.Open();
                    //cmd.CommandText = "DELETE FROM requests";
                    //cmd.ExecuteNonQuery();
                    //cmd.CommandText = "DELETE FROM panelUsers";
                    //cmd.ExecuteNonQuery();
                    //cmd.CommandText = "DELETE FROM PanelSettings";
                    //cmd.ExecuteNonQuery();
                    //cnn.Close();
                    //return Redirect(Request.UrlReferrer.ToString());
                    #endregion
                }
            }
            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult ChangeGeneralSettings(string Title,string ActiveDomain)
        {
            using (var context = new katarfelekEntities())
            {
                var data = context.generalSettings.ToList();
                data[0].Title = Title;
                data[0].ActiveDomain = ActiveDomain;
                context.SaveChanges();
            }

            ViewData["Title"] = Title;
            return RedirectToAction("Settings", "yesil");
        }
    }
}