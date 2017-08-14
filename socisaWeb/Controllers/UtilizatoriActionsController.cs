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
    public class UtilizatoriActionsController : Controller
    {
        [AuthorizeUser(ActionName = "Utilizatori", Recursive = false)]
        [HttpPost]
        public JsonResult Save(int ID_UTILIZATOR, int ID_ACTION)
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);
            UtilizatorAction ua = new UtilizatorAction(_CURENT_USER_ID, conStr);
            ua.ID_UTILIZATOR = ID_UTILIZATOR;
            ua.ID_ACTION = ID_ACTION;
            response r = ua.Insert();
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Utilizatori", Recursive = false)]
        [HttpPost]
        public JsonResult Delete(int ID_UTILIZATOR, int ID_ACTION)
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);
            UtilizatorAction ua = new UtilizatorAction(_CURENT_USER_ID, conStr);
            ua.ID_UTILIZATOR = ID_UTILIZATOR;
            ua.ID_ACTION = ID_ACTION;
            response r = ua.Delete();
            return Json(r, JsonRequestBehavior.AllowGet);
        }
    }
}