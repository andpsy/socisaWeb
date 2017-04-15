using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using SOCISA;
using SOCISA.Models;
using Newtonsoft.Json;
using System.Reflection;

namespace socisaWeb.Controllers
{
    [Authorize]
    public class DocumenteScanateController : Controller
    {
        // GET: DocumenteScanate
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search()
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            return PartialView("_DocumenteScanate", new DocumentView(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr));
        }

        
        public JsonResult Details(int id)
        {
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            DosareRepository dr = new DosareRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            Dosar d = (Dosar)dr.Find(id).Result;
            response r = d.GetDocumente();

            return Json(r, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public JsonResult Edit(DocumentView DocumentView)
        {
            response r = new response();
            string conStr = ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            if (DocumentView.CurDocumentScanat.ID == null) // insert
            {
                DocumentScanat d = new DocumentScanat(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
                PropertyInfo[] pis = DocumentView.CurDocumentScanat.GetType().GetProperties();
                foreach(PropertyInfo pi in pis)
                {
                    pi.SetValue(d, pi.GetValue(DocumentView.CurDocumentScanat));
                }
                r = d.Insert();
                return Json(r, JsonRequestBehavior.AllowGet);
            }else // edit
            {
                DocumenteScanateRepository dsr = new DocumenteScanateRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
                DocumentScanat d = (DocumentScanat)dsr.Find(Convert.ToInt32(DocumentView.CurDocumentScanat.ID)).Result;
                string s = JsonConvert.SerializeObject(DocumentView.CurDocumentScanat, Formatting.None, new Newtonsoft.Json.Converters.IsoDateTimeConverter() { DateTimeFormat = "dd.MM.yyyy" });
                r = d.Update(s);
                return Json(r, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult PostFile()
        {
            HttpPostedFileBase f = Request.Files[0];
            string initFName = f.FileName;
            string extension = f.FileName.Substring(f.FileName.LastIndexOf('.'));
            string newFName = Guid.NewGuid() + extension;
            Request.Files[0].SaveAs(System.IO.Path.Combine(CommonFunctions.GetScansFolder(), newFName));
            string toReturn = "{\"DENUMIRE_FISIER\":\"" + initFName + "\",\"EXTENSIE_FISIER\":\"" + extension + "\",\"CALE_FISIER\":\"" + newFName + "\"}";
            return Json(toReturn, JsonRequestBehavior.AllowGet);
        }
    }
}