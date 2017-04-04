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
        public ActionResult Index()
        {
            DashboardJson dj = new DashboardJson();
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            Utilizator u = (Utilizator)Session["CURENT_USER"];
            DosareRepository dr = new DosareRepository(Convert.ToInt32(u.ID), conStr);
            dj.DOSARE_TOTAL = Convert.ToInt32(dr.CountAll().Result);
            dj.DOSARE_FROM_LAST_LOGIN = 30;
            dj.MESAJE_NOI = 25;

            return PartialView("_Dashboard", dj);
        }
    }
}