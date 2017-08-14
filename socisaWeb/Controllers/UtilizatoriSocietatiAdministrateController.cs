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
    public class UtilizatoriSocietatiAdministrateController : Controller
    {
        [AuthorizeUser(ActionName = "Utilizatori", Recursive = false)]
        [HttpPost]
        public JsonResult Save(int ID_UTILIZATOR, int ID_SOCIETATE_ADMINISTRATA)
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);
            UtilizatorSocietateAdministrata usa = new UtilizatorSocietateAdministrata(_CURENT_USER_ID, conStr);
            usa.ID_UTILIZATOR = ID_UTILIZATOR;
            usa.ID_SOCIETATE = ID_SOCIETATE_ADMINISTRATA;
            response r = usa.Insert();
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Utilizatori", Recursive = false)]
        [HttpPost]
        public JsonResult Delete(int ID_UTILIZATOR, int ID_SOCIETATE_ADMINISTRATA)
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);
            UtilizatorSocietateAdministrata usa = new UtilizatorSocietateAdministrata(_CURENT_USER_ID, conStr);
            usa.ID_UTILIZATOR = ID_UTILIZATOR;
            usa.ID_SOCIETATE = ID_SOCIETATE_ADMINISTRATA;
            response r = usa.Delete();
            return Json(r, JsonRequestBehavior.AllowGet);
        }
    }
}