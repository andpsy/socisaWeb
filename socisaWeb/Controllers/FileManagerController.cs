using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using socisaWeb.ServiceReference1;
using SOCISA;
using SOCISA.Models;
using System.Configuration;

namespace socisaWeb.Controllers
{
    public class FileManagerController : Controller
    {
        public ActionResult Index()
        {
            return PartialView("FileManagerView");
        }

        [AuthorizeUser(ActionName = "Manager fisiere", Recursive = false)]
        [HttpGet]
        public JsonResult GetOrphanFiles()
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int id = Convert.ToInt32(Session["CURENT_USER_ID"]);
            DocumenteScanateRepository dsr = new DocumenteScanateRepository(id, conStr);
            string[] files = (string[])dsr.GetOrphanFiles().Result;
            List<OrphanFile> ofs = new List<OrphanFile>();
            foreach(string file in files)
            {
                OrphanFile of = new OrphanFile();
                of.FILE_NAME = file; of.SELECTED = true;
                ofs.Add(of);
            }
            return Json(ofs, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Manager fisiere", Recursive = false)]
        [HttpGet]
        public JsonResult GetOrphanDocuments()
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int id = Convert.ToInt32(Session["CURENT_USER_ID"]);
            DocumenteScanateRepository dsr = new DocumenteScanateRepository(id, conStr);
            DocumentScanat[] docs = (DocumentScanat[])dsr.GetOrphanDocuments().Result;
            List<OrphanDocument> ofs = new List<OrphanDocument>();
            foreach(DocumentScanat ds in docs)
            {
                OrphanDocument od = new OrphanDocument();
                od.DOCUMENT_SCANAT = ds; od.SELECTED = true;
                ofs.Add(od);
            }
            return Json(ofs, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Manager fisiere", Recursive = false)]
        [HttpPost]
        public JsonResult DeleteOrphanFile(string fileName)
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int id = Convert.ToInt32(Session["CURENT_USER_ID"]);
            DocumenteScanateRepository dsr = new DocumenteScanateRepository(id, conStr);
            response r = dsr.DeleteOrphanFile(fileName);
            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeUser(ActionName = "Manager fisiere", Recursive = false)]
        [HttpPost]
        public JsonResult RestoreOrphanDocument(int id)
        {
            string conStr = Session["conStr"].ToString(); //ConfigurationManager.ConnectionStrings["MySQLConnectionString"].ConnectionString;
            int _id = Convert.ToInt32(Session["CURENT_USER_ID"]);
            DocumenteScanateRepository dsr = new DocumenteScanateRepository(_id, conStr);
            response r = dsr.RestoreOrphanDocument(id);
            return Json(r, JsonRequestBehavior.AllowGet);
        }

    }
}