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
        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        public ActionResult Index()
        {
            return PartialView();
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        public ActionResult Search()
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            return PartialView("_DocumenteScanate", new DocumentView(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr));
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        public JsonResult Details(int id)
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            /*
            DosareRepository dr = new DosareRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            Dosar d = (Dosar)dr.Find(id).Result;
            response r = d.GetDocumente();
            //return Json(r, JsonRequestBehavior.AllowGet);
            JsonResult result = Json(r, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = Int32.MaxValue;
            return result;
            */
            DocumentView dv = new DocumentView(Convert.ToInt32(Session["CURENT_USER_ID"]), id, conStr);
            JsonResult result = Json(dv, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = Int32.MaxValue;
            return result;
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        public JsonResult Detail(int id)
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            DocumenteScanateRepository dsr = new DocumenteScanateRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            response r = dsr.Find(id);

            //return Json(r, JsonRequestBehavior.AllowGet);
            JsonResult result = Json(r, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = Int32.MaxValue;
            return result;
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpPost]
        public JsonResult Edit(DocumentScanat CurDocumentScanat)
        {
            response r = new response();
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            if (CurDocumentScanat.ID == null) // insert
            {
                DocumentScanat d = new DocumentScanat(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
                PropertyInfo[] pis = CurDocumentScanat.GetType().GetProperties();
                foreach(PropertyInfo pi in pis)
                {
                    pi.SetValue(d, pi.GetValue(CurDocumentScanat));
                }
                r = d.Insert();
                //return Json(r, JsonRequestBehavior.AllowGet);
                JsonResult result = Json(r, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = Int32.MaxValue;
                return result;
            }
            else // edit
            {
                DocumenteScanateRepository dsr = new DocumenteScanateRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
                DocumentScanat d = (DocumentScanat)dsr.Find(Convert.ToInt32(CurDocumentScanat.ID)).Result;
                //string s = JsonConvert.SerializeObject(CurDocumentScanat, Formatting.None, new Newtonsoft.Json.Converters.IsoDateTimeConverter() { DateTimeFormat = "dd.MM.yyyy" });
                string s = CommonFunctions.GenerateJsonFromModifiedFields(d, CurDocumentScanat);
                r = d.Update(s);
                //return Json(r, JsonRequestBehavior.AllowGet);
                JsonResult result = Json(r, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = Int32.MaxValue;
                return result;
            }
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpPost]
        public JsonResult Avizare(DocumentScanat CurDocumentScanat)
        {
            response r = new response();
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            DocumenteScanateRepository dsr = new DocumenteScanateRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            DocumentScanat d = (DocumentScanat)dsr.Find(Convert.ToInt32(CurDocumentScanat.ID)).Result;
            r = d.Avizare(CurDocumentScanat.VIZA_CASCO);
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        /*
        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpPost]
        public JsonResult PostFile()
        {
            HttpPostedFileBase f = Request.Files[0];
            string initFName = f.FileName;
            string extension = f.FileName.Substring(f.FileName.LastIndexOf('.'));
            string newFName = Guid.NewGuid() + extension;
            Request.Files[0].SaveAs(System.IO.Path.Combine(CommonFunctions.GetScansFolder(), newFName));
            string toReturn = "{\"DENUMIRE_FISIER\":\"" + initFName + "\",\"EXTENSIE_FISIER\":\"" + extension + "\",\"CALE_FISIER\":\"" + newFName + "\"}";
            //return Json(toReturn, JsonRequestBehavior.AllowGet);

            JsonResult result = Json(toReturn, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = Int32.MaxValue;
            return result;
        }
        */

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpPost]
        public JsonResult PostFile(int id_tip_document, int id_dosar)
        {
            HttpPostedFileBase f = Request.Files[0];
            string initFName = f.FileName;
            string extension = f.FileName.Substring(f.FileName.LastIndexOf('.'));
            string newFName = Guid.NewGuid() + extension;
            Request.Files[0].SaveAs(System.IO.Path.Combine(CommonFunctions.GetScansFolder(), newFName));
            //string toReturn = "{\"DENUMIRE_FISIER\":\"" + initFName + "\",\"EXTENSIE_FISIER\":\"" + extension + "\",\"CALE_FISIER\":\"" + newFName + "\"}";
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            DocumentScanat ds = new DocumentScanat(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            ds.ID_DOSAR = id_dosar;
            ds.ID_TIP_DOCUMENT = id_tip_document;
            ds.CALE_FISIER = newFName;
            ds.DENUMIRE_FISIER = initFName;
            ds.EXTENSIE_FISIER = extension;
            response r = ds.Insert();
            JsonResult result = Json(r, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = Int32.MaxValue;
            return result;
        }

        [AuthorizeUser(ActionName = "Dosare", Recursive = false)]
        [HttpGet]
        public JsonResult Delete(int id)
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            DocumenteScanateRepository dsr = new DocumenteScanateRepository(Convert.ToInt32(Session["CURENT_USER_ID"]), conStr);
            DocumentScanat d = (DocumentScanat)dsr.Find(id).Result;
            response r = d.Delete();
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}