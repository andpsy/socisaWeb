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
    public class UtilizatoriDosareController : Controller
    {
        // GET: UtilizatoriDosare
        public ActionResult Index()
        {
            return View();
        }

        /*
        [AuthorizeUser(ActionName = "Utilizatori", Recursive = false)]
        [HttpPost]
        public JsonResult Edit(int id_utilizator, int id_dosar)
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            UtilizatorDosar ud = new UtilizatorDosar(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            ud.ID_UTILIZATOR = id_utilizator;
            ud.ID_DOSAR = id_dosar;
            response r = ud.Insert();
            return Json(r, JsonRequestBehavior.AllowGet);
        }
        */

        [AuthorizeUser(ActionName = "Utilizatori", Recursive = false)]
        [HttpPost]
        public JsonResult Edit(UtilizatorDosar[] UtilizatoriDosare)
        {
            response toReturn = new response(true, "", null, null, new List<Error>());
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int curent_user_id = Convert.ToInt32(Session["CURENT_USER_ID"]);
            foreach (UtilizatorDosar udP in UtilizatoriDosare)
            {
                Dosar d = new Dosar(curent_user_id, conStr, udP.ID_DOSAR);
                UtilizatorDosar ud = new UtilizatorDosar(curent_user_id, conStr);
                ud.ID_UTILIZATOR = udP.ID_UTILIZATOR;
                ud.ID_DOSAR = udP.ID_DOSAR;
                response r = ud.Insert();
                if (!r.Status)
                {
                    toReturn.AddResponse(r);
                }else
                {
                    Mesaj m = new Mesaj(curent_user_id, conStr, ud.ID_DOSAR, DateTime.Now, String.Format("DOSAR NOU ({0})", d.NR_DOSAR_CASCO), String.Format("DOSAR NOU ({0})", d.NR_DOSAR_CASCO), "DOSAR NOU", curent_user_id, (int)Importanta.Low);
                    response rm = m.Insert();
                    if(rm.Status && rm.InsertedId != null)
                    {
                        m.ID = rm.InsertedId;
                        MesajUtilizator mu = new MesajUtilizator(curent_user_id, conStr);
                        mu.ID_MESAJ = Convert.ToInt32(m.ID);
                        mu.ID_UTILIZATOR = ud.ID_UTILIZATOR;
                        response rmu = mu.Insert();
                        if (!rmu.Status)
                        {
                            toReturn.AddResponse(rmu);
                        }
                    }
                    else
                    {
                        toReturn.AddResponse(rm);
                    }
                }
            }
            return Json(toReturn, JsonRequestBehavior.AllowGet);
        }
    }
}