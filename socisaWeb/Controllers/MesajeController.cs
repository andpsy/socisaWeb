using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using SOCISA;
using SOCISA.Models;
using System.Reflection;

namespace socisaWeb.Controllers
{
    [Authorize]
    public class MesajeController : Controller
    {
        // GET: Messages
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetMessages(int? id)
        {
            if (id == null || id == -1) return PartialView("_MesajeView", null);

            MesajView mv = new MesajView();
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            DosareRepository dr = new DosareRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            Dosar d = (Dosar)dr.Find(Convert.ToInt32(id)).Result;

            mv.Mesaj = new Mesaj(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            mv.Receivers = new List<Utilizator>().ToArray();
            mv.InvolvedParties = (Utilizator[])d.GetInvolvedParties().Result;
            mv.Mesaje = (Mesaj[])dr.GetMesaje(d).Result;
            mv.TipuriMesaj = (Nomenclator[])(new NomenclatoareRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr).GetAll("tip_mesaje").Result);
            return PartialView("_MesajeView", mv);
        }

        [HttpPost]
        public JsonResult Send(MesajView MesajView)
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            response r = new response();
            Mesaj m = new Mesaj(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            PropertyInfo[] pis = m.GetType().GetProperties();
            foreach(PropertyInfo pi in pis)
            {
                pi.SetValue(m, pi.GetValue(MesajView.Mesaj));
            }
            m.DATA = DateTime.Now;
            m.ID_SENDER = Convert.ToInt32(Session["CURENT_USER_ID"]);
            r = m.Insert();
            if (r.Status && r.InsertedId != null)
            {
                foreach (Utilizator u in MesajView.Receivers)
                {
                    MesajUtilizator mu = new MesajUtilizator(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
                    mu.ID_MESAJ = Convert.ToInt32(r.InsertedId);
                    mu.ID_UTILIZATOR = Convert.ToInt32(u.ID);
                    mu.Insert();
                }
            }
            return Json(r, JsonRequestBehavior.AllowGet);
        }
    }
}