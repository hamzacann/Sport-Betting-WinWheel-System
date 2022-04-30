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
    public class HomeController : Controller
    {
        //public string cnnstr = ConfigurationManager.ConnectionStrings["DCDatabase"].ConnectionString;
        //public string cnnstr = ConfigurationManager.ConnectionStrings["DCLocalDatabase"].ConnectionString;
        public SqlConnection cnn;
        public SqlCommand cmd;
        public DataSet ds;
        public DataTable dt;
        public SqlDataAdapter da;
        List<RequestClass> list;
        List<PanelSettings> PSlist;
        public PanelClass publicPC = new PanelClass();
        public PanelSettings PS;
        public Dictionary<int, string> dict = new Dictionary<int, string>();
        public Dictionary<int, double> dictPrize = new Dictionary<int, double>();

        public void LoadSettings()
        {
            
            using (var context = new katarfelekEntities())
            {
                var data = context.generalSettings.ToList();
                HttpContext.Application.Add("Title", data[0].Title);
                Session.Add("Title", data[0].Title);
                Session.Add("ActiveDomain", data[0].ActiveDomain);

            }
        }

        public HomeController()
        {
            
            dict.Add(1, "500 TL");
            dict.Add(2, "5 TL");
            dict.Add(3, "75 TL");
            dict.Add(4, "20 TL");
            dict.Add(5, "5 TL");
            dict.Add(6, "PAS");
            dict.Add(7, "10 TL");
            dict.Add(8, "Araba TL");
            dict.Add(9, "15 TL");
            dict.Add(10, "50 TL");

            dictPrize.Add(1, 500);
            dictPrize.Add(2, 5);
            dictPrize.Add(3, 75);
            dictPrize.Add(4, 20);
            dictPrize.Add(5, 5);
            dictPrize.Add(6, 0);
            dictPrize.Add(7, 10);
            dictPrize.Add(8, 0);
            dictPrize.Add(9, 15);
            dictPrize.Add(10, 50);
        }
        

        public bool checkIfLogged()
        {
            var obj = Session["LoggedUser11"];
            if (obj != null)
            {
                if (Session["LoggedUser11"].ToString() != null)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public int getPrize()
        {
            //int[] abc = { 1,2,3,4,5,6,7,9,10 };
            //int[] abc5 = { 1 };
            int[] abc15 = { 4, 10, 3 };
            int[] abc80 = { 2, 5, 6, 7, 9 };
            int a = 0;
            Random random = new Random();
            int chance = random.Next(0, 99);
            //if (chance <= 5) { a = random.Next(1); return abc5[a]; }
            if (chance <= 20) { a = random.Next(0, 2); return abc15[a]; }
            else if (chance <= 100) { a = random.Next(0, 4); return abc80[a]; } //%80 5 10 15 tl
            else { a = random.Next(0, 4); return abc80[a]; } //%80 5 10 15 tl

        }
        [HttpGet]
        public JsonResult getPrizee()
        {
            string usernamee = Session["LoggedUser11"].ToString();
            string domain = Session["ActiveDomain"].ToString();
            var getsegmentnum = getPrize();

            try
            {
                using(var check = new katarfelekEntities())
                {
                    var client = new RestClient("https://"+domain+".com/Api/PayList?UserName="+usernamee+"&token=62365083e665aeb0e5433871");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    IRestResponse response = client.Execute(request);

                    var paylist = JsonConvert.DeserializeObject<List<PayList>>(response.Content);
                    var bytoday = paylist.Where(p => p.createDateTR.Date == DateTime.Now.Date);
                    var upDown1 = bytoday.Where(t => t.upDown == 1);
                    var todaytotal = upDown1.Select(x=>x.addBalance).Sum();
                    if (todaytotal < 200)
                    {
                        return Json(new
                        {
                            success = 0,
                            msg = "Günlük Çevirme Hakkı Kazanmak İçin En Az 200TL Yükleme Yapmanız Gerekmektedir!"
                        }, JsonRequestBehavior.AllowGet);
                    }


                }





                requests RC = new requests();
                using(var context = new katarfelekEntities())
                {
                    
                    var data = context.requests.Where(r=>r.UserName == usernamee).OrderByDescending(r=>r.Id).Take(1).ToList();
                    if(data.Count > 0)
                    {
                        RC.Id= data[0].Id;
                        RC.UserName=data[0].UserName;
                        RC.Prize=data[0].Prize;
                        RC.RequestTime= data[0].RequestTime;
                    }
                }

                if (RC.RequestTime.Date != DateTime.Now.Date)
                {


                    RC.UserName = Session["LoggedUser11"].ToString();
                    RC.Prize = dict[getsegmentnum].ToString();
                    RC.RequestTime = DateTime.Now;

                    using (var context = new katarfelekEntities())
                    {
                        // Add data to the particular table
                        context.requests.Add(RC);
                        context.SaveChanges();
                    }


                    var client = new RestClient("https://"+domain+".com/Api/AddBalance?UserName=" + usernamee + "&token=62365083e665aeb0e5433871&price=" + dictPrize[getsegmentnum]);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    IRestResponse response = client.Execute(request);

                    var data = JsonConvert.DeserializeObject<responseClass>(response.Content);

                    return Json(new
                    {
                        success = getsegmentnum,
                        msg = dict[getsegmentnum].ToString(),
                        bresult = data.newBalance
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        success = 0,
                        msg = "Günlük Çevirme Hakkı Zaten Kullanılmış!"
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch(Exception ex)
            {
                return Json(new
                {
                    success = 0,
                    msg = ex.Message.ToString()
                }, JsonRequestBehavior.AllowGet);
            }

            

        }
        public ActionResult Index()
        {
            LoadSettings();
            var key = checkIfLogged();
            //var key = true;
            if (key == false)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        public ActionResult Login()
        {
            LoadSettings();
            var key = checkIfLogged();
            if(key)
            {
                ViewData.Clear();
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username,string password)
        {
            string domain = Session["ActiveDomain"].ToString();
            var client = new RestClient("https://"+domain+".com/Api/CheckCustomer?UserName=" + username.ToLower().Trim() + "&token=62365083e665aeb0e5433871");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            IRestResponse response = client.Execute(request);
            if (response != null)
            {
                if (response.Content != null)
                {
                    CheckUserRoot check = new CheckUserRoot();
                    var ioo = response.Content.IndexOf("errorMsg");
                    if(ioo > -1)
                    {
                        check = JsonConvert.DeserializeObject<CheckUserRoot>(response.Content);
                        if (check.status == false)
                        {
                            //ViewData.Add("UserError", check.errorMsg);
                            ViewBag.Error = check.errorMsg;
                            return View();
                        }
                        else
                        {
                            return View();
                        }
                    }
                    else
                    {
                        UserRoot user = JsonConvert.DeserializeObject<UserRoot>(response.Content);
                        if (user.username == username.ToLower().Trim() && user.password == password)
                        {
                            Session["LoggedUser11"] = user.username;
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Login", "Home");
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            else
                return RedirectToAction("Login", "Home");


        }
        

    }
}