using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using SOCISA;
using SOCISA.Models;
using System.Reflection;
using Newtonsoft.Json;

namespace socisaWeb.Controllers
{
    [Authorize]
    public class MesajeController : Controller
    {
        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        public ActionResult Index()
        {
            return PartialView("_MesajeView", new MesajView());
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpGet]
        public JsonResult GetMessages(int? id)
        {
            //if (id == null || id == -1) return PartialView("_MesajeView", null);

            MesajView mv = new MesajView();
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            DosareRepository dr = new DosareRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            Dosar d = (Dosar)dr.Find(Convert.ToInt32(id)).Result;

            Mesaj mesaj = new Mesaj(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            Utilizator[] us = (Utilizator[])mesaj.GetReceivers().Result;
            Utilizator s = (Utilizator)mesaj.GetSender().Result;
            Nomenclator n = (Nomenclator)mesaj.GetTipMesaj().Result;
            DateTime? da = (DateTime?)mesaj.GetMessageReadDate(Convert.ToInt32(Session["CURENT_USER_ID"])).Result;

            mv.MesajJson = new MesajJson(mesaj, s, us, n, da);
            mv.InvolvedParties = (Utilizator[])d.GetInvolvedParties().Result;
            Mesaj[] ms = (Mesaj[])d.GetMesaje().Result;
            List<MesajJson> ls = new List<MesajJson>();
            foreach(Mesaj m in ms)
            {
                ls.Add(new MesajJson(m, (Utilizator)m.GetSender().Result, (Utilizator[])m.GetReceivers().Result, (Nomenclator)m.GetTipMesaj().Result, (DateTime?)m.GetMessageReadDate(Convert.ToInt32(Session["CURENT_USER_ID"])).Result));
            }

            mv.MesajeJson = ls.ToArray();
            mv.TipuriMesaj = (Nomenclator[])(new NomenclatoareRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr).GetAll("tip_mesaje").Result);
            //return PartialView("_MesajeView", mv);
            return Json(mv, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpGet]
        public JsonResult GetSentMessages(int? id)
        {
            //if (id == null || id == -1) return PartialView("_MesajeView", null);

            MesajView mv = new MesajView();
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            DosareRepository dr = new DosareRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            Dosar d = (Dosar)dr.Find(Convert.ToInt32(id)).Result;

            Mesaj mesaj = new Mesaj(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            Utilizator[] us = (Utilizator[])mesaj.GetReceivers().Result;
            Utilizator s = (Utilizator)mesaj.GetSender().Result;
            Nomenclator n = (Nomenclator)mesaj.GetTipMesaj().Result;
            DateTime? da = (DateTime?)mesaj.GetMessageReadDate(Convert.ToInt32(Session["CURENT_USER_ID"])).Result;

            mv.MesajJson = new MesajJson(mesaj, s, us, n, da);
            mv.InvolvedParties = (Utilizator[])d.GetInvolvedParties().Result;
            Mesaj[] ms = (Mesaj[])d.GetSentMesaje().Result;
            List<MesajJson> ls = new List<MesajJson>();
            foreach (Mesaj m in ms)
            {
                ls.Add(new MesajJson(m, (Utilizator)m.GetSender().Result, (Utilizator[])m.GetReceivers().Result, (Nomenclator)m.GetTipMesaj().Result, (DateTime?)m.GetMessageReadDate(Convert.ToInt32(Session["CURENT_USER_ID"])).Result));
            }

            mv.MesajeJson = ls.ToArray();
            mv.TipuriMesaj = (Nomenclator[])(new NomenclatoareRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr).GetAll("tip_mesaje").Result);
            //return PartialView("_MesajeView", mv);
            return Json(mv, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpPost]
        public JsonResult GetNewMessages(string j)
        {
            dynamic x = JsonConvert.DeserializeObject(j);
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            DosareRepository dr = new DosareRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            Dosar d = (Dosar)dr.Find(Convert.ToInt32(x.id_dosar)).Result;
            //return Json(d.GetNewMesaje(Convert.ToDateTime(x.last_refresh)), JsonRequestBehavior.AllowGet);
            return Json(d.GetNewMesaje(DateTime.ParseExact(Convert.ToString(x.last_refresh), "dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)), JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpPost]
        public JsonResult Send(MesajJson MesajJson)
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            response r = new response();
            Mesaj m = new Mesaj(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            PropertyInfo[] pis = m.GetType().GetProperties();
            foreach(PropertyInfo pi in pis)
            {
                pi.SetValue(m, pi.GetValue(MesajJson.Mesaj));
            }
            m.DATA = DateTime.Now;
            m.ID_SENDER = Convert.ToInt32(Session["CURENT_USER_ID"]);
            r = m.Insert();
            if (r.Status && r.InsertedId != null)
            {
                foreach (Utilizator u in MesajJson.Receivers)
                {
                    MesajUtilizator mu = new MesajUtilizator(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
                    mu.ID_MESAJ = Convert.ToInt32(r.InsertedId);
                    mu.ID_UTILIZATOR = Convert.ToInt32(u.ID);
                    mu.Insert();
                }
            }
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpPost]
        public JsonResult SetDataCitire(MesajJson MesajJson)
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            response r = new response();
            Mesaj m = new Mesaj(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            PropertyInfo[] pis = m.GetType().GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                pi.SetValue(m, pi.GetValue(MesajJson.Mesaj));
            }
            r = m.SetMessageReadDate(Convert.ToInt32(Session["CURENT_USER_ID"]), (DateTime)MesajJson.DataCitire);
            return Json(r, JsonRequestBehavior.AllowGet);
        }
    }
}