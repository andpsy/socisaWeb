using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOCISA;
using SOCISA.Models;
using System.Configuration;

namespace socisaWeb.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        [AuthorizeUser(ActionName = "Dashboard", Recursive = false)]
        public ActionResult Index()
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            Utilizator u = (Utilizator)Session["CURENT_USER"];
            DashboardJson dj = new DashboardJson(Convert.ToInt32( u.ID), Convert.ToInt32(Session["ID_SOCIETATE"]), conStr);
            /*
            DosareRepository dr = new DosareRepository(Convert.ToInt32(u.ID), conStr);
            dj.DOSARE_TOTAL = Convert.ToInt32(dr.CountAll().Result);
            dj.DOSARE_FROM_LAST_LOGIN = Convert.ToInt32(dr.CountFromLastLogin().Result);
            dj.MESAJE_NOI = 0;
            */
            return PartialView("_Dashboard", dj);
        }

        [AuthorizeUser(ActionName = "Dashboard", Recursive = false)]
        public ActionResult IndexMain()
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            Utilizator u = (Utilizator)Session["CURENT_USER"];
            DashboardJson dj = new DashboardJson(Convert.ToInt32(u.ID), Convert.ToInt32(Session["ID_SOCIETATE"]), conStr);
            return PartialView("_DashboardMain", dj);
        }

        [AuthorizeUser(ActionName = "Dashboard", Recursive = false)]
        public ActionResult GetDosareNoi()
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            Utilizator u = (Utilizator)Session["CURENT_USER"];
            /*
            Dosar[] ds = (Dosar[])u.GetDosareNoi(Convert.ToInt32(Session["ID_SOCIETATE"])).Result;
            List<DosarExtended> des = new List<DosarExtended>(ds.Length);
            foreach(Dosar d in ds)
            {
                DosarExtended de = new DosarExtended();
                de.Dosar = d;
                de.AsiguratCasco = (Asigurat)d.GetAsiguratCasco().Result;
                de.AsiguratRca = (Asigurat)d.GetAsiguratCasco().Result;
                de.AutoCasco = (Auto)d.GetAutoCasco().Result;
                de.AutoRca = (Auto)d.GetAutoRca().Result;
                de.Intervenient = (Intervenient)d.GetIntervenient().Result;
                de.SocietateCasco = (SocietateAsigurare)d.GetSocietateCasco().Result;
                de.SocietateRca = (SocietateAsigurare)d.GetSocietateRca().Result;
                de.TipDosar = (Nomenclator)d.GetTipDosar().Result;
                des.Add(de);
            }
            */
            DashBoardView dbv = new DashBoardView(u, conStr, Convert.ToInt32(Session["ID_SOCIETATE"]));
            //return PartialView("_DosareNoi", des.ToArray());
            return PartialView("_DosareNoi", dbv);
        }

    }
}