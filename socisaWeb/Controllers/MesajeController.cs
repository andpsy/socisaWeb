using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using SOCISA;
using SOCISA.Models;

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
            mv.Receivers = "";
            mv.InvolvedParties = (Utilizator[])d.GetInvolvedParties().Result;
            mv.Mesaje = (Mesaj[])dr.GetMesaje(d).Result;
            return PartialView("_MesajeView", mv);
        }
    }
}