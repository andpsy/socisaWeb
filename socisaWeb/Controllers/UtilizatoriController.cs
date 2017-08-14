using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using SOCISA;
using SOCISA.Models;
using System.Configuration;
using System.Web.Security;
using System.Reflection;

namespace socisaWeb
{
    [Authorize]
    public class UtilizatoriController : Controller
    {
        [AuthorizeUser(ActionName = "Utilizatori", Recursive = false)]
        public ActionResult Index()
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            UtilizatorView uv = new UtilizatorView(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            return PartialView("Utilizatori", uv);
        }

        [AuthorizeUser(ActionName = "Utilizatori", Recursive = false)]
        public JsonResult IndexJson()
        {
            string conStr = Session["conStr"].ToString();  //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            UtilizatorView uv = new UtilizatorView(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            return Json(uv, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Utilizatori", Recursive = false)]
        [HttpPost]
        public JsonResult Save(Utilizator Utilizator)
        {
            response r = new response();

            string conStr = Session["conStr"].ToString();  //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);
            UtilizatoriRepository ur = new UtilizatoriRepository(_CURENT_USER_ID, conStr);
            Utilizator u = new Utilizator(_CURENT_USER_ID, conStr);
            PropertyInfo[] pis = Utilizator.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                pi.SetValue(u, pi.GetValue(Utilizator));
            }
            if (Utilizator.ID == null) // insert
            {
                r = u.Insert();
            }
            else // update
            {
                r = u.Update();
            }
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Utilizatori", Recursive = false)]
        [HttpPost]
        public JsonResult SetPassword(int id_utilizator, string password, string confirmPassword)
        {
            response r = new response();
            if(password != confirmPassword) // alte validari aici !!!
            {
                r = new response(false, "Parolele nu coincid!", null, null, new List<Error>() { ErrorParser.ErrorMessage("passwordsDontMatch") });
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            string conStr = Session["conStr"].ToString();  //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);
            UtilizatoriRepository ur = new UtilizatoriRepository(_CURENT_USER_ID, conStr);
            r = ur.SetPassword(id_utilizator, password);
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Utilizatori", Recursive = false)]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            response r = new response();

            string conStr = Session["conStr"].ToString();  //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);
            UtilizatoriRepository ur = new UtilizatoriRepository(_CURENT_USER_ID, conStr);
            Utilizator u = new Utilizator(_CURENT_USER_ID, conStr, id);
            r = u.Delete();
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginJson());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginJson model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //string conStr = null;
            //conStr = Request.Url.ToString().IndexOf("_test") > 0 || Request.Url.ToString().IndexOf("8081") > 0 ? ConfigurationManager.ConnectionStrings["MySQLConnectionString_test"].ConnectionString : ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString; // separam socisa de socisa_test
            //Session["conStr"] = conStr;
            //UtilizatoriRepository ur = new UtilizatoriRepository(null, conStr);
            UtilizatoriRepository ur = new UtilizatoriRepository(null, Session["conStr"].ToString());
            response r = ur.Login(model.Username, model.Password);
            if (r.Result != null)
            {
                Utilizator u = (Utilizator)r.Result;
                u.IS_ONLINE = true;
                Session["LAST_LOGIN"] = DateTime.Now;
                //string s = "{'IS_ONLINE':true}";
                u.Update();
                Session["CURENT_USER"] = u;
                Session["CURENT_USER_ID"] = u.ID;
                FormsAuthentication.SetAuthCookie(u.USER_NAME, false);
                //NomenclatoareRepository nr = new NomenclatoareRepository(Convert.ToInt32(u.ID), conStr);
                NomenclatoareRepository nr = new NomenclatoareRepository(Convert.ToInt32(u.ID), Session["conStr"].ToString());
                Nomenclator n = (Nomenclator)nr.Find("TIP_UTILIZATORI", Convert.ToInt32(u.ID_TIP_UTILIZATOR)).Result;

                Session["CURENT_USER_TYPE"] = n;
                Session["CURENT_USER_RIGHTS"] = (Drept[])u.GetDrepturi().Result;
                Session["CURENT_USER_ACTIONS"] = (SOCISA.Models.Action[])u.GetActions().Result;
                Session["CURENT_USER_SETTINGS"] = (Setare[])u.GetSetari().Result;
                Session["CURENT_USER_SOCIETATI_ADMINISTRATE"] = (SocietateAsigurare[])u.GetSocietatiAdministrate().Result;

                if (u.ID_SOCIETATE == null && (n != null && n.DENUMIRE.ToUpper() == "ADMINISTRATOR"))
                {
                    //return Redirect(returnUrl ?? Url.Action("SelectSocietate", "UtilizatoriController"));
                    return RedirectToAction("SelectSocietate");
                }
                else
                {
                    Session["ID_SOCIETATE"] = u.ID_SOCIETATE;
                    //SocietatiAsigurareRepository sar = new SocietatiAsigurareRepository(Convert.ToInt32(u.ID), conStr);
                    SocietatiAsigurareRepository sar = new SocietatiAsigurareRepository(Convert.ToInt32(u.ID), Session["conStr"].ToString());
                    SocietateAsigurare sa = (SocietateAsigurare)sar.Find(Convert.ToInt32(u.ID_SOCIETATE)).Result;
                    Session["SOCIETATE_ASIGURARE"] = sa;

                    //return RedirectToAction("Index", "Home");
                    //return Redirect(returnUrl ?? Url.Action("Index", "Home"));
                    //return Redirect(returnUrl ?? Url.Action("IndexMain", "Dashboard"));
                    if (returnUrl != null && returnUrl != "/")
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("IndexMain", "Dashboard");
                }
            }
            else
            {
                ModelState.AddModelError("", "Autentificare esuata!");
                return View(model);
            }
        }

        [AuthorizeUser(ActionName = "Dashboard", Recursive = false)]
        public ActionResult SelectSocietate()
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            Utilizator u = (Utilizator)Session["CURENT_USER"];
            SocietatiAsigurareRepository sar = new SocietatiAsigurareRepository(Convert.ToInt32(u.ID), conStr);
            SocietateAsigurare[] sas = (SocietateAsigurare[])sar.GetAll().Result;
            return View(sas);
        }

