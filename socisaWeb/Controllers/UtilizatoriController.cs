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
        public ActionResult Index()
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            UtilizatorView uv = new UtilizatorView(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            return PartialView("Utilizatori", uv);
        }

        public JsonResult IndexJson()
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            UtilizatorView uv = new UtilizatorView(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            return Json(uv, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save(Utilizator Utilizator)
        {
            response r = new response();

            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
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

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginJson());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginJson model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            UtilizatoriRepository ur = new UtilizatoriRepository(null, conStr);
            response r = ur.Login(model.Username, model.Password);
            if (r.Result != null)
            {
                Utilizator u = (Utilizator)r.Result;
                u.IS_ONLINE = true;
                Session["LAST_LOGIN"] = DateTime.Now;
                u.Update();
                Session["CURENT_USER"] = u;
                Session["CURENT_USER_ID"] = u.ID;
                FormsAuthentication.SetAuthCookie(u.USER_NAME, false);
                NomenclatoareRepository nr = new NomenclatoareRepository(Convert.ToInt32(u.ID), conStr);
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
                    SocietatiAsigurareRepository sar = new SocietatiAsigurareRepository(Convert.ToInt32(u.ID), conStr);
                    SocietateAsigurare sa = (SocietateAsigurare)sar.Find(Convert.ToInt32(u.ID_SOCIETATE)).Result;
                    Session["SOCIETATE_ASIGURARE"] = sa;
                    return Redirect(returnUrl ?? Url.Action("Index", "Home"));
                    //return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Autentificare esuata!");
                return View(model);
            }
        }

        public ActionResult SelectSocietate()
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            Utilizator u = (Utilizator)Session["CURENT_USER"];
            SocietatiAsigurareRepository sar = new SocietatiAsigurareRepository(Convert.ToInt32(u.ID), conStr);
            SocietateAsigurare[] sas = (SocietateAsigurare[])sar.GetAll().Result;
            return View(sas);
        }

        [HttpPost]
        public ActionResult SelectSocietate(FormCollection model)
        {
            if(model["item.ID"] != null)
            {
                string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
                Session["ID_SOCIETATE"] = model["item.ID"];
                SocietatiAsigurareRepository sar = new SocietatiAsigurareRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
                SocietateAsigurare sa = (SocietateAsigurare)sar.Find(Convert.ToInt32(model["item.ID"])).Result;
                Session["SOCIETATE_ASIGURARE"] = sa;
                return RedirectToAction("Index", "Home");
            }
            return View(model);
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