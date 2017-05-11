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
            DashboardJson dj = new DashboardJson(Convert.ToInt32( u.ID), conStr);
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
            DashboardJson dj = new DashboardJson(Convert.ToInt32(u.ID), conStr);
            /*
            DosareRepository dr = new DosareRepository(Convert.ToInt32(u.ID), conStr);
            dj.DOSARE_TOTAL = Convert.ToInt32(dr.CountAll().Result);
            dj.DOSARE_FROM_LAST_LOGIN = Convert.ToInt32(dr.CountFromLastLogin().Result);
            dj.MESAJE_NOI = 0;
            */
            return PartialView("_DashboardMain", dj);
        }
    }
}