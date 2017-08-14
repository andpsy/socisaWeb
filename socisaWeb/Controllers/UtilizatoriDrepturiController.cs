using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using SOCISA;
using SOCISA.Models;

namespace socisaWeb
{
    [Authorize]
    public class UtilizatoriDrepturiController : Controller
    {
        [AuthorizeUser(ActionName = "Utilizatori", Recursive = false)]
        [HttpPost]
        public JsonResult Save(int ID_UTILIZATOR, int ID_DREPT)
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);
            UtilizatorDrept ud = new UtilizatorDrept(_CURENT_USER_ID, conStr);
            ud.ID_UTILIZATOR = ID_UTILIZATOR;
            ud.ID_DREPT = ID_DREPT;
            response r = ud.Insert();
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Utilizatori", Recursive = false)]
        [HttpPost]
        public JsonResult Delete(int ID_UTILIZATOR, int ID_DREPT)
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);
            UtilizatorDrept ud = new UtilizatorDrept(_CURENT_USER_ID, conStr);
            ud.ID_UTILIZATOR = ID_UTILIZATOR;
            ud.ID_DREPT = ID_DREPT;
            response r = ud.Delete();
            return Json(r, JsonRequestBehavior.AllowGet);
        }
    }
}