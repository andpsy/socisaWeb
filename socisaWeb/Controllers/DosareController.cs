using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOCISA;
using SOCISA.Models;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace socisaWeb.Controllers
{
    [Authorize]
    public class DosareController : Controller
    {
        // GET: Dosare
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Search()
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            return PartialView("_DosareNavigator", new DosarView(Convert.ToInt32(Session["CURENT_USER_ID"]), Convert.ToInt32(Session["ID_SOCIETATE"]), conStr));
        }

        [HttpPost]
        public JsonResult Search(DosarJson DosarJson)
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _CURENT_USER_ID = Convert.ToInt32(Session["CURENT_USER_ID"]);
            DosareRepository dr = new DosareRepository(_CURENT_USER_ID, conStr);
            string jsonFilter = JsonConvert.SerializeObject(DosarJson);
            JObject f = (JObject)JsonConvert.DeserializeObject(Request.Form[0]);
            JToken j = f["dosarJson"];
            if(j["DataEvenimentStart"] != null && !String.IsNullOrEmpty(j["DataEvenimentStart"].ToString()) && j["DataEvenimentEnd"] != null && !String.IsNullOrEmpty(j["DataEvenimentEnd"].ToString()))
            {
                j["DATA_EVENIMENT"] = CommonFunctions.ToMySqlFormatDate(Convert.ToDateTime(j["DataEvenimentStart"].ToString())) + "?" + CommonFunctions.ToMySqlFormatDate(Convert.ToDateTime(j["DataEvenimentEnd"].ToString()));
            }
            response r = dr.GetFiltered(null, null, JsonConvert.SerializeObject(j), null);
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        // GET: Dosare/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Dosare/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dosare/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Dosare/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Dosare/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Dosare/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Dosare/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