        [AuthorizeUser(ActionName = "Dashboard", Recursive = false)]
        [HttpPost]
        public ActionResult SelectSocietate(FormCollection model)
        {
            string conStr = Session["conStr"].ToString(); // ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            Utilizator u = (Utilizator)Session["CURENT_USER"];
            SocietatiAsigurareRepository sar = new SocietatiAsigurareRepository(Convert.ToInt32(u.ID), conStr);
            SocietateAsigurare[] sas = (SocietateAsigurare[])sar.GetAll().Result;
            if (!ModelState.IsValid)
            {
                return View(sas);
            }
            /* -- modelul cu lista --
            if(model["item.ID"] != null)
            {
                Session["ID_SOCIETATE"] = model["item.ID"];
                SocietateAsigurare sa = (SocietateAsigurare)sar.Find(Convert.ToInt32(model["item.ID"])).Result;
                Session["SOCIETATE_ASIGURARE"] = sa;
                return RedirectToAction("Index", "Home");
            }
            */
            if (model["Societate"] != null && model["Societate"] != "")
            {
                Session["ID_SOCIETATE"] = model["Societate"];
                SocietateAsigurare sa = (SocietateAsigurare)sar.Find(Convert.ToInt32(model["Societate"])).Result;
                Session["SOCIETATE_ASIGURARE"] = sa;
                //return RedirectToAction("Index", "Home");
                return RedirectToAction("IndexMain", "Dashboard");
            }
            else
            {
                ModelState.AddModelError("", "Selectati societatea!");
                return View(sas);
            }
        }

        public ActionResult Logout()
        {
            try
            {
                Utilizator u = (Utilizator)Session["CURENT_USER"];
                u.IS_ONLINE = false;
                u.LAST_LOGIN = Convert.ToDateTime(Session["LAST_LOGIN"]);
                u.Update();
            }
            catch { }
            Session["CURENT_USER"] = null;
            Session["CURENT_USER_ID"] = null;
            Session["ID_SOCIETATE"] = null;
            Session["SOCIETATE_ASIGURARE"] = null;
            Session["CURENT_USER_TYPE"] = null;
            Session["CURENT_USER_RIGHTS"] = null;
            Session["CURENT_USER_ACTIONS"] = null;
            Session["CURENT_USER_SETTINGS"] = null;
            Session["CURENT_USER_SOCIETATI_ADMINISTRATE"] = null;

            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}